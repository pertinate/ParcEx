using Delivery;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialUIManager : MonoBehaviour
{
    public static TutorialUIManager instance;

    public Dictionary<BaseDelivery, GameObject> pendingList = new Dictionary<BaseDelivery, GameObject>();
    public Dictionary<BaseDelivery, GameObject> acceptedList = new Dictionary<BaseDelivery, GameObject>();

    public ScrollRect pendingSR;

    public GameObject mainUI, deliveryUI, settingsUI, mapUI, detailText, pendingGO, acceptedGO, AccDecl, completedUI;

    private Text detail;
    public Text loggedIn, Cash, FPS, rewBA, rewTex;

    public Button saveButton, loadButton, rewAccept, rewDecline, pickupButton;

    public GameObject pendDelPrefab, accDelPrefab;
    public Transform pendParent, accParent;
    private void Awake()
    {
        instance = this;
        Application.targetFrameRate = 30;
        TutorialDeliveryManager.onPendDelAdd.AddListener((BaseDelivery del) => HandleNewDelivery(del));
        TutorialDeliveryManager._onDeliverySuccessAccept.AddListener((BaseDelivery del) => HandleUIOnDeliveryUpdate(true, del));
        TutorialDeliveryManager._onDeliveryDecline.AddListener((BaseDelivery del) => HandleUIOnDeliveryUpdate(false, del));
        saveButton.onClick.AddListener(() => GPServices.m_saveSystem.GPlay_SaveGame());
        loadButton.onClick.AddListener(() => GPServices.m_saveSystem.GPlay_LoadGame(this));
        detail = detailText.GetComponent<Text>();
        mainUI.SetActive(false);
        detailText.SetActive(false);
        pendingGO.SetActive(true);
        acceptedGO.SetActive(false);
        settingsUI.SetActive(false);
        mapUI.SetActive(false);
        AccDecl.SetActive(false);
        completedUI.SetActive(false);
        Swipe.Horizontal.AddListener("textswipe", (Swipe s) => HandleDeliverySwitch());
        Swipe.Click.AddListener("textswipe", (Swipe s) => HandleDeliverySwitch());
        GPServices.onSignIn.AddListener((bool b) =>
        {
            if (b && GPServices.instance.Authenticated)
            {
                loggedIn.text = "Logged in as: " + GPServices.instance.localUser.userName;
            }
            else
            {
                loggedIn.text = "Failed to log in.";
            }
        });
        TutorialDeliveryManager._onDeliveryCompleted.AddListener((BaseDelivery del) =>
        {
            AccDecl.SetActive(true);
            del.multiplier = (float)Math.Round(UnityEngine.Random.Range(1.5f, 2f), 2);
            del.cashReward = ((del.initTime - del.timeLeft) * del.packageWeight * UnityEngine.Random.Range(45, 90)) / 850;
            rewTex.text = string.Format("Would you watch an ad for a multiplier of {0} to your reward?", del.multiplier);
            rewBA.text = string.Format("Before: {0}\nAfter: {1}", del.cashReward, (del.cashReward * del.multiplier).ToString());
            rewAccept.onClick.AddListener(() =>
            {
                AdServices.onAdFinish.AddListener((ShowResult sr) =>
                {
                    if (sr == ShowResult.Finished)
                    {
                        del.cashReward *= del.multiplier;
                    }
                    GPServices.m_saveSystem.CurrentSave.m_Cash += del.cashReward;
                    ShowCompletedUI(del);
                    AdServices.onAdFinish.RemoveAllListeners();
                });
                Destroy(del.package);
                StartCoroutine(AdServices.instance.ShowRewardedUnityAd());
                AccDecl.SetActive(false);
                rewAccept.onClick.RemoveAllListeners();
            });
            rewDecline.onClick.AddListener(() =>
            {
                GPServices.m_saveSystem.CurrentSave.m_Cash += del.cashReward;
                ShowCompletedUI(del);
                rewDecline.onClick.RemoveAllListeners();
            });

        });
        foreach (Transform t in settingsUI.transform)
        {
            if (t.name == "Global")
            {
                t.GetComponent<Slider>().value = GPServices.m_saveSystem.CurrentSave.globalAudio;
            }
            else if (t.name == "Music")
            {
                t.GetComponent<Slider>().value = GPServices.m_saveSystem.CurrentSave.musicAudio;
            }
            else if (t.name == "SFX")
            {
                t.GetComponent<Slider>().value = GPServices.m_saveSystem.CurrentSave.sfxAudio;
            }
            else if (t.name == "Mute")
            {
                t.GetComponent<Toggle>().isOn = GPServices.m_saveSystem.CurrentSave.muted;
                t.GetComponent<Toggle>().onValueChanged.Invoke(GPServices.m_saveSystem.CurrentSave.muted);
            }
            else if (t.name == "Logged In As")
            {
                t.GetComponent<Text>().text = "Logged in as: " + GPServices.instance.localUser.userName + ".";
            }
        }
    }

    private void ShowCompletedUI(BaseDelivery del)
    {
        completedUI.SetActive(true);
        GPServices.m_saveSystem.SaveGame();
        SwipeExt.AddListener(Swipe.Click, "Completed", (Swipe s) => {
            completedUI.SetActive(false);
        });
        if (completedUI.activeSelf)
        {
            foreach (Transform t in completedUI.transform)
            {
                if (t.name == "Time took")
                {
                    TimeSpan ts = TimeSpan.FromSeconds(del.timeLeft);
                    t.GetComponent<Text>().text = string.Format("You took {0} minutes and {1} seconds to complete the delivery.", ts.Minutes, ts.Seconds);
                }
                else if (t.name == "Time alotted")
                {
                    TimeSpan ts = TimeSpan.FromSeconds(del.initTime);
                    t.GetComponent<Text>().text = string.Format("You had {0} minutes and {1} seconds to complete the delivery.", ts.Minutes, ts.Seconds);
                }
                else if (t.name == "End reward")
                {
                    t.GetComponent<Text>().text = string.Format("You got rewarded ${0}", del.cashReward);
                }
                else if (t.name == "Closing in")
                {
                    StartCoroutine(handleClosingCompUI(t.GetComponent<Text>()));
                }
            }
        }
    }
    private IEnumerator handleClosingCompUI(Text t)
    {
        int time = 2;
        t.text = string.Format("Closing in: {0}", time);
        while (time > 0 && completedUI.activeSelf)
        {
            yield return new WaitForSeconds(1f);
            time--;
            t.text = string.Format("Closing in: {0}", time);
        }
        if (time <= 2)
        {
            completedUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (mainUI.activeSelf && !detailText.activeSelf && deliveryUI.activeSelf)
            detailText.SetActive(true);
        else if (!mainUI.activeSelf && detailText.activeSelf && deliveryUI.activeSelf)
            detailText.SetActive(false);
        else if (!deliveryUI.activeSelf && detailText.activeSelf)
            detailText.SetActive(false);

        if (mainUI.activeSelf)
        {
            Cash.text = string.Format("Cash: ${0}", GPServices.m_saveSystem.CurrentSave.m_Cash.ToString("#,##0"));
        }

        if (FPS != null)
            FPS.text = ((int)(1.0f / Time.smoothDeltaTime)).ToString();
    }
    private void HandleNewDelivery(BaseDelivery del)
    {
        if (del != null)
        {
            GameObject go = Instantiate(pendDelPrefab);
            //set scale, position, parent
            go.transform.SetParent(pendParent);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.localScale = new Vector3(1, 1, 1);
            //set info
            foreach (Transform t in go.transform)
            {
                switch (t.name)
                {
                    case "Address":
                        t.GetComponent<Text>().text = "Address: " + del.deliveryAddress;
                        break;
                    case "Time to Complete":
                        t.GetComponent<Text>().text = "Complete in: " + del.initTime + " seconds";
                        break;
                    case "Time Until Expire":
                        t.GetComponent<Text>().text = "Expires in: " + del.timeUntilExpire + " seconds";
                        break;
                    case "Reward":
                        t.GetComponent<Text>().text = "Reward: $0.";
                        break;
                    case "Accept"://button
                        t.GetComponent<Button>().onClick.AddListener(() => TutorialDeliveryManager._onDeliveryAccept.Invoke(del));
                        break;
                    case "Decline"://button
                        t.GetComponent<Button>().onClick.AddListener(() => TutorialDeliveryManager._onDeliveryDecline.Invoke(del));
                        break;
                }
            }
            pendingList.Add(del, go);
        }
    }
    private IEnumerator HandleExpiredText(Text t, BaseDelivery del)
    {
        while (del.timeUntilExpire > 0)
        {
            if (del.isCanceled || del.isAccepted)
                break;
            yield return new WaitForSeconds(1f);
            if (!del.isCanceled && !del.isAccepted)
            {
                del.timeUntilExpire--;
                t.text = "Expires in: " + del.timeUntilExpire + " seconds";
            }
        }
        if ((del.timeUntilExpire <= 0 || del.isCanceled) && !del.isAccepted)
        {
            HandleExpiredDelivery(del);
        }
    }
    private void HandleExpiredDelivery(BaseDelivery del)
    {
        TutorialDeliveryManager._onDeliveryExpire.Invoke(del);

        for (int i = 0; i < pendingList.Count; i++)
        {
            GameObject test;
            if (pendingList.TryGetValue(del, out test))
            {
                Destroy(test);
                pendingList.Remove(del);
            }
        }
    }
    private void HandleUIOnDeliveryUpdate(bool accept, BaseDelivery del)
    {
        if (accept)
        {
            del.isAccepted = true;
            //Handle removing from ui and call appropriate events
            GameObject go = Instantiate(accDelPrefab);

            go.transform.SetParent(accParent);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.localScale = new Vector3(1, 1, 1);

            foreach (Transform t in go.transform)
            {
                switch (t.name)
                {
                    case "Address":
                        t.GetComponent<Text>().text = "Address: " + del.deliveryAddress;
                        break;
                    case "Time to Complete":
                        t.GetComponent<Text>().text = (-1).ToString();
                        break;
                    case "Reward":
                        t.GetComponent<Text>().text = "Reward: 0.";
                        break;
                }
            }
            TutorialDeliveryManager.instance.currentDeliveryCount--;
            for (int i = 0; i < pendingList.Count; i++)
            {
                GameObject test;
                if (pendingList.TryGetValue(del, out test))
                {
                    Destroy(test);
                }
            }
            acceptedList.Add(del, go);
        }
        else
        {
            del.isCanceled = true;
        }
    }

    private void HandleDeliverySwitch()
    {
        if (pendingGO.activeSelf && !acceptedGO.activeSelf)
        {
            pendingGO.SetActive(false);
            acceptedGO.SetActive(true);
            detail.text = "ACCEPTED DELIVERIES";
        }
        else if (!pendingGO.activeSelf && acceptedGO.activeSelf)
        {
            pendingGO.SetActive(true);
            acceptedGO.SetActive(false);
            detail.text = "PENDING DELIVERIES";
        }
    }

    public void HandleUISwitch(string text)
    {
        switch (text)
        {
            case "settings":
                if (!settingsUI.activeSelf)
                {
                    settingsUI.SetActive(true);
                    deliveryUI.SetActive(false);
                    mapUI.SetActive(false);
                }
                break;
            case "delivery":
                if (!deliveryUI.activeSelf)
                {
                    settingsUI.SetActive(false);
                    deliveryUI.SetActive(true);
                    mapUI.SetActive(false);
                }
                break;
            case "map":
                if (!mapUI.activeSelf)
                {
                    settingsUI.SetActive(false);
                    deliveryUI.SetActive(false);
                    mapUI.SetActive(true);
                }
                break;
        }
    }

}
