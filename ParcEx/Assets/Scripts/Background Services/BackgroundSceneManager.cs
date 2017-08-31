using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackgroundSceneManager : MonoBehaviour {
    [System.Serializable]
    public class SceneTransitionInfo
    {
        public string SceneName;
        public List<GameObject> ObjectNames = new List<GameObject>();
    }

    public static BackgroundSceneManager instance;
    public AsyncOperation loadingOp;
    public string SceneNameToLoad;
    public List<string> SceneNames = new List<string>() { "Login", "Loading", "Main", "Tutorial", "Play"  };
    public SceneTransitionInfo SceneSpecificInfo = new SceneTransitionInfo();

    private void Awake()
    { 
        if (instance != null && instance != this)
        {
            if (instance.SceneSpecificInfo != this.SceneSpecificInfo)
                instance.SceneSpecificInfo = this.SceneSpecificInfo;
            Destroy(this.gameObject);
            return;//Avoid doing anything else
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        AndroidSaveSystem.postSaveLoaded.AddListener((SaveDataBundle sdb) =>
        {
            LoadScene(SceneNames[2]);
            ActivateScene();
        });
    }
    private void Start()
    {

    }

    private void SceneManager_activeSceneChanged(Scene previousScene, Scene loadedScene)
    {
        if(loadedScene.name == SceneNames[1])
        {
            loadingOp = SceneManager.LoadSceneAsync(SceneNameToLoad);
            SceneNameToLoad = string.Empty;
            SceneSpecificInfo = new SceneTransitionInfo();
        }
        else if(loadedScene.name == SceneNames[2])
        {
            if(SceneSpecificInfo.SceneName == SceneNames[2])
            {
                SceneSpecificInfo.ObjectNames[0].GetComponent<Button>().onClick.AddListener(() =>
                {
                    GPServices.m_saveSystem.CurrentSave.hasDoneTutorial = true;
                    LoadScene(SceneNames[3]);
                    ActivateScene();
                });
                SceneSpecificInfo.ObjectNames[1].GetComponent<Button>().onClick.AddListener(() =>
                {
                    GPServices.m_saveSystem.CurrentSave.hasDoneTutorial = true;
                    LoadScene(SceneNames[4]);
                    ActivateScene();
                });
            }
        }
        else if(loadedScene.name == SceneNames[3])
        {
            if (SceneSpecificInfo.SceneName == SceneNames[3])
            {
                SceneSpecificInfo.ObjectNames[0].GetComponent<Button>().onClick.AddListener(() =>
                {
                    if(SceneNameToLoad == string.Empty)
                    {
                        AndroidSaveSystem.onSaveGame.AddListener((SaveDataBundle sdb) => { ActivateScene(); });
                        LoadScene(SceneNames[2]);
                        GPServices.m_saveSystem.GPlay_SaveGame();
                        GameObject.Find("EventSystem").SetActive(false);
                    }
                }) ;
            }
        }
        else if(loadedScene.name == SceneNames[4])
        {
            if(SceneSpecificInfo.SceneName == SceneNames[4])
            {
                SceneSpecificInfo.ObjectNames[0].GetComponent<Button>().onClick.AddListener(() =>
                {
                    if(SceneNameToLoad == string.Empty)
                    {
                        AndroidSaveSystem.onSaveGame.AddListener((SaveDataBundle sdb) => { ActivateScene(); });
                        LoadScene(SceneNames[2]);
                        GPServices.m_saveSystem.GPlay_SaveGame();
                        GameObject.Find("EventSystem").SetActive(false);
                    }
                });
            }
        }
    }

    public void LoadScene(string sceneName)
    {
        if (!SceneNames.Contains(sceneName))
            return;
        SceneNameToLoad = sceneName;
        loadingOp = SceneManager.LoadSceneAsync(SceneNames[1]);
        loadingOp.allowSceneActivation = false;
    }
    public void ActivateScene()
    {
        if (SceneNameToLoad == string.Empty)
            return;
        loadingOp.allowSceneActivation = true;
    }
}


