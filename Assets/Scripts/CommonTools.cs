using UnityEngine;

public static class CommonTools {

	public static float VectorToAngle(Vector2 v) {
		if(v.x < 0) {
			return 360 - (Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg * -1);
		} else {
			return Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg;
		}
	}
}
