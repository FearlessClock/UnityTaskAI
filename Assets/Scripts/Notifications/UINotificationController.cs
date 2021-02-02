using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINotificationController : MonoBehaviour
{
    [SerializeField] private NotificationController connectNotif = null;
    [SerializeField] private CanvasGroup canvasGroup = null;
    [SerializeField] private float notificationOffset = 1;

    [SerializeField] private float angleOffset = 90;
    private Canvas canvas = null;

    private new Camera camera = null;
    private new CameraController cameraController = null;
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private float cameraMoveToNotifOffset = 5;

    private void Awake()
    {
        camera = FindObjectOfType<Camera>();
        cameraController = FindObjectOfType<CameraController>();
        canvas = FindObjectOfType<Canvas>();
    }

    private void Update()
    {
        bool isVisible = connectNotif.target.IsVisibleFrom(camera);
        canvasGroup.alpha = isVisible ? 0 : 1;
        if (!isVisible)
        {
            Vector3 toTarget = (connectNotif.transform.position - camera.transform.position).normalized;
            Debug.DrawLine(camera.transform.position, camera.transform.position + toTarget * 10);

            float angle = (Vector3.SignedAngle(Vector3.forward, toTarget, Vector3.up) + angleOffset) * Mathf.Deg2Rad;
            float cameraAngle = Vector3.SignedAngle(Vector3.forward, camera.transform.forward.FlattenVectorY(), Vector3.up) * Mathf.Deg2Rad;

            this.transform.position = canvas.pixelRect.center + new Vector2(-Mathf.Cos(angle - cameraAngle), Mathf.Sin(angle - cameraAngle)) * notificationOffset;
        }
    }

    public void AddConnectedNotif(NotificationController notif)
    {
        connectNotif = notif;
    }

    public void MoveToFire()
    {
        Debug.Log(cameraController.ZoomPercentage);
        Vector3 vec =  connectNotif.transform.position + cameraController.transform.rotation*Vector3.forward * cameraController.ZoomPercentage * cameraMoveToNotifOffset;
        vec.y = camera.transform.position.y;

        cameraController.SlideTo(vec, moveDuration);
    }
}