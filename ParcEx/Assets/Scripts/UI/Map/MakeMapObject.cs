using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MakeMapObject : MonoBehaviour {
	public Image image;
	private void OnEnable () {
		MapController.RegisterMapObject(gameObject, image);
	}
	private void OnDisable()
	{
		MapController.RemoveMapObject(gameObject);
	}
}
