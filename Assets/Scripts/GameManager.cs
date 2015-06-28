using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	public float LevelStartDelay = 2f;
	public float TurnDelay = 0.1f;
	public static GameManager Instance = null;

	public BoardManager boardScript;
	public int PlayerFoodPoints = 100;
	
	[HideInInspector]
	public bool PlayersTurn = true;

	private Text _levelText;
	private GameObject _levelImage;
	private bool _doingSetup;
	private int _level = 1;
	private List<Enemy> _enemies;
	private bool _enemiesMoving;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else if (Instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);

		_enemies = new List<Enemy>();
		boardScript = GetComponent<BoardManager>();
		InitGame();
	}

	private void OnLevelWasLoaded(int index)
	{
		_level++;
		InitGame();
	}

	void Update()
	{
		if (PlayersTurn || _enemiesMoving || _doingSetup)
			return;

		StartCoroutine(MoveEnemies());
	}

	public void AddEnemyToList(Enemy enemy)
	{
		_enemies.Add(enemy);
	}

	public void GameOver()
	{
		_levelText.text = "After " + _level + " days, you starved.";
		_levelImage.SetActive(true);

		enabled = false;
	}

	private void InitGame()
	{
		_doingSetup = true;
		_levelImage = GameObject.Find("LevelImage");
		_levelText = GameObject.Find("LevelText").GetComponent<Text>();
		_levelText.text = "Day " + _level;
		_levelImage.SetActive(true);

		_enemies.Clear();
		boardScript.SetupScene(_level);
		Invoke("HideLevelImage", LevelStartDelay);
	}

	private void HideLevelImage()
	{
		_levelImage.SetActive(false);
		_doingSetup = false;
	}

	IEnumerator MoveEnemies()
	{
		_enemiesMoving = true;
		yield return new WaitForSeconds(TurnDelay);
		if (_enemies.Count == 0)
		{
			yield return new WaitForSeconds(TurnDelay);
		}

		for (var i = 0; i < _enemies.Count; i++)
		{
			_enemies[i].MoveEnemy();
			yield return new WaitForSeconds(_enemies[i].MoveTime);
		}

		PlayersTurn = true;
		_enemiesMoving = false;
	}
}
