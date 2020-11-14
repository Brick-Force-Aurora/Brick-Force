using System;
using UnityEngine;

[Serializable]
public class UITextFiled : UIBase
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
			Debug.LogError("UITextFiled controlName not setting. enter name");
		}
		Vector2 showPosition = base.showPosition;
		float x = showPosition.x;
		Vector2 showPosition2 = base.showPosition;
		inputText = GUI.TextField(new Rect(x, showPosition2.y, area.x, area.y), inputText);
		if (inputText.Length > maxTextLength)
		{
			inputText = text;
		}
		return false;
	}

	public string GetInputText()
	{
		inputText = inputText.Replace("\t", string.Empty);
		inputText = inputText.Replace("\n", string.Empty);
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
