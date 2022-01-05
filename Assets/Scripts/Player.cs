using UnityEngine;

public class Player {
	public int id;
	public ControllerMap cm;
	public bool alive;

	public Player(int id, ControllerMap cm) {
		this.id = id;
		this.cm = cm;
		this.alive = true;
	}
}
