using UnityEngine;

public class PlayerPrefsHelper
{
    public static PlayerPrefsHelper instance = new PlayerPrefsHelper();
    public int Level
    {
        get { return PlayerPrefs.GetInt("Level", 1); }
        set { PlayerPrefs.SetInt("Level", value); }
    }

    private int dollar = 0;
    public int Dollar
    {
        get { return dollar; }
        set { dollar = value; }
    }

    public int Hint
    {
        get { return PlayerPrefs.GetInt("Hint", 3); }
        set { if (value > 99) value = 99; PlayerPrefs.SetInt("Hint", value); }
    }

    public int Shulfe
    {
        get { return PlayerPrefs.GetInt("Shulfe", 3); }
        set { if (value > 99) value = 99; PlayerPrefs.SetInt("Shulfe", value); }
    }

    public int SoundOn
    {
        get { return PlayerPrefs.GetInt("SoundOn", 1); }
        set { PlayerPrefs.SetInt("SoundOn", value); }
    }

    public int MusicOn
    {
        get { return PlayerPrefs.GetInt("MusicOn", 1); }
        set { PlayerPrefs.SetInt("MusicOn", value); }
    }

    public int VibrateOn
    {
        get { return PlayerPrefs.GetInt("VibrateOn", 0); }
        set { PlayerPrefs.SetInt("VibrateOn", value); }
    }
}