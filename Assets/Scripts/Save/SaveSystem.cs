using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Save
{
    public class SaveSystem : MonoBehaviour
    {
        [SerializeField] private LevelGridGeneration gen = null;
        [SerializeField] private IntVariable currentNumberOfScientists = null;
        [SerializeField] private IntVariable totalNumberOfScientists = null;

        public void Save()
        {
            List<LevelBlock> blocks = new List<LevelBlock>();
            GridLevelSquareInformation[] gennedRooms = gen.GeneratedRooms;
            for (int i = 0; i < gennedRooms.Length; i++)
            {
                blocks.Add(new LevelBlock() { ID = gennedRooms[i].blockID, x = gennedRooms[i].RoomInfo.transform.position.x, y = gennedRooms[i].RoomInfo.transform.position.z });
            }
            Lab lab = new Lab() { blocks = blocks };
            lab.availableSpots = gen.GetAvailablePositions;
            lab.numberOfAliveScientists = currentNumberOfScientists.value;
            lab.numberOfScientists = totalNumberOfScientists.value;
            lab.maxBuildings = gen.MaxBuildings;
            System.IO.File.WriteAllText(Application.persistentDataPath + "/SaveData.json", JsonConvert.SerializeObject(lab));
            Debug.Log(Application.persistentDataPath + "/SaveData.json");
        }

        public Lab Load()
        {
            string json = System.IO.File.ReadAllText(Application.persistentDataPath + "/SaveData.json");
            return JsonConvert.DeserializeObject<Lab>(json);
        }
    }

    public class LevelBlock
    {
        public int ID;
        public float x;
        public float y;
    }

    public class Lab
    {
        public List<LevelBlock> blocks;
        public Vector2Int[] availableSpots;
        public int numberOfAliveScientists;
        public int numberOfScientists;
        public int maxBuildings;
    }
}