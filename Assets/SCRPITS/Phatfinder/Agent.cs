using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    List<Vector2Int> path;
    // Start is called before the first frame update
    void Start()
    {
        path = NodeGenerator.instance.GetShortestPath(new Vector2Int(0, 0), new Vector2Int(8, 3));
        StartCoroutine(Move());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Move()
    {
        for (int i = 0; i < path.Count; i++)
        {
            transform.position = new Vector3(path[i].x,path[i].y);
            yield return new WaitForSeconds(1.0f);
        }
    }
}
