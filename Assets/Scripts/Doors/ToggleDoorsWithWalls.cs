using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleDoorsWithWalls : MonoBehaviour
{
    [SerializeField] private GameObject[] doors = null;
    [SerializeField] private GameObject walls = null;
    [SerializeField] private bool areWallsShowing = false;

    private void Awake()
    {
        ToggleDoors(areWallsShowing);
    }

    /// <param name="toggleValue">True to show doors, false to show walls</param>
    public void ToggleDoors(bool toggleValue)
    {
        if (toggleValue)
        {
            foreach (GameObject door in doors)
            {
                door.SetActive(true);
            }
            walls.SetActive(false);
        }
        else
        {
            foreach (GameObject door in doors)
            {
                door.SetActive(false);
            }
            walls.SetActive(true);
        }
    }
}
