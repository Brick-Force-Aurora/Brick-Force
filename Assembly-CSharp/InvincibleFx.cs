using UnityEngine;

public class InvincibleFx : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.SCREEN_FX;

	public Texture2D shieldFx;

	private LocalController localController;

	public Color clrFrom = Color.blue;

	public Color clrTo = Color.white;

	public float flickerFrom = 1f;

	public float flickerTo = 0.2f;

	private float flicker;

	private float deltaTime;

	private bool showFx = true;

	private void VerifyLocalController()
	{
		if (null == localController)
		{
			localController = GetComponent<LocalController>();
		}
	}

	private void Start()
	{
		VerifyLocalController();
	}

	private void OnGUI()
	{
		if (null != localController && localController.Invincible && showFx)
		{
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			Color color = GUI.color;
			GUI.color = Color.Lerp(clrFrom, clrTo, localController.NormalizedInvincibleTimer);
			TextureUtil.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), shieldFx, ScaleMode.StretchToFill, alphaBlend: true);
			GUI.color = color;
			GUI.enabled = true;
		}
	}

	private void OnRespawn(Quaternion rotation)
	{
		deltaTime = 0f;
		showFx = true;
	}

	private void Update()
	{
		VerifyLocalController();
		if (null != localController && localController.Invincible)
		{
			flicker = Mathf.Lerp(flickerFrom, flickerTo, localController.NormalizedInvincibleTimer);
			deltaTime += Time.deltaTime;
			if (showFx)
			{
				if (deltaTime > flicker)
				{
					deltaTime = 0f;
					showFx = false;
				}
			}
			else if (deltaTime > flickerTo)
			{
				deltaTime = 0f;
				showFx = true;
			}
		}
	}
}
