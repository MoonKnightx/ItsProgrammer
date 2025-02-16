using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using static UnityEngine.InputSystem.InputAction;

public class Controller : MonoBehaviour
{
    [SerializeField]private bool isOnGround = true;
    public LayerMask groundLayer;
    [SerializeField] private float moveSpeed = 15f, jumpForce = 5f;

    [SerializeField]private Rigidbody rb;
    Vector3 moveVector = Vector3.zero;

    [SerializeField]private InputSystem_Actions playerInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += moveVector * Time.deltaTime;
        
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 direction = context.ReadValue<Vector2>();
        moveVector = new Vector3(direction.x,0,direction.y) * moveSpeed;
    }

    public void OnJump(InputAction.CallbackContext context) { 
        JumpUpdate();
    }

    private void JumpUpdate()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        if (!isOnGround)
        {
            Vector3 tvelocity = rb.linearVelocity;
            tvelocity.y /= 2f;
            rb.linearVelocity = tvelocity;
        }
    }

    
    private void FixedUpdate()
    {
        isOnGround = Physics.Raycast(transform.position, Vector3.down, 100f, groundLayer);

    }


}
