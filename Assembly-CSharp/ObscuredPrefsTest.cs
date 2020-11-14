using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class ObscuredPrefsTest : MonoBehaviour
{
	public string encryptionKey = "change me!";

	internal string gameData = string.Empty;

	private void OnApplicationQuit()
	{
		PlayerPrefs.DeleteKey("money");
		PlayerPrefs.DeleteKey("lifeBar");
		PlayerPrefs.DeleteKey("playerName");
		ObscuredPrefs.DeleteKey("money");
		ObscuredPrefs.DeleteKey("lifeBar");
		ObscuredPrefs.DeleteKey("playerName");
		ObscuredPrefs.DeleteKey("gameComplete");
		ObscuredPrefs.DeleteKey("demoLong");
		ObscuredPrefs.DeleteKey("demoDouble");
		ObscuredPrefs.DeleteKey("demoByteArray");
		ObscuredPrefs.DeleteKey("demoVector3");
	}

	private void Awake()
	{
		ObscuredPrefs.SetNewCryptoKey(encryptionKey);
	}

	public void SaveGame(bool obscured)
	{
		if (obscured)
		{
			ObscuredPrefs.SetInt("money", 1500);
			ObscuredPrefs.SetFloat("lifeBar", 25.9f);
			ObscuredPrefs.SetString("playerName", "focus xD");
			ObscuredPrefs.SetBool("gameComplete", value: true);
			ObscuredPrefs.SetLong("demoLong", 3457657543456775432L);
			ObscuredPrefs.SetDouble("demoDouble", 345765.13123156782);
			ObscuredPrefs.SetByteArray("demoByteArray", new byte[4]
			{
				44,
				104,
				43,
				32
			});
			ObscuredPrefs.SetVector3("demoVector3", new Vector3(123.312f, 453.123444f, 1223f));
			Debug.Log("Game saved using ObscuredPrefs. Try to find and change saved data now! ;)");
		}
		else
		{
			PlayerPrefs.SetInt("money", 2100);
			PlayerPrefs.SetFloat("lifeBar", 88.4f);
			PlayerPrefs.SetString("playerName", "focus :D");
			Debug.Log("Game saved with regular PlayerPrefs. Try to find and change saved data now (it's easy)!");
		}
		ObscuredPrefs.Save();
	}

	public void ReadSavedGame(bool obscured)
	{
		if (obscured)
		{
			gameData = "Money: " + ObscuredPrefs.GetInt("money") + "\n";
			string text = gameData;
			gameData = text + "Life bar: " + ObscuredPrefs.GetFloat("lifeBar") + "\n";
			gameData = gameData + "Player name: " + ObscuredPrefs.GetString("playerName") + "\n";
			text = gameData;
			gameData = text + "bool: " + ObscuredPrefs.GetBool("gameComplete") + "\n";
			text = gameData;
			gameData = text + "long: " + ObscuredPrefs.GetLong("demoLong") + "\n";
			text = gameData;
			gameData = text + "double: " + ObscuredPrefs.GetDouble("demoDouble") + "\n";
			byte[] byteArray = ObscuredPrefs.GetByteArray("demoByteArray", 0, 4);
			text = gameData;
			gameData = text + "Vector3: " + ObscuredPrefs.GetVector3("demoVector3") + "\n";
			text = gameData;
			gameData = text + "byte[]: {" + byteArray[0] + "," + byteArray[1] + "," + byteArray[2] + "," + byteArray[3] + "}";
		}
		else
		{
			gameData = "Money: " + PlayerPrefs.GetInt("money") + "\n";
			string text = gameData;
			gameData = text + "Life bar: " + PlayerPrefs.GetFloat("lifeBar") + "\n";
			gameData = gameData + "Player name: " + PlayerPrefs.GetString("playerName");
		}
	}
}
