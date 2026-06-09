using System;
using UnityEngine;

public class ScoreLogic : MonoBehaviour
{
    [SerializeField] private PlaytimeValues playtimeValues;
    public float score;
    public event Action timeEnded;
    public event Action scoreUpdate;

    private float adjustedTime;
    private float timer;
    private bool winHappen = false;

    private void Start()
    {
        if (playtimeValues.scoreMultiplier == 0)
            Debug.LogError($"Score multiplier is set to O.");

        adjustedTime = playtimeValues.playTime * 60;
        InvokeScoreUpdate();
    }

    private void Update()
    {
        if (timer >= adjustedTime && !winHappen)
        {
            timeEnded?.Invoke();
            winHappen = true;
        }
            
        if (!winHappen)
        {
            timer += Time.deltaTime;
            score += Time.deltaTime * playtimeValues.scoreMultiplier;
            InvokeScoreUpdate();
        }
    }

    public void InvokeScoreUpdate() => scoreUpdate?.Invoke();
}
