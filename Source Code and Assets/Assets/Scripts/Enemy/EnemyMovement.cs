using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour {

	[SerializeField] GameObject playerObj;
	[SerializeField] Transform enemyWeapon;
	[SerializeField] EnemyGunScript gunScript;

	Transform bulletSpanwer;

	float speed = 2.0f;
	float mass = 5.0f;
	float force = 50.0f;
	float minimunDistancetoAvoid = 5.0f;
	float rotationSpeed = 2.0f;
	float health = 100;

	void Awake()
	{
		playerObj = GameObject.FindGameObjectWithTag("Player");
	}

	// Update is called once per frame
	void Update () {

		if(gunScript.isDeadStatus() == false)
	    {
			Vector3 direction = (playerObj.transform.position - transform.position);
			direction.y = 0.0f;
			direction.Normalize();

			AvoidObstacle(ref direction);


			if(Vector3.Distance(playerObj.transform.position, transform.position) < 26.5f)
			{	
				PointAndShoot();	
				return;
			}

			Quaternion  rotation = Quaternion.LookRotation(direction);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 5.0f * Time.deltaTime);
			transform.Translate (Vector3.forward * speed * Time.deltaTime);
		}
		else
		{
			Destroy(gameObject, 3.0f);
		}
	}

	void AvoidObstacle(ref Vector3 direction)
	{
		RaycastHit hit;

		if(Physics.Raycast(transform.position, transform.forward, out hit, minimunDistancetoAvoid))
		{
			if(hit.collider.tag == "obstacle" || hit.collider.tag == "Enemy")
			{				
				Vector3 hitNormal = hit.normal;
				hitNormal.y = 0.0f;
				direction = transform.forward + hitNormal * force;

			}
		}
		if (Physics.Raycast (transform.position, (transform.forward + transform.right), out hit, minimunDistancetoAvoid)) 
		{
			if (hit.collider.tag == "obstacle" || hit.collider.tag == "Enemy") 
			{
				transform.Rotate (0.0f, -5.0f, 0.0f);
			}
		}
		// Left side sensor detection
		 if(Physics.Raycast (transform.position, (transform.forward - transform.right), out hit, minimunDistancetoAvoid))
		{
			if (hit.collider.tag == "obstacle" || hit.collider.tag == "Enemy") 
			{
				transform.Rotate (0.0f, 5.0f, 0.0f);
			}
		}

	}

	void PointAndShoot()
	{	
		bulletSpanwer = enemyWeapon.GetChild(2).transform;

		Vector3 destVect = playerObj.transform.position - bulletSpanwer.position;

		Quaternion destRot = Quaternion.LookRotation (destVect);

		enemyWeapon.rotation = Quaternion.Slerp(transform.rotation, destRot, rotationSpeed);

		enemyWeapon.eulerAngles = new Vector3 (enemyWeapon.eulerAngles.x, enemyWeapon.eulerAngles.y , 0.0f);
	}

	public void UpdateHealth(float dmg)
	{
		health -= dmg;
	}

	public float getHealth()
	{
		return health;
	}
}
