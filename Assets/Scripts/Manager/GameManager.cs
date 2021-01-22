using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [SerializeField] private Transform container;

    [HideInInspector]
    public bool endMatching = true;

    public bool debug;

    private Dictionary<Point, Vector3> pointPositionPair;
    private Dictionary<Point, Item> pointItemPair;

    private IEnumerator hintIE;

    private Item firstItem;
    private Item hintItem1, hintItem2;
    public GameState gameState;
    public MoveTypeAtStart moveTypeAtStart;
    public MoveTypeInGame moveTypeInGame;
    private ItemType itemType;

    private int totalEatedPair;
    public int row, col;
    private int offset;

    void Start()
    {
        instance = this;

        pointPositionPair = new Dictionary<Point, Vector3>();
        pointItemPair = new Dictionary<Point, Item>();

        if (debug) Time.timeScale = 0.02f;

        StartNewGame();
    }

    public void Restart(float delay)
    {
        StartCoroutine(StartNewGameIE(delay));
    }

    private void StartNewGame()
    {
        gameState = GameState.None;

        totalEatedPair = 0;

        while (container.childCount > 0)
        {
            ObjectPool.Instance.ReleaseObject(container.GetChild(0).gameObject);
        }

        pointPositionPair.Clear();
        pointItemPair.Clear();

        var level = PlayerPrefsHelper.instance.Level;
        moveTypeAtStart = LevelManager.Instance.GetMoveTypeAtStart(level);
        moveTypeInGame = LevelManager.Instance.GetMoveTypeInGame(level);

        var mapType = Test.instance.levelPairs[level].mapType;
        var widthLineInRect = 0f;
        if (mapType == 1)
        {
            row = 6; col = 5;
        }
        else if (mapType == 2)
        {
            row = 8; col = 6;
        }
        else
        {
            row = 10; col = 7;
        }
        if (row == 6 && col == 5)
        {
            offset = 30;
            widthLineInRect = 0.2f;
        }
        if (row == 8 && col == 6)
        {
            offset = 20;
            widthLineInRect = 0.17f;
        }
        if (row == 10 && col == 7)
        {
            offset = 15;
            widthLineInRect = 0.14f;
        }

        Debug.Log("levelllll:" + level);

        itemType = (ItemType)Test.instance.levelPairs[level].itemType;

        EffectController.instance.widtLineInRect = widthLineInRect;

        MapManager.instance.GetMapPosition(row, col, ref pointPositionPair, out var itemSize);

        CreateMatrix();
        GenMap(itemSize);

        ItemManager.instance.Setup(row, col, pointPositionPair, pointItemPair);
        InGameUIManager.instance.Setup(level, Test.instance.levelPairs[level].totalSec);
        // InGameUIManager.instance.Setup(level, 20);

        StartCoroutine(CheckEndMoving(StartPlay));

        MyAnalytic.EventLevelStart(PlayerPrefsHelper.instance.Level);
    }

    private void CreateMatrix()
    {
        var itemCount = 0;
        // itemPrefs = ItemPrefManager.instance.itemPrefsByTypesPair[itemType];
        // itemCount = itemPrefs.Count;

        Debug.Log("item type:" + itemType);

        var sprites = ItemPrefManager.instance.itemSpriteByType[itemType];
        itemCount = sprites.Count;

        var level = PlayerPrefsHelper.instance.Level;
        var maxItemCount = Test.instance.levelPairs[level].maxItemAmount;
        ItemController.Instance.CreateMatrix(row, col, itemCount, maxItemCount);
    }

    private void GenMap(Vector2 itemSize)
    {
        var itemPref = ItemPrefManager.instance.itemPref;
        var matrix = ItemController.Instance.GetMatrix();
        foreach (var pointPosition in pointPositionPair)
        {
            if (pointPosition.Key.x == 0 || pointPosition.Key.y == 0 || pointPosition.Key.x == row + 1 || pointPosition.Key.y == col + 1)
                continue;

            // Debug.Log("GenMap:" + pointPosition.Key.x + " " + pointPosition.Key.y);

            var index = matrix[pointPosition.Key.x, pointPosition.Key.y];

            var targetPos = pointPosition.Value;
            var firstPos = MapManager.instance.GetFirstPosByMoveType(moveTypeAtStart, targetPos);

            var itemGO = ObjectPool.Instance.GetGameObject(itemPref, firstPos, Quaternion.identity);

            itemGO.transform.parent = container;

            itemGO.transform.localScale = new Vector3(1, 1, 1);

            itemGO.GetComponent<RectTransform>().sizeDelta = itemSize;
            var imageGO = itemGO.transform.Find("Image");
            if (imageGO != null) imageGO.GetComponent<RectTransform>().sizeDelta = itemSize - new Vector2(offset, offset);

            imageGO.GetComponent<Image>().sprite = ItemPrefManager.instance.itemSpriteByType[itemType][index];

            var item = itemGO.GetComponent<Item>();
            item.Reset4AnglePos();
            item.point = pointPosition.Key;
            item.index = index;

            item.SetTargetPosition(firstPos, targetPos);

            pointItemPair[pointPosition.Key] = item;
        }
    }

    private void StartPlay()
    {
        gameState = GameState.Play;
        InGameUIManager.instance.StartGame();
    }

    public void CheckMatch(Item item)
    {
        if (!IsPlaying() || ItemsMoving() || !endMatching) return;

        if (firstItem == null)
        {
            firstItem = item;
            EffectController.instance.SetupRectAroundItem(1, item, false);
            AudioManager.instance.TileSelect();
            return;
        }
        else if (firstItem.point == item.point)
        {
            firstItem = null;
            EffectController.instance.HideRect(1);
            return;
        }
        EffectController.instance.SetupRectAroundItem(2, item, false);

        AudioManager.instance.TileSelect();

        if (firstItem.index != item.index)
        {
            StartCoroutine(HidePair(firstItem, item, false));
            firstItem = null;
            return;
        }


        var myLine = ItemController.Instance.CheckPoint(firstItem.point, item.point, out bool hasNewPoint);

        // Debug.Log($"{firstItem.point} {myLine?.P1} {myLine?.P2} {item.point}");

        if (myLine != null)
        {
            var points = new List<Point>();
            if (!hasNewPoint)
            {
                points.Add(firstItem.point);
                points.Add(item.point);
            }
            else
            {
                points.Add(firstItem.point);
                if (myLine.P1 != firstItem.point) points.Add(myLine.P1);
                if (myLine.P2 != item.point) points.Add(myLine.P2);
                points.Add(item.point);
            }
            var linePositions = ItemManager.instance.PointsToPositions(points, out var res2);
            EffectController.instance.SetupLine(linePositions, res2);

            StartCoroutine(HidePair(firstItem, item, true));
        }
        else
        {
            StartCoroutine(HidePair(firstItem, item, false));
        }

        firstItem = null;
    }

    private IEnumerator HidePair(Item item1, Item item2, bool isMatch)
    {
        endMatching = false;

        StopHint();
        item1.StopScale();
        item2.StopScale();

        if (isMatch)
        {
            item1.Scale(true, 0.1f, 0.05f);
            item2.Scale(true, 0.1f, 0.05f);

            StartCoroutine(EffectController.instance.FadeLineRenderer(0.25f));

            EffectController.instance.HideDollars(0.55f);

            yield return new WaitForSeconds(0.7f);

        }
        else
        {
            yield return new WaitForSeconds(0.1f);

            EffectController.instance.HideRect(-1);

            foreach (var pointItem in pointItemPair)
            {
                var item = pointItem.Value;
                if (item != null)
                {
                    item.MoveWhenWrong();
                }
            }

            if (PlayerPrefsHelper.instance.VibrateOn == 1)
                Handheld.Vibrate();

            yield return new WaitForSeconds(0.6f);
        }

        endMatching = true;
        InGameUIManager.instance.inHint = false;

        if (isMatch)
        {
            pointItemPair[item1.point] = null;
            pointItemPair[item2.point] = null;

            var matrix = ItemController.Instance.GetMatrix();
            matrix[item1.point.x, item1.point.y] = -1;
            matrix[item2.point.x, item2.point.y] = -1;

            totalEatedPair += 1;

            ItemManager.instance.MoveInGame(moveTypeInGame, item1.point.x, item1.point.y, item2.point.x, item2.point.y);

            StartCoroutine(CheckEndMoving(CheckEndGame));
        }
    }

    private IEnumerator CheckEndMoving(Action action)
    {
        while (true)
        {
            bool moving = ItemsMoving();

            if (moving) yield return new WaitForSeconds(0.1f);
            else break;
        }

        if (action != null) action();
    }

    private bool ItemsMoving()
    {
        foreach (var pointItem in pointItemPair)
        {
            var item = pointItem.Value;
            if (item != null && item.moving)
            {
                return true;
            }
        }
        return false;
    }

    private void CheckEndGame()
    {
        if (totalEatedPair == col * row / 2)
        {
            gameState = GameState.Win;
            GameManager.instance.GameOver();
        }
        else
        {
            ItemController.Instance.SearchHint(true, out var p1, out var p2);
            if (p1 == default && p2 == default)
            {
                Shuffle();
            }
        }
    }

    private IEnumerator StartNewGameIE(float timeDelay = 1f)
    {
        yield return new WaitForSeconds(timeDelay);
        StartNewGame();
    }

    public void Hint(Point p1, Point p2)
    {
        if (hintIE != null) StopCoroutine(hintIE);
        hintIE = HintIE(p1, p2);
        StartCoroutine(hintIE);
    }

    public void StopHint()
    {
        if (hintIE != null) StopCoroutine(hintIE);
        if (hintItem1 != null) hintItem1.StopScale();
        if (hintItem2 != null) hintItem2.StopScale();
    }

    private IEnumerator HintIE(Point p1, Point p2)
    {
        var item1 = pointItemPair[p1];
        var item2 = pointItemPair[p2];
        hintItem1 = item1;
        hintItem2 = item2;
        while (InGameUIManager.instance.inHint)
        {
            item1.Scale(false, 0.1f, 0.05f);
            item2.Scale(false, 0.1f, 0.05f);
            yield return new WaitForSeconds(1.5f);
        }
    }

    public void Shuffle()
    {
        EffectController.instance.HideRect(1);
        EffectController.instance.HideRect(2);

        StopHint();

        ItemController.Instance.Shuff((i, j, i1, j1) =>
        {
            ItemManager.instance.Swap(i, j, i1, j1);
        });
        InGameUIManager.instance.inShuff = false;
    }

    public bool IsPlaying()
    {
        return gameState == GameState.Play;
    }

    public void GameOver()
    {
        EffectController.instance.CheckMovingDollars(() => { GameOverIE(); });
        // StartCoroutine(GameOverIE(isWin, delay));
    }

    void GameOverIE()
    {
        bool isWin = gameState == GameState.Win;
        InGameUIManager.instance.EndGame(isWin);
    }

    public void ChangeTiles()
    {
        var length = Enum.GetNames(typeof(ItemType)).Length;
        var rd = UnityEngine.Random.Range(0, length);
        while ((ItemType)rd != itemType)
        {
            break;
        }

        itemType = (ItemType)rd;

        foreach (var pointItem in pointItemPair)
        {
            var item = pointItem.Value;
            if (item != null)
            {
                var imageGO = item.transform.Find("Image");
                imageGO.gameObject.GetComponent<Image>().sprite = ItemPrefManager.instance.itemSpriteByType[itemType][item.index];
            }
        }
    }
}
