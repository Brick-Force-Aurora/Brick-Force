using System;
using UnityEngine;

[Serializable]
public class UIComboBox : UIBase
{
	public Vector2 area;

	public string[] list;

	public Vector2 parentSize;

	public string buttonStyle = "BoxFilterBg";

	public string boxStyle = "BoxFilterCombo";

	public string btnStyleDn = "BtnArrowDn";

	public string btnStyleUp = "BtnArrowUp";

	public UIComboBox dependentComboBox;

	public bool IsStringKey = true;

	private ComboBox cbox;

	private int select;

	private GUIContent selContent;

	private GUIContent[] listContent;

	public int Select => select;

	public override bool Draw()
	{
		if (!isDraw)
		{
			return false;
		}
		if (cbox == null)
		{
			cbox = new ComboBox();
			cbox.Initialize(bImage: false, area);
			cbox.setStyleNames(buttonStyle, btnStyleDn, btnStyleUp, boxStyle);
			cbox.setTextColor(Color.white, GlobalVars.Instance.GetByteColor2FloatColor(205, 100, 36));
			cbox.setBackground(Color.white, GlobalVars.Instance.GetByteColor2FloatColor(0, 53, 92));
			if (parentSize != Vector2.zero)
			{
				cbox.SetParentWindowSize(parentSize);
			}
			ResetList();
		}
		DoCombo();
		return false;
	}

	public bool IsClickedComboButton()
	{
		if (cbox == null)
		{
			return false;
		}
		return cbox.IsClickedComboButton();
	}

	public void ResetList()
	{
		if (list != null && list.Length > 0)
		{
			if (cbox == null)
			{
				cbox = new ComboBox();
				cbox.Initialize(bImage: false, area);
				cbox.setStyleNames(buttonStyle, btnStyleDn, btnStyleUp, boxStyle);
				cbox.setTextColor(Color.white, GlobalVars.Instance.GetByteColor2FloatColor(205, 100, 36));
				cbox.setBackground(Color.white, GlobalVars.Instance.GetByteColor2FloatColor(0, 53, 92));
				if (parentSize != Vector2.zero)
				{
					cbox.SetParentWindowSize(parentSize);
				}
			}
			listContent = new GUIContent[list.Length];
			for (int i = 0; i < listContent.Length; i++)
			{
				if (IsStringKey)
				{
					listContent[i] = new GUIContent(StringMgr.Instance.Get(list[i]));
				}
				else
				{
					listContent[i] = new GUIContent(list[i]);
				}
			}
			select = 0;
			selContent = listContent[select];
			cbox.SetSelectedItemIndex(select);
		}
	}

	public void DoCombo()
	{
		if (list != null && list.Length > 0)
		{
			bool enabled = GUI.enabled;
			if (dependentComboBox != null)
			{
				GUI.enabled = !dependentComboBox.IsClickedComboButton();
			}
			ComboBox comboBox = cbox;
			Vector2 showPosition = base.showPosition;
			float x = showPosition.x;
			Vector2 showPosition2 = base.showPosition;
			select = comboBox.List(new Rect(x, showPosition2.y, area.x, area.y), selContent, listContent);
			if (dependentComboBox != null)
			{
				GUI.enabled = enabled;
			}
			selContent = listContent[select];
		}
	}

	public string GetSelectString()
	{
		return list[select];
	}
}
