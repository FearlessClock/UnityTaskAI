using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalDoorController : MonoBehaviour
{
    private new Camera camera;
    [SerializeField] private LayerMask doorMask = 0;
    Collider2D[] results = null;
    [SerializeField] private float touchRadius = 1;

    private void Start()
    {
        camera = FindObjectOfType<Camera>();
        results = new Collider2D[10];
    }

    private void Update()
    {
        if (InputManager.Instance.InputExistsUp() && InputManager.Instance.IsMouseFree)
        {
            int size = Physics2D.OverlapCircleNonAlloc(camera.ScreenToWorldPoint(InputManager.Instance.GetInput(0)), touchRadius, results, doorMask);
            if (size > 0)
            {
                for (int i = 0; i < size; i++)  
                {
                    results[i].GetComponent<DoorController>().ToggleDoor();
                }
            }
        }
    }
}
