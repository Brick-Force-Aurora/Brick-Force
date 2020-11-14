using UnityEngine;

public class UVAnimation : MonoBehaviour
{
	public enum SCROLL_DIR
	{
		X,
		Y,
		XY
	}

	public float speed = 1f;

	public SCROLL_DIR scrollDir = SCROLL_DIR.XY;

	private Material _mat;

	private void Start()
	{
		_mat = base.renderer.material;
		if (_mat == null)
		{
			Debug.LogError($"UVAnimation.cs - 오브젝트 {base.name}에 매터리얼이 지정되지 않았습니다 !!!");
		}
	}

	private void Update()
	{
		Vector2 zero = Vector2.zero;
		if (scrollDir == SCROLL_DIR.X)
		{
			zero.x = Mathf.Sin(Time.time) * speed;
		}
		else if (scrollDir == SCROLL_DIR.Y)
		{
			zero.y = Mathf.Sin(Time.time) * speed;
		}
		else
		{
			zero.x = Mathf.Sin(Time.time) * speed;
			zero.y = Mathf.Sin(Time.time) * speed;
		}
		_mat.SetTextureOffset("_MainTex", zero);
		Color color = _mat.color;
		color.a = (Mathf.Sin(Time.time) + 1f) / 2f;
		_mat.SetColor("_Color", color);
	}
}
