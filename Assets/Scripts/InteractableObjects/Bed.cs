using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : InteractableObject
{
    [SerializeField] private float bedComfort = 1;

    public override TaskBase GenerateTask()
    {
        return new TaskBase(interactionPoint, 10, 10, 10, eAnimationType.Sleep, this);
    }

    public override bool Work(PersonBase workingPerson)
    {
        workingPerson.FillTiredness(bedComfort);
        return true;
    }
}
