using UnityEngine;

public class Banner
{
	private int row;

	private string imagePath;

	private int actionType;

	private string actionParam;

	private Texture2D bnnr;

	private WWW cdn;

	public int Row
	{
		get
		{
			return row;
		}
		set
		{
			row = value;
		}
	}

	public string ImagePath
	{
		get
		{
			return imagePath;
		}
		set
		{
			imagePath = value;
		}
	}

	public int ActionType
	{
		get
		{
			return actionType;
		}
		set
		{
			actionType = value;
		}
	}

	public string ActionParam
	{
		get
		{
			return actionParam;
		}
		set
		{
			actionParam = value;
		}
	}

	public Texture2D Bnnr
	{
		get
		{
			return bnnr;
		}
		set
		{
			bnnr = value;
		}
	}

	public WWW CDN
	{
		get
		{
			return cdn;
		}
		set
		{
			cdn = value;
		}
	}

	public Banner(int _row, string _imagePath, int _actionType, string _actionParam)
	{
		row = _row;
		imagePath = _imagePath;
		actionType = _actionType;
		actionParam = _actionParam;
		bnnr = null;
		cdn = null;
	}
}
