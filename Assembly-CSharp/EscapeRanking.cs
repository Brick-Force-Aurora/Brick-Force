using System.Collections.Generic;
using UnityEngine;

public class EscapeRanking : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	public UIGroup offset;

	public UIImageSizeChange myRanking;

	public UILabel myCount;

	public Texture2D[] rankImage;

	public UIImageSizeChange[] rankEffect;

	public UIImage[] rankBackground;

	public UIImage[] clanMark;

	public UILabel[] nickName;

	public UILabel[] countLabel;

	private int[] rank;

	private bool[] upRank;

	private int[] rankNext;

	private LocalController localController;

	public Vector2[] effectTime;

	private void Start()
	{
		GameObject gameObject = GameObject.Find("Me");
		if (null == gameObject)
		{
			Debug.LogError("Fail to find Me");
		}
		else
		{
			localController = gameObject.GetComponent<LocalController>();
			if (null == localController)
			{
				Debug.LogError("Fail to get LocalController component for Me");
			}
		}
		rank = new int[16];
		rankNext = new int[16];
		upRank = new bool[16];
		for (int i = 0; i < 16; i++)
		{
			rank[i] = -1;
			rankNext[i] = -1;
			upRank[i] = false;
		}
		for (int j = 0; j < effectTime.Length; j++)
		{
			myRanking.AddStep(effectTime[j].x * myRanking.startSize, effectTime[j].y);
			rankEffect[0].AddStep(effectTime[j].x * rankEffect[0].startSize, effectTime[j].y);
			rankEffect[1].AddStep(effectTime[j].x * rankEffect[1].startSize, effectTime[j].y);
			rankEffect[2].AddStep(effectTime[j].x * rankEffect[2].startSize, effectTime[j].y);
		}
		myRanking.SetEndStep();
		rankEffect[0].SetEndStep();
		rankEffect[1].SetEndStep();
		rankEffect[2].SetEndStep();
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn)
		{
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			offset.BeginGroup();
			myRanking.Draw();
			myCount.SetTextFormat(MyInfoManager.Instance.Kill);
			myCount.Draw();
			int num = BrickManManager.Instance.GetDescCount() + 1;
			if (num > 3)
			{
				num = 3;
			}
			for (int i = 0; i < num; i++)
			{
				rankEffect[i].Draw();
				rankBackground[i].Draw();
				BrickManDesc desc = BrickManManager.Instance.GetDesc(rankNext[i]);
				if (desc != null)
				{
					Vector2 showPosition = clanMark[i].showPosition;
					float x = showPosition.x;
					Vector2 showPosition2 = clanMark[i].showPosition;
					DrawClanMark(new Rect(x, showPosition2.y, clanMark[i].area.x, clanMark[i].area.y), desc.ClanMark);
					nickName[i].SetText(desc.Nickname);
					nickName[i].Draw();
					countLabel[i].SetTextFormat(desc.Kill);
					ref Vector2 position = ref countLabel[i].position;
					float x2 = nickName[i].position.x;
					Vector2 vector = nickName[i].CalcLength();
					position.x = x2 + vector.x + 5f;
					countLabel[i].Draw();
				}
				else if (rankNext[i] == MyInfoManager.Instance.Seq)
				{
					Vector2 showPosition3 = clanMark[i].showPosition;
					float x3 = showPosition3.x;
					Vector2 showPosition4 = clanMark[i].showPosition;
					DrawClanMark(new Rect(x3, showPosition4.y, clanMark[i].area.x, clanMark[i].area.y), MyInfoManager.Instance.ClanMark);
					nickName[i].SetText(MyInfoManager.Instance.Nickname);
					nickName[i].Draw();
					countLabel[i].SetTextFormat(MyInfoManager.Instance.Kill);
					ref Vector2 position2 = ref countLabel[i].position;
					float x4 = nickName[i].position.x;
					Vector2 vector2 = nickName[i].CalcLength();
					position2.x = x4 + vector2.x + 5f;
					countLabel[i].Draw();
				}
			}
			offset.EndGroup();
			GUI.enabled = true;
		}
	}

	private void Update()
	{
		UpdateRanking();
		myRanking.Update();
		for (int i = 0; i < 3; i++)
		{
			rankEffect[i].Update();
		}
	}

	public void UpdateRanking()
	{
		bool flag = false;
		List<KeyValuePair<int, BrickManDesc>> list = BrickManManager.Instance.ToEscapeSortedList();
		int num = 0;
		for (int i = 0; i < 16; i++)
		{
			rank[i] = rankNext[i];
			if (num < list.Count)
			{
				BrickManDesc value = list[num].Value;
				if (!flag && (MyInfoManager.Instance.Kill > value.Kill || (MyInfoManager.Instance.Kill == value.Kill && MyInfoManager.Instance.Score > value.Score) || (MyInfoManager.Instance.Kill == value.Kill && MyInfoManager.Instance.Score == value.Score && MyInfoManager.Instance.Seq > value.Seq)))
				{
					flag = true;
					rankNext[i] = MyInfoManager.Instance.Seq;
				}
				else
				{
					rankNext[i] = value.Seq;
					num++;
				}
			}
			else if (!flag)
			{
				flag = true;
				rankNext[i] = MyInfoManager.Instance.Seq;
			}
			else
			{
				rankNext[i] = -1;
			}
			if (MyInfoManager.Instance.Seq == rankNext[i] && rankNext[i] != rank[i])
			{
				myRanking.texImage = rankImage[i];
			}
		}
		for (int j = 0; j < 16; j++)
		{
			for (int k = 0; k < 16; k++)
			{
				if (rank[j] == rankNext[k])
				{
					if (k > j)
					{
						upRank[j] = true;
					}
					else
					{
						upRank[j] = false;
					}
				}
			}
		}
		if (IsUpRankingBySeq(MyInfoManager.Instance.Seq))
		{
			myRanking.Reset();
		}
		for (int l = 0; l < 3; l++)
		{
			if (IsUpRankingByRanking(l))
			{
				rankEffect[l].Reset();
			}
		}
	}

	public bool IsUpRankingBySeq(int seq)
	{
		for (int i = 0; i < 16; i++)
		{
			if (rankNext[i] == seq)
			{
				return upRank[i];
			}
		}
		return false;
	}

	public bool IsUpRankingByRanking(int ranking)
	{
		return upRank[ranking];
	}

	private void DrawClanMark(Rect rc, int mark)
	{
		if (mark >= 0)
		{
			Texture2D bg = ClanMarkManager.Instance.GetBg(mark);
			Color colorValue = ClanMarkManager.Instance.GetColorValue(mark);
			Texture2D amblum = ClanMarkManager.Instance.GetAmblum(mark);
			if (null != bg)
			{
				TextureUtil.DrawTexture(rc, bg);
			}
			Color color = GUI.color;
			GUI.color = colorValue;
			if (null != amblum)
			{
				TextureUtil.DrawTexture(rc, amblum);
			}
			GUI.color = color;
		}
	}
}
