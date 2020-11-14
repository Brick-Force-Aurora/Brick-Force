using UnityEngine;

public class BlackholeScreenFX : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.SCREEN_FX;

	public Texture2D shieldFx;

	private LocalController localController;

	public Color clrFrom = Color.white;

	public Color clrTo = Color.black;

	public AudioClip sndStart;

	public AudioClip sndEnd;

	private float deltaTime;

	private bool showFx;

	private int userSeq = -1;

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
		if (null != localController && showFx)
		{
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			float num = (3f - deltaTime) / 3f;
			if (num > 1f)
			{
				num = 1f;
			}
			Color color = GUI.color;
			GUI.color = Color.Lerp(clrFrom, clrTo, num);
			GUI.color = color;
			GUI.enabled = true;
		}
	}

	public void Reset(int _userSeq)
	{
		deltaTime = 0f;
		showFx = true;
		userSeq = _userSeq;
		GlobalVars.Instance.PlayOneShot(sndStart);
		GlobalVars.Instance.EnableScreenBrightSmart(bEnable: true, 6f, 0f, -0.5f);
	}

	private void Update()
	{
		VerifyLocalController();
		if (null != localController && showFx)
		{
			deltaTime += Time.deltaTime;
			if (deltaTime > 3f)
			{
				deltaTime = 0f;
				showFx = false;
				if (!localController.IsDead && !localController.bungeeRespawn)
				{
					P2PManager.Instance.SendPEER_BLACKHOLE_EFF(localController.TranceformPosition);
					BlackHole component = GameObject.Find("Main").GetComponent<BlackHole>();
					localController.TranceformPosition = component.gotoPos(MyInfoManager.Instance.Slot);
					localController.ActivateBlackhole = true;
					localController.ActivateBlackholeUser = userSeq;
					if (MyInfoManager.Instance.Seq == userSeq)
					{
						GlobalVars.Instance.PlayOneShot(sndEnd);
					}
				}
			}
		}
	}
}
