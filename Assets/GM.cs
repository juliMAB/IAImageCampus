using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : MonoBehaviour
{
    public static GM instance = null;

    public float DeltaTime;

    private void Awake()
    {
        singleton();
    }

    private void singleton()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public static GM Instance
    {
        get { return instance; }
    }
    private void Update()
    {
        DeltaTime = Time.deltaTime;
    }

    public bool xd(Vector3 minePos, Vector3 localPos)
    {
        Vector2 dir = (minePos - localPos).normalized;
        if (Vector2.Distance(minePos, localPos) > 1.0f)
        {
            Vector2 movement = dir * 10.0f * GM.Instance.DeltaTime;
            localPos += new Vector3(movement.x, movement.y);
            return true;
        }
        return false;
    }
}
