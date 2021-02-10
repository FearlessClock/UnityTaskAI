using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleDoorsWithWalls : MonoBehaviour
{
    [SerializeField] private GameObject[] doors = null;
    [SerializeField] private GameObject walls = null;
    [SerializeField] private bool areWallsHidden = false;
    [SerializeField] private Collider2D col = null;

    private void Awake()
    {
        ToggleDoors(areWallsHidden);
    }

    /// <param name="toggleValue">True to show doors, false to show walls</param>
    public void ToggleDoors(bool toggleValue)
    {
        areWallsHidden = toggleValue;
        if (toggleValue)
        {
            foreach (GameObject door in doors)
            {
                door.SetActive(true);
                col.enabled = true;
            }
            walls.SetActive(false);
        }
        else
        {
            foreach (GameObject door in doors)
            {
                door.SetActive(false);
                col.enabled = false;
            }
            walls.SetActive(true);
        }
    }
}
