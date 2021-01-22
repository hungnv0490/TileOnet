using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    public static InGameUIManager instance = null;

    public Action OnTipShufleChange = null;

    [SerializeField] private Text tipNumberTxt;
    [SerializeField] private Text shuffleNumberTxt;
    [SerializeField] private Text coinNumberTxt;
    [SerializeField] private Text timeTxt;
    [SerializeField] private Text levelAmountTxt;

    [SerializeField] private PanelHelp panelHelp;
    [SerializeField] private InGamePanelPause inGamePanelPause;
    [SerializeField] private PanelWin panelWin;
    [SerializeField] private PanelLose panelLose;

    private int tipNumber, shuffleNumber, coinNumber, levelAmount, curTime;
    private IEnumerator runTimeIE;
    public bool inHint, inShuff;
    private int maxTimeSec;

    void Awake()
    {
        // if (instance != null && instance.gameObject.GetInstanceID() != gameObject.GetInstanceID())
        // {
        //     Destroy(instance.gameObject);
        // }
        instance = this;
        // DontDestroyOnLoad(gameObject);

        OnTipShufleChange = TipShufleChange;
    }

    public void Setup(int level, int time)
    {
        panelHelp.gameObject.SetActive(false);
        inGamePanelPause.gameObject.SetActive(false);

        Debug.Log(" PlayerPrefsHelper.instance.Hint:" + PlayerPrefsHelper.instance.Hint);
        tipNumber = PlayerPrefsHelper.instance.Hint;
        shuffleNumber = PlayerPrefsHelper.instance.Shulfe;
        coinNumber = 0;
        levelAmount = level;
        curTime = time;

        inHint = false;
        inShuff = false;

        tipNumberTxt.text = tipNumber.ToString();
        shuffleNumberTxt.text = shuffleNumber.ToString();
        coinNumberTxt.text = coinNumber.ToString();
        levelAmountTxt.text = levelAmount.ToString();
        timeTxt.text = SecondToString(time);

        // maxTimeSec = time * 3 / 4;
        maxTimeSec = time;

        panelHelp.gameObject.SetActive(false);

        EffectController.instance.HideRect(-1);
    }

    public void StartGame()
    {
        if (runTimeIE != null) StopCoroutine(runTimeIE);
        runTimeIE = RunTime();
        StartCoroutine(runTimeIE);
    }

    public void CoinChange(int coin)
    {
        coinNumber += coin;
        coinNumberTxt.text = coinNumber.ToString();
        // PlayerPrefsHelper.instance.Dollar += coinNumber;
    }

    public void TipChange(int tip)
    {
        tipNumber += tip;
        tipNumberTxt.text = tipNumber.ToString();

        PlayerPrefsHelper.instance.Hint = tipNumber;
    }

    private void TipShufleChange()
    {
        tipNumberTxt.text = PlayerPrefsHelper.instance.Hint.ToString();
        shuffleNumberTxt.text = PlayerPrefsHelper.instance.Shulfe.ToString();
    }

    public void ShuffleChange(int shuffle)
    {
        shuffleNumber += shuffle;
        shuffleNumberTxt.text = shuffleNumber.ToString();

        PlayerPrefsHelper.instance.Shulfe = shuffleNumber;
    }

    public void LevelChange(int level)
    {
        levelAmount += level;
        levelAmountTxt.text = levelAmount.ToString();
    }

    private IEnumerator RunTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            curTime--;
            timeTxt.text = SecondToString(curTime);
            if (curTime <= 0)
            {
                GameManager.instance.gameState = GameState.Lose;
                GameManager.instance.GameOver();
                yield break;
            }
        }
    }

    public void AddMoreTime(int time)
    {
        // maxTimeSec = time * 3 / 4;
        maxTimeSec = time;
        curTime = time;

        GameManager.instance.gameState = GameState.Play;

        if (runTimeIE != null) StopCoroutine(runTimeIE);
        runTimeIE = RunTime();
        StartCoroutine(runTimeIE);
    }

    public void EndGame(bool isWin)
    {
        StopCoroutine(runTimeIE);
        runTimeIE = null;

        if (isWin)
        {
            MyAnalytic.EventLevelCompleted(PlayerPrefsHelper.instance.Level);

            panelWin.gameObject.SetActive(true);
            panelWin.SetUp(coinNumber, maxTimeSec, curTime);

            AudioManager.instance.Victory();
            var level = PlayerPrefsHelper.instance.Level;
            if (level >= 5 && level % 2 == 0)
            {
#if UNITY_EDITOR
                PlayerPrefsHelper.instance.Level++;
                // StartCoroutine(StartNewGameIE());
#else
                AdManager.instance.ShowInterAd(() =>
                {
                    PlayerPrefsHelper.instance.Level++;
                    // StartCoroutine(StartNewGameIE());
                    MyAnalytic.EventShowInter();
                });
#endif

            }
            else
            {
                PlayerPrefsHelper.instance.Level++;
                // StartCoroutine(StartNewGameIE());
            }
        }
        else
        {
            MyAnalytic.EventLevelFailed(PlayerPrefsHelper.instance.Level);
            AudioManager.instance.Defeat();

            panelLose.gameObject.SetActive(true);
            // StartCoroutine(StartNewGameIE());
        }
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

    public void BtnTipClick()
    {
        if (inHint || !GameManager.instance.IsPlaying()
            || !GameManager.instance.endMatching) return;

        AudioManager.instance.Hint();

        if (tipNumber <= 0)
        {
            panelHelp.gameObject.SetActive(true);
            panelHelp.Setup(1);
            return;
        }

        TipChange(-1);

        inHint = true;

        ItemController.Instance.SearchHint(false, out Point p1, out Point p2);
        if (p1 != default)
        {
            GameManager.instance.Hint(p1, p2);
        }
    }

    public void BtnShuffleClick()
    {
        if (inShuff || !GameManager.instance.IsPlaying()
            || !GameManager.instance.endMatching) return;

        AudioManager.instance.Shuffle();

        if (shuffleNumber <= 0)
        {
            panelHelp.gameObject.SetActive(true);
            panelHelp.Setup(2);
            return;
        }

        ShuffleChange(-1);

        inShuff = true;

        GameManager.instance.Shuffle();
    }

    public void BtnChangeType()
    {
        AudioManager.instance.ButtonClick();

        panelHelp.gameObject.SetActive(true);
        panelHelp.Setup(3);
    }

    public void BtnPause()
    {
        AudioManager.instance.ButtonClick();
        inGamePanelPause.gameObject.SetActive(true);
    }

    public void BtnSkip()
    {
        AudioManager.instance.ButtonClick();

#if UNITY_EDITOR
        PlayerPrefsHelper.instance.Level++;
        GameManager.instance.Restart(0.1f);

#else
        AdManager.instance.UserChoseToWatchAd(() =>
        {
            PlayerPrefsHelper.instance.Level++;
            GameManager.instance.Restart(0f);
        }, null);    
#endif
    }
}
