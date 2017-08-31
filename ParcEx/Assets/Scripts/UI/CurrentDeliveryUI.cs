using Delivery;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentDeliveryUI : MonoBehaviour
{
    public Text Address, timeLeft;
    public GameObject go;
    public bool doUpdates = false;
    public BaseDelivery del;

    private void OpenUI(BaseDelivery del)
    {
        this.del = del;
        go.SetActive(true);
        Address.text = del.deliveryAddress;
    }
    private void CloseUI(BaseDelivery del)
    {
        del = null;
        go.SetActive(false);
    }

    private void Update()
    {
        if(del != null)
        {
            timeLeft.text = del.timeLeft.ToString();
        }
    }
    private void Awake()
    {
        go.SetActive(false);
        DeliveryManager.onDeliveryAccept.AddListener((BaseDelivery delv) => OpenUI(delv));
        DeliveryManager.onDeliveryCompleted.AddListener((BaseDelivery delv) => CloseUI(delv));
    }
}
