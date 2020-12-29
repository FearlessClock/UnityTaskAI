using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalDoorController : MonoBehaviour
{
    private new Camera camera;
    [SerializeField] private LayerMask doorMask = 0;

    private void Start()
    {
        camera = FindObjectOfType<Camera>();
    }

    private void Update()
    {
        if (InputManager.Instance.InputExistsUp() && InputManager.Instance.IsMouseFree)
        {
            RaycastHit[] results = new RaycastHit[10];
            int size = Physics.RaycastNonAlloc(camera.ScreenPointToRay(InputManager.Instance.GetInput(0)), results, 1000, doorMask);
            if (size > 0)
            {
                for (int i = 0; i < size; i++)  
                {
                    results[i].collider.GetComponent<DoorController>().ToggleDoor();
                }
            }
        }
    }
}
