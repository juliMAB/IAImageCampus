using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private float hp = 30f;
    [SerializeField] private Mediator mediator;


    // Update is called once per frame
    void Update()
    {
        hp -= Time.deltaTime * 2f;
        HpChangedCommand cmd = new HpChangedCommand();
        cmd.Hp = hp;
        mediator.Publish(cmd);
    }
}
