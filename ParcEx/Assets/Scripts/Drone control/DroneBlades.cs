using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneBlades : MonoBehaviour {
    public GameObject BladeOne, BladeTwo, BladeThree, BladeFour;
    public int RotationsPerMinute = 200;

    private void Update()
    {
        if (Drone.instance != null)
        {
            RotateBladeThree();
            RotateBladeFour();
            RotateBladeOne();
            RotateBladeTwo();
        }
    }

    private void RotateBladeThree()
    {
        Vector3 temp = new Vector3(0, 3 * RotationsPerMinute * Time.deltaTime, 0);//rotate a certain amount per minute by default
        if(Drone.instance.Pitch != 0 || Drone.instance.Roll != 0)
        {
            temp = new Vector3(0, 15 * RotationsPerMinute * Time.deltaTime, 0);
            if (Drone.instance.Pitch > 0) //if going forward
            {
                temp.y *= (Mathf.Clamp(1 - Drone.instance.Pitch, 0.25f, 0.5f));
            }
            else if (Drone.instance.Pitch < 0) //if going back
            {
                temp.y *= (Mathf.Clamp(Mathf.Abs(Drone.instance.Pitch), 0.5f, 1f));
            }

            if (Drone.instance.Roll > 0)
            {
                temp.y *= (Mathf.Clamp(Drone.instance.Roll, 0.5f, 1f));
            }
            else if (Drone.instance.Roll < 0)
            {
                temp.y *= (Mathf.Clamp(1 - Mathf.Abs(Drone.instance.Roll), 0.25f, 0.5f));
            }
        }
        BladeThree.transform.Rotate(temp);
    }
    private void RotateBladeFour()
    {
        Vector3 temp = new Vector3(0, 3 * RotationsPerMinute * Time.deltaTime, 0);//rotate a certain amount per minute by default
        if (Drone.instance.Pitch != 0 || Drone.instance.Roll != 0)
        {
            temp = new Vector3(0, 15 * RotationsPerMinute * Time.deltaTime, 0);
            if (Drone.instance.Pitch > 0) //if going forward
            {
                temp.y *= (Mathf.Clamp(1 - Drone.instance.Pitch, 0.25f, 0.5f));
            }
            else if (Drone.instance.Pitch < 0) //if going back
            {
                temp.y *= (Mathf.Clamp(Mathf.Abs(Drone.instance.Pitch), 0.5f, 1f));
            }

            if (Drone.instance.Roll < 0)
            {
                temp.y *= (Mathf.Clamp(Mathf.Abs(Drone.instance.Roll), 0.5f, 1f));
            }
            else if (Drone.instance.Roll > 0)
            {
                temp.y *= (Mathf.Clamp(1 - Mathf.Abs(Drone.instance.Roll), 0.25f, 0.5f));
            }
        }
        BladeFour.transform.Rotate(temp);
    }
    private void RotateBladeOne()
    {
        Vector3 temp = new Vector3(0, 3 * RotationsPerMinute * Time.deltaTime, 0);//rotate a certain amount per minute by default
        if (Drone.instance.Pitch != 0 || Drone.instance.Roll != 0)
        {
            temp = new Vector3(0, 15 * RotationsPerMinute * Time.deltaTime, 0);
            if (Drone.instance.Pitch > 0) //if going forward
            {
                temp.y *= (Mathf.Clamp(Drone.instance.Pitch, 0.5f, 1f));
            }
            else if (Drone.instance.Pitch < 0) //if going back
            {
                temp.y *= (Mathf.Clamp(1 - Mathf.Abs(Drone.instance.Pitch), 0.25f, 0.5f));
            }

            if (Drone.instance.Roll < 0)
            {
                temp.y *= (Mathf.Clamp(Mathf.Abs(Drone.instance.Roll), 0.5f, 1f));
            }
            else if (Drone.instance.Roll > 0)
            {
                temp.y *= (Mathf.Clamp(1 - Mathf.Abs(Drone.instance.Roll), 0.25f, 0.5f));
            }
        }
        BladeOne.transform.Rotate(temp);
    }
    private void RotateBladeTwo()
    {
        Vector3 temp = new Vector3(0, 3 * RotationsPerMinute * Time.deltaTime, 0);//rotate a certain amount per minute by default
        if (Drone.instance.Pitch != 0 || Drone.instance.Roll != 0)
        {
            temp = new Vector3(0, 15 * RotationsPerMinute * Time.deltaTime, 0);
            if (Drone.instance.Pitch > 0) //if going forward
            {
                temp.y *= (Mathf.Clamp(Drone.instance.Pitch, 0.5f, 1f));
            }
            else if (Drone.instance.Pitch < 0) //if going back
            {
                temp.y *= (Mathf.Clamp(1 - Mathf.Abs(Drone.instance.Pitch), 0.25f, 0.5f));
            }

            if (Drone.instance.Roll < 0)
            {
                temp.y *= (Mathf.Clamp(1 - Drone.instance.Roll, 0.25f, 0.5f));
            }
            else if (Drone.instance.Roll > 0)
            {
                temp.y *= (Mathf.Clamp( Mathf.Abs(Drone.instance.Roll), 0.5f, 1f));
            }
        }
        BladeTwo.transform.Rotate(temp);
    }
}
