using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class cameraController : MonoBehaviour
{
    public float horizontalSpeed = 20.0f;
    public float verticalSpeed = 20.0f;
    public float cameraDamp = 0.5f;

    private IUserInput playerInput;
    private GameObject playerHandle;
    private GameObject cameraHandle;
    private float tempEulerx;
    private GameObject model;
    private new GameObject camera;

    private Vector3 cameraDampVelocity;

    // Start is called before the first frame update
    private void Awake()
    {
        cameraHandle = transform.parent.gameObject;
        playerHandle = cameraHandle.transform.parent.gameObject;
        tempEulerx = 20;
        ActorController ac = playerHandle.GetComponent<ActorController>();
        model = ac.model;
        playerInput = ac.playInput;
        camera = Camera.main.gameObject;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {

        Vector3 tempModelEuler = model.transform.eulerAngles; 

        playerHandle.transform.Rotate(Vector3.up, playerInput.Jright * horizontalSpeed * Time.fixedDeltaTime);
        tempEulerx -= playerInput.Jup * -verticalSpeed * Time.deltaTime;
        cameraHandle.transform.localEulerAngles = new Vector3(
            Mathf.Clamp(tempEulerx, -40, 30), 0, 0);
        model.transform.eulerAngles = tempModelEuler;

        camera.transform.position = Vector3.Lerp(camera.transform.position,transform.position,0.2f);

        camera.transform.position = 
            Vector3.SmoothDamp(camera.transform.position,
                               transform.position,
                               ref cameraDampVelocity,
                               cameraDamp);

        //camera.transform.eulerAngles = transform.eulerAngles;
        camera.transform.LookAt(cameraHandle.transform);
    }
}
