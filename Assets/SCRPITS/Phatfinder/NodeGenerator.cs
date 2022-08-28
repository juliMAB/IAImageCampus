using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NodeGenerator : MonoBehaviour
{
    public static NodeGenerator instance = null;


    public Vector2Int mapSize;
    private Node[] map;
    private PathFinding pathfinding;


    private void Awake()
    {
        instance = this;
        pathfinding = new PathFinding();
        NodeUtils.MapSize = mapSize;
        map = new Node[mapSize.x * mapSize.y];
        int ID = 0;
        for (int i = 0; i < mapSize.y; i++)
        {
            for (int j = 0; j < mapSize.x; j++)
            {
                map[ID] = new Node(ID, new Vector2Int(j, i));
                ID++;
            }
        }

        map[NodeUtils.PositionToIndex(new Vector2Int(1, 0))].state = Node.NodeState.Obstacle;
        map[NodeUtils.PositionToIndex(new Vector2Int(3, 1))].state = Node.NodeState.Obstacle;
        map[NodeUtils.PositionToIndex(new Vector2Int(1, 1))].SetWeight(2);
        map[NodeUtils.PositionToIndex(new Vector2Int(1, 2))].SetWeight(2);
        map[NodeUtils.PositionToIndex(new Vector2Int(1, 3))].SetWeight(2);
        map[NodeUtils.PositionToIndex(new Vector2Int(1, 4))].SetWeight(2);
        map[NodeUtils.PositionToIndex(new Vector2Int(1, 5))].SetWeight(2);
        map[NodeUtils.PositionToIndex(new Vector2Int(1, 6))].SetWeight(2);
    }
    void Start()
    {

  
    }

    public List<Vector2Int> GetShortestPath(Vector2Int origin, Vector2Int destination)
    {
        return pathfinding.GetPath(map,
            map[NodeUtils.PositionToIndex(origin)],
            map[NodeUtils.PositionToIndex(destination)]);
    }
    private void OnDrawGizmos()
    {
        if(map == null)
            return;
        Gizmos.color = Color.green;
        GUIStyle style = new GUIStyle() { fontSize = 25 };
        foreach (Node node in map)
        {
            switch (node.state)
            {
                case Node.NodeState.Open:
                    Gizmos.color = Color.green;
                    break;
                case Node.NodeState.Closed:
                    Gizmos.color = Color.black;
                    break;
                case Node.NodeState.Ready:
                    Gizmos.color = Color.white;
                    break;
                case Node.NodeState.Obstacle:
                    Gizmos.color = Color.red;
                    break;
                default:
                    break;
            }
            
            Vector3 worldPosition = new Vector3(node.position.x, node.position.y, 0.0f);
            Gizmos.DrawWireSphere(worldPosition, 0.2f);
            Handles.Label(worldPosition, node.ID.ToString(), style);
        }
    }
}
