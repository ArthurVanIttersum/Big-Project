using UnityEngine;

[CreateAssetMenu(fileName = "Playtime", menuName = "Scriptable Objects/Playtime")]

//Valus related to playtime mechanics 
public class PlaytimeValues : ScriptableObject
{
    [Tooltip("This value is in minutes")]
    public float playTime = 1; //in minutes
    public float scoreMultiplier = 1; //How much score is affected by time (ex. 1 -> score goes up by 1 every second; 0.5 -> score goes up by 0.5 every second)
    public float ballScoreRatio; //Compared to the score how big the ball gets (ex. 1 -> 1 score = 1 unit increase of the ball in every dimention; 0.01 -> 100 score = 1 unit increase of the ball in every dimention)
    public float maxBallSize; //The maximum size of the ball. Past this point, the point would not grow no matter the score. 
    public float massScoreRatio; //Compared to the score how the mass of the ball changes (ex. 1 -> 1 score = 1 unit increase of the mass; 0.01 -> 100 score = 1 unit increase of the mass)
    public float maxMass; //How much mass can the ball have
}
