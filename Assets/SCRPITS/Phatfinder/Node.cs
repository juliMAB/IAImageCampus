using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int ID;
    public Vector2Int position;
    public List<int> adjacentNodeIDs;

    public Node(int ID, Vector2Int position)
    {
        this.ID = ID;
        this.position = position;
        this.adjacentNodeIDs = NodeUtils.GetAdjacentsNodesIDs(position);
    } 
}
public static class NodeUtils
{
    public static Vector2Int MapSize;

    public static List<int> GetAdjacentsNodesIDs(Vector2Int position)
    {
        List<int> IDs = new List<int>();
        IDs.Add(PositionToIndex(new Vector2Int(position.x + 1, position.y)));
        IDs.Add(PositionToIndex(new Vector2Int(position.x, position.y - 1)));
        IDs.Add(PositionToIndex(new Vector2Int(position.x - 1, position.y)));
        IDs.Add(PositionToIndex(new Vector2Int(position.x, position.y + 1)));
        return IDs;
    }

    public static int PositionToIndex(Vector2Int position)
    {
        if (position.x < 0 || position.x >= MapSize.x ||
            position.y < 0 || position.y >= MapSize.y)
            return -1;
        return position.y * MapSize.x + position.x;
    }
}