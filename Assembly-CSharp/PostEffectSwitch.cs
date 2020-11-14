using UnityEngine;

public class PostEffectSwitch : MonoBehaviour
{
	private void Start()
	{
		BloomAndFlares component = GetComponent<BloomAndFlares>();
		if (null != component)
		{
			component.enabled = (QualitySettings.GetQualityLevel() >= 3);
		}
	}

	private void Update()
	{
	}
}
