using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : InteractableObject
{
    [SerializeField] private float bedComfort = 1;

    public override BasicTask GenerateTask()
    {
        return new BasicTask("Bed-" + this.name, TaskScope.Personal, interactionPoint, GetContainedRoom(), 10, 5, 10, false, 1, null, eAnimationType.Sleep, this) ;
    }

    public override bool Work(PersonBase workingPerson)
    {
        return true;
    }
}
