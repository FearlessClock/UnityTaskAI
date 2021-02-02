using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Pieter.Nighttime
{
    [RequireComponent(typeof(Button))]
    public class NightTimeBuildingButtonController : MonoBehaviour
    {
        private Button button = null;
        [SerializeField] private Image icon = null;
        private GridLevelSquareInformation building = null;
        public GridLevelSquareInformation Building => building;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        public void SetBuilding(GridLevelSquareInformation building)
        {
            icon.sprite = building.sprite;
            this.building = building;
            button.interactable = true;
        }

        public void SetCallback(UnityAction action)
        {
            button.onClick.AddListener(action);
        }

        public void RemoveCallback(UnityAction action)
        {
            button.onClick.RemoveListener(action);
        }

        public void DeactivateButton()
        {
            button.interactable = false;
        }
    }
}
