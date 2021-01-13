using Pieter.Grid;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBlock
{
    public float fireResistance = 1;
    public float smolderTime = 1;
    public float maxFuel = 1;
    public float currentFuel = 1;
    public float currentOxygen = 1;
    public GridPoint gridPoint = null;
    public bool isOnFire = false;

    public bool IsBurnedOut => currentFuel <= 0;
    public bool CanLightOnFire => fireResistance <= 0;
    public bool IsSmoldered => smolderTime <= 0;

    public void Burn(float burnRate)
    {
        if (isOnFire)
        {
            currentFuel -= burnRate;
        }
        else
        {
            fireResistance -= burnRate;
        }
    }

    public void Smolder(float burnRate)
    {
        smolderTime -= burnRate;
    }
}
