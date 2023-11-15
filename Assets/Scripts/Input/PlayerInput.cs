using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : IUserInput
{

    [Header("----- Mouse Setting -----")]
    public bool mouseEnable = false;
    public float mouseSensitivityX = 1.0f;
    public float mouseSensitivityY = 1.0f;


    private void Update()
    {

        if (mouseEnable)
        {
            _PlayerCameraControllerFromMouse();
        }else
        {
            Jup = targetJ.y;
            Jright = targetJ.x;
        }

        Dup = Mathf.SmoothDamp(Dup, targetD.y, ref velocityDup, 0.1f);
        Dup = Mathf.Abs(Dup) < 0.005f ? 0 : Dup;
        Dright = Mathf.SmoothDamp(Dright, targetD.x, ref velocityDright, 0.1f);
        Dright = Mathf.Abs(Dright) < 0.005f ? 0 : Dright;

        

        //检测是否能够移动
        if (!inputEnabled)
        {
            targetD = Vector2.zero;
        }

        Dmag = Mathf.Sqrt(Dup * Dup + Dright * Dright);
        Dvec = Dright * transform.right + Dup * transform.forward;

        //触发功能
        KeyTriggerFunction(newJump,ref lastJump,ref jump);

        KeyTriggerFunction(newAttack, ref lastAttack, ref attack);

        KeyTriggerFunction(newDefense, ref lastDefense, ref defense);
        
    }

    #region 按键信号接收
    public void _PlayerRunning(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            isRunning = true;
        } else if (value.canceled)
        {
            isRunning = false;
        }
    }
    public void _PlayerWalking(InputAction.CallbackContext value)
    {
        targetD = value.ReadValue<Vector2>();
    }
    public void _PlayerJumping(InputAction.CallbackContext value)
    {
        if(value.started)
        {
            newJump = true;
        }else if (value.canceled)
        {
            newJump = false;
        }
    }
    public void _PlayerCameraControllerFromKey(InputAction.CallbackContext value)
    {
        targetJ = value.ReadValue<Vector2>();
    }
    public void _PlayerAttacking(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            newAttack = true;
        } else if (value.canceled)
        {
            newAttack = false;
        }
    }
    public void _playerDefense(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            newDefense = true;
        } else if (value.performed)
        {
            defending = true;
        } else if (value.canceled)
        {
            newDefense = false;
            defending = false;
        }

    }
    //旧输入方法
    public void _PlayerCameraControllerFromMouse()
    {
        Jup = Input.GetAxis("Mouse Y") * 2.5f * mouseSensitivityY;
        Jright = Input.GetAxis("Mouse X") * 3f * mouseSensitivityX;
    }



    #endregion
}
