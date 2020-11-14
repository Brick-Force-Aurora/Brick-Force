using UnityEngine;

public class RareFx
{
	public enum RAREFX_STEP
	{
		SIZE_UP,
		FLY,
		BOUNCE,
		DONE
	}

	private float deltaTime;

	private RAREFX_STEP rareFxStep;

	private float sizeUpRandomMin = 0.5f;

	private float sizeUpRandomMax = 1f;

	private float flyRandomMin = 0.1f;

	private float flyRandomMax = 0.3f;

	private float bounceMax = 1f;

	private float flyMax;

	private float sizeUpMax;

	private Vector2 start;

	private Vector2 end;

	public float Delta
	{
		get
		{
			switch (rareFxStep)
			{
			case RAREFX_STEP.SIZE_UP:
				return deltaTime / sizeUpMax;
			case RAREFX_STEP.FLY:
				return deltaTime / flyMax;
			case RAREFX_STEP.BOUNCE:
				return deltaTime / bounceMax;
			default:
				return 1f;
			}
		}
	}

	public RAREFX_STEP RareFxStep => rareFxStep;

	public Vector2 Start => start;

	public Vector2 End => end;

	public RareFx(Vector2 src, Vector2 dst)
	{
		start = new Vector2(src.x + Random.Range(-24f, 24f), src.y + Random.Range(-24f, 24f));
		end = new Vector2(dst.x + Random.Range(-24f, 24f), dst.y + Random.Range(-24f, 24f));
		rareFxStep = RAREFX_STEP.SIZE_UP;
		flyMax = Random.Range(flyRandomMin, flyRandomMax);
		sizeUpMax = Random.Range(sizeUpRandomMin, sizeUpRandomMax);
	}

	public void Update()
	{
		deltaTime += Time.deltaTime;
		switch (rareFxStep)
		{
		case RAREFX_STEP.SIZE_UP:
			if (deltaTime > sizeUpMax)
			{
				deltaTime = 0f;
				rareFxStep = RAREFX_STEP.FLY;
			}
			break;
		case RAREFX_STEP.FLY:
			if (deltaTime > flyMax)
			{
				deltaTime = 0f;
				rareFxStep = RAREFX_STEP.BOUNCE;
			}
			break;
		case RAREFX_STEP.BOUNCE:
			if (deltaTime > bounceMax)
			{
				deltaTime = 0f;
				rareFxStep = RAREFX_STEP.DONE;
			}
			break;
		}
	}
}
