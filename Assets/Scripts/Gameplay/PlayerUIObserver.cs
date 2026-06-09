using TMPro;
using UnityEngine;

public class PlayerUIObserver : PlayerObserver
{
    [SerializeField] TextMeshProUGUI score;

    protected override void OnScoreUpdate()
    {
        if (scoreLogic.score <= 0)
            scoreLogic.score = 0;

        score.text = $"Score: {(int)scoreLogic.score}";
    }

    protected override void OnTimeEnd()
    {}
}
