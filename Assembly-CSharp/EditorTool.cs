using UnityEngine;

public class EditorTool
{
	public BattleChat battleChat;

	protected Item item;

	protected EditorToolScript editorToolScript;

	protected bool active;

	public bool IsActive => active;

	public Texture2D Icon => (!IsEnable()) ? editorToolScript.desc.disable : editorToolScript.desc.enable;

	public string Hotkey => custom_inputs.Instance.GetKeyCodeName(editorToolScript.inputKey);

	public string Name => editorToolScript.desc.name;

	public string Amount
	{
		get
		{
			if (item == null)
			{
				return string.Empty;
			}
			return item.GetAmountString();
		}
	}

	public EditorTool(EditorToolScript ets, Item i, BattleChat _battleChat)
	{
		editorToolScript = ets;
		item = i;
		active = false;
		battleChat = _battleChat;
	}

	public virtual void OnClose()
	{
	}

	public virtual bool IsEnable()
	{
		return true;
	}

	public virtual bool Update()
	{
		return false;
	}

	public void Activate(bool activate)
	{
		active = activate;
		if (!active)
		{
			OnClose();
		}
	}
}
