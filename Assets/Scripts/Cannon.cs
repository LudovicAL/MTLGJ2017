using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour {

	public float m_CooldownDuration = 2.0f;
	public GameObject m_CannonBall;
	public float m_CannonBallSpawnDistance = 1.5f;
	public float m_InitialCannonBallForce = 5.0f;
	public AudioClip m_ShootingSound;
	public GameObject m_ShootingFX;

	float m_CooldownRemaining = 0.0f;

	// Use this for initialization
	void Start () {
		UpdateColor();
	}
	
	// Update is called once per frame
	void Update () {
		Debug.DrawLine(transform.position, transform.position + (transform.forward * m_CannonBallSpawnDistance));

		UpdateCooldown();
	}

	public bool Fire(WaveController waveController)
	{
		bool success = false;

		if (m_CooldownRemaining <= 0.0f)
		{
			success = true;

			// cannonball
			GameObject newCannonball = GameObject.Instantiate(m_CannonBall, transform.position + (transform.forward * m_CannonBallSpawnDistance), Quaternion.identity, null);
			newCannonball.GetComponent<Rigidbody>().AddForce(transform.forward * m_InitialCannonBallForce, ForceMode.Impulse);
            newCannonball.GetComponent<CannonBall>().SetWaveController(waveController);

			// sound
			AudioSource audioSource = GetComponent<AudioSource>();
			if (audioSource != null)
				audioSource.PlayOneShot(m_ShootingSound);

			// fx
			GameObject spawnedFX = GameObject.Instantiate(m_ShootingFX, transform.position, transform.rotation, transform);
			spawnedFX.transform.localPosition = Vector3.zero;

			m_CooldownRemaining = m_CooldownDuration;
		}

		Debug.Log("Cannon ball " + (success ? "fired" : "not fired"));
		return success;
	}

	void UpdateCooldown()
	{
		if (m_CooldownRemaining > 0.0f)
		{
			m_CooldownRemaining -= Time.fixedDeltaTime;
			m_CooldownRemaining = Mathf.Max(m_CooldownRemaining, 0.0f);

			UpdateColor();
		}
	}

	void UpdateColor()
	{
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		if (meshRenderer != null)
		{
			Color cannonColor = m_CooldownRemaining > 0.0f ? Color.red : Color.black;
			meshRenderer.material.color = cannonColor;
		}
	}
}
