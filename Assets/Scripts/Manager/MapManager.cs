using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapPref
{
    public Vector2 rowCol;
    public Transform map;
}

public class MapManager : MonoBehaviour
{
    public static MapManager instance = null;

    [SerializeField] private List<MapPref> MapPrefs;
    [SerializeField] private Transform BottomLeft;
    [SerializeField] private Transform TopRight;

    public Dictionary<Vector2, Transform> MapPrefPair;
    // private Dictionary<Point, Vector3> pointPositionPair;

    void Awake()
    {
        if (instance != null && instance.gameObject.GetInstanceID() != gameObject.GetInstanceID())
        {
            Destroy(instance.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // pointPositionPair = new Dictionary<Point, Vector3>();
        MapPrefPair = new Dictionary<Vector2, Transform>();

        for (int i = 0; i < MapPrefs.Count; i++)
        {
            MapPrefPair[MapPrefs[i].rowCol] = MapPrefs[i].map;
        }
    }

    public Dictionary<Point, Vector3> GetMapPosition(int row, int col, ref Dictionary<Point, Vector3> pointPositionPair, out Vector2 sizeItem)
    {
        sizeItem = default;
        pointPositionPair.Clear();

        Transform map = MapPrefPair[new Vector2(row, col)];

        map.gameObject.SetActive(true);
        // Debug.Log("map.childCount:" + map.childCount);
        for (int i = map.childCount - 1; i >= 0; i--)
        {
            var mapLine = map.GetChild(i);
            // Debug.Log("mapLine.childCount:" + mapLine.childCount);

            for (int j = 0; j < mapLine.childCount; j++)
            {
                pointPositionPair[new Point(map.childCount - 1 - i, j)] = mapLine.GetChild(j).position;

                if (sizeItem == default)
                {
                    sizeItem = mapLine.GetChild(j).GetComponent<RectTransform>().sizeDelta;
                }
            }
        }
        map.gameObject.SetActive(false);

        return pointPositionPair;
    }

    public Vector3 GetFirstPosByMoveType(MoveTypeAtStart moveType, Vector3 target)
    {
        Vector3 pos = target;
        switch (moveType)
        {
            case MoveTypeAtStart.Bottom:
                pos.x = target.x;
                pos.y = BottomLeft.position.y;
                break;
            case MoveTypeAtStart.Top:
                pos.x = target.x;
                pos.y = TopRight.position.y;
                break;
            case MoveTypeAtStart.Right:
                pos.y = target.y;
                pos.x = TopRight.position.x;
                break;
            case MoveTypeAtStart.Left:
                pos.y = target.y;
                pos.x = BottomLeft.position.x;
                break;
            case MoveTypeAtStart.TopLeft:
                pos.x = -TopRight.position.x;
                pos.y = TopRight.position.y;
                break;
            case MoveTypeAtStart.TopRight:
                pos.x = TopRight.position.x;
                pos.y = TopRight.position.y;
                break;
            case MoveTypeAtStart.BottomLeft:
                pos.x = BottomLeft.position.x;
                pos.y = BottomLeft.position.y;
                break;
            case MoveTypeAtStart.BottomRight:
                pos.x = -BottomLeft.position.x;
                pos.y = BottomLeft.position.y;
                break;
        }
        return pos;
    }
}
