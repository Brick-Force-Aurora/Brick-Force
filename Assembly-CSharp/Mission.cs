using UnityEngine;

public class Mission
{
	private int index;

	private string description;

	private int goal;

	private int atleast;

	private int progress;

	private bool complete;

	public int Index => index;

	public string Description
	{
		get
		{
			string text = string.Format(StringMgr.Instance.Get(description), goal);
			if (atleast > 0)
			{
				text += " ";
				text += string.Format(StringMgr.Instance.Get("ATLEAST_CONDITION"), atleast);
			}
			return text;
		}
	}

	public string MiniDescription => string.Format(StringMgr.Instance.Get(description), goal);

	public string ProgressString
	{
		get
		{
			if (complete)
			{
				return "(" + StringMgr.Instance.Get("MISSION_COMPLETE_STATUS") + ")";
			}
			return "(" + progress.ToString() + " / " + goal.ToString() + ")";
		}
	}

	public float Progress
	{
		get
		{
			if (complete || progress >= goal)
			{
				return 1f;
			}
			return (float)progress / (float)goal;
		}
	}

	public bool CanComplete => progress >= goal && !complete;

	public bool Completed => progress >= goal && complete;

	public Mission(int _index, string _description, int _goal, int _progress, bool _complete, int _atleast)
	{
		index = _index;
		description = _description;
		goal = _goal;
		atleast = _atleast;
		progress = Mathf.Min(_goal, _progress);
		complete = _complete;
	}

	public void Complete()
	{
		complete = true;
	}

	public void SetMission(int _progress, bool _complete)
	{
		progress = Mathf.Min(goal, _progress);
		complete = _complete;
	}

	public void SetProgress(int _progress)
	{
		progress = Mathf.Min(goal, _progress);
	}
}
