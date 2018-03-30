using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeeleDamage : MonoBehaviour {

	[SerializeField] MeeleMovement meeleScriptReference;
	[SerializeField] GameObject   playerObj;
	[SerializeField] Transform    sensorSpawner;
	private float damage = 15.0f;
	private bool applyDamage = true;
	RaycastHit hit;

	void Start()
	{
		playerObj = GameObject.FindGameObjectWithTag("Player");
	}

	void Update()
	{
		//Debug.DrawRay(sensorSpawner.position, sensorSpawner.forward * 7.0f, Color.red);
		if(Physics.Raycast(sensorSpawner.position, sensorSpawner.forward, out hit, 9.0f))
		{			
			if(hit.collider.tag == "playerControl")
			{
				if(Vector3.Distance(playerObj.transform.position, transform.position) <= 6f && meeleScriptReference.getIsAttacking() && applyDamage)
				{
					applyDamage = false;
					GameManager.managerSharedInstance.updatePlayerHealth(damage);
					Invoke("damageCoolDown", 2.0f);
				}
			}
		}
	}

	void damageCoolDown()
	{
		applyDamage = true;
	}
}
