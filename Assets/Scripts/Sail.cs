using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sail : MonoBehaviour {

	public float rotationSpeed;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		Quaternion desiredRotation = Quaternion.LookRotation(
			new Vector3(StaticData.windDirection.x, 0.0f, StaticData.windDirection.y),
		    Vector3.up
		);
		transform.localRotation = Quaternion.Slerp(transform.localRotation, desiredRotation, Time.deltaTime * rotationSpeed);
	}
}
