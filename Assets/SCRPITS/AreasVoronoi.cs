using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreasVoronoi : MonoBehaviour
{
    [SerializeField] Vector3[] limits = null;
    [SerializeField] int pointsAmount = -1;

    Vector3[] points;
    List<(Vector3 p1,Vector3 p2)> cuts = new List<(Vector3, Vector3)>();

    enum STATE
    {
        zero,
        one,
        two,
        three,
    } STATE state = STATE.zero;

    private void Start()
    {


        

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            state++;
        }

        switch (state)
        {
            case STATE.one:
                CreatePointsAndCuts();
                state++;
                break;
            case STATE.two:
                break;
            case STATE.three:
                EstrudeCutes();
                state++;
                break;
            default:
                break;
        }
    }

    private void EstrudeCutes()
    {
        for (int i = 0; i < cuts.Count; i++)
        {
            float pendiente = (cuts[i].p1.y - cuts[i].p2.y) / (cuts[i].p1.x - cuts[i].p2.x);

            //formula recta = y2 -y1 = m(x2-x1)

            // (y - cuts[i].p2.y)  = pendiente(x - cuts[i].p2.x)

            // y = (pendiente(x- cuts[i].p2.x)) - cuts[i].p2.y
            for (int j = i; j < cuts.Count; j++)
            {
                float y = -1;
                float x = -1;
                float pendienteB = (cuts[j].p1.y - cuts[j].p2.y) / (cuts[j].p1.x - cuts[j].p2.x);

                //(pendiente(x - cuts[i].p2.x)) - cuts[i].p2.y = (pendienteB(x- cuts[j].p2.x)) - cuts[j].p2.y

                //(pendiente * x) - (pendiente * cuts[i].p2.x) - cuts[i].p2.y = (pendienteB * x) - (pendienteB * cuts[j].p2.x) - cuts[j].p2.y

                //pendiente*x - pendienteB*x = - (pendienteB * cuts[j].p2.x) - cuts[j].p2.y + (pendiente * cuts[i].p2.x) + cuts[i].p2.y

                // x (pendiente - pendienteB) = ...
                x = (-(pendienteB * cuts[j].p2.x) - cuts[j].p2.y + (pendiente * cuts[i].p2.x) + cuts[i].p2.y) / (pendiente - pendienteB);
                Debug.Log(x);
                y = x + pendiente;
                if (x > limits[1].x)
                    x = limits[1].x;
                else if (x < limits[0].x)
                    x = limits[0].x;
                if (y > limits[1].y)
                    y = limits[1].y;
                else if (y < limits[0].y)
                    y = limits[0].y;
                cuts[i] = (new Vector3(x, y, 0), cuts[i].p2);
            }

        }
    }

    private void CreatePointsAndCuts()
    {
        points = new Vector3[pointsAmount];

        for (int i = 0; i < pointsAmount; i++)
        {
            float rx = Random.Range(limits[0].x, limits[1].x);
            float ry = Random.Range(limits[0].y, limits[1].y);
            float rz = Random.Range(limits[0].z, limits[1].z);
            points[i] = new Vector3(rx, ry, rz);
        }

        for (int i = 0; i < pointsAmount; i++)
        {
            for (int j = i + 1; j < pointsAmount; j++)
            {
                Vector3 puntomedio = Vector3.Lerp(points[j], points[i], 0.5f);

                Vector3 vectorDirector = points[j] - points[i];

                Vector3 vectorDirectorPerpendicular = new Vector3(-vectorDirector.y, vectorDirector.x);

                Vector3 p1 = puntomedio + vectorDirectorPerpendicular.normalized;
                Vector3 p2 = -vectorDirectorPerpendicular.normalized + puntomedio;
                cuts.Add((p1, p2));
            }
        }
    }


    private void OnDrawGizmos()
    {
        for (int i = 0; i < cuts.Count; i++)
        {
            Debug.DrawLine(cuts[i].p1, cuts[i].p2);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(cuts[i].p1, 0.1f);
            Gizmos.DrawWireSphere(cuts[i].p2, 0.1f);
        }

        Gizmos.color = Color.white;

        for (int i = 0; i < pointsAmount; i++)
        {
            Gizmos.DrawWireSphere(points[i], 0.1f);
        } 
           
    }
}
