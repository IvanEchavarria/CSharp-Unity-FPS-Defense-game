using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIFSM : MonoBehaviour {

	[SerializeField] Transform goalObject;
	[SerializeField] Transform enemyWeapon;

	//List to hold possible cover places and obstacles
	//public static List<Vector3> obstacles = new List<Vector3>();

	//List to hold hazzards to avoid 
	//public static List<Vector3> hazzards = new List<Vector3>();

	//List to hold best way to reach the player location
	public static List<Vector3> roadToPlayer = new List<Vector3>();

	public static Vector3     bestDirectionToGoal = Vector3.zero; 		  // Vector Direction to goal
	public static GameObject  PlayerObj = null;			                      // Get a hold of the player location so the enemy knows where he is
	public static bool        knowsPlayerlocation = false;	              // True if we know the player location
	public static float       finalDistanceToGoal = 500000.0f;            // Is the best distance reached by a character

	public enum FiniteStateMachine
	{
		MoveToTarget,
		MoveForward,
		Rotate,
		FoundPlayer
	}

	public enum BattleStateMachine
	{
		BeingDamaged,
		JumpSide,
		ShootPlayer,
		FindCover,
		Dead
	}


	FiniteStateMachine currentState ; 	// Current State of the state machine
	BattleStateMachine battleState;
	Vector3 PositionToFollow = Vector3.zero;			// Target Position the enemy will move towards to based on the information it has
	Vector3 newDirection;
	Vector3 hitNormal;
	RaycastHit hit;
	RaycastHit obstacleHit;
	float distanceToGoal;				// How far are we from the goal 
	float movementSpeed; 				// Movement speed of the enemy  
	float rotationSpeed; 				// Rotation speed of the enemy
	float stopDistance;
	float minimumDistToAvoid;
	int   health;						// Health of the enemy
	bool  isDead;        				// Has this unit been destroyed
	bool  hasTarget;                    // If we have target follow it
	bool  turnLeft;
	bool  turnRight;
	bool  keepForward;
	float speed = 20.0f;
	float mass = 5.0f;
	float force  = 40.0f;



	// Use this for initialization
	void Start ()
	{		
		ParameterSetup ();
	}
	
	// Update is called once per frame
	void Update () 
	{	
		UpdateStateMachine ();
		sensorInputDetection ();
	}

	void ParameterSetup()
	{
		currentState = FiniteStateMachine.MoveForward;
		movementSpeed = 5.0f;
		rotationSpeed = 2.0f;
		stopDistance = 25.0f;
		newDirection = Vector3.zero;
	    hitNormal = Vector3.zero;
		minimumDistToAvoid = 5.0f;
		isDead = false;
		turnLeft = false;
		turnRight = false;
		keepForward = false;
		health = 100;
		Vector3 distance = goalObject.position - transform.position;
		distanceToGoal = distance.magnitude;

		if (roadToPlayer.Count <= 0 && !knowsPlayerlocation) // We dont have location to follow
		{
			transform.Rotate (0.0f, Random.Range (0.0f, 180f), 0.0f); // Rotate transforms in random directions 
		} 
		else 
		{
			Quaternion targetRotation = Quaternion.LookRotation(PositionToFollow - transform.position); 
			transform.rotation = Quaternion.RotateTowards (transform.rotation, targetRotation, rotationSpeed);
		}

		if (knowsPlayerlocation) 
		{
			hasTarget = true;
			PositionToFollow = PlayerObj.transform.position;
			currentState = FiniteStateMachine.MoveToTarget;
		}
		else if (roadToPlayer.Count <= 0) 
		{
			hasTarget = false;
			currentState = FiniteStateMachine.MoveForward;
		} 
		else 
		{
			PositionToFollow = roadToPlayer [0];
			hasTarget = true;
			currentState = FiniteStateMachine.MoveToTarget;
		}

	}

	void UpdateStateMachine()
	{
		switch (currentState) 
		{
		case FiniteStateMachine.MoveToTarget:
			 FindTargetPosition ();
			 break;
		case FiniteStateMachine.MoveForward:
			 MoveForwardDirection ();
			 break;
		case FiniteStateMachine.Rotate:
			 Rotate45Degrees ();
			 break;
		case FiniteStateMachine.FoundPlayer:
			 BattleStateMachineUpdate();
			 break;
		}
		if(knowsPlayerlocation)
		{	
			PositionToFollow = roadToPlayer[0];

			if(Vector3.Distance (transform.position, PlayerObj.transform.position) > 30.0f)
			{
				currentState = FiniteStateMachine.MoveToTarget;
			}
			else
			{
				currentState = FiniteStateMachine.FoundPlayer;
				battleState = BattleStateMachine.ShootPlayer;
			}
		}

	}

	void BattleStateMachineUpdate()
	{
		roadToPlayer.Insert (0, PlayerObj.transform.position);
		PositionToFollow = roadToPlayer [0];
		Quaternion targetRotation = Quaternion.LookRotation(PositionToFollow - transform.position); 
		transform.rotation = Quaternion.RotateTowards (transform.rotation, targetRotation, rotationSpeed);
		transform.eulerAngles = new Vector3 (0.0f, transform.eulerAngles.y, 0.0f);
		switch(battleState)
		{
		case BattleStateMachine.ShootPlayer:
			 PointAndShoot();
			 break;
		}
	}

	void FindTargetPosition ()
	{
		if(knowsPlayerlocation)
		{
			Quaternion targetRotation = Quaternion.LookRotation(PositionToFollow - transform.position); 
			transform.rotation = Quaternion.RotateTowards (transform.rotation, targetRotation, rotationSpeed);
			transform.eulerAngles = new Vector3 (0.0f, transform.eulerAngles.y, 0.0f);
			transform.Translate(Vector3.forward * Time.deltaTime * movementSpeed); 
		}

		//Rotate to the target point
		if (Vector3.Distance (transform.position, PositionToFollow) < 5.0f) 
		{
			currentState = FiniteStateMachine.Rotate;
			currentState = FiniteStateMachine.MoveForward;
		} 
		else 
		{	
			PositionToFollow = roadToPlayer [0];
			Quaternion targetRotation = Quaternion.LookRotation(PositionToFollow - transform.position); 
			transform.rotation = Quaternion.RotateTowards (transform.rotation, targetRotation, rotationSpeed);
			transform.eulerAngles = new Vector3 (0.0f, transform.eulerAngles.y, 0.0f);
			//Go Forward
			transform.Translate(Vector3.forward * Time.deltaTime * movementSpeed); 
		}
	}

	void MoveForwardDirection ()
	{

		//Go Forward
		transform.Translate(Vector3.forward * Time.deltaTime * movementSpeed); 


		//Is the distance to the goal being reduced? if so then create a new way point
		float newDistanceToGoal = Vector3.Distance(transform.position, goalObject.position);


		if (newDistanceToGoal < distanceToGoal ) 
		{
			if (newDistanceToGoal < finalDistanceToGoal) 
			{
				bestDirectionToGoal = transform.forward;
				bestDirectionToGoal.Normalize ();
				roadToPlayer.Insert (0, transform.position);
				PositionToFollow = roadToPlayer [0];
				finalDistanceToGoal = newDistanceToGoal;
				//choose side to go to
				//currentState = FiniteStateMachine.MoveToTarget;
			//	Destroy(positionToFollowInst, 3.0f);
				currentState = FiniteStateMachine.Rotate;
			}
		}

		// If the distance from the goal is increasing, then we need to rotate, if there is an obstacle or hazzard we need to rotate
		if (Vector3.Distance(transform.position, goalObject.position) >= finalDistanceToGoal || Vector3.Distance (transform.position, goalObject.position) >= distanceToGoal) 
		{
			currentState = FiniteStateMachine.MoveToTarget;
		}
	}

	void Rotate45Degrees()
	{
		
		int choice = Random.Range (0, 3);

		if (choice == 0 && !keepForward || turnLeft && turnRight) 
		{
			transform.Rotate (0.0f, 0.0f, 0.0f); // Rotate transforms in random directions 
			keepForward = true;
			currentState = FiniteStateMachine.MoveForward; // Move Forward
		} 
		else if (choice == 1 && !turnLeft || keepForward && turnRight) 
		{
			transform.Rotate (0.0f, -15.0f, 0.0f); // Rotate transforms in random directions 
			turnLeft = true;
			currentState = FiniteStateMachine.MoveForward; // Move Forward
		} 
		else if (choice == 2 && !turnRight || keepForward && turnLeft) 
		{
			transform.Rotate (0.0f, 15.0f, 0.0f); // Rotate transforms in random directions 
			turnRight = true;
			currentState = FiniteStateMachine.MoveForward; // Move Forward

		} 
		else 
		{
			turnLeft = false;
			turnRight = false;
			keepForward = false;
			currentState = FiniteStateMachine.Rotate;
		}


	}

	void sensorInputDetection()
	{
		Debug.DrawRay (transform.position, (transform.forward + transform.right) * 10.0f, Color.red);
		Debug.DrawRay (transform.position, transform.forward * 10.0f, Color.red);
		Debug.DrawRay (transform.position, (transform.forward - transform.right) * 10.0f, Color.red);
		Debug.DrawRay (transform.position, (transform.forward + hitNormal)* 10.0f, Color.blue);

		if(Physics.Raycast(transform.position, transform.forward, out obstacleHit, minimumDistToAvoid))
		{
			if(obstacleHit.collider.tag == "obstacle")
			{
				Debug.Log("I see a wall");
				hitNormal = obstacleHit.normal;
				float angle = Vector3.Angle(transform.forward, hitNormal);
				hitNormal.y = 0.0f;
				transform.Rotate (0.0f, angle, 0.0f);

			}
		}

		// Right side sensor detection
		if (Physics.Raycast (transform.position, (transform.forward + transform.right), out hit, minimumDistToAvoid)) 
		{
			if (hit.collider.tag == "rightWall") 
			{
				transform.Rotate (0.0f, -15.0f, 0.0f);
			}

			if(hit.collider.tag == "goalObject" && !knowsPlayerlocation)
			{
				GameObject objectPlayer = GameObject.FindGameObjectWithTag("Player");
				PlayerObj = objectPlayer;
				knowsPlayerlocation = true;
				currentState = FiniteStateMachine.FoundPlayer;
			}
		}
		// Left side sensor detection
		else if(Physics.Raycast (transform.position, (transform.forward - transform.right), out hit, minimumDistToAvoid))
		{
			if (hit.collider.tag == "leftWall") 
			{
				transform.Rotate (0.0f, 15.0f, 0.0f);
			}
			if(hit.collider.tag == "goalObject" && !knowsPlayerlocation)
			{
				GameObject objectPlayer = GameObject.FindGameObjectWithTag("Player");
				PlayerObj = objectPlayer;
				knowsPlayerlocation = true;
				currentState = FiniteStateMachine.FoundPlayer;
			}
		}
		// Forward sensor detection
		else if(Physics.Raycast (transform.position, transform.forward , out hit, minimumDistToAvoid))
		{
			if (hit.collider.tag == "backWall") 
			{
				transform.Rotate (0.0f, 180.0f, 0.0f);
			}
			if(hit.collider.tag == "goalObject" && !knowsPlayerlocation)
			{
				GameObject objectPlayer = GameObject.FindGameObjectWithTag("Player");
				PlayerObj = objectPlayer;
				knowsPlayerlocation = true;
				currentState = FiniteStateMachine.FoundPlayer;
			}
		}
	}

	void PointAndShoot()
	{
		Vector3 destVect = PlayerObj.transform.position - transform.position;

		Quaternion destRot = Quaternion.LookRotation (destVect);

		enemyWeapon.rotation = Quaternion.RotateTowards (enemyWeapon.rotation, destRot, rotationSpeed);

		enemyWeapon.eulerAngles = new Vector3 (enemyWeapon.eulerAngles.x, enemyWeapon.eulerAngles.y, 0.0f);
	}






	/*
/*	
		//Set Random destination point first
		FindNextPoint();

	//Update each frame
	protected override void FSMUpdate()
	{
		switch (curState)
		{
		case FSMState.Patrol: UpdatePatrolState(); break;
		case FSMState.Chase: UpdateChaseState(); break;
		case FSMState.Attack: UpdateAttackState(); break;
		case FSMState.Dead: UpdateDeadState(); break;
		}

		//Go to dead state is no health left
		if (health <= 0)
			curState = FSMState.Dead;
	}

	/// <summary>
	/// Patrol state
	/// </summary>
	protected void UpdatePatrolState()
	{
		//Find another random patrol point if the current point is reached
		if (Vector3.Distance(transform.position, destPos) <= 100.0f)
		{
			FindNextPoint();
		}
		//Check the distance with player tank
		//When the distance is near, transition to chase state
		else if (Vector3.Distance(transform.position, playerTransform.position) <= 300.0f)
		{

			curState = FSMState.Chase;
		}

		//Rotate to the target point
		Quaternion targetRotation = Quaternion.LookRotation(destPos - transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * curRotSpeed);

		//Go Forward
		transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);
	}

	/// <summary>
	/// Chase state
	/// </summary>
	protected void UpdateChaseState()
	{
		//Set the target position as the player position
		destPos = playerTransform.position;

		//Check the distance with player tank
		//When the distance is near, transition to attack state
		float dist = Vector3.Distance(transform.position, playerTransform.position);
		if (dist <= 200.0f)
		{
			curState = FSMState.Attack;
		}
		//Go back to patrol is it become too far
		else if (dist >= 300.0f)
		{
			curState = FSMState.Patrol;
		}

		//Go Forward
		transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);
	}

	/// <summary>
	/// Attack state
	/// </summary>
	protected void UpdateAttackState()
	{
		//Set the target position as the player position
		destPos = playerTransform.position;

		//Check the distance with the player tank
		float dist = Vector3.Distance(transform.position, playerTransform.position);
		if (dist >= 200.0f && dist < 300.0f)
		{
			//Rotate to the target point
			Quaternion targetRotation = Quaternion.LookRotation(destPos - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * curRotSpeed);

			//Go Forward
			transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);

			curState = FSMState.Attack;
		}
		//Transition to patrol is the tank become too far
		else if (dist >= 300.0f)
		{
			curState = FSMState.Patrol;
		}

		//Always Turn the turret towards the player
		Quaternion turretRotation = Quaternion.LookRotation(destPos - turret.position);
		turret.rotation = Quaternion.Slerp(turret.rotation, turretRotation, Time.deltaTime * curRotSpeed);

		//Shoot the bullets
		ShootBullet();
	}

	/// <summary>
	/// Dead state
	/// </summary>
	protected void UpdateDeadState()
	{
		//Show the dead animation with some physics effects
		if (!bDead)
		{
			bDead = true;
			Explode();
		}
	}

	/// <summary>
	/// Shoot the bullet from the turret
	/// </summary>
	private void ShootBullet()
	{
		if (elapsedTime >= shootRate)
		{
			//Shoot the bullet
			Instantiate(Bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
			elapsedTime = 0.0f;
		}
	}

	/// <summary>
	/// Check the collision with the bullet
	/// </summary>
	/// <param name="collision"></param>
	void OnCollisionEnter(Collision collision)
	{
		//Reduce health
		if (collision.gameObject.tag == "Bullet")
			health -= collision.gameObject.GetComponent<Bullet>().damage;
	}

	/// <summary>
	/// Find the next semi-random patrol point
	/// </summary>
	protected void FindNextPoint()
	{
		int rndIndex = Random.Range(0, pointList.Length);
		Vector3 rndPosition = Vector3.zero;
		destPos = pointList[rndIndex].transform.position + rndPosition;



	}


	void Explode()
	{
		Destroy(gameObject, 1.5f);
	}   */

}
