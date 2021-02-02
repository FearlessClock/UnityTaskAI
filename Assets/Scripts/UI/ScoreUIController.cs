using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh = null;
    private void Start()
    {
        OnScoreUpdate();
        ScoreController.instance.OnScoreUpdate += OnScoreUpdate;
    }

    private void OnScoreUpdate()
    {
        textMesh.SetText(ScoreController.instance.GetScore.ToString());
    }
}
