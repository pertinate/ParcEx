using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Delivery
{
    public class DeliveryDatabase : MonoBehaviour
    {
        private enum DeliveryType
        {
            Normal,
            Special,
            Test,
        }
        public static DeliveryDatabase instance;

        public static DeliveryEvent onPendDelAdd = new DeliveryEvent();
        public static DeliveryEvent onSpecialDeliveryCreated = new DeliveryEvent();

        public List<DropZoneTrigger> dropZones = new List<DropZoneTrigger>();

        public static DropZoneTrigger GetDZT(string address)
        {
            for(int i = 0; i < instance.dropZones.Count; i++)
            {
                if(address == instance.dropZones[i].address)
                {
                    return instance.dropZones[i];
                }
            }
            return null;
        }

        private List<string> availableAddresses = new List<string>();

        private bool isFirstDel = true;

        public int maxDeliveryCount = 3;
        public int maxDeliveryStart = 1;
        public int currentDeliveryCount = 0;

        public float Timer = 0;
        public float nextDeliveryDelay = 15;

        private void Awake()
        {
            instance = this;
            DeliveryManager.onDeliveryExpire.AddListener((BaseDelivery del) => HandleExpireDelivery(del));
            foreach(DropZoneTrigger dzt in GameObject.FindObjectsOfType<DropZoneTrigger>())
            {
                dropZones.Add(dzt);
                availableAddresses.Add(dzt.address);
            }
        }
        private void Start()
        {

        }
        private void Update()
        {
            if (CanCreateNewDel)
            {
                Timer += Time.deltaTime;

                if (Timer >= nextDeliveryDelay || isFirstDel)
                {
                        CreateDelivery(DeliveryType.Test);
                        Timer = 0;
                        isFirstDel = false;
                }
            }
        }

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

        private void CreateDelivery(DeliveryType type)
        {
            BaseDelivery del = null;
            switch (type)
            {
                case DeliveryType.Test:
                    if(availableAddresses.Count > 0)
                    {
                        int addIndex = UnityEngine.Random.Range(0, availableAddresses.Count - 1);
                        if (addIndex >= 0)
                        {
                            bool tmpBool = false;
                            while (!tmpBool)
                            {
                                int packageWeight = (new int[] { 10, 20, 30 })[Random.Range(0, (int)2)];
                                del = new BaseDelivery(true, availableAddresses[addIndex], Random.Range(30, 60), packageWeight);
                                del.dzt = GetDZT(del.deliveryAddress);
                                del.dzt.deliveryForZone = del;
                                availableAddresses.RemoveAt(addIndex);
                                currentDeliveryCount++;
                                int totalTimeGiven = (int)(packageWeight * Vector3.Distance(Vector3.zero, del.dzt.transform.position)) / 50;
                                del.timeLeft = totalTimeGiven;
                                del.initTime = totalTimeGiven;
                                tmpBool = true;
                            }
                        }
                    }
                    break;
            }
            if (del != null)
                onPendDelAdd.Invoke(del);
        }

        public void HandleExpireDelivery(BaseDelivery del)
        {
            currentDeliveryCount--;
            if (!availableAddresses.Contains(del.deliveryAddress))
            {
                availableAddresses.Add(del.deliveryAddress);
            }
            for(int i = 0; i < DeliveryManager.instance.packageSpawn.Count; i++)
            {
                if(DeliveryManager.instance.packageSpawn[i].Key == del.package)
                {
                    DeliveryManager.instance.packageSpawn[i] = new KeyValuePair<GameObject, bool>(DeliveryManager.instance.packageSpawn[i].Key, true);
                }
            }
            PackageDelivery.hasPickup = false;
            Destroy(del.package);
        }
    }
}