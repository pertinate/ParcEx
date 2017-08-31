using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DronePartBase {
    public float weight;
    public float speedModifier;
    public string PartName;

    public GameObject bodyObject;

    public virtual void Attach(ref Drone drone)
    {
        drone.rb.mass += weight;
        drone.speedAdditive += speedModifier;
    }
}
