using UnityEngine;

public class SunRotation : MonoBehaviour
{
    [SerializeField] private ScoreLogic scoreLogic;

    private const float startAngle = 90f;
    private const float endAngle = -90f;

    private void Start()
    {
        transform.rotation = Quaternion.AngleAxis(startAngle, Vector3.forward);
    }

    private void Update()
    {
        if (scoreLogic.adjustedTime <= 0f) return;

        float progress = Mathf.Clamp01(scoreLogic.timer / scoreLogic.adjustedTime);
        float currentAngle = Mathf.Lerp(startAngle, endAngle, progress);

        transform.rotation = Quaternion.AngleAxis(currentAngle, Vector3.forward);
    }
}
