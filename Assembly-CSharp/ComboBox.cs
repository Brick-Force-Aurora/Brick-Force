using System;
using UnityEngine;

[Serializable]
public class ComboBox
{
	private bool forceToUnShow;

	private int useControlID = -1;

	private bool bClickedComboButton;

	private Vector2 scrollViewVector = Vector2.zero;

	private int selectedItemIndex;

	private GUIStyle guiStyle;

	private bool bOpenList;

	private Texture2D texture;

	private int maxList;

	private string buttonStyle = "BoxCombo";

	private string boxStyle = "BoxComboBg";

	private bool haveImage;

	private string btnStyleDn = "BtnComboDn";

	private string btnStyleUp = "BtnComboUp";

	private bool showUpDnButton = true;

	private bool scrollDown = true;

	private bool battleUI;

	private bool isDialog;

	private Vector2 parentWindowSize = Vector2.zero;

	public void Initialize(bool bImage, Vector2 shellSize, bool showUpDn = true, bool _scrollDown = true)
	{
		texture = new Texture2D(2, 2);
		texture.SetPixel(0, 0, Color.gray);
		texture.SetPixel(1, 0, Color.gray);
		texture.SetPixel(0, 1, Color.gray);
		texture.SetPixel(1, 1, Color.gray);
		texture.Apply();
		bOpenList = false;
		forceToUnShow = false;
		useControlID = -1;
		haveImage = bImage;
		showUpDnButton = showUpDn;
		scrollDown = _scrollDown;
		SetGUIStyle(14, (int)shellSize.x, (int)shellSize.y);
	}

	public void setBattleUI(bool set)
	{
		battleUI = set;
	}

	public void setStyleNames(string btnFake, string btnDn, string btnUp, string box)
	{
		buttonStyle = btnFake;
		btnStyleDn = btnDn;
		btnStyleUp = btnUp;
		boxStyle = box;
	}

	public void setBackground(Color nmlclr, Color hvrClr)
	{
		texture = null;
		texture = new Texture2D(2, 2);
		texture.SetPixel(0, 0, nmlclr);
		texture.SetPixel(1, 0, nmlclr);
		texture.SetPixel(0, 1, nmlclr);
		texture.SetPixel(1, 1, nmlclr);
		texture.Apply();
		guiStyle.onHover.background = texture;
		texture.SetPixel(0, 0, hvrClr);
		texture.SetPixel(1, 0, hvrClr);
		texture.SetPixel(0, 1, hvrClr);
		texture.SetPixel(1, 1, hvrClr);
		texture.Apply();
		guiStyle.hover.background = texture;
	}

	public void setTextColor(Color nmlClr, Color hvrClr)
	{
		guiStyle.normal.textColor = nmlClr;
		guiStyle.hover.textColor = hvrClr;
	}

	public void SetGUIStyle(int fontsize, int fixedWidth, int fixedHeight)
	{
		guiStyle = null;
		guiStyle = new GUIStyle();
		guiStyle.fontSize = fontsize;
		guiStyle.fixedWidth = (float)fixedWidth;
		guiStyle.fixedHeight = (float)fixedHeight;
		guiStyle.normal.textColor = Color.white;
		guiStyle.hover.textColor = Color.black;
		guiStyle.onHover.background = texture;
		guiStyle.hover.background = texture;
		guiStyle.padding.left = 6;
		RectOffset padding = guiStyle.padding;
		int num = 2;
		guiStyle.padding.bottom = num;
		padding.top = num;
		if (haveImage)
		{
			guiStyle.imagePosition = ImagePosition.ImageLeft;
		}
	}

	public int List(Rect rect, string buttonText, GUIContent[] listContent)
	{
		return List(rect, new GUIContent(buttonText), listContent, buttonStyle, boxStyle);
	}

	public int List(Rect rect, Texture buttonTex, GUIContent[] listContent)
	{
		return List(rect, new GUIContent(buttonTex), listContent, buttonStyle, boxStyle);
	}

	public int List(Rect rect, GUIContent buttonContent, GUIContent[] listContent)
	{
		return List(rect, buttonContent, listContent, buttonStyle, boxStyle);
	}

	public int List(Rect rect, string buttonText, GUIContent[] listContent, GUIStyle buttonStyle, GUIStyle boxStyle)
	{
		return List(rect, new GUIContent(buttonText), listContent, buttonStyle, boxStyle);
	}

	public int List(Rect rect, Texture buttonTex, GUIContent[] listContent, GUIStyle buttonStyle, GUIStyle boxStyle)
	{
		return List(rect, new GUIContent(buttonTex), listContent, buttonStyle, boxStyle);
	}

	public int List(Rect rect, GUIContent buttonContent, GUIContent[] listContent, GUIStyle buttonStyle, GUIStyle boxStyle)
	{
		if (forceToUnShow)
		{
			if (bClickedComboButton)
			{
				GlobalVars.Instance.PlaySoundButtonClick();
				bOpenList = false;
			}
			forceToUnShow = false;
			bClickedComboButton = false;
			return GetSelectedItemIndex();
		}
		maxList = listContent.Length;
		bool flag = false;
		int controlID = GUIUtility.GetControlID(FocusType.Passive);
		EventType typeForControl = Event.current.GetTypeForControl(controlID);
		if (typeForControl != EventType.MouseUp || bClickedComboButton)
		{
		}
		if (GUI.Button(rect, buttonContent, buttonStyle))
		{
			if (useControlID == -1)
			{
				useControlID = controlID;
				bClickedComboButton = false;
				bOpenList = false;
			}
			if (useControlID != controlID)
			{
				useControlID = controlID;
			}
			bClickedComboButton = !bClickedComboButton;
			if (bClickedComboButton)
			{
				bOpenList = true;
			}
			GlobalVars.Instance.PlaySoundButtonClick();
		}
		if (showUpDnButton)
		{
			GUIStyle style = GUI.skin.GetStyle(btnStyleDn);
			int width = style.normal.background.width;
			int height = style.normal.background.height;
			Rect position = new Rect(rect.x + rect.width - (float)width - 4f, rect.y + rect.height / 2f - (float)(height / 2), (float)width, (float)height);
			if (!bClickedComboButton)
			{
				GUI.Box(position, string.Empty, btnStyleDn);
			}
			else
			{
				GUI.Box(position, string.Empty, btnStyleUp);
			}
		}
		if (bClickedComboButton)
		{
			float num = guiStyle.fixedHeight * (float)listContent.Length;
			Rect position2 = new Rect(rect.x, rect.y + rect.height + 2f, rect.width, num);
			if (!scrollDown)
			{
				position2 = new Rect(rect.x, rect.y - num - 2f, rect.width, num);
			}
			if (scrollDown)
			{
				scrollViewVector = GUI.BeginScrollView(new Rect(rect.x, rect.y + rect.height, rect.width, num + 2f), scrollViewVector, new Rect(rect.x, rect.y + rect.height, rect.width, num), alwaysShowHorizontal: false, alwaysShowVertical: false);
			}
			else
			{
				scrollViewVector = GUI.BeginScrollView(new Rect(rect.x, rect.y - num, rect.width, num + 2f), scrollViewVector, new Rect(rect.x, rect.y - num, rect.width, num), alwaysShowHorizontal: false, alwaysShowVertical: false);
			}
			if (scrollDown)
			{
				GUI.Box(new Rect(rect.x, rect.y, rect.width, num + rect.height + 2f), string.Empty, boxStyle);
			}
			else
			{
				GUI.Box(new Rect(rect.x, rect.y - num, rect.width, num + rect.height + 2f), string.Empty, boxStyle);
			}
			int num2 = GUI.SelectionGrid(position2, selectedItemIndex, listContent, 1, guiStyle);
			if (num2 != selectedItemIndex)
			{
				selectedItemIndex = num2;
				flag = true;
				bOpenList = false;
			}
			GUI.EndScrollView();
			if (Input.GetMouseButtonDown(0))
			{
				Vector2 point = MouseUtil.ScreenToPixelPoint(Input.mousePosition);
				if (!battleUI)
				{
					point.x *= GlobalVars.Instance.ScreenRect.width / (float)Screen.width;
					point.y *= GlobalVars.Instance.ScreenRect.height / (float)Screen.height;
					if (isDialog)
					{
						point.x -= (GlobalVars.Instance.ScreenRect.width - parentWindowSize.x) / 2f;
						point.y -= (GlobalVars.Instance.ScreenRect.height - parentWindowSize.y) / 2f;
					}
					else
					{
						point.x -= (GlobalVars.Instance.ScreenRect.width - GlobalVars.Instance.UIScreenRect.width) / 2f;
						point.y -= (GlobalVars.Instance.ScreenRect.height - GlobalVars.Instance.UIScreenRect.height) / 2f;
					}
				}
				if (!position2.Contains(point))
				{
					ForceUnShow();
				}
			}
		}
		if (flag)
		{
			bClickedComboButton = false;
		}
		return GetSelectedItemIndex();
	}

	public void SetParentWindowSize(Vector2 size)
	{
		isDialog = true;
		parentWindowSize = size;
	}

	public void ForceUnShow()
	{
		forceToUnShow = true;
	}

	public bool IsClickedComboButton()
	{
		return bOpenList;
	}

	public void SetSelectedItemIndex(int n)
	{
		selectedItemIndex = n;
	}

	public int GetSelectedItemIndex()
	{
		return selectedItemIndex;
	}

	public void NextSelectItemIndex()
	{
		selectedItemIndex++;
		if (selectedItemIndex >= maxList)
		{
			selectedItemIndex = 0;
		}
	}

	public int GetNextSelectItemIndex()
	{
		selectedItemIndex++;
		if (selectedItemIndex >= maxList)
		{
			selectedItemIndex = 0;
		}
		return selectedItemIndex;
	}
}
