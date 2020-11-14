using UnityEngine;

public class ScriptAlarm : MonoBehaviour
{
	private float deltaTime;

	private float timer;

	public float DeltaTime
	{
		get
		{
			return deltaTime;
		}
		set
		{
			deltaTime = value;
		}
	}

	private void Start()
	{
		timer = 0f;
	}

	private void Update()
	{
		timer += Time.deltaTime;
		if (timer > deltaTime)
		{
			ScriptExecutor component = GetComponent<ScriptExecutor>();
			if (null != component)
			{
				component.enabled = true;
			}
			Object.DestroyImmediate(this);
		}
	}
}
