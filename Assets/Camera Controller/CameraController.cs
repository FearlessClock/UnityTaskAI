using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CameraController : MonoBehaviour
{
    //--//--// > BEFORE USING THE SCRIPT PLEASE READ THE MESSAGE BELOW < \\--\\--\\

    // Everything that do something is commented on the code, you can change anything as you like
    // Ex: You dont like the scroll up and down with mouse scroll, then just go to line 106 and comment that code
    // I am commenting it because i think that some of people use Unity for fun and dont know how to code

    // Pré-requirements -> A camera in the scene
    // Tutorial on How to Use ->

    // STEP 1 - Put this script in any gameobject on your scene
    // Nice job, you are done

    // If you are trying to do an Online RTS you can use something like "if(!LocalPlayer) Destroy(gameObject)"
    // Hope you like

    // Editor Properties
    public Camera TheCamera = null;
    public LayerMask GroundLayer = 0;

    // Camera Properties
    public bool useDefaultSettings = false;
    [Range(8, 32)]
    public float cameraSpeed = 1;
    [Range(.8f, 20f)]
    public float cameraBorder = 1;

    // Minimun and maxium distance from the detected ground the Camera can be
    [Range(.8f, 32f)]
    public float cameraMinHeight = 1;
    [Range(.8f, 32f)]
    public float cameraMaxHeight = 1;

    // Map properties
    public float mapSize = 0;

    // Properties that shold not change to make sure the camera will work
    private float _savedCameraSpeed = 0;

    private RaycastHit _rayHit;
    private Vector2 _leftMouseInitial;
    private Vector2 _leftMouseFinal;
    private Vector3 _middleMouseInitial;
    [SerializeField] private float downwardsAngle = 45;
    private bool isRotating = false;

    private Vector3 pressedDownPosition;
    [SerializeField] private float dragSpeed = 1;

    void Start()
    {
        CheckSettings();
    }

    void Update()
    {
        Controller();
    }

    void CheckSettings()
    {
        if (useDefaultSettings)
        {
            TheCamera = Camera.main;
            cameraSpeed = 20;
            cameraBorder = 2f;
            cameraMinHeight = 2f;
            cameraMaxHeight = 20f;
            mapSize = float.MaxValue;
        }

        _savedCameraSpeed = cameraSpeed;
    }

    void Controller()
    {
        Vector3 position = TheCamera.transform.position;
        Vector3 rotation = new Vector3(downwardsAngle, TheCamera.transform.eulerAngles.y, 0);

        // W, A, S, D Movement
        if (Input.GetKey(KeyCode.W))
            position += new Vector3(TheCamera.transform.forward.x, 0, TheCamera.transform.forward.z) * (cameraSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.S))
            position -= new Vector3(TheCamera.transform.forward.x, 0, TheCamera.transform.forward.z) * (cameraSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.A))
            position -= new Vector3(TheCamera.transform.right.x, 0, TheCamera.transform.right.z) * (cameraSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.D))
            position += new Vector3(TheCamera.transform.right.x, 0, TheCamera.transform.right.z) * (cameraSpeed * Time.deltaTime);


        if (!isRotating)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                pressedDownPosition = Input.mousePosition; 
            }
            if(Input.GetKey(KeyCode.Mouse0))
            {
                Vector3 delta = Quaternion.Euler(0, rotation.y,0) * (Input.mousePosition - pressedDownPosition);
                delta = new Vector3(delta.x, 0, delta.y);
                position += delta * (dragSpeed * Time.deltaTime);
            }
            // Border Touch Movement
            if (Input.mousePosition.y >= Screen.height - cameraBorder && Input.mousePosition.y <= Screen.height + cameraBorder*2)
                position += new Vector3(TheCamera.transform.forward.x, 0, TheCamera.transform.forward.z) * (cameraSpeed * Time.deltaTime);
            if (Input.mousePosition.y <= 0 + cameraBorder && Input.mousePosition.y >= 0 - cameraBorder*2)
                position -= new Vector3(TheCamera.transform.forward.x, 0, TheCamera.transform.forward.z) * (cameraSpeed * Time.deltaTime);
            if (Input.mousePosition.x >= Screen.width - cameraBorder && Input.mousePosition.x <= Screen.width + cameraBorder*2)
                position += new Vector3(TheCamera.transform.right.x, 0, TheCamera.transform.right.z) * (cameraSpeed * Time.deltaTime);
            if (Input.mousePosition.x <= 0 + cameraBorder && Input.mousePosition.x >= 0 - cameraBorder*2)
                position -= new Vector3(TheCamera.transform.right.x, 0, TheCamera.transform.right.z) * (cameraSpeed * Time.deltaTime);
        }

        // Mouse Rotation
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            _middleMouseInitial = Input.mousePosition;
            isRotating = true;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse2))
        {
            isRotating = false;
        }

        if (Input.GetKey(KeyCode.Mouse2))
        {
            if (_middleMouseInitial.x - Input.mousePosition.x > 100 || _middleMouseInitial.x - Input.mousePosition.x < -100)
                rotation.y -= (_middleMouseInitial.x - Input.mousePosition.x) / cameraSpeed;
            if (_middleMouseInitial.y - Input.mousePosition.y > 100 || _middleMouseInitial.y - Input.mousePosition.y < -100)
                position += transform.up * -(_middleMouseInitial.y - Input.mousePosition.y) / 480;
        }

        // Mouse Scroll Zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        position.y -= scroll * 20;

        // Shift Acelleration
        if (Input.GetKey(KeyCode.LeftShift))
            cameraSpeed = (_savedCameraSpeed * 2f);
        else
            cameraSpeed = _savedCameraSpeed;

        // Effects when camera hit the ground or the top surface
        if (position.y <= cameraMinHeight + 1)
            position.y = cameraMinHeight;

        // Save Changes
        TheCamera.transform.position = Vector3.Slerp(TheCamera.transform.position, position, .8f);
        TheCamera.transform.eulerAngles = Vector3.Slerp(TheCamera.transform.eulerAngles, rotation, .2f);
    }
}
