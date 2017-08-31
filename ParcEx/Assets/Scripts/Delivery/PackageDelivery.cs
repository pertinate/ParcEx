using Delivery;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PackageDelivery : MonoBehaviour {
    protected static PackageDelivery instance;
    public Button button;
    public BaseDelivery del;
    public static bool hasPickup = false;

    private void Awake()
    {
        Init();
    }
    protected virtual void Init()
    {
        instance = this;
    }
    private void Start()
    {
        button = GameObject.Find("HookButton").GetComponent<Button>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Player")
        {
            button.onClick.AddListener(() =>
            {
                if (!hasPickup)
                {
                    if (instance == this)
                        DeliveryManager.instance.PickupPackage(del);
                    else
                        TutorialDeliveryManager.instance.PickupPackage(del);
                    hasPickup = true;
                }
            });
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
        {
            button.onClick.RemoveAllListeners();
            hasPickup = false;
        }
    }
}
