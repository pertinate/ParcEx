using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Delivery;

public class TutorialDeliveryManager : MonoBehaviour {
    public static Dictionary<BaseDelivery, DropZoneTrigger> acceptedDeliveries = new Dictionary<BaseDelivery, DropZoneTrigger>();

    public static TutorialDeliveryManager instance;

    public static DeliveryEvent _onPendDelAdd = new DeliveryEvent();
    public static DeliveryEvent _onSpecialDeliveryCreated = new DeliveryEvent();
    public static DeliveryEvent _onDeliveryAccept = new DeliveryEvent();
    public static DeliveryEvent _onDeliveryDecline = new DeliveryEvent();
    public static DeliveryEvent _onDeliveryExpire = new DeliveryEvent();
    public static DeliveryEvent _onDeliveryCompleted = new DeliveryEvent();
    public static DeliveryEvent _onDeliveryFail = new DeliveryEvent();
    public static DeliveryEvent _onDeliverySuccessAccept = new DeliveryEvent();
    public static DeliveryEvent _onPackagePickup = new DeliveryEvent();
    public static DeliveryEvent _onPackageSpawn = new DeliveryEvent();



    public static DeliveryEvent onPendDelAdd = new DeliveryEvent();
    public static DeliveryEvent onSpecialDeliveryCreated = new DeliveryEvent();

    public List<DropZoneTrigger> dropZones = new List<DropZoneTrigger>();


    private List<string> availableAddresses = new List<string>();

    public int maxDeliveryCount = 3;
    public int maxDeliveryStart = 1;
    public int currentDeliveryCount = 0;

    public float Timer = 0;
    public float nextDeliveryDelay = 15;

    public int maxAcceptableDeliveryCount = 3;

    private void Awake()
    {
        instance = this;

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Spawn"))
        {
            packageSpawn.Add(new KeyValuePair<GameObject, bool>(go, true));
        }
        DeliveryManager.onDeliveryExpire.AddListener((BaseDelivery del) => HandleExpireDelivery(del));
        foreach (DropZoneTrigger dzt in GameObject.FindObjectsOfType<DropZoneTrigger>())
        {
            dropZones.Add(dzt);
            availableAddresses.Add(dzt.address);
        }
    }
    private void Start()
    {
        _onDeliveryDecline.AddListener((BaseDelivery del) => HandlePendingDelivery(del, false));
        _onDeliveryAccept.AddListener((BaseDelivery del) => HandlePendingDelivery(del, true));
        _onDeliveryFail.AddListener((BaseDelivery del) => HandleFailedDelivery(del));
    }

    public List<KeyValuePair<GameObject, bool>> packageSpawn = new List<KeyValuePair<GameObject, bool>>();

    public GameObject package;

    private bool CanCreateNewDel
    {
        get
        {
            if ((currentDeliveryCount < maxDeliveryCount) && (availableAddresses.Count > 0))
                return true;
            else
                return false;
        }
    }

    public static DropZoneTrigger GetDZT(string address)
    {
        for (int i = 0; i < instance.dropZones.Count; i++)
        {
            if (address == instance.dropZones[i].address)
            {
                return instance.dropZones[i];
            }
        }
        return null;
    }

    public void CreateDelivery()
    {
        BaseDelivery del = null;
        if (availableAddresses.Count > 0)
        {
            int addIndex = UnityEngine.Random.Range(0, availableAddresses.Count - 1);
            if (addIndex >= 0)
            {
                bool tmpBool = false;
                while (!tmpBool)
                {
                    int packageWeight = (new int[] { 10, 20, 30 })[Random.Range(0, 2)];
                    del = new BaseDelivery(true, availableAddresses[addIndex], -1, packageWeight);
                    del.dzt = GetDZT(del.deliveryAddress);
                    del.dzt.deliveryForZone = del;
                    availableAddresses.RemoveAt(addIndex);
                    currentDeliveryCount++;
                    del.timeLeft = -1;
                    del.initTime = -1;
                    tmpBool = true;
                }
            }
        }
        if (del != null)
            onPendDelAdd.Invoke(del);
    }

    public void HandlePendingDelivery(BaseDelivery del, bool accept)
    {
        if (del.dzt != null && del != null && acceptedDeliveries.Count < maxAcceptableDeliveryCount && !acceptedDeliveries.ContainsKey(del) && accept)
        {
            //assign delivery and 
            acceptedDeliveries.Add(del, del.dzt);
            _onDeliverySuccessAccept.Invoke(del);
            //need to spawn in package(s) at a space
            List<int> temp = new List<int>();
            for (int i = 0; i < packageSpawn.Count; i++)
            {
                if (packageSpawn[i].Value == true)
                    temp.Add(i);
            }
            if (temp.Count > 0)
            {

                del.package = Instantiate(package);
                int t = Random.Range(0, temp.Count - 1);
                del.package.transform.SetParent(packageSpawn[temp[t]].Key.transform);
                packageSpawn[temp[t]] = new KeyValuePair<GameObject, bool>(packageSpawn[temp[t]].Key, false);
                del.package.transform.localPosition = new Vector3(0, 0, 0);
                del.package.name = del.deliveryAddress;
                del.package.GetComponent<PackageDelivery>().del = del;
                _onPackageSpawn.Invoke(del);
            }
        }
    }

    public void HandleCompletedDelivery(BaseDelivery del, DropZoneTrigger pdt)
    {

    }

    public void HandleFailedDelivery(BaseDelivery del)
    {
        HandleExpireDelivery(del);
        del.dzt.Disable();
    }

    public void PickupPackage(BaseDelivery del)
    {
        del.package.transform.SetParent(Drone.instance.transform);
        del.package.transform.localPosition = new Vector3(0, -0.39f, 0);
        _onPackagePickup.Invoke(del);
    }



    public void HandleExpireDelivery(BaseDelivery del)
    {
        currentDeliveryCount--;
        if (!availableAddresses.Contains(del.deliveryAddress))
        {
            availableAddresses.Add(del.deliveryAddress);
        }
        for (int i = 0; i < DeliveryManager.instance.packageSpawn.Count; i++)
        {
            if (DeliveryManager.instance.packageSpawn[i].Key == del.package)
            {
                DeliveryManager.instance.packageSpawn[i] = new KeyValuePair<GameObject, bool>(DeliveryManager.instance.packageSpawn[i].Key, true);
            }
        }
        TutorialPackageDelivery.hasPickup = false;
        Destroy(del.package);
    }
}
