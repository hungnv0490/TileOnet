using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class IconDes
{
    public int type;//1, 2, 3
    public Sprite icon;
    public string des;
}

public class PanelHelp : MonoBehaviour
{
    public List<IconDes> iconDes;
    public Text text;
    public Image imge;
    public PanelLoading panelLoading;

    private int curType;
    private float timeLoadedAd;

    void Awake()
    {
    }

    void OnDisable()
    {
        Time.timeScale = 1;
    }

    void OnEnable()
    {
        Time.timeScale = 0;
    }

    public void Setup(int type)
    {
        curType = type;

        var id = iconDes.First(ic => ic.type == type);
        text.text = id.des;
        imge.sprite = id.icon;
    }

    public void ClickViewRewardedAd()
    {
        AudioManager.instance.ButtonClick();
        UserChoseToWatchAd(() =>
        {
            StartCoroutine(RunLoadAd());
        });
    }

    private IEnumerator RunLoadAd()
    {
        panelLoading.StartLoading();

        var time = 0f;
        while (!AdManager.instance.LoadedRewardedAd() && time < 3)
        {
            yield return null;

            time += Time.deltaTime;
        }

        UserChoseToWatchAd(null);
        panelLoading.StopLoading();
    }

    private void UserChoseToWatchAd(Action action)
    {
#if UNITY_EDITOR
        if (curType == 1) InGameUIManager.instance.TipChange(3);
        if (curType == 2) InGameUIManager.instance.ShuffleChange(3);
        if (curType == 3) GameManager.instance.ChangeTiles();
        gameObject.SetActive(false);
#else
        AdManager.instance.UserChoseToWatchAd(() =>
        {
            if (curType == 1){
                InGameUIManager.instance.TipChange(3);
                MyAnalytic.EventReward("TipChange");
            }
            if (curType == 2){
                InGameUIManager.instance.ShuffleChange(3);
                MyAnalytic.EventReward("ShuffleChange");
            } 
            if (curType == 3)
            {
                 GameManager.instance.ChangeTiles();
                 MyAnalytic.EventReward("ChangeTiles");
            }

            gameObject.SetActive(false);
        }, action);
#endif
    }


    public void Cancel()
    {
        AudioManager.instance.ButtonClick();
        gameObject.SetActive(false);
    }
}
