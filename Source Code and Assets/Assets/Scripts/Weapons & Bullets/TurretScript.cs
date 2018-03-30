using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : MonoBehaviour {

	[SerializeField] Transform turretBase;
	[SerializeField] Transform leftSensor;
	[SerializeField] Transform rightSensor;
	[SerializeField] Transform bulletSpawnerLeftOne;
	[SerializeField] Transform bulletSpawnerLeftTwo;
	[SerializeField] Transform bulletSpawnerRightOne;
	[SerializeField] Transform bulletSpawnerRightTwo;
	[SerializeField] GameObject bulletPrefab;
	[SerializeField] float  bulletForce = 400000;

	private Transform enemyToFollow;
	private bool allowToShoot = true;
	private bool isFollowingEnemy = false;
	private float rotationValue = 5.0f;
	private float switchRotation = 0.0f;
	private float rotationSpeed = 2.0f;

	private Quaternion originalBaseRotation;
	private Quaternion originalTransformRotation;

	RaycastHit hit;
	AudioSource aud;

	// Use this for initialization
	void Start () {

		InvokeRepeating("rotateBase", 0.0f, 0.1f);
		originalBaseRotation = turretBase.rotation;
		originalTransformRotation = transform.rotation;
		aud = this.GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {

		Debug.DrawRay(leftSensor.position, leftSensor.forward * 50.0f, Color.blue);
		Debug.DrawRay(rightSensor.position, rightSensor.forward * 50.0f, Color.blue);

		if(Physics.Raycast (leftSensor.position, leftSensor.forward , out hit, 50.0f))
		{
			if(hit.collider.tag == "Enemy" && !isFollowingEnemy)
			{
				Debug.Log("Saw enemy");
				isFollowingEnemy = true;
				enemyToFollow = hit.transform;
			}

		}
		if(Physics.Raycast (rightSensor.position, rightSensor.forward , out hit, 50.0f))
		{
			if(hit.collider.tag == "Enemy" && !isFollowingEnemy)
			{
				Debug.Log("Saw enemy");
				isFollowingEnemy = true;
				enemyToFollow = hit.transform;
			}
		}

		if(isFollowingEnemy)
		{
			if(enemyToFollow == null)
			{
				isFollowingEnemy  = false;
				turretBase.rotation = originalBaseRotation;
				transform.rotation =  originalTransformRotation;
				switchRotation = 0.0f;
				return;
			}
			Vector3 destVect = enemyToFollow.position - turretBase.position;

			Quaternion destRot = Quaternion.LookRotation (destVect);

			turretBase.rotation = Quaternion.RotateTowards(turretBase.rotation, destRot, rotationSpeed * Time.deltaTime );

			turretBase.eulerAngles = new Vector3 (0.0f, turretBase.eulerAngles.y , 0.0f);

			transform.rotation = Quaternion.Slerp(transform.rotation, destRot, rotationSpeed * Time.deltaTime);

			transform.eulerAngles = new Vector3 (transform.eulerAngles.x + 0.34f, transform.eulerAngles.y, 0.0f);

			if(allowToShoot)
			{
				GameObject bulletInst = Instantiate (bulletPrefab, bulletSpawnerLeftOne.position, bulletSpawnerLeftOne.rotation) as GameObject;
				GameObject bulletInstTwo = Instantiate (bulletPrefab, bulletSpawnerLeftTwo.position, bulletSpawnerLeftTwo.rotation) as GameObject;

				GameObject bulletInstRightOne = Instantiate (bulletPrefab, bulletSpawnerRightOne.position, bulletSpawnerRightOne.rotation) as GameObject;
				GameObject bulletInstRigthTwo = Instantiate (bulletPrefab, bulletSpawnerRightTwo.position, bulletSpawnerRightTwo.rotation) as GameObject;


				Vector3 fwd = bulletSpawnerLeftOne.TransformDirection (Vector3.forward);

				bulletInst.GetComponent<Rigidbody> ().AddForce (fwd * bulletForce * Time.deltaTime);
				bulletInstTwo.GetComponent<Rigidbody> ().AddForce (fwd * bulletForce * Time.deltaTime);
				bulletInstRightOne.GetComponent<Rigidbody> ().AddForce (fwd * bulletForce * Time.deltaTime);
				bulletInstRigthTwo.GetComponent<Rigidbody> ().AddForce (fwd * bulletForce * Time.deltaTime);


				aud.Play ();

				Destroy (bulletInst, 5.0f);
				Destroy (bulletInstTwo, 5.0f);
				Destroy (bulletInstRightOne, 5.0f);
				Destroy (bulletInstRigthTwo, 5.0f);

				allowToShoot = false;

				Invoke ("allowFire", 1.5f);
			}

		}
	}

	void rotateBase()
	{
		if(!isFollowingEnemy)
		{
			turretBase.Rotate(0.0f,rotationValue,0.0f);
			if(rotationValue > 0)
			{
				switchRotation += 5.0f;
			}
			else
			{
				switchRotation -= 5.0f;
			}
			if(switchRotation >= 45.0f || switchRotation <= -41.47f)
			{
				rotationValue *= -1;
			}
		}
	}

	void allowFire()
	{
		allowToShoot = true;
	}
}
