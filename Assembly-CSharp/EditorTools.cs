using UnityEngine;

public class EditorTools : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.MENU;

	public GameObject dummy;

	public EditorToolScript[] editorToolScripts;

	private Vector2 crdToolSize = new Vector2(64f, 64f);

	private float offset = 7f;

	private Vector2 crdHotkey = new Vector2(55f, 18f);

	private Vector2 crdAmount = new Vector2(60f, 25f);

	private Vector2 crdOnOff = new Vector2(25f, 15f);

	private EditorTool[] editorTool;

	private BattleChat battleChat;

	public string GetActiveEditorTool()
	{
		for (int i = 0; i < editorTool.Length; i++)
		{
			if (editorTool[i].IsActive)
			{
				return editorTool[i].Name;
			}
		}
		return string.Empty;
	}

	private void Start()
	{
		battleChat = GetComponent<BattleChat>();
		editorToolScripts[0].desc = ConsumableManager.Instance.Get("build_tool");
		editorToolScripts[1].desc = ConsumableManager.Instance.Get("line_tool");
		editorToolScripts[2].desc = ConsumableManager.Instance.Get("replace_tool");
		editorTool = new EditorTool[editorToolScripts.Length];
		for (int i = 0; i < editorToolScripts.Length; i++)
		{
			Item i2 = null;
			long num = MyInfoManager.Instance.HaveFunction(editorToolScripts[i].desc.name);
			if (num >= 0)
			{
				i2 = MyInfoManager.Instance.GetItemBySequence(num);
			}
			if (editorToolScripts[i].desc.name == "build_tool")
			{
				editorTool[i] = new BuildTool(editorToolScripts[i], battleChat);
			}
			else if (editorToolScripts[i].desc.name == "line_tool")
			{
				editorTool[i] = new LineTool(editorToolScripts[i], i2, dummy, battleChat);
			}
			else if (editorToolScripts[i].desc.name == "replace_tool")
			{
				editorTool[i] = new ReplaceTool(editorToolScripts[i], i2, battleChat);
			}
		}
		editorTool[0].Activate(activate: true);
	}

	private void Update()
	{
		EditorTool editorTool = null;
		for (int i = 0; i < this.editorTool.Length; i++)
		{
			if (this.editorTool[i].Update())
			{
				editorTool = this.editorTool[i];
			}
		}
		if (editorTool != null)
		{
			for (int j = 0; j < this.editorTool.Length; j++)
			{
				if (this.editorTool[j] != editorTool)
				{
					this.editorTool[j].Activate(activate: false);
				}
			}
		}
	}

	public LineTool GetLineTool()
	{
		for (int i = 0; i < editorTool.Length; i++)
		{
			if (editorTool[i].IsActive && editorTool[i].Name == "line_tool")
			{
				return (LineTool)editorTool[i];
			}
		}
		return null;
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn)
		{
			GUISkin skin = GUI.skin;
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			int num = editorTool.Length;
			float num2 = (float)num * crdToolSize.x + (float)(num + 1) * offset;
			float num3 = crdToolSize.y + crdHotkey.y + crdAmount.y;
			Rect position = new Rect(((float)Screen.width - num2) / 2f, (float)Screen.height - num3, num2, num3);
			GUI.BeginGroup(position);
			for (int i = 0; i < editorTool.Length; i++)
			{
				Rect position2 = new Rect(offset + (float)i * (crdToolSize.x + offset), crdHotkey.y, crdToolSize.x, crdToolSize.y);
				GUI.Box(new Rect(position2.x + 6f, position2.y - crdHotkey.y, crdHotkey.x, crdHotkey.y), string.Empty, "cns_hotkey");
				GUI.Box(position2, new GUIContent(editorTool[i].Icon), "cns_item");
				Color clrText = Color.gray;
				string text = "off";
				if (editorTool[i].IsActive)
				{
					clrText = new Color(1f, 1f, 0f, 1f);
					text = "on";
				}
				GUI.Box(new Rect(position2.x + 38f, position2.y + 46f, crdOnOff.x, crdOnOff.y), string.Empty, "cns_onoff");
				LabelUtil.TextOut(new Vector2(position2.x + 32f, position2.y + 9f - crdHotkey.y), editorTool[i].Hotkey, "TinyLabel", clrText, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				LabelUtil.TextOut(new Vector2(position2.x + 38f + 10f, position2.y + 46f + 6f), text, "MiniLabel", clrText, GlobalVars.txtEmptyColor, TextAnchor.MiddleCenter);
				GUI.Box(new Rect(position2.x + 11f, position2.y + 60f, crdAmount.x, crdAmount.y), editorTool[i].Amount, "cns_count");
			}
			GUI.EndGroup();
			GUI.enabled = true;
			GUI.skin = skin;
		}
	}
}
