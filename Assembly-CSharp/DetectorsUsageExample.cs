using CodeStage.AntiCheat.Detectors;
using UnityEngine;

public class DetectorsUsageExample : MonoBehaviour
{
	[HideInInspector]
	public bool injectionDetected;

	[HideInInspector]
	public bool speedHackDetected;

	private void Start()
	{
		SpeedHackDetector.StartDetection(OnSpeedHackDetected, 1f, 5);
		InjectionDetector.Instance.autoDispose = true;
		InjectionDetector.Instance.keepAlive = true;
		InjectionDetector.StartDetection(OnInjectionDetected);
	}

	private void OnSpeedHackDetected()
	{
		speedHackDetected = true;
		Debug.LogWarning("Speed hack detected!");
	}

	private void OnInjectionDetected()
	{
		injectionDetected = true;
		Debug.LogWarning("Injection detected!");
	}
}
