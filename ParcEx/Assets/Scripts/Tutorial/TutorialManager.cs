using Delivery;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {
	public static TutorialManager instance;
	public Transform target;

	public GameObject[] tutorialPrompts;
	public GameObject menuIcon, deliveryButton;
	public GameObject menuIconHighlight, deliveryButtonHighlight, mapHighlight;

	// Use this for initialization
	void Start () {
		instance = this;
		PauseGame();
		SetUpPrompts();
	}

	private void SetUpPrompts()
	{
		foreach(GameObject go in tutorialPrompts)
		{
			go.SetActive(false);
		}
		tutorialPrompts[0].SetActive(true);
		menuIconHighlight.SetActive(false);
		deliveryButtonHighlight.SetActive(false);
		Swipe.Click.AddListener("TutorialIntroduction", (Swipe s) =>
		{//Accept delivery tutorial.
            tutorialPrompts[0].SetActive(false);
            tutorialPrompts[1].SetActive(true);
            Swipe.Click.AddListener("TutorialIntroduction2", (Swipe sw) =>
            {
                tutorialPrompts[1].SetActive(false);
                menuIconHighlight.SetActive(true);
                menuIcon.GetComponent<Button>().onClick.AddListener(() =>
                {//on menu icon click
                    menuIconHighlight.SetActive(false);
                    deliveryButtonHighlight.SetActive(true);
                    deliveryButton.GetComponent<Button>().onClick.AddListener(() =>
                    {//spawn tutorial delivery
                        deliveryButtonHighlight.SetActive(false);
                        TutorialDeliveryManager.instance.CreateDelivery();
                        //get accept button
                        TutorialDeliveryManager._onDeliverySuccessAccept.AddListener((BaseDelivery bd) =>
                        {
                            GameObject.Find("Main").SetActive(false);
                            TutorialDeliveryManager._onPackageSpawn.AddListener((BaseDelivery b) =>
                            {
                                MobileCamera.instance.Resume(b.package.transform.parent);
                                tutorialPrompts[2].SetActive(true);
                                Swipe.Click.AddListener("PackageIntroduction", (Swipe swi) =>
                                {
                                    tutorialPrompts[2].SetActive(false);
                                    ReturnToGame();
                                    TutorialDeliveryManager._onPackagePickup.AddListener((BaseDelivery d) =>
                                    {
                                        PauseGame();
                                        MobileCamera.instance.Resume(d.dzt.transform);
                                        tutorialPrompts[3].SetActive(true);
                                        d.dzt.Enable(d);
                                        Swipe.Click.AddListener("DropIntroduction", (Swipe swip) =>
                                        {
                                            tutorialPrompts[3].SetActive(false);
                                            ReturnToGame();
                                            TutorialDeliveryManager._onDeliveryCompleted.AddListener((BaseDelivery bdd) =>
                                            {
                                                tutorialPrompts[4].SetActive(true);
                                                Swipe.Click.AddListener("CompletedTutorial", (Swipe swipe) =>
                                                {
                                                    BackgroundSceneManager.instance.LoadScene(BackgroundSceneManager.instance.SceneNames[2]);
                                                });
                                            });
                                        });
                                    });
                                });
                            });
                        });
                        /*
                         * delegate =>
                         * {
                         *      
                         * }
                         */
                    });
                });
            });
		});
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void ReturnToGame()
	{
		MobileCamera.instance.Resume();
		Drone.instance.stopMotors = false;
	}

	public void PauseGame()
	{
		MobileCamera.instance.Pause();
		Drone.instance.stopMotors = true;
	}
}
