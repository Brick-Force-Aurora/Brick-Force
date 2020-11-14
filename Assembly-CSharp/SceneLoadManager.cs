using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadManager : MonoBehaviour
{
	public Dictionary<int, SceneLoadManager> autoFunctionMap = new Dictionary<int, SceneLoadManager>();

	private static SceneLoadManager _instance;

	private AsyncOperation async;

	private string levelName;

	public static SceneLoadManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(SceneLoadManager)) as SceneLoadManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the SceneLoadManager Instance");
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	private void Update()
	{
	}

	public void SceneLoadLevelAsync(string level)
	{
		StartCoroutine("LoadLevelAsync", level);
	}

	private IEnumerator LoadLevelAsync(string level)
	{
		levelName = level;
		async = Application.LoadLevelAsync(level);
		yield return (object)async;
	}

	public bool IsLoadedDone()
	{
		if (async != null && async.isDone)
		{
			return true;
		}
		return false;
	}

	public bool IsLoadStart(string level)
	{
		return level.Equals(levelName);
	}

	public string GetProgressString()
	{
		if (async == null)
		{
			return "Load Ready";
		}
		if (async.isDone)
		{
			return " Load Compleate";
		}
		return " " + ((int)(async.progress * 100f)).ToString() + " %";
	}
}
