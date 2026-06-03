using System;
using UnityEngine;

public class ContinuousGeneration : MonoBehaviour
{
    //important gameobjects
    public GameObject player;

    //settings files
    public ChunkSettings chunkSettings;

    //private valiables
    private int lastPosition;
    private int currentPosition;

    //events
    public event Action<int> GenerateAtPosition;
    public event Action<int> DeleteChunkAtPosition;

    private void Start()
    {
        //calculate current chunk
        Vector3 playerPosition = player.transform.position;
        int chunkSize = chunkSettings.chunksize;
        int roundedPlayerPosition = (Mathf.FloorToInt(playerPosition.z) / chunkSize) * chunkSize;
        lastPosition = roundedPlayerPosition;

        //generate a few chunks ahead
        if (chunkSettings == null) return;//quick test

        //interpolate to generate and delete chunks
        for (int i = currentPosition; i < chunkSettings.chunkGenerationDistanceInChunks * chunkSize; i += chunkSize)
        {
            print("Updating Chunk Generation");
            GenerateAtPosition.Invoke(i);
        }
    }
    private void Update()
    {
        UpdatePosition();
        UpdateChunkGeneration();
        lastPosition = currentPosition;
    }

    void UpdatePosition()
    {
        if (player == null) return;//quick test

        //calculate current chunk
        Vector3 playerPosition = player.transform.position;
        int chunkSize = chunkSettings.chunksize;
        int roundedPlayerPosition = (Mathf.FloorToInt(playerPosition.z) / chunkSize) * chunkSize;
        currentPosition = roundedPlayerPosition;
        
    }

    void UpdateChunkGeneration()
    {
        if (chunkSettings == null) return;//quick test

        //interpolate to generate and delete chunks
        for (int i = lastPosition; i < currentPosition; i += chunkSettings.chunksize)
        {
            print("Updating Chunk Generation");
            GenerateAtPosition.Invoke(i + chunkSettings.chunkGenerationDistanceInChunks * chunkSettings.chunksize);
            DeleteChunkAtPosition.Invoke(i - chunkSettings.chunkDeletionDistanceInChunks * chunkSettings.chunksize);
        }
    }
    
}

