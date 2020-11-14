using UnityEngine;

public class ClanMarkManager : MonoBehaviour
{
	public Texture2D[] bg;

	public Texture2D[] amblum;

	public Color[] colorTable;

	public Texture2D[] colorPanel;

	private static ClanMarkManager _instance;

	public static ClanMarkManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(ClanMarkManager)) as ClanMarkManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the ClanMarkManager Instance");
				}
			}
			return _instance;
		}
	}

	public int IndexToMark(int bg, int color, int amblum)
	{
		return amblum | (color << 16) | (bg << 24);
	}

	public int MarkToBg(int mark)
	{
		if (mark < 0)
		{
			return -1;
		}
		int num = 2130706432;
		int num2 = mark & num;
		return num2 >> 24;
	}

	public int MarkToColor(int mark)
	{
		if (mark < 0)
		{
			return -1;
		}
		int num = 255;
		num <<= 16;
		int num2 = mark & num;
		return num2 >> 16;
	}

	public int MarkToAmblum(int mark)
	{
		if (mark < 0)
		{
			return -1;
		}
		int num = 65535;
		return mark & num;
	}

	public Texture2D GetBg(int mark)
	{
		return GetBgByIndex(MarkToBg(mark));
	}

	public Texture2D GetColor(int mark)
	{
		return GetColorByIndex(MarkToColor(mark));
	}

	public Texture2D GetAmblum(int mark)
	{
		return GetAmblumByIndex(MarkToAmblum(mark));
	}

	public Color GetColorValue(int mark)
	{
		return GetColorValueByIndex(MarkToColor(mark));
	}

	public Texture2D GetBgByIndex(int index)
	{
		if (0 > index || bg.Length <= index)
		{
			return null;
		}
		return bg[index];
	}

	public Texture2D GetAmblumByIndex(int index)
	{
		if (0 > index || amblum.Length <= index)
		{
			return null;
		}
		return amblum[index];
	}

	public Texture2D GetColorByIndex(int index)
	{
		if (0 > index || colorPanel.Length <= index)
		{
			return null;
		}
		return colorPanel[index];
	}

	public Color GetColorValueByIndex(int index)
	{
		if (0 > index || colorPanel.Length <= index)
		{
			return Color.white;
		}
		return colorTable[index];
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
