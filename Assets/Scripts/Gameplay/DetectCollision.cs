using System;
using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    public event Action<int> addHealth;
    public event Action<int> doDamage;

    public PlayerData playerData;

    //settings
    public GenerationSettings settingsFile;

    private void Awake()
    {
        addHealth?.Invoke(0); //initial update of the text 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (settingsFile == null) return;
        int listIndex = other.GetComponent<ObstacleType>().listIndex;
        Destroy(other);
        var theType = settingsFile.objects[listIndex].type;
        float theValue = settingsFile.objects[listIndex].value;

        

        if (theType == Type.Damage)
        {
            doDamage?.Invoke((int)theValue);
            playerData.health -= (int)theValue;
            print("doing damage" + "index:" + listIndex + " the value: " + theValue);
            
            
        }
        if (theType == Type.Health)
        {
            addHealth?.Invoke((int)theValue);
            playerData.health += (int)theValue;
            print("doing health" + "index:" + listIndex + " the value: " + theValue);

        }

    }
}
