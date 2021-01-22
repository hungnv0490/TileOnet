using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public static Test instance;

    const int MAX_MAP = 3;

    Dictionary<int, int> maxItemTypeMap = new Dictionary<int, int>()
    {
        {1, 10},//15, 10
        {2, 15},//24, 15
        {3, 20}//35, 20
    };

    Dictionary<int, int> totalItemMap = new Dictionary<int, int>()
    {
        {1, 30},
        {2, 48},
        {3, 70}
    };

    public Dictionary<int, Level> levelPairs = new Dictionary<int, Level>();

    void Awake()
    {
        instance = this;
        Init();
    }

    void Init()
    {
        string str = "";
        int itemType = -1;
        int mapType = 0;

        var itemTypeNames = Enum.GetNames(typeof(ItemType));
        Dictionary<int, int> mapTime = new Dictionary<int, int>()
        {
            {1, 130},
            {2, 160},
            {3, 190}
        };
        Dictionary<int, int> mapMaxItem = new Dictionary<int, int>()
        {
            {1, 5},
            {2, 5},
            {3, 4}
        };

        int cycle = 0;
        for (int level = 1; level <= 434; level++)
        {
            mapType++;
            itemType++;

            // level++;

            if (level <= itemTypeNames.Length)
            {
                if (mapType == 3) mapType = 1;
            }
            if (mapType > MAX_MAP) mapType = 1;
            if (itemType >= itemTypeNames.Length)
            {
                itemType = 0;
                if (mapMaxItem[1] < 15)
                    mapMaxItem[1]++;
                if (mapMaxItem[2] < 24)
                    mapMaxItem[2]++;
                if (mapMaxItem[3] < 35)
                    mapMaxItem[3]++;
                cycle++;
                if (cycle >= 2)
                {
                    if (mapMaxItem[1] < 15)
                        mapTime[1] += 4;
                    if (mapMaxItem[2] < 24)
                        mapTime[2] += 4;
                    if (mapMaxItem[3] < 35)
                        mapTime[3] += 4;
                }
            }
            // Get(itemType, out int tmpMapType, out int itemTypeAmount, out int totalTime);
            // Debug.Log("level:" + level + " mapType:" + mapType + " itemType:" + itemType + " itemTypeAmount:"
            //             + mapMaxItem[mapType] + " totalTime:" + mapTime[mapType]);
            // str += level + "," + tmpMapType + "," + itemType + "," + itemTypeAmount + "," + totalTime + "|";

            levelPairs.Add(level, new Level()
            {
                id = level,
                itemType = itemType,
                mapType = mapType,
                maxItemAmount = mapMaxItem[mapType],
                totalSec = mapTime[mapType]
            });

            // yield return null;
        }
        // Debug.Log(str);
    }
}
