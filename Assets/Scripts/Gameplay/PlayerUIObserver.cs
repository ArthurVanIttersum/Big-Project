using TMPro;
using UnityEngine;

public class PlayerUIObserver : PlayerObserver
{
    [SerializeField] TextMeshProUGUI score;

    //Add the proper UI elements to showcase when a player loses or wins
    //[SerializeField] private List<GameObject> images = new List<GameObject>();

    protected override void OnVSFX(int value)
    {
        if (value == 0)
        {
            //images[0].gameObject.SetActive(true);
            Debug.Log("Score lowered");
        }

        else
        {
            //images[1].gameObject.SetActive(true);
            Debug.Log("Score added");
        }
    }

    protected override void OnScoreUpdate()
    {
        if (detectCollision.scoreLogic.score <= 0)
            detectCollision.scoreLogic.score = 0;

        score.text = $"Score: {(int)detectCollision.scoreLogic.score}";
    }

    protected override void OnTimeEnd()
    {}
}
