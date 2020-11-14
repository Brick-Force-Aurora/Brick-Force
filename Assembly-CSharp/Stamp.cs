using UnityEngine;

internal class Stamp
{
	private float deltaTime;

	private MissionDialog.STAMP_STEP step;

	public MissionDialog.STAMP_STEP Step => step;

	public bool IsDoing => step != MissionDialog.STAMP_STEP.STAMP_NONE;

	public Stamp(MissionDialog.STAMP_STEP stampStep)
	{
		step = stampStep;
		deltaTime = 0f;
	}

	public bool Update(AudioClip kwang)
	{
		if (step == MissionDialog.STAMP_STEP.STAMP_NONE)
		{
			return false;
		}
		deltaTime += Time.deltaTime;
		switch (step)
		{
		case MissionDialog.STAMP_STEP.STAMP_BEGIN:
			step = MissionDialog.STAMP_STEP.STAMP_OUT;
			break;
		case MissionDialog.STAMP_STEP.STAMP_OUT:
			if (deltaTime > 0.3f)
			{
				deltaTime = 0f;
				step = MissionDialog.STAMP_STEP.STAMP_IN;
			}
			break;
		case MissionDialog.STAMP_STEP.STAMP_IN:
			if (deltaTime > 0.2f)
			{
				GlobalVars.Instance.PlayOneShot(kwang);
				deltaTime = 0f;
				step = MissionDialog.STAMP_STEP.STAMP_WAIT;
			}
			break;
		case MissionDialog.STAMP_STEP.STAMP_WAIT:
			if (deltaTime > 0.2f)
			{
				deltaTime = 0f;
				step = MissionDialog.STAMP_STEP.STAMP_NONE;
			}
			break;
		}
		return true;
	}

	public float LerpSize()
	{
		float result = 0.9f;
		MissionDialog.STAMP_STEP sTAMP_STEP = step;
		if (sTAMP_STEP == MissionDialog.STAMP_STEP.STAMP_OUT)
		{
			result = Mathf.Lerp(1.4f, 1.5f, deltaTime / 0.3f);
		}
		return result;
	}

	public float LerpOffset()
	{
		float result = 0f;
		switch (step)
		{
		case MissionDialog.STAMP_STEP.STAMP_IN:
			result = 10f;
			break;
		case MissionDialog.STAMP_STEP.STAMP_WAIT:
		case MissionDialog.STAMP_STEP.STAMP_NONE:
			result = 10f;
			break;
		}
		return result;
	}
}
