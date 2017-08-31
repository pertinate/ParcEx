using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Delivery;
using UnityEngine.UI;

namespace Delivery
{
    public class DeliveryManager : MonoBehaviour
    {
        public static Dictionary<BaseDelivery, DropZoneTrigger> acceptedDeliveries = new Dictionary<BaseDelivery, DropZoneTrigger>();

        public static DeliveryManager instance;

        public static DeliveryEvent onDeliveryAccept = new DeliveryEvent();
        public static DeliveryEvent onDeliveryDecline = new DeliveryEvent();
        public static DeliveryEvent onDeliveryExpire = new DeliveryEvent();
        public static DeliveryEvent onDeliveryCompleted = new DeliveryEvent();
        public static DeliveryEvent onDeliveryFail = new DeliveryEvent();

        public static DeliveryEvent onDeliverySuccessAccept = new DeliveryEvent();
        public static DeliveryEvent onPackagePickup = new DeliveryEvent();

        public List<KeyValuePair<GameObject, bool>> packageSpawn = new List<KeyValuePair<GameObject, bool>>();

        public GameObject package;

        public int maxDeliveryCount = 3;

        private void Awake()
        {
            instance = this;

            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Spawn"))
            {
                packageSpawn.Add(new KeyValuePair<GameObject, bool>(go, true));
            }
        }
        private void Start()
        {
            onDeliveryDecline.AddListener((BaseDelivery del) => HandlePendingDelivery(del, false));
            onDeliveryAccept.AddListener((BaseDelivery del) => HandlePendingDelivery(del, true));
            onDeliveryFail.AddListener((BaseDelivery del) => HandleFailedDelivery(del));
        }

        public void HandlePendingDelivery(BaseDelivery del, bool accept)
        {
            if (del.dzt != null && del != null && acceptedDeliveries.Count < maxDeliveryCount && !acceptedDeliveries.ContainsKey(del) && accept)
            {
                //assign delivery and 
                acceptedDeliveries.Add(del, del.dzt);
                onDeliverySuccessAccept.Invoke(del);
                //need to spawn in package(s) at a space
                List<int> temp = new List<int>();
                for(int i = 0; i < packageSpawn.Count; i++)
                {
                    if (packageSpawn[i].Value == true)
                        temp.Add(i);
                }
                if(temp.Count > 0)
                {
                    
                    del.package = Instantiate(package);
                    int t = Random.Range(0, temp.Count - 1);
                    del.package.transform.SetParent(packageSpawn[temp[t]].Key.transform);
                    packageSpawn[temp[t]] = new KeyValuePair<GameObject, bool>(packageSpawn[temp[t]].Key, false);
                    del.package.transform.localPosition = new Vector3(0, 0, 0);
                    del.package.name = del.deliveryAddress;
                    del.package.GetComponent<PackageDelivery>().del = del;
                }
            }
        }
        
        public void HandleCompletedDelivery(BaseDelivery del, DropZoneTrigger pdt)
        {

        }

        public void HandleFailedDelivery(BaseDelivery del)
        {
            DeliveryDatabase.instance.HandleExpireDelivery(del);
            del.dzt.Disable();
        }

        public void PickupPackage(BaseDelivery del)
        {
            del.package.transform.SetParent(Drone.instance.transform);
            del.package.transform.localPosition = new Vector3(0, -0.39f, 0);
            onPackagePickup.Invoke(del);
        }
    }
}
