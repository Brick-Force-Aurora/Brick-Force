using UnityEngine;

public class ActiveItemBase : MonoBehaviour
{
	protected int useUserSeq;

	public void UseItem(int seq)
	{
		useUserSeq = seq;
		StartItem();
	}

	public virtual void StartItem()
	{
	}

	public bool IsMyItem()
	{
		return MyInfoManager.Instance.Seq == useUserSeq;
	}
}
