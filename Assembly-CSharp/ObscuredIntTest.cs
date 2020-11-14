using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class ObscuredIntTest : MonoBehaviour
{
	internal int cleanLivesCount = 11;

	internal ObscuredInt obscuredLivesCount = 11;

	internal bool useRegular;

	internal bool cheatingDetected;

	private void Start()
	{
		Debug.Log("===== ObscuredIntTest =====\n");
		cleanLivesCount = 5;
		Debug.Log("Original lives count:\n" + cleanLivesCount);
		obscuredLivesCount = cleanLivesCount;
		Debug.Log("How your lives count is stored in memory when obscured:\n" + obscuredLivesCount.GetEncrypted());
		ObscuredInt.SetNewCryptoKey(666);
		ObscuredInt value = 100;
		value = (int)value - 10;
		value = (int)value + 100;
		value = (int)value / 10;
		ObscuredInt.SetNewCryptoKey(888);
		value = ++value;
		ObscuredInt.SetNewCryptoKey(999);
		value = ++value;
		value = --value;
		Debug.Log("Lives count: " + value + " (" + value.ToString("X") + "h)");
		ObscuredInt.onCheatingDetected = OnCheatingDetected;
	}

	private void OnCheatingDetected()
	{
		Debug.Log("Cheating detected!");
		cheatingDetected = true;
	}

	public void UseRegular()
	{
		useRegular = true;
		cleanLivesCount += Random.Range(-10, 50);
		obscuredLivesCount = 11;
		Debug.Log("Try to change this int in memory:\n" + cleanLivesCount);
	}

	public void UseObscured()
	{
		useRegular = false;
		obscuredLivesCount = (int)obscuredLivesCount + Random.Range(-10, 50);
		cleanLivesCount = 11;
		Debug.Log("Try to change this int in memory:\n" + obscuredLivesCount);
	}
}
