using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    float movementForce = 1;
    [SerializeField]
    float maxSpeed = 5;
    [SerializeField]
    float jumpForce = 5;
    private InputActionAsset inputAsset;
    private InputActionMap actionMap;
    private InputAction movementAction;
    private Rigidbody2D rb;

    private void Awake()
    {
        inputAsset = GetComponent<PlayerInput>().actions;
        rb = GetComponent<Rigidbody2D>();
        actionMap = inputAsset.FindActionMap("Player");
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector2 forceDirection = Vector2.zero;

        forceDirection.x += movementAction.ReadValue<Vector2>().x * movementForce;

        rb.AddForce(forceDirection, ForceMode2D.Impulse);
        forceDirection = Vector3.zero;

        if (rb.velocity.y < 0f)
            rb.velocity -= Vector2.down * Physics.gravity.y * Time.fixedDeltaTime;

        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0;
        if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
            rb.velocity = horizontalVelocity.normalized * maxSpeed + Vector3.up * rb.velocity.y;
    }

    private void Jump(InputAction.CallbackContext obj)
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
    }

    private void OnEnable()
    {
        movementAction = actionMap.FindAction("Movement");
        actionMap.FindAction("Jump").started += Jump;
    }
}
