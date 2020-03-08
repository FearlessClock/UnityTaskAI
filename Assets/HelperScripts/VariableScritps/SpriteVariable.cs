using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteVariable", menuName = "UnityHelperScripts/SpriteVariable", order = 0)]
public class SpriteVariable : ScriptableObject
{
    public Sprite value;

    public static implicit operator Sprite(SpriteVariable reference)
    {
        return reference.value;
    }

    public void SetValue(Sprite v)
    {
        this.value = v;
    }

    public override string ToString()
    {
        return value.name;
    }
}