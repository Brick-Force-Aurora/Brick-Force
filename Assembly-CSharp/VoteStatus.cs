using UnityEngine;

public class VoteStatus
{
	public int yes;

	public int no;

	public int total;

	public int reason;

	public int target;

	public string targetNickname;

	public bool isVoteAble;

	public bool isVoted;

	public int remainTime;

	public float makeTime;

	public string GetVoteReason()
	{
		bool flag = true;
		string text = string.Empty;
		if (IsReason(KICKOUT_VOTE.WHY_BAD_WORD))
		{
			if (!flag)
			{
				text += ", ";
			}
			text += StringMgr.Instance.Get("KICK_REASON01");
			flag = false;
		}
		if (IsReason(KICKOUT_VOTE.WHY_BAD_MANNER))
		{
			if (!flag)
			{
				text += ", ";
			}
			text += StringMgr.Instance.Get("KICK_REASON03");
			flag = false;
		}
		if (IsReason(KICKOUT_VOTE.WHY_HACK))
		{
			if (!flag)
			{
				text += ", ";
			}
			text += StringMgr.Instance.Get("KICK_REASON02");
			flag = false;
		}
		if (IsReason(KICKOUT_VOTE.WHY_ETC))
		{
			if (!flag)
			{
				text += ", ";
			}
			text += StringMgr.Instance.Get("KICK_REASON05");
			flag = false;
		}
		return text;
	}

	private bool IsReason(KICKOUT_VOTE voteReason)
	{
		return (reason & (int)voteReason) != 0;
	}

	public void SetMakeTime()
	{
		makeTime = Time.time;
	}

	public int GetRemainTime()
	{
		return (remainTime - (int)((Time.time - makeTime) * 1000f)) / 1000;
	}
}
