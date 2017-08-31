using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Delivery
{
    public class DropZoneTrigger : MonoBehaviour
    {
        public BaseDelivery deliveryForZone;
        public Renderer dztRend;
        public string address;
        public Image image;
        public bool hasStayed = false;
        public bool hasEntered = false;
        public int waitFor = 1;

        private void Awake()
        {
            dztRend = GetComponent<Renderer>();
            Disable();
            DeliveryManager.onDeliverySuccessAccept.AddListener((BaseDelivery del) =>
            {
                if(del.deliveryAddress == address)
                {
                    MapController.RegisterMapObject(this.gameObject, image);
                }
            });
            //TutorialDelivery.onDeliveryAccept.AddListener((BaseDelivery del) => Enable(del));
            DeliveryManager.onPackagePickup.AddListener((BaseDelivery del) => Enable(del));
            DeliveryManager.onDeliveryFail.AddListener((BaseDelivery del) => Disable());
        }
        private void OnTriggerEnter(Collider other)
        {
            if(other.name == address)
            {
                StartCoroutine(delay());
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if(other.name == address)
            {
                if (hasStayed)
                {
                    if (deliveryForZone != null)
                    {
                        DeliveryManager.onDeliveryCompleted.Invoke(deliveryForZone);
                        deliveryForZone = null;
                        Disable();
                    }
                }
            }
        }
        private IEnumerator delay()
        {
            yield return new WaitForSeconds(waitFor);
            hasStayed = true;
        }
        public void Enable(BaseDelivery del)
        {
            if(del.deliveryAddress == address)
            {
                deliveryForZone = del;
                Color tempCol = dztRend.material.color;
                tempCol.a = 0.3f;
                dztRend.material.color = tempCol;
                hasStayed = false;
            }
        }
        public void Disable()
        {
            Color tempCol = dztRend.material.color;
            tempCol.a = 0;
            dztRend.material.color = tempCol;
            MapController.RemoveMapObject(this.gameObject);
        }
    }
}
