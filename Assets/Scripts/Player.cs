using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float WalkSpeed = 5;
    public float SprintSpeed = 10;
    
    [Space]
    public InputActionReference Move;
    public InputActionReference Jump;
    public InputActionReference Attack;
    public InputActionReference Sprint;
    public InputActionReference Grapple;
    public InputActionReference Interact;

    public void Awake()
    {
        Jump.ToInputAction().performed += (context) =>
        {
            Debug.Log("JUMP");
        };
        
        Attack.ToInputAction().performed += (context) =>
        {
            Debug.Log("ATTACK");
        };
        
        Sprint.ToInputAction().performed += (context) =>
        {
            Debug.Log("SPRINT");
        };
        
        Grapple.ToInputAction().performed += (context) =>
        {
            Debug.Log("GRAPPLE");
        };
        
        Interact.ToInputAction().performed += (context) =>
        {
            Debug.Log("INTERACT");
        };
    }

    public void Update()
    {
        Vector2 movement = Move.ToInputAction().ReadValue<Vector2>();
        transform.Translate(new Vector3(movement.x, 0, movement.y) * Time.deltaTime * WalkSpeed);
    }
}
