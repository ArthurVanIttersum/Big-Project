using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using static UnityEngine.LowLevelPhysics2D.PhysicsLayers;

public class scoredisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreDisplay;
    private string scoreText;

    private int tempScore;
    private string tempName;
    private static bool published;

    public int topN = 10;
    private int topNUpdating;

    private states state;

    enum states
    {
        writeName,
        displayScores
    }

    private void Start()
    {
        tempScore = SaveSystem.Instance.data.pendingScore;
        tempName = "";
        state = states.writeName;
        published = false;
    }

    private void Update()
    {
        if (state == states.displayScores)
        {
            DisplayTopScores();
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                SaveSystem.Instance.DeleteAllScores();
            }
        }
        if (state == states.writeName)
        {
            TypeName();
            DisplayPendingScoreAndName();
        }
    }

    private void TypeName()
    {
        foreach (char c in Input.inputString)
        {
            if (c == '\b' && tempName.Length > 0) // Backspace
            {
                tempName = tempName.Substring(0, tempName.Length - 1);
            }
            else if (c == '\n' || c == '\r') // Enter
            {
                PublishScore();
                state = states.displayScores;
                return;
            }
            else if (!char.IsControl(c))
            {
                tempName += c;
                published = false;
            }
        }
    }

    private void DisplayPendingScoreAndName()
    {
        scoreText = "NewScore: " + tempScore.ToString() + " Write your username: " + tempName;
        scoreDisplay.text = scoreText;
    }

    private void PublishScore()
    {
        if (published) return;
        published = true;
        ScoreboardData data = new ScoreboardData(SaveSystem.Instance.data.pendingScore, tempName);
        SaveSystem.Instance.data.scoresList.Add(data);

        SaveSystem.Instance.SaveGame();
        tempName = "";
        tempScore = 0;
        SaveSystem.Instance.data.pendingScore = 0;
    }

    private void DisplayTopScores()
    {
        scoreText = "Scoreboard\n";
        topNUpdating = topN;

        // Ensure the SaveSystem exists before trying to read from it
        if (SaveSystem.Instance == null)
        {
            Debug.LogError("SaveSystem is missing from the scene!");
            return;
        }

        // Duplicate the list from our SaveSystem so we don't accidentally ruin the original order while sorting
        List<ScoreboardData> temporaryScoresList = new List<ScoreboardData>(SaveSystem.Instance.data.scoresList).OrderByDescending(x => x.score).ToList();

        if (temporaryScoresList.Count < topN)
        {
            topNUpdating = temporaryScoresList.Count;
        }


        for (int i = 0; i < topNUpdating; i++)
        {
            scoreText += "Rank: " + (i + 1).ToString() + " Score: " + temporaryScoresList[i].score.ToString() + " UserName: " + temporaryScoresList[i].name + "\n";
        }
        scoreDisplay.text = scoreText;
    }
}