using System;
using UnityEngine;

[Serializable]
public class TooltipBrick : Dialog
{
	private Brick brick;

	public string[] category;

	public Vector2 crdName = new Vector2(10f, 7f);

	public Vector2 crdCategory = new Vector2(10f, 46f);

	public Vector2 crdMax = new Vector2(10f, 64f);

	public Rect crdComment = new Rect(5f, 90f, 230f, 210f);

	public Brick TargetBrick
	{
		get
		{
			return brick;
		}
		set
		{
			brick = value;
		}
	}

	private void DoName()
	{
		LabelUtil.TextOut(crdName, StringMgr.Instance.Get(brick.brickAlias), "Label", new Color(0.87f, 0.63f, 0.32f), GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
	}

	private void DoCategory()
	{
		LabelUtil.TextOut(crdCategory, StringMgr.Instance.Get(category[(int)brick.category]), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
	}

	private void DoMax()
	{
		if (brick.maxInstancePerMap < 0)
		{
			LabelUtil.TextOut(crdMax, StringMgr.Instance.Get("NO_BRICK_LIMIT"), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
		else
		{
			LabelUtil.TextOut(crdMax, "max. " + brick.maxInstancePerMap.ToString(), "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
		}
	}

	private void DoComment()
	{
		Vector2 vector = new Vector2(crdComment.width, crdComment.height);
		GUIStyle style = GUI.skin.GetStyle("Label");
		if (style != null)
		{
			vector = new Vector2(crdComment.width, style.CalcHeight(new GUIContent(StringMgr.Instance.Get(brick.brickComment)), crdComment.width));
		}
		GUI.Label(new Rect(crdComment.x, crdComment.y, vector.x, vector.y), StringMgr.Instance.Get(brick.brickComment));
	}

	public override bool DoDialog()
	{
		if (brick != null)
		{
			GUISkin skin = GUI.skin;
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			DoName();
			DoCategory();
			DoMax();
			DoComment();
			GUI.skin = skin;
		}
		return false;
	}

	public override void Start()
	{
	}

	public void SetCoord(Vector2 pos)
	{
		if (brick != null)
		{
			Vector2 vector = new Vector2(crdComment.width, crdComment.height);
			GUIStyle style = GUI.skin.GetStyle("Label");
			if (style != null)
			{
				vector = new Vector2(crdComment.width, style.CalcHeight(new GUIContent(StringMgr.Instance.Get(brick.brickComment)), crdComment.width));
			}
			size.y = crdComment.y + vector.y + 20f;
			float num = pos.x + size.x;
			if (num > (float)Screen.width)
			{
				pos.x -= size.x + 64f;
			}
			float num2 = pos.y + size.y;
			if (num2 > (float)Screen.height)
			{
				pos.y -= num2 - (float)Screen.height;
			}
			base.ClientRect = new Rect(pos.x, pos.y, size.x, size.y);
		}
	}
}
