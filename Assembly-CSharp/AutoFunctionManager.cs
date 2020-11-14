using System.Collections.Generic;
using UnityEngine;

public class AutoFunctionManager : MonoBehaviour
{
	public Dictionary<int, AutoFunction> autoFunctionMap = new Dictionary<int, AutoFunction>();

	private static AutoFunctionManager _instance;

	public static AutoFunctionManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(AutoFunctionManager)) as AutoFunctionManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the AutoFunctionManager Instance");
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	public int AddAutoFunction(AutoFunction autoFunction)
	{
		int hashCode = autoFunction.GetHashCode();
		autoFunctionMap.Add(hashCode, autoFunction);
		if (autoFunctionMap.Count > 100)
		{
			Debug.LogError("autoFunctionMap.Count > 100 : Too Many AddAutoFunction");
		}
		return hashCode;
	}

	public int AddRepeatFunction(FunctionPointer fp, float endTime)
	{
		return AddAutoFunction(new AutoFunction(fp, endTime));
	}

	public int AddEndFunction(float endTime, FunctionPointer endfp)
	{
		return AddAutoFunction(new AutoFunction(null, endTime, 1E-05f, endfp));
	}

	private void Update()
	{
		List<object> list = new List<object>();
		foreach (KeyValuePair<int, AutoFunction> item in autoFunctionMap)
		{
			if (item.Value.Update())
			{
				list.Add(item.Key);
			}
		}
		if (list.Count > 0)
		{
			foreach (int item2 in list)
			{
				DeleteAutoFunction(item2);
			}
		}
	}

	public int DeleteAutoFunction(int key)
	{
		if (autoFunctionMap.ContainsKey(key))
		{
			autoFunctionMap[key].EndFunctionCall();
			autoFunctionMap.Remove(key);
		}
		return 0;
	}

	public void DeleteOnly(int key)
	{
		if (autoFunctionMap.ContainsKey(key))
		{
			autoFunctionMap.Remove(key);
		}
	}

	public void DeleteAllAutoFunction()
	{
		autoFunctionMap.Clear();
	}
}
