using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class quill18creates : MonoBehaviour
{
    bool isRunning = false;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start() :: Starting.");
        SlowJob();
        Debug.Log("Start() :: Done.");
    }

    // Update is called once per frame
    void Update()
    {
        if (isRunning)
        {
            Debug.Log("SlowJob isRuning");
        }
    }

    void SlowJob()
    {
        isRunning = true;

        Debug.Log("ExampleScript::SlowJob() -- Doing 1000 things, each taking 2ms.");

        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        for (int i = 0; i < 1000; i++)
        {
            Thread.Sleep(2);
        }

        sw.Stop();

        Debug.Log("ExampleScript::SlowJob() -- Done! Elapsed time: " + sw.ElapsedMilliseconds / 1000F);

        isRunning = false;
    }
}
