using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpBetween : MonoBehaviour {
	public Vector3 mainPos, secondPos, thirdPos;
	public Vector3 mainRot, secondRot, thirdRot;

    private Vector3 currentAngle;

    public bool moveToMain;
    public bool moveToOptions;
    public bool moveToDrone;

	public float moveSpeed;
    public float rotSpeed;

	// Use this for initialization
	void Start () {
        transform.position = mainPos;
        transform.eulerAngles = mainRot;
        currentAngle = transform.eulerAngles;
	}
	
	// Update is called once per frame
	void Update () {
		if(moveToDrone)
		{
			transform.position = Vector3.Lerp(transform.position, secondPos, moveSpeed * Time.deltaTime);
            currentAngle = new Vector3(Mathf.LerpAngle(currentAngle.x, secondRot.x, Time.deltaTime * rotSpeed),
                Mathf.LerpAngle(currentAngle.y, secondRot.y, Time.deltaTime * rotSpeed),
                0);
            transform.eulerAngles = currentAngle;
            if(Vector3.Angle(transform.eulerAngles, secondRot) < 0.1f && Vector3.Distance(transform.position, secondPos) < 1f)
            {
                moveToDrone = false;
            }
		}
        else if(moveToMain)
        {
            transform.position = Vector3.Lerp(transform.position, mainPos, moveSpeed * Time.deltaTime);
            currentAngle = new Vector3(Mathf.LerpAngle(currentAngle.x, mainRot.x, Time.deltaTime * rotSpeed),
                Mathf.LerpAngle(currentAngle.y, mainRot.y, Time.deltaTime * rotSpeed),
                0);
            transform.eulerAngles = currentAngle;
            if (Vector3.Angle(transform.eulerAngles, mainRot) < 0.1f && Vector3.Distance(transform.position, mainPos) < 1f)
            {
                moveToMain = false;
            }
        }
        else if (moveToOptions)
        {
            transform.position = Vector3.Lerp(transform.position, thirdPos, moveSpeed * Time.deltaTime);
            currentAngle = new Vector3(Mathf.LerpAngle(currentAngle.x, thirdRot.x, Time.deltaTime * rotSpeed),
                Mathf.LerpAngle(currentAngle.y, thirdRot.y, Time.deltaTime * rotSpeed),
                0);
            transform.eulerAngles = currentAngle;
            if (Vector3.Angle(transform.eulerAngles, thirdRot) < 0.1f && Vector3.Distance(transform.position, thirdPos) < 1f)
            {
                moveToOptions = false;
            }
        }
	}
    public void StartMovement(string str)
    {
        if(!moveToMain && !moveToDrone && !moveToOptions)
        {
            if(str == "main")
            {
                moveToMain = true;
            }
            else if(str == "options")
            {
                moveToOptions = true;
            }
            else if(str == "drone")
            {
                moveToDrone = true;
            }
        }
    }
}
