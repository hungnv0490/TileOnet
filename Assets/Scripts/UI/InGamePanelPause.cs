using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGamePanelPause : MonoBehaviour
{
    void OnEnable()
    {
        Time.timeScale = 0;
    }
    
    void OnDisable()
    {
        Time.timeScale = 1;
    }

    public void Restart()
    {
        AudioManager.instance.ButtonClick();
        gameObject.SetActive(false);
        GameManager.instance.Restart(0.1f);
    }

    public void Continue()
    {
        AudioManager.instance.ButtonClick();
        gameObject.SetActive(false);
    }

    public void Home()
    {
        AudioManager.instance.ButtonClick();
        gameObject.SetActive(false);
    }
}
