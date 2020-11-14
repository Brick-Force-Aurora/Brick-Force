using UnityEngine;

public class KillerPortrait : MonoBehaviour
{
	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	private string[] hahaVoc = new string[5]
	{
		"Smile01",
		"Smile08",
		"Smile09",
		"Smile10",
		"Smile12"
	};

	public Texture2D killerPortraitFrame;

	private float deltaTime;

	private KillInfo killInfo;

	private bool show;

	private Rect crdPortrait = new Rect(0f, 19f, 256f, 256f);

	private Rect crdFrameSize = new Rect(0f, 0f, 256f, 294f);

	private Vector2 crdNickname = new Vector2(128f, 286f);

	private Color killerColor = Color.red;

	public float showTime = 1.3f;

	public float hideTime = 3f;

	private void Start()
	{
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn)
		{
			GUISkin skin = GUI.skin;
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			if (show && killInfo != null)
			{
				GameObject gameObject = BrickManManager.Instance.Get(killInfo.KillerSequence);
				if (null != gameObject)
				{
					Camera camera = null;
					Camera[] componentsInChildren = gameObject.GetComponentsInChildren<Camera>();
					int num = 0;
					while (camera == null && num < componentsInChildren.Length)
					{
						if (componentsInChildren[num].enabled)
						{
							camera = componentsInChildren[num];
						}
						num++;
					}
					if (null != camera)
					{
						GUI.BeginGroup(new Rect(((float)Screen.width - crdFrameSize.width) / 2f, 64f, crdFrameSize.width, crdFrameSize.height));
						TextureUtil.DrawTexture(crdPortrait, camera.targetTexture, ScaleMode.StretchToFill, alphaBlend: false);
						TextureUtil.DrawTexture(crdFrameSize, killerPortraitFrame, ScaleMode.StretchToFill);
						LabelUtil.TextOut(crdNickname, killInfo.Killer, "Label", killerColor, Color.white, TextAnchor.LowerCenter);
						GUI.EndGroup();
					}
				}
			}
			GUI.enabled = true;
			GUI.skin = skin;
		}
	}

	private void OnKillLog(KillInfo log)
	{
		if (log.VictimSequence == MyInfoManager.Instance.Seq && log.KillerSequence != log.VictimSequence)
		{
			killInfo = log;
			deltaTime = 0f;
			show = false;
		}
	}

	private void Update()
	{
		if (killInfo != null)
		{
			deltaTime += Time.deltaTime;
			if (deltaTime > hideTime)
			{
				killInfo = null;
				deltaTime = 0f;
				show = false;
			}
			else if (!show && deltaTime > showTime)
			{
				show = true;
				GameObject gameObject = BrickManManager.Instance.Get(killInfo.KillerSequence);
				if (gameObject != null)
				{
					LookCoordinator component = gameObject.GetComponent<LookCoordinator>();
					if (component != null)
					{
						if (component.IsYang)
						{
							VoiceManager.Instance.Play2(hahaVoc[Random.Range(0, hahaVoc.Length)]);
						}
						else
						{
							VoiceManager.Instance.Play0(hahaVoc[Random.Range(0, hahaVoc.Length)]);
						}
					}
				}
			}
		}
	}
}
