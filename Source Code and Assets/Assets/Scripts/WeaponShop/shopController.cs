using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shopController : MonoBehaviour {

	public GameObject shopPanel;
	public GameObject backToGameBtn;
	public Texture2D  cursorTexture;

	private bool shopCD;

	void Start()
	{
		shopCD = false;
		Cursor.SetCursor(cursorTexture,Vector2.zero,CursorMode.Auto);
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag("playerControl") && !shopCD)
		{
			shopCD = true;
			Invoke("allowShopping", 1.5f);
			OpenShop();
		}
	}

	void OpenShop()
	{
		shopPanel.SetActive(true);
		backToGameBtn.SetActive(true);
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		Time.timeScale = 0;
	}

	public void CloseShop()
	{
		shopPanel.SetActive(false);
		backToGameBtn.SetActive(false);
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		Time.timeScale = 1;
	}

	void allowShopping()
	{
		shopCD = false;
	}
}
