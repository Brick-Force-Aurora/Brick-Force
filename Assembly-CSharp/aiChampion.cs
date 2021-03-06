using UnityEngine;

public class aiChampion : MonAI
{
	public override void changeTexture()
	{
		Defense component = GameObject.Find("Main").GetComponent<Defense>();
		if (component != null)
		{
			MonProperty component2 = GetComponent<MonProperty>();
			smr = GetComponentInChildren<SkinnedMeshRenderer>();
			if (null == smr)
			{
				Debug.LogError("Fail to get skinned mesh renderer for flags");
			}
			if (component2.Desc.bRedTeam)
			{
				((Component)smr).renderer.material.mainTexture = component.texChampionRed;
			}
			else
			{
				((Component)smr).renderer.material.mainTexture = component.texChampionBlue;
			}
		}
	}
}
