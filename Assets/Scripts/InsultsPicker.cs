using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsultsPicker : MonoBehaviour {

	public List<AudioClip> m_Insults;
	List<AudioClip> m_UnusedInsults;

	// Use this for initialization
	void Start () {
		m_UnusedInsults = new List<AudioClip>();
		foreach (AudioClip clip in m_Insults)
		{
			m_UnusedInsults.Add(clip);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public AudioClip GetRandomInsult()
	{
		if (m_Insults.Count == 0)
			return null;

		int randomIndex = Random.Range(0, m_Insults.Count);
		m_UnusedInsults.Remove(m_Insults[randomIndex]);

		UpdateUnused();

		return m_Insults[randomIndex]; 
	}

	public AudioClip GetUnsusedInsult()
	{
		if (m_Insults.Count == 0)
			return null;

		int randomIndex = Random.Range(0, m_UnusedInsults.Count);
		AudioClip clip = m_UnusedInsults[randomIndex];
		m_UnusedInsults.Remove(clip);

		UpdateUnused();

		return clip; 
	}

	void UpdateUnused()
	{
		if (m_UnusedInsults.Count == 0)
		{
			foreach (AudioClip clip in m_Insults)
			{
				m_UnusedInsults.Add(clip);
			}
		}
	}
}
