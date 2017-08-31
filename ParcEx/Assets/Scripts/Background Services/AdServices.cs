using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;

public class AdServices : MonoBehaviour {
    public static string gameID = (Application.platform == RuntimePlatform.Android) ? "1425875" : (Application.platform == RuntimePlatform.IPhonePlayer) ? "1235758" : string.Empty;

    public static AdServices instance;

    public static AdEvent onAdFinish = new AdEvent();

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;//Avoid doing anything else
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
        Advertisement.Initialize("1425875", true);
    }

    private void Start()
    {

    }

    public IEnumerator ShowUnitySimpleAd()
    {
        while (!Advertisement.IsReady())
            yield return null;
        Advertisement.Show();
    }

    public void ShowUnityAd()
    {
        StartCoroutine(WaitForAd());
        if (Advertisement.IsReady())
            Advertisement.Show();
    }

    private IEnumerator WaitForAd()
    {
        float currentTimeScale = Time.timeScale;
        Time.timeScale = 0;
        yield return null;
    
        while (Advertisement.isShowing)
            yield return null;
        Time.timeScale = currentTimeScale;
    }
    public IEnumerator ShowRewardedUnityAd()
    {
        while (!Advertisement.IsReady("reward"))
        {
            yield return null;
        }
        var options = new ShowOptions { resultCallback = HandleShowResult };
        Advertisement.Show("reward", options);
    }

    private void HandleShowResult(ShowResult result)
    {
        onAdFinish.Invoke(result);
    }
}
public class AdEvent : UnityEvent<ShowResult> {}
