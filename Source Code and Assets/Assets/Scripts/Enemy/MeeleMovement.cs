using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeeleMovement : MonoBehaviour {


	[SerializeField] GameObject modelMesh;
	[SerializeField] float jumpForce;
	[SerializeField] float forceSpeed;

	GameObject playerObj;
	GameObject distanceToFortress;
	Animator anim;
	Rigidbody rb;


	bool  isEnemyDead = false;
	bool  jumpToPlayer = true;
	bool  onFortress = false;
	bool  isAttacking = false;
	bool  attackPoint = false;
	float speed = 4.0f;
	float mass = 5.0f;
	float force = 50.0f;
	float minimunDistancetoAvoid = 5.0f;
	float rotationSpeed = 2.0f;
	float health = 150f;
	int   rewardValue = 25;



	void Awake()
	{
		playerObj = GameObject.FindGameObjectWithTag("Player");
		distanceToFortress = GameObject.FindGameObjectWithTag("fortressDistance");
		anim = modelMesh.GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
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


			if(Vector3.Distance(distanceToFortress.transform.position, transform.position) < 36.5f && !onFortress)
			{	
				if(jumpToPlayer)
				{	
					jumpToPlayer = false;
					anim.SetTrigger("isJumping");
					Invoke("JumpToFortress", 1.2f);
				}
				return;
			}

			if(Vector3.Distance(playerObj.transform.position, transform.position) < 4.5f)
			{
				if(!isAttacking && onFortress)
				{
					isAttacking = true;
					anim.SetTrigger("isAttacking");
					Invoke("setAttackPoint", 0.9f);
					Invoke("attackIntervalFinished", 1.2f);
					Invoke("AttackCoolDown", 2.0f);					
				}
			}


			if(!isAttacking)
			{
				Quaternion  rotation = Quaternion.LookRotation(direction);
				transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 5.0f * Time.deltaTime);
				transform.Translate (Vector3.forward * speed * Time.deltaTime);
			}
		}

	}

	void OnCollisionEnter(Collision other)
	{
		if(other.gameObject.tag == "ground")
		{			
			//rb.AddForce(Vector3.down * 150000, ForceMode.Impulse);
		}

		if(other.gameObject.tag == "fortress")
		{
			onFortress = true;
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

	void JumpToFortress()
	{
		Vector3 JumpDirection = new Vector3 (0,1,-1);
		rb.AddForce(JumpDirection * forceSpeed * Time.deltaTime);
		rb.AddForce(JumpDirection * jumpForce, ForceMode.Impulse);
		Invoke("LandAnimation", 1.5f);
	}

	void LandAnimation()
	{
		anim.SetTrigger("onGround");
	}

	void AttackCoolDown()
	{
		isAttacking = false;
	}

	void setAttackPoint()
	{
		attackPoint = true;
	}

	void attackIntervalFinished()
	{
		attackPoint = false;
	}

	public bool getIsAttacking()
	{
		return attackPoint;
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
