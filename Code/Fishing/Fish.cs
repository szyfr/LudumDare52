using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor.Animations;

public enum FishType {
	empty,
	dreepy,
	hunting,
	smiling,
	bob
}

public struct ReelBarStats {
	public float minVal, maxVal;
	public float topPos, botPos;
	public int   damage;

	public ReelBarStats(
		float min, float max,
		float top, float bot,
		int dmg
	) {
		minVal = min;
		maxVal = max;
		topPos = top;
		botPos = bot;
		damage = dmg;
	}
}

public class Fish : MonoBehaviour {

	public FishType type = FishType.empty;

	public List<RuntimeAnimatorController> anims;

	public GameObject sight;
	public Transform bobber;

	public ReelBarStats reelBar;

	public bool hooked    = false;
	public bool seesHook  = false;
	public bool flipped   = true;
	public bool flee      = false;
	private int fleeTimer = 0;

	public Vector3 lastLocation = Vector3.zero;

	private float fishSpeed = 0.025f;
	private int   moveDelay = 0;
	private int   moveDelayVal = 50;

    void Start() {
		sight = transform.GetChild(0).gameObject;
	}
    void FixedUpdate() {
		moveDelay++;
		if (flee) {
			fleeTimer++;
			if (fleeTimer > 100) {
				// Run
			} else flee = false;
		}

		if (flipped && sight.transform.localPosition.x > 0) {
			gameObject.GetComponent<SpriteRenderer>().flipX = false;
			sight.transform.localPosition = new Vector3( -4,0,0 );
		}
		if (!flipped && sight.transform.localPosition.x < 0) {
			gameObject.GetComponent<SpriteRenderer>().flipX = true;
			sight.transform.localPosition = new Vector3(  4,0,0 );
		}

		if (!hooked) {
			if ((lastLocation != Vector3.zero || !Vector3Approx(lastLocation,transform.localPosition)) && moveDelay > moveDelayVal) {
				float speedMod = fishSpeed;
				if (!seesHook) speedMod = speedMod/2;

				transform.localPosition = new Vector3(
					Mathf.Lerp(transform.localPosition.x, lastLocation.x, fishSpeed),
					Mathf.Lerp(transform.localPosition.y, lastLocation.y, fishSpeed),
					0f
				);
			}
			if (lastLocation == Vector3.zero || Vector3Approx(lastLocation,transform.localPosition)) {
				moveDelay = 0;
				float x = 0;

				float randomThing = Random.Range(0,10);
				if (randomThing > 5) flipped = !flipped;

				if (!flipped) {
					x = Random.Range(transform.position.x+2, transform.position.x+8);
				} else {
					x = Random.Range(transform.position.x-2, transform.position.x-8);
				}
				lastLocation = new Vector3(
					x,
					Random.Range(transform.position.y-3, transform.position.y+3),
					0
				);
			}
		} else {
			if (bobber != null) {
				transform.localPosition = bobber.localPosition;
			}
		}
	}

	void OnCollisionEnter2D(Collision2D coll) {
		PlayerControllerFishing player = coll.transform.parent.gameObject.GetComponent<PlayerControllerFishing>();
		if (seesHook && player.gameObject.tag == "Player" && player.fish == null) {
			hooked = true;
			gameObject.GetComponent<BoxCollider2D>().enabled = false;
			bobber = coll.transform;

			coll.transform.parent.GetComponent<PlayerControllerFishing>().Hooked(this);
		}
	}

	public void SetNewTarget(Vector3 v) {
		//moveDelay = 0;
		lastLocation = v;
	}

	public void SetType(FishType type) {
		this.type = type;

		//TODO:
		switch (type) {
			case FishType.dreepy:
				gameObject.GetComponent<Animator>().runtimeAnimatorController = anims[0];

				reelBar = new ReelBarStats(
					 0.19f,  0.88f,
					29.00f, 44.00f,
					500
				);
				break;
			
			case FishType.hunting:
				gameObject.GetComponent<Animator>().runtimeAnimatorController = anims[1];

				reelBar = new ReelBarStats(
					 0.19f,  0.88f,
					29.00f, 44.00f,
					400
				);
				break;

			case FishType.smiling:
				gameObject.GetComponent<Animator>().runtimeAnimatorController = anims[2];

				reelBar = new ReelBarStats(
					 0.19f,  0.88f,
					29.00f, 44.00f,
					300
				);
				break;
			
			case FishType.bob:
				gameObject.GetComponent<Animator>().runtimeAnimatorController = anims[3];

				reelBar = new ReelBarStats(
					 0.19f,  0.88f,
					29.00f, 44.00f,
					200
				);
				break;
		}
	}

	private bool Vector3Approx(Vector3 v1, Vector3 v2) {
		bool result = true;

		if (v1.x.ToString("0.###") != v2.x.ToString("0.###")) result = false;
		if (v1.y.ToString("0.###") != v2.y.ToString("0.###")) result = false;
		if (v1.z.ToString("0.###") != v2.z.ToString("0.###")) result = false;

		return result;
	}
}
