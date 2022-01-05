using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatControler : MonoBehaviour {

	public GameObject scriptsBucket;
	public Player player;
	public GameObject cannonFlyingOff;

	private Vector3 initialPosition;
	private Quaternion initialRotation;
	private StaticData.AvailableGameStates gameState;
	private Rigidbody selfRigidbody;
	private int nextLeftCannonIdx = 0;
	private int nextRightCannonIdx = 0;
	List<Cannon> leftCannons = new List<Cannon>();
	List<Cannon> rightCannons = new List<Cannon>();
	bool leftCannonInputReleased = true;
	bool rightCannonInputReleased = true;

    public float m_RabbitVerticalOffset = 0.0f;
    private Vector3 m_MostRecentGoodRabbitPosition = new Vector3(0.0f, 0.0f, -1.0f);
    private bool m_AreSailsUp;
    public GameObject m_Rabbit;
    public float m_RabbitTurningForce;
    public float m_MaxSpeed;
    private float m_Speed;
    public float m_AccelerationSpeed;
    public float m_DeccelerationSpeed;
	public float m_DestroyedCannonImpulse = 6.0f;
	public float m_DestroyedCannonTorque = 10.0f;

    public GameObject m_WakeFX;
    private bool m_HasWake = false;
    public GameObject m_WakeQuad;
    private float m_WakeQuadTimer;

    WaveController m_WaveController;

    void Awake () {
		selfRigidbody = this.GetComponent<Rigidbody> ();
	}

	// Use this for initialization
	void Start () {
		initialPosition = transform.root.gameObject.transform.position;
		initialRotation = transform.root.gameObject.transform.rotation;
		scriptsBucket.GetComponent<GameStateManager> ().MenuGameState.AddListener(OnMenu);
		scriptsBucket.GetComponent<GameStateManager> ().StartingGameState.AddListener(OnStarting);
		scriptsBucket.GetComponent<GameStateManager> ().PlayingGameState.AddListener(OnPlaying);
		scriptsBucket.GetComponent<GameStateManager> ().PausedGameState.AddListener(OnPausing);
		SetCanvasState (scriptsBucket.GetComponent<GameStateManager> ().GameState);
		Debug.Log("Boat Starting game as " + gameState);
		OrganizeCannons();

        m_WaveController = gameObject.GetComponent<Buoyancy>().m_WaterPlane.GetComponent<WaveController>();

    }

	// Update is called once per frame
	void FixedUpdate () {
		Debug.DrawLine(transform.position, transform.position + (transform.forward * 3.0f));

		if (gameState == StaticData.AvailableGameStates.Playing || gameState == StaticData.AvailableGameStates.Starting)
        {
            Vector3 localOffset = new Vector3(Input.GetAxisRaw(player.cm.horizontal), 0.0f, -Input.GetAxisRaw(player.cm.vertical));
            if (localOffset.magnitude > 0.25f)
            {
                SetTheRabbit(localOffset);
                m_AreSailsUp = true;
            }
            else
            {
                SetTheRabbit(m_MostRecentGoodRabbitPosition);
                m_AreSailsUp = false;
            }

            //Move foward
            if (m_AreSailsUp)
            {
                m_Speed += (Time.fixedDeltaTime * m_AccelerationSpeed);
                m_Speed = Mathf.Clamp(m_Speed, 0.0f, m_MaxSpeed);
			}
            else
            {
                m_Speed -= (Time.fixedDeltaTime * m_DeccelerationSpeed);
                m_Speed = Mathf.Max(m_Speed, 0.0f);
            }

            if (m_Speed > 500)
            {
                if (!m_HasWake)
                {
                    m_HasWake = true;
                    m_WakeFX.GetComponent<ParticleSystem>().Play();
                    Debug.Log("Start Wake");
                }
            }
            else
            {
                if (m_HasWake)
                {
                    m_WakeFX.GetComponent<ParticleSystem>().Stop();
                    m_HasWake = false;
                    Debug.Log("STop Wake");
                }
            }

            if (m_HasWake)
            {
                m_WakeQuadTimer += Time.deltaTime;
                if (m_WakeQuadTimer > 0.05f)
                {
                    GameObject spawnedWakeQuad = GameObject.Instantiate(m_WakeQuad, transform.position, Quaternion.identity);
                    spawnedWakeQuad.GetComponent<WaterLevelDecal>().SetWaveController(m_WaveController);
                    spawnedWakeQuad.transform.Rotate(Vector3.right, 90.0f);
                    m_WakeQuadTimer = 0.0f;
                }

            }

            PushBoatForwards();

            RotateToRabbit();

			if (gameState == StaticData.AvailableGameStates.Playing)
            	HandleShootingInput();
		}
	}

	//Moves the boat foward
	public void PushBoatForwards(){
        gameObject.GetComponent<Buoyancy>().ForwardThrust(m_Speed);
	}

    public void SetTheRabbit(Vector3 localOffset)
    {
        m_MostRecentGoodRabbitPosition = localOffset;
        m_Rabbit.transform.position = (localOffset.normalized * 2.0f) + transform.position + (Vector3.up * m_RabbitVerticalOffset);
    }

    public void RotateToRabbit()
    {
        //Rabbit vector
        Vector3 rabbitVector = (m_Rabbit.transform.position - (Vector3.up * m_RabbitVerticalOffset)) - selfRigidbody.gameObject.transform.position;
        rabbitVector.y = 0.0f;

        Vector3 frontVector = selfRigidbody.gameObject.transform.forward;
        frontVector.y = 0.0f;

        Vector3 rightVector = selfRigidbody.gameObject.transform.right;
        rightVector.y = 0.0f;

        float theta = Vector3.Dot(rightVector.normalized, rabbitVector.normalized);
        float angle = 1.0f + (Mathf.Acos(Vector3.Dot(frontVector.normalized, rabbitVector.normalized)) / Mathf.PI);
        //Debug.Log("Boat Angle " + angle);
        if (theta > 0.0)
        {
            selfRigidbody.AddTorque(Vector3.up * m_RabbitTurningForce * Mathf.Abs(angle));
        }
        else
        {
            selfRigidbody.AddTorque(Vector3.up * -m_RabbitTurningForce * Mathf.Abs(angle));
        }
    }

	//Resets the boat to its original position and rotation
	public void Reset() {
		if (player != null) {
			player.alive = true;
			transform.root.gameObject.SetActive (true);
			transform.root.gameObject.transform.position = initialPosition;
			transform.root.gameObject.transform.rotation = initialRotation;
			Damageable damageComponent = transform.root.transform.GetComponentInChildren<Damageable>();
			if (damageComponent != null)
				damageComponent.Reset();
			foreach (Cannon cannon in leftCannons)
				cannon.gameObject.SetActive(true);
			foreach (Cannon cannon in rightCannons)
				cannon.gameObject.SetActive(true);
			nextLeftCannonIdx = 0;
			nextRightCannonIdx = 0;
		} else {
			transform.root.gameObject.SetActive (false);
		}
	}

	public void Die()
	{
		transform.root.gameObject.transform.position = new Vector3(100000, 0.0f, 0.0f);
	}

	protected void OnMenu() {
		SetCanvasState (StaticData.AvailableGameStates.Menu);
	}

	protected void OnStarting() {
		SetCanvasState (StaticData.AvailableGameStates.Starting);
		Reset ();
	}

	protected void OnPlaying() {
		SetCanvasState (StaticData.AvailableGameStates.Playing);
	}

	protected void OnPausing() {
		SetCanvasState (StaticData.AvailableGameStates.Paused);
	}

	public void SetCanvasState(StaticData.AvailableGameStates state) {
		gameState = state;
	}

	void OrganizeCannons()
	{
		Cannon[] cannons = transform.root.GetComponentsInChildren<Cannon>();
		for (int i = 0; i < cannons.Length; ++i)
		{
			Cannon cannon = cannons[i];

			if (IsPositionOnRightSideOfBoat(cannon.transform.position))
			{
				rightCannons.Add(cannon);
				Debug.Log("Found right cannon " + cannon);
			}
			else
			{
				leftCannons.Add(cannon);
				Debug.Log("Found left cannon " + cannon);
			}
		}
	}

	void HandleShootingInput()
	{
		// left cannons
		if (Input.GetAxisRaw(player.cm.fireLeft) != 0.0f) 
		{
			Debug.Log("player " + player.id + " input:" + player.cm.fireRight + " fire left input: " + Input.GetAxisRaw(player.cm.fireLeft));

			if (leftCannonInputReleased && leftCannons.Count > 0 && nextLeftCannonIdx != -1) 
			{
				bool fireSuccess = leftCannons[nextLeftCannonIdx].Fire(m_WaveController);
				if (fireSuccess)
					nextLeftCannonIdx = FindNextActiveCannon(leftCannons, nextLeftCannonIdx + 1);

				leftCannonInputReleased = false;
			}
		}
		else
		{
			leftCannonInputReleased = true;
		}

		// Right cannons
		if (Input.GetAxis(player.cm.fireRight) != 0) 
		{
			Debug.Log("player " + player.id + " input:" + player.cm.fireRight + " fire right input: " + Input.GetAxisRaw(player.cm.fireRight));

			if (rightCannonInputReleased && rightCannons.Count > 0 && nextRightCannonIdx != -1) 
			{
				bool fireSuccess = rightCannons[nextRightCannonIdx].Fire(m_WaveController);
				if (fireSuccess)
					nextRightCannonIdx = FindNextActiveCannon(rightCannons, nextRightCannonIdx + 1);
				
				rightCannonInputReleased = false;
			}
		}
		else 
		{
			rightCannonInputReleased = true;
		}
	}

	int FindNextActiveCannon(List<Cannon> _cannonList, int _startingIdx)
	{
		int activeCannonIdx = -1;

		int firstIdx = _startingIdx == -1 ? 0 : _startingIdx;
		for (int i = 0; i < _cannonList.Count; ++i)
		{
			int indexToCheck = (firstIdx + i) % _cannonList.Count;

			// disabled
			if (!_cannonList[indexToCheck].gameObject.activeSelf)
				continue;

			activeCannonIdx = indexToCheck;
			break;
		}

		return activeCannonIdx;
	}

	bool IsPositionOnRightSideOfBoat(Vector3 _position)
	{
		Vector3 shipCenter = transform.root.position;
		float dotProduct = Vector3.Dot(_position - shipCenter, transform.right);
		if (dotProduct > 0) // < 180 degrees
			return true;
		else
			return false;
	}

	public void OnTakeDamage(Vector3 _impactLocation)
	{
		Cannon closestActiveCannon = null;
		float smallestSqrDist = 1000000;

		// left
		foreach(Cannon cannon in leftCannons)
		{
			if (!cannon.isActiveAndEnabled)
				continue;

			float impactToCannonSqrDist = Vector3.SqrMagnitude(_impactLocation - cannon.transform.position);
			if (impactToCannonSqrDist < smallestSqrDist)
			{
				smallestSqrDist = impactToCannonSqrDist;
				closestActiveCannon = cannon;
			}
		}

		// right
		foreach(Cannon cannon in rightCannons)
		{
			if (!cannon.isActiveAndEnabled)
				continue;

			float impactToCannonSqrDist = Vector3.SqrMagnitude(_impactLocation - cannon.transform.position);
			if (impactToCannonSqrDist < smallestSqrDist)
			{
				smallestSqrDist = impactToCannonSqrDist;
				closestActiveCannon = cannon;
			}
		}

		if (closestActiveCannon != null)
		{
			closestActiveCannon.gameObject.SetActive(false);
			if (cannonFlyingOff)
			{
				GameObject destroyedCannon = GameObject.Instantiate(cannonFlyingOff, closestActiveCannon.transform.position, closestActiveCannon.transform.rotation, null);
				Vector3 offset = (-destroyedCannon.transform.up) * Random.Range(0.05f, 0.5f) + destroyedCannon.transform.right * Random.Range(-0.5f, 0.5f);
				destroyedCannon.GetComponent<Rigidbody>().AddForce(Vector3.up * m_DestroyedCannonImpulse + destroyedCannon.transform.right * Random.Range(-0.5f, 0.5f), ForceMode.Impulse);
				destroyedCannon.GetComponent<Rigidbody>().AddTorque(destroyedCannon.transform.right * m_DestroyedCannonTorque, ForceMode.Impulse);
				destroyedCannon.GetComponent<MeshRenderer>().material.color = Color.black;
			}

			// update next shooting cannon
			nextLeftCannonIdx = FindNextActiveCannon(leftCannons, nextLeftCannonIdx);
			nextRightCannonIdx = FindNextActiveCannon(rightCannons, nextRightCannonIdx);
		}
	}
}
