using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class MissionLoadManager : MonoBehaviour
{
	private static MissionLoadManager _instance;

	private bool[] reward1IsPoints;

	private bool[] reward2IsPoints;

	private bool[] reward3IsPoints;

	private int[] reward1Counts;

	private int[] reward2Counts;

	private int[] reward3Counts;

	public static MissionLoadManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = (UnityEngine.Object.FindObjectOfType(typeof(MissionLoadManager)) as MissionLoadManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get MissionLoadManager Instance");
				}
			}
			return _instance;
		}
	}

	public bool[] Reward1IsPoints => reward1IsPoints;

	public bool[] Reward2IsPoints => reward2IsPoints;

	public bool[] Reward3IsPoints => reward3IsPoints;

	public int[] Reward1Counts => reward1Counts;

	public int[] Reward2Counts => reward2Counts;

	public int[] Reward3Counts => reward3Counts;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	public void Load()
	{
		Property props = BuildOption.Instance.Props;
		if (props.isWebPlayer)
		{
			StartCoroutine(LoadFromWWW());
		}
		else
		{
			LoadFromLocalFileSystem();
		}
	}

	private IEnumerator LoadFromWWW()
	{
		bool Loaded = false;
		Property prop = BuildOption.Instance.Props;
		string tempName = "/BfData/Template/missionReward.txt.cooked";
		string url = "http://" + prop.GetResourceServer + tempName;
		WWW www = new WWW(url);
		yield return (object)www;
		using (MemoryStream stream = new MemoryStream(www.bytes))
		{
			using (BinaryReader reader = new BinaryReader(stream))
			{
				CSVLoader csvLoader = new CSVLoader();
				if (csvLoader.SecuredLoadFromBinaryReader(reader))
				{
					Parse(csvLoader);
					Loaded = true;
				}
			}
		}
		if (!Loaded)
		{
			Debug.LogError("Fail to download " + url);
		}
	}

	private bool LoadFromLocalFileSystems()
	{
		string text = Path.Combine(Application.dataPath, "Resources");
		if (!Directory.Exists(text))
		{
			Debug.LogError("ERROR, Fail to find directory for Resources");
			return false;
		}
		string path = "Template/missionReward.txt";
		string text2 = Path.Combine(text, path);
		CSVLoader cSVLoader = new CSVLoader();
		if (Application.platform == RuntimePlatform.WindowsEditor || !cSVLoader.SecuredLoad(text2))
		{
			if (!cSVLoader.Load(text2))
			{
				Debug.LogError("ERROR, Fail to load resource file" + text2);
				return false;
			}
			if (!cSVLoader.SecuredSave(text2))
			{
				Debug.LogError("ERROR, Load success " + text2 + " but save secured failed");
			}
		}
		Parse(cSVLoader);
		return true;
	}

	private bool LoadFromLocalFileSystem()
	{
		string text = Path.Combine(Application.dataPath, "Resources");
		if (!Directory.Exists(text))
		{
			Debug.LogError("ERROR, Fail to find directory for Resources");
			return false;
		}
		string path = "Template/missionReward.txt";
		string text2 = Path.Combine(text, path);
		CSVLoader cSVLoader = new CSVLoader();
		if (Application.platform == RuntimePlatform.WindowsEditor || !cSVLoader.SecuredLoad(text2))
		{
			if (!cSVLoader.Load(text2))
			{
				Debug.LogError("ERROR, Fail to load resource file" + text2);
				return false;
			}
			if (!cSVLoader.SecuredSave(text2))
			{
				Debug.LogError("ERROR, Load success " + text2 + " but save secured failed");
			}
		}
		Parse(cSVLoader);
		return true;
	}

	private void Parse(CSVLoader csvLoader)
	{
		reward1IsPoints = new bool[csvLoader.Rows];
		reward2IsPoints = new bool[csvLoader.Rows];
		reward3IsPoints = new bool[csvLoader.Rows];
		reward1Counts = new int[csvLoader.Rows];
		reward2Counts = new int[csvLoader.Rows];
		reward3Counts = new int[csvLoader.Rows];
		for (int i = 0; i < csvLoader.Rows; i++)
		{
			csvLoader.ReadValue(0, i, string.Empty, out string Value);
			csvLoader.ReadValue(1, i, string.Empty, out string Value2);
			csvLoader.ReadValue(2, i, string.Empty, out string Value3);
			csvLoader.ReadValue(3, i, string.Empty, out string Value4);
			csvLoader.ReadValue(4, i, string.Empty, out string Value5);
			csvLoader.ReadValue(5, i, string.Empty, out string Value6);
			csvLoader.ReadValue(6, i, string.Empty, out string Value7);
			Value.Trim();
			Value.ToLower();
			Value2.Trim();
			Value2.ToLower();
			Value3.Trim();
			Value3.ToLower();
			Value4.Trim();
			Value4.ToLower();
			Value5.Trim();
			Value5.ToLower();
			Value6.Trim();
			Value6.ToLower();
			Value7.Trim();
			Value7.ToLower();
			reward1IsPoints[i] = ((Convert.ToInt32(Value2) == 1) ? true : false);
			reward2IsPoints[i] = ((Convert.ToInt32(Value3) == 1) ? true : false);
			reward3IsPoints[i] = ((Convert.ToInt32(Value4) == 1) ? true : false);
			reward1Counts[i] = Convert.ToInt32(Value5);
			reward2Counts[i] = Convert.ToInt32(Value6);
			reward3Counts[i] = Convert.ToInt32(Value7);
		}
	}

	public void ChangeReward(int step, int forcePoint, int freeCoin)
	{
		int target = (int)BuildOption.Instance.target;
		if (target < reward1IsPoints.Length)
		{
			switch (step)
			{
			case 1:
				if (forcePoint > 0)
				{
					reward1IsPoints[target] = true;
					reward1Counts[target] = forcePoint;
				}
				if (freeCoin > 0)
				{
					reward1IsPoints[target] = false;
					reward1Counts[target] = freeCoin;
				}
				break;
			case 2:
				if (forcePoint > 0)
				{
					reward2IsPoints[target] = true;
					reward2Counts[target] = forcePoint;
				}
				if (freeCoin > 0)
				{
					reward2IsPoints[target] = false;
					reward2Counts[target] = freeCoin;
				}
				break;
			case 3:
				if (forcePoint > 0)
				{
					reward3IsPoints[target] = true;
					reward3Counts[target] = forcePoint;
				}
				if (freeCoin > 0)
				{
					reward3IsPoints[target] = false;
					reward3Counts[target] = freeCoin;
				}
				break;
			}
		}
	}
}
