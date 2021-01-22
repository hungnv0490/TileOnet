using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelWin : MonoBehaviour
{
    public Slider slider;
    public Button viewAdBtn;

    public Text coinTxt;
    public Text timeTxt;

    private int hintBonus, shufleBonus;

    public void SetUp(int coin, int secTotal, int secRemain)
    {
        coinTxt.text = coin.ToString();
        timeTxt.text = SecondToString(secRemain);

        var per = secRemain * 1f / secTotal;
        slider.maxValue = 1f;
        slider.value = per;
        if (per >= 0.75f)
        {
            PlayerPrefsHelper.instance.Hint += 1;
            PlayerPrefsHelper.instance.Shulfe += 1;
            InGameUIManager.instance.OnTipShufleChange.Invoke();
        }
        else if (per >= 0.5f)
        {
            PlayerPrefsHelper.instance.Shulfe += 1;
            InGameUIManager.instance.OnTipShufleChange.Invoke();
        }
        else viewAdBtn.interactable = false;
    }

    void OnDisable()
    {
        GameManager.instance.Restart(1f);
    }

    private string SecondToString(int time)
    {
        if (time <= 0)
            return "00:00";

        int min = time / 60;
        int sec = time - min * 60;
        if (min < 10)
        {
            if (sec < 10) return $"0{min}:0{sec}";
            else return $"0{min}:{sec}";
        }
        return $"{min}:{sec}";
    }

    public void ViewAd()
    {
#if UNITY_EDITOR
        PlayerPrefsHelper.instance.Hint += 1;
        PlayerPrefsHelper.instance.Shulfe += 1;
        InGameUIManager.instance.OnTipShufleChange.Invoke();
        gameObject.SetActive(false);
#else
       UserChoseToWatchAd();
#endif
    }

    void UserChoseToWatchAd()
    {
        AdManager.instance.UserChoseToWatchAd(() =>
               {
                   PlayerPrefsHelper.instance.Hint += 1;
                   PlayerPrefsHelper.instance.Shulfe += 1;
                   InGameUIManager.instance.OnTipShufleChange.Invoke();
                   gameObject.SetActive(false);
               }, null);
    }
}
