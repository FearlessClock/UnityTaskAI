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
    public long lastActivatedTick = 0;

    public void Burn(float burnRate, long currentTick)
    {
        if (isOnFire)
        {
            currentFuel -= burnRate *(currentTick - lastActivatedTick);
        }
        else
        {
            fireResistance -= burnRate * (currentTick - lastActivatedTick);
        }
        lastActivatedTick = currentTick;
    }

    public void Smolder(float burnRate, long currentTick)
    {
        smolderTime -= burnRate * (currentTick - lastActivatedTick);
        lastActivatedTick = currentTick;
    }

    public override string ToString()
    {
        return "FB " + gridPoint.gridPosition + " FR " + fireResistance + " CF " + currentFuel + " ST " + smolderTime;
    }
}
