using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class HoneyCombChunkGeneration : MonoBehaviour
{
    //settings Files
    [SerializeField] private BiomeSelection biomeSelection;
    public ReworkedHoneyGenerationSettings generationSettings;
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

    //caching
    private bool cached = false;

    //special data for generation
    private List<Vector2> masterAvailableKeys = new();
    private Dictionary<Vector2, int> revMasterAvailableKeys = new();
    private int[] indices;
    
    //special data for coin generation
    private HashSet<Vector2> coinPositions = new();
    private HashSet<Vector2> bigCoinPositions = new();
    private HashSet<Vector2> smallCoinPositions = new();
    private Vector2[] smallCoinArray;
    private Vector2[] bigCoinArray;


    private void Awake()
    {
        generationSettings = (ReworkedHoneyGenerationSettings)biomeSelection.Biome(0);
    }

    void GenerateChunk(GameObject chunk)
    {
        generationSettings = (ReworkedHoneyGenerationSettings)biomeSelection.Biome(biomeSelection.randomBiomeChance);

        print("Generating Chunk");
        if (generationSettings == null) return;//quick test
        Debug.Log("genSetting passed");
        if (chunkSettings == null) return;//quick test

        print("pastChecks");
        this.chunk = chunk;

        int chunkSize = chunkSettings.chunksize;
        Vector2Int centerPos = Vector2Int.zero;

        areaMax = (Vector2Int.one * (int)chunkSize / 2) + centerPos;
        areaMin = -(Vector2Int.one * (int)chunkSize / 2) + centerPos;

        if (!cached)
        {
            print("caching data");
            CacheData();
            cached = true;

        }


        
        ResetTerrainGeneration();
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
        //generate coin chance
        GenerateCoins();


        //actually place the objects
        PlaceObjects(); //this is about 1/3 of garbage collection allocation
    }

    private void CacheData()
    {
        ClearDataStructures();
        MakeDataStructures();
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

    private void MakeDataStructures()//this might be part of the problem
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
                    //print("adding values");
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
                    //print("adding values");
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
                    if (triToTri[tri1].Contains(tri2)) continue;
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

        //build datastructures for generation
        BuildAvailableKeys();

        //make grid

        //initiate nodes
        int iD = 0;
        foreach (var item in hexToTri.Keys)//hexnodes
        {
            iD++;
            theHexGrid.theGraph.Add(item, new Node(item, iD));
        }
        iD = 0;
        foreach (var item in triToHex.Keys)//trinodes
        {
            theHexGrid.theGraph.Add(item, new Node(item, iD));
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
                if (VectorCompareDirectional(pos0, pos1))
                {
                    theHexGrid.theGraph[pos0].neighborsHex.Add(theHexGrid.theGraph[pos1]);
                }
                if (VectorCompareDirectional(pos0, pos1n))
                {
                    theHexGrid.theGraph[pos0].neighborsHex.Add(theHexGrid.theGraph[pos1n]);
                }
                if (VectorCompareDirectional(pos0, item))
                {
                    theHexGrid.theGraph[pos0].neighborsHex.Add(theHexGrid.theGraph[item]);
                }
                if (VectorCompareDirectional(item, pos0))
                {
                    theHexGrid.theGraph[item].neighborsHex.Add(theHexGrid.theGraph[pos0]);
                }
            }
        }

        //initiate destination list
        foreach (var item in destinationPoints)
        {
            theHexGrid.destinationIDs.Add(theHexGrid.theGraph[item].iD);
        }
    }

    private bool VectorCompareDirectional(Vector2 node, Vector2 neighbor)
    {
        if (playerMoveAlongZaxis)
        {
            return (node.y < neighbor.y);
        }
        else
        {
            return (node.x < neighbor.x);
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

        return value;
    }

    private void BuildAvailableKeys()
    {
        masterAvailableKeys.Clear();
        revMasterAvailableKeys.Clear();
        foreach (var key in triToHex.Keys)
        {
            revMasterAvailableKeys.Add(key, masterAvailableKeys.Count);
            masterAvailableKeys.Add(key);
        }
    }


    


    private void GenerateTerrain()
    {
        
        int masterCount = masterAvailableKeys.Count;
        if (masterCount == 0) return;

        // Create an index buffer 0..n-1
        indices = new int[masterCount];
        for (int i = 0; i < masterCount; i++) indices[i] = i;

        // active prefix length (number of available keys remaining)
        int keyCount = masterCount;
        int toSpawnCount;
        int spawnedObjectCount = 0;
        int rangeStart;
        int rangeEnd;


        int tmpIdx;
        int randomIndex;

        // iterate over object types
        for (int objectToSpawn = 0; objectToSpawn < generationSettings.objects.Count; objectToSpawn++)
        {
            // compute how many objects to spawn for this type
            ReworkedHoneySpawnedObject reworkedHoneyGeneration = generationSettings.objects[objectToSpawn];
            toSpawnCount = UnityEngine.Random.Range(reworkedHoneyGeneration.minSpawnCount, reworkedHoneyGeneration.maxSpawnCount);
            
            //calculate range
            rangeStart = spawnedObjectCount;
            rangeEnd = rangeStart + toSpawnCount;
            if (rangeEnd > keyCount)
            {
                rangeEnd = keyCount;
            }

            //randomize the indices array for the first n indices
            for (int j = rangeStart; j < rangeEnd; j++)
            {
                // pick random index in the remaining range [i, keyCount-1]
                randomIndex = UnityEngine.Random.Range(j, keyCount);

                // swap indices[randomIndex] <-> indices[i]
                tmpIdx = indices[randomIndex];
                indices[randomIndex] = indices[j];
                indices[j] = tmpIdx;
            }

            //step1 spawn basic objects
            for (int j = rangeStart; j < rangeEnd; j++)
            { 
                // chosen key is masterAvailableKeys[ indices[i] ]
                Vector2 chosenKey = masterAvailableKeys[indices[j]];

                // defensive checks using theHexGrid
                if (!theHexGrid.theGraph.TryGetValue(chosenKey, out var chosenNode)) continue;
                if (chosenNode.spawnedIndex != -1) continue;

                chosenNode.spawnedIndex = objectToSpawn;
                spawnedObjectCount ++;

            }



            //step2, spawn objects around it.
            foreach (IncreaseSpawnChance extraObject in reworkedHoneyGeneration.extraObjects)
            {
                int chance = extraObject.addedSpawnChance;

                for (int j = rangeStart; j < rangeEnd; j++)
                {
                    Vector2 chosenKey = masterAvailableKeys[indices[j]];
                    foreach (Vector2 pos in triToTri[chosenKey])
                    {
                        if (!theHexGrid.theGraph.TryGetValue(pos, out var chosenNode)) continue;
                        if (chosenNode.spawnedIndex != -1) continue;
                        if (UnityEngine.Random.Range(0, 100) > chance) continue;

                        chosenNode.spawnedIndex = extraObject.indexInList;

                        //swap
                        tmpIdx = indices[revMasterAvailableKeys[pos]];
                        indices[revMasterAvailableKeys[pos]] = indices[spawnedObjectCount];
                        indices[spawnedObjectCount] = tmpIdx;

                        spawnedObjectCount++;
                    }
                }
            }
        }
    }




    private void ResetTerrainGeneration()
    {
        foreach (Vector2 triPoint in triToHex.Keys)
        {
            
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
        bool result = false;
        foreach (var startingPoint in startingPoints)
        {
            result = theHexGrid.ReworkedBFS(startingPoint, playerMoveAlongZaxis);
            
            if (result == false)
            {
                return false;
            }
        }
        return true;
    }

    private void GenerateCoins()
    {
        //reset everything
        coinPositions.Clear();
        smallCoinPositions.Clear();
        bigCoinPositions.Clear();

        //generate hashset
        foreach (Vector2 key in triToTri.Keys)
        {
            if (theHexGrid.theGraph[key].spawnedIndex == -1) continue;
            foreach (var neighbor in triToTri[key])
            {
                if (theHexGrid.theGraph[neighbor].spawnedIndex != -1) continue;
                coinPositions.Add(neighbor);
            }
        }
        int neighbors = 0;
        foreach (Vector2 key in coinPositions)
        {
            foreach (Vector2 neighbor in triToTri[key])
            {
                if (theHexGrid.theGraph[neighbor].spawnedIndex == -1) continue;
                neighbors++;
            }
            if (neighbors == 1)
            {
                smallCoinPositions.Add(key);
            }
            else
            {
                bigCoinPositions.Add(key);
            }
            neighbors = 0;
        }
        //print("coinPositions: " + coinPositions.Count.ToString() + ", smallCoinPositions: " + smallCoinPositions.Count.ToString() + ", bigCoinPositions: " + bigCoinPositions.Count.ToString());


        //initialize ints
        int randomIndex;
        int tmpIdx;


        //      small coins

        //initialize arrays

        indices = new int[smallCoinPositions.Count];
        for (int i = 0; i < smallCoinPositions.Count; i++) indices[i] = i;
        smallCoinArray = smallCoinPositions.ToArray();


        //compute how many objects to spawn for this type
        CoinData coinData = generationSettings.smallCoinData;
        int toSpawnCount = UnityEngine.Random.Range(coinData.minSpawnCount, coinData.maxSpawnCount);

        //calculate range
        if (toSpawnCount > smallCoinPositions.Count)
        {
            toSpawnCount = smallCoinPositions.Count;
        }

        //randomize the indices array for the first n indices
        for (int j = 0; j < toSpawnCount; j++)
        {
            // pick random index in the remaining range [i, keyCount-1]
            randomIndex = UnityEngine.Random.Range(j, smallCoinPositions.Count);

            // swap indices[randomIndex] <-> indices[i]
            tmpIdx = indices[randomIndex];
            indices[randomIndex] = indices[j];
            indices[j] = tmpIdx;
        }

        for (int j = 0; j < toSpawnCount; j++)
        {
            Instantiate(generationSettings.smallCoinData.prefab, VectorConversion.vec2Tovec3(smallCoinArray[indices[j]]) + chunk.transform.position, Quaternion.identity, chunk.transform);
        }

        //      big coins

        //initialize arrays

        indices = new int[bigCoinPositions.Count];
        for (int i = 0; i < bigCoinPositions.Count; i++) indices[i] = i;
        bigCoinArray = bigCoinPositions.ToArray();


        //compute how many objects to spawn for this type
        coinData = generationSettings.bigCoinData;
        toSpawnCount = UnityEngine.Random.Range(coinData.minSpawnCount, coinData.maxSpawnCount);

        //calculate range
        if (toSpawnCount > bigCoinPositions.Count)
        {
            toSpawnCount = bigCoinPositions.Count;
        }

        //randomize the indices array for the first n indices
        for (int j = 0; j < toSpawnCount; j++)
        {
            // pick random index in the remaining range [i, keyCount-1]
            randomIndex = UnityEngine.Random.Range(j, bigCoinPositions.Count);

            // swap indices[randomIndex] <-> indices[i]
            tmpIdx = indices[randomIndex];
            indices[randomIndex] = indices[j];
            indices[j] = tmpIdx;
        }

        for (int j = 0; j < toSpawnCount; j++)
        {
            Instantiate(generationSettings.bigCoinData.prefab, VectorConversion.vec2Tovec3(bigCoinArray[indices[j]]) + chunk.transform.position, Quaternion.identity, chunk.transform);
        }

    }

    private void PlaceObjects()
    {
        int indexValue = 0;
        foreach (var hexPos in triToHex.Keys)
        {
            indexValue = theHexGrid.theGraph[hexPos].spawnedIndex;
            if (indexValue != -1)
            {
                Instantiate(generationSettings.objects[indexValue].prefab, VectorConversion.vec2Tovec3(hexPos) + chunk.transform.position, Quaternion.identity, chunk.transform);
            }
        }
    }

    void RemoveChunk(GameObject removed)
    {
        RemoveObjectHelper.RemoveObject(removed);
    }

    private void OnEnable()
    {
        Continuous2DGeneration script = FindAnyObjectByType<Continuous2DGeneration>();
        script.DeleteChunkAtPosition += RemoveChunk;
        script.GenerateAtPosition += GenerateChunk;
    }
}
