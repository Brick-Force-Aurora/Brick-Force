using UnityEngine;

public class EscapeGoalTrigger : MonoBehaviour
{
	private static bool isSendGoal;

	private void Start()
	{
		GoalSendReset();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!Application.loadedLevelName.Contains("MapEditor"))
		{
			LocalController component = other.GetComponent<LocalController>();
			if (component != null && !isSendGoal)
			{
				isSendGoal = true;
				CSNetManager.Instance.Sock.SendCS_RESPAWN_TICKET_REQ();
				CSNetManager.Instance.Sock.SendCS_ESCAPE_GOAL_REQ();
			}
		}
	}

	public static void GoalSendReset()
	{
		isSendGoal = false;
	}

	public static bool IsSendGoal()
	{
		return isSendGoal;
	}
}
