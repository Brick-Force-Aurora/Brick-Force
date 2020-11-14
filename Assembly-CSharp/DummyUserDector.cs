using UnityEngine;

public class DummyUserDector : MonoBehaviour
{
	private enum USER_TYPE
	{
		ACTIVE_USER,
		LAZY_USER
	}

	private float dummyTime;

	private float timeout = 3600f;

	private USER_TYPE userType;

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
		Clear();
	}

	private void Clear()
	{
		userType = USER_TYPE.ACTIVE_USER;
		dummyTime = 0f;
	}

	private void Update()
	{
		if (MyInfoManager.Instance.Seq > 0 && BuffManager.Instance.netCafeCode == 0 && CSNetManager.Instance.Sock != null && CSNetManager.Instance.Sock.IsConnected())
		{
			dummyTime += Time.deltaTime;
			if (Input.anyKeyDown)
			{
				Clear();
			}
			if (userType == USER_TYPE.ACTIVE_USER && Mathf.Abs(timeout - dummyTime) < 60f)
			{
				userType = USER_TYPE.LAZY_USER;
				SystemMsgManager.Instance.ShowMessage(StringMgr.Instance.Get("SHUTDOWN_DUMMY_USER_SOON"));
			}
			if (dummyTime >= timeout)
			{
				Clear();
				GlobalVars.Instance.shutdownNow = true;
				CSNetManager.Instance.Sock.SendCS_LOGOUT_REQ();
				CSNetManager.Instance.Sock.Close();
				P2PManager.Instance.Shutdown();
				MessageBoxMgr.Instance.AddQuitMesssge(StringMgr.Instance.Get("SHUTDOWN_DUMMY_USER"));
			}
		}
	}
}
