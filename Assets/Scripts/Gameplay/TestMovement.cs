using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]

public class TestMovement : MonoBehaviour
{
    [SerializeField] private float force = 10f; //used for forward and side movement
    [SerializeField][Range(0, 1f)] private float lateralBias = 0.3f; //how much of the force is dedicated to side movement (0 - none; 1 - all)
    [SerializeField] private float maxSpeed = 15f;
    [SerializeField] private float deceleraionRate = 2f; //how much speed per second is lost
    public float speed; //temp variable for testing

    private bool player1Pressed;
    private bool player2Pressed;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = 0f;
    }

    private void OnPlayer1(InputValue value)
    {
        if (value.isPressed)
            player1Pressed = true;
    }

    private void OnPlayer2(InputValue value)
    {
        if (value.isPressed)
            player2Pressed = true;
    }

    private void FixedUpdate()
    {
        if (player1Pressed)
        {
            ApplyForce(true);
            player1Pressed = false;
        }

        if (player2Pressed)
        {
            ApplyForce(false);
            player2Pressed = false;
        }

        ApplyDrag();
        ClampSpeed();
    }

    private void ApplyForce(bool leftBias)
    {
        float lateral = leftBias ? -lateralBias : lateralBias;
        Vector3 localDir = new Vector3(lateral, 0f, 1f).normalized;
        Vector3 worldDir = transform.TransformDirection(localDir);

        rb.AddForce(worldDir * force, ForceMode.Impulse);
    }

    private void ApplyDrag()
    {
        speed = rb.linearVelocity.magnitude;
        if (speed <= 0f) return;

        float speedDrop = deceleraionRate * Time.fixedDeltaTime;
        float newSpeed = Mathf.Max(0f, speed - speedDrop);
        rb.linearVelocity = rb.linearVelocity.normalized * newSpeed;
    }

    private void ClampSpeed()
    {
        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }
}
