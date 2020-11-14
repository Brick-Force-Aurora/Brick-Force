using System;
using UnityEngine;
using wlic;

public class XTrap : MonoBehaviour
{
	private float xtrapAliveTime;

	private static XTrap _instance;

	public static XTrap Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = (UnityEngine.Object.FindObjectOfType(typeof(XTrap)) as XTrap);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get XTrap Instance");
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		/*UnityEngine.Object.DontDestroyOnLoad(this);
		try
		{
			if (BuildOption.Instance.Props.UseXTrapTest)
			{
				WXCS_IF.XTrap_C_Start("660970B47839EBB5C5806D9844CFEF629C9B2D8AD273C1BE5D0B8117768FF54AC6C746C71A51B695767691E832A71E4B1859D9851BDDE2F260A23D686283B8A60F7D04245246E413A0661910D9427E0E7226C09C5F22FCF60F58C7A9E58C5829700DDDEF6ABC92DC39", null);
			}
			else if (BuildOption.Instance.Props.UseXTrapNetmarble)
			{
				WXCS_IF.XTrap_C_Start("660970B47839EBB5C5806D9844CFEF629C9B2D8AD273C1BE5D0B8117768FF54AC6C746C71A51B695767691E832A71E4B1859D9851BDDE2F260A23D686283B8A60F7D04245246E417A4610E5695527143EA62E1CC24C8DEA1DCD51A74E4550B71F4B98A85DBE32C9C69C8B6BFD33595F6", null);
			}
			else if (BuildOption.Instance.Props.UseXTrapInfernum)
			{
				string getRoundRobinServer = BuildOption.Instance.Props.GetRoundRobinServer;
				getRoundRobinServer = getRoundRobinServer.ToLower();
				if (getRoundRobinServer.Contains(".eu."))
				{
					WXCS_IF.XTrap_C_Start("660970B49738EBBBC5206D9844CFEF6276466602689764EFEFC7B705F31721D0699B7854FD0CE639A7A9317796D3DA2B7AAEEF571E0BBE780F2A4243ECA219810F7D04245246E404A17B435BCE1E751FE93256B7C063AB90595C326B3CF1809C9D86BF", null);
				}
				else if (getRoundRobinServer.Contains(".us."))
				{
					WXCS_IF.XTrap_C_Start("660970B49738EBBBC5206D9844CFEF6276466602689764EFEFC7B705F31721D0699B7854FD0CE639A7A9317796D3DA2B7AAEEF571E0BBE780F2A4243ECA219810F7D04245246E404A17B434BC81E751F42FA5B6896F90C04382BB5397C36F0078AA2D1", null);
				}
			}
			else if (BuildOption.Instance.Props.UseXTrapWave)
			{
				WXCS_IF.XTrap_C_Start("660970B4C939EBB5C5806D9844CFEF62345967B0B536A80026169237841FF27CF8EB03C973D20D28D2E03D802E9BC51B5AE248A1A1F38DC49631EF0D7889CAE60F7D04245246E41FB1670C4E955265042DCA5E1C17F78DC356C563C734AAAABCA96785E07E", null);
			}
			else if (BuildOption.Instance.Props.UseXTrapAxeso5)
			{
				WXCS_IF.XTrap_C_Start("660970B4963BEBB5C5156F9844CFEF62D0D8896A4B1F5B97DD36F1A5869CEF86004C3FEEBC5FA17FDE5709785957E79DC2A44E5F9F9A6EB62416DDECB13EE63A0F7D04245246E405A3710110DA48721ECD1FFE3F5B52A722F86C6370", null);
			}
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			Application.Quit();
		}*/
	}

	private void Start()
	{
		xtrapAliveTime = 0f;
		if (BuildOption.Instance.Props.UseXTrap)
		{
			WXCS_IF.XTrap_C_KeepAlive();
		}
	}

	private void Update()
	{
        xtrapAliveTime = 0f; //+= Time.deltaTime;
		if (BuildOption.Instance.Props.UseXTrap && xtrapAliveTime > 2f)
		{
			int period = Mathf.FloorToInt(xtrapAliveTime) * 1000;
			WXCS_IF.XTrap_C_CallbackAlive(period);
		}
	}

	public void SetUserInfo(string nickname)
	{
		if (BuildOption.Instance.Props.UseXTrap)
		{
			WXCS_IF.XTrap_C_SetUserInfoEx(string.Empty, string.Empty, nickname, string.Empty, 0, null);
		}
	}
}
