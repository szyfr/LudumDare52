using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour {

    public static PlayerManager Instance;

	public GameObject gui;

	//* Upgrades

	//* Resources
	public int spirit = 0, determination = 0, will = 0;
	public int survivalRuns = 0, fishingRuns = 0;

	public int updateTimer = 0;

    void Start() {
		if (Instance == null) {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        } else if (Instance != this) {
            Destroy (gameObject);
        }
	}
    void Update() {
		if (gui == null) {
			gui = GameObject.Find("gui");
			if (gui != null) gui.transform.GetChild(6).gameObject.GetComponent<Button>().onClick.AddListener(Win);
		}
		if (spirit        >= 10 &&
			determination >=  5 &&
			will          >=  2 &&
			gui != null) {
			gui.transform.GetChild(6).gameObject.SetActive(true);
		}
		updateTimer++;
		if (Input.GetKey("backspace")) updateTimer = 999;

		if (gui != null) {
			gui.transform.GetChild(0).gameObject.GetComponent<Text>().text = spirit.ToString();
			gui.transform.GetChild(1).gameObject.GetComponent<Text>().text = determination.ToString();
			gui.transform.GetChild(2).gameObject.GetComponent<Text>().text = will.ToString();

			if (updateTimer < 1000) {
				gui.transform.localPosition = new Vector3(Mathf.Lerp(gui.transform.localPosition.x, 340, 0.05f), 0, 0);
			}
			if (updateTimer > 1000) {
				gui.transform.localPosition = new Vector3(Mathf.Lerp(gui.transform.localPosition.x, 460, 0.05f), 0, 0);
			}
		}
	}

	public void GetFish(Fish fish) {
		updateTimer = 0;
		switch (fish.type) {
			case FishType.dreepy:
				spirit++;
				break;
			case FishType.hunting:
				determination++;
				break;
			case FishType.smiling:
				will++;
				break;
			case FishType.bob:
				spirit++;
				determination++;
				will++;
				break;
		}
	}

	public void SwitchToSurvival() {
		fishingRuns++;
		SceneManager.LoadScene("Survival");
	}
	public void SwitchToFishing() {
		survivalRuns++;
		SceneManager.LoadScene("Fishing");
	}
	public void Win() {
		if (SceneManager.GetActiveScene().name == "Survival") survivalRuns++;
		if (SceneManager.GetActiveScene().name == "Fishing")  fishingRuns++;
		
		PlayerPrefs.SetInt("survivalRuns", survivalRuns);
		PlayerPrefs.SetInt("fishingRuns", fishingRuns);
		PlayerPrefs.Save();

		SceneManager.LoadScene("Title");
	}

	public void Damaged()
	{
		int randItem = Random.Range(0,3);
		if(randItem == 0 && spirit != 0)
		{
			spirit--;
		}if(randItem == 1 && will != 0)
		{
			will--;
		}if(randItem == 2 && determination != 0)
		{
			determination--;
		}
	}
}
