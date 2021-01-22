using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    public static AdManager instance = null;

    public bool testMode = false;

    private BannerView bannerView;
    private RewardedAd rewardedAd;
    private InterstitialAd interstitial, interReward;

    private string bannerId_test = "ca-app-pub-3940256099942544/6300978111";
    private string interId_test = "ca-app-pub-3940256099942544/1033173712";
    private string rewardId_test = "ca-app-pub-3940256099942544/5224354917";
    private string interRewardId_test = "ca-app-pub-3940256099942544/1033173712";

    private string bannerId = "ca-app-pub-5517456668364384/8968336529";
    private string interId = "ca-app-pub-5517456668364384/2856571357";
    private string rewardId = "ca-app-pub-5517456668364384/9898274816";
    private string interRewardId = "ca-app-pub-5517456668364384/4837519823";

    private Action callback = null, interRewardCallback = null;
    private Action interCallback = null;

    void Awake()
    {
        if (instance != null && instance.gameObject.GetInstanceID() != gameObject.GetInstanceID())
        {
            Destroy(instance.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        MobileAds.Initialize(initStatus =>
        {
            RequestBanner();
            RequestInterstitial();
            RequestRewardedAd();
            RequestInterReward();
        });
    }

    private void RequestBanner()
    {
#if UNITY_ANDROID
        string adUnitId = bannerId;
#elif UNITY_IPHONE
            string adUnitId = bannerId;
#else
        string adUnitId = bannerId;
#endif

        if (testMode) adUnitId = bannerId_test;

        // Create a 320x50 banner at the top of the screen.
        this.bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

        AdRequest request = new AdRequest.Builder().Build();
        this.bannerView.LoadAd(request);
    }

    private void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = interId;
#elif UNITY_IPHONE
        string adUnitId = interId;
#else
        string adUnitId = "unexpected_platform";
#endif

        if (testMode) adUnitId = interId;

        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitId);
        this.interstitial.OnAdClosed += HandleOnAdClosed;
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }

    void HandleOnAdClosed(object sender, EventArgs args)
    {
        RequestInterstitial();
        interCallback?.Invoke();
        interCallback = null;
    }

    public void ShowInterAd(Action callback)
    {
        if (this.interstitial.IsLoaded())
        {
            this.interstitial.Show();
            interCallback = callback;
        }
        else
        {
            callback.Invoke();
        }
    }

    private void RequestRewardedAd()
    {
#if UNITY_ANDROID
        string adUnitId = rewardId;
#elif UNITY_IPHONE
            string adUnitId = rewardId;
#else
            string adUnitId = "unexpected_platform";
#endif

        if (testMode) adUnitId = rewardId_test;

        this.rewardedAd = new RewardedAd(adUnitId);

        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);
    }

    private void RequestInterReward()
    {
#if UNITY_ANDROID
        string adUnitId = interRewardId;
#elif UNITY_IPHONE
        string adUnitId = interRewardId;
#else
        string adUnitId = "unexpected_platform";
#endif

        if (testMode) adUnitId = interRewardId_test;

        // Initialize an InterstitialAd.
        this.interReward = new InterstitialAd(adUnitId);
        this.interReward.OnAdClosed += HandleInterRewardClosed;
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interReward.LoadAd(request);
    }

    private void HandleInterRewardClosed(object sender, EventArgs args)
    {
        if (interRewardCallback != null) interRewardCallback.Invoke();
        interRewardCallback = null;
        this.RequestInterReward();
    }

    private void HandleUserEarnedReward(object sender, Reward args)
    {
        if (callback != null)
        {
            callback.Invoke();
            callback = null;
        }
    }

    private void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        this.RequestRewardedAd();
    }

    public void UserChoseToWatchAd(Action cb, Action cbNotLoadedAd)
    {
        callback = cb;
        if (this.rewardedAd.IsLoaded())
        {
            this.rewardedAd.Show();
        }
        else if (this.interReward.IsLoaded())
        {
            interCallback = cb;
            this.interReward.Show();
        }
        else
        {
            if (cbNotLoadedAd != null)
            {
                cbNotLoadedAd.Invoke();
            }
        }
    }

    public bool LoadedRewardedAd()
    {
        return rewardedAd.IsLoaded();
    }
}
