using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceTrigger : MonoBehaviour {

    public float m_RepulseForce;
    public bool m_IsSpherical;

	// Use this for initialization
	void Start () {
		
	}

    void OnTriggerStay(Collider collider)
    {
        Vector3 forceVector = transform.forward;
        if (m_IsSpherical)
        {
            forceVector = collider.gameObject.transform.position - transform.position;
        }
        collider.gameObject.transform.root.GetComponentInChildren<Rigidbody>().AddForce(forceVector * m_RepulseForce);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
