using UnityEngine;
using System.Collections;

public class Enemy : MovingObject
{
	public int PlayerDamage;

	private Animator _animator;
	private Transform _target;
	private bool _skipMove;

	public AudioClip[] EnemyAttackClips;

	protected override void Start()
	{
		GameManager.Instance.AddEnemyToList(this);

		_animator = GetComponent<Animator>();
		_target = GameObject.FindGameObjectWithTag("Player").transform;

		base.Start();
	}

	public void MoveEnemy()
	{
		var xDir = 0;
		var yDir = 0;

		if (Mathf.Abs(_target.position.x - transform.position.x) < float.Epsilon)
			yDir = _target.position.y > transform.position.y ? 1 : -1;
		else
			xDir = _target.position.x > transform.position.x ? 1 : -1;

		AttemptMove<Player>(xDir, yDir);
	}

	protected override void AttemptMove<T>(int xDir, int yDir)
	{
		if (_skipMove)
		{
			_skipMove = false;
			return;
		}

		base.AttemptMove<T>(xDir, yDir);

		_skipMove = true;
	}

	protected override void OnCantMove<T>(T component)
	{
		var player = component as Player;
		if (player != null)
		{
			player.LoseFood(PlayerDamage);
			_animator.SetTrigger("enemyAttack");

			SoundManager.Instance.RandomizeSfx(EnemyAttackClips);
		}
	}
}
