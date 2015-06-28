using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour
{
	public Sprite DamageSprite;
	public int HP = 4;

	private SpriteRenderer _spriteRenderer;

	public AudioClip[] ChopSounds;

	void Awake()
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void DamageWall(int loss)
	{
		SoundManager.Instance.RandomizeSfx(ChopSounds);

		_spriteRenderer.sprite = DamageSprite;
		HP -= loss;

		if (HP <= 0)
			gameObject.SetActive(false);
	}
}
