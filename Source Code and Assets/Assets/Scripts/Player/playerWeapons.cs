using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace weaponShopInteraction 
{
	public class playerWeapons : MonoBehaviour {

		[SerializeField] GameObject weaponHolder;
		[SerializeField] GameObject mainWeapon;
		private static GameObject[] weaponsInventory;
		private static int numberOfWeapons;
		private static int weaponActiveNumber;



		// Use this for initialization
		void Start () {

			weaponsInventory = new GameObject[3];
			weaponsInventory[0] = mainWeapon;
			numberOfWeapons = 1;
			weaponActiveNumber = 0;
		}
		
		// Update is called once per frame
		void Update () 
		{
			if(Input.GetKeyDown(KeyCode.Alpha1))
			{
				activateWeapon(0);
			}
			else if(Input.GetKeyDown(KeyCode.Alpha2) && numberOfWeapons > 1)
			{
				activateWeapon(1);
			}
			else if(Input.GetKeyDown(KeyCode.Alpha3) && numberOfWeapons > 2)
			{
				activateWeapon(2);
			}
			
		}

		public void setCoins(int cost)
		{
			GameManager.managerSharedInstance.updateMoney(-cost);
		}

		public int getCoins()
		{
			return GameManager.managerSharedInstance.getMoney();
		}

		public void addWeapon(ref GameObject weapon)
		{
			if(numberOfWeapons < 3)
			{
				weaponsInventory[numberOfWeapons] = weapon;
				addWeaponChild();
				activateWeapon(numberOfWeapons);
				numberOfWeapons++;
			}
		}

		void addWeaponChild()
		{
			weaponsInventory[numberOfWeapons].transform.SetParent(weaponHolder.transform, false);
		}

		void activateWeapon(int weaponToActivate)
		{
			weaponsInventory[weaponActiveNumber].SetActive(false);
			weaponsInventory[weaponToActivate].SetActive(true);
			weaponActiveNumber = weaponToActivate;
		}


	}
}