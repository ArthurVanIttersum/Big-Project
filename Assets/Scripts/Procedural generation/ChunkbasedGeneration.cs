using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;

public class ChunkBasedGeneration : MonoBehaviour
{
    //settings Files
    public GenerationSettings generationSettings;
    public ChunkSettings chunkSettings;


    //empty prefab
    public GameObject prefabEmptyGameObject;

    private Dictionary<int, GameObject> generatedChunks = new();

    
    void GenerateChunk(int zPos)
    {
        print("Generating Chunk");
        if (generationSettings == null) return;//quick test
        if (chunkSettings == null) return;//quick test
        if (generatedChunks.ContainsKey(zPos)) return;//quick test
        
        print("pastChecks");

        //Create an empty gameobject representing a chunk.
        //store it in the dictionary instead of storing individual objects.
        GameObject chunk = Instantiate(prefabEmptyGameObject, Vector3.zero + Vector3.forward * zPos, Quaternion.identity, transform);
        chunk.name = "Chunk " + zPos.ToString();
        
        print(chunk);

        //Add chunk to dictionary
        int chunkSize = chunkSettings.chunksize;
        
        generatedChunks.Add(zPos, chunk);

        print(generatedChunks.Count);
        print(zPos);

        //generate objects in chunk
        foreach (var item in generationSettings.objects)
        {
            for (int i = 0; i < item.objectCount; i++)
            {
                Vector3 randomOffset = new Vector3(
                    UnityEngine.Random.Range(-chunkSize/2, chunkSize/2),
                    0,
                    UnityEngine.Random.Range(-chunkSize / 2, chunkSize / 2)
                );
                Instantiate(item.prefab, randomOffset + Vector3.forward * zPos, Quaternion.identity, chunk.transform);
            }
        }
    }


    void RemoveChunk(int zPos)
    {
        print("Removing Chunk");
        if (!generatedChunks.ContainsKey(zPos)) return;//quick test
        print(zPos);
        generatedChunks.Remove(zPos, out GameObject removed);
        RemoveObjectHelper.RemoveObject(removed);
    }

    private void OnEnable()
    {
        
        ContinuousGeneration script = FindAnyObjectByType<ContinuousGeneration>();
        script.DeleteChunkAtPosition += RemoveChunk;
        script.GenerateAtPosition += GenerateChunk;
    }
    
}

