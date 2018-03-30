using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour {


	[SerializeField] GameObject targetObj;
	[SerializeField] float SlowDistPerc;
	[SerializeField] float stopDist;
	[SerializeField] float slowRotPerc;
	[SerializeField] float VelocityMax;
	[SerializeField] float rotationMax;
	[SerializeField] float accelLinearInc;
	[SerializeField] float accelAngularInc;
	[SerializeField] float accelLinearMax;
	[SerializeField] float accelAngularMax;
	[SerializeField] Transform enemyWeapon;

	float slowDist;
	float slowRot;
	float rotLeft;
	float Velocity = 0.0f;
	float rotation = 0.0f;
	float accelLinear;
	float accelAngular;
	bool hasTarget = false;
	bool shootTarget= false;
	Vector3 destVect;
	Quaternion destRot;
	float distTo;

	// Use this for initialization
	void Start () {
		
		hasTarget = true;
		StartMoving ();
	}

	// Update is called once per frame
	void Update () {
		
		if (hasTarget) 
		{
			destVect = targetObj.transform.position - transform.position;
			distTo = destVect.magnitude;

			destRot = Quaternion.LookRotation (destVect);
			rotLeft = Quaternion.Angle (transform.rotation, destRot);


			transform.Translate (Vector3.forward * GetMoveSpeed () * Time.deltaTime);
			Velocity = Mathf.Clamp (Velocity + accelLinear, 0.0f, VelocityMax);
			accelLinear = Mathf.Clamp (accelLinear + accelLinearInc, 0.0f, accelLinearMax);

			transform.rotation = Quaternion.RotateTowards (transform.rotation, destRot, GetRotSpeed () * Time.deltaTime);
			rotation = Mathf.Clamp ((rotation + accelAngular), 0.0f, rotationMax);
			accelAngular = Mathf.Clamp ((accelAngular + accelAngularInc), 0.0f, accelAngularMax);

			enemyWeapon.rotation = Quaternion.RotateTowards (enemyWeapon.rotation, destRot, GetRotSpeed () * Time.deltaTime);


			enemyWeapon.eulerAngles = new Vector3 (enemyWeapon.eulerAngles.x, enemyWeapon.eulerAngles.y, 0.0f);
			transform.eulerAngles = new Vector3 (0.0f, transform.eulerAngles.y, 0.0f);

			if (distTo < stopDist) 
			{				
				hasTarget = false;
				Velocity = 0.0f;
				shootTarget = true;
			}

		}

		if (shootTarget) 
		{
			destVect = targetObj.transform.position - transform.position;
			distTo = destVect.magnitude;

			destRot = Quaternion.LookRotation (destVect);
			rotLeft = Quaternion.Angle (transform.rotation, destRot);

			transform.rotation = Quaternion.RotateTowards (transform.rotation, destRot, GetRotSpeed () * Time.deltaTime);
			rotation = Mathf.Clamp ((rotation + accelAngular), 0.0f, rotationMax);
			accelAngular = Mathf.Clamp ((accelAngular + accelAngularInc), 0.0f, accelAngularMax);

			enemyWeapon.rotation = Quaternion.RotateTowards (enemyWeapon.rotation, destRot, GetRotSpeed () * Time.deltaTime);

			enemyWeapon.eulerAngles = new Vector3 (enemyWeapon.eulerAngles.x, enemyWeapon.eulerAngles.y, 0.0f);
			transform.eulerAngles = new Vector3 (0.0f, transform.eulerAngles.y, 0.0f);
		}

	}

	void StartMoving()
	{
		accelLinear = 0.0f;
		accelAngular = 0.0f;
		rotation = 0.0f;

		destVect = targetObj.transform.position - transform.position;
		distTo = destVect.magnitude;
		slowDist = distTo * SlowDistPerc;

		destRot = Quaternion.LookRotation (destVect);
		rotLeft = Quaternion.Angle (transform.rotation, destRot);
		slowRot = rotLeft * slowRotPerc;
	}

	float GetMoveSpeed ()
	{
		return (distTo >= slowDist ? Velocity : Mathf.Lerp (0.0f, Velocity, distTo / slowDist));
	}

	float GetRotSpeed()
	{
		return (rotLeft >= slowRot ? rotation : Mathf.Lerp (0.0f, rotation, rotLeft / slowRot));
	}

	public bool ableToShoot()
	{
		return shootTarget;
	}

}
