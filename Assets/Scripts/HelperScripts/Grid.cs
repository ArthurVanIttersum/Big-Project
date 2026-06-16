using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[System.Serializable]
public class Grid
{
    public Dictionary<Vector2, Node> theGraph = new();


    public bool BFD(Vector2 startingPosition, HashSet<Vector2> destinations, out List<Vector2> path, bool playerMoveAlongZaxis)
    {
        Node currentNode = theGraph[startingPosition];
        Queue<Node> toBeExplored = new();
        HashSet<Vector2> discovered = new();
        toBeExplored.Enqueue(currentNode);
        discovered.Add(currentNode.position);
        while (toBeExplored.Count > 0)
        {
            currentNode = toBeExplored.Dequeue();

            
            foreach (Node foundHex in currentNode.neighborsHex)
            {
                if (destinations.Contains(foundHex.position))
                {
                    //Debug.Log("found destination node");
                    //Debug.Log(discovered.Count);
                    path = discovered.ToList();
                    return true;
                }
                if (discovered.Contains(foundHex.position)) continue;
                if (foundHex.obstructed) continue;
                if (playerMoveAlongZaxis)
                {
                    if (foundHex.position.y < currentNode.position.y) continue;
                }
                else
                {
                    if (foundHex.position.x < currentNode.position.x) continue;
                }

                toBeExplored.Enqueue(foundHex);
                discovered.Add(foundHex.position);
            }
            if (toBeExplored.Count > 1000)
            {
                Debug.Log("warning BFS problem");
                //Debug.Log(discovered.Count);
                path = discovered.ToList();
                return false;
            }
        }
        //Debug.Log("did not find destination node");
        //Debug.Log(discovered.Count);
        path = discovered.ToList();
        return false;
    }
}

[System.Serializable]
public class Node
{
    //general data
    public Vector2 position;
    public List<Node> neighborsHex;
    public List<Node> neighborsTri;
    public int spawnedIndex = -1;
    public List<int> spawnchance = new();

    //pathfinding
    public bool obstructed = false;

    public Node(Vector2 pos)
    {
        position = pos;
        neighborsHex = new();
        neighborsTri = new();
    }
}

