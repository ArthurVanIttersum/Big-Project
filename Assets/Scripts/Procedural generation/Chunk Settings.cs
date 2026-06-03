using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChunkSettings", menuName = "Scriptable Objects/ChunkSettings")]
public class ChunkSettings : ScriptableObject
{
    public int chunksize = 10;
    public int chunkGenerationDistanceInChunks;
    public int chunkDeletionDistanceInChunks;
}
