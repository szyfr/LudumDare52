using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobberCollider : MonoBehaviour {
	public int counter = 0;

	void Update() {
		if (counter >= 50) {
			PlayerControllerFishing cont = transform.parent.gameObject.GetComponent<PlayerControllerFishing>();
			if (cont.state == PlayerFishingState.cast_animating) cont.state = PlayerFishingState.floating;
			
			Vector2 velo = gameObject.GetComponent<Rigidbody2D>().velocity;
			gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(
				Mathf.Lerp(velo.x, 0, 0.005f),
				velo.y
			);
		}
	}

	void OnTriggerStay2D(Collider2D coll) {
		if (coll.gameObject.GetComponent<BuoyancyEffector2D>() != null) {
			counter++;
		}
	}
}
