using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pieter.Nighttime
{
    public class NightTimeBuildSelectedRoomButton : MonoBehaviour
    {
        [SerializeField] private Button button = null;
        public void OnCanBuild(object val)
        {
            button.interactable = (bool)val;
        }

        public void BuildRoom()
        {
            NightTimeBuildingController.instance.BuildRoom();
        }
    }
}
