using UnityEngine;

public class Aps : MonoBehaviour
{
	public enum APS_TYPE
	{
		NONE,
		CHINESE,
		KOREAN
	}

	public ApsData[] apsData;

	private APS_TYPE curType;

	private int curLevel;

	private static Aps _instance;

	public static Aps Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(Aps)) as Aps);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the Aps Instance");
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	public string SetLevel(int apsType, int level)
	{
		if (apsType < apsData.Length)
		{
			curType = (APS_TYPE)apsType;
			curLevel = level;
			if (level < apsData[apsType].warnings.Length)
			{
				return apsData[apsType].warnings[level];
			}
		}
		return string.Empty;
	}

	public Texture2D GetCurLevelIcon(bool flip)
	{
		if ((int)curType < apsData.Length && curLevel < apsData[(int)curType].icons.Length)
		{
			if (flip)
			{
				return apsData[(int)curType].flips[curLevel];
			}
			return apsData[(int)curType].icons[curLevel];
		}
		return null;
	}

	public string GetCurTooltip()
	{
		if ((int)curType < apsData.Length && curLevel < apsData[(int)curType].tooltips.Length && apsData[(int)curType].tooltips[curLevel].Length > 0)
		{
			return StringMgr.Instance.Get(apsData[(int)curType].tooltips[curLevel]);
		}
		return string.Empty;
	}
}
