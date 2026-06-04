using System;
using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    public event Action<int> addHealth;
    public event Action<int> doDamage;

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
            doDamage?.Invoke((int)theValue);
            print("doing damage" + "index:" + listIndex + " the value: " + theValue);
            
            
        }
        if (theType == Type.Health)
        {
            addHealth?.Invoke((int)theValue);
            print("doing health" + "index:" + listIndex + " the value: " + theValue);

        }

    }
    
}
