using UnityEngine;

public class PlayerTeleportation : MonoBehaviour
{
    private Vector3 startingPos;
    private Rigidbody rb;
    [SerializeField] private float offset;

    private void Awake()
    {
        startingPos = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float localX = Vector3.Dot(rb.position - startingPos, transform.right);

        if (localX > offset)
            rb.Move(rb.position - transform.right * (localX + offset), Quaternion.identity);

        else if (localX < -offset)
            rb.Move(rb.position - transform.right * (localX - offset), Quaternion.identity);
    }
}
