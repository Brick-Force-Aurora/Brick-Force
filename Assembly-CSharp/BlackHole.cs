using UnityEngine;

public class BlackHole : MonoBehaviour
{
	public GameObject objBlackhole;

	public GameObject fxOn;

	private Vector3 posOn = Vector3.zero;

	private Vector3[] users;

	public void placeTo(Vector3 p)
	{
		Object.Instantiate((Object)objBlackhole, p, Quaternion.Euler(0f, 0f, 0f));
		posOn = p;
		makeUserPositions(p);
	}

	public void On()
	{
		Object.Instantiate((Object)fxOn, posOn, Quaternion.Euler(0f, 0f, 0f));
	}

	private void makeUserPositions(Vector3 p)
	{
		Vector3 vector = p;
		vector.y += 10f;
		users = new Vector3[8];
		for (int i = 0; i < 8; i++)
		{
			users[i] = vector;
		}
		users[0].z += 10f;
		users[1].x += 7f;
		users[1].z += 7f;
		users[2].x += 10f;
		users[3].x += 7f;
		users[3].z -= 7f;
		users[4].z -= 10f;
		users[5].x -= 7f;
		users[5].z -= 7f;
		users[6].x -= 10f;
		users[7].z -= 7f;
		users[7].z += 7f;
	}

	public Vector3 gotoPos(int id)
	{
		return users[id];
	}
}
