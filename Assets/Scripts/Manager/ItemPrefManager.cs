

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemPrefManager : MonoBehaviour
{
    public static ItemPrefManager instance;

    public GameObject itemPref;

    public Dictionary<ItemType, List<Sprite>> itemSpriteByType;

    void Awake()
    {
        if (instance != null && instance.gameObject.GetInstanceID() != gameObject.GetInstanceID())
        {
            Destroy(instance.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        itemSpriteByType = new Dictionary<ItemType, List<Sprite>>();

        var itemTypeNames = Enum.GetNames(typeof(ItemType));

        for (int i = 0; i < itemTypeNames.Length; i++)
        {
            var itemType = (ItemType)i;
            var path = "Sprites/" + itemTypeNames[i];
            itemSpriteByType[itemType] = Resources.LoadAll<Sprite>(path).ToList();
        }
    }
}