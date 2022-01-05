using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buoyancy : MonoBehaviour {

    public List<GameObject> m_BuoyancyBalls;
    public GameObject m_WaterPlane;
    public float bounceDamp = 0.5f;
    public float forcePerMeter = 0.5f;
    public float floatHeightOffset = 0.0f;

    public float speed = 10.0F;
    public float rotationSpeed = 100.0F;

    private float rhoWater = 1027f;
    public float buoyancyMultiplier = 1.0f;

    // Use this for initialization
    void Start ()
    {
		
	}

    void Update()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
		foreach (GameObject buoyancyBall in m_BuoyancyBalls)
        {
            /*
            if (buoyancyBall.transform.position.y < m_WaterPlane.transform.position.y)
            {
                float distance = (m_WaterPlane.transform.position.y + floatHeightOffset) - buoyancyBall.transform.position.y;

                //float forceFactor = 1f - ((actionPoint.y - waterLevel) / floatHeight);
                float bounceDamping = Mathf.Clamp((gameObject.GetComponent<Rigidbody>().velocity.y * bounceDamp), 0.0f, 1000.0f);
                float forceMultiplier = Mathf.Clamp((forcePerMeter * distance), 0.0f, 10.0f);

                forceMultiplier = Mathf.Clamp(forceMultiplier - bounceDamping, 0.0f, 1000.0f);

                Vector3 uplift = /*(-Physics.gravity / m_BuoyancyBalls.Count) + *///(Vector3.up * forceMultiplier * gameObject.GetComponent<Rigidbody>().mass);
                                                                                  //gameObject.GetComponent<Rigidbody>().AddForceAtPosition(uplift, buoyancyBall.transform.position, ForceMode.Force);

            //}

            gameObject.GetComponent<Rigidbody>().AddForceAtPosition(GetBuoyancyForce(rhoWater, buoyancyBall), buoyancyBall.transform.position, ForceMode.Force);
        }

        //float translation = Input.GetAxis("Vertical") * speed;
        //float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
        //translation *= Time.deltaTime;
        //rotation *= Time.deltaTime;
        //gameObject.GetComponent<Rigidbody>().AddTorque(gameObject.transform.up * rotation);

        //Vector3 offset = (gameObject.transform.forward * -1.0f) + (gameObject.transform.up * -0.0f);
        //gameObject.GetComponent<Rigidbody>().AddForceAtPosition(gameObject.transform.forward * translation, gameObject.transform.position + offset, ForceMode.Force);
    }

    public void ForwardThrust(float amount)
    {
        Vector3 offset = (gameObject.transform.forward * -1.0f) + (gameObject.transform.up * -0.1f);
        gameObject.GetComponent<Rigidbody>().AddForceAtPosition(gameObject.transform.forward * amount, gameObject.transform.position + offset, ForceMode.Force);
    }

    private Vector3 GetBuoyancyForce(float rho, GameObject buoyancyBall)
    {
        //Buoyancy is a hydrostatic force - it's there even if the water isn't flowing or if the boat stays still

        // F_buoyancy = rho * g * V
        // rho - density of the mediaum you are in
        // g - gravity
        // V - volume of fluid directly above the curved surface 

        // V = z * S * n 
        // z - distance to surface
        // S - surface area
        // n - normal to the surface
        float radius = buoyancyBall.transform.localScale.x * buoyancyMultiplier;
        float surfaceArea = (radius * radius * Mathf.PI);
        float waterHeight = m_WaterPlane.GetComponent<WaveController>().GetWaveYPos(buoyancyBall.transform.position);
        float distanceToSurface = Mathf.Max(waterHeight - buoyancyBall.transform.position.y, 0.0f);

        Vector3 buoyancyForce = rho * -Physics.gravity.y * distanceToSurface * surfaceArea * Vector3.up;

        //The vertical component of the hydrostatic forces don't cancel out but the horizontal do
        buoyancyForce.x = 0f;
        buoyancyForce.z = 0f;

        return buoyancyForce;
    }
}
