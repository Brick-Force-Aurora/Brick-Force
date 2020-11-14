using System;
using UnityEngine;

[Serializable]
public class UISpriteMove : UISprite
{
	public Vector2 moveSpeed;

	public float deadTime;

	public override bool Update()
	{
		base.Update();
		position += moveSpeed * Time.deltaTime;
		return true;
	}

	public bool IsTimeOver()
	{
		return currentTime > deadTime;
	}
}
