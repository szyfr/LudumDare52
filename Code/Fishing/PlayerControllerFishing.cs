using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerFishingState {
	none,
	casting,
	cast_animating,
	floating,
	reeling,
	returning,
}

public class PlayerControllerFishing : MonoBehaviour {

	public PlayerManager manager;
	public GameObject mainCamera;
	public GameObject bobber;
	public GameObject distanceIndicator;
	public GameObject timeIndicator;
	public Slider castingBar;
	public Slider reelingBar;
	public Fish   fish;

	public bool canFish           = true;
	public bool sentBobber        = false;
	public bool bobberMoving      = false;
	public bool reverseCastingBar = false;
	public bool fishOnLine        = false;

	public PlayerFishingState state = PlayerFishingState.none;
	public int castingTimer = 50;
	public int fishingErrorTime = 0;
	private int timeLeft = 5 * 60 * 60;

	private Vector2 forceCastStrengthBase = new Vector2(10f,15f);
	private Vector2 forcePullStrengthBase = new Vector2(-0.75f,-0.25f);

	private float castingModifierBase = 0.5f;

	public Animator anim;

    void Start() {
		manager    = GameObject.Find("manager").GetComponent<PlayerManager>();
		mainCamera = transform.GetChild(0).gameObject;
		bobber     = transform.GetChild(1).gameObject;
		castingBar = GameObject.Find("casting_bar").GetComponent<Slider>();
		reelingBar = GameObject.Find("reeling_bar").GetComponent<Slider>();
		distanceIndicator = GameObject.Find("distance_indicator");
		timeIndicator = GameObject.Find("timer");
		anim = this.gameObject.GetComponent<Animator>();
    }

    void FixedUpdate() {
		castingTimer++;

		switch (state) {
			case PlayerFishingState.none:
				//* Turn off bars
				if (castingBar.gameObject.activeSelf != false) castingBar.gameObject.SetActive(false);
				if (reelingBar.gameObject.activeSelf != false) reelingBar.gameObject.SetActive(false);

				//* Controls
				if (Input.GetAxis("Fire1")  > 0 && castingTimer > 50) state = PlayerFishingState.casting;
				break;

			case PlayerFishingState.casting:
				//* Turn on casting bar and off reeling bar
				if (castingBar.gameObject.activeSelf == false) castingBar.gameObject.SetActive(true);
				if (reelingBar.gameObject.activeSelf != false) reelingBar.gameObject.SetActive(false);

				if (Input.GetAxis("Fire1")  > 0 && castingTimer > 50) {
					//* Play holding anim
					anim.SetBool("isHold", true);

					//* Calculating casting bar value
					float castingMod = 0.005f + (castingBar.value / 10);
					if (reverseCastingBar) castingMod = -castingMod;
					castingBar.value = Mathf.Clamp(
						castingBar.value += castingMod,
						0f,1f
					);
					if (castingBar.value == 1 || castingBar.value == 0) reverseCastingBar = !reverseCastingBar;
				} else {
					//* Cast line
					state = PlayerFishingState.cast_animating;
					if (!sentBobber) {
						//* Apply force to bobber
						bobber.GetComponent<Rigidbody2D>().simulated = true;
						bobber.GetComponent<Rigidbody2D>().AddForce(
							forceCastStrengthBase * (castingBar.value + castingModifierBase),
							ForceMode2D.Impulse
						);
						sentBobber = true;
					}
				}
				break;
			
			case PlayerFishingState.cast_animating:
				//* Turn off bars
				if (castingBar.gameObject.activeSelf != false) castingBar.gameObject.SetActive(false);
				if (reelingBar.gameObject.activeSelf != false) reelingBar.gameObject.SetActive(false);

				//* Play casting anim
				if (Input.GetAxis("Fire1") == 0 && castingTimer > 50) {
					anim.SetBool("isHold", false);
					anim.SetBool("isRelease", true);
				}
				break;

			case PlayerFishingState.floating:
				//* Turn off bars
				if (castingBar.gameObject.activeSelf != false) castingBar.gameObject.SetActive(false);
				if (reelingBar.gameObject.activeSelf != false) reelingBar.gameObject.SetActive(false);

				//* Play release anim
				anim.SetBool("isRelease", false);

				//* Reeling in bobber
				if (Input.GetAxis("Fire1")  > 0 && castingTimer > 50) {
					Vector2 direction = DegreeToVector2(GetAngle(transform.localPosition, bobber.transform.localPosition, 0));
					bobber.GetComponent<Rigidbody2D>().AddForce(direction * forcePullStrengthBase,ForceMode2D.Impulse);
					bobberMoving = true;
				}
				//* Letting it float
				if (Input.GetAxis("Fire1") == 0 && castingTimer > 50) bobberMoving = false;

				//* Reset if no fish
				if (GetDistance(transform.localPosition, bobber.transform.localPosition) <= 1.5f && !fishOnLine) ResetPlayer();
				break;

			case PlayerFishingState.reeling:
				//* Turn off casting bar and on reeling bar
				if (castingBar.gameObject.activeSelf != false) castingBar.gameObject.SetActive(false);
				if (reelingBar.gameObject.activeSelf == false) reelingBar.gameObject.SetActive(true);

				//* Calculating casting bar value
				if (Input.GetAxis("Fire1")  > 0) {
					reelingBar.value += 0.01f;
					
					//* Reeling
					Vector2 direction = DegreeToVector2(GetAngle(transform.localPosition, bobber.transform.localPosition, 0));
					bobber.GetComponent<Rigidbody2D>().AddForce((direction * forcePullStrengthBase)/2,ForceMode2D.Impulse);
					bobberMoving = true;
				} else {
					reelingBar.value -= 0.01f;
				}

				//* Calculating fish catching
				if (reelingBar.value < fish.reelBar.minVal || reelingBar.value > fish.reelBar.maxVal) {
					fishingErrorTime++;
				}

				//* Fail fishing
				if (fishingErrorTime > 500) {
					ResetPlayer();
				}

				//* Grab Fish then reset
				if (GetDistance(transform.localPosition, bobber.transform.localPosition) <= 1.5f && fishOnLine) {
					manager.GetFish(fish);
					ResetPlayer();
				}
				break;

			case PlayerFishingState.returning:
				//* Turn off bars
				if (castingBar.gameObject.activeSelf != false) castingBar.gameObject.SetActive(false);
				if (reelingBar.gameObject.activeSelf != false) reelingBar.gameObject.SetActive(false);
				break;
		}

		//* Camera
		switch (state) {
			case PlayerFishingState.none:
				goto case PlayerFishingState.casting;
			case PlayerFishingState.casting:
				mainCamera.transform.localPosition = new Vector3(6,1,-10);
				break;

			case PlayerFishingState.cast_animating:
				goto case PlayerFishingState.reeling;
			case PlayerFishingState.floating:
				goto case PlayerFishingState.reeling;
			case PlayerFishingState.reeling:
				mainCamera.transform.localPosition = new Vector3(
					bobber.transform.localPosition.x + 6,
					bobber.transform.localPosition.y + 1,
					-10
				);
				break;
		}

		//* Distance indicator logic
		if (mainCamera.transform.localPosition.x >= 10) {
			distanceIndicator.SetActive(true);
			float dist  = GetDistance(transform.localPosition, bobber.transform.localPosition);
			float angle = GetAngle(transform.localPosition, bobber.transform.localPosition, 0);
			distanceIndicator.transform.eulerAngles = new Vector3(0,0,angle);
			distanceIndicator.transform.GetChild(0).GetComponent<Text>().text = (int)dist + "m";
		} else {
			distanceIndicator.SetActive(false);
		}

		//* Timer logic
		timeLeft--;
		float time = timeLeft / 60;
		float minutes = Mathf.FloorToInt(time / 60);
		float seconds = Mathf.FloorToInt(time % 60);
		timeIndicator.transform.GetChild(0).gameObject.GetComponent<Text>().text = string.Format("{0:00}:{1:00}", minutes, seconds);
		if (timeLeft <= 0) manager.SwitchToSurvival();
	}

	public void Hooked(Fish fish) {
		this.fish = fish;
		fishOnLine = true;
		state = PlayerFishingState.reeling;

		switch (fish.type) {
			case FishType.dreepy:
				// Change reeling bar
				break;
		}
	}

	private void ResetPlayer() {
		mainCamera.transform.localPosition = new Vector3(6,1,-10);
		sentBobber = false;
		bobberMoving = false;
		state = PlayerFishingState.none;
		bobber.transform.localPosition = new Vector3(0,0,0);
		bobber.GetComponent<BobberCollider>().counter = 0;
		bobber.GetComponent<Rigidbody2D>().simulated = false;
		bobber.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		castingBar.value = 0;
		castingBar.gameObject.SetActive(false);
		castingTimer = 0;
		fishOnLine = false;
		if (fish != null) Destroy(fish.gameObject);
		fish = null;
		fishingErrorTime = 0;
	}

	private float GetDistance( Vector3 p1, Vector3 p2 ) => Mathf.Sqrt(Mathf.Pow(p2.x - p1.x, 2) + Mathf.Pow(p2.y - p1.y, 2));

	private float GetAngle( Vector2 p1, Vector2 p2, float offset ) {
		Vector2 vect = p2 - p1;
		return (((Mathf.Atan2(vect.y, vect.x) + (2 * Mathf.PI)) * 360) / (2*Mathf.PI) + offset);
	}

	public Vector2 RadianToVector2( float radian ) => new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));

	public Vector2 DegreeToVector2( float degree ) => RadianToVector2(degree * Mathf.Deg2Rad);
}
