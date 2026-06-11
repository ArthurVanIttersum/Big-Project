using System;
using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    //public PlayerData playerData;
    public ScoreLogic scoreLogic;

    public event Action<int> vsfx;

    //settings
    public GenerationSettings settingsFile;

    private void OnTriggerEnter(Collider other)
    {
        if (settingsFile == null) return;
        int listIndex = other.GetComponent<ObstacleType>().listIndex;
        RemoveObjectHelper.RemoveObject(other.gameObject);
        var theType = settingsFile.objects[listIndex].type;
        float theValue = settingsFile.objects[listIndex].value;

        if (theType == Type.Damage)
        {
            scoreLogic.score -= (int)theValue;
            vsfx?.Invoke(0);
            scoreLogic.InvokeScoreUpdate();
            print("doing damage" + "index:" + listIndex + " the value: " + theValue);

        }

        if (theType == Type.Health)
        {
            scoreLogic.score += (int)theValue;
            vsfx?.Invoke(1);
            scoreLogic.InvokeScoreUpdate();
            print("doing health" + "index:" + listIndex + " the value: " + theValue);
        }
    }
}
