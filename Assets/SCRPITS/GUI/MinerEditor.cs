using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(FSM))]
public class MinerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
       FSM miner = (FSM)target;
       
       DrawDetails(miner);
        //
        //EditorGUILayout.Space();
        DrawRelation(miner);
    }

    static void DrawDetails(FSM miner)
    {
       EditorGUILayout.Space();

       EditorGUILayout.LabelField("Actual State:");
       EditorGUILayout.LabelField(Enum.GetName(typeof(FSM.States), miner.GetCurrentState()));
    }
    void DrawRelation(FSM miner)
    {
        if (GUILayout.Button("relate"))
            miner.SetRelation2();
        if (GUILayout.Button("relate2"))
            miner.SetBeheaver2();
    }
}