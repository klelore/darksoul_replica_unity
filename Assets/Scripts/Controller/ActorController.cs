using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActorController : MonoBehaviour
{
    enum AnimatorLayers
    {
        movement,
        attack,
    }

    [Header("-----Dehavioral Setting-----")]
    public GameObject model;
    public IUserInput playInput;
    public float walkSpeed = 2.0f;
    public float runMultiplier = 2.0f;
    public float jumpVelocity = 5.0f;
    public float rollVelocity = 1.0f;

    [Space(10)]
    [Header("-----Friction Setting-----")]
    public PhysicMaterial frictionZero;
    public PhysicMaterial frictionOne;

    private Animator anim;
    private Rigidbody rb;//玩家刚体
    private Vector3 movingVec;
    private Vector3 thrustVec;
    private bool canAttack;
    private CapsuleCollider col;
    private float lerpTarget;
    private Vector3 deltaPos;

    private bool lockPlaner = false;
    public InputActionReference[] keyboardAction;

    private void Awake()
    {
        IUserInput[] inputs = GetComponents<IUserInput>();
        foreach (IUserInput input in inputs)
        {
            if(input.enabled==true)
            {
                playInput = input;
                break;
            }
        }
        anim = model.GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
        if(rb == null )
        {
            Debug.Log("你没有装Rigidbody组件");
        }
    }

    private void Update()
    {
        SetAnimations();

        //设置角色平滑转弯
        if (playInput.Dmag > 0.1f)
        {
            model.transform.forward = Vector3.Slerp(model.transform.forward, playInput.Dvec, 0.3f);
        }

        if (!lockPlaner)
        {
            movingVec = playInput.Dmag *
                        model.transform.forward *
                        walkSpeed *
                        ((playInput.isRunning) ? runMultiplier : 1.0f);
        }
    }
    private void FixedUpdate()
    {
        rb.position += deltaPos;
        rb.velocity = new Vector3(movingVec.x, rb.velocity.y, movingVec.z) + thrustVec;
        thrustVec = Vector3.zero;
        deltaPos = Vector3.zero;
    }

    private bool CheckState(string stateName,int stateLayer = ((int)AnimatorLayers.movement))
    {
        return anim.GetCurrentAnimatorStateInfo(stateLayer).IsName(stateName);
    }
    private void SetAnimations()
    {
        float targetRunMulti = ((playInput.isRunning) ? 2.0f : 1.0f);

        anim.SetFloat("forward", playInput.Dmag * Mathf.Lerp(anim.GetFloat("forward"), targetRunMulti, 0.4f));
        anim.SetBool("defense", playInput.defending);


        if (rb.velocity.magnitude > 1.0f)
        {
            anim.SetTrigger("roll");
        }
        if (playInput.jump)
        {
            anim.SetTrigger("jump");
            canAttack = false;
        }
        if (playInput.attack && CheckState("ground") && canAttack)
        {
            anim.SetTrigger("attack");
        }
    }

    #region 接受动画信号
    public void OnJumpEnter()
    {
        foreach (var key in keyboardAction)
        {
            key.action.Disable();
        }
        lockPlaner = true;
        thrustVec = new Vector3(0, jumpVelocity, 0);
    }
    public void IsGround()
    {
        anim.SetBool("isGround", true);
    }
    public void IsNotGround()
    {
        anim.SetBool("isGround", false);
    }
    public void OnGroundEnter()
    {
        foreach (var key in keyboardAction)
        {
            key.action.Enable();
        }
        lockPlaner = false;
        canAttack = true;
        col.material = frictionOne;
    }
    public void OnGroundExit()
    {
        col.material = frictionZero;
    }
    public void OnFallEnter()
    {
        foreach (var key in keyboardAction)
        {
            key.action.Disable();
        }
        lockPlaner = true;
    }
    public void OnRollEnter()
    {
        foreach (var key in keyboardAction)
        {
            key.action.Disable();
        }
        lockPlaner = true;
        thrustVec = new Vector3(0, rollVelocity, 0);
    }
    public void OnJabEnter()
    {
        foreach (var key in keyboardAction)
        {
            key.action.Disable();
        }
        lockPlaner = true;
    }
    public void OnJabUpdate()
    {
        thrustVec = model.transform.forward * anim.GetFloat("jabVelocity");
    }
    public void OnAttack1hAEnter()
    {
        foreach (var key in keyboardAction)
        {
            key.action.Disable();
        }
        //lockPlaner = true;
        lerpTarget = 1.0f;
    }
    public void OnAttack1hAUpdate()
    {
        thrustVec = model.transform.forward * anim.GetFloat("attack1hAVelocity");
        anim.SetLayerWeight(
            anim.GetLayerIndex("attack"), 
            Mathf.Lerp(anim.GetLayerWeight(anim.GetLayerIndex("attack")), 
            lerpTarget, 0.2f)
            );
    }
    public void OnAttackIdleEnter()
    {
        foreach (var key in keyboardAction)
        {
            key.action.Enable();
        }
        //lockPlaner = false;
        lerpTarget = 0;
    }
    public void OnAttackIdleUpdate()
    {
        anim.SetLayerWeight(
            anim.GetLayerIndex("attack"),
            Mathf.Lerp(anim.GetLayerWeight(anim.GetLayerIndex("attack")),
            lerpTarget, 0.2f)
        );
    }

    public void OnUpdateRM(object _deltaPos)
    {
        if (CheckState("attack1hC",((int)AnimatorLayers.attack)))
        {
            deltaPos += (0.8f * deltaPos + 0.2f * (Vector3)_deltaPos) * 1.0f;
        }        
    }
    #endregion
}
