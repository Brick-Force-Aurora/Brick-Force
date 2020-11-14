using System;
using UnityEngine;

[Serializable]
public class BungeeTool
{
	private float coolTimeInst = -1f;

	public UIImage itemIcon;

	private ActiveItemData desc;

	public UIChangeColor uiEffect;

	private float deltaTime = 10000f;

	private bool useItem;

	public string CoolTime
	{
		get
		{
			if (desc == null || deltaTime >= coolTimeInst)
			{
				return string.Empty;
			}
			return Mathf.CeilToInt(coolTimeInst - deltaTime).ToString();
		}
	}

	public void Update()
	{
		deltaTime += Time.deltaTime;
		uiEffect.Update();
		if (useItem && UseAble())
		{
			ResetSlot();
		}
	}

	public bool UseAble()
	{
		if (desc == null)
		{
			return false;
		}
		return deltaTime > coolTimeInst;
	}

	public void StartCoolTime()
	{
		deltaTime = 0f;
		useItem = true;
		if (desc.cooltime == -1f)
		{
			ResetSlot();
		}
	}

	public void Use()
	{
		ActiveItemManager.Instance.UseItem(MyInfoManager.Instance.Seq, desc.GetItemType());
		P2PManager.Instance.SendPEER_USE_ACTIVE_ITEM(MyInfoManager.Instance.Seq, desc.GetItemType());
		StartCoolTime();
		uiEffect.Reset();
	}

	public bool AddActiveItem(ActiveItemData item)
	{
		if (desc == null)
		{
			desc = item;
			deltaTime = 10000f;
			useItem = false;
			itemIcon.texImage = item.icon;
			uiEffect.Reset();
			return true;
		}
		return false;
	}

	public ActiveItemData GetActiveItem()
	{
		return desc;
	}

	public void ResetSlot()
	{
		desc = null;
		coolTimeInst = -1f;
		deltaTime = 10000f;
		useItem = false;
		itemIcon.texImage = null;
	}
}
