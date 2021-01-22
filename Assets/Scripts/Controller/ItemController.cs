
using System;
using System.Collections.Generic;
using UnityEngine;

public struct Point
{
    public int x { get; set; }
    public int y { get; set; }

    public Point(int x1, int y1)
    {
        x = x1;
        y = y1;
    }

    public static bool operator ==(Point per1, Point per2)
    {
        return per1.x == per2.x && per1.y == per2.y;
    }

    public static bool operator !=(Point per1, Point per2)
    {
        return per1.x != per2.x || per1.y != per2.y;
    }

    public override bool Equals(object obj)
    {
        return obj is Point point &&
               x == point.x &&
               y == point.y;
    }

    public override int GetHashCode()
    {
        int hashCode = 1861411795;
        hashCode = hashCode * -1521134295 + x.GetHashCode();
        hashCode = hashCode * -1521134295 + y.GetHashCode();
        return hashCode;
    }

    public override string ToString()
    {
        return $"({x},{y})";
    }
}

public class MyLine
{
    public Point P1 { get; set; }
    public Point P2 { get; set; }

    public MyLine(Point p1, Point p2)
    {
        P1 = p1;
        P2 = p2;
    }
}

public class ItemController
{
    public static ItemController Instance = new ItemController();

    private int[,] matrix;
    private List<Point> points;
    private int row, col;

    public ItemController()
    {
        points = new List<Point>();
    }

    // maxItemCount by level
    public void CreateMatrix(int nrow, int ncol, int itemCount, int maxItemCount)
    {
        points.Clear();
        row = nrow;
        col = ncol;

        matrix = new int[nrow + 2, ncol + 2];
        for (int i = 0; i < nrow + 2; i++)
        {
            for (int j = 0; j < ncol + 2; j++)
            {
                // Debug.Log("CreateMatrix:" + i + " " + j);
                matrix[i, j] = -1;
                if (i != 0 && j != 0 && i != nrow + 1 && j != ncol + 1)
                    points.Add(new Point(i, j));
            }
        }

        List<int> totalItemIndexs = new List<int>();
        List<int> itemIndexs = new List<int>();
        for (int i = 0; i < itemCount; i++)
        {
            totalItemIndexs.Add(i);
        }

        if (itemCount > maxItemCount)
        {
            for (int i = 0; i < maxItemCount; i++)
            {
                int index = totalItemIndexs[UnityEngine.Random.Range(0, totalItemIndexs.Count)];
                totalItemIndexs.Remove(index);

                itemIndexs.Add(index);
            }
        }
        else itemIndexs = totalItemIndexs;

        Dictionary<int, int> arr = new Dictionary<int, int>();
        for (int i = 0; i < itemIndexs.Count; i++)
        {
            arr[itemIndexs[i]] = 0;
        }

        int nRowCol = nrow * ncol;
        var maxOneItem = (int)(nRowCol / itemIndexs.Count) + 1;
        if (maxOneItem % 2 != 0) maxOneItem = (int)(nRowCol / itemIndexs.Count) + 2;

        Debug.Log("itemCount:" + itemCount + "maxItemCount:" + maxItemCount + " maxOneItem:" + maxOneItem);

        int k = 0;
        do
        {
            int index = itemIndexs[UnityEngine.Random.Range(0, itemIndexs.Count)];

            if (arr[index] < maxOneItem)
            {
                arr[index] += 2;
                for (int j = 0; j < 2; j++)
                {
                    try
                    {
                        int size = points.Count;
                        int pointIndex = UnityEngine.Random.Range(0, size);

                        matrix[points[pointIndex].x, points[pointIndex].y] = index;
                        points.RemoveAt(pointIndex);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning(ex.StackTrace);
                    }
                }
                k++;
            }
        } while (k < nRowCol / 2);
    }

    public int[,] GetMatrix()
    {
        return matrix;
    }

    // same row
    private bool CheckLineX(int y1, int y2, int x)
    {
        int min = Math.Min(y1, y2);
        int max = Math.Max(y1, y2);

        for (int y = min + 1; y < max; y++)
        {
            if (matrix[x, y] != -1)
            {
                return false;
            }
        }
        return true;
    }

    // same col
    private bool CheckLineY(int x1, int x2, int y)
    {
        int min = Math.Min(x1, x2);
        int max = Math.Max(x1, x2);

        for (int x = min + 1; x < max; x++)
        {
            if (matrix[x, y] != -1)
            {
                return false;
            }
        }
        return true;
    }

    private int CheckRectX(Point p1, Point p2)
    {
        Point pMinY = p1, pMaxY = p2;
        if (p1.y > p2.y)
        {
            pMinY = p2;
            pMaxY = p1;
        }
        for (int y = pMinY.y; y <= pMaxY.y; y++)
        {
            // Debug.Log($"CheckRectX y:{y} matrix[pMinY.x, y]:{matrix[pMinY.x, y]}");
            if (y > pMinY.y && matrix[pMinY.x, y] != -1)
            {
                return -1;
            }
            // Debug.Log($"CheckRectX (matrix[pMaxY.x, y]:{matrix[pMaxY.x, y]} CheckLineY(pMinY.x, pMaxY.x, y):{CheckLineY(pMinY.x, pMaxY.x, y)}"
            //        + $"CheckLineX(y, pMaxY.y, pMaxY.x):{CheckLineX(y, pMaxY.y, pMaxY.x)}");

            if ((matrix[pMaxY.x, y] == -1 || y == pMaxY.y)
                    && CheckLineY(pMinY.x, pMaxY.x, y)
                    && CheckLineX(y, pMaxY.y, pMaxY.x))
            {
                return y;
            }
        }
        return -1;
    }

    private int CheckRectY(Point p1, Point p2)
    {
        Point pMinX = p1, pMaxX = p2;
        if (p1.x > p2.x)
        {
            pMinX = p2;
            pMaxX = p1;
        }
        // find line and y begin
        for (int x = pMinX.x; x <= pMaxX.x; x++)
        {
            if (x > pMinX.x && matrix[x, pMinX.y] != -1)
            {
                return -1;
            }
            if (
                (matrix[x, pMaxX.y] == -1 || pMaxX.x == x)
                    && CheckLineX(pMinX.y, pMaxX.y, x)
                    && CheckLineY(x, pMaxX.x, pMaxX.y))
            {

                return x;
            }
        }
        return -1;
    }

    private int CheckMoreLineX(Point p1, Point p2, int type)
    {
        // find point have y min
        Point pMinY = p1, pMaxY = p2;
        if (p1.y > p2.y)
        {
            pMinY = p2;
            pMaxY = p1;
        }
        // find line and y begin
        int y = pMaxY.y + type;
        int row = pMinY.x;
        int colFinish = pMaxY.y;
        if (type == -1)
        {
            colFinish = pMinY.y;
            y = pMinY.y + type;
            row = pMaxY.x;
        }

        // find column finish of line
        // check more
        if ((matrix[row, colFinish] == -1 || pMinY.y == pMaxY.y)
                && CheckLineX(pMinY.y, pMaxY.y, row))
        {
            while (matrix[pMinY.x, y] == -1
                    && matrix[pMaxY.x, y] == -1)
            {
                if (CheckLineY(pMinY.x, pMaxY.x, y))
                {
                    return y;
                }
                y += type;
            }
        }
        return -1;
    }

    private int CheckMoreLineY(Point p1, Point p2, int type)
    {
        Point pMinX = p1, pMaxX = p2;
        if (p1.x > p2.x)
        {
            pMinX = p2;
            pMaxX = p1;
        }
        int x = pMaxX.x + type;
        int col = pMinX.y;
        int rowFinish = pMaxX.x;
        if (type == -1)
        {
            rowFinish = pMinX.x;
            x = pMinX.x + type;
            col = pMaxX.y;
        }
        if ((matrix[rowFinish, col] == -1 || pMinX.x == pMaxX.x)
                && CheckLineY(pMinX.x, pMaxX.x, col))
        {
            while (matrix[x, pMinX.y] == -1
                    && matrix[x, pMaxX.y] == -1)
            {
                if (CheckLineX(pMinX.y, pMaxX.y, x))
                {
                    return x;
                }
                x += type;
            }
        }
        return -1;
    }

    public MyLine CheckPoint(Point point1, Point point2, out bool hasNewPoint)
    {
        hasNewPoint = false;
        // Debug.Log("CheckPoint");

        if (point1.x == point2.x)
        {
            if (CheckLineX(point1.y, point2.y, point1.x))
                return new MyLine(point1, point2);
        }

        if (point1.y == point2.y)
        {
            if (CheckLineY(point1.x, point2.x, point1.y))
                return new MyLine(point1, point2);
        }

        int t = -1;

        t = CheckRectX(point1, point2);
        if (t != -1)
        {
            hasNewPoint = true;
            // Debug.Log("CheckRectX:" + t);
            return new MyLine(new Point(point1.x, t), new Point(point2.x, t));
        }

        t = CheckRectY(point1, point2);
        if (t != -1)
        {
            hasNewPoint = true;
            // Debug.Log("CheckRectY:" + t);
            return new MyLine(new Point(t, point1.y), new Point(t, point2.y));
        }

        t = CheckMoreLineX(point1, point2, 1);
        if (t != -1)
        {
            hasNewPoint = true;
            // Debug.Log("CheckMoreLineX:" + t + " 1");
            return new MyLine(new Point(point1.x, t), new Point(point2.x, t));
        }

        t = CheckMoreLineX(point1, point2, -1);
        if (t != -1)
        {
            hasNewPoint = true;
            // Debug.Log("CheckMoreLineX:" + t + " -1");
            return new MyLine(new Point(point1.x, t), new Point(point2.x, t));
        }

        t = CheckMoreLineY(point1, point2, 1);
        if (t != -1)
        {
            hasNewPoint = true;
            // Debug.Log("CheckMoreLineY:" + t + " 1");
            return new MyLine(new Point(t, point1.y), new Point(t, point2.y));
        }

        t = CheckMoreLineY(point1, point2, -1);
        if (t != -1)
        {
            hasNewPoint = true;
            // Debug.Log("CheckMoreLineY:" + t + " -1");
            return new MyLine(new Point(t, point1.y), new Point(t, point2.y));
        }

        return null;
    }

    public void SearchHint(bool once, out Point p1, out Point p2)
    {
        List<Tuple<Point, Point>> lst = new List<Tuple<Point, Point>>();

        for (int i = 1; i <= row; i++)
        {
            for (int j = 1; j <= col; j++)
            {
                int t = matrix[i, j];
                if (t == -1)
                    continue;

                for (int i1 = 1; i1 <= row; i1++)
                {
                    for (int j1 = 1; j1 <= col; j1++)
                    {
                        if (t == matrix[i1, j1] && (i != i1 || j != j1))
                        {
                            var _p1 = new Point(i, j);
                            var _p2 = new Point(i1, j1);
                            var myLine = CheckPoint(_p1, _p2, out _);
                            if (myLine != null)
                            {
                                lst.Add(new Tuple<Point, Point>(_p1, _p2));
                                if (once)
                                {
                                    p1 = _p1;
                                    p2 = _p2;
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
        if (lst.Count != 0)
        {
            var index = UnityEngine.Random.Range(0, lst.Count);
            p1 = lst[index].Item1;
            p2 = lst[index].Item2;
        }
        else
        {
            p1 = default;
            p2 = default;
        }
    }

    public void Shuff(Action<int, int, int, int> action)
    {
        List<Point> points = new List<Point>();
        for (int i1 = 1; i1 <= row; i1++)
        {
            for (int j1 = 1; j1 <= col; j1++)
            {
                var t2 = matrix[i1, j1];
                if (t2 == -1) continue;

                points.Add(new Point(i1, j1));
            }
        }

        Debug.Log("Start Shuff");
        Point p1 = default;
        // int cnt = 0;
        while (p1 == default)
        {
            for (int i = 0; i < points.Count; i++)
            {
                int index = UnityEngine.Random.Range(0, points.Count);
                if (index != i)
                {
                    var point1 = points[i];
                    var point2 = points[index];

                    action(point1.x, point1.y, point2.x, point2.y);
                }
            }

            SearchHint(true, out p1, out _);
        }
        Debug.Log("End Shuff: ");
    }
}