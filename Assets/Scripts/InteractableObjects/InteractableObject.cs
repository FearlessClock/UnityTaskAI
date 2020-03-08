using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] protected Transform interactionPoint = null;

    public virtual TaskBase GenerateTask()
    {
        return new TaskBase(interactionPoint, 10, 10, 10, eAnimationType.Work, this);
    }

    public virtual bool Work(PersonBase workingPerson) { return true; }
}
