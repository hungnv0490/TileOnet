using Firebase.Analytics;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MyAnalytic : MonoBehaviour
{
    const string EVENT_LEVEL_COMPLETED = "event_level_completed";
    const string EVENT_LEVEL_FAILED = "event_level_failed";
    const string EVENT_LEVEL_START = "event_level_start";
    const string EVENT_REWARD_ADS = "event_reward_ads";
    const string EVENT_SHOW_INTER = "event_show_inter";

    static bool init = false;
    private void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
                // InitalizeFirebase();
                // FetchDataAsync();
                FirebaseAnalytics.LogEvent("my_start_session");
                init = true;
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }
    public static void EventLevelCompleted(int level)
    {
        if(!init) return;

        FirebaseAnalytics.LogEvent(EVENT_LEVEL_COMPLETED + "_" + level);
    }
    public static void EventLevelFailed(int level)
    {
        if(!init) return;

        FirebaseAnalytics.LogEvent(EVENT_LEVEL_FAILED + "_" + level);
    }
    public static void EventLevelStart(int level)
    {
        if(!init) return;

        FirebaseAnalytics.LogEvent(EVENT_LEVEL_START + "_" + level);
    }
    public static void EventReward(string name)
    {
        if(!init) return;

        FirebaseAnalytics.LogEvent(EVENT_REWARD_ADS + "_" + name);
    }

    public static void EventShowInter()
    {
        if(!init) return;

        FirebaseAnalytics.LogEvent(EVENT_SHOW_INTER);
    }
}
