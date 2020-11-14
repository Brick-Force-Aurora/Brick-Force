using UnityEngine;

public class PainSnd : MonoBehaviour
{
	private string[] deathVoc = new string[4]
	{
		"Die_long01",
		"Die_long02",
		"Die_long03",
		"Die_long04"
	};

	private string[] cryVoc = new string[3]
	{
		"Cry01",
		"Cry06",
		"Cry07"
	};

	private string[] hitVoc = new string[4]
	{
		"Hit02",
		"Hit03",
		"Hit04",
		"Hit06"
	};

	public bool isThirdPerson;

	private float damageVoiceTimeout;

	private float deltaTime;

	private AudioSource audioSource;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		if (null == audioSource)
		{
			Debug.LogError("Fail to find audio source ");
		}
		ResetDamageVoiceTimeout();
	}

	private void ResetDamageVoiceTimeout()
	{
		deltaTime = 0f;
		damageVoiceTimeout = Random.Range(0.7f, 1.5f);
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
	}

	private void OnDeath(int manID)
	{
		if (null != audioSource && audioSource.isPlaying)
		{
			audioSource.Stop();
		}
		AudioClip audioClip = null;
		if (!isThirdPerson)
		{
			audioClip = ((!MyInfoManager.Instance.IsYang) ? VoiceManager.Instance.Get(deathVoc[Random.Range(0, deathVoc.Length)]) : VoiceManager.Instance.Get2(deathVoc[Random.Range(0, deathVoc.Length)]));
		}
		else
		{
			GameObject gameObject = BrickManManager.Instance.Get(manID);
			if (gameObject != null)
			{
				LookCoordinator component = gameObject.GetComponent<LookCoordinator>();
				if (component != null)
				{
					audioClip = ((!component.IsYang) ? VoiceManager.Instance.Get(deathVoc[Random.Range(0, deathVoc.Length)]) : VoiceManager.Instance.Get2(deathVoc[Random.Range(0, deathVoc.Length)]));
				}
			}
		}
		if (null != audioClip && null != audioSource)
		{
			audioSource.PlayOneShot(audioClip);
		}
		ResetDamageVoiceTimeout();
	}

	private void OnHitSnd(int brickManBy)
	{
		if (deltaTime > damageVoiceTimeout)
		{
			AudioClip audioClip = null;
			if (!isThirdPerson)
			{
				audioClip = ((!MyInfoManager.Instance.IsYang) ? VoiceManager.Instance.Get(hitVoc[Random.Range(0, hitVoc.Length)]) : VoiceManager.Instance.Get2(hitVoc[Random.Range(0, hitVoc.Length)]));
			}
			else
			{
				GameObject gameObject = BrickManManager.Instance.Get(brickManBy);
				if (gameObject != null)
				{
					LookCoordinator component = gameObject.GetComponent<LookCoordinator>();
					if (component != null)
					{
						audioClip = ((!component.IsYang) ? VoiceManager.Instance.Get(cryVoc[Random.Range(0, cryVoc.Length)]) : VoiceManager.Instance.Get2(cryVoc[Random.Range(0, cryVoc.Length)]));
					}
				}
			}
			if (null != audioClip)
			{
				audioSource.PlayOneShot(audioClip);
			}
			ResetDamageVoiceTimeout();
		}
	}

	private void OnHitByUnknown(int hitMan)
	{
		if (deltaTime > damageVoiceTimeout)
		{
			AudioClip audioClip = null;
			if (!isThirdPerson)
			{
				audioClip = ((!MyInfoManager.Instance.IsYang) ? VoiceManager.Instance.Get(hitVoc[Random.Range(0, hitVoc.Length)]) : VoiceManager.Instance.Get2(hitVoc[Random.Range(0, hitVoc.Length)]));
			}
			else
			{
				GameObject gameObject = BrickManManager.Instance.Get(hitMan);
				if (gameObject != null)
				{
					LookCoordinator component = gameObject.GetComponent<LookCoordinator>();
					if (component != null)
					{
						audioClip = ((!component.IsYang) ? VoiceManager.Instance.Get(cryVoc[Random.Range(0, cryVoc.Length)]) : VoiceManager.Instance.Get2(cryVoc[Random.Range(0, cryVoc.Length)]));
					}
				}
			}
			if (null != audioClip)
			{
				audioSource.PlayOneShot(audioClip);
			}
			ResetDamageVoiceTimeout();
		}
	}
}
