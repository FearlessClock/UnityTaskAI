using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth = 10;
    private AnimationCommandController animationCommandController = null;
    private float health = 0;

    public bool IsAlive => health > 0;
     
    private void Awake()
    {
        health = startingHealth;
        animationCommandController = GetComponent<AnimationCommandController>();
    }


    public void LoseHealth(float value)
    {
        health -= value;
        if (!IsAlive)
        {
            animationCommandController.ChangeState(eAnimationType.Death);
        }
    }
}
