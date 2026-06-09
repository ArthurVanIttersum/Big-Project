using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class HoneyCombPrototype : MonoBehaviour
{
    public GenerationSettings settingsFile;
    public List<GameObject> generatedObjects;
    public float areaSize = 10.0f;
    private Vector2Int areaMax;
    private Vector2Int areaMin;

    private List<Vector2Int> trianglePoints = new();
    private List<float> horList = new();
    private List<float> verList = new();

    [ContextMenu("GenerateEnvironment")]
    void GenerateEnvironment()
    {
        print("Removing Objects");
        RemoveObjects();
        print("Generating Environment");
        if (settingsFile == null) return;

        areaMax = Vector2Int.one * (int)areaSize/2;
        areaMin = -areaMax;

        trianglePoints.Clear();
        horList.Clear();
        verList.Clear();

        

    }


    [ContextMenu("RemoveObjects")]
    void RemoveObjects()
    {
        foreach (GameObject gameobject in generatedObjects)
        {
            RemoveObjectHelper.RemoveObject(gameobject);
        }
        generatedObjects.Clear();
    }

    private void OnDrawGizmos()
    {
        //sphere test
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(new Vector3(0,0,0), 0.3f);

        //draw min max
        Gizmos.color = Color.cyan;
        //Gizmos.DrawSphere(Vector2IntToVector3Float.vec2IntTovec3(areaMin), 0.3f);
        Gizmos.color = Color.magenta;
        //Gizmos.DrawSphere(Vector2IntToVector3Float.vec2IntTovec3(areaMax), 0.3f);

        //chunkArea
        Gizmos.color = Color.blue;
        Vector3 topleft = new Vector3(areaMin.x, 0, areaMax.y);
        Vector3 bottomleft = new Vector3(areaMin.x, 0, areaMin.y);
        Vector3 topright = new Vector3(areaMax.x, 0, areaMax.y);
        Vector3 bottomright = new Vector3(areaMax.x, 0, areaMin.y);

        Gizmos.DrawSphere(topleft, 0.3f);
        Gizmos.DrawSphere(topright, 0.3f);
        Gizmos.DrawSphere(bottomleft, 0.3f);
        Gizmos.DrawSphere(bottomright, 0.3f);

        Gizmos.DrawLine(topleft, topright);
        Gizmos.DrawLine(bottomleft, bottomright);
        Gizmos.DrawLine(topleft, bottomleft);
        Gizmos.DrawLine(topright, bottomright);

        //grid test
        Gizmos.color = Color.red;
        for (int i = 0; i < areaSize; i++)
        {
            Vector3 horOffset = Vector3.right * i;
            Vector3 verOffset = Vector3.forward * i;

            Gizmos.DrawLine(bottomleft + horOffset, topleft + horOffset);
            Gizmos.DrawLine(bottomleft + verOffset, bottomright + verOffset);
        }


        //label test
        Gizmos.color = Color.white;
        UnityEditor.Handles.Label(new Vector3(0, 0, 0), "text");
    }

}
