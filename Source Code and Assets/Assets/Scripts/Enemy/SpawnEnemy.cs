using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour {

	[SerializeField] GameObject EnemyPrefab;
	[SerializeField] float    spawnInterval; 
	[SerializeField] float    initialSpawnInterval;
	// Use this for initialization
	void Start () 
	{
		InvokeRepeating("CreateEnemy",initialSpawnInterval, spawnInterval);	
	}

	void CreateEnemy()
	{
		Instantiate(EnemyPrefab,transform.position,transform.rotation);
	}
}
