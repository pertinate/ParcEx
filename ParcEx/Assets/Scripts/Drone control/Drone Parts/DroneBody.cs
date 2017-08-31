using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneBody : DronePartBase {
    public DroneBody(string partName, float weight, float speedModifier)
    {
        this.PartName = partName;
        this.weight = weight;
        this.speedModifier = speedModifier;
    }
    public override void Attach(ref Drone drone)
    {
        base.Attach(ref drone);
    }
}
