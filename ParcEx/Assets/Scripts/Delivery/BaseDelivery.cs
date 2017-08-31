using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Delivery
{
    public class BaseDelivery
    {
        public BaseDelivery()
        {
            timeLeft = 20f;
            timeUntilExpire = 20f;
        }
        public BaseDelivery(bool perm, string add, float timePending, int packageWeight)
        {
            isPerm = perm;
            isCompleted = false;
            deliveryAddress = add;
            timeUntilExpire = timePending;
            this.packageWeight = packageWeight;
        }
        public bool isPerm = false;
        public bool isCompleted = false;
        public bool isCanceled = false;
        public bool isAccepted = false;
        public string deliveryAddress = "123 Apple Ln";
        public float timeLeft = -1;
        public float initTime = -1;
        public float timeUntilExpire = -1;
        public float cashReward = 10f;
        public float multiplier = 1f;
        public float packageWeight = 10f;

        public IEnumerator pendExpire;
        public IEnumerator delivExpire;

        public DropZoneTrigger dzt;

        public GameObject package;
    }
}
