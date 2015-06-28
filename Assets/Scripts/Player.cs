using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MovingObject
{
	public Text FoodText;
	public int WallDamage = 1;

	public int PointsPerFood = 10;
	public int PointsPerSoda = 20;

	public float RestartLevelDelay = 1f;

	public AudioClip[] MoveSounds;
	public AudioClip[] EatSounds;
	public AudioClip[] DrinkSounds;
	public AudioClip GameOverSound;

	private Animator _animator;
	private int _food;
	private Vector2 _touchOrigin = -Vector2.one;

	protected override void Start()
	{
		_animator = GetComponent<Animator>();
		_food = GameManager.Instance.PlayerFoodPoints;

		FoodText.text = "Food: " + _food;
		base.Start();
	}

	void Update()
	{
		if (!GameManager.Instance.PlayersTurn)
			return;

		var horizontal = 0;
		var vertical = 0;

#if UNITY_STANDALONE || UNITY_WEBPLAYER
		horizontal = (int) Input.GetAxisRaw("Horizontal");
		vertical = (int) Input.GetAxisRaw("Vertical");

		if (horizontal != 0)
			vertical = 0;
#else
		if (Input.touchCount > 0)
		{
			var touch = Input.touches[0];
			if (touch.phase == TouchPhase.Began)
			{
				_touchOrigin = touch.position;
			}
			else if (touch.phase == TouchPhase.Ended && _touchOrigin.x >= 0)
			{
				var touchEnd = touch.position;

				var x = touchEnd.x = _touchOrigin.x;
				var y = touchEnd.y = _touchOrigin.y;

				_touchOrigin.x = -1; // now ignore for the future

				if (Mathf.Abs(x) > Mathf.Abs(y))
					horizontal = x > 0 ? 1 : -1;
				else
					vertical = y > 0 ? 1 : -1;
			}
		}
#endif

		if (horizontal != 0 || vertical != 0)
			AttemptMove<Wall>(horizontal, vertical);
	}

	void OnDisable()
	{
		GameManager.Instance.PlayerFoodPoints = _food;
	}

	public void LoseFood(int loss)
	{
		_animator.SetTrigger("playerHit");
		_food -= loss;
		FoodText.text = "-" + loss + " Food: " + _food;
		CheckIfGameOver();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Exit")
		{
			Invoke("Restart", RestartLevelDelay);
			enabled = false;
		}
		else if (other.tag == "Food" || other.tag == "Soda")
		{
			var amt = PointsPerFood;
			var clips = EatSounds;

			if (other.tag == "Soda")
			{
				amt = PointsPerSoda;
				clips = DrinkSounds;
			}

			_food += amt;

			FoodText.text = "+" + amt + " Food: " + _food;
			SoundManager.Instance.RandomizeSfx(clips);

			other.gameObject.SetActive(false);
		}
	}

	protected override void AttemptMove<T>(int xDir, int yDir)
	{
		_food--;
		FoodText.text = "Food: " + _food;

		base.AttemptMove<T>(xDir, yDir);

		RaycastHit2D hit;
		if (Move(xDir, yDir, out hit))
			SoundManager.Instance.RandomizeSfx(MoveSounds);

		CheckIfGameOver();

		GameManager.Instance.PlayersTurn = false;
	}

	protected override void OnCantMove<T>(T component)
	{
		var hitWall = component as Wall;
		if (hitWall != null)
		{
			hitWall.DamageWall(WallDamage);

			_animator.SetTrigger("playerChop");
		}
	}

	private void Restart()
	{
		Application.LoadLevel(Application.loadedLevel);
	}

	private void CheckIfGameOver()
	{
		if (_food <= 0)
		{
			SoundManager.Instance.PlaySingle(GameOverSound);
			SoundManager.Instance.musicSource.Stop();
			GameManager.Instance.GameOver();
		}
	}
}
