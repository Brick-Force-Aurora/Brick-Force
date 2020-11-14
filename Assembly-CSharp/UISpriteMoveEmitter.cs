using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UISpriteMoveEmitter : UIBase
{
	public float deadTime;

	public Rect totalArea;

	public Rect createArea;

	public Vector2 moveSpeed;

	public Vector2 scaleScope = new Vector2(1f, 1f);

	public float emitTime;

	private float curTime;

	public UISpriteMove sampleSprite;

	private List<UISpriteMove> listSprite = new List<UISpriteMove>();

	public override bool Update()
	{
		curTime += Time.deltaTime;
		if (curTime > emitTime)
		{
			CreateParticle();
			curTime -= emitTime;
		}
		int num = 0;
		while (num < listSprite.Count)
		{
			listSprite[num].Update();
			if (listSprite[num].IsTimeOver())
			{
				listSprite.RemoveAt(num);
			}
			else
			{
				num++;
			}
		}
		return true;
	}

	public override bool Draw()
	{
		if (!isDraw)
		{
			return false;
		}
		GUI.BeginGroup(totalArea);
		for (int i = 0; i < listSprite.Count; i++)
		{
			listSprite[i].Draw();
		}
		GUI.EndGroup();
		return false;
	}

	private void CreateParticle()
	{
		UISpriteMove uISpriteMove = new UISpriteMove();
		uISpriteMove.texImage = sampleSprite.texImage;
		uISpriteMove.changeTime = sampleSprite.changeTime;
		uISpriteMove.playOnce = sampleSprite.playOnce;
		float num = UnityEngine.Random.Range(scaleScope.x, scaleScope.y);
		uISpriteMove.moveSpeed = moveSpeed * num;
		uISpriteMove.deadTime = deadTime / num;
		uISpriteMove.area = new Vector2((float)sampleSprite.texImage[0].width * num, (float)sampleSprite.texImage[0].height * num);
		uISpriteMove.position = new Vector2(UnityEngine.Random.Range(createArea.x, createArea.x + createArea.width - uISpriteMove.area.x), UnityEngine.Random.Range(createArea.y, createArea.y + createArea.height - uISpriteMove.area.y));
		listSprite.Add(uISpriteMove);
	}

	private void Clear()
	{
		listSprite.Clear();
	}
}
