using UnityEngine;

public class Buff : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	public Vector2 offset;

	private bool flip;

	private float deltaTime;

	private void Start()
	{
		flip = false;
		deltaTime = 0f;
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn)
		{
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			string curTooltip = Aps.Instance.GetCurTooltip();
			Texture2D curLevelIcon = Aps.Instance.GetCurLevelIcon(flip);
			if (null != curLevelIcon)
			{
				GUI.Button(new Rect(offset.x, offset.y, 44f, 44f), new GUIContent(curLevelIcon, curTooltip), "InvisibleButton");
			}
			if (!DialogManager.Instance.IsModal && Event.current.type == EventType.Repaint && GUI.tooltip.Length > 0)
			{
				string text = StringMgr.Instance.Get(GUI.tooltip);
				if (text.Length > 0)
				{
					Vector2 mousePosition = Event.current.mousePosition;
					GUIStyle style = GUI.skin.GetStyle("MiniLabel");
					if (style != null)
					{
						Vector2 vector = style.CalcSize(new GUIContent(text));
						GUI.Box(new Rect(mousePosition.x, mousePosition.y, vector.x + 32f, vector.y + 16f), string.Empty, "BlackBox");
					}
					LabelUtil.TextOut(new Vector2(mousePosition.x + 16f, mousePosition.y + 8f), text, "MiniLabel", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperLeft);
				}
			}
			GUI.enabled = true;
		}
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		if (flip)
		{
			if (deltaTime > 0.3f)
			{
				deltaTime = 0f;
				flip = false;
			}
		}
		else if (deltaTime > 1f)
		{
			deltaTime = 0f;
			flip = true;
		}
	}
}
