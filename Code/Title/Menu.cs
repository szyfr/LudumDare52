using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

	public Text stats;

    void Start() {
		stats = GameObject.Find("stats").GetComponent<Text>();
		stats.text = string.Format("Fishing: {0:00} Survival: {1:00}", PlayerPrefs.GetInt("survivalRuns"), PlayerPrefs.GetInt("fishingRuns"));
	}
    void Update() { }

	public void Play() {
		SceneManager.LoadScene("Fishing");
	}
	public void Quit() {
		Application.Quit();
	}
}
