using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationController : MonoBehaviour
{
    [SerializeField] private GameObject onDestroyParticleSystemPrefab = null;
    [SerializeField] private GameObject connectUI = null;
    public Renderer target = null;

    public void DestroyNotification()
    {
        Instantiate<GameObject>(onDestroyParticleSystemPrefab, this.transform.position, Quaternion.identity);
        Destroy(this.gameObject);
        Destroy(connectUI);
    }

    public void AddUINotif(UINotificationController notifUI)
    {
        connectUI = notifUI.gameObject;
    }
}
