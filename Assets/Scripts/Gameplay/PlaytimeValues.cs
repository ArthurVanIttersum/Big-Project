using UnityEngine;

[CreateAssetMenu(fileName = "Playtime", menuName = "Scriptable Objects/Playtime")]
public class PlaytimeValues : ScriptableObject
{
    [Tooltip("This value is in minutes")]
    public float playTime = 1; //in minutes
    public float scoreMultiplier = 1;
    public float ballScoreRatio;
    public float maxBallSize;
}
