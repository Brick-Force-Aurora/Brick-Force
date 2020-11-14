using UnityEngine;

public class FeverFx : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.SCREEN_FX;

	public Texture2D[] screenFx;

	private LocalController localController;

	private bool secondImgIn;

	private float secondIn = 0.35f;

	private float deltaTime1;

	private float deltaTime2;

	private Color FromColor = Color.white;

	private Color ToColor = new Color(1f, 1f, 1f, 0f);

	private Color screenColor1 = Color.white;

	private Color screenColor2 = Color.white;

	private void Start()
	{
		deltaTime1 = 0f;
		deltaTime2 = 0f;
		secondImgIn = false;
		localController = GetComponent<LocalController>();
	}

	public void reset()
	{
		deltaTime1 = 0f;
		deltaTime2 = 0f;
		secondImgIn = false;
	}

	private void OnGUI()
	{
		if (localController.ActingFever)
		{
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			Color color = GUI.color;
			GUI.color = screenColor1;
			TextureUtil.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), screenFx[0], ScaleMode.StretchToFill, alphaBlend: true);
			if (secondImgIn)
			{
				GUI.color = screenColor2;
				TextureUtil.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), screenFx[1], ScaleMode.StretchToFill, alphaBlend: true);
			}
			GUI.color = color;
			GUI.enabled = true;
		}
	}

	private void Update()
	{
		if (localController.ActingFever)
		{
			deltaTime1 += Time.deltaTime;
			screenColor1 = Color.Lerp(FromColor, ToColor, deltaTime1);
			if (deltaTime1 > 1f)
			{
				deltaTime1 = 0f;
			}
			if (!secondImgIn && deltaTime1 > secondIn)
			{
				secondImgIn = true;
			}
			if (secondImgIn)
			{
				deltaTime2 += Time.deltaTime;
				screenColor2 = Color.Lerp(FromColor, ToColor, deltaTime2);
				if (deltaTime2 > 1f)
				{
					deltaTime2 = 0f;
				}
			}
		}
	}
}
