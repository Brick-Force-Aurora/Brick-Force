using UnityEngine;

public class MissionLog : MonoBehaviour
{
	private string progress = string.Empty;

	private string title = string.Empty;

	private string sub = string.Empty;

	private string text = string.Empty;

	private int fmtType = -1;

	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	public Texture missionBg;

	public Texture2D[] wpnicons;

	private int iconidx = -1;

	private float titley;

	private float titleh;

	private float suby;

	private float subh;

	private bool drawWeaponIcon;

	private string[] wpnNames = new string[6]
	{
		"TUTO_HELP_ITEM03",
		"TUTO_HELP_ITEM01",
		"TUTO_HELP_ITEM05",
		"TUTO_HELP_ITEM07",
		"TUTO_HELP_ITEM09",
		"TUTO_HELP_ITEM11"
	};

	private string[] wpnEXpls = new string[6]
	{
		"TUTO_HELP_ITEM04",
		"TUTO_HELP_ITEM02",
		"TUTO_HELP_ITEM06",
		"TUTO_HELP_ITEM08",
		"TUTO_HELP_ITEM10",
		"TUTO_HELP_ITEM12"
	};

	public void SetMission(string _p, string _t, string _s, string tag, string fmt)
	{
		progress = _p;
		title = _t;
		sub = _s;
		text = tag;
		drawWeaponIcon = false;
		if (fmt.Contains("Help001"))
		{
			fmtType = 0;
		}
		else if (fmt.Contains("Help003"))
		{
			fmtType = 1;
		}
		else if (fmt.Contains("Help007"))
		{
			fmtType = 2;
		}
		else if (fmt.Contains("Help008"))
		{
			fmtType = 3;
		}
		else if (fmt.Contains("Help009"))
		{
			fmtType = 4;
		}
		else if (fmt.Contains("Help011"))
		{
			fmtType = 4;
		}
		else if (fmt.Contains("Help015"))
		{
			fmtType = 5;
		}
		else if (fmt.Contains("Help019"))
		{
			fmtType = 6;
		}
	}

	public void needPicture()
	{
		drawWeaponIcon = true;
		iconidx++;
	}

	private void Start()
	{
		text = string.Empty;
		iconidx = -1;
	}

	private float calcTitleBoxHeight()
	{
		float num = titley = 15f;
		GUIStyle style = GUI.skin.GetStyle("MissionTitleLabel");
		num += (titleh = style.CalcHeight(new GUIContent(title), 200f));
		num = (suby = num + 5f);
		GUIStyle style2 = GUI.skin.GetStyle("MissionSubTitleLabel");
		num += (subh = style2.CalcHeight(new GUIContent(sub), 220f));
		return num + 15f;
	}

	private void checkStringFormat()
	{
		if (fmtType >= 0)
		{
			switch (fmtType)
			{
			case 0:
			{
				int num = custom_inputs.Instance.KeyIndex("K_FORWARD");
				int num2 = custom_inputs.Instance.KeyIndex("K_BACKWARD");
				int num3 = custom_inputs.Instance.KeyIndex("K_RIGHT");
				int num4 = custom_inputs.Instance.KeyIndex("K_LEFT");
				text = string.Format(text, custom_inputs.Instance.InputKey[num].ToString(), custom_inputs.Instance.InputKey[num2].ToString(), custom_inputs.Instance.InputKey[num3].ToString(), custom_inputs.Instance.InputKey[num4].ToString());
				break;
			}
			case 1:
			{
				int num = custom_inputs.Instance.KeyIndex("K_JUMP");
				int num2 = custom_inputs.Instance.KeyIndex("K_SIT");
				text = string.Format(text, custom_inputs.Instance.InputKey[num].ToString(), custom_inputs.Instance.InputKey[num2].ToString());
				break;
			}
			case 2:
			{
				int num = custom_inputs.Instance.KeyIndex("K_FORWARD");
				int num2 = custom_inputs.Instance.KeyIndex("K_BACKWARD");
				int num3 = custom_inputs.Instance.KeyIndex("K_RIGHT");
				int num4 = custom_inputs.Instance.KeyIndex("K_LEFT");
				int num5 = custom_inputs.Instance.KeyIndex("K_JUMP");
				int num6 = custom_inputs.Instance.KeyIndex("K_SIT");
				text = string.Format(text, custom_inputs.Instance.InputKey[num].ToString(), custom_inputs.Instance.InputKey[num2].ToString(), custom_inputs.Instance.InputKey[num3].ToString(), custom_inputs.Instance.InputKey[num4].ToString(), custom_inputs.Instance.InputKey[num5].ToString(), custom_inputs.Instance.InputKey[num6].ToString());
				break;
			}
			case 3:
			{
				int num = custom_inputs.Instance.KeyIndex("K_FORWARD");
				int num2 = custom_inputs.Instance.KeyIndex("K_BACKWARD");
				text = string.Format(text, custom_inputs.Instance.InputKey[num].ToString(), custom_inputs.Instance.InputKey[num2].ToString());
				break;
			}
			case 4:
			{
				int num = custom_inputs.Instance.KeyIndex("K_RELOAD");
				text = string.Format(text, custom_inputs.Instance.InputKey[num].ToString());
				break;
			}
			case 5:
			{
				int num = custom_inputs.Instance.KeyIndex("K_ACTION");
				int num2 = custom_inputs.Instance.KeyIndex("K_JUMP");
				text = string.Format(text, custom_inputs.Instance.InputKey[num].ToString(), custom_inputs.Instance.InputKey[num2].ToString());
				break;
			}
			case 6:
			{
				int num = custom_inputs.Instance.KeyIndex("K_BRICK_MENU");
				text = string.Format(text, custom_inputs.Instance.InputKey[num].ToString());
				break;
			}
			}
		}
	}

	private void OnGUI()
	{
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		GUI.depth = (int)guiDepth;
		GUI.enabled = !DialogManager.Instance.IsModal;
		if (text.Length > 0)
		{
			float num = calcTitleBoxHeight();
			Color byteColor2FloatColor = GlobalVars.Instance.GetByteColor2FloatColor(139, 222, 32);
			Rect position = new Rect(0f, 217f, 254f, 26f);
			GUI.Box(position, string.Empty, "BoxTuto2");
			LabelUtil.TextOut(new Vector2(254f, 221f), progress, "MissionLabel", byteColor2FloatColor, GlobalVars.txtEmptyColor, TextAnchor.UpperRight);
			Rect position2 = new Rect(0f, 245f, 254f, num);
			GUI.Box(position2, string.Empty, "BoxTuto2");
			GUI.Box(new Rect(5f, position2.y - 20f, 34f, 78f), string.Empty, "TutoPoint");
			Color color = GUI.color;
			GUI.color = GlobalVars.Instance.GetByteColor2FloatColor(139, 222, 32);
			GUI.Label(new Rect(position2.x + 40f, position2.y + titley, 200f, titleh), title, "MissionTitleLabel");
			GUI.color = color;
			GUI.Label(new Rect(position2.x + 40f, position2.y + suby, 200f, subh), sub, "MissionSubTitleLabel");
			if (drawWeaponIcon)
			{
				Rect position3 = new Rect(0f, 245f + num + 2f, 254f, 220f);
				GUI.Box(position3, string.Empty, "BoxTuto2");
				TextureUtil.DrawTexture(new Rect(30f, position3.y, (float)wpnicons[iconidx].width, (float)wpnicons[iconidx].height), wpnicons[iconidx]);
				GUI.color = GlobalVars.Instance.txtMainColor;
				GUI.Label(new Rect(5f, position3.y + 105f, 200f, 25f), StringMgr.Instance.Get(wpnNames[iconidx]), "MissionTitleLabel");
				GUI.color = color;
				GUI.Label(new Rect(5f, position3.y + 120f, 200f, 100f), StringMgr.Instance.Get(wpnEXpls[iconidx]), "MissionSubTitleLabel");
			}
		}
		GUI.enabled = true;
	}
}
