using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationGlobalController : MonoBehaviour
{
    [SerializeField] private LayerMask notificationMask = 0;
    [SerializeField] private NotificationController notification = null;
    [SerializeField] private UINotificationController uINotificationPrefab = null;
    [SerializeField] private float notificationOffset = 1;
    [SerializeField] private Canvas canvas = null;
    private new Camera camera = null;

    private List<NotificationController> notifs = null;

    private void Awake()
    {
        camera = FindObjectOfType<Camera>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray clickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(clickRay, 1000, notificationMask);
            for (int i = 0; i < hits.Length; i++)
            {
                hits[i].collider.GetComponent<NotificationController>().DestroyNotification();
            }
        }
    }

    public void SpawnNotification(Vector3 eventPosition)
    {
        NotificationController notif = Instantiate<NotificationController>(notification, eventPosition + Vector3.up * notificationOffset, Quaternion.identity);
        UINotificationController notifUI = Instantiate<UINotificationController>(uINotificationPrefab, canvas.transform);
        notifUI.AddConnectedNotif(notif);
        notif.AddUINotif(notifUI);
    }
}
