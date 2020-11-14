using UnityEngine;

public class TokenManager : MonoBehaviour
{
	private static TokenManager _instance;

	public Token[] tokens;

	public Token currentToken;

	public static TokenManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(TokenManager)) as TokenManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the TokenManager Instance");
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
		currentToken = tokens[(int)BuildOption.Instance.Props.TokenType];
	}

	public void SetCurrentToken(Token.TYPE type)
	{
		currentToken = tokens[(int)type];
	}

	public string GetTokenString()
	{
		return StringMgr.Instance.Get(currentToken.name);
	}
}
