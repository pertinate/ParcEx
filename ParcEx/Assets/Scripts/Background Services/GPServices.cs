using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames.BasicApi.SavedGame;
using System;

public class GPServices : MonoBehaviour {
    public static GPServices instance;
    public bool debug = true;

    public static GPSAuthEvent onSignIn = new GPSAuthEvent();

    public static AndroidSaveSystem m_saveSystem;

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;//Avoid doing anything else
        }

        if (m_saveSystem == null)
            m_saveSystem = new AndroidSaveSystem();

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .EnableSavedGames()
            //.RequireGooglePlus()
            .Build();
        PlayGamesPlatform.InitializeInstance(config);
        if(debug)
            PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }

    private void Start()
    {
        Authenticate();
    }

    public bool Override = false;

    public bool Authenticated
    {
        get { return Social.Active.localUser.authenticated; }
    }

    public PlayGamesLocalUser localUser
    {
        get { return ((PlayGamesLocalUser)Social.localUser); }
    }

    public void Authenticate()
    {
        Social.localUser.Authenticate((bool success) => {
            if (success)
            {
                Override = false;
                onSignIn.Invoke(success);
                m_saveSystem.GPlay_LoadGame(this);
                //NotifService.instance.CreateNotif(string.Format("Authenticated as: {0}.{1}", localUser.userName), NotifService.MessageType.Notification);
            }
            else
            {
                Override = true;
                m_saveSystem.LoadGame(this);
                onSignIn.Invoke(success);
                //NotifService.instance.CreateNotif("Failed to authenticate.", NotifService.MessageType.Alert);
            }
        });
    }

    public void SignOut()
    {
        PlayGamesPlatform.Instance.SignOut();
    }

    public void ShowAchievements()
    {
        Social.ShowAchievementsUI();
    }

    public void ShowLeaderboard()
    {
        Social.ShowLeaderboardUI();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            m_saveSystem.GPlay_SaveGame();
        }
    }

    private void OnApplicationQuit()
    {
        m_saveSystem.GPlay_SaveGame();
    }
}
