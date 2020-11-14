using System.Collections.Generic;
using UnityEngine;

public class MyKillLog : MonoBehaviour
{
	private enum ALPHASTEP
	{
		NONE = -1,
		WAIT,
		START
	}

	private Queue<KillInfo> logQ;

	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	private ALPHASTEP alphaStep = ALPHASTEP.NONE;

	private float deltaNext;

	private void Awake()
	{
		logQ = new Queue<KillInfo>();
	}

	private void Start()
	{
	}

	private void OnKillLog(KillInfo log)
	{
		if (logQ != null && !(MyInfoManager.Instance.Nickname == log.Victim) && MyInfoManager.Instance.Nickname == log.Killer && log.WeaponBy != 0)
		{
			logQ.Enqueue(log);
			alphaStep = ALPHASTEP.NONE;
			if (logQ.Count > 1)
			{
				alphaStep = ALPHASTEP.WAIT;
				deltaNext = 0f;
			}
		}
	}

	private void DrawMyKill()
	{
		foreach (KillInfo item in logQ)
		{
			if (!(item.Alpha <= 0f) && !(item.DragY >= 36f))
			{
				Color color = GUI.color;
				if (alphaStep == ALPHASTEP.START || item.IsAlpha)
				{
					item.Alpha -= Time.deltaTime;
					item.DragY += Time.deltaTime * 36f;
					if (item.Alpha < 0f)
					{
						item.Alpha = 0f;
						alphaStep = ALPHASTEP.NONE;
					}
					if (item.DragY > 36f)
					{
						item.DragY = 36f;
						alphaStep = ALPHASTEP.NONE;
					}
					GUI.color = new Color(1f, 1f, 1f, item.Alpha);
				}
				TextureUtil.DrawTexture(new Rect(320f, 290f - item.DragY, 384f, 36f), GlobalVars.Instance.iconEnemyKillBg, ScaleMode.StretchToFill);
				TextureUtil.DrawTexture(new Rect(396f, 259f - item.DragY, (float)GlobalVars.Instance.iconEnemyKill.width, (float)GlobalVars.Instance.iconEnemyKill.height), GlobalVars.Instance.iconEnemyKill, ScaleMode.StretchToFill);
				LabelUtil.PushSize("Label", 20);
				LabelUtil.TextOut(new Vector2(531f, 309f - item.DragY), item.Victim, "Label", GUI.color, GlobalVars.txtEmptyColor, TextAnchor.MiddleLeft);
				LabelUtil.PopSize();
				GUI.color = color;
				break;
			}
		}
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn)
		{
			GUISkin skin = GUI.skin;
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			DrawMyKill();
			GUI.enabled = true;
			GUI.skin = skin;
		}
	}

	private void Update()
	{
		foreach (KillInfo item in logQ)
		{
			item.Update();
		}
		if (alphaStep == ALPHASTEP.WAIT)
		{
			deltaNext += Time.deltaTime;
			if (deltaNext > 1f)
			{
				alphaStep = ALPHASTEP.START;
			}
		}
		while (logQ.Count > 0)
		{
			KillInfo killInfo = logQ.Peek();
			if (killInfo.IsAlive)
			{
				break;
			}
			logQ.Dequeue();
			if (logQ.Count == 1)
			{
				alphaStep = ALPHASTEP.NONE;
			}
		}
	}
}
