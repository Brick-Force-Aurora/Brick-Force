using System;
using UnityEngine;

[Serializable]
public class RouletteEffect : UIGroup
{
	public UIImageList imgList;

	public UILabelList labelList;

	public UIScrollView itemScroll;

	public UIImage itemBackNomal;

	public UIImage itemBackRare;

	public UIImage itemIcon;

	public UILabel itemTime;

	public UIImage darkEffect;

	public UISprite selectBox;

	public UIChangeColor changeColor;

	public AudioClip sndStart;

	public AudioClip sndCheck;

	public AudioClip sndItemGet;

	public AudioClip sndItemRare;

	public float rarePercent = 25f;

	public float rareAdd = 10f;

	public int itemFixCount = 100;

	public int itemAddCount = 5;

	public float startSpeed = 100f;

	public float minusSpeed = 10f;

	public float minSpeed = 2f;

	private TcTItem[] tcTItemList;

	private TcStatus tcStatus;

	private long seq;

	private int index;

	private string code;

	private int amount;

	private bool wasKey;

	private float curSpeed;

	private float destPosition;

	private bool isProgress;

	private int curIndex;

	public void start()
	{
		itemScroll.listBases.Add(itemBackNomal);
		itemScroll.listBases.Add(itemBackRare);
		itemScroll.listBases.Add(itemIcon);
		itemScroll.listBases.Add(itemTime);
	}

	public override bool Update()
	{
		if (!isDraw)
		{
			return true;
		}
		selectBox.Update();
		changeColor.Update();
		if (!changeColor.IsDraw && isProgress)
		{
			itemScroll.scrollPoint.x += curSpeed * Time.deltaTime;
			if (curSpeed > minSpeed)
			{
				curSpeed -= minusSpeed * Time.deltaTime;
			}
			else
			{
				curSpeed = minSpeed;
			}
			if (itemScroll.scrollPoint.x > destPosition)
			{
				itemScroll.scrollPoint.x = destPosition;
				isProgress = false;
				AutoFunctionManager.Instance.AddEndFunction(1f, ShowResultWindow);
			}
			else
			{
				int num = (int)(itemScroll.scrollPoint.x / itemScroll.offSetX);
				if (curIndex != num)
				{
					curIndex = num;
					GlobalVars.Instance.PlayOneShot(sndCheck);
				}
			}
		}
		return true;
	}

	public override bool Draw()
	{
		if (!isDraw)
		{
			return false;
		}
		BeginGroup();
		base.Draw();
		itemScroll.ListResetAddPosition();
		itemScroll.SetListCount(tcTItemList.Length);
		Rect viewRect = new Rect(0f, 0f, itemScroll.offSetX * (float)tcTItemList.Length, itemScroll.area.y - 25f);
		Vector2 showPosition = itemScroll.showPosition;
		float x = showPosition.x;
		Vector2 showPosition2 = itemScroll.showPosition;
		GUI.BeginScrollView(new Rect(x, showPosition2.y, itemScroll.area.x, itemScroll.area.y), itemScroll.scrollPoint, viewRect);
		for (int i = 0; i < tcTItemList.Length; i++)
		{
			if (tcTItemList[i].isKey)
			{
				itemBackNomal.IsDraw = false;
				itemBackRare.IsDraw = true;
			}
			else
			{
				itemBackNomal.IsDraw = true;
				itemBackRare.IsDraw = false;
			}
			TItem tItem = TItemManager.Instance.Get<TItem>(tcTItemList[i].code);
			if (tItem == null)
			{
				Debug.LogError("Fail to get TItem for " + tcTItemList[i].code);
			}
			else
			{
				itemIcon.texImage = tItem.CurIcon();
				if (tItem.IsAmount)
				{
					itemTime.SetText(tcTItemList[i].opt.ToString() + " " + StringMgr.Instance.Get("TIMES_UNIT"));
				}
				else if (tcTItemList[i].opt >= 1000000)
				{
					itemTime.SetText(StringMgr.Instance.Get("INFINITE"));
				}
				else
				{
					itemTime.SetText(tcTItemList[i].opt.ToString() + " " + StringMgr.Instance.Get("DAYS"));
				}
			}
			itemScroll.SetListPostion(i);
			itemScroll.Draw();
		}
		GUI.EndScrollView();
		imgList.Draw();
		labelList.Draw();
		darkEffect.Draw();
		EndGroup();
		selectBox.Draw();
		changeColor.Draw();
		return false;
	}

	public void InitDialog(long _seq, int _index, string _code, int _amount, bool _wasKey, TcStatus _tcStatus)
	{
		TItem tItem = TItemManager.Instance.Get<TItem>(_code);
		if (tItem == null)
		{
			Debug.LogError("Fail to get TItem for " + _code);
		}
		else
		{
			seq = _seq;
			index = _index;
			code = _code;
			amount = _amount;
			wasKey = _wasKey;
			tcStatus = _tcStatus;
			isDraw = true;
			changeColor.Reset();
			itemScroll.scrollPoint = Vector2.zero;
			curSpeed = startSpeed;
			isProgress = true;
			curIndex = 0;
			InitItemList();
			GlobalVars.Instance.PlayOneShot(sndStart);
		}
	}

	private void InitItemList()
	{
		int num = itemFixCount + UnityEngine.Random.Range(0, itemAddCount);
		TcTItem[] rareArray = tcStatus.GetRareArray();
		TcTItem[] normalArray = tcStatus.GetNormalArray();
		float num2 = rarePercent + UnityEngine.Random.Range(0f, rareAdd);
		if (normalArray.Length == 0)
		{
			num2 = 100f;
		}
		else if (rareArray.Length == 0)
		{
			num2 = 0f;
		}
		tcTItemList = new TcTItem[num];
		for (int i = 0; i < tcTItemList.Length; i++)
		{
			TcTItem tcTItem = (!(num2 < UnityEngine.Random.Range(0f, 100f))) ? rareArray[UnityEngine.Random.Range(0, rareArray.Length)] : normalArray[UnityEngine.Random.Range(0, normalArray.Length)];
			tcTItemList[i] = tcTItem;
		}
		if (tcTItemList.Length > 3)
		{
			tcTItemList[tcTItemList.Length - 3].code = code;
			tcTItemList[tcTItemList.Length - 3].isKey = wasKey;
			tcTItemList[tcTItemList.Length - 3].opt = amount;
		}
		destPosition = (float)(tcTItemList.Length - 4) * itemScroll.offSetX;
	}

	private bool ShowResultWindow()
	{
		TCResultItemDialog tCResultItemDialog = (TCResultItemDialog)DialogManager.Instance.Popup(DialogManager.DIALOG_INDEX.TCRESULT, exclusive: false);
		if (tCResultItemDialog != null)
		{
			tCResultItemDialog.InitDialog(seq, index, code, amount, wasKey);
			if (wasKey)
			{
				GlobalVars.Instance.PlayOneShot(sndItemRare);
			}
			else
			{
				GlobalVars.Instance.PlayOneShot(sndItemGet);
			}
		}
		isDraw = false;
		return true;
	}
}
