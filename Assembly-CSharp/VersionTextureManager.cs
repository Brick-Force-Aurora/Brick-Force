using UnityEngine;

public class VersionTextureManager : MonoBehaviour
{
	public GameObject buildObject;

	public GameObject seasonObject;

	public ArmorTexture buildTexture;

	public SeasonTexture seasonTexture;

	public MovieTexture moviePublisher;

	private static VersionTextureManager _instance;

	public static VersionTextureManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(VersionTextureManager)) as VersionTextureManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the VersionTextureManager Instance");
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
		buildTexture = buildObject.GetComponent<ArmorTexture>();
		seasonTexture = seasonObject.GetComponent<SeasonTexture>();
	}
}
