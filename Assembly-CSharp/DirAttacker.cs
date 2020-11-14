using UnityEngine;

public class DirAttacker
{
	private float lifeTime;

	private Texture2D image;

	private Color clrStart;

	private Color clrEnd;

	private Color clr;

	private float deltaTime;

	private int attacker;

	private CameraController cameraController;

	public int Attacker => attacker;

	public DirAttacker(int _attacker, Texture2D _image, Color _clrStart, Color _clrEnd, float _lifeTime, CameraController _cameraController)
	{
		attacker = _attacker;
		image = _image;
		clrStart = _clrStart;
		clrEnd = _clrEnd;
		lifeTime = _lifeTime;
		cameraController = _cameraController;
		Reset();
	}

	public void Reset()
	{
		deltaTime = 0f;
		clr = clrStart;
	}

	public bool Update()
	{
		deltaTime += Time.deltaTime;
		clr = Color.Lerp(clrStart, clrEnd, deltaTime / lifeTime);
		return deltaTime <= lifeTime;
	}

	public bool Draw()
	{
		GameObject gameObject = BrickManManager.Instance.Get(attacker);
		if (null == gameObject)
		{
			return false;
		}
		Vector3 position = cameraController.transform.position;
		Vector3 from = cameraController.transform.TransformDirection(Vector3.forward);
		from.y = 0f;
		from = from.normalized;
		Vector3 from2 = cameraController.transform.TransformDirection(Vector3.right);
		from2.y = 0f;
		from2 = from2.normalized;
		Vector3 from3 = cameraController.transform.TransformDirection(Vector3.left);
		from3.y = 0f;
		from3 = from3.normalized;
		Vector3 to = gameObject.transform.position - position;
		to.y = 0f;
		to = to.normalized;
		float num = Vector3.Angle(from, to);
		if (Vector3.Angle(from2, to) >= Vector3.Angle(from3, to))
		{
			num = 180f + (180f - num);
		}
		GUI.color = clr;
		Vector2 pivotPoint = new Vector2((float)(Screen.width / 2), (float)(Screen.height / 2));
		float num2 = (float)Screen.height * 0.7f;
		float num3 = (float)Screen.height * 0.7f;
		Rect position2 = new Rect(pivotPoint.x - num2 / 2f, pivotPoint.y - num3 / 2f, num2, num3);
		Matrix4x4 matrix = GUI.matrix;
		GUIUtility.RotateAroundPivot(num, pivotPoint);
		TextureUtil.DrawTexture(position2, image, ScaleMode.StretchToFill);
		GUI.matrix = matrix;
		return true;
	}
}
