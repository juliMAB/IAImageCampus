using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using UnityEditor;
using UnityEngine;
public class Voronoi2 : MonoBehaviour
{
    public Coroutine LocalCorrutine = null;

    public Rect limits = new Rect();

    [Serializable]

    public enum mode
    {
        normal,
        weight
    }
    public mode modeActual;


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
        public Point(Vector3 a)
        {
            pos = a;
            weight = 1;
            this.ID = -1;
        }
        public bool EnLimitesInclusivo(Rect limites)
        {
            return pos.x <= limites.xMax && pos.x >= limites.xMin && pos.y <= limites.yMax && pos.y >= limites.yMin;
        }
        public bool EnLimitesExclusivo(Rect limites)
        {
            return pos.x < limites.xMax && pos.x > limites.xMin && pos.y < limites.yMax && pos.y > limites.yMin;
        }
        public static bool operator ==(Point lhs, Point rhs)
        {
            return lhs.pos == rhs.pos;
        }
        public static bool operator !=(Point lhs, Point rhs)
        {
            return lhs.pos != rhs.pos;

        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override bool Equals(object o)
        {
            return true;
        }
    }

    [Serializable]
    public class Segmento
    {
        public bool show;
        public Point p1;
        public Point p2;


        public Point p1primo;
        public Point p2primo;

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

        public override int GetHashCode()
        {
            return 0;
        }

        public override bool Equals(object o)
        {
            return true;
        }

        public Segmento(Point p1, Point p2)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p1primo = new Point(-p1.pos,0,-1);
            this.p2primo = new Point(-p2.pos, 0, -1);
        }

        public static Vector3 ObtenerPuntoMedio(Point a, Point b)
        {
            return Vector3.Lerp(a.pos, b.pos, 0.5f);
        }

        public static Vector3 ObtenerPuntoMedioPorPeso(Point a, Point b)
        {
            return Vector3.Lerp(a.pos, b.pos, ((-a.weight + b.weight) / 2) - 0.5f);
        }
        public Vector3 ObtenerPuntoMedio()
        {
            return Vector3.Lerp(p1.pos, p2.pos, 0.5f);
        }
        public Vector3 ObtenerPuntoMedioModificadoPorPeso()
        {
            return Vector3.Lerp(p1.pos, p2.pos, MathF.Abs( ((-p1.weight + p2.weight) / 2) - 0.5f));
        }
    }
    [Serializable]
    public class FuncionLineal
    {

        public Point Punto1Primo;
        public Point Punto2Primo;
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
                return true;
            if (b.x-0.001f< x && x< b.x+0.001f)
            {
                return true;
            }
            return x == b.x;
        }
        /// <summary>
        /// Decir que se cortan, no toma en cuenta los puntos que conforman el segmento.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool SeCortan(Segmento a, Vector3 b)
        {
            FuncionLineal fL = new FuncionLineal(a.p1.pos, a.p2.pos);
            float x = fL.GetX(b.y);
            if (float.IsNaN(x))
                return true;
            if (b.x - 0.001f < x && x < b.x + 0.001f)
            {
                float maxX;
                float minX;
                float maxY;
                float minY;
                if (a.p1.pos.x>a.p2.pos.x)
                {
                    maxX = a.p1.pos.x;
                    minX = a.p2.pos.x;
                }
                else
                {
                    maxX = a.p2.pos.x;
                    minX = a.p1.pos.x;
                }

                if (a.p1.pos.y > a.p2.pos.y)
                {
                    maxY = a.p1.pos.y;
                    minY = a.p2.pos.y;
                }
                else
                {
                    maxY = a.p2.pos.y;
                    minY = a.p1.pos.y;
                }

                return b.x<maxX && b.x>minX && b.y>minY && b.y<maxY;
            }
            return x == b.x;
        }
    }


    public int NodesCuantity = 0;

    public List<Point> puntos = new List<Point>();

    public List<Segmento> segmentos = new List<Segmento>();

    public List<FuncionLineal> bizectrizesPerpendiculares = new List<FuncionLineal>();

    public List<Segmento> List_SegmentosLimites = new List<Segmento>();

    public List<Point> interseccionesMediatrices = new List<Point>();

    public List<Segmento> segmentosCortados = new List<Segmento>();

    public List<Segmento> segmentosCortadosExtra = new List<Segmento>();

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
        InitBizectrizPerpendicular();
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
        puntos = new List<Point>();
        for (int j = 0; j < NodesCuantity; j++)
            puntos.Add(new Point(GetRandomVec2InLimits(), UnityEngine.Random.Range(0.1f, 1), j));
    }
    private Vector2 GetRandomVec2InLimits()
    {
        return new Vector2(UnityEngine.Random.Range(limits.xMin, limits.xMax), UnityEngine.Random.Range(limits.yMin, limits.yMax));
    }

    public void InitSegmentos()
    {
        segmentos = new List<Segmento>();


        for (int i = 0; i < puntos.Count; i++)
        {
            for (int j = i+1; j < puntos.Count; j++)
            {
                Segmento edge = new Segmento(puntos[i], puntos[j]);
                if (!segmentos.Any(x => x == edge))
                    segmentos.Add(edge);
            }
        }
    }

    public void InitBizectrizPerpendicular()
    {
        bizectrizesPerpendiculares = new List<FuncionLineal>();

        for (int i = 0; i < segmentos.Count; i++)
        {
            Vector3 puntomedio;
            if (modeActual == mode.normal)
                puntomedio = segmentos[i].ObtenerPuntoMedio();
            else
                puntomedio = segmentos[i].ObtenerPuntoMedioModificadoPorPeso();
            float m = FuncionLineal.ObtenerPendienteMediatriz(segmentos[i].p1.pos, segmentos[i].p2.pos);
            FuncionLineal bizectrizPerpendicular = new FuncionLineal(puntomedio,m);
            bizectrizesPerpendiculares.Add(bizectrizPerpendicular);
        }
    }

    public void InitPuntosDeCorte()
    {
        interseccionesMediatrices = new List<Point>();
        for (int i = 0; i < bizectrizesPerpendiculares.Count; i++)
        {
            for (int j = i + 1; j < bizectrizesPerpendiculares.Count; j++)
            {
                Vector2 pos = FuncionLineal.SeCortan(bizectrizesPerpendiculares[i], bizectrizesPerpendiculares[j]);
                Point punto = new Point(pos, 0, -1);
                if (punto.EnLimitesInclusivo(limits))
                    if (!interseccionesMediatrices.Any(x => x == punto))
                        interseccionesMediatrices.Add(punto);
            }
            float x = bizectrizesPerpendiculares[i].GetX(limits.yMin);
            float y = limits.yMin;
            Vector3 pos2 = new Vector3(x, y);
            if (FuncionLineal.SeCortan(bizectrizesPerpendiculares[i],pos2))
            {
                Point punto = new Point(pos2);
                if (punto.EnLimitesInclusivo(limits))
                    if (!interseccionesMediatrices.Any(x => x == punto))
                        interseccionesMediatrices.Add(punto);
            }
            x = bizectrizesPerpendiculares[i].GetX(limits.yMax);
            y = limits.yMax;
            pos2 = new Vector3(x, y);
            if (FuncionLineal.SeCortan(bizectrizesPerpendiculares[i], pos2))
            {
                Point punto = new Point(pos2);
                if (punto.EnLimitesInclusivo(limits))
                    if (!interseccionesMediatrices.Any(x => x == punto))
                        interseccionesMediatrices.Add(punto);
            }
            x = limits.xMin;
            y = bizectrizesPerpendiculares[i].GetY(limits.xMin);
            pos2 = new Vector3(x, y);
            if (FuncionLineal.SeCortan(bizectrizesPerpendiculares[i], pos2))
            {
                Point punto = new Point(pos2);
                if (punto.EnLimitesInclusivo(limits))
                    if (!interseccionesMediatrices.Any(x => x == punto))
                        interseccionesMediatrices.Add(punto);
            }
            x = limits.xMax;
            y = bizectrizesPerpendiculares[i].GetY(limits.xMax);
            pos2 = new Vector3(x, y);
            if (FuncionLineal.SeCortan(bizectrizesPerpendiculares[i], pos2))
            {
                Point punto = new Point(pos2);
                if (punto.EnLimitesInclusivo(limits))
                    if (!interseccionesMediatrices.Any(x => x == punto))
                        interseccionesMediatrices.Add(punto);
            }
        }
    }

    public void InitSegmentosCortados()
    {
        segmentosCortados = new List<Segmento>();
        for (int i = 0; i < bizectrizesPerpendiculares.Count; i++)
        {
            for (int j = 0; j < interseccionesMediatrices.Count; j++)
            {
                if (FuncionLineal.SeCortan(bizectrizesPerpendiculares[i], interseccionesMediatrices[j].pos))
                {
                    for (int w = 0; w < interseccionesMediatrices.Count; w++)
                    {
                        if (j == w)
                            continue;
                        if (interseccionesMediatrices[j].EnLimitesExclusivo(limits) &&
                               interseccionesMediatrices[w].EnLimitesExclusivo(limits))
                        {
                            Segmento segmento = new Segmento(interseccionesMediatrices[j], interseccionesMediatrices[w]);
                            if (!segmentosCortados.Any(x => x == segmento))
                                segmentosCortados.Add(segmento);
                        }
                        if (interseccionesMediatrices[j].EnLimitesInclusivo(limits) &&
                            interseccionesMediatrices[w].EnLimitesExclusivo(limits))
                        {
                            Segmento segmento = new Segmento(interseccionesMediatrices[j], interseccionesMediatrices[w]);
                            if (!segmentosCortados.Any(x => x == segmento))
                                segmentosCortados.Add(segmento);
                        }
                        else if (interseccionesMediatrices[w].EnLimitesInclusivo(limits) &&
                                 interseccionesMediatrices[j].EnLimitesExclusivo(limits))
                        {
                            Segmento segmento = new Segmento(interseccionesMediatrices[j], interseccionesMediatrices[w]);
                            if (!segmentosCortados.Any(x => x == segmento))
                                segmentosCortados.Add(segmento);
                        }
                    }            
                }
            }
        }

    }

    public void InitDeleteSegmentosExtras()
    {
        segmentosCortadosExtra = new List<Segmento>();

        for (int i = 0; i < segmentos.Count; i++)
        {
            Point p1 = new Point(Vector3.one * 99999);
            Segmento segmentomascercano = new Segmento(p1,p1);
            Vector3 puntomedio;
            if(modeActual == mode.normal)
                puntomedio = segmentos[i].ObtenerPuntoMedio();
            else
                puntomedio = segmentos[i].ObtenerPuntoMedioModificadoPorPeso();
            float m = FuncionLineal.ObtenerPendienteMediatriz(segmentos[i].p1.pos, segmentos[i].p2.pos);
            FuncionLineal bizectrizPerpendicular = new FuncionLineal(puntomedio, m);
            for (int j = 0; j < segmentosCortados.Count; j++)
            {
                FuncionLineal bizectrizPerpendicular2 = new FuncionLineal(segmentosCortados[j].p1.pos, segmentosCortados[j].p2.pos);
                if (bizectrizPerpendicular == bizectrizPerpendicular2)
                {
                    if (FuncionLineal.SeCortan(segmentosCortados[j], puntomedio))
                    {
                        if (modeActual == mode.normal)
                        {
                            if (Vector3.Distance(puntomedio, segmentomascercano.ObtenerPuntoMedio()) >
                                Vector3.Distance(puntomedio, segmentosCortados[j].ObtenerPuntoMedio()))
                            {
                                segmentomascercano = segmentosCortados[j];
                            }
                        }
                        else
                        {
                            if (Vector3.Distance(puntomedio, segmentomascercano.ObtenerPuntoMedioModificadoPorPeso()) >
                                Vector3.Distance(puntomedio, segmentosCortados[j].ObtenerPuntoMedioModificadoPorPeso()))
                            {
                                segmentomascercano = segmentosCortados[j];
                            }
                        }
                    }
                    
                }
            }
            segmentosCortadosExtra.Add(segmentomascercano);
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
            if (puntos != null)
                for (int i = 0; i < puntos.Count; i++)
                {
                    Gizmos.color = colores.Initial_Nodes;
                    Gizmos.DrawWireSphere(puntos[i].pos, puntos[i].weight);
                    Handles.Label(puntos[i].pos, puntos[i].ID.ToString());
                }

        Gizmos.color = colores.Initial_Edges;
        if (b_drawSegments)
            if (segmentos != null)
                for (int i = 0; i < segmentos.Count; i++)
                {
                    Gizmos.DrawLine(segmentos[i].p1.pos, segmentos[i].p2.pos);
                }

        Gizmos.color = colores.Initial_Funciones;
        if (b_drawFunciones)
            if(bizectrizesPerpendiculares!=null)
                for (int i = 0; i < bizectrizesPerpendiculares.Count; i++)
                {
                    Gizmos.color = colores.Initial_Funciones;
                    if (bizectrizesPerpendiculares[i].m != 0 && !float.IsInfinity(bizectrizesPerpendiculares[i].m))
                    {
                        float yTop = 10;
                        float yDown = -10;
                    Gizmos.DrawLine(
                        new Vector3(bizectrizesPerpendiculares[i].GetX(yTop), yTop),
                        new Vector3(bizectrizesPerpendiculares[i].GetX(yDown), yDown));

                    }
                    else
                    {
                        if (bizectrizesPerpendiculares[i].m == 0)
                        {
                            if (modeActual == mode.normal)
                            {
                                Gizmos.DrawLine(
                                    new Vector3(limits.x               , segmentos[i].ObtenerPuntoMedio().y),
                                    new Vector3(limits.x + limits.width, segmentos[i].ObtenerPuntoMedio().y));
                            }
                            else
                            {
                                Gizmos.DrawLine(
                                    new Vector3(limits.x, segmentos[i].ObtenerPuntoMedioModificadoPorPeso().y),
                                    new Vector3(limits.x + limits.width, segmentos[i].ObtenerPuntoMedioModificadoPorPeso().y));
                            }
                            continue;
                        }
                        else //fijarse dentro de la funcion si se divide por 0, tiende a infinito, por ende pregunto esto.
                        {
                            if (modeActual == mode.normal)
                            {
                                Gizmos.DrawLine(
                                new Vector3(segmentos[i].ObtenerPuntoMedio().x, limits.y),
                                new Vector3(segmentos[i].ObtenerPuntoMedio().x, limits.y + limits.height));

                            }
                            else
                            {
                                Gizmos.DrawLine(
                                new Vector3(segmentos[i].ObtenerPuntoMedioModificadoPorPeso().x, limits.y),
                                new Vector3(segmentos[i].ObtenerPuntoMedioModificadoPorPeso().x, limits.y + limits.height));
                            }
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
            if (interseccionesMediatrices != null)
                for (int i = 0; i < interseccionesMediatrices.Count; i++)
                    Gizmos.DrawSphere(interseccionesMediatrices[i].pos, 0.1f);
        Gizmos.color = colores.Cortes;
        if (b_drawSegmentosCortados)
            if (segmentosCortadosExtra != null)
                for (int i = 0; i < segmentosCortadosExtra.Count; i++)
                {
                    if (segmentosCortadosExtra[i].show)
                    {
                        Gizmos.DrawLine(segmentosCortadosExtra[i].p1.pos, segmentosCortadosExtra[i].p2.pos);
                    }
                }
    }
}
