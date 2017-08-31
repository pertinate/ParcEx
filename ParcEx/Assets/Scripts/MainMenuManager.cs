using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {
    public GameObject playTutorial;
    private void Start()
    {
        playTutorial.SetActive(false);
    }

    public void PressPlayButton()
    {
        if (GPServices.m_saveSystem.CurrentSave.hasDoneTutorial)
        {
            BackgroundSceneManager.instance.LoadScene(BackgroundSceneManager.instance.SceneNames[4]);
            BackgroundSceneManager.instance.ActivateScene();
        }
        else
        {
            playTutorial.SetActive(true);
        }
    }

    public void ResetTutorial()
    {
        GPServices.m_saveSystem.CurrentSave.hasDoneTutorial = false;
    }

    public void SaveOnBack()
    {
        GPServices.m_saveSystem.GPlay_SaveGame();
    }
}
