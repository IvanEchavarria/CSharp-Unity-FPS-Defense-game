using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	[SerializeField] Slider playerHealth;
	[SerializeField] Slider fortressHealth;
	[SerializeField] Text   PlayerHealthPercentage;
	[SerializeField] Text   fortressHealthPercentage;
	[SerializeField] Text   moneyText;
	[SerializeField] Text   timerText;

	private int playerMoney;
	private int timerCount;
	private string timerToString;
	private string moneyToString;

	private static GameManager instanceManager;

	public static GameManager managerSharedInstance
	{
		get
		{
			if (instanceManager == null)
			{
				instanceManager = new GameManager();
			}

			return instanceManager;
		}
	}

	void Awake()
	{
		instanceManager = null;

		if(instanceManager != null && instanceManager != this)
		{
			Destroy(this.gameObject);
			return;
		}

		instanceManager = this;
		DontDestroyOnLoad(this.gameObject);
	}

	// Use this for initialization
	void Start () {

		timerCount = 0;
		playerMoney = 0;
		InvokeRepeating("timerFunction", 1.0f, 1.0f);

	}

	void timerFunction()
	{
		timerCount++;
		timerToString = timerCount.ToString();
		if(timerText != null)
		{
			timerText.text = "Timer : " + timerToString;
		}
		if(timerCount >= 120)
		{
			CancelInvoke();
			SceneManager.LoadScene("GameWon");
		}
	}

	public void updatePlayerHealth(float dmg)
	{
		playerHealth.value -= dmg;
		PlayerHealthPercentage.text = playerHealth.value + "%";
		if(playerHealth.value <= 0.0f)
		{
			CancelInvoke();
			SceneManager.LoadScene("GameOver");
		}

	}

	public void updateFortressHealth(float dmg)
	{
		fortressHealth.value -= dmg;
		fortressHealthPercentage.text = fortressHealth.value + "%";
		if(fortressHealth.value <= 0.0f)
		{
			CancelInvoke();
			SceneManager.LoadScene("GameOver");
		}
	}

	public void updateMoney(int amount)
	{
		playerMoney += amount;
		moneyToString = playerMoney.ToString();
		moneyText.text = "$ " + moneyToString;
	}

	public int getMoney()
	{
		return playerMoney;
	}
}
