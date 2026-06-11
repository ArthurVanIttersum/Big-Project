using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChunkSettings", menuName = "Scriptable Objects/ChunkSettings")]
public class ChunkSettings : ScriptableObject
{
    public int chunksize = 10;
    public int chunkGenerationDistanceInChunks;//depricated
    public int chunkDeletionDistanceInChunks;//depricated
    //2D
    public Vector2Int chunkGenerationAreaInChunks = new Vector2Int(11, 3);
}
