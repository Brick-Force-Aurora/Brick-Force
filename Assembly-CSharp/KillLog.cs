using System.Collections.Generic;
using UnityEngine;

public class KillLog : MonoBehaviour
{
	private Queue<KillInfo> logQ;

	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	public float logHeight = 32f;

	public Vector2 headShotSize = new Vector2(64f, 64f);

	private Color clrKiller = Color.white;

	private Color clrDead = Color.white;

	private void Awake()
	{
		logQ = new Queue<KillInfo>();
	}

	private void Start()
	{
		clrKiller = GlobalVars.Instance.GetByteColor2FloatColor(213, 2, 2);
		clrDead = GlobalVars.Instance.GetByteColor2FloatColor(0, 90, byte.MaxValue);
	}

	private void OnKillLog(KillInfo log)
	{
		if (logQ != null)
		{
			logQ.Enqueue(log);
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
			float num = 256f;
			GUIStyle style = GUI.skin.GetStyle("Label");
			foreach (KillInfo item in logQ)
			{
				float num2 = logHeight / (float)item.WeaponTex.height;
				Vector2 vector = new Vector2((float)item.WeaponTex.width * num2, logHeight);
				Vector2 vector2 = style.CalcSize(new GUIContent(item.Killer));
				Vector2 vector3 = style.CalcSize(new GUIContent(item.Victim));
				float num3 = vector3.x + vector.x + 40f;
				if (item.Killer != item.Victim)
				{
					num3 += vector2.x;
				}
				if (item.HeadShot != null)
				{
					num3 += headShotSize.x;
				}
				GUI.Box(new Rect((float)Screen.width - num3, num, num3, logHeight), string.Empty, "BoxLogBase");
				float num4 = (float)(Screen.width - 15);
				LabelUtil.TextOut(new Vector2(num4, num + logHeight / 2f), item.Victim, "Label", GetColor(item.VictimSequence), Color.black, TextAnchor.MiddleRight);
				num4 -= vector3.x + 5f;
				if (item.HeadShot != null)
				{
					num4 -= headShotSize.x;
					TextureUtil.DrawTexture(new Rect(num4, num - headShotSize.y / 3f, headShotSize.x, headShotSize.y), item.HeadShot, ScaleMode.StretchToFill, alphaBlend: true);
					num4 -= 5f;
				}
				num4 -= vector.x;
				TextureUtil.DrawTexture(new Rect(num4, num, vector.x, vector.y), item.WeaponTex, ScaleMode.StretchToFill, alphaBlend: true);
				if (item.Killer != item.Victim)
				{
					num4 -= 5f;
					LabelUtil.TextOut(new Vector2(num4, num + logHeight / 2f), item.Killer, "Label", GetColor(item.KillerSequence), Color.black, TextAnchor.MiddleRight);
				}
				num += logHeight + 10f;
			}
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
		while (logQ.Count > 0)
		{
			KillInfo killInfo = logQ.Peek();
			if (killInfo.IsAlive)
			{
				break;
			}
			logQ.Dequeue();
		}
	}

	private Color GetColor(int sequence)
	{
		if (RoomManager.Instance.IsVsTeamScene)
		{
			int num = -1;
			if (sequence == MyInfoManager.Instance.Seq)
			{
				num = MyInfoManager.Instance.Slot;
			}
			else
			{
				BrickManDesc desc = BrickManManager.Instance.GetDesc(sequence);
				if (desc != null)
				{
					num = desc.Slot;
				}
			}
			if (num < 0)
			{
				return Color.white;
			}
			if (num < 8)
			{
				return clrKiller;
			}
			return clrDead;
		}
		return Color.white;
	}
}
