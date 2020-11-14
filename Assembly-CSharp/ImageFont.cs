using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ImageFont
{
	public Texture2D[] digits;

	public TextAnchor alignment;

	public int ceilNumber = 50;

	public int floorNumber;

	public Color ceilColor = Color.white;

	public Color floorColor = Color.white;

	public float normalScale = 1f;

	private float accel;

	private float scale = float.NegativeInfinity;

	public Texture2D[] Digits
	{
		get
		{
			return digits;
		}
		set
		{
			digits = value;
		}
	}

	public TextAnchor _alignment
	{
		get
		{
			return alignment;
		}
		set
		{
			alignment = value;
		}
	}

	public int CeilNumber
	{
		get
		{
			return ceilNumber;
		}
		set
		{
			ceilNumber = value;
		}
	}

	public int FloorNumber
	{
		get
		{
			return floorNumber;
		}
		set
		{
			floorNumber = value;
		}
	}

	public Color CceilColor
	{
		get
		{
			return ceilColor;
		}
		set
		{
			ceilColor = value;
		}
	}

	public Color FloorColor
	{
		get
		{
			return floorColor;
		}
		set
		{
			floorColor = value;
		}
	}

	public float Scale
	{
		set
		{
			scale = value;
			accel = 0f;
		}
	}

	public void Print(Vector2 pos, int number)
	{
		bool flag = false;
		int num = 1000000;
		if (scale < 0f)
		{
			scale = normalScale;
		}
		scale = Mathf.Lerp(scale, normalScale, accel * Time.deltaTime);
		accel += 10f * Time.deltaTime;
		Vector2 a = Vector2.zero;
		List<Texture2D> list = new List<Texture2D>();
		int num2 = number;
		while (num > 0)
		{
			int num3 = num2 / num;
			num2 %= num;
			if (0 <= num3 && num3 < digits.Length && (num3 > 0 || flag))
			{
				flag = true;
				list.Add(digits[num3]);
				a.x += (float)digits[num3].width;
				if (a.y < (float)digits[num3].height)
				{
					a.y = (float)digits[num3].height;
				}
			}
			num /= 10;
		}
		if (list.Count <= 0)
		{
			list.Add(digits[0]);
			a.x = (float)digits[0].width;
			a.y = (float)digits[0].height;
		}
		a *= scale;
		switch (alignment)
		{
		case TextAnchor.LowerCenter:
			pos.x -= a.x / 2f;
			pos.y -= a.y;
			break;
		case TextAnchor.LowerLeft:
			pos.y -= a.y;
			break;
		case TextAnchor.LowerRight:
			pos.x -= a.x;
			pos.y -= a.y;
			break;
		case TextAnchor.MiddleCenter:
			pos.x -= a.x / 2f;
			pos.y -= a.y / 2f;
			break;
		case TextAnchor.MiddleLeft:
			pos.y -= a.y / 2f;
			break;
		case TextAnchor.MiddleRight:
			pos.x -= a.x;
			pos.y -= a.y / 2f;
			break;
		case TextAnchor.UpperCenter:
			pos.x -= a.x / 2f;
			break;
		case TextAnchor.UpperRight:
			pos.x -= a.x;
			break;
		}
		Color color = GUI.color;
		if (ceilNumber < floorNumber)
		{
			Debug.LogError(" Ceil Number should be bigger than floor number ");
		}
		else if (ceilNumber <= number)
		{
			GUI.color = ceilColor;
		}
		else if (floorNumber >= number)
		{
			GUI.color = floorColor;
		}
		else
		{
			float num4 = (float)number;
			float num5 = (float)floorNumber;
			float num6 = (float)ceilNumber;
			float t = (num4 - num5) / (num6 - num5);
			GUI.color = Color.Lerp(floorColor, ceilColor, t);
		}
		for (int i = 0; i < list.Count; i++)
		{
			float num7 = (float)list[i].width * scale;
			float height = (float)list[i].height * scale;
			TextureUtil.DrawTexture(new Rect(pos.x, pos.y, num7, height), list[i], ScaleMode.StretchToFill);
			pos.x += num7;
		}
		GUI.color = color;
	}
}
