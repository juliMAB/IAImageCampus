using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FSM : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] private int currentState;
    [SerializeField] static private int[,] relations = null;
    [SerializeField] static public List<Package<int>> relationsList;
    [SerializeField] static public List<Package2<string>> relationsListStrign;
    [SerializeField] static private Dictionary<int, List<UnityEvent>> behaviours2;
    [SerializeField] private List<Package3<List<UnityEvent>>> BeahaversRelated;

    //lo que esta haciendo.
    public enum States
    {
        Mining,
        GoToMine,
        GoToDeposit,
        Idle,



        _Count
    }

    //la condicion que utiliza para cambiar de estado.
    public enum Flags
    {
        OnFullInventory,
        OnReachMine,
        OnReachDeposit,
        OnEmpyMine,



        _Count
    }

    private void OnValidate()
    {
        if (relations == null)
        {
            ResetRelations();
        }

        
    }
    // A package to store our stuff
    [System.Serializable]
    public struct Package<TElement>
    {
        public int Index0;
        public int Index1;
        public TElement Element;
        public Package(int idx0, int idx1, TElement element)
        {
            Index0 = idx0;
            Index1 = idx1;
            Element = element;
        }
    }
    [System.Serializable]
    public struct Package3<TElement>
    {
        public States state;
        public TElement Element;
        public Package3(States idx0, TElement element)
        {
            state = idx0;
            Element = element;
        }
    }
    [System.Serializable]
    public struct Package2<TElement>
    {
        public TElement Index0;
        public TElement Index1;
        public TElement Element;
        public Package2(TElement idx0, TElement idx1, TElement element)
        {
            Index0 = idx0;
            Index1 = idx1;
            Element = element;
        }
    }



    public void ResetRelations()
    {
            currentState = -1;
            relations = new int[(int)States._Count, (int)Flags._Count];
            for (int i = 0; i < (int)States._Count; i++)
                for (int j = 0; j < (int)Flags._Count; j++)
                    relations[i, j] = -1;

            behaviours2 = new Dictionary<int, List<UnityEvent>>();
    }
    public void ForceCurretState(int state)
    {
        currentState = state;
    }

    public void SetRelation(int sourceState, int flag, int destinationState)
    {
        relations[sourceState, flag] = destinationState;
    }

    public void SetFlag(int flag)
    {
        if (relations[currentState, flag] != -1)
            currentState = relations[currentState, flag];
    }

    public int GetCurrentState()
    {
        return currentState;
    }

    public void SetBehaviour(int state, UnityEvent behaviour)
    {
        List<UnityEvent> newBehaviours = new List<UnityEvent>();
        newBehaviours.Add(behaviour);

        if (behaviours2.ContainsKey(state))
            behaviours2[state] = newBehaviours;
        else
            behaviours2.Add(state, newBehaviours);
    }

    public static void AddBehaviour(int state, UnityEvent behaviour)
    {

        if (behaviours2.ContainsKey(state))
            behaviours2[state].Add(behaviour);
        else
        {
            List<UnityEvent> newBehaviours = new List<UnityEvent>();
            newBehaviours.Add(behaviour);
            behaviours2.Add(state, newBehaviours);
        }
    }

    public void Update()
    {
        if (behaviours2.ContainsKey(currentState))
        {
            List<UnityEvent> actions = behaviours2[currentState];
            if (actions != null)
            {
                for (int i = 0; i < actions.Count; i++)
                {
                    if (actions[i] != null)
                    {
                        actions[i].Invoke();
                    }
                }
            }
        }
    }

    public void OnBeforeSerialize()
    {
        // Convert our unserializable array into a serializable list
        if (relations == null) return;
        relationsList = new List<Package<int>>();
        relationsListStrign = new List<Package2<string>>();
        for (int i = 0; i < relations.GetLength(0); i++)
        {
            for (int j = 0; j < relations.GetLength(1); j++)
            {
                relationsList.Add(new Package<int>(i, j, relations[i, j]));
                relationsListStrign.Add(new Package2<string>(Enum.GetName(typeof(States), i), Enum.GetName(typeof(Flags), j), Enum.GetName(typeof(States), relations[i, j])));
            }
        }
        if (behaviours2.Count < 1) return;
        BeahaversRelated = new List<Package3<List<UnityEvent>>>();
        foreach (KeyValuePair<int, List<UnityEvent>> item in behaviours2)
        {
            BeahaversRelated.Add(new Package3<List<UnityEvent>>((States)item.Key, new List<UnityEvent>(behaviours2[item.Key])));
        }
    }
                

    public void OnAfterDeserialize()
    {
        // Convert the serializable list into our unserializable array
        if (relations == null) return;
        foreach (var package in relationsList)
        {
            relations[package.Index0, package.Index1] = package.Element;
        }
    }


    [Header("Select_To_Set_Relation_And_Press_Relate")]
    [SerializeField] private States sourceState;
    [SerializeField] private Flags  flag;
    [SerializeField] private States destinationState;
    [Header("Select_To_Set_Beheaver_And_Press_Relate2")]
    [SerializeField] private States sourceState2;
    [SerializeField] private UnityEvent Beheaver;


    public void SetRelation2()
    {
        SetRelation((int)sourceState, (int)flag, (int)destinationState);
    }
    public  void SetBeheaver2()
    {
        AddBehaviour((int)sourceState2, Beheaver);
    }
}
