using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionPoint : MonoBehaviour
{
    private RoomInformation containedRoom = null;

    private void Start()
    {
        containedRoom = GetComponentInParent<RoomInformation>();
    }

    public void StartFire()
    {
        if (!containedRoom.IsOnFire)
        {
            containedRoom.StartFire();
        }
    }

    public Vector3 FireStartLocation => containedRoom.GetCenterVertex.Position;
}
