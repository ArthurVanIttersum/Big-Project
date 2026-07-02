using TMPro;
using UnityEngine;

public class scoredisplay : MonoBehaviour
{
    public scores scores;
    [SerializeField] TextMeshProUGUI scoreDisplay;
    private string scoreText;
    public int topN = 10;
    private int topNUpdating;

    private void Start()
    {
        scoreText = "Scoreboard\n";
        topNUpdating = topN;
        if (scores.scoresList.Count < topN)
        {
            topNUpdating = scores.scoresList.Count;
        }
        scores.scoresList.Sort();
        scores.scoresList.Reverse();
        for (int i = 0; i < topNUpdating; i++)
        {
            scoreText += "Rank: " + (i+1).ToString() + " Score: " + scores.scoresList[i].ToString() + "\n";
        }
        scoreDisplay.text = scoreText;
    }


}
