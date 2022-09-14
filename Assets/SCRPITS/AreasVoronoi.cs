using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreasVoronoi : MonoBehaviour
{
    [SerializeField] Vector3[] limits = null;
    [SerializeField] int pointsAmount = -1;

    [SerializeField] Vector3[] points = null;
    [SerializeField] List<Segmento> cuts = new List<Segmento>();

    public class Segmento
    {
        public Vector3 p1;
        public Vector3 p2;


        public static bool operator ==(Segmento lhs, Segmento rhs)
        {
            
            return (lhs.p1 == rhs.p1 && lhs.p2 == rhs.p2) || (lhs.p2 == rhs.p1 && lhs.p1 == rhs.p2);
        }
        public static bool operator !=(Segmento lhs, Segmento rhs)
        {
            return (lhs.p1 == rhs.p1 && lhs.p2 != rhs.p2) ||
                   (lhs.p2 == rhs.p1 && lhs.p1 != rhs.p2) || 
                   (lhs.p1 != rhs.p1 && lhs.p2 == rhs.p2) ||
                   (lhs.p2 != rhs.p1 && lhs.p1 == rhs.p2);
        }
        public Segmento(Vector3 a,Vector3 b)
        {
            p1 = a;
            p2 = b;
        }
    }

    public enum STATE
    {
        zero,
        one,
        two,
        three,
    } 
    public STATE state = STATE.zero;

    public enum POINTS
    {
        random,
        noRandom,
    }   
    public POINTS modoPuntos = POINTS.random;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            state++;
        }

        switch (state)
        {
            case STATE.one:
                if (modoPuntos== POINTS.random)
                {
                    CreatePointsAndCuts();
                }
                else
                {
                    SetPointsAndCuts();
                }
                state++;
                break;
            case STATE.two:
                break;
            case STATE.three:
                EstrudeCutes();
                state=0;
                break;
            default:
                break;
        }
    }

    private void SetPointsAndCuts()
    {
        cuts.Clear();
        for (int i = 0; i < points.Length; i++)
        {
            for (int j = i + 1; j < points.Length; j++)
            {
                Vector3 puntomedio = Vector3.Lerp(points[j], points[i], 0.5f);

                Vector3 vectorDirector = points[j] - points[i];

                Vector3 vectorDirectorPerpendicular = new Vector3(-vectorDirector.y, vectorDirector.x);

                Vector3 p1 = puntomedio + vectorDirectorPerpendicular.normalized;
                Vector3 p2 = -vectorDirectorPerpendicular.normalized + puntomedio;
                cuts.Add(new Segmento(p1, p2));
            }
        }
    }

    private void EstrudeCutes()
    {
        Debug.Log("cuts: " + cuts.Count);
        for (int i = 0; i < cuts.Count; i++)
        {
            float y = -1;
            float x = -1;

            float y2 = -1;
            float x2 = -1;

            if (cuts[i].p1.x > cuts[i].p2.x)
                x = limits[1].x;
            if (cuts[i].p1.x < cuts[i].p2.x)
                x = limits[0].x;

            if (cuts[i].p1.y > cuts[i].p2.y)
                y = limits[1].y;
            if (cuts[i].p1.y < cuts[i].p2.y)
                y = limits[0].y;


            if (cuts[i].p2.x > cuts[i].p1.x)
                x2 = limits[1].x;
            if (cuts[i].p2.x < cuts[i].p1.x)
                x2 = limits[0].x;

            if (cuts[i].p2.y > cuts[i].p1.y)
                y2 = limits[1].y;
            if (cuts[i].p2.y < cuts[i].p1.y)
                y2 = limits[0].y;

            float pendiente = (cuts[i].p1.y - cuts[i].p2.y) / (cuts[i].p1.x - cuts[i].p2.x);

            Vector3 vectorDirector = new Vector3(cuts[i].p2.x - cuts[i].p1.x, cuts[i].p2.y - cuts[i].p1.y,0);

            vectorDirector.Normalize();

            //float pendiente2 = (cuts[i].p1.y - cuts[i].p2.y) / (cuts[i].p1.x - cuts[i].p2.x);

            //formula recta = y2 -y1 = m(x2-x1)

            // (y - cuts[i].p2.y)  = pendiente(x - cuts[i].p2.x)

            // y = (pendiente(x- cuts[i].p2.x)) - cuts[i].p2.y
            if (cuts[i].p2.x > cuts[i].p1.x)
                while (cuts[i].p2.x < limits[1].x)
                    cuts[i].p2.x += vectorDirector.x;


            if (cuts[i].p2.x < cuts[i].p1.x)
                while (cuts[i].p2.x > limits[0].x)
                    cuts[i].p2.x -= vectorDirector.x;


            if (cuts[i].p2.y > cuts[i].p1.y)
                while (cuts[i].p2.y < limits[1].y)
                    cuts[i].p2.y += vectorDirector.y;

            if (cuts[i].p2.y < cuts[i].p1.y)
                while (cuts[i].p2.y > limits[0].y)
                    cuts[i].p2.y -= vectorDirector.y; 


            //if (cuts[i].p2.x < cuts[i].p1.x)
            //    x2 = limits[0].x;


            for (int j = i+1; j < cuts.Count; j++)
            {
                
                float pendienteB = (cuts[j].p1.y - cuts[j].p2.y) / (cuts[j].p1.x - cuts[j].p2.x);

                //(pendiente(x - cuts[i].p2.x)) - cuts[i].p2.y = (pendienteB(x- cuts[j].p2.x)) - cuts[j].p2.y

                //(pendiente * x) - (pendiente * cuts[i].p2.x) - cuts[i].p2.y = (pendienteB * x) - (pendienteB * cuts[j].p2.x) - cuts[j].p2.y

                //pendiente*x - pendienteB*x = - (pendienteB * cuts[j].p2.x) - cuts[j].p2.y + (pendiente * cuts[i].p2.x) + cuts[i].p2.y

                // x (pendiente - pendienteB) = ...
                x = (-(pendienteB * cuts[j].p2.x) - cuts[j].p2.y + (pendiente * cuts[i].p2.x) + cuts[i].p2.y) / (pendiente - pendienteB);
               
                
               
            }

            //if (pendiente == 0)
            
            y = x * pendiente + cuts[i].p1.y;

            y2 = x2 * pendiente + cuts[i].p2.y;
            
            //if (x > limits[1].x)
            //    x = limits[1].x;
            //if (x < limits[0].x)
            //    x = limits[0].x;
            //if (y > limits[1].y)
            //    y = limits[1].y;
            //if (y < limits[0].y)
            //    y = limits[0].y;

            //if (x2 > limits[1].x)
            //    x2 = limits[1].x;
            //if (x2 < limits[0].x)
            //    x2 = limits[0].x;
            //if (y2 > limits[1].y)
            //    y2 = limits[1].y;
            //if (y2 < limits[0].y)
            //    y2 = limits[0].y;

            //if (x == float.NaN)
            //    x = 0;
            //
            //if (x2 == float.NaN)
            //    x2 = 0;
            //Debug.Log("x: " + x);
            //Debug.Log("y: " + y);
            //Debug.Log("pendiente: " + pendiente);
            //
            //StartCoroutine(movePointsA(cuts[i].p1.x, cuts[i].p1.y, x, y, i));
            //StartCoroutine(movePointsB(cuts[i].p2.x, cuts[i].p2.y, x2, y2, i));

        }
    }

    IEnumerator movePointsA(float initX,float initY,float lastX,float lastY, int index)
    {

        float time = 0;
        float x, y;
        while (time<1)
        {
            yield return null;
           
            x = Mathf.Lerp(initX, lastX, time);
            y = Mathf.Lerp(initY, lastY, time);

            cuts[index] = new Segmento(new Vector3(x, y, 0), cuts[index].p2);
            time += Time.deltaTime;
        }
        
    }
    IEnumerator movePointsB(float initX, float initY, float lastX, float lastY, int index)
    {

        float time = 0;
        float x, y;
        while (time < 1)
        {
            yield return null;

            x = Mathf.Lerp(initX, lastX, time);
            y = Mathf.Lerp(initY, lastY, time);

            cuts[index] = new Segmento(cuts[index].p1,new Vector3(x, y, 0));
            time += Time.deltaTime;
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
                cuts.Add(new Segmento(p1, p2));
            }
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(limits[0],new Vector3(limits[0].x,limits[1].y,0));
        Gizmos.DrawLine(limits[0], new Vector3(limits[1].x, limits[0].y, 0));
        Gizmos.DrawLine(limits[1], new Vector3(limits[0].x, limits[1].y, 0));
        Gizmos.DrawLine(limits[1], new Vector3(limits[1].x, limits[0].y, 0));


        if (points == null)
            return;
        for (int i = 0; i < cuts.Count; i++)
        {
            Debug.DrawLine(cuts[i].p1, cuts[i].p2);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(cuts[i].p1, 0.1f);
            Gizmos.DrawWireSphere(cuts[i].p2, 0.1f);
        }

        Gizmos.color = Color.white;

        for (int i = 0; i < points.Length; i++)
        {
            Gizmos.DrawWireSphere(points[i], 0.1f);
        }

        
        
        


    }
}
