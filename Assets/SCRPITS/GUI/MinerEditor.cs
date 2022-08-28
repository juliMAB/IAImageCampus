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
        DrawButtons(miner);

        DrawRelations(miner);
    }

    static void DrawDetails(FSM miner)
    {
       EditorGUILayout.Space();

       EditorGUILayout.LabelField("Actual State:");
       EditorGUILayout.LabelField(Enum.GetName(typeof(FSM.States), miner.GetCurrentState()));
    }
    void DrawButtons(FSM miner)
    {
        if (GUILayout.Button("relate"))
            miner.SetRelation2();
        if (GUILayout.Button("relate2"))
            miner.SetBeheaver2();
    }
    void DrawRelations(FSM miner)
    {
        EditorGUILayout.LabelField("RELACIONES:");
        for (int i = 0; i < FSM.relationsList.Count; i++)
            if (FSM.relationsList[i].Element != -1)
            
                EditorGUILayout.LabelField(
                                    FSM.relationsListStrign[i].Index0 +
                    " Cambia a " +  FSM.relationsListStrign[i].Index1 +
                    " Cuando " +    FSM.relationsListStrign[i].Element);
            

    }
}