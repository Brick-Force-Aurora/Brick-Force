using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class ObscuredStringTest : MonoBehaviour
{
	internal string cleanString;

	internal ObscuredString obscuredString;

	internal bool useRegular;

	private void Start()
	{
		Debug.Log("===== ObscuredStringTest =====\n");
		ObscuredString.SetNewCryptoKey("I LOVE MY GIRL");
		cleanString = "Try Goscurry! Or better buy it!";
		Debug.Log("Original string:\n" + cleanString);
		obscuredString = cleanString;
		Debug.Log("How your string is stored in memory when obscured:\n" + obscuredString.GetEncrypted());
		obscuredString = (cleanString = string.Empty);
	}

	public void UseRegular()
	{
		useRegular = true;
		cleanString = "Hey, you can easily change me in memory!";
		obscuredString = string.Empty;
		Debug.Log("Try to change this string in memory:\n" + cleanString);
	}

	public void UseObscured()
	{
		useRegular = false;
		obscuredString = "Hey, you can't change me in memory!";
		cleanString = string.Empty;
		Debug.Log("Try to change this string in memory:\n" + obscuredString);
	}
}
