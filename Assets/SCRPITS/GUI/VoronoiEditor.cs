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

        if (GUILayout.Button("1"))
            v.RandomNodes();
        if (GUILayout.Button("2"))
            v.InitSegmentos();
        if (GUILayout.Button("3"))
            v.InitFuncionLineales();
        if (GUILayout.Button("4"))
            v.InitPuntosDeCorte();
        if (GUILayout.Button("5"))
            v.InitCorte();
    }
    
}
