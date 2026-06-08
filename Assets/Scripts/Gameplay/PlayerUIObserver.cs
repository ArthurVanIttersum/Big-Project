using TMPro;
using UnityEngine;

public class PlayerUIObserver : PlayerObserver
{
    [SerializeField] TextMeshProUGUI health;
    [SerializeField] TextMeshProUGUI score;

    protected override void OnAddHealth(int value)
    {
        health.text = $"Health: {detectCollision.playerData.health}";
    }

    protected override void OnDoDamage(int value)
    {
        health.text = $"Health: {detectCollision.playerData.health}";
    }

    protected override void OnScoreUpdate()
    {
        score.text = $"Score: {(int)scoreLogic.score}";
    }

    protected override void OnTimeEnd()
    {}
}
