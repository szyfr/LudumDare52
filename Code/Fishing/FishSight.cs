using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSight : MonoBehaviour {

	
	
    void Start() { }
    void Update() { }

	void OnTriggerStay2D(Collider2D coll) {
		if (coll.transform.parent.gameObject.GetComponent<PlayerControllerFishing>().fishOnLine) return;
		
		Fish parent = transform.parent.gameObject.GetComponent<Fish>();
		parent.SetNewTarget(coll.transform.localPosition);
		parent.seesHook = true;

		if (coll.transform.parent.GetComponent<PlayerControllerFishing>().bobberMoving) {
			switch (parent.type) {
				case FishType.bob:
					parent.flee = true;
					break;
			}
		}
	}
}
