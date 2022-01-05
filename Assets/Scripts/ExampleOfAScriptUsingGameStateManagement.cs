using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleOfAScriptUsingGameStateManagement : MonoBehaviour {

	public GameObject scriptsBucket;
	private StaticData.AvailableGameStates gameState;

	// Use this for initialization
	void Start () {
		scriptsBucket.GetComponent<GameStateManager> ().MenuGameState.AddListener(OnMenu);
		scriptsBucket.GetComponent<GameStateManager> ().StartingGameState.AddListener(OnStarting);
		scriptsBucket.GetComponent<GameStateManager> ().PlayingGameState.AddListener(OnPlaying);
		scriptsBucket.GetComponent<GameStateManager> ().PausedGameState.AddListener(OnPausing);
		SetCanvasState (scriptsBucket.GetComponent<GameStateManager> ().GameState);
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (gameState == StaticData.AvailableGameStates.Playing) {
			//Do stuff
		}
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

	public void SetCanvasState(StaticData.AvailableGameStates state) {
		gameState = state;
	}
}
