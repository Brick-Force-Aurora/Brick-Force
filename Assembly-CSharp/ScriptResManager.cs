using System.Collections.Generic;
using UnityEngine;

public class ScriptResManager : MonoBehaviour
{
	public GameObject Executor;

	public ScriptDialogBg[] DialogBg;

	public ScriptSnd[] sounds;

	public Texture2D[] CmdIcon;

	public Texture2D sergeant;

	private static ScriptResManager _instance;

	public static ScriptResManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = (Object.FindObjectOfType(typeof(ScriptResManager)) as ScriptResManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get ScriptResManager Instance");
				}
			}
			return _instance;
		}
	}

	private void OnApplicationQuit()
	{
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
	}

	public AudioClip GetAudioClip(int index)
	{
		AudioClip result = null;
		if (0 <= index && index < sounds.Length)
		{
			result = sounds[index].audioClip;
		}
		return result;
	}

	public Texture2D GetDialogBg(int index)
	{
		Texture2D result = null;
		if (0 <= index && index < DialogBg.Length)
		{
			result = DialogBg[index].dialogBg;
		}
		return result;
	}

	public Texture2D[] GetDlgIconArray()
	{
		List<Texture2D> list = new List<Texture2D>();
		for (int i = 0; i < DialogBg.Length; i++)
		{
			list.Add(DialogBg[i].bgIcon);
		}
		return list.ToArray();
	}

	public string[] GetDlgAliasArray()
	{
		List<string> list = new List<string>();
		for (int i = 0; i < DialogBg.Length; i++)
		{
			list.Add(DialogBg[i].alias);
		}
		return list.ToArray();
	}

	public Texture2D[] GetSndIconArray()
	{
		List<Texture2D> list = new List<Texture2D>();
		for (int i = 0; i < sounds.Length; i++)
		{
			list.Add(sounds[i].audioIcon);
		}
		return list.ToArray();
	}

	public string[] GetSndAliasArray()
	{
		List<string> list = new List<string>();
		for (int i = 0; i < sounds.Length; i++)
		{
			list.Add(sounds[i].alias);
		}
		return list.ToArray();
	}
}
