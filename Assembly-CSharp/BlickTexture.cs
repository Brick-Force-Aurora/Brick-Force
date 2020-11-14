using System;
using UnityEngine;

[Serializable]
public class BlickTexture
{
	private float ux;

	private float uy;

	private bool isActive;

	private float deltaTime;

	private float blickTime = 0.2f;

	private bool view = true;

	private bool viewText;

	public bool IsActive
	{
		get
		{
			return isActive;
		}
		set
		{
			isActive = value;
			if (isActive)
			{
				deltaTime = 0f;
				view = true;
			}
		}
	}

	public bool ViewText
	{
		get
		{
			return viewText;
		}
		set
		{
			viewText = value;
		}
	}

	public void Draw(float ux, float uy, Texture image)
	{
		if (!(image == null) && isActive && view)
		{
			TextureUtil.DrawTexture(new Rect(ux - (float)(image.width / 2), uy, (float)image.width, (float)image.height), image);
		}
	}

	public void DrawReaminText(float ux, float uy, Texture image, float remain)
	{
		if (!(image == null) && isActive && viewText)
		{
			uy -= 15f;
			LabelUtil.TextOut(new Vector2(ux - (float)(image.width / 2), uy), remain.ToString("0.#"), "MiniLabel", Color.white, TextAnchor.UpperLeft);
		}
	}

	public void Update()
	{
		if (isActive)
		{
			deltaTime += Time.deltaTime;
			if (deltaTime > blickTime)
			{
				deltaTime = 0f;
				view = !view;
			}
		}
	}
}
