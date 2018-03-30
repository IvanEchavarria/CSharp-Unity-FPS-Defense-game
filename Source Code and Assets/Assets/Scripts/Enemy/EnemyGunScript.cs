using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGunScript : MonoBehaviour {

	[SerializeField] GameObject bulletPrefab;
	[SerializeField] GameObject flashObj;
	[SerializeField] GameObject modelMesh;
	[SerializeField] LineRenderer laser;
	[SerializeField] Transform bulletSpawn;
	[SerializeField] Transform laserSpawn;
	[SerializeField] float bulletForce;
	[SerializeField] EnemyMovement movementScript;

	Animator anim;

	RaycastHit hit;
	bool canFire;
	bool seesPlayer;
	bool isEnemyDead;
	int  rewardValue = 15;

	AudioSource aud;
	// Use this for initialization
	void Start () {

		isEnemyDead = false;
		canFire = true;
		seesPlayer = false;
		laser.enabled = false;
		flashObj.SetActive (false);
		aud = this.GetComponent<AudioSource> ();
		anim = modelMesh.GetComponent<Animator>();

	}

	// Update is called once per frame
	void Update () {

		if(movementScript.getHealth() <= 0.0f && !isEnemyDead)
		{
			isEnemyDead = true;
			seesPlayer = false;
			canFire = false;
			anim.SetTrigger("isDead");
			GameManager.managerSharedInstance.updateMoney(rewardValue);
			Destroy(gameObject, 0.5f);
		}

			 
		//Debug.DrawRay(bulletSpawn.position, bulletSpawn.forward * 50.0f, Color.red);
		if(Physics.Raycast (bulletSpawn.position, bulletSpawn.forward, out hit, 100.0f) && !isEnemyDead)
		{
			if (hit.collider.tag == "Player")
			{
				seesPlayer = true;
				anim.SetBool("isShooting", seesPlayer);
			}
			else
			{
				seesPlayer = false;
				anim.SetBool("isShooting", seesPlayer);
			}
		}

		if (seesPlayer && canFire && !isEnemyDead)
		{
			

			GameObject bulletInst = Instantiate (bulletPrefab, bulletSpawn.position, bulletSpawn.rotation) as GameObject;

			Vector3 fwd = bulletSpawn.TransformDirection (Vector3.forward);

			bulletInst.GetComponent<Rigidbody> ().AddForce (fwd * bulletForce * Time.deltaTime);

			aud.Play ();

			Destroy (bulletInst, 5);

			canFire = false;

			float randX = Random.Range (0.01f, 0.05f);
			float randY = Random.Range (0.01f, 0.05f);

			flashObj.transform.localScale = new Vector3 (randX, randY, 1f);

			flashObj.SetActive (true);

			Invoke ("allowFire", 0.3f);

		}

	}

	void allowFire()
	{
		flashObj.SetActive (false);
		canFire = true;
	}

	public bool isDeadStatus()
	{
		return isEnemyDead;
	}
}
