using TMPro;
using UnityEngine;

public class PlayerUIObserver : PlayerObserver
{
    [SerializeField] TextMeshProUGUI health;

    protected override void OnAddHealth(int value)
    {
        health.text = $"Health: {detectCollision.playerData.health}";
    }

    protected override void OnDoDamage(int value)
    {
        health.text = $"Health: {detectCollision.playerData.health}";
    }
}
