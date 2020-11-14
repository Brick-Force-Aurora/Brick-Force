using System.Collections.Generic;
using UnityEngine;

public class VoiceManager : MonoBehaviour
{
	private Dictionary<string, AudioClip> dic;

	private Dictionary<string, AudioClip> dic2;

	public bool bLoaded;

	public bool bLoaded2;

	private static VoiceManager _instance;

	public static VoiceManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(VoiceManager)) as VoiceManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the VoiceManager Instance");
				}
			}
			return _instance;
		}
	}

	public void Add(string key, AudioClip clip)
	{
		key = key.ToLower();
		if (!dic.ContainsKey(key))
		{
			dic.Add(key, clip);
		}
	}

	public void Add2(string key, AudioClip clip)
	{
		key = key.ToLower();
		if (!dic2.ContainsKey(key))
		{
			dic2.Add(key, clip);
		}
	}

	public AudioClip Get(string key)
	{
		key = key.ToLower();
		if (!dic.ContainsKey(key))
		{
			return null;
		}
		return dic[key];
	}

	public AudioClip Get2(string key)
	{
		key = key.ToLower();
		if (!dic2.ContainsKey(key))
		{
			return null;
		}
		return dic2[key];
	}

	public void Play0(string key)
	{
		AudioClip audioClip = Get(key);
		if (audioClip != null)
		{
			GlobalVars.Instance.PlaySound(audioClip);
		}
	}

	public void Play(string key)
	{
		if (MyInfoManager.Instance.IsYang)
		{
			Play2(key);
		}
		else
		{
			AudioClip audioClip = Get(key);
			if (audioClip != null)
			{
				GlobalVars.Instance.PlaySound(audioClip);
			}
		}
	}

	public void Play2(string key)
	{
		AudioClip audioClip = Get2(key);
		if (audioClip != null)
		{
			GlobalVars.Instance.PlaySound(audioClip);
		}
	}

	private void Start()
	{
	}

	public void Clear()
	{
		if (dic != null && dic.Count > 0)
		{
			dic.Clear();
		}
	}

	public void Clear2()
	{
		if (dic2 != null && dic2.Count > 0)
		{
			dic2.Clear();
		}
	}

	private void Update()
	{
	}

	private void Awake()
	{
		dic = new Dictionary<string, AudioClip>();
		dic2 = new Dictionary<string, AudioClip>();
		Object.DontDestroyOnLoad(this);
	}
}
