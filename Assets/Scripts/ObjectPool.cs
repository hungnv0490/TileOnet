using System;
using System.Collections;
using System.Collections.Generic;
using Assets.SimpleAndroidNotifications;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private static ObjectPool instance;
    public static ObjectPool Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject gameObject = new GameObject("ObjectPool");
                instance = gameObject.AddComponent<ObjectPool>();
                DontDestroyOnLoad(gameObject);
            }
            return instance;
        }
        private set { }
    }

    Dictionary<int, List<GameObject>> pools = new Dictionary<int, List<GameObject>>();
    private object syncRoot = new object();

    [SerializeField]
    List<GameObject> prebs;

    private void Awake()
    {
        if (instance != null && instance.GetInstanceID() != this.GetInstanceID())
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this as ObjectPool;
            DontDestroyOnLoad(gameObject);
        }

        if (prebs != null)
        {
            for (int i = 0; i < prebs.Count; i++)
            {
                var preb = prebs[i];
                CreateGameObject(preb);
            }
        }

#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
  Debug.unityLogger.logEnabled = false;
#endif

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        string[] titles = new string[] { "One To One", "Tile Onet", "Connect Onet" };
        string[] desArray = new string[] { "Time to relax!", "Matching your loved ones!", "How far can you progress?" };

        for (int k = 1; k <= 10; k++)
        {
            var title = titles[UnityEngine.Random.Range(0, titles.Length)];
            var des = desArray[UnityEngine.Random.Range(0, desArray.Length)];
            NotificationManager.SendWithAppIcon(TimeSpan.FromHours(k * 12), title, des,
                            new Color(0, 0.6f, 1), NotificationIcon.Star);
        }
    }

    private void CreateGameObject(GameObject prefab, int amount = 80)
    {
        int type = prefab.GetInstanceID();
        if (!pools.ContainsKey(type))
        {
            pools.Add(type, new List<GameObject>());
        }
        var pool = pools[type];

        for (int i = 0; i < amount; i++)
        {
            var go = Instantiate(prefab);
            go.transform.parent = transform;
            go.SetActive(false);
            pool.Add(go);
        }
    }

    public GameObject GetGameObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int type = prefab.GetInstanceID();
        if (!pools.ContainsKey(type))
        {
            pools.Add(type, new List<GameObject>());
        }
        var pool = pools[type];
        GameObject go = null;
        for (int k = 0; k < pool.Count; k++)
        {
            go = pool[k];
            if (go == null)
            {
                pool.Remove(go);
                continue;
            }
            if (go.activeSelf == false)
            {
                Transform goTransform = go.transform;
                goTransform.position = position;
                goTransform.rotation = rotation;

                go.SetActive(true);
                return go;
            }
        }
        go = Instantiate(prefab, position, rotation);
        go.transform.parent = transform;
        pool.Add(go);
        return go;
    }

    public void ReleaseObject(GameObject go)
    {
        go.transform.SetParent(transform);
        go.SetActive(false);
    }
}
