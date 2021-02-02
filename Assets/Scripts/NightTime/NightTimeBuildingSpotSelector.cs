using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightTimeBuildingSpotSelector : MonoBehaviour
{

    private new Camera camera = null;
    [SerializeField] private LayerMask availableSpotsLayer = 0;
    public Action<NightTimeAvailableSpotController> OnClickedOnSpot;
    private bool isChecking = false;

    private float timer = 0;
    [SerializeField] private float maxClickTime = 1;

    private void Awake()
    {
        camera = FindObjectOfType<Camera>();
    }

    private void Update()
    {
        if (isChecking)
        {
            if (Input.GetMouseButtonDown(0))
            {
                timer = maxClickTime;
            }
            if(timer > 0)
            {
                timer -= Time.deltaTime;
            }
            if (Input.GetMouseButtonUp(0) && timer > 0)
            {
                RaycastHit hit;
                bool res = Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, 1000, availableSpotsLayer, QueryTriggerInteraction.Collide);
                if (res)
                {
                    OnClickedOnSpot?.Invoke(hit.collider.gameObject.GetComponent<NightTimeAvailableSpotController>());
                }
            }
        }
    }

    public void StartChecking()
    {
        isChecking = true;
    }
    public void StopChecking()
    {
        isChecking = false;
    }
}
