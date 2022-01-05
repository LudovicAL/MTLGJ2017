using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPresets : MonoBehaviour {
	public List<WaveDataSet> m_Presets;
	int m_CurrentPresetIdx = 0;

	// Use this for initialization
	void Start () {
		SendPresetChanges();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ChangePreset()
	{
		m_CurrentPresetIdx = (m_CurrentPresetIdx + 1) % m_Presets.Count;
		SendPresetChanges();
	}

	void SendPresetChanges()
	{
		GameObject.Find("WaterPlane").GetComponent<WaveController>().TransitionToPreset(m_Presets[m_CurrentPresetIdx], 5.0f);
		GameObject.Find("WindMaker").GetComponent<WindMaker>().ToggleWind(m_Presets[m_CurrentPresetIdx].m_WindEnabled);
	}

	public WaveDataSet GetCurrentPreset()
	{
		if (m_CurrentPresetIdx < m_Presets.Count)
			return m_Presets[m_CurrentPresetIdx];
		
		return null;
	}
}

[System.Serializable]
public class WaveDataSet
{
	public string m_Name;

	public float m_WaveHeight = 0.1f;
	public float m_WaveSpeed = 1.0f;
	public float m_WavePeakDistance = 1.0f;
	public float m_WaveAngle = 90.0f;

	public bool m_WindEnabled = false;
}