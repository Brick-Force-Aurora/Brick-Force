using UnityEngine;

public class ScriptDialog : MonoBehaviour
{
	private const float lifeTime = 5f;

	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.GAME_CONTROL;

	private int speaker;

	private string text;

	public int fmt = -1;

	public int Speaker
	{
		get
		{
			return speaker;
		}
		set
		{
			speaker = value;
		}
	}

	public string Text
	{
		get
		{
			return text;
		}
		set
		{
			text = value;
		}
	}

	private void OnGUI()
	{
		GUI.skin = GUISkinFinder.Instance.GetGUISkin();
		GUI.depth = (int)guiDepth;
		GUI.enabled = !DialogManager.Instance.IsModal;
		Color color = GUI.color;
		GUI.color = Color.white;
		if (fmt > 0)
		{
			int num = custom_inputs.Instance.KeyIndex("K_JUMP");
			text = string.Format(text, custom_inputs.Instance.InputKey[num].ToString());
		}
		LabelUtil.TextOut(new Vector2((float)(Screen.width / 2 + 2), (float)(Screen.height - 244)), text, "MissionTitleLabel", Color.black, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter, 480f);
		LabelUtil.TextOut(new Vector2((float)(Screen.width / 2), (float)(Screen.height - 246)), text, "MissionTitleLabel", GlobalVars.Instance.txtMainColor, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter, 480f);
		GUI.color = color;
		GUI.enabled = true;
	}

	private void Start()
	{
	}

	private bool CheckSkipButton()
	{
		return Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0);
	}
}
