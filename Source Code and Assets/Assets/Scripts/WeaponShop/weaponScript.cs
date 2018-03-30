using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using weaponShopInteraction;

public class weaponScript : MonoBehaviour {

	[SerializeField] GameObject weaponObject;
	[SerializeField] playerWeapons playerInfo;
	[SerializeField] Transform  turretHolder;
	public int weaponCost;
	public Text name;
	public Text cost;
	public Text description;

	private GameObject weaponInstance;
	private AudioSource aud;
	private int weaponOneBought = 0;
	private int weaponTwoBought = 0;
	private int weaponThreeBought = 0 ;

	// Use this for initialization
	void Start () 
	{
		aud = GetComponent<AudioSource> ();
		SetButton ();
	}

	void SetButton()
	{
		if(this.gameObject.tag == "weaponOne")
		{
			name.text = "Heavy Rifle";
			cost.text = "$150";
			description.text = "Slower fire rate, but higher damage";
		}
		else if(this.gameObject.tag == "weaponTwo")
		{
			name.text = "Turret Aid";
			cost.text = "$250";
			description.text = "Set a sensor gun on your base";
		}
		else if(this.gameObject.tag == "weaponThree")
		{
			name.text = "Ray Gun";
			cost.text = "$400";
			description.text = "Guarantees to termintate all life, or money back.";
		}

	}

	public void OnClick()
	{
		if(this.gameObject.tag == "weaponOne")
		{
			if (playerInfo.getCoins () >= weaponCost && weaponOneBought == 0) 
			{
				weaponOneBought = 1;
				playerInfo.setCoins(weaponCost);
				weaponInstance = Instantiate(weaponObject, Vector3.zero, Quaternion.identity) as GameObject;
				playerInfo.addWeapon(ref weaponInstance);
				cost.text = "SOLD OUT";
			}
			else
			{
				//aud.Play();
			}
		}
		else if(this.gameObject.tag == "weaponTwo")
		{
			if (playerInfo.getCoins () >= weaponCost && weaponTwoBought == 0) 
			{
				weaponTwoBought = 1;
				playerInfo.setCoins(weaponCost);
				Instantiate(weaponObject, turretHolder.position, turretHolder.rotation);
				cost.text = "SOLD OUT";
			}
			else
			{
				//aud.Play();
			}
		}
		else if(this.gameObject.tag == "weaponThree")
		{
			if (playerInfo.getCoins () >= weaponCost && weaponThreeBought == 0) 
			{
				weaponThreeBought = 1;
				playerInfo.setCoins(weaponCost);
				weaponInstance = Instantiate(weaponObject, Vector3.zero, Quaternion.identity) as GameObject;
				playerInfo.addWeapon(ref weaponInstance);
				cost.text = "SOLD OUT";
			}
			else
			{
				//aud.Play();
			}
		}
	}

	
	// Update is called once per frame
	void Update () {
		
	}
}
