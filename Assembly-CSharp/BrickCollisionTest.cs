using UnityEngine;

public class BrickCollisionTest : MonoBehaviour
{
	private int seq;

	private int index;

	public int Seq
	{
		set
		{
			seq = value;
		}
	}

	public int Index
	{
		set
		{
			index = value;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		LocalController component = other.gameObject.GetComponent<LocalController>();
		if (component != null)
		{
			component.OnTrampoline(seq);
			BrickManager.Instance.AnimationPlay(index, seq, "fire");
		}
	}
}
