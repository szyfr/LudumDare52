using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType {
	melee,
	ranged
}

public class Enemy : MonoBehaviour {

	public GameObject player;
	public Rigidbody2D rb;
	public GameObject bulletPrefab;
	public SpriteRenderer sr;

	public EnemyType type = EnemyType.ranged;

	public int health, healthMax = 2;
	public int attackTimer = 0;

    void Start() {
		health = healthMax;

		player = GameObject.Find("Player");
		rb = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();
	}

    void FixedUpdate() {

		attackTimer++;
		if (type == EnemyType.melee) {
			rb.MovePosition(new Vector3(
				Mathf.MoveTowards(transform.localPosition.x, player.transform.localPosition.x, 0.05f),
				transform.localPosition.y, 0
			));
		} else if (type == EnemyType.ranged) {
			float distance = GetDistance(transform.localPosition, player.transform.localPosition);
			if (distance > 8) {
				rb.MovePosition(new Vector3(
					Mathf.MoveTowards(transform.localPosition.x, player.transform.localPosition.x, 0.05f),
					transform.localPosition.y, 0
				));
			} else if (attackTimer > 100) {
				GameObject go = Instantiate(bulletPrefab, transform.position, new Quaternion());
				go.GetComponent<Bullet>().SetTarget(player.transform.position);
				attackTimer = 0;
			}
		}
	}

	private float GetDistance( Vector3 p1, Vector3 p2 ) => Mathf.Sqrt(Mathf.Pow(p2.x - p1.x, 2) + Mathf.Pow(p2.y - p1.y, 2));


	public void Damaged()
	{
		health --;
		if(health == 0)
		{
			Destroy(gameObject);
		}
	}
}
