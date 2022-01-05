using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour {

	public float m_MaxHealth = 100.0f;
	public List<DamageThreshold> m_DamageThresholds;
	GameObject scriptsBucket;

	float m_CurrentHealth;

	// Use this for initialization
	void Start () {
		scriptsBucket = GameObject.Find("ScriptsBucket");
		m_CurrentHealth = m_MaxHealth;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void TakeDamage(float _damage, Vector3 _impactLocation)
	{
		m_CurrentHealth -= _damage;
		Debug.Log(gameObject + " took " + _damage + " damage!");

		UpdateVisual();

		BoatControler boatController = this.GetComponent<BoatControler> ();
		if (boatController != null)
		{
			boatController.OnTakeDamage(_impactLocation);
		}

		if (m_CurrentHealth <= 0.0f)
		{
			Die ();
		}
	}

	void Die(){
		// disable collisions
		Collider collider = GetComponent<Collider>();
		if (collider != null)
			collider.enabled = false;
		
		BoatControler bc = this.GetComponent<BoatControler> ();
		if (bc != null) {	//To check that this is really a player
			bc.Die();
			bc.player.alive = false;
			int numberOfPlayersAlive = 0;
			foreach (Player p in StaticData.playerList) {
				if (p.alive) {
					numberOfPlayersAlive++;
				}
			}
			if (numberOfPlayersAlive < 2) {
				scriptsBucket.GetComponent<GameStateManager> ().ChangeGameState (StaticData.AvailableGameStates.Paused);
			}
		}
	}

	void UpdateVisual()
	{
		float currentHealthPercentage = (m_CurrentHealth / m_MaxHealth) * 100.0f;
		DamageThreshold wantedThreshold = null;
		float highestValidThreshold = 0.0f;
		for (int i = 0; i < m_DamageThresholds.Count; ++i)
		{
			if (currentHealthPercentage < m_DamageThresholds[i].m_WhenBelowPercentage)
			{
				if (m_DamageThresholds[i].m_WhenBelowPercentage > highestValidThreshold)
				{
					wantedThreshold = m_DamageThresholds[i];
				}
			}
		}

		if (wantedThreshold != null && wantedThreshold.m_Visual != null)
		{
			GetComponent<MeshFilter>().mesh = wantedThreshold.m_Visual;
		}
	}

	public void Reset()
	{
		m_CurrentHealth = m_MaxHealth;
		UpdateVisual();
	}
}

[System.Serializable]
public class DamageThreshold
{
	public float m_WhenBelowPercentage;
	public Mesh m_Visual;
}