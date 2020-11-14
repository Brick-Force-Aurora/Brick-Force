using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class ObscuredVector3Test : MonoBehaviour
{
	internal Vector3 playerPosition = new Vector3(10.5f, 11.5f, 12.5f);

	internal ObscuredVector3 obscuredPlayerPosition = new Vector3(10.5f, 11.5f, 12.5f);

	internal bool useRegular = true;

	private void Start()
	{
		Debug.Log("===== ObscuredVector3Test =====\n");
		ObscuredVector3.SetNewCryptoKey(404);
		playerPosition = new Vector3(54.1f, 64.3f, 63.2f);
		Debug.Log("Original position:\n" + playerPosition);
		obscuredPlayerPosition = playerPosition;
		Vector3 encrypted = obscuredPlayerPosition.GetEncrypted();
		Debug.Log("How your position is stored in memory when obscured:\n(" + encrypted.x.ToString("0.000") + ", " + encrypted.y.ToString("0.000") + ", " + encrypted.z.ToString("0.000") + ")");
	}

	public void UseRegular()
	{
		useRegular = true;
		playerPosition += new Vector3(Random.Range(-10f, 50f), Random.Range(-10f, 50f), Random.Range(-10f, 50f));
		obscuredPlayerPosition = new Vector3(10.5f, 11.5f, 12.5f);
		Debug.Log("Try to change this Vector3 in memory:\n" + playerPosition);
	}

	public void UseObscured()
	{
		useRegular = false;
		obscuredPlayerPosition += new Vector3(Random.Range(-10f, 50f), Random.Range(-10f, 50f), Random.Range(-10f, 50f));
		playerPosition = new Vector3(10.5f, 11.5f, 12.5f);
		Debug.Log("Try to change this Vector3 in memory:\n" + obscuredPlayerPosition);
	}
}
