using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelLose : MonoBehaviour
{
    private bool viewAd = false;

    public void ViewAd()
    {
#if UNITY_EDITOR
        viewAd = true;
        InGameUIManager.instance.AddMoreTime(60);
        gameObject.SetActive(false);
#else
       UserChoseToWatchAd();
#endif
    }

    void OnEnable()
    {
        viewAd = false;
    }

    void OnDisable()
    {
        if (!viewAd)
            GameManager.instance.Restart(0.5f);
    }

    void UserChoseToWatchAd()
    {
        AdManager.instance.UserChoseToWatchAd(() =>
                       {
                           viewAd = true;
                           InGameUIManager.instance.AddMoreTime(60);
                           gameObject.SetActive(false);
                       }, null);
    }
}
