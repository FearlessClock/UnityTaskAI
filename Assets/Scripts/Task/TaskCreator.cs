using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskCreator : MonoBehaviour
{
    [SerializeField] private Human human = null;
    [SerializeField] private new Transform transform = null;
    [SerializeField] private RoomGraphHolder roomGraphHolder = null;
    public void CreateNewTask()
    {
        Vector3 newPos = (human.transform.position + Random.insideUnitSphere * 3).FlattenVector();
        transform.position = newPos;
        human.AddNewTask(new BasicTask("TaskCreator-"+this.name, TaskScope.Personal, transform, roomGraphHolder.FindRoomAtLocation(newPos), 10, Random.Range(2, 10), Random.Range(4, 7), true, 1, null, eAnimationType.Work));
    }
}
