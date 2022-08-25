using System.Collections;
using System.Collections.Generic;
using UnityEngine;
class HpChangedCommand : ICommand
{
    public float Hp
    {
        get;
        set;
    }
}