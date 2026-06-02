using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using Unity.Mathematics;
using System;

public class ProceduralGenerationProtoType : MonoBehaviour
{
    public List<GenerationSettings> settingsFiles;
    public List<GameObject> generatedObjects;

    

    [ContextMenu("GenerateEnvironment")]
    void GenerateEnvironment()
    {
        print("Removing Objects");
        RemoveObjects();
        print("Generating Environment");
        if (settingsFiles == null) return;
        if (settingsFiles.Count == 0) return;
        foreach (GameObject prefab in settingsFiles[0].prefabs)
        {
            Vector3 position = new Vector3(UnityEngine.Random.Range(-10.0f, 10.0f), 0, UnityEngine.Random.Range(-10.0f, 10.0f));
            generatedObjects.Add(Instantiate(prefab, position, Quaternion.identity, transform));
        }
    }

    void RemoveObjects()
    {
        bool isPlaying = Application.isPlaying;

        foreach (GameObject gameobject in generatedObjects)
        {
            if (gameobject == null) continue;

            if (isPlaying)
                Destroy(gameobject);
            else
                DestroyImmediate(gameobject);
        }

        generatedObjects.Clear();
    }

}
