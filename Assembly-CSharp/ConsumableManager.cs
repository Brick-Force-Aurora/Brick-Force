using UnityEngine;

public class ConsumableManager : MonoBehaviour
{
	public ConsumableDesc[] consumables;

	private static ConsumableManager instance;

	public static ConsumableManager Instance
	{
		get
		{
			if (null == instance)
			{
				instance = (Object.FindObjectOfType(typeof(ConsumableManager)) as ConsumableManager);
				if (null == instance)
				{
					Debug.LogError("ERROR, Fail to get the ConsumableManager Instance");
				}
			}
			return instance;
		}
	}

	public ConsumableDesc Get(string func)
	{
		func = func.ToLower();
		for (int i = 0; i < consumables.Length; i++)
		{
			if (consumables[i].name == func)
			{
				return consumables[i];
			}
		}
		return null;
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
