using UnityEngine;

public class InstalledBomb : MonoBehaviour
{
	public enum STATUS
	{
		NOT_INSTALLED,
		INSTALLED,
		UNINSTALLED,
		BLASTED
	}

	public GameObject kaboom;

	public GameObject kaboom11;

	public GameObject outOfOrder;

	public float detonatingTime = 30f;

	public float notifyCycle = 0.2f;

	public AudioClip tickTock;

	public AudioClip tickTickTickTickTick;

	public AudioClip disarm;

	public Texture2D bg;

	public Texture2D flicker;

	public Texture2D colon;

	public ImageFont d0;

	public ImageFont d1;

	public ImageFont d2;

	public ImageFont d3;

	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.HUD;

	private float deltaTime;

	private float notifyDelta;

	private float flickerDelta;

	private float popDelta;

	private float popNext;

	private STATUS status;

	private bool beep;

	private AudioSource audioSource;

	private Animation bipAnimation;

	private ExplosionMatch explosionMatch;

	private void Start()
	{
		status = STATUS.NOT_INSTALLED;
		VerifyAudioSource();
		VerifyExplosionMatch();
		Hide();
		InitializeAnimation();
	}

	private void VerifyAudioSource()
	{
		if (null == audioSource)
		{
			audioSource = GetComponent<AudioSource>();
		}
	}

	private void VerifyExplosionMatch()
	{
		if (explosionMatch == null)
		{
			GameObject gameObject = GameObject.Find("Main");
			if (null != gameObject)
			{
				explosionMatch = gameObject.GetComponent<ExplosionMatch>();
			}
		}
	}

	private void InitializeAnimation()
	{
		Animation[] componentsInChildren = GetComponentsInChildren<Animation>();
		bipAnimation = null;
		int num = 0;
		while (bipAnimation == null && num < componentsInChildren.Length)
		{
			if (componentsInChildren[num].name == "clockbomb")
			{
				bipAnimation = componentsInChildren[num];
			}
			num++;
		}
		if (null == bipAnimation)
		{
			Debug.LogError("Fail to find biped animation for InstalledBomb");
		}
		else
		{
			bipAnimation.wrapMode = WrapMode.Loop;
			bipAnimation["die"].layer = 1;
			bipAnimation["die"].wrapMode = WrapMode.Loop;
			bipAnimation["idle1"].layer = 1;
			bipAnimation["idle1"].wrapMode = WrapMode.Loop;
			bipAnimation["idle2"].layer = 1;
			bipAnimation["idle2"].wrapMode = WrapMode.Loop;
			bipAnimation["walk"].layer = 1;
			bipAnimation["walk"].wrapMode = WrapMode.Loop;
		}
	}

	private void Show()
	{
		SkinnedMeshRenderer[] componentsInChildren = GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: true);
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
		{
			skinnedMeshRenderer.enabled = true;
		}
	}

	private void Hide()
	{
		if (null != audioSource)
		{
			audioSource.Stop();
		}
		SkinnedMeshRenderer[] componentsInChildren = GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: true);
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
		{
			skinnedMeshRenderer.enabled = false;
		}
	}

	public void StopClockBombSound()
	{
		if (null != audioSource)
		{
			audioSource.Stop();
		}
	}

	public void HideAway()
	{
		Rigidbody component = GetComponent<Rigidbody>();
		if (null != component)
		{
			component.isKinematic = true;
		}
		Hide();
		base.transform.position = new Vector3(0f, -1000f, 0f);
		status = STATUS.NOT_INSTALLED;
	}

	private void OnGUI()
	{
		if (MyInfoManager.Instance.isGuiOn)
		{
			GUISkin skin = GUI.skin;
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			if (explosionMatch.Blastable)
			{
				TextureUtil.DrawTexture(new Rect((float)((Screen.width - bg.width) / 2), 55f, (float)bg.width, (float)bg.height), bg, ScaleMode.StretchToFill);
				if (deltaTime > 22f && beep)
				{
					TextureUtil.DrawTexture(new Rect((float)((Screen.width - flicker.width) / 2), 55f, (float)flicker.width, (float)flicker.height), flicker, ScaleMode.StretchToFill);
				}
				float remainTime = GetRemainTime();
				int num = Mathf.FloorToInt(remainTime * 100f);
				int[] array = new int[4];
				int num2 = 1000;
				for (int i = 0; i < 4; i++)
				{
					array[i] = num / num2;
					num %= num2;
					num2 /= 10;
				}
				d0.Print(new Vector2((float)(Screen.width / 2), 72f), array[0]);
				d1.Print(new Vector2((float)(Screen.width / 2 + 13), 72f), array[1]);
				d2.Print(new Vector2((float)(Screen.width / 2 + 23), 72f), array[2]);
				d3.Print(new Vector2((float)(Screen.width / 2 + 36), 72f), array[3]);
				TextureUtil.DrawTexture(new Rect((float)((Screen.width - colon.width) / 2 + 18), 72f, (float)colon.width, (float)colon.height), colon, ScaleMode.StretchToFill);
			}
			if (Application.loadedLevelName.Contains("Tutor"))
			{
				Camera camera = null;
				GameObject gameObject = GameObject.Find("Main Camera");
				if (null == gameObject)
				{
					Debug.LogError("Fail to find mainCamera for tutor arrow");
				}
				else
				{
					camera = gameObject.GetComponent<Camera>();
					if (null == camera)
					{
						Debug.LogError("Fail to get mainCam for tutor arrow");
					}
				}
				Vector3 position = camera.transform.position;
				float num3 = Vector3.Distance(position, base.transform.position);
				if (num3 < 2f)
				{
					int num4 = custom_inputs.Instance.KeyIndex("K_ACTION");
					string text = string.Format(StringMgr.Instance.Get("STRING_BOMB_INSTALL"), custom_inputs.Instance.InputKey[num4].ToString());
					Vector2 vector = LabelUtil.CalcLength("BoxResult", text);
					vector.x += 60f;
					GUI.Box(new Rect((float)(Screen.width / 2) - vector.x / 2f, (float)(Screen.height / 2 + 20), vector.x, 30f), text, "BoxResult");
				}
			}
			GUI.enabled = true;
			GUI.skin = skin;
		}
	}

	public void Blast()
	{
		status = STATUS.BLASTED;
		if (MyInfoManager.Instance.IsBelow12())
		{
			Object.Instantiate((Object)kaboom11, base.transform.position, Quaternion.Euler(0f, 0f, 0f));
		}
		else
		{
			Object.Instantiate((Object)kaboom, base.transform.position, Quaternion.Euler(0f, 0f, 0f));
		}
		Hide();
	}

	public void Install(Vector3 position, Vector3 normal)
	{
		Rigidbody component = GetComponent<Rigidbody>();
		if (null != component)
		{
			component.isKinematic = true;
		}
		base.transform.position = position;
		base.transform.forward = normal;
		flickerDelta = 0f;
		notifyDelta = 0f;
		deltaTime = 0f;
		popDelta = 0f;
		beep = false;
		status = STATUS.INSTALLED;
		Show();
		bipAnimation.Play("idle1");
		VerifyAudioSource();
		if (audioSource != null)
		{
			audioSource.Stop();
			audioSource.clip = tickTock;
			audioSource.loop = true;
			audioSource.Play();
		}
	}

	private void Pop()
	{
		popDelta = 0f;
		popNext = Random.Range(0.2f, 0.6f);
		Object.Instantiate((Object)outOfOrder, base.transform.position, Quaternion.identity);
	}

	public void Uninstall()
	{
		Rigidbody component = GetComponent<Rigidbody>();
		if (null != component)
		{
			component.isKinematic = false;
			component.AddForce((float)Random.Range(2, 10) * Vector3.forward, ForceMode.Impulse);
			component.AddTorque((float)Random.Range(2, 5) * Vector3.forward, ForceMode.Impulse);
		}
		flickerDelta = 0f;
		notifyDelta = 0f;
		deltaTime = 0f;
		beep = false;
		status = STATUS.UNINSTALLED;
		bipAnimation.Play("idle1");
		VerifyAudioSource();
		if (null != audioSource)
		{
			audioSource.Stop();
			audioSource.PlayOneShot(disarm);
		}
		Pop();
	}

	public void SetDeltaTime(float delta)
	{
		if (Mathf.Abs(deltaTime - delta) > 0.3f)
		{
			deltaTime = delta;
		}
	}

	private float GetRemainTime()
	{
		float num = detonatingTime - deltaTime;
		if (num < 0f)
		{
			num = 0f;
		}
		return num;
	}

	private void Update()
	{
		if (deltaTime > 22f)
		{
			flickerDelta += Time.deltaTime;
			if (flickerDelta > 0.5f)
			{
				flickerDelta = 0f;
				beep = !beep;
			}
		}
		if (status == STATUS.UNINSTALLED)
		{
			popDelta += Time.deltaTime;
			if (popDelta > popNext)
			{
				Pop();
			}
		}
		VerifyExplosionMatch();
		if (explosionMatch.Blastable)
		{
			deltaTime += Time.deltaTime;
			if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master)
			{
				notifyDelta += Time.deltaTime;
				if (notifyDelta > notifyCycle)
				{
					notifyDelta = 0f;
					P2PManager.Instance.SendPEER_BLAST_TIME(deltaTime);
				}
			}
			if (deltaTime > detonatingTime)
			{
				if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master && explosionMatch.CanBlast)
				{
					explosionMatch.Step = ExplosionMatch.STEP.BLASTING;
					CSNetManager.Instance.Sock.SendCS_BM_BLAST_REQ(explosionMatch.BlastTarget);
				}
			}
			else if (deltaTime > 22f)
			{
				if (!bipAnimation.IsPlaying("die"))
				{
					bipAnimation.Play("die");
					VerifyAudioSource();
					if (null != audioSource)
					{
						audioSource.Stop();
						audioSource.clip = tickTickTickTickTick;
						audioSource.loop = true;
						audioSource.Play();
					}
				}
			}
			else if (deltaTime > 16f && !bipAnimation.IsPlaying("idle2"))
			{
				bipAnimation.Play("idle2");
			}
		}
	}
}
