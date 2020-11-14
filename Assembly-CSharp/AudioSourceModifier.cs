using UnityEngine;

public class AudioSourceModifier : MonoBehaviour
{
	public enum TYPE
	{
		SFX,
		BGM
	}

	public TYPE type = TYPE.BGM;

	private AudioSource audioSource;

	private float fOriginalVolume = 1f;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		if (audioSource != null)
		{
			fOriginalVolume = audioSource.volume;
			Modify();
		}
	}

	private void OnChangeAudioSource()
	{
		if (!(audioSource == null))
		{
			Modify();
		}
	}

	private void Modify()
	{
		bool flag = false;
		float num = 1f;
		switch (type)
		{
		case TYPE.BGM:
			flag = ((PlayerPrefs.GetInt("BgmMute", 0) != 0) ? true : false);
			num = PlayerPrefs.GetFloat("BgmVolume", 1f);
			break;
		case TYPE.SFX:
			flag = ((PlayerPrefs.GetInt("SfxMute", 0) != 0) ? true : false);
			num = PlayerPrefs.GetFloat("SfxVolume", 1f);
			break;
		}
		audioSource.mute = flag;
		if (!flag)
		{
			audioSource.volume = fOriginalVolume * num;
		}
		else
		{
			audioSource.volume = 0f;
		}
	}

	private void Update()
	{
	}
}
