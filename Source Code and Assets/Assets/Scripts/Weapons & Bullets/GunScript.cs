using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour {


	[SerializeField] GameObject bulletPrefab;
	[SerializeField] GameObject flashObj;
	[SerializeField] LineRenderer laser;
	[SerializeField] Transform bulletSpawn;
	[SerializeField] Transform laserSpawn;
	[SerializeField] float bulletForce;

	float fireRate;
	bool canFire;

	AudioSource aud;
	// Use this for initialization

	void Start () {

		if(this.gameObject.tag == "FirstRifle")
		{
			fireRate = 0.1f;
		}
		else if(this.gameObject.tag == "HeavyGun")
		{
			fireRate = 0.4f;
		}
		else if(this.gameObject.tag == "RayGun")
		{
			fireRate = 0.8f;
		}
		canFire = true;
		laser.enabled = false;
		flashObj.SetActive (false);

		aud = this.GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (Input.GetMouseButton(0) && canFire)
		{
			canFire = false;

			flashObj.SetActive (true);

			GameObject bulletInst = Instantiate (bulletPrefab, bulletSpawn.position, bulletSpawn.rotation) as GameObject;

			Vector3 fwd = bulletSpawn.TransformDirection (Vector3.forward);

			bulletInst.GetComponent<Rigidbody> ().AddForce (fwd * bulletForce * Time.deltaTime);

			aud.Play ();

			Destroy (bulletInst, 5);

			Invoke ("allowFire", fireRate);
			Invoke ("muzzleAnimation", 0.1f);


		}

		if (Input.GetMouseButton(1))
		{
			laser.enabled = true;

			laser.SetPosition (0, laserSpawn.position);

			laser.SetPosition (1, laserSpawn.position + laserSpawn.forward * 50);

			RaycastHit hit;

			Vector3 fwd = laserSpawn.TransformDirection (Vector3.forward);

			//Debug.DrawRay (laserSpawn.position, fwd * 20, Color.green);

			if (Physics.Raycast(laserSpawn.position, fwd, out hit)) 
			{
				laser.SetPosition (1, hit.point);

			}

		}

		if (Input.GetMouseButtonUp (1)) 
		{
			laser.enabled = false;
		}

	}

	void allowFire()
	{
		//flashObj.SetActive (false);
		canFire = true;
	}

	void muzzleAnimation()
	{
		flashObj.SetActive (false);
	}

	public void disableScript()
	{
		this.gameObject.GetComponent<GunScript> ().enabled = false;

	}
}
