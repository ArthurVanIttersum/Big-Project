using System;
using UnityEngine;

public class ScoreLogic : MonoBehaviour
{
    [SerializeField] private PlaytimeValues playtimeValues;
    [SerializeField] private TestMovement ball;
    [HideInInspector] public float score;
    public event Action timeEnded;
    public event Action scoreUpdate;

    private Rigidbody rb;
    private float startMass;
    private Vector3 ballStartScale;
    private float ballScoreMultiplier;
    [HideInInspector] public float adjustedTime;
    [HideInInspector] public float timer;
    private bool winHappen = false;

    private void Start()
    {
        rb = ball.GetComponent<Rigidbody>();

        if (playtimeValues.scoreMultiplier == 0)
            Debug.LogError($"Score multiplier is set to O.");

        ballStartScale = ball.transform.localScale;
        startMass = rb.mass; 

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
            rb.mass = startMass + score * playtimeValues.massScoreRatio;
            ball.transform.localScale = ballStartScale + new Vector3(ballScoreMultiplier, ballScoreMultiplier, ballScoreMultiplier);
            InvokeScoreUpdate();
        }

        if (ball.transform.localScale.x >= playtimeValues.maxBallSize)
            ball.transform.localScale = new Vector3 (playtimeValues.maxBallSize, playtimeValues.maxBallSize, playtimeValues.maxBallSize);

        if (rb.mass >= playtimeValues.maxMass)
            rb.mass = playtimeValues.maxMass;
    }

    public void InvokeScoreUpdate() => scoreUpdate?.Invoke();
}
