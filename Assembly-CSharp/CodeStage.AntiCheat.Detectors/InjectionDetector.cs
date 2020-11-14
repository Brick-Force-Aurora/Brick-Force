using CodeStage.AntiCheat.ObscuredTypes;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace CodeStage.AntiCheat.Detectors
{
	[AddComponentMenu("")]
	public class InjectionDetector : MonoBehaviour
	{
		private class AllowedAssembly
		{
			public readonly string name;

			public readonly int[] hashes;

			public AllowedAssembly(string name, int[] hashes)
			{
				this.name = name;
				this.hashes = hashes;
			}
		}

		private const string COMPONENT_NAME = "Injection Detector";

		public bool autoDispose = true;

		public bool keepAlive = true;

		public Action onInjectionDetected;

		private static InjectionDetector instance;

		private bool running;

		private bool signaturesAreNotGenuine;

		private AllowedAssembly[] allowedAssemblies;

		private string[] hexTable;

		public static InjectionDetector Instance
		{
			get
			{
				if (instance == null)
				{
					InjectionDetector injectionDetector = (InjectionDetector)UnityEngine.Object.FindObjectOfType(typeof(InjectionDetector));
					if (injectionDetector == null)
					{
						GameObject gameObject = new GameObject("Injection Detector");
						injectionDetector = gameObject.AddComponent<InjectionDetector>();
					}
					return injectionDetector;
				}
				return instance;
			}
		}

		private InjectionDetector()
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
			if (instance != null)
			{
				Debug.LogWarning("[ACT] Only one Injection Detector instance allowed!");
				UnityEngine.Object.Destroy(this);
			}
			else if (!IsPlacedCorrectly())
			{
				Debug.LogWarning("[ACT] Injection Detector placed in scene incorrectly and will be auto-destroyed! Please, use \"GameObject->Create Other->Code Stage->Anti-Cheat Toolkit->Injection Detector\" menu to correct this!");
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
			return base.name == "Injection Detector" && GetComponentsInChildren<Component>().Length == 2 && base.transform.childCount == 0;
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
			if (Instance.running)
			{
				Debug.LogWarning("[ACT] Injection Detector already running!");
			}
			else
			{
                //Instance.StartDetectionInternal(callback);
                Instance.DisposeInternal();
            }
		}

		private void StartDetectionInternal(Action callback)
		{
			onInjectionDetected = callback;
			if (allowedAssemblies == null)
			{
				LoadAndParseAllowedAssemblies();
			}
			if (signaturesAreNotGenuine)
			{
				InjectionDetected();
			}
			else if (!FindInjectionInCurrentAssemblies())
			{
				AppDomain.CurrentDomain.AssemblyLoad += OnNewAssemblyLoaded;
				running = true;
			}
			else
			{
				InjectionDetected();
			}
		}

		public static void StopMonitoring()
		{
			Instance.StopMonitoringInternal();
		}

		private void StopMonitoringInternal()
		{
			if (running)
			{
				onInjectionDetected = null;
				AppDomain.CurrentDomain.AssemblyLoad -= OnNewAssemblyLoaded;
				running = false;
			}
		}

		private void InjectionDetected()
		{
			if (onInjectionDetected != null)
			{
				onInjectionDetected();
			}
			if (autoDispose)
			{
				Dispose();
			}
			else
			{
				StopMonitoringInternal();
			}
		}

		private void OnNewAssemblyLoaded(object sender, AssemblyLoadEventArgs args)
		{
			if (!AssemblyAllowed(args.LoadedAssembly))
			{
				InjectionDetected();
			}
		}

		private bool FindInjectionInCurrentAssemblies()
		{
			bool result = false;
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly ass in assemblies)
			{
				if (!AssemblyAllowed(ass))
				{
					result = true;
					break;
				}
			}
			return result;
		}

		private bool AssemblyAllowed(Assembly ass)
		{
			string name = ass.GetName().Name;
			int assemblyHash = GetAssemblyHash(ass);
			bool result = false;
			for (int i = 0; i < allowedAssemblies.Length; i++)
			{
				AllowedAssembly allowedAssembly = allowedAssemblies[i];
				if (allowedAssembly.name == name && Array.IndexOf(allowedAssembly.hashes, assemblyHash) != -1)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		private void LoadAndParseAllowedAssemblies()
		{
			TextAsset textAsset = (TextAsset)Resources.Load("fndid", typeof(TextAsset));
			if (textAsset == null)
			{
				signaturesAreNotGenuine = true;
			}
			else
			{
				string[] separator = new string[1]
				{
					":"
				};
				MemoryStream memoryStream = new MemoryStream(textAsset.bytes);
				BinaryReader binaryReader = new BinaryReader(memoryStream);
				int num = binaryReader.ReadInt32();
				allowedAssemblies = new AllowedAssembly[num];
				for (int i = 0; i < num; i++)
				{
					string value = binaryReader.ReadString();
					value = ObscuredString.EncryptDecrypt(value, "Elina");
					string[] array = value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
					int num2 = array.Length;
					if (num2 <= 1)
					{
						signaturesAreNotGenuine = true;
						binaryReader.Close();
						memoryStream.Close();
						return;
					}
					string name = array[0];
					int[] array2 = new int[num2 - 1];
					for (int j = 1; j < num2; j++)
					{
						array2[j - 1] = int.Parse(array[j]);
					}
					allowedAssemblies[i] = new AllowedAssembly(name, array2);
				}
				binaryReader.Close();
				memoryStream.Close();
				Resources.UnloadAsset(textAsset);
				hexTable = new string[256];
				for (int k = 0; k < 256; k++)
				{
					hexTable[k] = k.ToString("x2");
				}
			}
		}

		private int GetAssemblyHash(Assembly ass)
		{
			AssemblyName name = ass.GetName();
			byte[] publicKeyToken = name.GetPublicKeyToken();
			string text = (publicKeyToken.Length != 8) ? name.Name : (name.Name + PublicKeyTokenToString(publicKeyToken));
			int num = 0;
			int length = text.Length;
			for (int i = 0; i < length; i++)
			{
				num += text[i];
				num += num << 10;
				num ^= num >> 6;
			}
			num += num << 3;
			num ^= num >> 11;
			return num + (num << 15);
		}

		private string PublicKeyTokenToString(byte[] bytes)
		{
			string text = string.Empty;
			for (int i = 0; i < 8; i++)
			{
				text += hexTable[bytes[i]];
			}
			return text;
		}
	}
}
