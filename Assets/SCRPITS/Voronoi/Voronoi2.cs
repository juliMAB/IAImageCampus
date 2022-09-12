using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class Voronoi2 : MonoBehaviour
{
    public Rect limits = new Rect();

    [Serializable]
    public class Point
    {
        public int ID;
        public Vector3 pos;
        public float weight;

        public Point(Vector3 a, float b, int ID)
        {
            pos = a;
            weight = b;
            this.ID = ID;
        }
    }



    [Serializable]

    public class Triangle
    {
        public Point p1;
        public Point p2;
        public Point p3;

        public Segmento s12;
        public Segmento s23;
        public Segmento s31;



        public Triangle(Point p1, Point p2, Point p3, Segmento s12, Segmento s23, Segmento s31)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            this.s12 = s12;
            this.s23 = s23;
            this.s31 = s31;
        }
        public Triangle(Segmento s12, Segmento s23, Segmento s31)
        {
            this.s12 = s12;
            this.s23 = s23;
            this.s31 = s31;

            this.p1 = s12.p1;
            this.p2 = s12.p2;
            this.p3 = s23.p2;
        }
    }

    [Serializable]
    public class Segmento
    {
        public Point p1;
        public Point p2;

        public Point PuntoContrarioAP1yP2;

        public Segmento(Point p1, Point p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }
        public static Vector3 ObtenerPuntoMedio(Point a, Point b)
        {
            return Vector3.Lerp(a.pos, b.pos, 0.5f);
        }
        public Vector3 ObtenerPuntoMedio()
        {
            return Vector3.Lerp(p1.pos, p2.pos, 0.5f);
        }
        public Vector3 ObtenerPuntoMedioModificadoPorPeso()
        {
            return Vector3.Lerp(p1.pos, p2.pos, ((-p1.weight + p2.weight) / 2) - 0.5f);
        }
    }
    [Serializable]
    public class FuncionLineal
    {

        public Point PuntoRelacionado;
        /// <summary>
        /// Pendiente
        /// </summary>
        public float m;

        /// <summary>
        /// Intercepto con el eje Y
        /// </summary>
        public float b;

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
            b = p1.y - m * p1.x;
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
            if (b.x-0.001f< x && x< b.x+0.001f)
            {
                return true;
            }
            return x == b.x;
        }
    }

    //[Serializable]

    //public class PointAndLineal
    //{
    //    public Point point;
    //
    //    public FuncionLineal[] funcionesRelacionadas;
    //
    //    public PointAndLineal(Point point, FuncionLineal[] funcionesRelacionadas)
    //    {
    //        this.point = point;
    //        this.funcionesRelacionadas = funcionesRelacionadas;
    //    }
    //}

    public int NodesCuantity = 0;

    public List<Point> List_Nodes = new List<Point>();

    public List<Segmento> List_Segmentos = new List<Segmento>();

    public List<Triangle> List_Triangleos = new List<Triangle>();

    public List<FuncionLineal> List_FuncionLineal = new List<FuncionLineal>();

    public List<Segmento> List_SegmentosLimites = new List<Segmento>();

    public List<Point> Lista_Puntos_De_Corte = new List<Point>();

    public List<Segmento> List_Cortes = new List<Segmento>();

    [Serializable]
    public class MyColors
    {
        public Color LimitsColor;
        public Color Initial_Nodes;
        public Color Initial_Edges;
        public Color Initial_Funciones;
        public Color Initial_EdgesLimites;
        public Color PuntosDeCortes;
        public Color Cortes;
    }
    public MyColors colores;


    private void Start()
    {
        RandomNodes();
        InitSegmentos();
        InitFuncionLineales();
        InitPuntosDeCorte();
    }

    public bool b_drawLimits = false;

    public bool b_drawNodes = false;

    public bool b_drawSegments = false;

    public bool b_drawFunciones = false;

    public bool b_drawSegmentosLimites = false;

    public bool b_drawCutPoints = false;

    public bool b_drawSegmentosCortados = false;
    public void RandomNodes()
    {
        List_Nodes.Clear();
        for (int j = 0; j < NodesCuantity; j++)
            List_Nodes.Add(new Point(new Vector3(UnityEngine.Random.Range(limits.xMin, limits.xMax), UnityEngine.Random.Range(limits.yMin, limits.yMax), 0), UnityEngine.Random.Range(0.1f, 1), j));
    }

    public void InitSegmentos()
    {
        List_Segmentos = new List<Segmento>();
        List_Triangleos = new List<Triangle>();

        for (int i = 0; i < List_Nodes.Count; i++)
        {

            Point puntoMasCercano1 = new Point(Vector3.positiveInfinity, 0, -1);
            Point puntoMasCercano2 = new Point(Vector3.positiveInfinity, 0, -1);
            for (int j = 0; j < List_Nodes.Count; j++)
            {
                if (i == j)
                    continue;
                if (Vector3.Distance(puntoMasCercano1.pos, List_Nodes[i].pos) > Vector3.Distance(List_Nodes[i].pos, List_Nodes[j].pos))
                {
                    puntoMasCercano2 = puntoMasCercano1;
                    puntoMasCercano1 = List_Nodes[j];
                }
                else if (Vector3.Distance(puntoMasCercano2.pos, List_Nodes[i].pos) > Vector3.Distance(List_Nodes[i].pos, List_Nodes[j].pos))
                {
                    puntoMasCercano2 = List_Nodes[j];
                }
            }
            if ((puntoMasCercano1.pos.x != Vector3.positiveInfinity.x) && !PreguntarSiLaRelacionExiste(List_Nodes[i].ID, puntoMasCercano1.ID))
            {
                List_Segmentos.Add(new Segmento(List_Nodes[i], puntoMasCercano1));
            }

            if ((puntoMasCercano2.pos.x != Vector3.positiveInfinity.x) && !PreguntarSiLaRelacionExiste(List_Nodes[i].ID, puntoMasCercano2.ID))
            {
                List_Segmentos.Add(new Segmento(List_Nodes[i], puntoMasCercano2));
            }
            if (!PreguntarSiLaRelacionExiste(puntoMasCercano1.ID, puntoMasCercano2.ID))
            {
                List_Segmentos.Add(new Segmento(puntoMasCercano1, puntoMasCercano2));
            }
        }

        for (int i = 0; i < List_Segmentos.Count; i++)
        {

            List_Segmentos[i].PuntoContrarioAP1yP2 = ObtenerPuntoDelTriangulo(List_Segmentos[i].p1, List_Segmentos[i].p2);
        }
    }
    private Point ObtenerPuntoDelTriangulo(Point a, Point b)
    {
        
        for (int i = 0; i < List_Segmentos.Count; i++)
        {
            if ((List_Segmentos[i].p1 == a && List_Segmentos[i].p2 == b) || (List_Segmentos[i].p2 == a && List_Segmentos[i].p1 == b))
                continue;
            for (int j = i+1; j < List_Segmentos.Count; j++)
            {
                if (List_Segmentos[i].p1 == a || List_Segmentos[i].p2 == a )
                {
                    if (List_Segmentos[j].p1 == b || List_Segmentos[j].p2 == b)
                    {
                        if (List_Segmentos[i].p1 == a)
                        {
                            return List_Segmentos[i].p2;
                        }
                        else
                            return List_Segmentos[i].p1;
                    }
                }
                if (List_Segmentos[i].p1 == b || List_Segmentos[i].p2 == b)
                {
                    if(List_Segmentos[j].p1 == a || List_Segmentos[j].p2 == a)
                    {
                        if (List_Segmentos[i].p1 == a)
                        {
                            return List_Segmentos[i].p2;
                        }
                        else
                            return List_Segmentos[i].p1;
                    }
                }
            }
        }
        return null;
    }

    public void InitTriangles()
    {
        for (int i = 0; i < List_Segmentos.Count; i++)
        {
            for (int j = i + 1; j < List_Segmentos.Count; j++)
            {
                for (int w = j + 1; w < List_Segmentos.Count; w++)
                {
                    int[] ides = new int[6];
                    ides[0] = List_Segmentos[i].p1.ID;
                    ides[1] = List_Segmentos[i].p2.ID;

                    ides[2] = List_Segmentos[j].p1.ID;
                    ides[3] = List_Segmentos[j].p2.ID;

                    ides[4] = List_Segmentos[w].p1.ID;
                    ides[5] = List_Segmentos[w].p2.ID;

                    bool[] coincidencias = new bool[3];
                    for (int z = 0; z < coincidencias.Length; z++)
                        coincidencias[z] = false;
                    for (int z = 0; z < ides.Length; z++)
                    {
                        for (int c = 0; c < ides.Length; c++)
                        {
                            if (c == z)
                                continue;
                            if (ides[z] == ides[c])
                                for (int v = 0; v < coincidencias.Length; v++)
                                {
                                    if (!coincidencias[v])
                                    {
                                        coincidencias[v] = true;
                                        break;
                                    }
                                }
                        }
                    }
                    if (coincidencias[0] && coincidencias[1] && coincidencias[2])
                        if (!PreguntarSiElTrianguloExiste(List_Segmentos[i], List_Segmentos[j], List_Segmentos[w]))
                            List_Triangleos.Add(new Triangle(List_Segmentos[i], List_Segmentos[j], List_Segmentos[w]));
                }
            }
        }
    }

    private bool PreguntarSiElTrianguloExiste(Segmento a, Segmento b, Segmento c)
    {
        for (int i = 0; i < List_Triangleos.Count; i++)
        {
            if (List_Triangleos[i].s12 == a)
            {
                if (List_Triangleos[i].s23 == b)
                {
                    if (List_Triangleos[i].s31 == c)
                    {
                        return true;
                    }
                }
                if (List_Triangleos[i].s23 == c)
                {
                    if (List_Triangleos[i].s31 == b)
                    {
                        return true;
                    }
                }
            }
            if (List_Triangleos[i].s12 == b)
            {
                if (List_Triangleos[i].s23 == c)
                {
                    if (List_Triangleos[i].s31 == a)
                    {
                        return true;
                    }
                }
                if (List_Triangleos[i].s23 == c)
                {
                    if (List_Triangleos[i].s31 == a)
                    {
                        return true;
                    }
                }
            }
            if (List_Triangleos[i].s12 == c)
            {
                if (List_Triangleos[i].s23 == a)
                {
                    if (List_Triangleos[i].s31 == b)
                    {
                        return true;
                    }
                }
                if (List_Triangleos[i].s23 == b)
                {
                    if (List_Triangleos[i].s31 == a)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }


    private bool PreguntarSiLaRelacionExiste(int id1, int id2)
    {
        for (int j = 0; j < List_Segmentos.Count; j++)
        {
            if (List_Segmentos[j].p1.ID == id1 && List_Segmentos[j].p2.ID == id2 || List_Segmentos[j].p1.ID == id2 && List_Segmentos[j].p2.ID == id1)
            {
                return true;
            }
        }
        return false;
    }

    public void InitFuncionLineales()
    {
        List_FuncionLineal = new List<FuncionLineal>();
        for (int i = 0; i < List_Segmentos.Count; i++)
        {

            FuncionLineal funcionLinealLocal = new FuncionLineal(
                    List_Segmentos[i].ObtenerPuntoMedio(),
                    FuncionLineal.ObtenerPendienteMediatriz(List_Segmentos[i].p1.pos, List_Segmentos[i].p2.pos));
            funcionLinealLocal.PuntoRelacionado = List_Segmentos[i].PuntoContrarioAP1yP2;
            List_FuncionLineal.Add(funcionLinealLocal);
        }
        List_SegmentosLimites = new List<Segmento>();
       for (int i = 0; i < List_FuncionLineal.Count; i++)
       {
            Point p1 = null;
            Point p2 = null;
            float y = List_FuncionLineal[i].GetY(limits.x);
            float x = limits.x;
            if (y >= limits.y && y <= limits.y + limits.height)
            {
                 p2 = p1;
                 p1 = new Point(new Vector3 (x, y, 0),0,-1);
            }
            y = List_FuncionLineal[i].GetY(limits.x + limits.width);
            x = limits.x + limits.width;
            if (y >= limits.y && y <= limits.y + limits.height)
            {
                p2 = p1;
                p1 = new Point(new Vector3(x, y, 0), 0,-1);
            }
            y = limits.y;
            x = List_FuncionLineal[i].GetX(limits.y);
            if (x > limits.x && x < limits.x + limits.width)
            {
                p2 = p1;
                p1 = new Point(new Vector3(x, y, 0), 0,-1);
            }
            y = limits.y+limits.height;
            x = List_FuncionLineal[i].GetX(limits.y+ limits.height);
            if (x > limits.x && x < limits.x + limits.width)
            {
                p2 = p1;
                p1 = new Point(new Vector3(x, y, 0), 0,-1);
            }
            Segmento LocalSegmento = new Segmento(p1, p2);
            LocalSegmento.PuntoContrarioAP1yP2 = List_FuncionLineal[i].PuntoRelacionado;
            List_SegmentosLimites.Add(LocalSegmento);
        }
    }

    public void InitPuntosDeCorte()
    {
        Lista_Puntos_De_Corte = new List<Point>();
        for (int i = 0; i < List_FuncionLineal.Count; i++)
        {
            for (int j = i+1; j < List_FuncionLineal.Count; j++)
            {
                for (int w = j + 1; w < List_FuncionLineal.Count; w++)
                    if (FuncionLineal.SeCortan(List_FuncionLineal[i], List_FuncionLineal[j]) == FuncionLineal.SeCortan(List_FuncionLineal[j], List_FuncionLineal[w]))
                    { //se cortan 3 rectas en un mismo punto.
                        Vector2 p1 = FuncionLineal.SeCortan(List_FuncionLineal[i], List_FuncionLineal[j]);
                        Vector3 p2 = new Vector3(p1.x, p1.y, 0);
                        //FuncionLineal[] funcionesRelacionadas = { List_FuncionLineal[i], List_FuncionLineal[j], List_FuncionLineal[w] };
                        Lista_Puntos_De_Corte.Add(new Point(p2,0,w));
                    }
            }   
        }
    }
    public void InitCorte()
    {
        List_Cortes = new List<Segmento>();
        for (int i = 0; i < List_SegmentosLimites.Count; i++)
        {
            for (int j = 0; j < Lista_Puntos_De_Corte.Count; j++)
            {
                if (FuncionLineal.SeCortan (new FuncionLineal(List_SegmentosLimites[i].p1.pos, List_SegmentosLimites[i].p2.pos), Lista_Puntos_De_Corte[j].pos))
                {
                    Segmento localSegmento1 = new Segmento(List_SegmentosLimites[i].p1, Lista_Puntos_De_Corte[j]);
                    localSegmento1.PuntoContrarioAP1yP2 = List_SegmentosLimites[i].PuntoContrarioAP1yP2;
                    Segmento localSegmento2 = new Segmento(List_SegmentosLimites[i].p2, Lista_Puntos_De_Corte[j]);
                    localSegmento2.PuntoContrarioAP1yP2 = List_SegmentosLimites[i].PuntoContrarioAP1yP2;
                    //Debug.DrawLine(List_SegmentosLimites[i].PuntoContrarioAP1yP2.pos, List_SegmentosLimites[i].PuntoContrarioAP1yP2.pos+Vector3.one);
                    //Debug.Break();
                    if (Vector3.Distance(localSegmento1.ObtenerPuntoMedio(), List_SegmentosLimites[i].PuntoContrarioAP1yP2.pos) <
                        Vector3.Distance(localSegmento2.ObtenerPuntoMedio(), List_SegmentosLimites[i].PuntoContrarioAP1yP2.pos)
                        )
                    {
                        List_Cortes.Add(localSegmento2);
                    }
                    else
                    {

                        List_Cortes.Add(localSegmento1);
                    }

                }
            }
        }
    } 

    private void OnDrawGizmos()
    {
        Gizmos.color = colores.LimitsColor;
        if (b_drawLimits)
        {
            Gizmos.DrawLine(new Vector3(limits.position.x, limits.position.y), new Vector3(limits.position.x + limits.width, limits.position.y));
            Gizmos.DrawLine(new Vector3(limits.position.x, limits.position.y), new Vector3(limits.position.x, limits.position.y + limits.height));
            Gizmos.DrawLine(new Vector3(limits.position.x + limits.width, limits.position.y + limits.height), new Vector3(limits.position.x, limits.position.y + limits.height));
            Gizmos.DrawLine(new Vector3(limits.position.x + limits.width, limits.position.y + limits.height), new Vector3(limits.position.x + limits.width, limits.position.y));
        }

        Gizmos.color = colores.Initial_Nodes;
        if (b_drawNodes)
            if (List_Nodes != null)
                for (int i = 0; i < List_Nodes.Count; i++)
                {
                    Gizmos.color = colores.Initial_Nodes;
                    Gizmos.DrawWireSphere(List_Nodes[i].pos, List_Nodes[i].weight);
                    Handles.Label(List_Nodes[i].pos, i.ToString());
                }

        Gizmos.color = colores.Initial_Edges;
        if (b_drawSegments)
            if (List_Segmentos != null)
                for (int i = 0; i < List_Segmentos.Count; i++)
                {
                    Gizmos.DrawLine(List_Segmentos[i].p1.pos, List_Segmentos[i].p2.pos);
                }

        Gizmos.color = colores.Initial_Funciones;
        if (b_drawFunciones)
            if(List_FuncionLineal!=null)
                for (int i = 0; i < List_FuncionLineal.Count; i++)
                {
                    Gizmos.color = colores.Initial_Funciones;
                    if (List_FuncionLineal[i].m != 0 && !float.IsInfinity(List_FuncionLineal[i].m))
                    {
                        float yTop = 10;
                        float yDown = -10;
                    Gizmos.DrawLine(
                        new Vector3(List_FuncionLineal[i].GetX(yTop), yTop),
                        new Vector3(List_FuncionLineal[i].GetX(yDown), yDown));

                    }
                    else
                    {
                        if (List_FuncionLineal[i].m == 0)
                        {
                            Gizmos.DrawLine(
                                new Vector3(limits.x               , List_Segmentos[i].ObtenerPuntoMedio().y),
                                new Vector3(limits.x + limits.width, List_Segmentos[i].ObtenerPuntoMedio().y));
                            continue;
                        }
                        else //fijarse dentro de la funcion si se divide por 0, tiende a infinito, por ende pregunto esto.
                        {
                            Gizmos.DrawLine(
                            new Vector3(List_Segmentos[i].ObtenerPuntoMedio().x, limits.y),
                            new Vector3(List_Segmentos[i].ObtenerPuntoMedio().x, limits.y + limits.height));
                            continue;
                        }
                    }
                }

        Gizmos.color = colores.Initial_EdgesLimites;
        if (b_drawSegmentosLimites)
            if (List_SegmentosLimites != null)
                for (int i = 0; i < List_SegmentosLimites.Count; i++)
                    Gizmos.DrawLine(List_SegmentosLimites[i].p1.pos, List_SegmentosLimites[i].p2.pos);
        Gizmos.color = colores.PuntosDeCortes;
        if (b_drawCutPoints)
            if (Lista_Puntos_De_Corte != null)
                for (int i = 0; i < Lista_Puntos_De_Corte.Count; i++)
                    Gizmos.DrawSphere(Lista_Puntos_De_Corte[i].pos, 0.1f);
        Gizmos.color = colores.Cortes;
        if (b_drawSegmentosCortados)
            if (List_Cortes != null)
                for (int i = 0; i < List_Cortes.Count; i++)
                    Gizmos.DrawLine(List_Cortes[i].p1.pos, List_Cortes[i].p2.pos);
    }
}
