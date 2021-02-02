using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Human))]
public class ScientistScoringController : MonoBehaviour
{
    private MovementHandler scientistMovement = null;
    [SerializeField] private float scorePerDoor = 1;

    private void Awake()
    {
        scientistMovement = GetComponent<MovementHandler>();
        scientistMovement.OnPassedThroughDoor += GetPointFromDoor;
    }

    private void OnDestroy()
    {
        scientistMovement.OnPassedThroughDoor -= GetPointFromDoor;
    }

    private void GetPointFromDoor(int combo)
    {   
        ScoreController.instance.UpdateScore(scorePerDoor * combo);
    }

}
