using System;
using UnityEngine;

[Serializable]
public class UIRegMap : UIBase
{
	private RegMap regmap;

	public override bool Draw()
	{
		if (!isDraw)
		{
			return false;
		}
		if (regmap != null && regmap.Thumbnail != null)
		{
			Vector2 showPosition = base.showPosition;
			float x = showPosition.x;
			Vector2 showPosition2 = base.showPosition;
			TextureUtil.DrawTexture(new Rect(x, showPosition2.y, (float)regmap.Thumbnail.width, (float)regmap.Thumbnail.height), regmap.Thumbnail, ScaleMode.StretchToFill);
			DateTime registeredDate = regmap.RegisteredDate;
			if (registeredDate.Year == DateTime.Today.Year && registeredDate.Month == DateTime.Today.Month && registeredDate.Day == DateTime.Today.Day)
			{
				Vector2 showPosition3 = base.showPosition;
				float x2 = showPosition3.x;
				Vector2 showPosition4 = base.showPosition;
				TextureUtil.DrawTexture(new Rect(x2, showPosition4.y, (float)GlobalVars.Instance.iconNewmap.width, (float)GlobalVars.Instance.iconNewmap.height), GlobalVars.Instance.iconNewmap, ScaleMode.StretchToFill);
			}
			else if ((regmap.tagMask & 8) != 0)
			{
				Vector2 showPosition5 = base.showPosition;
				float x3 = showPosition5.x;
				Vector2 showPosition6 = base.showPosition;
				TextureUtil.DrawTexture(new Rect(x3, showPosition6.y, (float)GlobalVars.Instance.iconglory.width, (float)GlobalVars.Instance.iconglory.height), GlobalVars.Instance.iconglory, ScaleMode.StretchToFill);
			}
			else if ((regmap.tagMask & 4) != 0)
			{
				Vector2 showPosition7 = base.showPosition;
				float x4 = showPosition7.x;
				Vector2 showPosition8 = base.showPosition;
				TextureUtil.DrawTexture(new Rect(x4, showPosition8.y, (float)GlobalVars.Instance.iconMedal.width, (float)GlobalVars.Instance.iconMedal.height), GlobalVars.Instance.iconMedal, ScaleMode.StretchToFill);
			}
			else if ((regmap.tagMask & 2) != 0)
			{
				Vector2 showPosition9 = base.showPosition;
				float x5 = showPosition9.x;
				Vector2 showPosition10 = base.showPosition;
				TextureUtil.DrawTexture(new Rect(x5, showPosition10.y, (float)GlobalVars.Instance.icongoldRibbon.width, (float)GlobalVars.Instance.icongoldRibbon.height), GlobalVars.Instance.icongoldRibbon, ScaleMode.StretchToFill);
			}
			if (regmap.IsAbuseMap())
			{
				Vector2 showPosition11 = base.showPosition;
				float num = showPosition11.x + (float)regmap.Thumbnail.width - (float)GlobalVars.Instance.iconDeclare.width;
				float x6 = num;
				Vector2 showPosition12 = base.showPosition;
				TextureUtil.DrawTexture(new Rect(x6, showPosition12.y, (float)GlobalVars.Instance.iconDeclare.width, (float)GlobalVars.Instance.iconDeclare.height), GlobalVars.Instance.iconDeclare, ScaleMode.StretchToFill);
			}
		}
		return false;
	}

	public void SetRegMap(int id)
	{
		regmap = RegMapManager.Instance.Get(id);
	}

	public void SetRegMap(RegMap reg)
	{
		regmap = reg;
	}

	public int GetRegMapId()
	{
		if (regmap != null)
		{
			return regmap.Map;
		}
		return 0;
	}
}
