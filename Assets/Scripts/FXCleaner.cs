using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXCleaner : MonoBehaviour {
	public float m_CleanAfterDuration = 5.0f;
	float m_TimeUntilRemoval = 0.0f;

	// Use this for initialization
	void Start () {
		m_TimeUntilRemoval = m_CleanAfterDuration;
	}
	
	// Update is called once per frame
	void Update () {
		m_TimeUntilRemoval -= Time.fixedDeltaTime;
		if (m_TimeUntilRemoval <= 0.0f)
			GameObject.DestroyObject(gameObject);
	}
}
