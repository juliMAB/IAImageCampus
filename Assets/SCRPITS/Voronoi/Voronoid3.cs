using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Voronoid3 : MonoBehaviour
{

    public Rect limits = new Rect();
    [Serializable]
    public class Site
    {
        public string name;
        public Vector3 pos;
        public float weight;
    }
    [Serializable]
    public class Edge
    {
        public Site p1;
        public Site p2;

        public FuncionLineal funcionLineal;

        public Vector3 limit1;
        public Vector3 limit2;
        public void SetSites(Site p1, Site p2)
        {
            this.p1 = p1; this.p2 = p2;
        }
        public void SetLimits(Rect limites)
        {
            if (float.IsNaN(funcionLineal.m)) // para todos las x un solo y;
            {
                float y = funcionLineal.GetY(limites.xMin);
                this.limit1 = new Vector3(limites.xMin, y);
                this.limit2 = new Vector3(limites.xMax, y);
            }
            if (float.IsInfinity(funcionLineal.m)) // para todas las y un solo x;
            {
                float x = Mathf.Lerp(p1.pos.x, p2.pos.x, 0.5f);
                this.limit1 = new Vector3(x, limites.yMin);
                this.limit2 = new Vector3(x, limites.yMax);
            }
            else
            {
                float fMinx = funcionLineal.GetX(limites.yMin);
                float fMaxx = funcionLineal.GetX(limites.yMax);
                float fMiny = funcionLineal.GetY(limites.xMin);
                float fMaxy = funcionLineal.GetY(limites.xMax);
                float x1=0, y1=0, x2=0, y2=0;

                if (fMiny<=limites.yMax && fMiny>= limites.yMin)
                {
                    y1 = fMiny;
                    x1 = limites.xMin;
                }
                if (fMaxy <= limites.yMax && fMaxy >= limites.yMin)
                {
                    y2 = fMaxy;
                    x2 = limites.xMax;
                }
                if (fMinx <= limites.xMax && fMinx >= limites.xMin)
                {
                    x2 = fMinx;
                    y2 = limites.yMin;
                }

                if (fMaxx <= limites.xMax && fMaxx >= limites.xMin)
                {
                    x1 = fMaxx;
                    y1 = limites.yMax;
                }


                this.limit1 = new Vector3(x1, y1);
                this.limit2 = new Vector3(x2, y2);

            }


        }
        public void SetFuncion(FuncionLineal funcion)
        {
            this.funcionLineal = funcion;
        }

        public Edge(Site p1, Site p2, FuncionLineal funcionLineal, Vector3 limit1, Vector3 limit2)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.funcionLineal = funcionLineal;
            this.limit1 = limit1;
            this.limit2 = limit2;
        }
        public Edge(Edge edge)
        {
            p1 = edge.p1;
            p2 = edge.p2;
            funcionLineal = edge.funcionLineal;
            limit1 = edge.limit1;
            limit2 = edge.limit2;
        }

        public static bool SeCortan(Edge lhs, Edge rhs)
        {
            Vector2 cutPoint = FuncionLineal.SeCortan(rhs.funcionLineal, lhs.funcionLineal);
            if (!FuncionLineal.SeCortan(rhs.funcionLineal, cutPoint))
                return false;
            
            float minX, minY, maxX, maxY;
            Rect rhsRec = new Rect();
            if (rhs.limit1.x < rhs.limit2.x)
            {
                minX = rhs.limit1.x;
                maxX = rhs.limit2.x;
            }
            else
            {
                minX = rhs.limit2.x;
                maxX = rhs.limit1.x;
            }
            if (rhs.limit1.y < rhs.limit2.y)
            {
                minY = rhs.limit1.y;
                maxY = rhs.limit2.y;
            }
            else
            {
                minY = rhs.limit2.y;
                maxY = rhs.limit1.y;
            }
            rhsRec.min = new Vector2(minX, minY);
            rhsRec.max = new Vector2(maxX, maxY);

            Rect lhsRec = new Rect();
            if (lhs.limit1.x < lhs.limit2.x)
            {
                minX = lhs.limit1.x;
                maxX = lhs.limit2.x;
            }
            else
            {
                minX = lhs.limit2.x;
                maxX = lhs.limit1.x;
            }
            if (lhs.limit1.y < lhs.limit2.y)
            {
                minY = lhs.limit1.y;
                maxY = lhs.limit2.y;
            }
            else
            {
                minY = lhs.limit2.y;
                maxY = lhs.limit1.y;
            }
            lhsRec.min = new Vector2(minX, minY);
            lhsRec.max = new Vector2(maxX, maxY);
            if (lhsRec.yMin >= rhsRec.yMax || lhsRec.xMin >= rhsRec.xMax)
                return false;
            else
                return true;
        }
        public static bool SeCortan(Edge lhs, Vector2 rhs)
        {
            if (!FuncionLineal.SeCortan(lhs.funcionLineal, rhs))
                return false;
            float minX, minY, maxX, maxY;

            Rect lhsRec = new Rect();
            if (lhs.limit1.x < lhs.limit2.x)
            {
                minX = lhs.limit1.x;
                maxX = lhs.limit2.x;
            }
            else
            {
                minX = lhs.limit2.x;
                maxX = lhs.limit1.x;
            }
            if (lhs.limit1.y < lhs.limit2.y)
            {
                minY = lhs.limit1.y;
                maxY = lhs.limit2.y;
            }
            else
            {
                minY = lhs.limit2.y;
                maxY = lhs.limit1.y;
            }
            lhsRec.min = new Vector2(minX, minY);
            lhsRec.max = new Vector2(maxX, maxY);
            if (lhsRec.yMin < rhs.y && lhsRec.yMax > rhs.y && lhsRec.xMin < rhs.x && lhsRec.xMax > rhs.x)
                return true;
            return false;
        }

    }
    [Serializable]
    public class Vertex
    {
        public List<Edge> edges;
    }

    [Serializable]
    public class Cell
    {
        public Site site;
        public List<Edge> edges;
    }
    [Serializable]
    public class FuncionLineal
    {
        /// <summary>
        /// Pendiente
        /// </summary>
        public float m;

        /// <summary>
        /// Intercepto con el eje Y
        /// </summary>
        public float b;

        public static bool operator ==(FuncionLineal lhs, FuncionLineal rhs)
        {

            return lhs.b == rhs.b && lhs.m == rhs.m;
        }
        public static bool operator !=(FuncionLineal lhs, FuncionLineal rhs)
        {
            return lhs.b != rhs.b || lhs.m != rhs.m;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override bool Equals(object o)
        {
            return true;
        }

        public static float GetX(float m, float b, float y)
        {
            return (y - b) / m;
        }

        public static float SetB(Vector3 p, float m)
        {
            return p.y - m * p.x;
        }

        public FuncionLineal(Vector3 p1, Vector3 p2)
        {
            m = ObtenerPendiente(p1, p2);
            b = p1.y - m * p1.x;
        }
        public FuncionLineal(Vector3 p1, float m)
        {
            this.m = m;
            this.b = p1.y - m * p1.x;
            Debug.Log("se creo una nueva Funcion lineal con M: (" + m + ") B: (" + b + ");");
        }

        public float GetY(float x)
        {
            return m * x + b;
        }
        public float GetX(float y)
        {
            return (y - b) / m;
        }

        public static float ObtenerPendiente(Vector3 p1, Vector3 p2)
        {
            return (p2.y - p1.y) / (p2.x - p1.x);
        }
        public static float ObtenerPendienteMediatriz(Vector3 p1, Vector3 p2)
        {
            float m = ObtenerPendiente(p1, p2);
            return 1 / -m;
        }
        public static float PreguntarSiSeCortanX(FuncionLineal a, FuncionLineal b)
        {
            return (b.b - a.b) / (a.m - b.m);
        }
        public static float PreguntarSiSeCortanY(FuncionLineal a, FuncionLineal b)
        {
            return (-a.m * b.b + a.b * b.m) / (b.m - a.m);
        }
        public static Vector2 SeCortan(FuncionLineal a, FuncionLineal b)
        {
            //podria llegar a pasar que sean la misma funcion, pero poco probable, correre con el riesgo.
            //podrian nunca cortarse?, no seria f lineal en el caso.
            float x, y;
            x = PreguntarSiSeCortanX(a, b);
            y = PreguntarSiSeCortanY(a, b); //getY tambien.
            return new Vector2(x, y);
        }
        public static bool SeCortan(FuncionLineal a, Vector3 b)
        {
            float x = a.GetX(b.y);
            if (float.IsInfinity(x))
                return false;
            if (float.IsNaN(x))
                return false;
            if (b.x - 0.001f < x && x < b.x + 0.001f)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Decir que se cortan, no toma en cuenta los puntos que conforman el segmento.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        //public static bool SeCortan(Segmento a, Vector3 b)
        //{
        //    Slope fL = new FuncionLineal(a.p1.pos, a.p2.pos);
        //    float x = fL.GetX(b.y);
        //    if (float.IsNaN(x))
        //        return true;
        //    if (b.x - 0.001f < x && x < b.x + 0.001f)
        //    {
        //        float maxX;
        //        float minX;
        //        float maxY;
        //        float minY;
        //        if (a.p1.pos.x > a.p2.pos.x)
        //        {
        //            maxX = a.p1.pos.x;
        //            minX = a.p2.pos.x;
        //        }
        //        else
        //        {
        //            maxX = a.p2.pos.x;
        //            minX = a.p1.pos.x;
        //        }
        //
        //        if (a.p1.pos.y > a.p2.pos.y)
        //        {
        //            maxY = a.p1.pos.y;
        //            minY = a.p2.pos.y;
        //        }
        //        else
        //        {
        //            maxY = a.p2.pos.y;
        //            minY = a.p1.pos.y;
        //        }
        //
        //        return b.x < maxX && b.x > minX && b.y > minY && b.y < maxY;
        //    }
        //    return x == b.x;
        //}
    }



    public List<Site> sites = new List<Site>();
    public List<Cell> cells = new List<Cell>();
    public List<Edge> edges = new List<Edge>();
    public List<Edge> result = new List<Edge>();

    public bool drawedges;
    public bool drawcutedges;


    private void Start()
    {
        createEdges();
        //cutEdges();
    }

    public void createEdges()
    {
        edges.Clear();
        for (int i = 0; i < sites.Count; i++)
        {
            for (int j = i + 1; j < sites.Count; j++)
            {
                Vector3 midpoint = Vector3.Lerp(sites[i].pos, sites[j].pos, 0.5f);
                float slope = FuncionLineal.ObtenerPendiente(sites[i].pos, sites[j].pos);
                float slopePerpendicular = 1 / -slope;
                FuncionLineal funcion = new FuncionLineal(midpoint, slopePerpendicular);
                Edge edge = new Edge(sites[i], sites[j], funcion, Vector3.negativeInfinity, Vector3.negativeInfinity);
                edge.SetLimits(limits);
                edges.Add(edge);
            }
        }
    }
    public void cutEdges()
    {
        List<Vector2> cutPoints = new List<Vector2>();
        List<Edge> cutedges = new List<Edge>(edges);
        for (int i = 0; i < edges.Count; i++)
        {
            for (int j = 0; j < edges.Count; j++)
            {
                if (i == j)
                    continue;
                if (!Edge.SeCortan(edges[i], edges[j]))
                    continue;
                Vector2 f = FuncionLineal.SeCortan(edges[i].funcionLineal, edges[j].funcionLineal);
                if (!cutPoints.Any(
                    v => (
                    (v.x + 0.001f > f.x && v.x - 0.001f < f.x) &&
                    (v.y + 0.001f > f.y && v.y - 0.001f < f.y)
                    )))
                    cutPoints.Add(f);
            }
        }
        Debug.Log(cutPoints.Count);
        bool complete = false;
        while (!complete)
        {
            complete = true;
            for (int i = 0; i < cutedges.Count; i++)
            {
                if (cutedges[i] == null)
                    continue;
                for (int j = 0; j < cutPoints.Count; j++)
                {
                    if (cutedges[i] == null)
                        continue;
                    if ( Edge.SeCortan(cutedges[i], cutPoints[j]))
                   {
                        Edge edge1 = new Edge(cutedges[i]);
                        Edge edge2 = new Edge(cutedges[i]);
                        Vector2 f = cutPoints[j];
                        edge1.limit1 = f;
                        edge2.limit2 = f;
                        Vector2 midPoint1 = Vector2.Lerp(edge1.limit1, edge1.limit2, 0.5f);
                        Vector2 midPoint2 = Vector2.Lerp(edge2.limit1, edge2.limit2, 0.5f);
                        
                        for (int w = 0; w < sites.Count; w++)
                        {
                            if (sites[w] == cutedges[i].p1 || sites[w] == cutedges[i].p2)
                                continue;
                            if (edge1 != null)
                            {
                                if (Vector2.Distance(sites[w].pos, midPoint1) < Vector2.Distance(edge1.p1.pos, midPoint1))
                                {
                                    edge1 = null;
                                }
                            }
                            if (edge2 != null)
                            {
                                if (Vector2.Distance(sites[w].pos, midPoint2) < Vector2.Distance(edge2.p1.pos, midPoint2))
                                {
                                    edge2 = null;
                                }

                            }
                        }
                        if (edge1 != null)
                            cutedges.Add(edge1);
                        if (edge2 != null)
                            cutedges.Add(edge2);
                        cutedges[i] = null;
                        complete = false;
                   }
                }
            }
            if (cutedges.Count>1000)
            {
                continue;
            }
        }
        for (int i = 0; i < cutedges.Count; i++)
            if (cutedges[i]!=null)
                result.Add(cutedges[i]);
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < sites.Count; i++)
        {
            Gizmos.DrawWireSphere(sites[i].pos, sites[i].weight);
            Handles.Label(sites[i].pos, sites[i].name.ToString());
        }
        if (drawedges)
        {
            for (int i = 0; i < edges.Count; i++)
            {
                Gizmos.DrawLine(edges[i].limit1, edges[i].limit2);
            }
        }
        if (drawcutedges)
        {
            for (int i = 0; i < result.Count; i++)
            {
                Gizmos.DrawLine(result[i].limit1, result[i].limit2);
            }

        }

    }
}
