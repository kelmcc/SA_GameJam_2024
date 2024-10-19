using Cinemachine;
using System;
using System.Collections.Generic;
using Agents;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class Player : Damagable
{
    [FormerlySerializedAs("Respawn")] public Transform LastStableGroundPosition;
    public float WalkSpeed = 5;
    public float SprintSpeed = 10;
    public float JumpForce = 10;
    public float JumpApplyCutoffTime = 0.5f;
    public float MinimumJumpApplyTime = 0.1f;
    public float AllowJumpOnceUngroundedTime = 0.1f;

    [Space] public float GroundedRaycastDistance = 0.8f;

    [FormerlySerializedAs("FrictionCurve")] public AnimationCurve FrictionInverseCurve;
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

    private bool _grounded = false;
    private Rigidbody _body;
    private bool _jumpStarted;
    private float _jumpTime;
    private bool _hasInput;

    private float _currentSpeed = 0;
    private float _groundedDistance = 0;

    private float _lastGroundedTime = 0;

    [Header("Damagable")]
    [SerializeField] private float _fallDamage = 50;
    [SerializeField] private float _health = 200;

    [Header("Anim")] public SkaterAnimator Anim;
    
    public float VelocityAnimSpeedMultiplier = 0.1f;

    public void Awake()
    {
        _body = GetComponent<Rigidbody>();

        Attack.ToInputAction().performed += (context) => { Debug.Log("ATTACK"); };

        Grapple.ToInputAction().performed += (context) => { Debug.Log("GRAPPLE"); };

        Interact.ToInputAction().performed += (context) =>
        {
           
        };
    }

    public void Update()
    {
        Vector2 movement = Move.ToInputAction().ReadValue<Vector2>();
        movement.Normalize();

        _hasInput = Mathf.Abs(movement.y) > 0 || Mathf.Abs(movement.x) > 0;

        _currentSpeed = WalkSpeed;
        if (Sprint.ToInputAction().IsPressed())
        {
            _currentSpeed = SprintSpeed;
        }

        Vector3 forward = (transform.position - VCam.transform.position).normalized;
        Vector3 right = Quaternion.AngleAxis(90, Vector3.up) * forward;

        Debug.DrawLine(transform.position + Vector3.up * 0.5f, transform.position + -transform.up * GroundedRaycastDistance, Color.magenta);
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, -transform.up, out RaycastHit info, 100f))
        {
            if (info.distance < GroundedRaycastDistance)
            {
                _grounded = true;
                _groundedDistance = info.distance;
                _lastGroundedTime = Time.time;

                right = Vector3.Cross(info.normal, forward);
                forward = Quaternion.AngleAxis(-90, Vector3.up) * right;
                Debug.DrawLine(transform.position, transform.position + Vector3.up * 3, Color.green);

                LastStableGroundPosition.position = transform.position;
            }
        }
        else
        {
            Debug.Log("<color=cyan>Player Ungrounded</color>");
            _grounded = false;
            Debug.DrawLine(transform.position, transform.position + Vector3.up * 5, Color.cyan);
        }
        
        if(!_grounded)
        {
            Vector3 flatForward = new Vector3(forward.x, 0, forward.z).normalized;
           forward = flatForward;
        }
        
        right.Normalize();
        forward.Normalize();

        //Debug.
        Vector3 u = Vector3.up * 0.1f;
        Debug.DrawLine(transform.position + u, transform.position + u + forward * 5, Color.blue);
        Debug.DrawLine(transform.position + u, transform.position + u + right * 5, Color.red);
        //

        if (_grounded)
        {
            _body.AddForce(((forward * movement.y) + (right * movement.x)) * Time.deltaTime * _currentSpeed, ForceMode.VelocityChange);
        }
        
        float mag = _body.velocity.magnitude;
        float maxSpeedNorm =  mag  / MaxGroundedVelocity;
        Debug.Log($"{mag}/{MaxGroundedVelocity} = {maxSpeedNorm}");

        bool jumpingEvenIfStillGrounded = _jumpStarted && _jumpTime < MinimumJumpApplyTime;
        if (_grounded && !jumpingEvenIfStillGrounded)
        {
            mag  *= FrictionInverseCurve.Evaluate(Mathf.Clamp01(maxSpeedNorm));
            mag  = Mathf.Min(MaxGroundedVelocity, mag);
        }

        Vector3 vNorm = _body.velocity.normalized;

        if (_grounded)
        {
            //rotate v to facing dir
            Sliding = Mathf.Lerp(0, 1, maxSpeedNorm);
            _body.velocity = Vector3.Lerp(forward * mag, vNorm * mag, Sliding);

            if (!jumpingEvenIfStillGrounded)
            {
                if (_hasInput)
                {
                    Anim.PlaySkate();
                }
                else
                {
                    Anim.PlayFreewheel();
                }
            }
          
            Anim.Speed = mag * VelocityAnimSpeedMultiplier;
        }
        else
        {
            Anim.Speed = 1;
            
            //cant rotate to facing dir while falling.
            _body.velocity = vNorm * mag;
        }
        
        Vector3 vDir = new Vector3(vNorm.x, 0, vNorm.z).normalized;

        if (mag > 0.01)
        {
            _body.MoveRotation(Quaternion.LookRotation(vDir, Vector3.up));
        }

        CalculateJump();

        void CalculateJump()
        {
            if (Jump.ToInputAction().IsPressed())
            {
                if (!_jumpStarted)
                {
                    if (_grounded || Time.time - _lastGroundedTime < AllowJumpOnceUngroundedTime)
                    {
                        Anim.PlayJump();
                        _jumpStarted = true;
                        _jumpTime = 0;
                    }
                }
                else
                {
                    DoJump();
                }
            }
            else if (_jumpStarted && _jumpTime < MinimumJumpApplyTime)
            {
                DoJump();
            }
            else
            {
                _jumpStarted = false;
                _jumpTime = 0;
            }

            void DoJump()
            {
                _jumpTime += Time.deltaTime;
                if (_jumpTime < JumpApplyCutoffTime)
                {
                    _body.AddForce(Vector3.up * JumpForce * Time.deltaTime, ForceMode.VelocityChange);
                }
            }
        }
    }

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
        _health -= damage;

        if (_health <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        SceneManager.LoadScene(1);
    }
}