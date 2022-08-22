using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Miner : MonoBehaviour
{

    enum States
    {
        Mining,
        GoToMine,
        GoToDeposit,
        Idle,



        _Count
    }

    enum Flags
    {
        OnFullInventory,
        OnReachMine,
        OnReachDeposit,
        OnEmpyMine,



        _Count
    }

    public GameObject mine;
    public GameObject deposit;

    private float speed = 10.0f;
    private float miningTime = 5.0f;
    private float currentMiningTime = 0.0f;
    private int mineUses = 10;

    private FSM fsm;
    // Start is called before the first frame update
    public Vector3 V3Anterior;
    private Vector3 V3Calculado;
    public Vector3 V3Deposito;
    public Vector3 V3Mina;
    void Start()
    {
        V3Mina = mine.transform.position;
        V3Deposito = deposit.transform.position;
        fsm = new FSM((int)States._Count, (int)Flags._Count);
        fsm.ForceCurretState((int)States.GoToMine);

        fsm.SetRelation((int)States.GoToMine, (int)Flags.OnReachMine, (int)States.Mining);
        fsm.SetRelation((int)States.Mining, (int)Flags.OnFullInventory, (int)States.GoToDeposit);
        fsm.SetRelation((int)States.GoToDeposit, (int)Flags.OnReachDeposit, (int)States.GoToMine);
        fsm.SetRelation((int)States.GoToDeposit, (int)Flags.OnEmpyMine, (int)States.Idle);

        fsm.AddBehaviour((int)States.Idle, () => { Debug.Log("Idle"); });

        fsm.AddBehaviour((int)States.Mining, () =>
        {
            if (currentMiningTime < miningTime)
            {
                currentMiningTime += Time.deltaTime;
            }
            else
            {
                currentMiningTime = 0.0f;
                fsm.SetFlag((int)Flags.OnFullInventory);
                mineUses--;
            }
        });
        fsm.AddBehaviour((int)States.Mining, () =>{ Debug.Log("Mining"); });

        fsm.AddBehaviour((int)States.GoToMine, () =>
        {
            if (V3Anterior != V3Calculado)
            {
                transform.position = V3Calculado;
            }
            else
            {
                fsm.SetFlag((int)Flags.OnReachMine);
            }
            V3Anterior = transform.position;
        });
        fsm.AddBehaviour((int)States.GoToMine, () => { Debug.Log("GoToMine"); });

        fsm.AddBehaviour((int)States.GoToDeposit, () =>
        {
            if (V3Anterior != V3Calculado)
            {
                transform.position = V3Calculado;
            }
            else
            {
                if (mineUses <= 0)
                    fsm.SetFlag((int)Flags.OnEmpyMine);
                else
                    fsm.SetFlag((int)Flags.OnReachDeposit);
            }
            V3Anterior = transform.position;
        });
        fsm.AddBehaviour((int)States.GoToDeposit, () => { Debug.Log("GoToDeposit"); });

    }

    // Update is called once per frame
    public void MyUpdate()
    {
        fsm.Update();
    }

    public void CalcularNuevaPos(Vector3 tPos, Vector3 deposito,Vector3 mina ,float deltaTime)
    {
        Vector2 dir;
        switch (fsm.GetCurrentState())
        {
            case (int)States.GoToMine:
                dir = (mina - tPos).normalized;

                if (Vector2.Distance(mina, tPos) > 1.0f)
                {
                    Vector2 movement = dir * 10.0f * deltaTime;
                    V3Calculado = tPos + new Vector3(movement.x, movement.y);
                }
                break;
            case (int)States.GoToDeposit:
                dir = (deposito - tPos).normalized;

                if (Vector2.Distance(deposito, tPos) > 1.0f)
                {
                    Vector2 movement = dir * 10.0f * deltaTime;
                    V3Calculado = tPos + new Vector3(movement.x, movement.y);
                }
                break;
        }
        
    }
}
