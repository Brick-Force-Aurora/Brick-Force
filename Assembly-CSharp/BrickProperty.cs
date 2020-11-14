using UnityEngine;

public class BrickProperty : MonoBehaviour
{
	private int seq = -1;

	private int chunk = -1;

	private byte index = byte.MaxValue;

	private int hitPoint = -1;

	private bool visible_t;

	public int Seq
	{
		get
		{
			return seq;
		}
		set
		{
			seq = value;
		}
	}

	public int Chunk
	{
		get
		{
			return chunk;
		}
		set
		{
			chunk = value;
		}
	}

	public byte Index
	{
		get
		{
			return index;
		}
		set
		{
			index = value;
		}
	}

	public int HitPoint
	{
		get
		{
			return hitPoint;
		}
		set
		{
			hitPoint = value;
		}
	}

	public bool Visible_t
	{
		get
		{
			return visible_t;
		}
		set
		{
			visible_t = value;
		}
	}

	public void Hit(int AtkPow)
	{
		Brick brick = BrickManager.Instance.GetBrick(index);
		if (brick != null && brick.destructible)
		{
			if (brick.function == Brick.FUNCTION.SCRIPT)
			{
				Trigger componentInChildren = GetComponentInChildren<Trigger>();
				if (null == componentInChildren || !componentInChildren.enabled)
				{
					return;
				}
			}
			if (hitPoint > 0)
			{
				hitPoint -= AtkPow;
				if (hitPoint <= 0 && brick.function == Brick.FUNCTION.SCRIPT)
				{
					BroadcastMessage("OnBreak", SendMessageOptions.DontRequireReceiver);
					if (Application.loadedLevelName.Contains("Tutor"))
					{
						BrickManager.Instance.DestroyBrick(seq);
					}
				}
			}
		}
	}
}
