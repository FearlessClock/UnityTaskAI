using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractableObject))]
public class InteractableObjectExplosion : MonoBehaviour
{
    [Range(0,1)]
    [SerializeField] private float chanceToExplode = 1;
    private FireController fireController = null;
    [SerializeField] private RoomInformation room = null;
    private InteractableObject interactableObject = null;

    private void Awake()
    {
        fireController = FindObjectOfType<FireController>();
        interactableObject = GetComponent<InteractableObject>();
    }

    public void CheckIfExplode()
    {
        if (interactableObject.IsWorking && Random.value < chanceToExplode)
        {
            fireController.StartFire(this.transform.position, room);
            interactableObject.ExplodeObject();
        }
    }
}
