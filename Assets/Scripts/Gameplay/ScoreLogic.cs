using System;
using UnityEngine;

public class ScoreLogic : MonoBehaviour
{
    [SerializeField] private PlaytimeValues playtimeValues;
    [SerializeField] private GameObject ball;
    public float score;
    public event Action timeEnded;
    public event Action scoreUpdate;

    private Vector3 ballStartScale;
    private float ballScoreMultiplier;
    private float adjustedTime;
    private float timer;
    private bool winHappen = false;

    private void Start()
    {
        if (playtimeValues.scoreMultiplier == 0)
            Debug.LogError($"Score multiplier is set to O.");

        ballStartScale = ball.transform.localScale;

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
            ballScoreMultiplier = score * playtimeValues.ballScoreRatio;
            ball.transform.localScale = ballStartScale + new Vector3(ballScoreMultiplier, ballScoreMultiplier, ballScoreMultiplier);
            InvokeScoreUpdate();
        }

        if (ball.transform.localScale.x >= playtimeValues.maxBallSize)
            ball.transform.localScale = new Vector3 (playtimeValues.maxBallSize, playtimeValues.maxBallSize, playtimeValues.maxBallSize);
    }

    public void InvokeScoreUpdate() => scoreUpdate?.Invoke();
}
