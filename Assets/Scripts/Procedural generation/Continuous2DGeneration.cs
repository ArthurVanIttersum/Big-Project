using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Continuous2DGeneration : MonoBehaviour
{
    //important gameobjects
    public GameObject player;
    public GameObject prefabEmptyGameObject;

    //settings files
    public ChunkSettings chunkSettings;

    //private valiables
    private Vector2Int lastPosition;
    private Vector2Int currentPosition;
    private HashSet<Vector2Int> chunksInAABB = new();
    private Dictionary<Vector2Int, GameObject> generatedChunks = new();

    //events
    public event Action<GameObject> GenerateAtPosition;
    public event Action<GameObject> DeleteChunkAtPosition;

    private void Start()
    {
        if (player == null) return;//quick test
        if (chunkSettings == null) return;//quick test

        Vector2Int roundedPlayerPosition = GetPlayerChunkPosition();
        currentPosition = roundedPlayerPosition;
        lastPosition = roundedPlayerPosition;

        //generate a few chunks
        UpdateAABBList();
        UpdateChunkGeneration();
        print("are we generating");
    }
    private void Update()
    {
        UpdatePosition();
        if (currentPosition == lastPosition) return;
        UpdateAABBList();
        UpdateChunkGeneration();
        lastPosition = currentPosition;
        print("are we updating");
    }

    void UpdatePosition()
    {
        if (player == null) return;//quick test

        //calculate current chunk
        Vector2Int roundedPlayerPosition = GetPlayerChunkPosition();
        currentPosition = roundedPlayerPosition;
    }

    void UpdateChunkGeneration()
    {
        print("are we chunkGenerating");
        if (chunkSettings == null) return;//quick test

        //delete bad chunks
        var outside = generatedChunks.Keys.Where(k => !chunksInAABB.Contains(k)).ToList();
        print("outside size: " + outside.Count);

        foreach (var key in outside)
        {
            generatedChunks.Remove(key, out GameObject removed);
            DeleteChunkAtPosition?.Invoke(removed);
        }

        //add missing chunks
        foreach (var key in chunksInAABB)
        {
            if (!generatedChunks.ContainsKey(key))
            {
                GameObject chunk = MakeChunkGameobject(key);
                generatedChunks.Add(key, chunk);
                GenerateAtPosition?.Invoke(chunk);
                print("are we invoking");
            }
        }
    }

    private GameObject MakeChunkGameobject(Vector2Int pos)
    {
        //Create an empty gameobject representing a chunk.
        //store it in the dictionary instead of storing individual objects.
        GameObject chunk = Instantiate(prefabEmptyGameObject, Vector3.zero + new Vector3(pos.x, 0, pos.y), Quaternion.identity, transform);
        chunk.name = "Chunk " + pos.ToString();
        return chunk;
    }

    private void UpdateAABBList()
    {
        if (chunkSettings == null) return;//quick test

        //clear
        chunksInAABB.Clear();

        //chunkArea
        Vector2Int chunkArea = chunkSettings.chunkGenerationAreaInChunks;
        Vector2Int centerOffset = new Vector2Int(chunkArea.x / 2, chunkArea.y / 2);
        int chunksize = chunkSettings.chunksize;

        //corners
        Vector2Int maxPos = currentPosition + (centerOffset + Vector2Int.one) * chunksize;
        Vector2Int minPos = currentPosition - centerOffset * chunksize;

        print("area is: " + maxPos + ", " + minPos);

        //interpolate to generate and delete chunks
        for (int x = minPos.x; x < maxPos.x; x += chunksize)
        {
            print("AABB size: " + chunksInAABB.Count);
            for (int y = minPos.y; y < maxPos.y; y += chunksize)
            {
                chunksInAABB.Add(new Vector2Int(x, y));
                print("AABB size: " + chunksInAABB.Count);
            }
        }
        print("AABB size: " + chunksInAABB.Count);
    }

    private Vector2Int GetPlayerChunkPosition()
    {
        if (chunkSettings == null) print("warning: player not diffined");//quick test

        //values
        Vector3 position = player.transform.position;
        Vector2Int chunkArea =  chunkSettings.chunkGenerationAreaInChunks;
        int chunkSize = chunkSettings.chunksize;

        //calculation
        int roundedPlayerPositionx = (Mathf.FloorToInt(position.x) / chunkSize) * chunkSize;
        int roundedPlayerPositionz = (Mathf.FloorToInt(position.z) / chunkSize) * chunkSize;
        Vector2Int roundedPosition = new Vector2Int(roundedPlayerPositionx, roundedPlayerPositionz);

        //return
        return roundedPosition;
    }
}
