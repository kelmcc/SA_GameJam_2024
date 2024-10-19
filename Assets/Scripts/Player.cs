using Cinemachine;
using System;
using System.Collections.Generic;
using Agents;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Damagable
{
    public float WalkSpeed = 5;
    public float SprintSpeed = 10;
    public float JumpForce = 10;
    public float JumpApplyCutoffTime = 0.5f;
    public float MinimumJumpApplyTime = 0.1f;
    public float AllowJumpOnceUngroundedTime = 0.1f;
    public float RotateToFacingSpeed = 5;

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
    private bool _isMoving;

    private float _currentSpeed = 0;
    private float _groundedDistance = 0;

    private float _lastGroundedTime = 0;

    [SerializeField] private float _health = 200;

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

        _isMoving = Mathf.Abs(movement.y) > 0 || Mathf.Abs(movement.x) > 0;

        _currentSpeed = WalkSpeed;
        if (Sprint.ToInputAction().IsPressed())
        {
            _currentSpeed = SprintSpeed;
        }

        Vector3 forward = (transform.position - VCam.transform.position).normalized;
        Vector3 right = Quaternion.AngleAxis(90, Vector3.up) * forward;

        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, -transform.up, out RaycastHit info, 1f))
        {
            _grounded = true;
            _groundedDistance = info.distance;
            _lastGroundedTime = Time.time;

            right = Vector3.Cross(info.normal, forward);
            forward = Quaternion.AngleAxis(-90, Vector3.up) * right;
        }
        else
        {
            Debug.Log("<color=cyan>Player Ungrounded</color>");
            _grounded = false;
        }

        Vector3 flatForward = new Vector3(forward.x, 0, forward.z).normalized;

        if (_grounded)
        {
            //Vector3 v = _body.velocity * Mathf.Clamp01(Time.deltaTime * 20);
            //_body.velocity = new Vector3(v.x, _body.velocity.y, v.z);
        }
        else
        {
            forward = flatForward;
        }

        _body.MovePosition(_body.position +
                           ((forward * movement.y) + (right * movement.x)) * Time.deltaTime * _currentSpeed);

        if (_isMoving)
        {
            Vector3 current =
                Vector3.Slerp(transform.forward, flatForward, Time.deltaTime * RotateToFacingSpeed); //bad but yolo
            _body.MoveRotation(Quaternion.LookRotation(current, Vector3.up));
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
                        _body.velocity = new Vector3(_body.velocity.x, _body.velocity.y, _body.velocity.z);
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

    public void OnDeath()
    {
        // what happens when the player dies?

        Debug.LogError("Player has died");
    }

    protected override void TakeDamage(float damage)
    {
        _health -= damage;

        if (_health <= 0)
        {
            OnDeath();
        }
    }
}