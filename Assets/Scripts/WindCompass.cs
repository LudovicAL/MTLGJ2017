using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindCompass : MonoBehaviour {

	private Rigidbody selfRigidbody;
	public float m_RabbitTurningForce;

	// Use this for initialization
	void Start () {
		selfRigidbody = this.GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		RotateToWind ();
	}

	public void RotateToWind() {
		//Rabbit vector
		Vector3 rabbitVector = new Vector3(StaticData.windDirection.x, 0.0f, StaticData.windDirection.y);

		Vector3 rightVector = selfRigidbody.gameObject.transform.right;
		rightVector.y = 0.0f;

		float theta = Vector3.Dot(rightVector.normalized, rabbitVector.normalized);
		if (theta > 0.0) {
			selfRigidbody.AddTorque(Vector3.up * m_RabbitTurningForce * Mathf.Abs(theta));
		} else {
			selfRigidbody.AddTorque(Vector3.up * -m_RabbitTurningForce * Mathf.Abs(theta));
		}
	}
}
