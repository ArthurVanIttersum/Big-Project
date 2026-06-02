using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GenerationSettings", menuName = "Scriptable Objects/GenerationSettings")]
public class GenerationSettings : ScriptableObject
{
    public List<RandomSpawnedObject> objects;
}

[System.Serializable]
public struct RandomSpawnedObject
{
    public GameObject prefab;
    public int objectCount;
}