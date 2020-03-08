using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskCreator : MonoBehaviour
{
    [SerializeField] private Human human = null;
    [SerializeField] private Transform transform = null;
    public void CreateNewTask()
    {
        Vector3 newPos = (human.transform.position + Random.insideUnitSphere * 3).FlattenVector();
        transform.position = newPos;
        human.AddNewTask(new TaskBase(transform, 10, Random.Range(2, 10), Random.Range(4, 7), eAnimationType.Work));
    }
}
