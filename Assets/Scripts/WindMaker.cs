using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindMaker : MonoBehaviour {

	public enum WindState {
		NoWind,	//Wind is off
		Increasing,	//Wind strength is increasing
		Waiting,	//Wind is at full strength for a couple seconds
		Decreasing,	//Wind strength is decreasing
		Rotating	//Wind is changing direction
	};

	public GameObject scriptsBucket;
	private StaticData.AvailableGameStates gameState;
	private WindState windState = WindState.NoWind;
	private Vector2 desiredWindDirection;
	private float desiredWindSpeed;
	private float targetTime;
	public float minWindStrength;
	public float maxWindStrength;

	// Use this for initialization
	void Start () {
		scriptsBucket.GetComponent<GameStateManager> ().MenuGameState.AddListener(OnMenu);
		scriptsBucket.GetComponent<GameStateManager> ().StartingGameState.AddListener(OnStarting);
		scriptsBucket.GetComponent<GameStateManager> ().PlayingGameState.AddListener(OnPlaying);
		scriptsBucket.GetComponent<GameStateManager> ().PausedGameState.AddListener(OnPausing);
		SetCanvasState (scriptsBucket.GetComponent<GameStateManager> ().GameState);
		desiredWindDirection = Vector2.one;
		desiredWindSpeed = 1.0f;
		targetTime = Time.time;

	}

	public void ToggleWind(bool isWindOn) {
		if (isWindOn) {
			windState = WindState.Decreasing;
		} else {
			windState = WindState.NoWind;
		}
	}

	// Update is called once per frame
	void Update () {
		if (gameState == StaticData.AvailableGameStates.Playing) {
			switch (windState) {
				case WindState.Rotating:
					Rotate ();
					break;
				case WindState.Increasing:
					Increase ();
					break;
				case WindState.Waiting:
					DoWait ();
					break;
				case WindState.Decreasing:
					Decrease();
					break;
				case WindState.NoWind:
					DoNoWind ();
					break;
			}
		}
	}

	//Moves the windState to the next WindState
	private void ChangeWindState() {
		switch (windState) {
			case WindState.Rotating:
				Debug.Log ("Increasing");
				windState = WindState.Increasing;
				break;
			case WindState.Increasing:
				Debug.Log ("Waiting");
				windState = WindState.Waiting;
				break;
			case WindState.Waiting:
				Debug.Log ("Decreasing");
				windState = WindState.Decreasing;
				break;
			case WindState.Decreasing:
				Debug.Log ("Rotating");
				windState = WindState.Rotating;
				break;
			default:	//When the WindState is 'NoWind'...
				break;
		}
	}

	//Slowly increases the wind strength toward its desired speed.
	private void DoNoWind() {
		if ((desiredWindSpeed - StaticData.windSpeed) > 0.1f) {
			StaticData.windSpeed = Mathf.MoveTowards (StaticData.windSpeed, desiredWindSpeed, Time.deltaTime);
		}
	}

	//Slowly increases the wind strength toward its desired speed.
	private void Increase() {
		StaticData.windSpeed = Mathf.MoveTowards (StaticData.windSpeed, desiredWindSpeed, Time.deltaTime);
		if ((desiredWindSpeed - StaticData.windSpeed) < 0.1f) {
			RandomizeTargetTime ();
			ChangeWindState ();
		}
	}

	//Slowly decreases the wind strength toward its desired speed.
	private void Decrease() {
		StaticData.windSpeed = Mathf.MoveTowards (StaticData.windSpeed, desiredWindSpeed, Time.deltaTime);
		if ((StaticData.windSpeed - desiredWindSpeed) < 0.1f) {
			RandomizeWindDirection ();
			ChangeWindState ();
		}
	}

	//Slowly rotates the wind direction toward its desired rotation
	private void Rotate() {
		StaticData.windDirection = Vector2.MoveTowards(StaticData.windDirection, desiredWindDirection, Time.deltaTime);
		if (Vector2.Distance(StaticData.windDirection, desiredWindDirection) < 0.1f) {
			RandomizeWindSpeed ();
			ChangeWindState ();
		}
	}

	//Do nothing until the time has come to change WindState
	private void DoWait() {
		if (Time.time >= targetTime) {
			desiredWindSpeed = 0.0f;
			ChangeWindState ();
		}
	}

	//Sets a random wait time
	private void RandomizeTargetTime() {
		targetTime = Time.time + Random.Range (0.0f, 5.0f);
	}

	//Sets a random desired wind speed
	private void RandomizeWindSpeed() {
		desiredWindSpeed = Random.Range (minWindStrength, maxWindStrength);
        StaticData.desiredWindSpeed = desiredWindSpeed;
    }

	//Sets a random desired wind direction
	private void RandomizeWindDirection() {
		desiredWindDirection = new Vector2 (Random.Range (-1.0f, 1.0f), Random.Range (-1.0f, 1.0f));
	}

	protected void OnMenu() {
		SetCanvasState (StaticData.AvailableGameStates.Menu);

	}

	protected void OnStarting() {
		SetCanvasState (StaticData.AvailableGameStates.Starting);

	}

	protected void OnPlaying() {
		SetCanvasState (StaticData.AvailableGameStates.Playing);

	}

	protected void OnPausing() {
		SetCanvasState (StaticData.AvailableGameStates.Paused);
	}

	private void SetCanvasState(StaticData.AvailableGameStates state) {
		gameState = state;
	}
}
