using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PararelExample : MonoBehaviour
{
    [SerializeField] List<Miner> fsmList = new List<Miner>();
    [SerializeField] ConcurrentBag<Miner> mineros = new ConcurrentBag<Miner>();
    ParallelOptions parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = 8 };

    public bool ConMultytrheting=false;

    private void Start()
    {
        for (int i = 0; i < fsmList.Count; i++)
            mineros.Add(fsmList[i]);
    }

    private void Update()
    {
        if (ConMultytrheting)
        {
            float tD = Time.deltaTime;
            Parallel.ForEach(mineros, parallelOptions, (val) =>
            {
                val.CalcularNuevaPos(val.V3Anterior, val.V3Deposito, val.V3Mina, tD);
            });
            foreach (var val in mineros)
            {
                val.MyUpdate();
            }
            return;
        }
        else
        {
            foreach (var val in mineros)
            {
                val.CalcularNuevaPos(val.V3Anterior, val.V3Deposito, val.V3Mina, Time.deltaTime);
                val.MyUpdate();
            }
        }
        
    }
}
