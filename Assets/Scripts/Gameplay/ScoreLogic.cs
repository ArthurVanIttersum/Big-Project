using System;
using UnityEngine;

public class ScoreLogic : MonoBehaviour
{
    [SerializeField] private PlaytimeValues playtimeValues;
    [SerializeField] private GameObject ball;
    [SerializeField] private float ballScoreRatio;
    [SerializeField] private float maxBallSize; 
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
            ballScoreMultiplier = score * ballScoreRatio;
            ball.transform.localScale = ballStartScale + new Vector3(ballScoreMultiplier, ballScoreMultiplier, ballScoreMultiplier);
            InvokeScoreUpdate();
        }

        if (ball.transform.localScale.x >= maxBallSize)
            ball.transform.localScale = new Vector3 (maxBallSize, maxBallSize, maxBallSize);
    }

    public void InvokeScoreUpdate() => scoreUpdate?.Invoke();
}
