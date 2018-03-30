using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletKill : MonoBehaviour {

	public float damage;
	private float enemyBulletDamage = 2.5f;
	private float boulderDamage = 7.5f;
	[SerializeField] GameObject RailGunAnimation;
	RaycastHit hit;

	void Awake()
	{
		if(damage >= 210.0f)
		{
			Instantiate(RailGunAnimation, transform.position,transform.rotation);
		}
	}

	void FixedUpdate()
	{
		//Debug.DrawRay(transform.position, transform.forward * 10.0f, Color.red);
		if(Physics.Raycast(transform.position, transform.forward, out hit, 10.0f))
		{				
			if (hit.collider.tag == "Player")
			{
				Debug.Log ("Hit player");
				GameManager.managerSharedInstance.updatePlayerHealth(enemyBulletDamage);
			}
			else if (hit.collider.tag == "Enemy")
			{
				Debug.Log ("Hit Enemy");
				if(hit.collider.transform.gameObject.GetComponent<EnemyMovement>())
				{
					hit.collider.transform.gameObject.GetComponent<EnemyMovement>().UpdateHealth(damage);
				}
				else if(hit.collider.transform.gameObject.GetComponent<TankMovementScript>())
				{
					hit.collider.transform.gameObject.GetComponent<TankMovementScript>().UpdateHealth(damage);
				}

				else if(hit.collider.transform.gameObject.GetComponent<MeeleMovement>())
				{
					hit.collider.transform.gameObject.GetComponent<MeeleMovement>().UpdateHealth(damage);
				}
			}

			if(this.gameObject.tag != "boulder")
			{
				Destroy (gameObject , 0.2f);
			}
		}							
 	}

	void OnCollisionEnter(Collision other)
	{
		if(this.gameObject.tag == "boulder")
		{
			if (other.gameObject.tag == "fortress")
			{
				GameManager.managerSharedInstance.updateFortressHealth(boulderDamage);
			}
			Destroy(gameObject, 0.8f);
		}
		else
		{
			if (other.gameObject.tag == "Player")
			{
				Debug.Log ("Hit player");
				GameManager.managerSharedInstance.updatePlayerHealth(enemyBulletDamage);
			}
			else if (other.gameObject.tag == "Enemy")
			{
				Debug.Log ("Hit Enemy");
				if(other.gameObject.GetComponent<EnemyMovement>())
				{
					other.gameObject.GetComponent<EnemyMovement>().UpdateHealth(damage);
				}
				else if(other.gameObject.GetComponent<TankMovementScript>())
				{
					other.gameObject.GetComponent<TankMovementScript>().UpdateHealth(damage);
				}

				else if(other.gameObject.GetComponent<MeeleMovement>())
				{
					other.gameObject.GetComponent<MeeleMovement>().UpdateHealth(damage);
				}
			}

			Destroy (gameObject, 0.2f);
		}		
	}
}
