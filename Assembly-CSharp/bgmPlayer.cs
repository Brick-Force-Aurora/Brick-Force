using UnityEngine;

public class bgmPlayer : MonoBehaviour
{
	public AudioClip[] bgm;

	public float minDelay = 3f;

	public float maxDelay = 6f;

	private float delayTime;

	private float deltaTime;

	private void Start()
	{
		Play();
	}

	private void Play()
	{
		AudioSource component = GetComponent<AudioSource>();
		if (component != null && bgm.Length > 0)
		{
			deltaTime = 0f;
			delayTime = Random.Range(minDelay, maxDelay);
			component.clip = bgm[Random.Range(0, bgm.Length)];
			component.Play();
		}
	}

	private void Update()
	{
		if (!base.audio.isPlaying)
		{
			deltaTime += Time.deltaTime;
			if (deltaTime > delayTime)
			{
				Play();
			}
		}
	}
}
