using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NodeGenerator : MonoBehaviour
{
    public Vector2Int mapSize;
    private Node[] map;
    void Start()
    {
        NodeUtils.MapSize = mapSize;
        map = new Node[mapSize.x*mapSize.y];
        int ID = 0;
        for (int i = 0; i < mapSize.y; i++)
            for (int j = 0; j < mapSize.x; j++)
            {
                map[ID] = new Node(ID, new Vector2Int(j, i));
                ID++;
            }
    }

    private void OnDrawGizmos()
    {
        if(map == null)
            return;
        Gizmos.color = Color.green;
        GUIStyle style = new GUIStyle() { fontSize = 25 };
        foreach (Node node in map)
        {
            Vector3 worldPosition = new Vector3(node.position.x, node.position.y, 0.0f);
            Gizmos.DrawWireSphere(worldPosition, 0.2f);
            Handles.Label(worldPosition, node.ID.ToString(), style);
        }
    }
}
