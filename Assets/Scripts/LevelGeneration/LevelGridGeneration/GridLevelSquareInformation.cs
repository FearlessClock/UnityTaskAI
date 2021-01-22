using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLevelSquareInformation : MonoBehaviour
{
    [SerializeField] private RoomInformation roomInformation = null;
    public RoomInformation RoomInfo => roomInformation;

    public int blockID;
}
