using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainTitleScript : MonoBehaviour {

	[SerializeField] GameObject  instructionsObj;
	[SerializeField] GameObject  creditsObj;
	public Texture2D  cursorTexture;

	void Start()
	{
		Cursor.SetCursor(cursorTexture,Vector2.zero,CursorMode.Auto);
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	public void SceneChange(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}

	public void showInstructions()
	{
		instructionsObj.SetActive(true);
		creditsObj.SetActive(false);
	}

	public void showCredits()
	{
		instructionsObj.SetActive(false);
		creditsObj.SetActive(true);
	}

	public void exitGame()
	{
		Application.Quit();
	}




}
