using System;
using UnityEngine;

[Serializable]
public class ConsumableDesc
{
	public string name;

	public Texture2D enable;

	public Texture2D disable;

	public float cooltime = -1f;

	public bool passive;

	public AudioClip actionClip;

	public AudioClip errorClip;

	public bool isShooterTool;

	public Room.ROOM_TYPE[] disableByRoomType;

	public bool IsDisableRoom
	{
		get
		{
			for (int i = 0; i < disableByRoomType.Length; i++)
			{
				if (RoomManager.Instance.CurrentRoomType == disableByRoomType[i])
				{
					return true;
				}
			}
			return false;
		}
	}
}
