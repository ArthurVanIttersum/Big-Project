using System;
using System.Collections.Generic;
using UnityEngine;

public class HoneyCombChunkGeneration : MonoBehaviour
{
    //settings Files
    public HoneyGenerationSettings generationSettings;
    public ChunkSettings chunkSettings;

    //booleans
    public bool playerMoveAlongZaxis;
    public bool displayDebugVisualization;
    public bool enableDebugprints;

    //datastructures
    private Vector2Int areaMax;
    private Vector2Int areaMin;
    private GameObject chunk;

    //dictionaries
    public Dictionary<Vector2, List<Vector2>> triToHex = new();
    public Dictionary<Vector2, List<Vector2>> hexToTri = new();
    public Dictionary<Vector2, List<Vector2>> triToTri = new();

    //lists
    public HashSet<Vector2> startingPoints = new();
    public HashSet<Vector2> destinationPoints = new();
    private List<Vector2> hexOffsets2 = new();

    //hex grid
    public Grid theHexGrid = new();

    private void Start()
    {
        
    }

    void GenerateChunk(GameObject chunk)
    {
        print("Generating Chunk");
        if (generationSettings == null) return;//quick test
        if (chunkSettings == null) return;//quick test

        print("pastChecks");
        this.chunk = chunk;

        int chunkSize = chunkSettings.chunksize;
        Vector2Int centerPos = VectorConversion.vec3Tovec2Int(chunk.transform.position);

        areaMax = (Vector2Int.one * (int)chunkSize / 2) + centerPos;
        areaMin = -(Vector2Int.one * (int)chunkSize / 2) + centerPos;

        ClearDataStructures();
        MakeDataStructures();


        GenerateTerrain();


        //print(TestPathfinding());

        int tries = 1;
        while (!TestPathfinding())
        {
            tries++;
            ResetTerrainGeneration();
            GenerateTerrain();
            if (tries > 100)
            {
                print("warning tried over 100 times. aborting generation atempt");
                break;
            }
        }


        //actually place the objects
        PlaceObjects();
    }

    private void ClearDataStructures()
    {
        //clear datastructures
        theHexGrid.theGraph.Clear();
        triToHex.Clear();
        hexToTri.Clear();
        triToTri.Clear();
        MakeHexOffsets();
        startingPoints.Clear();
        destinationPoints.Clear();
    }

    private void MakeHexOffsets()
    {
        hexOffsets2.Clear();

        //floating point math
        float onethird = 1.0f / 3.0f;
        float twothird = 2.0f / 3.0f;
        if (playerMoveAlongZaxis)
        {
            hexOffsets2.Add(new Vector2(0f, twothird));
            hexOffsets2.Add(new Vector2(-0.5f, onethird));
            hexOffsets2.Add(new Vector2(-0.5f, -onethird));
            hexOffsets2.Add(new Vector2(0f, -twothird));
            hexOffsets2.Add(new Vector2(0.5f, -onethird));
            hexOffsets2.Add(new Vector2(0.5f, onethird));
        }
        else
        {
            hexOffsets2.Add(new Vector2(twothird, 0f));
            hexOffsets2.Add(new Vector2(onethird, -0.5f));
            hexOffsets2.Add(new Vector2(-onethird, -0.5f));
            hexOffsets2.Add(new Vector2(-twothird, 0f));
            hexOffsets2.Add(new Vector2(-onethird, 0.5f));
            hexOffsets2.Add(new Vector2(onethird, 0.5f));
        }
    }

    private void MakeDataStructures()
    {
        //make triangles
        if (playerMoveAlongZaxis)
        {
            float halfSpaceOffset = 0;
            for (int hor = areaMin.x; hor < areaMax.x; hor++)
            {

                for (int ver = areaMin.y; ver < areaMax.y; ver++)
                {
                    if (ver % 2 == 0)
                        halfSpaceOffset = 0.5f;
                    else
                        halfSpaceOffset = 0;
                    triToHex.Add(new Vector2(hor + halfSpaceOffset, ver), new());
                }
            }
        }
        else
        {
            float halfSpaceOffset = 0;
            for (int hor = areaMin.x; hor < areaMax.x; hor++)
            {
                if (hor % 2 == 0)
                    halfSpaceOffset = 0.5f;
                else
                    halfSpaceOffset = 0;
                for (int ver = areaMin.y; ver < areaMax.y; ver++)
                {
                    triToHex.Add(new Vector2(hor, ver + halfSpaceOffset), new());
                }
            }
        }

        //make hexagons
        foreach (var item in triToHex.Keys)
        {
            for (int i = 0; i < 6; i++)
            {
                Vector2 hexpos = SnapVector(item + hexOffsets2[i]);//snapping to thirds if close, to avoid hashing issues in the dictionary, making duplicate keys
                if (!hexToTri.ContainsKey(hexpos))
                {
                    hexToTri.Add(hexpos, new());
                }
                hexToTri[hexpos].Add(item);
                triToHex[item].Add(hexpos);

            }
        }

        //add triToTri neighbors
        foreach (var item in triToHex.Keys)
        {
            triToTri.Add(item, new());
        }

        foreach (var tri1 in triToHex.Keys)
        {
            foreach (var hex in triToHex[tri1])
            {
                foreach (var tri2 in hexToTri[hex])
                {
                    if (tri1 == tri2) continue;

                    triToTri[tri1].Add(tri2);
                }
            }
        }

        //starting points and destination points
        float onethird = 1.0f / 3.0f;
        float twothird = 2.0f / 3.0f;

        //positions to test
        List<Vector2> positionsToTest = new();


        if (playerMoveAlongZaxis)
        {
            positionsToTest.Add(SnapVector(new Vector2(0, -onethird) + areaMin));
            positionsToTest.Add(SnapVector(new Vector2(0, -twothird) + areaMin));
            positionsToTest.Add(SnapVector(new Vector2(0, -onethird) + areaMax));
            positionsToTest.Add(SnapVector(new Vector2(0, -twothird) + areaMax));
        }
        else
        {
            positionsToTest.Add(SnapVector(new Vector2(-onethird, 0) + areaMin));
            positionsToTest.Add(SnapVector(new Vector2(-twothird, 0) + areaMin));
            positionsToTest.Add(SnapVector(new Vector2(-onethird, 0) + areaMax));
            positionsToTest.Add(SnapVector(new Vector2(-twothird, 0) + areaMax));
        }


        if (playerMoveAlongZaxis)
        {
            foreach (var hexPos in hexToTri.Keys)
            {
                if (hexPos.y == positionsToTest[0].y)
                {
                    //startingPoints.Add(hexPos);
                }
                if (hexPos.y == positionsToTest[1].y)
                {
                    startingPoints.Add(hexPos);
                }
                if (hexPos.y == positionsToTest[2].y)
                {
                    destinationPoints.Add(hexPos);
                }
                if (hexPos.y == positionsToTest[3].y)
                {
                    //destinationPoints.Add(hexPos);
                }
            }
        }
        else
        {
            foreach (var hexPos in hexToTri.Keys)
            {
                if (hexPos.x == positionsToTest[0].x)
                {
                    //startingPoints.Add(hexPos);
                }
                if (hexPos.x == positionsToTest[1].x)
                {
                    startingPoints.Add(hexPos);
                }
                if (hexPos.x == positionsToTest[2].x)
                {
                    destinationPoints.Add(hexPos);
                }
                if (hexPos.x == positionsToTest[3].x)
                {
                    //destinationPoints.Add(hexPos);
                }
            }
        }


        //make grid

        //initiate nodes
        foreach (var item in hexToTri.Keys)//hexnodes
        {
            theHexGrid.theGraph.Add(item, new Node(item));
        }
        foreach (var item in triToHex.Keys)//trinodes
        {
            theHexGrid.theGraph.Add(item, new Node(item));
        }

        //add neighbors
        foreach (var item in triToHex.Keys)//hexneighbors
        {
            int count = triToHex[item].Count;
            for (int i = 0; i < count; i++)
            {

                int more = (i + 1 + count) % count;
                int less = (i - 1 + count) % count;
                Vector2 pos0 = triToHex[item][i];
                Vector2 pos1 = triToHex[item][more];
                Vector2 pos1n = triToHex[item][less];
                theHexGrid.theGraph[pos0].neighborsHex.Add(theHexGrid.theGraph[pos1]);
                theHexGrid.theGraph[pos0].neighborsHex.Add(theHexGrid.theGraph[pos1n]);
                theHexGrid.theGraph[pos0].neighborsTri.Add(theHexGrid.theGraph[item]);
                theHexGrid.theGraph[item].neighborsHex.Add(theHexGrid.theGraph[pos0]);
            }
        }
    }

    Vector2 SnapVector(Vector2 vector2)
    {
        return new Vector2(
            SnapToThirdIfClose(vector2.x),
            SnapToThirdIfClose(vector2.y)
        );
    }

    float SnapToThirdIfClose(float value, float epsilon = 0.0005f)
    {
        float third = value * 3f;
        float rounded = MathF.Round(third);

        if (MathF.Abs(third - rounded) < epsilon)
            return rounded / 3f;

        return value; // leave unchanged
    }

    private void GenerateTerrain()
    {
        //spawning
        int tempiteration = 0;
        foreach (var honeyGeneration in generationSettings.objects)
        {

            foreach (var triPoint in triToHex.Keys)
            {
                //check if the space is empty
                if (theHexGrid.theGraph[triPoint].spawnedIndex != -1) continue;

                //calculate chance
                int chance = honeyGeneration.ambientSpawnChance;
                foreach (var cluster in honeyGeneration.clusteredObjects)
                {
                    int addedChance = cluster.addedSpawnChance;

                    foreach (var triNeighborPos in triToTri[triPoint])
                    {
                        if (theHexGrid.theGraph[triNeighborPos].spawnedIndex != cluster.indexInList) continue;
                        chance += addedChance;
                        //print("adding chance " + addedChance + " " + chance + " " + tempiteration);
                    }
                }
                theHexGrid.theGraph[triPoint].spawnchance.Add(chance);

                //maximum chance
                if (chance > 100)
                {
                    chance = 100;
                }

                int random = UnityEngine.Random.Range(0, 100);
                if (random < chance)
                {
                    //Instantiate(honeyGeneration.prefab, VectorConversion.vec2Tovec3(triPoint), Quaternion.identity, chunk.transform);

                    theHexGrid.theGraph[triPoint].spawnedIndex = tempiteration;

                }
            }
            tempiteration++;
        }
    }

    private void ResetTerrainGeneration()
    {
        foreach (Vector2 triPoint in triToHex.Keys)
        {
            theHexGrid.theGraph[triPoint].spawnchance.Clear();
            theHexGrid.theGraph[triPoint].spawnedIndex = -1;
        }

        foreach (Vector2 hexPoint in hexToTri.Keys)
        {
            theHexGrid.theGraph[hexPoint].obstructed = false;
        }
    }

    private bool TestPathfinding()
    {
        //add obstacles
        foreach (var triPos in triToHex.Keys)
        {
            if (theHexGrid.theGraph[triPos].spawnedIndex == -1) continue;
            foreach (var neighbor in theHexGrid.theGraph[triPos].neighborsHex)
            {
                neighbor.obstructed = true;
            }
        }

        //test
        bool success = false;
        foreach (var startingPoint in startingPoints)
        {
            bool result = theHexGrid.BFD(startingPoint, destinationPoints, out List<Vector2> path, playerMoveAlongZaxis);

            if (result == true)
            {
                success = true;
            }

        }
        return success;
    }

    private void PlaceObjects()
    {
        foreach (var hexPos in triToHex.Keys)
        {
            int indexValue = theHexGrid.theGraph[hexPos].spawnedIndex;
            if (indexValue != -1)
            {
                Instantiate(generationSettings.objects[indexValue].prefab, VectorConversion.vec2Tovec3(hexPos), Quaternion.identity, chunk.transform);
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
