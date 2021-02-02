using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pieter.Nighttime
{
    [RequireComponent(typeof(NightTimeBuildingController))]
    public class NightTimeLoader : MonoBehaviour
    {
        [SerializeField] private GameObject gameplayUI = null;
        [SerializeField] private GameObject nighttimeUI = null;
        private NightTimeBuildingController nightTimeBuildingController = null;

        private void Awake()
        {
            nightTimeBuildingController = GetComponent<NightTimeBuildingController>();
        }

        private void OnEnable()
        {
            DayCycleController.OnEndOfDay += OnStartEndGame;
        }

        private void OnDisable()
        {
            DayCycleController.OnEndOfDay -= OnStartEndGame;
        }

        public void OnStartEndGame()
        {
            gameplayUI.SetActive(false);
            nighttimeUI.SetActive(true);
            nightTimeBuildingController.StartNighttimeBuilding();
        }

        public void OnStartNewDay()
        {
            gameplayUI.SetActive(true);
            nighttimeUI.SetActive(false);
        }
    }

}
