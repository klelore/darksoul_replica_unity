using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IUserInput : MonoBehaviour
{
    [Header("----- Output signals -----")]
    public float Dup;
    public float Dright;
    public float Jup;
    public float Jright;
    public float Dmag;
    public Vector3 Dvec;
    public Vector2 targetD;
    public Vector2 targetJ;

    //判断状态
    public bool isRunning;
    public bool jump;
    public bool attack;
    public bool defense;
    public bool defending;

    protected bool lastJump;
    protected bool newJump;

    protected bool lastAttack;
    protected bool newAttack;

    protected bool newDefense;
    protected bool lastDefense;


    [Header("----- Others -----")]
    public float velocityDup;
    public float velocityDright;
    public bool inputEnabled = true;//是否能进行输入

    protected Vector2 SquareToCircle(Vector2 input)
    {
        Vector2 output = Vector2.zero;

        output.x = input.x * Mathf.Sqrt(1 - input.y * input.y * 0.5f);
        output.y = input.y * Mathf.Sqrt(1 - input.x * input.x * 0.5f);

        return output;
    }
    protected void KeyTriggerFunction(bool newSignal, ref bool lastSignal, ref bool signal)
    {
        signal = (newSignal != lastSignal && newSignal) ? true : false;
        lastSignal = newSignal;
    }
}
