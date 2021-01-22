using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ActionType
{
    None,
    Sound,
    Vibrate,
    Music
}

public class ClickAction : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public ActionType action;
    public Sprite spriteOn;
    public Sprite spriteOff;

    public bool isOn;

    void Awake()
    {
    }

    void Start()
    {
        switch (action)
        {
            case ActionType.Sound:
                isOn = PlayerPrefsHelper.instance.SoundOn == 1;
                break;
            case ActionType.Vibrate:
                isOn = PlayerPrefsHelper.instance.VibrateOn == 1;
                break;
            case ActionType.Music:
                isOn = PlayerPrefsHelper.instance.MusicOn == 1;
                break;
        }
        SetSprite();
    }

    private void SetSprite()
    {
        var img = GetComponent<Image>();
        if (isOn == false)
        {
            img.sprite = spriteOff;
        }
        else
        {
            img.sprite = spriteOn;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        AudioManager.instance.ButtonClick();
        isOn = !isOn;

        SetSprite();
        switch (action)
        {
            case ActionType.Sound:
                PlayerPrefsHelper.instance.SoundOn = isOn == true ? 1 : 0;
                break;
            case ActionType.Vibrate:
                PlayerPrefsHelper.instance.VibrateOn = isOn == true ? 1 : 0;
                break;
            case ActionType.Music:
                PlayerPrefsHelper.instance.MusicOn = isOn == true ? 1 : 0;
                if (PlayerPrefsHelper.instance.MusicOn == 1)
                {
                    AudioManager.instance.PlayGameBg();
                }
                else
                {
                    AudioManager.instance.StopGameBg();
                }
                break;
        }
    }
}