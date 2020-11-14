using UnityEngine;

public class RadioSenderMark
{
	public int Seq;

	public float deltaTime;

	public RadioSenderMark(int seq)
	{
		Seq = seq;
		deltaTime = 0f;
	}

	public void Reset()
	{
		deltaTime = 0f;
	}

	public void Update()
	{
		deltaTime += Time.deltaTime;
	}

	public float GetSignalStrength()
	{
		if (deltaTime > 3f)
		{
			return float.NegativeInfinity;
		}
		return (3f - deltaTime) / 3f;
	}
}
