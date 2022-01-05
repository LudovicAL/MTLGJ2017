using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLevelDecal : MonoBehaviour {

    private float m_ExtraHeight = 0.5f;
    public WaveController m_WaveController;
    public bool m_HasWaveController = false;

	// Use this for initialization
	void Start () {
    }

    public void SetWaveController(WaveController waveController)
    {
        m_WaveController = waveController;
        m_HasWaveController = true;
    }

	// Update is called once per frame
	void Update () {
		if (m_HasWaveController)
        {
            float waterHeightAtThisPosition = m_WaveController.GetWaveYPos(transform.position);
            //Debug.Log("Decal Height: " + transform.position.y + "Water Height: " +waterHeightAtThisPosition);
            transform.position = new Vector3(transform.position.x, waterHeightAtThisPosition + 0.3f, transform.position.z);
        }
	}
}
