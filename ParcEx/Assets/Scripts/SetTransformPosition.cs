using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTransformPosition : MonoBehaviour {
	public Transform transf;
    // Update is called once per frame
    void Update() {
        transform.position = transf.position;
        transform.localRotation = transf.localRotation;

    }
}
