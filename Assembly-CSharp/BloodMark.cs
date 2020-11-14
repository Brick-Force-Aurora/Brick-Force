using UnityEngine;

public class BloodMark
{
	private Texture2D bloodMark;

	private Vector2 pos;

	private Color bloodColor;

	private Color toColor;

	private float scale;

	public bool IsAlive => bloodColor.a > 0.01f;

	public BloodMark(Texture2D blood, Color fromClr, Color toClr, float scaleFactor)
	{
		bloodMark = blood;
		Vector2 a = new Vector2((float)bloodMark.width, (float)bloodMark.height);
		a *= scaleFactor;
		Vector2 vector = new Vector2((float)(Screen.width / 4), (float)(Screen.height / 4));
		float num = Random.Range(0f - vector.x, vector.x);
		float num2 = Random.Range(0f - vector.y, vector.y);
		pos = new Vector2(((float)Screen.width - a.x) / 2f + num, ((float)Screen.height - a.y) / 2f + num2);
		bloodColor = fromClr;
		toColor = toClr;
		scale = scaleFactor;
	}

	public void Update()
	{
		bloodColor = Color.Lerp(bloodColor, toColor, Time.deltaTime);
	}

	public void Draw()
	{
		Color color = GUI.color;
		GUI.color = bloodColor;
		float width = (float)bloodMark.width * scale;
		float height = (float)bloodMark.height * scale;
		TextureUtil.DrawTexture(new Rect(pos.x, pos.y, width, height), bloodMark, ScaleMode.StretchToFill, alphaBlend: true);
		GUI.color = color;
	}
}
