using UnityEngine;

public class ContextMenuManager : MonoBehaviour
{
	public UserMenu userMenu;

	private bool isPopup;

	private static ContextMenuManager _instance;

	public bool IsPopup => isPopup;

	public static ContextMenuManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(ContextMenuManager)) as ContextMenuManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the ContextMenuManager Instance");
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	private void CheckClickOutside()
	{
		if (MouseUtil.ClickOutside(userMenu.ClientRect))
		{
			isPopup = false;
		}
	}

	public UserMenu Popup()
	{
		isPopup = true;
		return userMenu;
	}

	private void windowFunction(int id)
	{
		if (userMenu.DoDialog())
		{
			isPopup = false;
			userMenu.OnClose(DialogManager.DIALOG_INDEX.NUM);
		}
	}

	public void Clear()
	{
		isPopup = false;
	}

	public void CloseAll()
	{
		userMenu.OnClose(DialogManager.DIALOG_INDEX.NUM);
		isPopup = false;
	}

	private void OnGUI()
	{
		if (isPopup)
		{
			GlobalVars.Instance.BeginGUI(null);
			GUISkin gUISkin = GUISkinFinder.Instance.GetGUISkin();
			if (null != gUISkin)
			{
				GUI.skin = gUISkin;
				CheckClickOutside();
				if (isPopup)
				{
					GUI.Window(1025, userMenu.ClientRect, windowFunction, string.Empty, userMenu.WindowStyle);
					GUI.BringWindowToFront(1025);
				}
			}
			GlobalVars.Instance.EndGUI();
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (isPopup)
		{
			userMenu.Update();
		}
	}
}
