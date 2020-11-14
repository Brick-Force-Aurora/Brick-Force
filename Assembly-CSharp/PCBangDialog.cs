using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PCBangDialog : Dialog
{
	public UIImageList imgList;

	public UILabelList labelList;

	public UIMyButton exit;

	public UIScrollView scrollView;

	public UIImage outLine;

	public UIImage iconOutLine;

	public UIImage itemIcon;

	public UILabel itemText;

	public PCBangBenefit[] benefitArray;

	private List<PCBangBenefit> benefitList = new List<PCBangBenefit>();

	public override void Start()
	{
		id = DialogManager.DIALOG_INDEX.PC_BANG_NOTICE;
		scrollView.listBases.Add(outLine);
		scrollView.listBases.Add(iconOutLine);
		scrollView.listBases.Add(itemIcon);
		scrollView.listBases.Add(itemText);
		ResetBenerfitList(null);
	}

	public override void OnPopup()
	{
		rc = new Rect((GlobalVars.Instance.ScreenRect.width - size.x) / 2f, (GlobalVars.Instance.ScreenRect.height - size.y) / 2f, size.x, size.y);
	}

	public void InitDialog()
	{
		PremiumItemManager.Instance.ResetPcbangItems();
	}

	public override bool DoDialog()
	{
		bool result = false;
		GUISkin skin = GUI.skin;
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		imgList.Draw();
		labelList.Draw();
		if (exit.Draw())
		{
			result = true;
		}
		scrollView.SetListCount(benefitList.Count);
		scrollView.BeginScroll();
		for (int i = 0; i < benefitList.Count; i++)
		{
			itemIcon.texImage = benefitList[i].texImage;
			itemText.textKey = benefitList[i].textKey;
			scrollView.SetListPostion(i);
			scrollView.Draw();
		}
		scrollView.EndScroll();
		if (!ContextMenuManager.Instance.IsPopup)
		{
			WindowUtil.EatEvent();
		}
		GUI.skin = skin;
		return result;
	}

	public void ResetBenerfitList(PCBangBenefit[] array)
	{
		benefitList.Clear();
		for (int i = 0; i < benefitArray.Length; i++)
		{
			benefitList.Add(benefitArray[i]);
		}
		if (array != null)
		{
			for (int j = 0; j < array.Length; j++)
			{
				benefitList.Add(array[j]);
			}
		}
	}
}
