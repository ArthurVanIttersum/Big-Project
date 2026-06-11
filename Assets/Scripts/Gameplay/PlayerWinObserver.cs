using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerWinObserver : PlayerObserver
{
    [SerializeField] Button restart;
    [SerializeField] TestMovement movement;

    [SerializeField] float coroutineTime;
    private Coroutine currentCoroutine;

    protected override void OnVSFX(int value)
    { }

    protected override void OnScoreUpdate()
    { }

    protected override void OnTimeEnd()
    {
        movement.enabled = false;
        currentCoroutine = StartCoroutine(Ending());
        restart.gameObject.SetActive(true);

        Debug.Log("Time finished");
    }

    public void Restart()
    {
        Scene scene = SceneManager.GetActiveScene();

        Debug.Log("Restart");

        SceneManager.LoadScene(scene.buildIndex); //need to add all scenes in side 
    }

    IEnumerator Ending()
    {
        //code
        yield return new WaitForSeconds(coroutineTime);
        //code
        //yield return new WaitForSeconds(coroutineTime);
    }
}
