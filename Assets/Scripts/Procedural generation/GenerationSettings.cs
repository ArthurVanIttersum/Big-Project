using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GenerationSettings", menuName = "Scriptable Objects/GenerationSettings")]
public class GenerationSettings : ScriptableObject
{
    public List<GameObject> prefabs;
}
