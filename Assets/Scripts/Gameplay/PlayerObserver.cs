using UnityEngine;

public abstract class PlayerObserver : MonoBehaviour
{
    [SerializeField] protected ScoreLogic scoreLogic;

    protected void OnEnable()
    {
        scoreLogic.timeEnded += OnTimeEnd;
        scoreLogic.scoreUpdate += OnScoreUpdate;
    }

    protected void OnDisable()
    {
        scoreLogic.timeEnded -= OnTimeEnd;
        scoreLogic.scoreUpdate -= OnScoreUpdate;
    }

    protected abstract void OnTimeEnd();

    protected abstract void OnScoreUpdate();
}
