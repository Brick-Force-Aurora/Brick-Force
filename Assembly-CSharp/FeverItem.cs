using UnityEngine;

public class FeverItem : ActiveItemBase
{
	public AudioClip sndUseItem;

	private void Awake()
	{
	}

	private void Update()
	{
	}

	public override void StartItem()
	{
		if (useUserSeq == MyInfoManager.Instance.Seq)
		{
			GlobalVars.Instance.activeFeverMode();
			GameObject gameObject = GameObject.Find("Me");
			if (null != gameObject)
			{
				AudioSource component = gameObject.GetComponent<AudioSource>();
				if (component != null)
				{
					component.PlayOneShot(sndUseItem);
				}
			}
		}
	}
}
