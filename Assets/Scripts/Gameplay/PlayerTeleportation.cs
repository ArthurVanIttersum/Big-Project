using UnityEngine;

public class PlayerTeleportation : MonoBehaviour
{
    private Vector3 startingPos;
    private Vector3 currentPos;
    [SerializeField] private float offset;

    private void Awake()
    {
        startingPos = transform.position.normalized;
    }

    private void Update()
    {
        currentPos = transform.position;

        if (currentPos.x > startingPos.x + offset)
            gameObject.transform.position = new Vector3(startingPos.x - offset, currentPos.y, currentPos.z).normalized;

        if (currentPos.x < startingPos.x - offset)
            gameObject.transform.position = new Vector3(startingPos.x + offset, currentPos.y, currentPos.z).normalized;
    }
}
