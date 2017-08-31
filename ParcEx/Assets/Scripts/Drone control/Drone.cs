using Delivery;
using Pertinate.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TODO: Try to implement a cruise control type of control.
/// </summary>

public class Drone : MonoBehaviour
{
    public static Drone instance;

    public bool stopMotors = false;

    public GameObject drone;
    public GameObject Player;

    public VirtualJoystick leftJoystick;
    public VirtualJoystick rightJoystick;

    public Rigidbody rb;

    public float speedAdditive = 1f;

    public float yVel;

    public float initialMass = 20f;
    public float packageMassMultiplier = 1f;
    public bool isRotating = false;

    [Header("-Throttle Section-")]
    [Range(-1, 1)]
    public float Throttle;
    public float throttleMin = 50f;
    public float throttleMax = 20f;
    [Header("-Yaw Section-")]
    [Range(-1, 1)]
    public float Yaw;
    public float rotateMultiplier = 10f;

    [Header("-Roll Section-")]
    [Range(-1, 1)]
    public float Roll;
    public float rollMultiplier = 50f;
    [Header("-Pitch Section-")]
    [Range(-1, 1)]
    public float Pitch;
    public float pitchMultiplier = 50f;

    private void ThrottleControl()
    {
        if (Throttle != 0 && (Throttle > VirtualJoystick.deadZone || Throttle < -VirtualJoystick.deadZone))
        {
            rb.AddForce(Vector3.up * Throttle * ((Throttle > 0) ? throttleMax : throttleMin), ForceMode.Impulse);
        }
    }

    private void YawControl()
    {
        if (Yaw != 0 && (Yaw > VirtualJoystick.deadZone || Yaw < -VirtualJoystick.deadZone))
        {
            Player.transform.Rotate(Vector3.up * Yaw * rotateMultiplier * Time.deltaTime);
        }
    }

    private void PitchControl()
    {
        if(Pitch != 0 && (Pitch > VirtualJoystick.deadZone || Pitch < -VirtualJoystick.deadZone))
        {
            rb.AddForce(Player.transform.forward * Pitch * pitchMultiplier, ForceMode.Force);
        }
    }

    private void RollControl()
    {
        if(Roll != 0 && (Roll > VirtualJoystick.deadZone || Roll < -VirtualJoystick.deadZone))
        {
            rb.AddForce(Player.transform.right * Roll * rollMultiplier, ForceMode.Force);
        }
    }

    private void RotationControl()
    {
        transform.eulerAngles = new Vector3(Mathf.LerpAngle(transform.eulerAngles.x, 45 * Pitch, Time.deltaTime), transform.eulerAngles.y, Mathf.LerpAngle(transform.eulerAngles.z, 45 * -Roll, Time.deltaTime));
    }

    private void Float()
    {
        float temp = rb.mass / initialMass;
        yVel = rb.velocity.y + Physics.gravity.y + ((temp != 1) ? temp : 0);
        rb.AddForce(0, -yVel, 0, ForceMode.Acceleration);
    }

    private void FixedUpdate()
    {
        Float();
        if (!stopMotors)
        {
            ThrottleControl();
            PitchControl();
            YawControl();
            RollControl();
            RotationControl();
        }
    }

    private void Awake()
    {
        instance = this;
        if(drone == null)
            drone = transform.gameObject;
        if(Player == null)
            Player = transform.parent.gameObject;
        if (initialMass != rb.mass)
            initialMass = rb.mass;
        leftJoystick.onValueChange.AddListener((Vector3 x) =>
        {
            Throttle = x.y;
            Yaw = x.x;
        });
        rightJoystick.onValueChange.AddListener((Vector3 x) =>
        {
            Pitch = x.y;
            Roll = x.x;
        });
    }
}