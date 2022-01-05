using System.Collections.Generic;

public class ControllerMap {

	public string name;
	public string horizontal;
	public string vertical;
	public string fireLeft;
	public string fireRight;
	public string buttonA;
	public string buttonB;

	public ControllerMap(string name) {
		this.name = name;
		this.horizontal = name + "Horizontal";
		this.vertical = name + "Vertical";
		this.fireLeft = name + "LeftFire";
		this.fireRight = name + "RightFire";
		this.buttonA = name + "A";
		this.buttonB = name + "B";
	}
}

public static class ControllersList {
	public static List<ControllerMap> listOfContollerMaps = new List<ControllerMap>  {
		new ControllerMap("KL"),
		new ControllerMap("KR"),
		new ControllerMap("J1"),
		new ControllerMap("J2"),
		new ControllerMap("J3"),
		new ControllerMap("J4"),
		new ControllerMap("J5"),
		new ControllerMap("J6"),
		new ControllerMap("J7"),
		new ControllerMap("J8"),
		new ControllerMap("J9"),
		new ControllerMap("J10"),
		new ControllerMap("J11")
	};
}