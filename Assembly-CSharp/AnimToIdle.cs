using UnityEngine;

public class AnimToIdle : MonoBehaviour
{
	private Animation anim;

	private void Start()
	{
		anim = base.gameObject.GetComponentInChildren<Animation>();
		if (anim == null)
		{
			Debug.LogError("anim == null");
		}
	}

	private void Update()
	{
		if (!anim.IsPlaying("fire") && !anim.IsPlaying("idle"))
		{
			anim.CrossFade("idle");
		}
	}
}
