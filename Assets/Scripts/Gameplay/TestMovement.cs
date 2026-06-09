using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]

public class TestMovement : MonoBehaviour
{
    [SerializeField] private float force = 10f; //used for forward and side movement
    [SerializeField] private float maxSpeed = 15f;
    [SerializeField] private float deceleraionRate = 2f; //how much speed per second is lost
    [SerializeField] private float degreesPerClick = 1f;
    [SerializeField] private float maxDegree = 1f; //how much to the left or right the object can rotate
    [SerializeField][Range(0, 1f)] private float thresholdPercentage = 0.3f; //at what percentage of maxDegree, degreesPerSec starts to drop rapedly
    [SerializeField] private float exponentialStrength = 5f; //Controls steepness of the drop-off curve after the threshold (higher = steeper)
    public float speed; //temp variable for testing

    private bool player1Pressed;
    private bool player2Pressed;
    private Rigidbody rb;
    private float startYAngle;
    private float currentYAngle;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = 0f;

        startYAngle = transform.eulerAngles.y;
        currentYAngle = startYAngle;
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
            ApplyForce(-1);
            player1Pressed = false;
        }

        if (player2Pressed)
        {
            ApplyForce(1);
            player2Pressed = false;
        }

        ApplyDrag();
        ClampSpeed();
    }

    private void ApplyForce(int direction)
    {
        float scaleDegrees = CalculateRotation(direction);
        
        currentYAngle += scaleDegrees;
        currentYAngle = Mathf.Clamp(currentYAngle, startYAngle - maxDegree, startYAngle + maxDegree);

        Vector3 euler = transform.eulerAngles;
        euler.y = currentYAngle;
        transform.eulerAngles = euler;

        rb.AddForce(transform.forward * force, ForceMode.Impulse);
    }

    private float CalculateRotation(int direction)
    {
        float offset = currentYAngle - startYAngle;
        float distanceInDirection = direction * offset;

        float threshold = thresholdPercentage * maxDegree;

        float multiplier;

        if (distanceInDirection <= threshold)
            multiplier = 1;

        else
        {
            float thresholdCal = (distanceInDirection - threshold) / (maxDegree - threshold);

            thresholdCal = Mathf.Clamp01(thresholdCal);

            float maxVal = 1f;
            float minVal = Mathf.Exp(-exponentialStrength);
            float raw = Mathf.Exp(-exponentialStrength * thresholdCal);
            multiplier = (raw - minVal) / (maxVal - minVal);
            multiplier = Mathf.Clamp01(multiplier);
        }

        return direction * degreesPerClick * multiplier;
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
