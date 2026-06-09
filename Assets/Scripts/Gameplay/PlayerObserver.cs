using UnityEngine;

public abstract class PlayerObserver : MonoBehaviour
{
    [SerializeField] protected DetectCollision detectCollision;
    [SerializeField] protected ScoreLogic scoreLogic;

    protected void OnEnable()
    {
        detectCollision.addHealth += OnAddHealth;
        detectCollision.doDamage += OnDoDamage;
        scoreLogic.timeEnded += OnTimeEnd;
        scoreLogic.scoreUpdate += OnScoreUpdate;
    }

    protected void OnDisable()
    {
        detectCollision.addHealth -= OnAddHealth;
        detectCollision.doDamage -= OnDoDamage;
        scoreLogic.timeEnded -= OnTimeEnd;
        scoreLogic.scoreUpdate -= OnScoreUpdate;
    }

    protected abstract void OnAddHealth(int value);

    protected abstract void OnDoDamage(int value);

    protected abstract void OnTimeEnd();

    protected abstract void OnScoreUpdate();
}
