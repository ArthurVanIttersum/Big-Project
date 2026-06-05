using UnityEngine;

public abstract class PlayerObserver : MonoBehaviour
{
    [SerializeField] protected DetectCollision detectCollision;

    protected void OnEnable()
    {
        detectCollision.addHealth += OnAddHealth;
        detectCollision.doDamage += OnDoDamage;
    }

    protected void OnDisable()
    {
        detectCollision.addHealth -= OnAddHealth;
        detectCollision.doDamage -= OnDoDamage;
    }

    protected abstract void OnAddHealth(int value);

    protected abstract void OnDoDamage(int value);
}
