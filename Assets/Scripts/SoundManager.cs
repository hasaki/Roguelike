using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
	public AudioSource fxSource;
	public AudioSource musicSource;
	public static SoundManager Instance = null;

	public float LowPitchRange = 0.95f;
	public float HighPitchRange = 1.05f;

	void Awake()
	{
		if (SoundManager.Instance == null)
			SoundManager.Instance = this;
		else if (SoundManager.Instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}

	public void PlaySingle(AudioClip clip)
	{
		fxSource.clip = clip;
		fxSource.Play();
	}

	public void RandomizeSfx(params AudioClip[] clips)
	{
		var randomIndex = Random.Range(0, clips.Length);
		var randomPitch = Random.Range(LowPitchRange, HighPitchRange);

		fxSource.clip = clips[randomIndex];
		fxSource.pitch = randomPitch;
		fxSource.Play();
	}
}
