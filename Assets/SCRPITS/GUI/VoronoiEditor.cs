using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Voronoi2))]
public class VoronoiEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Voronoi2 v = (Voronoi2)target;

        if (GUILayout.Button("RandomNodes"))
            v.RandomNodes();
        if (GUILayout.Button("InitSegmentos"))
            v.InitSegmentos();
        if (GUILayout.Button("InitBizectrizPerpendicular"))
            v.InitBizectrizPerpendicular();
        if (GUILayout.Button("InitPuntosDeCorte"))
            v.InitPuntosDeCorte();
        if (GUILayout.Button("InitSegmentosCortados"))
            v.InitSegmentosCortados();
        if (GUILayout.Button("InitDeleteSegmentosExtras"))
            v.InitDeleteSegmentosExtras();
    }
    
}
