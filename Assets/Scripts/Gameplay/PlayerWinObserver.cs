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

    protected override void OnVSFX(int listIndex)
    { }

    protected override void OnScoreUpdate()
    { }

    protected override void OnTimeEnd()
    {
        movement.enabled = false; //Disable the movement of the player
        currentCoroutine = StartCoroutine(Ending()); //Start the coroutine for the ending
        restart.gameObject.SetActive(true);

        Debug.Log("Time finished");
    }

    public void Restart()
    {
        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.buildIndex); //Load the current scene
    }

    IEnumerator Ending()
    {
        //code
        yield return new WaitForSeconds(coroutineTime);
        //code
        //yield return new WaitForSeconds(coroutineTime);
    }
}
