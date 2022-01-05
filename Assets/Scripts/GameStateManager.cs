using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class GameStateManager : MonoBehaviour {

	public UnityEvent MenuGameState;
	public UnityEvent StartingGameState;
	public UnityEvent PausedGameState;
	public UnityEvent PlayingGameState;
	private StaticData.AvailableGameStates gameState;

	void Awake () {
		if (MenuGameState == null) {
			MenuGameState = new UnityEvent();
		}
		if (StartingGameState == null) {
			StartingGameState = new UnityEvent();
		}
		if (PlayingGameState == null) {
			PlayingGameState = new UnityEvent();
		}
		if (PausedGameState == null) {
			PausedGameState = new UnityEvent();
		}
		GameObject canvasObject = GameObject.Find("Canvas");
		if (canvasObject != null && canvasObject.activeSelf)
			ChangeGameState(StaticData.AvailableGameStates.Menu);
		else
			ChangeGameState(StaticData.AvailableGameStates.Playing);

		Debug.Log("Game State Manager starting game as " + gameState);
	}

	void Update () {
		if (Input.GetButtonDown("Start")) {
			OnEscapeKeyPressed ();
		}
	}

	//Call this to change the game state
	public void ChangeGameState(StaticData.AvailableGameStates desiredState) {
		gameState = desiredState;
		switch(desiredState) {
			case StaticData.AvailableGameStates.Menu:
				MenuGameState.Invoke ();
				break;
			case StaticData.AvailableGameStates.Starting:
				StartingGameState.Invoke ();
				break;
			case StaticData.AvailableGameStates.Playing:
				PlayingGameState.Invoke ();
				break;
			case StaticData.AvailableGameStates.Paused:
				PausedGameState.Invoke ();
				break;
		}
		Debug.Log("Manager changing state " + gameState);
	}

	//Called when users presses the escape key
	public void OnEscapeKeyPressed() {
		if (gameState == StaticData.AvailableGameStates.Menu) {
			//Do nothing
		} else if (gameState == StaticData.AvailableGameStates.Starting) {
			//Do nothing
		} else if (gameState == StaticData.AvailableGameStates.Playing) {
			ChangeGameState (StaticData.AvailableGameStates.Paused);
		} else if (gameState == StaticData.AvailableGameStates.Paused) {
			ChangeGameState (StaticData.AvailableGameStates.Playing);
		}
	}

	//Properties
	public StaticData.AvailableGameStates GameState {
		get {
			return gameState;
		}
		set {
			gameState = value;
		}
	}
}