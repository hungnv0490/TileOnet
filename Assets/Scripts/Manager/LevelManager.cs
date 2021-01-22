
using System;
using System.Collections.Generic;

public class LevelManager
{
    public static LevelManager Instance = new LevelManager();

    // public int Level { get; set; } = 1;

    private List<int> moveTypeAtStarts = new List<int>();
    private List<int> moveTypeInGames = new List<int>();

    public LevelManager()
    {
        var length = Enum.GetNames(typeof(MoveTypeAtStart)).Length;
        for (int i = 0; i < length; i++)
        {
            moveTypeAtStarts.Add(i);
        }
        length = Enum.GetNames(typeof(MoveTypeInGame)).Length;
        for (int i = 0; i < length; i++)
        {
            moveTypeInGames.Add(i);
        }
    }

    public int GetItemCountByLevel(int level)
    {
        if (level <= 10)
            return 5;

        return 100;
    }

    public MoveTypeAtStart GetMoveTypeAtStart(int level)
    {
        if (level == 1)
        {
            return MoveTypeAtStart.None;
        }
        if (moveTypeAtStarts.Count == 0)
        {
            var length = Enum.GetNames(typeof(MoveTypeAtStart)).Length;
            for (int i = 0; i < length; i++)
            {
                moveTypeAtStarts.Add(i);
            }
        }

        var id = moveTypeAtStarts[UnityEngine.Random.Range(0, moveTypeAtStarts.Count)];
        moveTypeAtStarts.Remove(id);

        return (MoveTypeAtStart)(id);
    }

    public MoveTypeInGame GetMoveTypeInGame(int level)
    {
        if (level == 1)
        {
            return MoveTypeInGame.None;
        }
        if (moveTypeInGames.Count == 0)
        {
            var length = Enum.GetNames(typeof(MoveTypeInGame)).Length;
            for (int i = 0; i < length; i++)
            {
                moveTypeInGames.Add(i);
            }
        }

        var id = moveTypeInGames[UnityEngine.Random.Range(0, moveTypeInGames.Count)];
        moveTypeInGames.Remove(id);

        return (MoveTypeInGame)(id);
    }
}