using UnityEngine;

public class PlayerTeleportation : MonoBehaviour
{
    private Vector3 startingPos;
    [SerializeField] private float offset;

    private void Awake()
    {
        startingPos = transform.localPosition;
    }

    private void Update()
    {
        float localX = Vector3.Dot(transform.position - startingPos, transform.right);

        if (localX > offset)
            transform.position -= transform.right * (localX + offset);

        if (localX < -offset)
            transform.position -= transform.right * (localX - offset);
    }
}
