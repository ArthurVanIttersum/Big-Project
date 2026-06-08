using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerWinObserver : PlayerObserver
{
    [SerializeField] Button restart;
    [SerializeField] TestMovement movement;

    protected override void OnAddHealth(int value)
    {}

    protected override void OnDoDamage(int value)
    {}

    protected override void OnScoreUpdate()
    {}

    protected override void OnTimeEnd()
    {
        movement.enabled = false;

        restart.gameObject.SetActive(true);

        Debug.Log("Time finished");
    }

    public void Restart()
    {
        Scene scene = SceneManager.GetActiveScene();
        
        Debug.Log("Restart");

        //SceneManager.LoadScene(scene.buildIndex); //need to add all scenes in side 
    }
}
