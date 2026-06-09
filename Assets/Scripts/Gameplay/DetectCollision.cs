using System;
using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    //public PlayerData playerData;
    public ScoreLogic scoreLogic;

    //settings
    public GenerationSettings settingsFile;

    private void OnTriggerEnter(Collider other)
    {
        if (settingsFile == null) return;
        int listIndex = other.GetComponent<ObstacleType>().listIndex;
        Destroy(other);
        var theType = settingsFile.objects[listIndex].type;
        float theValue = settingsFile.objects[listIndex].value;

        if (theType == Type.Damage)
        {
            scoreLogic.score -= (int)theValue;
            scoreLogic.InvokeScoreUpdate();
            print("doing damage" + "index:" + listIndex + " the value: " + theValue);

        }

        if (theType == Type.Health)
        {
            scoreLogic.score += (int)theValue;
            scoreLogic.InvokeScoreUpdate();
            print("doing health" + "index:" + listIndex + " the value: " + theValue);
        }
    }
}
