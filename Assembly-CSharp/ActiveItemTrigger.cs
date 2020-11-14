using UnityEngine;

public class ActiveItemTrigger : MonoBehaviour
{
	public int seq;

	private void OnTriggerEnter(Collider other)
	{
		if (!Application.loadedLevelName.Contains("MapEditor"))
		{
			LocalController component = other.GetComponent<LocalController>();
			if (component != null)
			{
				ActiveItemManager.Instance.EatItem(seq, MyInfoManager.Instance.Seq);
			}
		}
	}
}
