using System.Collections.Generic;
using UnityEngine;

public class LineTool : EditorTool
{
	public enum PHASE
	{
		NONE,
		START,
		END
	}

	private Vector3 start = Vector3.zero;

	private Vector3 end = Vector3.zero;

	private byte rotation;

	private Queue<GameObject> line;

	private Queue<GameObject> invisible;

	private GameObject prefab;

	private PHASE phase;

	public int Count => line.Count;

	public PHASE Phase => phase;

	public LineTool(EditorToolScript ets, Item i, GameObject dummy, BattleChat _battleChat)
		: base(ets, i, _battleChat)
	{
		line = new Queue<GameObject>();
		invisible = new Queue<GameObject>();
		prefab = dummy;
		start = new Vector3(10000f, 10000f, 10000f);
		end = new Vector3(10000f, 10000f, 10000f);
	}

	public override void OnClose()
	{
		Reset();
	}

	public void Reset()
	{
		ClearLine();
		phase = PHASE.NONE;
		start = new Vector3(10000f, 10000f, 10000f);
		end = new Vector3(10000f, 10000f, 10000f);
	}

	private GameObject PopDummy(Vector3 pos)
	{
		GameObject gameObject = null;
		if (invisible.Count > 0)
		{
			gameObject = invisible.Peek();
			invisible.Dequeue();
		}
		if (null == gameObject)
		{
			gameObject = (Object.Instantiate((Object)prefab) as GameObject);
		}
		gameObject.transform.position = pos;
		gameObject.GetComponent<MeshRenderer>().enabled = true;
		return gameObject;
	}

	private void PushDummy(GameObject dummy)
	{
		dummy.GetComponent<MeshRenderer>().enabled = false;
		invisible.Enqueue(dummy);
	}

	public override bool IsEnable()
	{
		return item != null && item.EnoughToConsume;
	}

	public override bool Update()
	{
		if (!battleChat.IsChatting && custom_inputs.Instance.GetButtonDown(editorToolScript.inputKey) && IsEnable())
		{
			active = true;
			phase = PHASE.NONE;
			start = new Vector3(10000f, 10000f, 10000f);
			end = new Vector3(10000f, 10000f, 10000f);
			return true;
		}
		if (!battleChat.IsChatting && custom_inputs.Instance.GetButtonDown(editorToolScript.inputKey) && !IsEnable())
		{
			GameObject gameObject = GameObject.Find("Me");
			if (null != gameObject)
			{
				LocalController component = gameObject.GetComponent<LocalController>();
				if (null != component)
				{
					component.addStatusMsg(StringMgr.Instance.Get("ITEM_USED_ALL"));
				}
			}
			return false;
		}
		return false;
	}

	private bool CheckCount(Brick with)
	{
		if (with.maxInstancePerMap < 0)
		{
			return true;
		}
		if (BrickManager.Instance.CountLimitedBrick(with.GetIndex()) < with.maxInstancePerMap)
		{
			return true;
		}
		SystemMsgManager.Instance.ShowMessage("OVER_NUM");
		return false;
	}

	private bool CheckEmpty(Brick with, byte x, byte y, byte z)
	{
		return BrickManager.Instance.IsEmpty2(with, new Vector3((float)(int)x, (float)(int)y, (float)(int)z), brickOnly: false);
	}

	public bool MoveFirst(Brick with, ref byte x, ref byte y, ref byte z, ref byte rot)
	{
		if (line.Count <= 0 || !CheckCount(with) || !BrickManager.Instance.checkAddMinMaxGravity(with.seq) || !UserMapInfoManager.Instance.CheckAuth(showMessage: true))
		{
			Reset();
			return false;
		}
		GameObject gameObject = null;
		while (gameObject == null && line.Count > 0)
		{
			gameObject = line.Dequeue();
			Vector3 position = gameObject.transform.position;
			x = (byte)Mathf.FloorToInt(position.x);
			Vector3 position2 = gameObject.transform.position;
			y = (byte)Mathf.FloorToInt(position2.y);
			Vector3 position3 = gameObject.transform.position;
			z = (byte)Mathf.FloorToInt(position3.z);
			if (!CheckEmpty(with, x, y, z))
			{
				PushDummy(gameObject);
				gameObject = null;
			}
		}
		if (null == gameObject)
		{
			Reset();
			return false;
		}
		rot = rotation;
		PushDummy(gameObject);
		return true;
	}

	public bool MoveNext(Brick with, ref byte x, ref byte y, ref byte z, ref byte rot)
	{
		if (line.Count <= 0 || !CheckCount(with))
		{
			Reset();
			return false;
		}
		GameObject gameObject = null;
		while (gameObject == null && line.Count > 0)
		{
			gameObject = line.Dequeue();
			Vector3 position = gameObject.transform.position;
			x = (byte)Mathf.FloorToInt(position.x);
			Vector3 position2 = gameObject.transform.position;
			y = (byte)Mathf.FloorToInt(position2.y);
			Vector3 position3 = gameObject.transform.position;
			z = (byte)Mathf.FloorToInt(position3.z);
			if (!CheckEmpty(with, x, y, z))
			{
				PushDummy(gameObject);
				gameObject = null;
			}
		}
		if (null == gameObject)
		{
			Reset();
			return false;
		}
		rot = rotation;
		PushDummy(gameObject);
		return true;
	}

	private void ClearLine()
	{
		while (line.Count > 0)
		{
			PushDummy(line.Dequeue());
		}
	}

	public void GoBack()
	{
		switch (phase)
		{
		case PHASE.START:
			ClearLine();
			phase = PHASE.NONE;
			break;
		case PHASE.END:
			ClearLine();
			phase = PHASE.START;
			break;
		}
	}

	public void SetStart(Vector3 point, byte rot)
	{
		ClearLine();
		rotation = rot;
		start = point;
		phase = PHASE.START;
	}

	public void SetPreview(Vector3 point)
	{
		if (end != point)
		{
			ClearLine();
			end = point;
			byte x = 0;
			byte y = 0;
			byte z = 0;
			byte x2 = 0;
			byte y2 = 0;
			byte z2 = 0;
			if (BrickManager.Instance.ToCoord(start, ref x, ref y, ref z) && BrickManager.Instance.ToCoord(end, ref x2, ref y2, ref z2))
			{
				Draw3DLine(x, y, z, x2, y2, z2);
				if (line.Count <= 0)
				{
					Draw3DLine(x2, y2, z2, x, y, z);
					ReverseLine();
				}
			}
		}
	}

	public void SetEnd(Vector3 point)
	{
		ClearLine();
		end = point;
		phase = PHASE.END;
		byte x = 0;
		byte y = 0;
		byte z = 0;
		byte x2 = 0;
		byte y2 = 0;
		byte z2 = 0;
		if (BrickManager.Instance.ToCoord(start, ref x, ref y, ref z) && BrickManager.Instance.ToCoord(end, ref x2, ref y2, ref z2))
		{
			Draw3DLine(x, y, z, x2, y2, z2);
			if (line.Count <= 0)
			{
				Draw3DLine(x2, y2, z2, x, y, z);
				ReverseLine();
			}
		}
	}

	private void ReverseLine()
	{
		Stack<GameObject> stack = new Stack<GameObject>();
		while (line.Count > 0)
		{
			stack.Push(line.Dequeue());
		}
		while (stack.Count > 0)
		{
			line.Enqueue(stack.Pop());
		}
	}

	private void PushPoint(int x, int y, int z)
	{
		GameObject item = PopDummy(new Vector3((float)x, (float)y, (float)z));
		line.Enqueue(item);
	}

	public static void Swap<T>(ref T x, ref T y)
	{
		T val = y;
		y = x;
		x = val;
	}

	private void Draw3DLine(int x0, int y0, int z0, int x1, int y1, int z1)
	{
		bool flag = Mathf.Abs(y1 - y0) > Mathf.Abs(x1 - x0);
		if (flag)
		{
			Swap(ref x0, ref y0);
			Swap(ref x1, ref y1);
		}
		bool flag2 = Mathf.Abs(z1 - z0) > Mathf.Abs(x1 - x0);
		if (flag2)
		{
			Swap(ref x0, ref z0);
			Swap(ref x1, ref z1);
		}
		int num = Mathf.Abs(x1 - x0);
		int num2 = Mathf.Abs(y1 - y0);
		int num3 = Mathf.Abs(z1 - z0);
		int num4 = num / 2;
		int num5 = num / 2;
		int num6 = (x0 <= x1) ? 1 : (-1);
		int num7 = (y0 <= y1) ? 1 : (-1);
		int num8 = (z0 <= z1) ? 1 : (-1);
		int num9 = y0;
		int num10 = z0;
		for (int i = x0; i <= x1; i += num6)
		{
			int x2 = i;
			int y2 = num9;
			int y3 = num10;
			if (flag2)
			{
				Swap(ref x2, ref y3);
			}
			if (flag)
			{
				Swap(ref x2, ref y2);
			}
			PushPoint(x2, y2, y3);
			num4 -= num2;
			num5 -= num3;
			if (num4 < 0)
			{
				num9 += num7;
				num4 += num;
			}
			if (num5 < 0)
			{
				num10 += num8;
				num5 += num;
			}
		}
	}
}
