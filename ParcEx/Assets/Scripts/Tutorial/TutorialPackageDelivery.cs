using Delivery;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPackageDelivery : PackageDelivery
{
    protected override void Init()
    {
        instance = this;
    }
}