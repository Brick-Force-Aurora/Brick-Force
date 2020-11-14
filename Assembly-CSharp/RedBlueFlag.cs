using UnityEngine;

public class RedBlueFlag : MonoBehaviour
{
	public Material redFlag;

	public Material blueFlag;

	private SkinnedMeshRenderer smr;

	private void SetTeam(bool red)
	{
		if (!(null == smr))
		{
			if (red)
			{
				if (smr.material != redFlag)
				{
					smr.material = redFlag;
				}
			}
			else if (smr.material != blueFlag)
			{
				smr.material = blueFlag;
			}
		}
	}

	private void Start()
	{
		smr = GetComponentInChildren<SkinnedMeshRenderer>();
		if (null == smr)
		{
			Debug.LogError("Fail to get skinned mesh renderer for flags");
		}
		int num = 8;
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.MISSION)
		{
			num = 4;
		}
		SetTeam(MyInfoManager.Instance.Slot < num);
	}

	private void OnChangeTeam()
	{
		int num = 8;
		if (RoomManager.Instance.CurrentRoomType == Room.ROOM_TYPE.MISSION)
		{
			num = 4;
		}
		SetTeam(MyInfoManager.Instance.Slot < num);
	}
}
