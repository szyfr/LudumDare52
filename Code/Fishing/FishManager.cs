using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct Area {
	public Area(Vector3 mi, Vector3 ma) {
		min = mi;
		max = ma;
	}

	public Vector3 min { get; }
	public Vector3 max { get; }
}

public class FishManager : MonoBehaviour {
    
	public GameObject fishPrefab;

	public List<GameObject> dreepyList, huntingList, smilingList, bobList;
	public int dreepySpawnTimer = 0,
		huntingSpawnTimer = 0,
		smilingSpawnTimer = 0,
		bobSpawnTimer     = 0;

	private Area dreepySpawningArea = new Area(new Vector2(0,-25), new Vector2(200,-1)),
				huntingSpawningArea = new Area(new Vector2(0,-30), new Vector2(200,-1)),
				smilingSpawningArea = new Area(new Vector2(0,-30), new Vector2(200,-1)),
				bobSpawningArea     = new Area(new Vector2(0,-30), new Vector2(200,-1));

    void Start() {
		
	}

    void FixedUpdate() {
		//* Update timers
		dreepySpawnTimer++;
		huntingSpawnTimer++;
		smilingSpawnTimer++;
		bobSpawnTimer++;

		//* Spawning
		if (dreepyList.Count < 30 && dreepySpawnTimer > 100) {
			Vector3 position = GetRandomPosition(ref dreepySpawningArea);

			GameObject newFish = Instantiate(fishPrefab, position, new Quaternion());
			newFish.GetComponent<Fish>().SetType(FishType.dreepy);
			dreepyList.Add(newFish);

			dreepySpawnTimer = 0;
		}
		if (huntingList.Count < 25 && huntingSpawnTimer > 200) {
			Vector3 position = GetRandomPosition(ref huntingSpawningArea);

			GameObject newFish = Instantiate(fishPrefab, position, new Quaternion());
			newFish.GetComponent<Fish>().SetType(FishType.hunting);
			huntingList.Add(newFish);

			huntingSpawnTimer = 0;
		}
		if (smilingList.Count < 20 && smilingSpawnTimer > 300) {
			Vector3 position = GetRandomPosition(ref smilingSpawningArea);

			GameObject newFish = Instantiate(fishPrefab, position, new Quaternion());
			newFish.GetComponent<Fish>().SetType(FishType.smiling);
			smilingList.Add(newFish);

			smilingSpawnTimer = 0;
		}
		if (bobList.Count < 10 && bobSpawnTimer > 400) {
			Vector3 position = GetRandomPosition(ref bobSpawningArea);

			GameObject newFish = Instantiate(fishPrefab, position, new Quaternion());
			newFish.GetComponent<Fish>().SetType(FishType.bob);
			bobList.Add(newFish);

			bobSpawnTimer = 0;
		}
	}

	private Vector3 GetRandomPosition(ref Area spawn) {
		Vector3 result = Vector3.zero;

		result.x = Random.Range(spawn.min.x, spawn.max.x);
		result.y = Random.Range(spawn.min.y, spawn.max.y);
		result.z = Random.Range(spawn.min.z, spawn.max.z);

		return result;
	}
}
