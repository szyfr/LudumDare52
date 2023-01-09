using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerStateSurvival {
	idle,
	moving,
	jumping,
	attacking,
	stopped
}

public class PlayerControllerSurvival : MonoBehaviour {

	public PlayerManager manager;
	public Text timerText;
	public Rigidbody2D rb;
	public Animator anim;
	public SpriteRenderer sr;


	public PlayerStateSurvival state = PlayerStateSurvival.idle;


	public int health = 10, healthMax = 10;
	private float timeLeft = 3 * 60 * 60;

	public int attackTimer = 0;
	public int attackDuration = 25;
	public int attackCooldown = 50;
	public bool attackDirection = false;

	public bool isGrounded = false;
	

    void Start() {
		manager   = GameObject.Find("manager").GetComponent<PlayerManager>();
		timerText = GameObject.Find("timer").transform.GetChild(0).gameObject.GetComponent<Text>();
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		sr = GetComponent<SpriteRenderer>();
	}

    void FixedUpdate() {
		//* Timer
		timeLeft--;
		float time = timeLeft / 60;
		float minutes = Mathf.FloorToInt(time / 60);
		float seconds = Mathf.FloorToInt(time % 60);
		timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
		if (timeLeft <= 0) manager.SwitchToFishing();

		//* Movement
		if (state != PlayerStateSurvival.stopped) {
			float moveX = Input.GetAxis("Horizontal") / 8;

			if(moveX > 0)
			{
				sr.flipX = false;
				}else if(moveX < 0)
				{
					sr.flipX = true;
				}
			if (moveX != 0) {
				state = PlayerStateSurvival.moving;
				rb.AddForce(new Vector3(moveX * 2,0,0), ForceMode2D.Impulse);
			}
			else state = PlayerStateSurvival.idle;

			if (Input.GetAxis("Jump") > 0 && isGrounded) {
				isGrounded = false;
				rb.AddForce(new Vector3(0,8,0), ForceMode2D.Impulse);
			}
			if (!isGrounded) state = PlayerStateSurvival.jumping;

			if (moveX == 0 && isGrounded) {
				rb.velocity = new Vector3(
					0,
					rb.velocity.y,
					0
				);
			} else {
				rb.velocity = new Vector3(
					Mathf.Clamp(rb.velocity.x,-10,10),
					rb.velocity.y,
					0
				);
			}
		}

		//* Attacking
		attackTimer++;
		if (Input.GetAxis("Fire1") > 0 && attackTimer > attackCooldown) {
			attackTimer = 0;
		}
		if (attackTimer <= attackDuration) state = PlayerStateSurvival.attacking;
		if (Input.mousePosition.x < Screen.width / 2) attackDirection = false;
		if (Input.mousePosition.x > Screen.width / 2) attackDirection = true;

		//* Anims
		switch (state) {
			case PlayerStateSurvival.stopped:
				goto case PlayerStateSurvival.idle;
			case PlayerStateSurvival.idle:
				anim.SetBool("isJumping", false);
				anim.SetBool("isFalling", false);
				anim.SetBool("isAttacking", false);
				anim.SetFloat("movement", 0);
				break;
			case PlayerStateSurvival.moving:
				anim.SetFloat("movement", 1);
				anim.SetBool("isAttacking", false);
				anim.SetBool("isFalling", false);
				break;
			case PlayerStateSurvival.jumping:
				if (rb.velocity.y > 0) {
				anim.SetBool("isJumping", true);
				anim.SetBool("isAttacking", false);
				anim.SetBool("isFalling", false);
				}
				if (rb.velocity.y < 0) {
					anim.SetBool("isJumping", false);
					anim.SetBool("isFalling", true);
					anim.SetBool("isAttacking", false);
				}
				break;
			case PlayerStateSurvival.attacking:
				if (isGrounded) {
					anim.SetBool("isAttacking", true);
					anim.SetBool("isFalling", false);
				} else {
					anim.SetBool("isAttacking", true);
					anim.SetBool("isFalling", true);
				}
				break;
		}
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "ground") {
			isGrounded = true;
		}
	}
	void OnCollisionExit2D(Collision2D coll) {
		if (coll.gameObject.tag == "ground") {
			isGrounded = false;
		}
	}
}
