using UnityEngine;

public class CheckMonDead : MonoBehaviour
{
	private int idMon = -1;

	public int MonID
	{
		get
		{
			return idMon;
		}
		set
		{
			idMon = value;
		}
	}

	private void Update()
	{
		if (idMon >= 0)
		{
			MonDesc desc = MonManager.Instance.GetDesc(idMon);
			if (desc == null)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
