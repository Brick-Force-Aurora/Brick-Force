using UnityEngine;

public class WindowUtil : MonoBehaviour
{
	public static void EatEvent()
	{
		if (Event.current.type != EventType.Layout && Event.current.type != EventType.Repaint)
		{
			Event.current.Use();
		}
	}
}
