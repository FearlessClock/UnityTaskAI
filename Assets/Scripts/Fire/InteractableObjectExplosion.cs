using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractableObject))]
public class InteractableObjectExplosion : MonoBehaviour
{
    [Range(0,10)]
    [SerializeField] private float chanceToExplode = 1;
    private FireController fireController = null;
    [SerializeField] private RoomInformation room = null;
    private InteractableObject interactableObject = null;

    private void Start()
    {
        fireController = FindObjectOfType<FireController>();
        interactableObject = GetComponent<InteractableObject>();
    }

    public void CheckIfExplode()
    {
        if(fireController == null)
        {
            fireController = FindObjectOfType<FireController>();
        }
        float rand = Random.Range(0f, 10f);
        rand = Random.Range(0f, 10f);
        if (interactableObject.IsWorking && rand < chanceToExplode)
        {
            fireController.StartFire(this.transform.position, room);
            NotificationGlobalController.instance.SpawnNotification(this.transform.position);
            interactableObject.ExplodeObject();
        }
    }
}
