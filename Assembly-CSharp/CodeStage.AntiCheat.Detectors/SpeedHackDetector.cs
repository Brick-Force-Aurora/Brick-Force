using System;
using UnityEngine;

namespace CodeStage.AntiCheat.Detectors
{
	[AddComponentMenu("")]
	public class SpeedHackDetector : MonoBehaviour
	{
		private const string COMPONENT_NAME = "Speed Hack Detector";

		private const int THRESHOLD = 5000000;

		private static SpeedHackDetector instance;

		public bool autoDispose = true;

		public bool keepAlive = true;

		public Action onSpeedHackDetected;

		public float interval = 1f;

		public byte maxFalsePositives = 3;

		private int errorsCount;

		private long ticksOnStart;

		private long ticksOnStartVulnerable;

		private bool running;

		public static SpeedHackDetector Instance
		{
			get
			{
				if (instance == null)
				{
					SpeedHackDetector speedHackDetector = (SpeedHackDetector)UnityEngine.Object.FindObjectOfType(typeof(SpeedHackDetector));
					if (speedHackDetector == null)
					{
						GameObject gameObject = new GameObject("Speed Hack Detector");
						speedHackDetector = gameObject.AddComponent<SpeedHackDetector>();
					}
					return speedHackDetector;
				}
				return instance;
			}
		}

		private SpeedHackDetector()
		{
		}

		public static void Dispose()
		{
			Instance.DisposeInternal();
		}

		private void DisposeInternal()
		{
			StopMonitoringInternal();
			instance = null;
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private void Awake()
		{
			if (!IsPlacedCorrectly())
			{
				Debug.LogWarning("[ACT] Speed Hack Detector is placed in scene incorrectly and will be auto-destroyed! Please, use \"GameObject->Create Other->Code Stage->Anti-Cheat Toolkit->Speed Hack Detector\" menu to correct this!");
				UnityEngine.Object.Destroy(this);
			}
			else
			{
				instance = this;
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
		}

		private bool IsPlacedCorrectly()
		{
			return base.name == "Speed Hack Detector" && GetComponentsInChildren<Component>().Length == 2 && base.transform.childCount == 0;
		}

		private void OnLevelWasLoaded(int index)
		{
			if (!keepAlive)
			{
				Dispose();
			}
		}

		private void OnDisable()
		{
			StopMonitoringInternal();
		}

		private void OnApplicationQuit()
		{
			DisposeInternal();
		}

		public static void StartDetection(Action callback)
		{
			StartDetection(callback, Instance.interval);
		}

		public static void StartDetection(Action callback, float checkInterval)
		{
			StartDetection(callback, checkInterval, Instance.maxFalsePositives);
		}

		public static void StartDetection(Action callback, float checkInterval, byte maxErrors)
		{
			if (Instance.running)
			{
				Debug.LogWarning("[ACT] Speed Hack Detector already running!");
			}
			else
			{
				Instance.StartDetectionInternal(callback, checkInterval, maxErrors);
			}
		}

		private void StartDetectionInternal(Action callback, float checkInterval, byte maxErrors)
		{
			onSpeedHackDetected = callback;
			interval = checkInterval;
			maxFalsePositives = maxErrors;
			ticksOnStart = DateTime.UtcNow.Ticks;
			ticksOnStartVulnerable = (long)Environment.TickCount * 10000L;
			InvokeRepeating("OnTimer", checkInterval, checkInterval);
			errorsCount = 0;
			running = true;
		}

		public static void StopMonitoring()
		{
			Instance.StopMonitoringInternal();
		}

		private void StopMonitoringInternal()
		{
			if (running)
			{
				CancelInvoke("OnTimer");
				onSpeedHackDetected = null;
				running = false;
			}
		}

		private void OnTimer()
		{
			long num = 0L;
			long num2 = 0L;
			num = DateTime.UtcNow.Ticks;
			num2 = (long)Environment.TickCount * 10000L;
			if (Mathf.Abs((float)(num2 - ticksOnStartVulnerable - (num - ticksOnStart))) > 5000000f)
			{
				errorsCount++;
				Debug.LogWarning("[ACT] SpeedHackDetector: detection! Silent detections left: " + (maxFalsePositives - errorsCount));
				if (errorsCount > maxFalsePositives)
				{
					if (onSpeedHackDetected != null)
					{
						onSpeedHackDetected();
					}
					if (autoDispose)
					{
						Dispose();
					}
					else
					{
						StopMonitoring();
					}
				}
			}
		}
	}
}
