using UnityEngine;

public class AngelWingItem : ActiveItemBase
{
	private const float ANGELWING_ITEM_TIME = 10f;

	private float deltaTime;

	private bool itemEnable;

	public GameObject wingEffect;

	public string firstPersonAttachBone;

	public string thirdPersonAttachBone;

	public AudioClip itemStartSound;

	private void Awake()
	{
	}

	private void Update()
	{
		if (itemEnable)
		{
			deltaTime += Time.deltaTime;
			if (deltaTime > 10f)
			{
				enableWingItem(enable: false);
			}
		}
	}

	public override void StartItem()
	{
		enableWingItem(enable: true);
		deltaTime = 0f;
		if (IsMyItem())
		{
			GameObject gameObject = GameObject.Find("Me");
			if (null != gameObject)
			{
				AudioSource component = gameObject.GetComponent<AudioSource>();
				if (component != null)
				{
					component.PlayOneShot(itemStartSound);
				}
			}
		}
	}

	private void enableWingItem(bool enable)
	{
		itemEnable = enable;
		if (IsMyItem())
		{
			GameObject gameObject = GameObject.Find("Me");
			if (null != gameObject)
			{
				EquipCoordinator component = gameObject.GetComponent<EquipCoordinator>();
				if (component != null)
				{
					component.enableWingEffect(enable, this);
				}
			}
		}
		else
		{
			GameObject gameObject2 = BrickManManager.Instance.Get(useUserSeq);
			if (gameObject2 != null)
			{
				LookCoordinator component2 = gameObject2.GetComponent<LookCoordinator>();
				if (component2 != null)
				{
					component2.enableWingEffect(enable, this);
				}
			}
		}
	}
}
