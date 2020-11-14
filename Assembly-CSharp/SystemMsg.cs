using UnityEngine;

public class SystemMsg
{
	private const float speedUp = 30f;

	private Color clrText;

	private Color clrOutline;

	private float offset;

	private string message;

	private float lap;

	private bool valid;

	public Color startText = new Color(0.91f, 0.6f, 0f, 1f);

	public Color endText = new Color(0.91f, 0.6f, 0f, 0f);

	public Color startLine = Color.black;

	public Color endLine = GlobalVars.txtEmptyColor;

	public Rect rc = new Rect(0f, 0f, 0f, 0f);

	public bool calculatedRC;

	private float lapTime = 5f;

	private float fadeoutTime = 2f;

	public string Message => message;

	public bool Valid => valid;

	public float Laptime
	{
		set
		{
			lapTime = value;
		}
	}

	public SystemMsg(string msg, float _lapTime)
	{
		clrText = startText;
		clrOutline = startLine;
		offset = 0f;
		lap = 0f;
		valid = true;
		message = msg;
		lapTime = _lapTime;
	}

	public void Update()
	{
		lap += Time.deltaTime;
		if (lap > lapTime + fadeoutTime)
		{
			valid = false;
		}
		else if (lap > lapTime)
		{
			clrText = Color.Lerp(startText, endText, lap - lapTime);
			clrOutline = Color.Lerp(startLine, endLine, lap - lapTime);
			offset += 30f * Time.deltaTime;
		}
	}

	public bool CalcRC(ref float height)
	{
		if (calculatedRC)
		{
			return false;
		}
		GUIStyle style = GUI.skin.GetStyle("BigLabel");
		TextAnchor alignment = style.alignment;
		style.alignment = TextAnchor.MiddleCenter;
		Vector2 vector = LabelUtil.CalcSize("BigLabel", message, (float)Screen.width * 0.8f);
		style.alignment = alignment;
		rc = new Rect(((float)Screen.width - vector.x) / 2f, ((float)Screen.height - vector.y) / 4f, vector.x, vector.y);
		height = vector.y + 32f;
		calculatedRC = true;
		return true;
	}

	public void Adjust(float height)
	{
		rc.y += height;
	}

	public void Show()
	{
		if (message.Length > 0)
		{
			GUIStyle style = GUI.skin.GetStyle("BigLabel");
			TextAnchor alignment = style.alignment;
			style.alignment = TextAnchor.MiddleCenter;
			Color color = GUI.color;
			GUI.color = clrOutline;
			GUI.Label(new Rect(rc.x, rc.y + offset + 2f, rc.width, rc.height), message, "BigLabel");
			GUI.color = clrText;
			GUI.Label(new Rect(rc.x, rc.y + offset, rc.width, rc.height), message, "BigLabel");
			GUI.color = color;
			style.alignment = alignment;
		}
	}

	public void Reset()
	{
		clrText = startText;
		clrOutline = startLine;
		offset = 0f;
		lap = 0f;
	}
}
