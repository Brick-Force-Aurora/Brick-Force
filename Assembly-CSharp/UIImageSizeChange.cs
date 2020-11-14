using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UIImageSizeChange : UIBase
{
	public Texture2D texImage;

	public float startSize;

	public bool repeat;

	private float currentTime;

	private List<SizeChangeStep> sizeChange = new List<SizeChangeStep>();

	private float curSize;

	private int curStep;

	public override bool Update()
	{
		if (!isDraw)
		{
			return false;
		}
		if (texImage == null)
		{
			return false;
		}
		if (curStep >= sizeChange.Count)
		{
			texImage = null;
			return false;
		}
		if (sizeChange[curStep].endSize == curSize && currentTime > sizeChange[curStep].stepTime)
		{
			curStep++;
			currentTime = 0f;
			if (curStep >= sizeChange.Count)
			{
				if (!repeat)
				{
					texImage = null;
					return false;
				}
				curStep = 0;
			}
		}
		currentTime += Time.deltaTime;
		curSize += sizeChange[curStep].speed * Time.deltaTime;
		if (sizeChange[curStep].speed > 0f && sizeChange[curStep].endSize < curSize)
		{
			curSize = sizeChange[curStep].endSize;
		}
		else if (sizeChange[curStep].speed < 0f && sizeChange[curStep].endSize > curSize)
		{
			curSize = sizeChange[curStep].endSize;
		}
		return true;
	}

	public override bool Draw()
	{
		if (texImage != null)
		{
			float num = (float)texImage.width * curSize;
			float num2 = (float)texImage.height * curSize;
			Vector2 showPosition = base.showPosition;
			float x = showPosition.x - num * 0.5f;
			Vector2 showPosition2 = base.showPosition;
			TextureUtil.DrawTexture(new Rect(x, showPosition2.y - num2 * 0.5f, num, num2), texImage);
		}
		return false;
	}

	public void AddStep(float size, float time)
	{
		if (!(time <= 0f))
		{
			float endSize = startSize;
			if (sizeChange.Count > 0)
			{
				endSize = sizeChange[sizeChange.Count - 1].endSize;
			}
			float speed = (size - endSize) / time;
			SizeChangeStep sizeChangeStep = new SizeChangeStep();
			sizeChangeStep.startSize = endSize;
			sizeChangeStep.endSize = size;
			sizeChangeStep.stepTime = time;
			sizeChangeStep.speed = speed;
			sizeChange.Add(sizeChangeStep);
		}
	}

	public void Reset()
	{
		currentTime = 0f;
		curSize = 0f;
		curStep = 0;
		curSize = startSize;
	}

	public void SetEndStep()
	{
		if (0 < sizeChange.Count)
		{
			curStep = sizeChange.Count - 1;
			currentTime = 0f;
			curSize = sizeChange[curStep].endSize;
		}
	}
}
