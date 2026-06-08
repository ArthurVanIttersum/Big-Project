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


    
    void GenerateChunk(GameObject chunk)
    {
        print("Generating Chunk");
        if (generationSettings == null) return;//quick test
        if (chunkSettings == null) return;//quick test
        
        print("pastChecks");

        //Add chunk to dictionary
        int chunkSize = chunkSettings.chunksize;
        
        
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
                Instantiate(item.prefab, randomOffset + chunk.transform.position, Quaternion.identity, chunk.transform);
            }
        }
    }


    void RemoveChunk(GameObject removed)
    {
        print("Removing Chunk");
        
        RemoveObjectHelper.RemoveObject(removed);
    }

    private void OnEnable()
    {
        
        ContinuousGeneration script = FindAnyObjectByType<ContinuousGeneration>();
        script.DeleteChunkAtPosition += RemoveChunk;
        script.GenerateAtPosition += GenerateChunk;
        Continuous2DGeneration script2 = FindAnyObjectByType<Continuous2DGeneration>();
        script2.DeleteChunkAtPosition += RemoveChunk;
        script2.GenerateAtPosition += GenerateChunk;
    }
    
}

