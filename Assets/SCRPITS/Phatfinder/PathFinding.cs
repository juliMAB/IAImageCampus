using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding
{

    enum Methods
    {
        BreadthFirst,
        dephFirst,
        Dijkstra,
        AStar
    }

    Methods methods = Methods.AStar;
    List<int> openNodesID = new List<int>();
    List<int> closedNodesID = new List<int>();
    Vector2Int destinationPosition;
    public List<Vector2Int> GetPath(Node[] map, Node origin, Node destination)
    {
        openNodesID.Add(origin.ID);
        destinationPosition = destination.position;
        Node currentNode = origin;
        while (currentNode.position != destination.position)
        {
            currentNode = GetNextNode(map, currentNode);
            if (currentNode == null)
                return new List<Vector2Int>();
            for (int i = 0; i < currentNode.adjacentNodeIDs.Count; i++)
            {
                if (currentNode.adjacentNodeIDs[i] != -1)
                {
                    if (map[ currentNode.adjacentNodeIDs[i]].state == Node.NodeState.Ready)
                    {
                        map[currentNode.adjacentNodeIDs[i]].Open(currentNode.ID,currentNode.totalWeight);
                        openNodesID.Add(map[ currentNode.adjacentNodeIDs[i]].ID);
                    }
                }
            }
            currentNode.state = Node.NodeState.Closed;
            openNodesID.Remove(currentNode.ID);
            closedNodesID.Add(currentNode.ID);

        }
        List<Vector2Int> path = GeneratePath(map,currentNode);

        foreach (Node node in map)
        {
            //node.Reset();
        }
        return path;
    }

    private List<Vector2Int> GeneratePath(Node[] map, Node current)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        while (current.openerID != -1)
        {
            path.Add(current.position);
            current = map[current.openerID];
        }
        path.Add(current.position);
        path.Reverse();
        return path;
    }

    private Node GetNextNode(Node[] map, Node currentNode)
    {

        switch (methods)
        {
            case Methods.BreadthFirst:
                return map[openNodesID[0]];
            case Methods.dephFirst:
                return map[openNodesID[openNodesID.Count-1]];
            case Methods.Dijkstra:
                {
                    Node n = null;
                    int currentMaxWeight = int.MaxValue;
                    for (int i = 0; i < openNodesID.Count; i++)
                        if (map[openNodesID[i]].totalWeight < currentMaxWeight)
                        {
                            n = map[openNodesID[i]];
                            currentMaxWeight = map[openNodesID[i]].totalWeight;
                        }
                    return n;
                }
            case Methods.AStar:
                {
                    Node n = null;
                    int currentMaxWeightAndDistance = int.MaxValue;
                    for (int i = 0; i < openNodesID.Count; i++)
                        if (map[openNodesID[i]].totalWeight + GetManhattanDistance(map[openNodesID[i]].position, destinationPosition) < currentMaxWeightAndDistance)
                        {
                            n = map[openNodesID[i]];
                            currentMaxWeightAndDistance = map[openNodesID[i]].totalWeight + GetManhattanDistance(map[openNodesID[i]].position,destinationPosition);
                        }
                    return n;
                }
        }
        return null;
        
        //for (int i = 0; i < currentNode.adjacentNodeIDs.Count; i++)
        //    if (currentNode.adjacentNodeIDs[i]!= -1)
        //        if (map[currentNode.adjacentNodeIDs[i]].state == Node.NodeState.Open)
        //            //if (map[currentNode.adjacentNodeIDs[i]].openerID == currentNode.ID)
        //                return map[currentNode.adjacentNodeIDs[i]];
        //if (currentNode.openerID == -1)
        //    return null;
        //return GetNextNode(map, map[currentNode.openerID]);
    }

    private int GetManhattanDistance(Vector2Int origin, Vector2Int destination)
    {
        int disX = Mathf.Abs(origin.x - destination.x);
        int disY = Mathf.Abs(origin.y - destination.y);
        return disX + disY;
    }
}
