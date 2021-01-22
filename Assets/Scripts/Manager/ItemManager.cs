using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance = null;

    private int row, col;
    private Dictionary<Point, Vector3> pointPositionPair;
    private Dictionary<Point, Item> pointItemPair;

    void Awake()
    {
        if (instance != null && instance.gameObject.GetInstanceID() != gameObject.GetInstanceID())
        {
            Destroy(instance.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Setup(int row, int col, Dictionary<Point, Vector3> pointPositionPair, Dictionary<Point, Item> pointItemPair)
    {
        this.row = row;
        this.col = col;
        this.pointPositionPair = pointPositionPair;
        this.pointItemPair = pointItemPair;
    }

    private void MoveToEmpty(Item item, Point p1, Point p2)
    {
        var firstPointPos = pointPositionPair[p1];
        var targetPointPos = pointPositionPair[p2];

        var matrix = ItemController.Instance.GetMatrix();
        pointItemPair[p1] = null;
        matrix[p1.x, p1.y] = -1;

        pointItemPair[p2] = item;
        item.point = p2;
        matrix[p2.x, p2.y] = item.index;
        item.SetTargetPosition(firstPointPos, targetPointPos, 10);
    }

    public void Swap(int i, int j, int i1, int j1)
    {
        var matrix = ItemController.Instance.GetMatrix();

        var p1 = new Point(i, j);
        var p2 = new Point(i1, j1);

        var item1 = pointItemPair[p1];
        var posItem1 = item1.transform.position;

        var item2 = pointItemPair[p2];
        var posItem2 = item2.transform.position;

        pointItemPair[p1] = item2;
        item2.point = p1;
        item2.transform.position = posItem1;
        matrix[p1.x, p1.y] = item2.index;

        pointItemPair[p2] = item1;
        item1.point = p2;
        item1.transform.position = posItem2;
        matrix[p2.x, p2.y] = item1.index;

        item1.Reset4AnglePos();
        item2.Reset4AnglePos();
    }

    public void HideItem(Item item)
    {
        var matrix = ItemController.Instance.GetMatrix();
        pointItemPair[item.point] = null;
        matrix[item.point.x, item.point.y] = -1;
    }

    public void MoveInGame(MoveTypeInGame moveTypeInGame, int i, int j, int i1, int j1)
    {
        switch (moveTypeInGame)
        {
            case MoveTypeInGame.BottomTop:
                MoveBottomTop(i, j, i1, j1);
                break;
            case MoveTypeInGame.TopBottom:
                MoveTopBottom(i, j, i1, j1);
                break;
            case MoveTypeInGame.RightLeft:
                MoveRightLeft(i, j, i1, j1);
                break;
            case MoveTypeInGame.LeftRight:
                MoveLeftRight(i, j, i1, j1);
                break;
            case MoveTypeInGame.ColCenter:
                MoveColCenter(i, j, i1, j1);
                break;
            case MoveTypeInGame.RowCenter:
                MoveRowCenter(i, j, i1, j1);
                break;
        }
    }

    private void MoveTopBottom(int i, int j, int i1, int j1)
    {
        for (int k = 1; k <= row; k++)
        {
            if (k > i)
            {
                MoveTopBottomOne(i, k, j);
            }
        }

        for (int k = 1; k <= row; k++)
        {
            if (k > i1)
            {
                MoveTopBottomOne(i1, k, j1);
            }
        }
    }

    private void MoveTopBottomOne(int i, int k, int j)
    {
        var firstPoint = new Point(k, j);
        var item = pointItemPair[firstPoint];

        // Debug.Log("firstPoint:" + " i:" + k + " j: " + j);

        if (item == null) return;

        for (int l = i; l < k; l++)
        {
            var targetPoint = new Point(l, j);
            var item2 = pointItemPair[targetPoint];

            // Debug.Log("i:" + l + " j: " + j + " value:" + (item2 == null));

            if (item2 == null)
            {
                MoveToEmpty(item, firstPoint, targetPoint);
                // firstPoint = targetPoint;
                break;
            }
        }
    }

    private void MoveBottomTop(int i, int j, int i1, int j1)
    {
        for (int k = i - 1; k >= 1; k--)
        {
            MoveBottomTopOne(i, k, j);
        }
        for (int k = i1 - 1; k >= 1; k--)
        {
            MoveBottomTopOne(i1, k, j1);
        }
    }

    private void MoveBottomTopOne(int i, int k, int j)
    {
        var firstPoint = new Point(k, j);
        var item = pointItemPair[firstPoint];

        if (item == null) return;
        for (int l = i; l > k; l--)
        {
            var targetPoint = new Point(l, j);
            var item2 = pointItemPair[targetPoint];

            if (item2 == null)
            {
                MoveToEmpty(item, firstPoint, targetPoint);
                break;
            }
        }
        // for (int l = k + 1; l <= i; l++)
        // {
        //     var targetPoint = new Point(l, j);
        //     var item2 = pointItemPair[targetPoint];

        //     if (item2 == null)
        //     {
        //         MoveToEmpty(item, firstPoint, targetPoint);
        //         firstPoint = targetPoint;
        //     }
        // }
    }

    private void MoveRightLeft(int i, int j, int i1, int j1)
    {
        for (int k = j + 1; k <= col; k++)
        {
            MoveRightLeftOne(i, k, j);
        }

        for (int k = j1 + 1; k <= col; k++)
        {
            MoveRightLeftOne(i1, k, j1);
        }
    }

    private void MoveRightLeftOne(int i, int k, int j)
    {
        // Debug.Log("MoveRightLeftOne:" + i + " " + k + " " + j);

        var firstPoint = new Point(i, k);
        var item = pointItemPair[firstPoint];

        if (item == null) return;

        for (int l = j; l <= k; l++)
        {
            var targetPoint = new Point(i, l);
            var item2 = pointItemPair[targetPoint];

            // Debug.Log("WTF");

            if (item2 == null)
            {
                MoveToEmpty(item, firstPoint, targetPoint);
                break;
            }
        }
    }

    private void MoveLeftRight(int i, int j, int i1, int j1)
    {
        for (int k = j - 1; k >= 1; k--)
        {
            MoveLeftRightOne(i, k, j);
        }

        for (int k = j1 - 1; k >= 1; k--)
        {
            MoveLeftRightOne(i1, k, j1);
        }
    }

    private void MoveLeftRightOne(int i, int k, int j)
    {
        var firstPoint = new Point(i, k);
        var item = pointItemPair[firstPoint];

        Debug.Log("first:" + firstPoint);

        if (item == null) return;

        for (int l = j; l > k; l--)
        {
            var targetPoint = new Point(i, l);
            var item2 = pointItemPair[targetPoint];
            
            Debug.Log("targetPoint:" + targetPoint);

            if (item2 == null)
            {
                MoveToEmpty(item, firstPoint, targetPoint);
                break;
            }
        }
    }

    private void MoveColCenter(int i, int j, int i1, int j1)
    {
        var p1 = new Point(i, j);
        var p2 = new Point(i1, j1);

        Point minJ = p1, maxJ = p2;
        if (minJ.y > maxJ.y)
        {
            minJ = p2;
            maxJ = p1;
        }

        for (int k = minJ.y - 1; k >= 1; k--)
        {
            MoveLeftRightOne(minJ.x, k, minJ.y);
        }

        for (int k = maxJ.y + 1; k <= col; k++)
        {
            MoveRightLeftOne(maxJ.x, k, maxJ.y);
        }
    }

    private void MoveRowCenter(int i, int j, int i1, int j1)
    {
        var p1 = new Point(i, j);
        var p2 = new Point(i1, j1);

        Point minI = p1, maxI = p2;
        if (minI.x > maxI.x)
        {
            minI = p2;
            maxI = p1;
        }

        for (int k = minI.x - 1; k >= 1; k--)
        {
            MoveBottomTopOne(minI.x, k, minI.y);
        }

        for (int k = maxI.x + 1; k <= row; k++)
        {
            MoveTopBottomOne(maxI.x, k, maxI.y);
        }
    }

    public List<Vector3> PointsToPositions(List<Point> points, out List<Vector3> allCenterPos)
    {
        allCenterPos = new List<Vector3>();

        for (int i = 0; i < points.Count; i++)
        {
            var pos = pointPositionPair[points[i]];

            if (i < points.Count - 1)
            {
                if (points[i].x == points[i + 1].x)
                {
                    var minY = points[i].y;
                    var maxY = points[i + 1].y;
                    if (minY > maxY)
                    {
                        minY = points[i + 1].y;
                        maxY = points[i].y;
                    }
                    for (int k = minY; k <= maxY; k++)
                    {
                        var p = new Point(points[i].x, k);
                        pos = pointPositionPair[p];
                        if (!allCenterPos.Contains(pos))
                            allCenterPos.Add(pos);
                    }
                }
                else if (points[i].y == points[i + 1].y)
                {
                    var minX = points[i].x;
                    var maxX = points[i + 1].x;
                    if (minX > maxX)
                    {
                        minX = points[i + 1].x;
                        maxX = points[i].x;
                    }
                    for (int k = minX; k <= maxX; k++)
                    {
                        var p = new Point(k, points[i].y);
                        pos = pointPositionPair[p];
                        if (!allCenterPos.Contains(pos))
                            allCenterPos.Add(pos);
                    }
                }
            }
        }

        if (points.Count == 2)
        {
            if (
                (points[0].x == points[1].x
                && Math.Abs(points[0].y - points[1].y) <= 1)
                || (points[0].y == points[1].y
                && Math.Abs(points[0].x - points[1].x) <= 1)
            )
                return null;
        }

        // all center pos ignor( first, last )
        List<Vector3> result = new List<Vector3>();

        for (int i = 0; i < points.Count; i++)
        {
            var pos = pointPositionPair[points[i]];
            Point prevPoint = default;
            if (i == 0) prevPoint = points[i + 1];
            if (i == points.Count - 1) prevPoint = points[i - 1];

            if (i == 0 || i == points.Count - 1)
            {
                if (prevPoint.x == points[i].x)
                {
                    if (prevPoint.y > points[i].y)
                        pos += (pointPositionPair[new Point(points[i].x, points[i].y + 1)] - pos) / 2;
                    else
                        pos -= (pos - pointPositionPair[new Point(points[i].x, points[i].y - 1)]) / 2;
                }
                else if (prevPoint.y == points[i].y)
                {
                    if (prevPoint.x > points[i].x)
                        pos += (pointPositionPair[new Point(points[i].x + 1, points[i].y)] - pos) / 2;
                    else
                        pos -= (pos - pointPositionPair[new Point(points[i].x - 1, points[i].y)]) / 2;
                }
            }
            result.Add(pos);
        }

        return result;
    }

}
