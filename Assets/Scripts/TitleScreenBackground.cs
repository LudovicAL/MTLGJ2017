using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenBackground : MonoBehaviour {

	public float m_Multiplier = 10.0f;
	float initialY;

	// Use this for initialization
	void Start () {
		initialY = GetComponent<RectTransform>().anchoredPosition.y;
	}
	
	// Update is called once per frame
	void Update () {
		RectTransform rectTransform = GetComponent<RectTransform>();
		rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x, initialY + Mathf.Sin(Time.timeSinceLevelLoad) * m_Multiplier, 0.0f);
	}
}
