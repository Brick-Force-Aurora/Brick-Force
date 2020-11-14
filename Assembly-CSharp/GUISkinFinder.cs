using System.Collections.Generic;
using UnityEngine;

public class GUISkinFinder : MonoBehaviour
{
	public GUISkin guiSkin;

	private static string[] builtinGUIStyleNames = new string[20]
	{
		"box",
		"button",
		"toggle",
		"label",
		"textField",
		"textArea",
		"window",
		"horizontalSlider",
		"horizontalSliderThumb",
		"verticalSlider",
		"verticalSliderThumb",
		"horizontalScrollbar",
		"horizontalScrollbarThumb",
		"horizontalScrollbarLeftButton",
		"horizontalScrollbarRightButton",
		"verticalScrollbar",
		"verticalScrollbarThumb",
		"verticalScrollbarUpButton",
		"verticalScrollbarDownButton",
		"scrollView"
	};

	private static GUISkinFinder _instance;

	public static GUISkinFinder Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = (Object.FindObjectOfType(typeof(GUISkinFinder)) as GUISkinFinder);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get GUISkinFinder Instance");
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	public void UpdateFont(Font curFont)
	{
		List<string> list = new List<string>();
		for (int i = 0; i < builtinGUIStyleNames.Length; i++)
		{
			list.Add(builtinGUIStyleNames[i]);
		}
		for (int j = 0; j < guiSkin.customStyles.Length; j++)
		{
			list.Add(guiSkin.customStyles[j].name);
		}
		string[] array = list.ToArray();
		guiSkin.font = curFont;
		for (int k = 0; k < array.Length; k++)
		{
			GUIStyle style = guiSkin.GetStyle(array[k]);
			if (style != null)
			{
				style.font = curFont;
			}
		}
	}

	public GUISkin GetGUISkin()
	{
		return guiSkin;
	}

	public void LanguageChanged()
	{
		LangOptManager.Instance.SetFont();
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			gameObject.SendMessage("OnLanguageChanged", SendMessageOptions.DontRequireReceiver);
		}
	}
}
