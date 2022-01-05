using System.Collections.Generic;
using UnityEngine;

public static class StaticData {
	public enum AvailableGameStates {
		Menu,	//Player is consulting the menu
		Starting,	//Game is starting
		Paused,	//Game is paused
		Playing	//Game is playing
	};

	public static Vector2 windDirection = Vector2.one;
	public static float windSpeed = 1.0f;
    public static bool justChangedWindValues;
    public static float desiredWindSpeed;
    public static List<Player> playerList = new List<Player>();
}
