using System;
using System.Collections.Generic;

public class BfScript
{
	private string alias;

	private bool enableOnAwake;

	private bool visibleOnAwake;

	private List<ScriptCmd> cmdList;

	public string Alias
	{
		get
		{
			return alias;
		}
		set
		{
			alias = value;
		}
	}

	public bool EnableOnAwake
	{
		get
		{
			return enableOnAwake;
		}
		set
		{
			enableOnAwake = value;
		}
	}

	public bool VisibleOnAwake
	{
		get
		{
			return visibleOnAwake;
		}
		set
		{
			visibleOnAwake = value;
		}
	}

	public List<ScriptCmd> CmdList => cmdList;

	public BfScript(string _alias, bool _enableOnAwake, bool _visibleOnAwake, string _cmdList)
	{
		_alias = _alias.Trim(default(char));
		alias = _alias;
		enableOnAwake = _enableOnAwake;
		visibleOnAwake = _visibleOnAwake;
		cmdList = new List<ScriptCmd>();
		string[] array = _cmdList.Split(ScriptCmd.CmdDelimeters, StringSplitOptions.RemoveEmptyEntries);
		if (array != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				ScriptCmd item = ScriptCmdFactory.Create(array[i]);
				cmdList.Add(item);
			}
		}
	}

	public string GetCommandString()
	{
		string text = string.Empty;
		for (int i = 0; i < cmdList.Count; i++)
		{
			text += cmdList[i].GetDescription();
			if (i < cmdList.Count - 1)
			{
				text += ScriptCmd.CmdDelimeters[0];
			}
		}
		return text;
	}
}
