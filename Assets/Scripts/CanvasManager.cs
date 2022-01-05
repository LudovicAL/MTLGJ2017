using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour {


	public GameObject panelMenu;
	public GameObject panelStarting;
	public GameObject panelGame;
	public GameObject panelPaused;
	private GameObject scriptsBucket;
	private StaticData.AvailableGameStates gameState;
	public Text textWindSpeed;
	public Text textCountDown;
	public float totalCountDownDuration = 2.1f;
	public Image compass;
	public GameObject[] playerPanels;
	public GameObject[] boats;
	private WorldPresets m_WorldPreset;

	void Start () {
		scriptsBucket = GameObject.Find("ScriptsBucket");
		scriptsBucket.GetComponent<GameStateManager> ().MenuGameState.AddListener(OnMenu);
		scriptsBucket.GetComponent<GameStateManager> ().StartingGameState.AddListener(OnStarting);
		scriptsBucket.GetComponent<GameStateManager> ().PlayingGameState.AddListener(OnPlaying);
		scriptsBucket.GetComponent<GameStateManager> ().PausedGameState.AddListener(OnPausing);
		SetCanvasState (scriptsBucket.GetComponent<GameStateManager> ().GameState);

		GameObject worldPresetObj = GameObject.Find("WorldPresets");
		if (worldPresetObj != null)
			m_WorldPreset = worldPresetObj.GetComponent<WorldPresets>();
		UpdateWorldPresetBox();

		for (int i = 1; i < 5; i++) {
			UpdateReadyJoin(playerPanels[i-1], i);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (gameState == StaticData.AvailableGameStates.Playing) {
			textWindSpeed.text = "Wind Speed: " + StaticData.windSpeed.ToString("0.#");
			compass.rectTransform.rotation = Quaternion.LookRotation (Vector3.forward, StaticData.windDirection);
		} else if (gameState == StaticData.AvailableGameStates.Menu) {
			foreach (ControllerMap cm in ControllersList.listOfContollerMaps) {
				if (Input.GetButton(cm.buttonB)) {
					AddPlayer (cm);
					break;
				}
			}
			foreach (Player p in StaticData.playerList) {
				if (Input.GetAxisRaw(p.cm.fireLeft) != 0) {
					RemovePlayer (p);
					break;
				}
			}
			if (Input.GetButtonDown("XboxY"))
			{
				if (m_WorldPreset != null)
					m_WorldPreset.ChangePreset();
				UpdateWorldPresetBox();
			}
		}
	}

	void UpdateWorldPresetBox()
	{
		WaveDataSet preset = m_WorldPreset ? m_WorldPreset.GetCurrentPreset() : null;
		Text presetName = GameObject.Find("WorldPresetBox").GetComponentInChildren<Text>();
		presetName.text = preset.m_Name;
	}

	//Adds a player to the game and registers his ControllerMap to his ID
	public void AddPlayer(ControllerMap cm) {
		if (StaticData.playerList.Count < 4) {
			int id = 0;
			for (int i = 1; i < 5; i++) {
				if (!IsPlayerReady(i)) {
					id = i;
					break;
				}
			}
			if (id == 0) {
				Debug.Log ("Error: AddPlayer");
				return;
			}
			//playerPanels [id - 1].SetActive (true);
			Player player = new Player (id, cm);
			StaticData.playerList.Add (player);
			boats [id - 1].GetComponentInChildren<BoatControler> ().player = player;
			ControllersList.listOfContollerMaps.Remove (cm);

			UpdateReadyJoin(playerPanels[id-1], id);
			// deactivate "join"
			Transform join = playerPanels[id-1].gameObject.transform.Find("Join");
			if (join != null)
				join.gameObject.SetActive(false);
			// activate "ready"
			Transform ready = playerPanels[id-1].gameObject.transform.Find("Ready");
			if (ready != null)
				ready.gameObject.SetActive(true);
			
			//playerPanels [id - 1].SetActive (true);
			Text[] tTempo = playerPanels [id - 1].GetComponentsInChildren<Text> ();
			foreach (Text t in tTempo) {
				if (t.gameObject.tag == "ID") {
					t.text = cm.name;
					break;
				}
			}
			if (StaticData.playerList.Count >= 4) {
				playerPanels [4].SetActive(false);
			}
		}
	}
		
	//Removes a player from the game
	public void RemovePlayer(Player p) {
		boats [p.id - 1].GetComponentInChildren<BoatControler> ().player = null;
		//playerPanels [p.id - 1].SetActive (false);
		ControllersList.listOfContollerMaps.Add (p.cm);
		StaticData.playerList.Remove (p);
		//playerPanels [4].SetActive(true);
		UpdateReadyJoin(playerPanels[p.id-1], p.id); 
	}

	void UpdateReadyJoin(GameObject _panel, int _playerID) {
		bool isReady = IsPlayerReady(_playerID);

		// deactivate "join"
		Transform join = _panel.gameObject.transform.Find("Join");
		if (join != null)
			join.gameObject.SetActive(!isReady);
		// activate "ready"
		Transform ready = _panel.gameObject.transform.Find("Ready");
		if (ready != null)
			ready.gameObject.SetActive(isReady);
	}

	bool IsPlayerReady(int _playerID)
	{
		bool isReady = (boats [_playerID - 1].GetComponentInChildren<BoatControler> ().player != null);
		return isReady;
	}

	//Displays the appropriate UI panel considering the current gameState
	public void ShowAppropriatePanel() {
		panelMenu.SetActive (false);
		panelStarting.SetActive (false);
		panelGame.SetActive (false);
		panelPaused.SetActive (false);
		Button[] buttons;
		switch (gameState) {
			case StaticData.AvailableGameStates.Menu:
				panelMenu.SetActive (true);
				buttons = panelMenu.GetComponentsInChildren<Button> ();
				if (buttons.Length > 0) {
					buttons [0].Select();
				}
				break;
			case StaticData.AvailableGameStates.Starting:
				panelStarting.SetActive (true);
				foreach(GameObject go in boats) {
					go.GetComponentInChildren<BoatControler> ().Reset ();
				}
				StartCoroutine (ShowCountDown ());
				break;
			case StaticData.AvailableGameStates.Playing:
				panelGame.SetActive (true);
				break;
			case StaticData.AvailableGameStates.Paused:
				panelPaused.SetActive (true);
				buttons = panelPaused.GetComponentsInChildren<Button> ();
				if (buttons.Length > 0) {
					buttons [0].Select();
				}
				break;
		}
	}

	IEnumerator ShowCountDown() {
		for (int i = 3; i > 0; i--) {
			textCountDown.text = i.ToString();
			yield return new WaitForSeconds(totalCountDownDuration / 3.0f);
		}
		scriptsBucket.GetComponent<GameStateManager> ().ChangeGameState (StaticData.AvailableGameStates.Playing);
	}

	public void OnMenuButtonClick() {
		scriptsBucket.GetComponent<GameStateManager> ().ChangeGameState (StaticData.AvailableGameStates.Menu);
	}

	public void OnStartButtonClick() {
		scriptsBucket.GetComponent<GameStateManager> ().ChangeGameState (StaticData.AvailableGameStates.Starting);
	}

	public void OnExitButtonClick() {
		Application.Quit();
	}

	protected void OnMenu() {
		SetCanvasState (StaticData.AvailableGameStates.Menu);

	}

	protected void OnStarting() {
		SetCanvasState (StaticData.AvailableGameStates.Starting);
		GameObject insultManager = GameObject.Find("Insults");
		AudioSource audioSource = GetComponent<AudioSource>();
		if (audioSource && insultManager)
			audioSource.PlayOneShot(insultManager.GetComponent<InsultsPicker>().GetUnsusedInsult());
	}

	protected void OnPlaying() {
		SetCanvasState (StaticData.AvailableGameStates.Playing);
	}

	protected void OnPausing() {
		SetCanvasState (StaticData.AvailableGameStates.Paused);
	}

	public void SetCanvasState(StaticData.AvailableGameStates state) {
		gameState = state;
		ShowAppropriatePanel();
		Update ();
	}
}
