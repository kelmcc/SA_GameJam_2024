using Cinemachine;
using System;
using System.Collections.Generic;
using Agents;
using Framework;
using SoundManager;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.VFX;

public class Player : Damagable
{
    public EffectSoundBank GrindSFX;
    private EffectSoundInstance _grindInstance;
    public EffectSoundBank JumpSFX;
    public EffectSoundBank SpiderDieSFX;
    [Space]
    [FormerlySerializedAs("Respawn")] public Transform LastStableGroundPosition;
    public float WalkSpeed = 5;
    public float SprintSpeed = 10;
    [FormerlySerializedAs("JumpForce")] public float JumpForceAtSpeed = 10;
    public float JumpForceAtRest = 10;
    public float ZipJumpOffForce = 10;
    public float AllowJumpOnceUngroundedTime = 0.1f;
    public float FlailDelay = 0.5f;

    public float ZipSyncSpeed = 10;
    public float ZipAcceleration;
    public float MaxZipVelocity;

    public int WinningCoinCount = 10000;

    [Space]
    public float FrictionCoeff = 0.1f;

    [FormerlySerializedAs("zoopForce")] public Vector3 _zoopForce;
    
    public LayerMask GroundRaycastMask;

    [Space] public float GroundedRaycastDistance = 0.8f;
    
    public float MaxGroundedVelocity = 20;

    [Range(0, 1)]
    public float Sliding = 0;

    [Space] public CinemachineFreeLook VCam;

    [Space] public InputActionReference Move;
    public InputActionReference Jump;
    public InputActionReference Attack;
    public InputActionReference Sprint;
    public InputActionReference Grapple;
    public InputActionReference Interact;

    [FormerlySerializedAs("CoinSpawn")] public CoinUISpawner coinUISpawner;

    private bool _grounded = false;
    public bool Grounded => _grounded && !InJumpWindow();
    public bool IsZipping { get; private set; }

    private Rigidbody _body;
    private bool _jumpStarted;
    private bool _hasInput;

    private float _currentSpeed = 0;
    private float _groundedDistance = 0;

    private float _lastGroundedTime = 0;

    [Header("Damagable")]
    [SerializeField] private float _fallDamage = 50;
    [FormerlySerializedAs("_health")] [SerializeField] private float _coinz = 50;
    public int Coinz => (int)_coinz;

    [Header("Anim")] public SkaterAnimator Anim;
    public SkaterAnimator AnimUI;

    public Volume ZipVFXVolume;
    public AnimationCurve ZipLineVFXAnimationCurve;
    public VisualEffect ZipLineSparks1;
    public VisualEffect ZipLineSparks2;

    public float VelocityAnimSpeedMultiplier = 0.1f;

    public static Player Instance;

    public Animator _startingAnim;

    private UnityEngine.Pool.ObjectPool<ParticleSystem> _pool;
    
    public void Awake()
    {
        _body = GetComponent<Rigidbody>();
        Instance = this;
        
        _pool = new UnityEngine.Pool.ObjectPool<ParticleSystem>(() => Instantiate(EnemyDieParticles, transform, false),
            g => { g.gameObject.SetActive(true); },
            gr => gr.gameObject.SetActive(false), (o)=>Destroy(o.gameObject));

        Attack.ToInputAction().performed += (context) => { Debug.Log("ATTACK"); };

        Grapple.ToInputAction().performed += (context) => { Debug.Log("GRAPPLE"); };

        Interact.ToInputAction().performed += (context) =>
        {
            
        };
    }

    private IEnumerator Start()
    {
        int i = 0;
        int startingC = (int)_coinz;
        LastStableGroundPosition.position = transform.position;
        yield return new WaitForSeconds(5);

        if (_startingAnim != null)
        {
            _startingAnim.SetTrigger("Start");
        }
        
        while (i < startingC)
        {
            yield return new WaitForSeconds(0.1f);
            i++;
          //  coinUISpawner.Spawn();
        }
    }
    
    public float JumpWindowTime = 0.5f;
    private float LastJumpTime;
    private bool _flailing;
    private float _flailTime;
    private bool _zoopPending;
    private float _lastZoopTime;
    private float _zipPosition;
    private float _zipVelocity;

    public bool InJumpWindow()
    {
        return Time.time - LastJumpTime < JumpWindowTime;
    }

    protected new void OnTriggerEnter(Collider collider)
    {
        Pickup pickup = collider.GetComponent<Pickup>();
        if (pickup != null)
        {
            _coinz += 1;
            coinUISpawner.Spawn();
            pickup.PickedUp();
            return;
        }
        
        base.OnTriggerEnter(collider);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            _coinz = WinningCoinCount;
        }
        
        if (_coinz >= WinningCoinCount)
        {
            if (reloading)
            {
                return;
            }
            reloading = true;
            SceneManager.LoadScene(3);
        }
        
        if (_coinz <= 0)
        {
            Death();
        }
        if (IsZipping)
        {
            UpdateZip();

            return;
        }


        Vector2 movement = Move.ToInputAction().ReadValue<Vector2>();
        movement.Normalize();
        
        
        if (movement.y < 0)
        {
            _currentBreakTime += Time.time;
            movement.y *= 3;

            // _body.velocity = Vector3.Lerp(_body.velocity, Vector3.zero, Mathf.Clamp01(_currentBreakTime / BreakTime));
        }
        else
        {
            _currentBreakTime = 0;
        }

        _hasInput = Mathf.Abs(movement.y) > 0 || Mathf.Abs(movement.x) > 0;

        _currentSpeed = WalkSpeed;
        if (Sprint.ToInputAction().IsPressed())
        {
            _currentSpeed = SprintSpeed;
        }

        Vector3 forward = (transform.position - VCam.transform.position).normalized;
        Vector3 right = Quaternion.AngleAxis(90, Vector3.up) * forward;

        Debug.DrawLine(transform.position + Vector3.up * 0.5f, transform.position + Vector3.up * 0.5f + -transform.up * GroundedRaycastDistance, Color.magenta);
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, -transform.up, out RaycastHit info, 100f, GroundRaycastMask))
        {
            if (info.distance < GroundedRaycastDistance)
            {
//                Debug.Log("<color=green>Player Grounded</color>");
                if (_grounded == false)
                {
                    JumpSFX.Play();
                }
                _grounded = true;
               
                _groundedDistance = info.distance;
                _lastGroundedTime = Time.time;

                Debug.DrawLine(info.point, info.point + info.normal*10, Color.green);

                Vector3 norm = info.normal.normalized;
                right = Vector3.Cross(norm, forward);
                forward = Quaternion.AngleAxis(-90, norm) * right;
                
                Debug.DrawLine(transform.position, transform.position +right * 10, Color.red);
                Debug.DrawLine(transform.position, transform.position +forward * 10, Color.blue);
            }
            else
            {
                Unground();
            }
        }
        else
        {
            Unground();
        }

        void Unground()
        {
            Debug.Log("<color=cyan>Player Ungrounded</color>");
            _grounded = false;
            Debug.DrawLine(transform.position, transform.position + Vector3.up * 3, Color.cyan);
        }

        if (!Grounded)
        {
            Vector3 flatForward = new Vector3(forward.x, 0, forward.z).normalized;
            forward = flatForward;
        }

        right.Normalize();
        forward.Normalize();

        //Debug.
        Vector3 u = Vector3.up * 0.1f;
        //Debug.DrawLine(transform.position + u, transform.position + u + forward * 10, Color.blue);
        //Debug.DrawLine(transform.position + u, transform.position + u + right * 5, Color.red);
        //

        // if (Grounded)
        {
            _body.AddForce(((forward * movement.y) + (right * movement.x)) * Time.deltaTime * _currentSpeed, ForceMode.VelocityChange);
        }

        float mag = _body.velocity.magnitude;
        float maxSpeedNorm = mag / MaxGroundedVelocity;
        bool inJumpWindow = InJumpWindow();
        bool isZooping = Time.time - _lastZoopTime < 2;
        //if (Grounded || _flailing && !isZooping)
        {
            //mag  *= FrictionInverseCurve.Evaluate(Mathf.Clamp01(maxSpeedNorm));
            mag *= 1 - (FrictionCoeff * Time.deltaTime);
            mag = Mathf.Min(MaxGroundedVelocity, mag);
        }

        Vector3 vNorm = _body.velocity.normalized;
        
        if (Grounded)
        {
            //rotate v to facing dir
            //   Sliding = Mathf.Lerp(1, 0, avSpeedNorm);
            _body.velocity = Vector3.Lerp(forward * mag, vNorm * mag, Sliding);
            
            if (_hasInput)
            {
                if (movement.y < 0)
                {
                    Anim.PlayBreak();
                    AnimUI.PlayBreak();
                }
                else
                {
                    Anim.PlaySkate();
                    AnimUI.PlaySkate();
                }
            }
            else
            {
                if (mag < 0.1f)
                {
                    Anim.PlayIdle();
                    AnimUI.PlayIdle();
                }
                else
                {
                    Anim.PlayFreewheel();
                    AnimUI.PlayFreewheel();
                }
            }

            float speed = mag * VelocityAnimSpeedMultiplier;
            Anim.Speed = speed;
            AnimUI.Speed = speed;
        }
        else
        {
            Anim.Speed = 1;
            AnimUI.Speed = 1;

            _body.velocity = Vector3.Lerp(forward * mag, vNorm * mag, Sliding).WithY(_body.velocity.y);
            //cant rotate to facing dir while falling.
           // _body.velocity = vNorm * mag;
        }


      
        if (Mathf.Abs(movement.x) > 0)
        {
            Vector3 dir = right * movement.x;
            if (Grounded)
            {
                _body.velocity = Vector3.Slerp(_body.velocity, dir * _body.velocity.magnitude, Time.deltaTime * 5);
            }
            else
            {
                _body.velocity = Vector3.Slerp(_body.velocity, dir * _body.velocity.magnitude, Time.deltaTime * 5).WithY(_body.velocity.y);
            }
        }


        Vector3 vDir = new Vector3(vNorm.x, 0, vNorm.z).normalized;

        if (mag > 0.1)
        {
            _body.MoveRotation(Quaternion.LookRotation(vDir, Vector3.up));
        }

        _body.AddForce(_zoopForce);
        _zoopForce = Vector3.zero;

        float jump = Mathf.Lerp(JumpForceAtRest, JumpForceAtSpeed, maxSpeedNorm);

        CalculateJump();

        void CalculateJump()
        {
            if (Jump.ToInputAction().IsPressed())
            {
                _flailing = false;
                if (Grounded && !_jumpStarted)// || Time.time - _lastGroundedTime < AllowJumpOnceUngroundedTime)
                {
                    _jumpStarted = true;
                    Debug.Log("JUMP");
                    Anim.PlayJump();
                    AnimUI.PlayJump();
                    LastJumpTime = Time.time;
                    _body.AddForce(Vector3.up * jump, ForceMode.VelocityChange);
                }
            }
            else
            {
                if (!Grounded)
                {
                    _flailing = true;

                    _flailTime += Time.deltaTime;
                    if (_flailTime > FlailDelay)
                    {
                        Anim.PlayFall();
                        AnimUI.PlayFall();
                    }

                }
                else
                {
                    _flailTime = 0;
                    _flailing = false;
                }
                _jumpStarted = false;
            }
        }
    }

    public float BreakTime = 1.5f;

    public void OnFall()
    {
        // what happens when the player dies?

        TakeDamage(_fallDamage);

        _body.velocity = Vector3.zero;
        transform.position = LastStableGroundPosition.position;

        Debug.LogError("Player has fallen");
    }

    protected override void TakeDamage(float damage)
    {
        _coinz -= damage;

        if (_coinz <= 0)
        {
            Death();
        }
    }

    private bool reloading = false;
    public void Death()
    {
        if (reloading)
        {
            return;
        }
        reloading = true;
        SceneManager.LoadScene(4);
    }

    //ZOOP

    public void Zoop(Vector3 zoopForce)
    {
        _lastZoopTime = Time.time;
        _zoopForce += zoopForce;
    }


    //ZIP

    private ZiplinePole _startZip;
    private ZiplinePole _endZip;
    private Zipline _zipline;
    private float _startZipTime;
    private float _currentBreakTime;

    private void UpdateZip()
    {
        ZipVFXVolume.weight = Mathf.Clamp01(ZipLineVFXAnimationCurve.Evaluate(Time.time - _startZipTime));
        _body.velocity = Vector3.zero;
        _body.isKinematic = true;
        _zipVelocity += ZipAcceleration * Time.deltaTime;
        _zipVelocity = Mathf.Min(MaxZipVelocity, _zipVelocity);
        _zipPosition += _zipVelocity * Time.deltaTime;
        Vector3 position = _zipline.GetPosition(_startZip, _zipPosition);
        _body.MovePosition(Vector3.MoveTowards(_body.position, position, (Mathf.Max(_zipVelocity, ZipSyncSpeed) * Time.deltaTime)));

        if (_zipPosition >= _zipline.Length)//|| Jump.ToInputAction().IsPressed())
        {
            _grindInstance?.StopAndDestroy();
            _body.isKinematic = false;
            _body.velocity = _zipline.GetTangent(_startZip, _zipPosition) * _zipVelocity;
            _body.AddForce(Vector3.up * ZipJumpOffForce, ForceMode.VelocityChange);

            IsZipping = false;

            _zipPosition = 0;
            _zipVelocity = 0;
            _startZip = null;
            _endZip = null;
            _zipline = null;
            ZipVFXVolume.weight = 0;
            ZipLineSparks1.SendEvent(Shader.PropertyToID("StopSpark"));
            ZipLineSparks2.SendEvent(Shader.PropertyToID("StopSpark"));
        }
    }

    public void Zip(ZiplinePole point1, ZiplinePole point2, Zipline zipline)
    {
        Debug.Log("ZIPPING");
        ZipLineSparks1.SendEvent(Shader.PropertyToID("Spark"));
        ZipLineSparks2.SendEvent(Shader.PropertyToID("Spark"));
        
       _grindInstance = GrindSFX.Play();
        Anim.PlayGrind();
        AnimUI.PlayGrind();
        IsZipping = true;
        _startZipTime = Time.time;

        _zipPosition = 0;
        _zipVelocity = _body.velocity.magnitude;
        _body.velocity = Vector3.zero;
        _startZip = point1;
        _endZip = point2;
        _zipline = zipline;
    }

    public ParticleSystem EnemyDieParticles;

    public void PlayEnemyDie(Vector3 transformPosition)
    {
        ParticleSystem instance = _pool.Get();
        instance.transform.position = transformPosition;
        instance.Play();
        SpiderDieSFX.Play(transformPosition);
        StartCoroutine(Wait());
        IEnumerator Wait()
        {
            yield return new WaitForSeconds(0.75f);
            _pool.Release(instance);
        }
    }

    public void Buy(int cost)
    {
        _coinz -= cost;
    }
}