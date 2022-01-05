using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour {

	public float m_Damage = 20.0f;
	public AudioClip m_WaterSplooshSound;
	public GameObject m_ExplosionFX;
    public GameObject m_WaterSplooshFX;
    public WaveController m_WaveController;

    float m_LifeTimeRemaining = 5.0f;
	bool m_HasObjectRequestedDestruction = false; // GameObject.Destroy(self), need this check in case hit twice in one frame

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		m_LifeTimeRemaining -= Time.fixedDeltaTime;
		if (m_LifeTimeRemaining <= 0.0f)
			Object.Destroy(gameObject);

        float waveHeight = m_WaveController.GetWaveYPos(transform.position);
        if (m_WaveController != null && waveHeight > transform.position.y)
        {
            if (m_ExplosionFX != null)
            {
                GameObject cannonBall = GameObject.Instantiate(m_WaterSplooshFX, transform.position, Quaternion.identity, null);
                cannonBall.transform.Rotate(Vector3.right, -90.0f);
            }

            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
                audioSource.PlayOneShot(m_WaterSplooshSound);

            // destroy self
            Object.Destroy(gameObject);
            m_HasObjectRequestedDestruction = true;
        }

	}

    public void SetWaveController(WaveController waveController)
    {
        m_WaveController = waveController;
    }

	void OnCollisionEnter(Collision _collision)
	{
		if (m_HasObjectRequestedDestruction)
			return;
		
		Damageable damageComponent = _collision.transform.GetComponent<Damageable>();
		if (damageComponent == null)
            damageComponent = _collision.transform.root.GetComponentInChildren<Damageable>();
		
		if (damageComponent != null)
		{
			damageComponent.TakeDamage(m_Damage, _collision.contacts[0].point);

			if (m_ExplosionFX != null)
			{
				GameObject.Instantiate(m_ExplosionFX, transform.position, Quaternion.identity, null);
			}

			// destroy self
			Object.Destroy(gameObject);
			m_HasObjectRequestedDestruction = true;
		}
	}
}
