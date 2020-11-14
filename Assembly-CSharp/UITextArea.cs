using System;
using UnityEngine;

[Serializable]
public class UITextArea : UIBase
{
	public Vector2 area;

	public string controlName = string.Empty;

	public int maxTextLength = 40;

	public bool deleteSpace;

	private string inputText = string.Empty;

	public override bool Draw()
	{
		if (!isDraw)
		{
			return false;
		}
		string text = inputText;
		if (controlName.Length > 0)
		{
			GUI.SetNextControlName(controlName);
		}
		else
		{
			Debug.LogError("UITextArea controlName not setting. enter name");
		}
		Vector2 showPosition = base.showPosition;
		float x = showPosition.x;
		Vector2 showPosition2 = base.showPosition;
		inputText = GUI.TextArea(new Rect(x, showPosition2.y, area.x, area.y), inputText, maxTextLength);
		if (inputText.Length > maxTextLength)
		{
			inputText = text;
		}
		return false;
	}

	public string GetInputText()
	{
		inputText = inputText.Replace("\t", string.Empty);
		if (deleteSpace)
		{
			inputText = inputText.Replace(" ", string.Empty);
		}
		return inputText;
	}

	public void ResetText()
	{
		inputText = string.Empty;
	}
}
