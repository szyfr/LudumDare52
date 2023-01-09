using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	
	public Vector3 target;
	public PlayerManager manager;

    void Awake()
     {
     	manager = GameObject.FindGameObjectWithTag("manager").GetComponent<PlayerManager>();
      }
    void FixedUpdate() {
		transform.position = Vector2.MoveTowards(transform.position, target, 4f * Time.deltaTime);

		if (transform.position == target) Destroy(gameObject);
	}

	public void SetTarget(Vector3 target) => this.target = target;

	public void OnTriggerEnter2D(Collider2D col)
	{
		if(col.tag == "Player")
		{
			manager.Damaged();
			Destroy(gameObject);
		}
	}
}
