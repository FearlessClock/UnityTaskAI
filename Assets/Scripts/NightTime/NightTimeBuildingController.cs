using HelperScripts.EventSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Pieter.Nighttime
{
    [RequireComponent(typeof(NightTimeBuildingSpotSelector))]
    public class NightTimeBuildingController : MonoBehaviour
    {
        public static NightTimeBuildingController instance = null;
        [SerializeField] private LevelGridGeneration levelGridGeneration = null;


        private Vector2Int[] availablePositions = null;
        [SerializeField] private NightTimeAvailableSpotController spotControllerPrefab = null;
        private NightTimeAvailableSpotController[] spotControllers = null;
        private bool isNighttimeBuilding = false;
        private NightTimeBuildingSpotSelector nightTimeBuildingSpotSelector = null;
        private NightTimeAvailableSpotController lastControllerSelected = null;
        [Space]
        [SerializeField] private RoomPrefabsList roomPrefabs = null;
        private NightTimeBuildingButtonController[] buttonController = null;
        [SerializeField] private EventObjectScriptable OnCanBuildRoom = null;
        [SerializeField] private EventScriptable OnBuiltRoom = null;
        private GridLevelSquareInformation selectedRoom = null;


        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }
            nightTimeBuildingSpotSelector = GetComponent<NightTimeBuildingSpotSelector>();
        }

        public void BuildButtonController(NightTimeBuildingButtonController controller)
        {
            isNighttimeBuilding = true;
            nightTimeBuildingSpotSelector.StartChecking();
            OnCanBuildRoom?.Call(false);
            nightTimeBuildingSpotSelector.OnClickedOnSpot += SelectSpot;
            selectedRoom = controller.Building;
            GetAvailableBuildPositionsForRoom(controller.Building);
        }

        public void BuildRoom()
        {
            levelGridGeneration.CreateNewRoom(lastControllerSelected.GridPoint, selectedRoom);
            StopNighttimeBuilding();
            OnBuiltRoom?.Call();
        }

        public void StartNighttimeBuilding()
        {
            buttonController = FindObjectsOfType<NightTimeBuildingButtonController>();
            int indexRoom1 = Random.Range(0, roomPrefabs.Length);
            int indexRoom2 = Random.Range(0, roomPrefabs.Length);
            if(indexRoom1 == indexRoom2)
            {
                if (indexRoom2 > 0) indexRoom2--;
                else indexRoom2++;
            }
            buttonController[0].SetBuilding(roomPrefabs.gridLevels[indexRoom1]);
            buttonController[1].SetBuilding(roomPrefabs.gridLevels[indexRoom2]);
            for (int i = 0; i < buttonController.Length; i++)
            {
                NightTimeBuildingButtonController cont = buttonController[i];
                buttonController[i].SetCallback(() 
                    => BuildButtonController(cont));
            }
        }

        public void StopNighttimeBuilding()
        {
            isNighttimeBuilding = false;
            nightTimeBuildingSpotSelector.StopChecking();
            nightTimeBuildingSpotSelector.OnClickedOnSpot -= SelectSpot;
            for (int i = 0; i < buttonController.Length; i++)
            {
                buttonController[i].DeactivateButton();
            }
            for (int i = 0; i < spotControllers.Length; i++)
            {
                Destroy(spotControllers[i].gameObject);
            }
            spotControllers = null;
        }

        private void GetAvailableBuildPositions()
        {
            availablePositions = levelGridGeneration.GetAvailablePositions;
            spotControllers = new NightTimeAvailableSpotController[availablePositions.Length];
            for (int i = 0; i < availablePositions.Length; i++)
            {
                spotControllers[i] = Instantiate<NightTimeAvailableSpotController>(spotControllerPrefab, this.transform);
                Vector3 pos = LevelGridGeneration.Vec2IntToVec3D(availablePositions[i] * levelGridGeneration.GetTileSize);
                spotControllers[i].SetPosition(pos, availablePositions[i]);
            }
        }

        private void GetAvailableBuildPositionsForRoom(GridLevelSquareInformation building)
        {
            availablePositions = levelGridGeneration.GetAvailablePositions;
            Vector2Int[] newRoomDirections = building.RoomInfo.EntrancePoints.Directions;
            GridLevelSquareInformation[] rooms = levelGridGeneration.GeneratedRooms;
            if(spotControllers != null)
            {
                for (int i = 0; i < spotControllers.Length; i++)
                {
                    Destroy(spotControllers[i].gameObject);
                }
            }
            List<NightTimeAvailableSpotController> newSpots = new List<NightTimeAvailableSpotController>();
            List<Vector2Int> validPositions = new List<Vector2Int>();
            for (int i = 0; i < availablePositions.Length; i++)
            {
                for (int j = 0; j < newRoomDirections.Length; j++)
                {
                    Vector2Int pos = availablePositions[i] + newRoomDirections[j];
                    for (int k = 0; k < rooms.Length; k++)
                    {
                        if (!validPositions.Contains(availablePositions[i]) && rooms[k].gridPoint.Equals(pos))
                        {
                            validPositions.Add(availablePositions[i]);
                            NightTimeAvailableSpotController spot = Instantiate<NightTimeAvailableSpotController>(spotControllerPrefab, this.transform);
                            Vector3 position = LevelGridGeneration.Vec2IntToVec3D(availablePositions[i] * levelGridGeneration.GetTileSize);
                            spot.SetPosition(position, availablePositions[i]);
                            newSpots.Add(spot);
                        }
                    }
                }
            }

            spotControllers = newSpots.ToArray();
            availablePositions = validPositions.ToArray();
        }


        private void SelectSpot(NightTimeAvailableSpotController controller)
        {
            if (lastControllerSelected != null)
            {
                lastControllerSelected.UpdateLineRendererColor(false);
            }
            controller.UpdateLineRendererColor(true);
            OnCanBuildRoom?.Call(true);
            lastControllerSelected = controller;
        }
    }

}
