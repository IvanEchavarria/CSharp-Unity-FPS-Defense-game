using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovementScript : MonoBehaviour {

	[SerializeField] GameObject playerObj;
	[SerializeField] GameObject modelMesh;
	[SerializeField] GameObject rockPrefab;
	[SerializeField] Transform  rockSpawn;
	[SerializeField] float      throwForce;

	Animator anim;

	bool  isEnemyDead = false;
	bool  canAttack = true;
	float speed = 4.0f;
	float mass = 5.0f;
	float force = 50.0f;
	float minimunDistancetoAvoid = 5.0f;
	float rotationSpeed = 2.0f;
	float health = 200f;
	int   rewardValue = 125;



	void Awake()
	{
		playerObj = GameObject.FindGameObjectWithTag("Player");
		anim = modelMesh.GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update () {

		if(health <= 0.0f && !isEnemyDead)
		{
			isEnemyDead = true;
			anim.SetTrigger("isDead");
			GameManager.managerSharedInstance.updateMoney(rewardValue);
			Destroy(gameObject, 5.0f);
		}

		if(!isEnemyDead)
		{
			Vector3 direction = (playerObj.transform.position - transform.position);
			direction.y = 0.0f;
			direction.Normalize();

			AvoidObstacle(ref direction);


			if(Vector3.Distance(playerObj.transform.position, transform.position) < 46.5f)
			{	
				if(canAttack)
				{	
					canAttack = false;
					anim.SetTrigger("attack");
					Invoke("throwRock", 2.37f);
				}
				return;
			}

			Quaternion  rotation = Quaternion.LookRotation(direction);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 5.0f * Time.deltaTime);
			transform.Translate (Vector3.forward * speed * Time.deltaTime);
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

	void throwRock()
	{
		GameObject rockInst = Instantiate (rockPrefab, rockSpawn.position, rockSpawn.rotation) as GameObject;

		Vector3 fwd = rockSpawn.TransformDirection (Vector3.forward);

		rockInst.GetComponent<Rigidbody> ().AddForce (fwd * throwForce * Time.deltaTime);

		Destroy (rockInst, 5);

		Invoke("attackCooldown", 2.8f);
	}

	void attackCooldown()
	{
		canAttack = true;
	}


	public void UpdateHealth(float dmg)
	{
		health -= dmg/2;
	}

	public float getHealth()
	{
		return health;
	}
}
