using UnityEngine;

public class MonController : MonoBehaviour
{
	private Animation bipAnimation;

	private MonProperty monProperty;

	private MonAI monAI;

	private float deltaTime;

	private bool bDie;

	private string dieStr;

	public bool bSleep;

	public bool bImmediateBoom;

	public bool bBigBoom;

	public float timerBoomTimeMax = 1.5f;

	public int monTblID = -1;

	private clipNameManager clipNameMgr;

	private void InitializeAnimation()
	{
		monProperty = GetComponent<MonProperty>();
		if (null == monProperty)
		{
			Debug.LogError("Fail to get MonProperty component");
		}
		else
		{
			Animation[] componentsInChildren = GetComponentsInChildren<Animation>();
			clipNameMgr = new clipNameManager();
			clipNameMgr.Alloc();
			bipAnimation = null;
			int num = 0;
			while (bipAnimation == null && num < componentsInChildren.Length)
			{
				bipAnimation = componentsInChildren[num];
				clipNameMgr.Add(componentsInChildren[num].clip.name);
				num++;
			}
			if (null == bipAnimation)
			{
				Debug.LogError("Fail to get Animation component");
			}
			else
			{
				SetIdle(queued: false);
			}
		}
	}

	public void Reset()
	{
		bSleep = false;
		bDie = false;
		SetIdle(queued: false);
	}

	private void SetIdle(bool queued)
	{
		if (!bDie)
		{
			if (queued)
			{
				bipAnimation.CrossFadeQueued("walk");
			}
			else
			{
				bipAnimation.CrossFade("walk");
			}
			bipAnimation["walk"].wrapMode = WrapMode.Loop;
			deltaTime = 0f;
		}
	}

	private void Start()
	{
		InitializeAnimation();
	}

	private void Update()
	{
		if (!bSleep)
		{
			if (bDie)
			{
				deltaTime += Time.deltaTime;
				if (bImmediateBoom || deltaTime > timerBoomTimeMax)
				{
					if (bBigBoom)
					{
						Object.Instantiate((Object)MonManager.Instance.m_expFX4x, base.transform.position, Quaternion.Euler(0f, 0f, 0f));
					}
					else
					{
						Object.Instantiate((Object)MonManager.Instance.m_expFX, base.transform.position, Quaternion.Euler(0f, 0f, 0f));
					}
					if (monAI != null)
					{
						monAI.LastCommand();
					}
					MonManager.Instance.Remove(monProperty.Desc.Seq);
					bSleep = true;
				}
			}
			else
			{
				deltaTime += Time.deltaTime;
				if (!bDie && monProperty.Desc.Xp <= 0)
				{
					bDie = true;
					bImmediateBoom = true;
					monAI = MonManager.Instance.GetAIClass(monProperty.Desc.Seq, monProperty.Desc.tblID);
					if (MyInfoManager.Instance.Seq == RoomManager.Instance.Master)
					{
						if (monProperty.Desc.bRedTeam)
						{
							DefenseManager.Instance.BluePoint += monProperty.Desc.Dp;
						}
						else
						{
							DefenseManager.Instance.RedPoint += monProperty.Desc.Dp;
						}
						CSNetManager.Instance.Sock.SendCS_MISSION_POINT_REQ(DefenseManager.Instance.RedPoint, DefenseManager.Instance.BluePoint);
					}
					if (monProperty.Desc.tblID == 4)
					{
						MonManager.Instance.BossUnVisibleAll(monProperty.Desc.bRedTeam);
					}
					if (monAI == null)
					{
						Debug.LogError("{ MonController } monai == null: " + monProperty.Desc.tblID);
					}
					monAI.Die();
				}
			}
		}
	}

	public void HitAniamtion()
	{
		if (clipNameMgr.Find("hit"))
		{
			bipAnimation.Play("hit");
		}
	}
}
