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
    private void Awake()
    {
        camera = FindObjectOfType<Camera>();
        canvas = FindObjectOfType<Canvas>();
    }

    private void Update()
    {
        canvasGroup.alpha = connectNotif.target.IsVisibleFrom(camera) ? 0 : 1;
        Vector3 toTarget = (connectNotif.transform.position - camera.transform.position).normalized;
        Debug.DrawLine(camera.transform.position, camera.transform.position + toTarget * 10);

        float angle = (Vector3.SignedAngle(Vector3.forward, toTarget, Vector3.up) + angleOffset) * Mathf.Deg2Rad;
        float cameraAngle = Vector3.SignedAngle(Vector3.forward, camera.transform.forward.FlattenVectorY(), Vector3.up) * Mathf.Deg2Rad;

        this.transform.position = canvas.pixelRect.center + new Vector2(-Mathf.Cos(angle - cameraAngle), Mathf.Sin(angle - cameraAngle)) * notificationOffset;
    }

    public void AddConnectedNotif(NotificationController notif)
    {
        connectNotif = notif;
    }
}
