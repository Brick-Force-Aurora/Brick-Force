public class BuildTool : EditorTool
{
	public BuildTool(EditorToolScript ets, BattleChat _battleChat)
		: base(ets, null, _battleChat)
	{
	}

	public override bool Update()
	{
		if (!battleChat.IsChatting && custom_inputs.Instance.GetButtonDown(editorToolScript.inputKey))
		{
			active = true;
			return true;
		}
		return false;
	}
}
