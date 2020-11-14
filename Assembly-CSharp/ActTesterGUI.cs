using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class ActTesterGUI : MonoBehaviour
{
	private bool savesAlterationDetected;

	private int savesLock;

	private bool foreignSavesDetected;

	public ObscuredVector3Test obscuredVector3Test;

	public ObscuredFloatTest obscuredFloatTest;

	public ObscuredIntTest obscuredIntTest;

	public ObscuredStringTest obscuredStringTest;

	public ObscuredPrefsTest obscuredPrefsTest;

	private DetectorsUsageExample detectorsUsageExample;

	private void Awake()
	{
		ObscuredPrefs.onAlterationDetected = SavesAlterationDetected;
		ObscuredPrefs.onPossibleForeignSavesDetected = ForeignSavesDetected;
		detectorsUsageExample = (DetectorsUsageExample)Object.FindObjectOfType(typeof(DetectorsUsageExample));
	}

	private void SavesAlterationDetected()
	{
		savesAlterationDetected = true;
	}

	private void ForeignSavesDetected()
	{
		foreignSavesDetected = true;
	}

	private void OnGUI()
	{
		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();
		CenteredLabel("Memory cheating protection");
		GUILayout.Space(10f);
		if ((bool)obscuredStringTest && obscuredStringTest.enabled)
		{
			if (GUILayout.Button("Use regular string"))
			{
				obscuredStringTest.UseRegular();
			}
			if (GUILayout.Button("Use obscured string"))
			{
				obscuredStringTest.UseObscured();
			}
			string str = (!obscuredStringTest.useRegular) ? ((string)obscuredStringTest.obscuredString) : obscuredStringTest.cleanString;
			GUILayout.Label("Current string (try to change it!):\n" + str);
		}
		if ((bool)obscuredIntTest && obscuredIntTest.enabled)
		{
			GUILayout.Space(10f);
			if (GUILayout.Button("Use regular int (click to generate new number)"))
			{
				obscuredIntTest.UseRegular();
			}
			if (GUILayout.Button("Use ObscuredInt (click to generate new number)"))
			{
				obscuredIntTest.UseObscured();
			}
			int num = (!obscuredIntTest.useRegular) ? ((int)obscuredIntTest.obscuredLivesCount) : obscuredIntTest.cleanLivesCount;
			GUILayout.Label("Current lives count (try to change them!):\n" + num);
			if (obscuredIntTest.cheatingDetected)
			{
				GUILayout.Label("ObscuredInt cheating try detected!");
			}
		}
		if ((bool)obscuredFloatTest && obscuredFloatTest.enabled)
		{
			GUILayout.Space(10f);
			if (GUILayout.Button("Use regular float (click to generate new number)"))
			{
				obscuredFloatTest.UseRegular();
			}
			if (GUILayout.Button("Use ObscuredFloat (click to generate new number)"))
			{
				obscuredFloatTest.UseObscured();
			}
			float num2 = (!obscuredFloatTest.useRegular) ? ((float)obscuredFloatTest.obscuredHealthBar) : obscuredFloatTest.healthBar;
			GUILayout.Label("Current health bar (try to change it!):\n" + $"{num2:0.000}");
			if (obscuredFloatTest.cheatingDetected)
			{
				GUILayout.Label("ObscuredFloat cheating try detected!");
			}
		}
		if ((bool)obscuredVector3Test && obscuredVector3Test.enabled)
		{
			GUILayout.Space(10f);
			if (GUILayout.Button("Use regular Vector3 (click to generate new one)"))
			{
				obscuredVector3Test.UseRegular();
			}
			if (GUILayout.Button("Use ObscuredVector3 (click to generate new one)"))
			{
				obscuredVector3Test.UseObscured();
			}
			Vector3 vector = (!obscuredVector3Test.useRegular) ? ((Vector3)obscuredVector3Test.obscuredPlayerPosition) : obscuredVector3Test.playerPosition;
			GUILayout.Label("Current player position (try to change it!):\n" + vector);
		}
		GUILayout.Space(10f);
		GUILayout.EndVertical();
		GUILayout.Space(10f);
		GUILayout.BeginVertical();
		CenteredLabel("Saves cheating protection");
		GUILayout.Space(10f);
		if ((bool)obscuredPrefsTest && obscuredPrefsTest.enabled)
		{
			if (GUILayout.Button("Save game with regular PlayerPrefs!"))
			{
				obscuredPrefsTest.SaveGame(obscured: false);
			}
			if (GUILayout.Button("Read data saved with regular PlayerPrefs"))
			{
				obscuredPrefsTest.ReadSavedGame(obscured: false);
			}
			GUILayout.Space(10f);
			if (GUILayout.Button("Save game with ObscuredPrefs!"))
			{
				obscuredPrefsTest.SaveGame(obscured: true);
			}
			if (GUILayout.Button("Read data saved with ObscuredPrefs"))
			{
				obscuredPrefsTest.ReadSavedGame(obscured: true);
			}
			ObscuredPrefs.preservePlayerPrefs = GUILayout.Toggle(ObscuredPrefs.preservePlayerPrefs, "preservePlayerPrefs");
			ObscuredPrefs.emergencyMode = GUILayout.Toggle(ObscuredPrefs.emergencyMode, "emergencyMode");
			GUILayout.Label("LockToDevice level:");
			savesLock = GUILayout.SelectionGrid(savesLock, new string[3]
			{
				ObscuredPrefs.DeviceLockLevel.None.ToString(),
				ObscuredPrefs.DeviceLockLevel.Soft.ToString(),
				ObscuredPrefs.DeviceLockLevel.Strict.ToString()
			}, 3);
			ObscuredPrefs.lockToDevice = (ObscuredPrefs.DeviceLockLevel)savesLock;
			ObscuredPrefs.readForeignSaves = GUILayout.Toggle(ObscuredPrefs.readForeignSaves, "readForeignSaves");
			GUILayout.Label("PlayerPrefs: \n" + obscuredPrefsTest.gameData);
			if (savesAlterationDetected)
			{
				GUILayout.Label("Saves were altered! }:>");
			}
			if (foreignSavesDetected)
			{
				GUILayout.Label("Saves more likely from another device! }:>");
			}
		}
		if (detectorsUsageExample != null)
		{
			GUILayout.Label("Speed hack detected: " + detectorsUsageExample.speedHackDetected);
			GUILayout.Label("Injection detected: " + detectorsUsageExample.injectionDetected);
		}
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}

	private void CenteredLabel(string caption)
	{
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label(caption);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}
}
