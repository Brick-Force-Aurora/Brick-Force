using UnityEngine;

public class AutoFunction
{
	public const float ONCE_TIME = 1E-05f;

	public const float FRAME_PER = 1E-05f;

	public FunctionPointer functionPointer;

	public FunctionPointer endFunctionPointer;

	public float endTime = 1E-05f;

	private float totalTime;

	public float updateTime = 1E-05f;

	private float currentTime;

	public AutoFunction(FunctionPointer fp, float endTime)
	{
		functionPointer = fp;
		this.endTime = endTime;
	}

	public AutoFunction(FunctionPointer fp, float endTime, float updateTime)
		: this(fp, endTime)
	{
		this.updateTime = updateTime;
	}

	public AutoFunction(FunctionPointer fp, float endTime, float updateTime, FunctionPointer endfp)
		: this(fp, endTime, updateTime)
	{
		endFunctionPointer = endfp;
	}

	public bool Update()
	{
		float deltaTime = Time.deltaTime;
		bool result = false;
		totalTime += deltaTime;
		if ((currentTime += deltaTime) >= updateTime)
		{
			currentTime = 0f;
			if (functionPointer != null)
			{
				result = functionPointer();
			}
			if (totalTime >= endTime)
			{
				result = true;
			}
		}
		return result;
	}

	public void EndFunctionCall()
	{
		if (endFunctionPointer != null)
		{
			endFunctionPointer();
		}
	}
}
