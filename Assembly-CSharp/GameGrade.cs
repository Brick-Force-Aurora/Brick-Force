using UnityEngine;

public class GameGrade : MonoBehaviour
{
	public Texture2D texGrade;

	private Rect crdBg = new Rect(828f, 2f, 194f, 114f);

	private float delta;

	public float maxDelta = 3600f;

	private float deltaShow;

	private float maxDeltaShow = 3f;

	private bool showPic;

	private static GameGrade _instance;

	public static GameGrade Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(GameGrade)) as GameGrade);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the GameGrade Instance");
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	private void OnGUI()
	{
		if (BuildOption.Instance.IsNetmarble || BuildOption.Instance.IsDeveloper)
		{
			GlobalVars.Instance.BeginGUI(null);
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = 0;
			if (showPic)
			{
				TextureUtil.DrawTexture(crdBg, texGrade, ScaleMode.StretchToFill);
			}
			GlobalVars.Instance.EndGUI();
		}
	}

	private void Update()
	{
		if (BuildOption.Instance.IsNetmarble || BuildOption.Instance.IsDeveloper)
		{
			delta += Time.deltaTime;
			if (delta > maxDelta)
			{
				delta = 0f;
				deltaShow = 0f;
				showPic = true;
			}
			if (showPic)
			{
				deltaShow += Time.deltaTime;
				if (deltaShow > maxDeltaShow)
				{
					showPic = false;
				}
			}
		}
	}
}
