using UnityEngine;

public class WebParam : MonoBehaviour
{
	private string param = string.Empty;

	private static WebParam _instance;

	public string Parameters => param;

	public bool HasParameters => param.Length > 0;

	public static WebParam Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(WebParam)) as WebParam);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the WebParam Instance");
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	public void SetLoginParameters(string parameters)
	{
		Debug.Log("SetLoginParameters: " + parameters);
		param = parameters;
		GameObject gameObject = GameObject.Find("Main");
		if (null != gameObject)
		{
			AutoLogout component = gameObject.GetComponent<AutoLogout>();
			if (null != component)
			{
				component.Relogin(param);
			}
		}
	}
}
