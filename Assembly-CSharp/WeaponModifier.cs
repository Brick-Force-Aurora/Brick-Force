using System.Collections.Generic;
using UnityEngine;

public class WeaponModifier : MonoBehaviour
{
	private Dictionary<int, WpnMod> dic;

	private Dictionary<int, WpnModEx> dicEx;

	public static WeaponModifier _instance;

	public static WeaponModifier Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(WeaponModifier)) as WeaponModifier);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the WeaponModifier Instance");
				}
			}
			
			return _instance;
		}
	}

	public WpnMod Get(int weaponBy)
	{
		if (dic.ContainsKey(weaponBy))
		{
			return dic[weaponBy];
		}
		return null;
	}

	public void UpdateWpnMod(int nSeq, float fReloadSpeed, float fDrawSpeed, float fRange, float fSpeedFactor, float fAtkPow, float fRigidity, float fRateOfFire, float fRecoilPitch, float fThrowForce, float fAccuracy, float fAccurateMin, float fAccurateMax, float fInaccurateMin, float fInaccurateMax, float fAccurateSpread, float fAccurateCenter, float fInaccurateSpread, float fInaccurateCenter, float fMoveInaccuracyFactor, float fZAccuracy, float fZAccurateMin, float fZAccurateMax, float fZInaccurateMin, float fZInaccurateMax, float fZAccurateSpread, float fZAccurateCenter, float fZInaccurateSpread, float fZInaccurateCenter, float fZMoveInaccuracyFactor, float fZFov, float fZCamSpeed, float fSlashSpeed, int maxAmmo, int maxMagazine, float fExplosionTime, float fEffectiveRange, float fRecoilYaw, float brokenRatio, float radius)
	{
		if (!dic.ContainsKey(nSeq))
		{
			dic.Add(nSeq, new WpnMod());
		}
		if (dic.ContainsKey(nSeq))
		{
			dic[nSeq].nSeq = nSeq;
			dic[nSeq].fReloadSpeed = ((!(fReloadSpeed < 0f)) ? fReloadSpeed : float.PositiveInfinity);
			dic[nSeq].fDrawSpeed = ((!(fDrawSpeed < 0f)) ? fDrawSpeed : float.PositiveInfinity);
			dic[nSeq].fRange = ((!(fRange < 0f)) ? fRange : float.PositiveInfinity);
			dic[nSeq].fSpeedFactor = ((!(fSpeedFactor < 0f)) ? fSpeedFactor : float.PositiveInfinity);
			dic[nSeq].fAtkPow = ((!(fAtkPow < 0f)) ? fAtkPow : float.PositiveInfinity);
			dic[nSeq].fRigidity = ((!(fRigidity < 0f)) ? fRigidity : float.PositiveInfinity);
			dic[nSeq].fRateOfFire = ((!(fRateOfFire < 0f)) ? fRateOfFire : float.PositiveInfinity);
			dic[nSeq].fRecoilPitch = ((!(fRecoilPitch < 0f)) ? fRecoilPitch : float.PositiveInfinity);
			dic[nSeq].fThrowForce = ((!(fThrowForce < 0f)) ? fThrowForce : float.PositiveInfinity);
			dic[nSeq].fAccuracy = ((!(fAccuracy < 0f)) ? fAccuracy : float.PositiveInfinity);
			dic[nSeq].fAccurateMin = ((!(fAccurateMin < 0f)) ? fAccurateMin : float.PositiveInfinity);
			dic[nSeq].fAccurateMax = ((!(fAccurateMax < 0f)) ? fAccurateMax : float.PositiveInfinity);
			dic[nSeq].fInaccurateMin = ((!(fInaccurateMin < 0f)) ? fInaccurateMin : float.PositiveInfinity);
			dic[nSeq].fInaccurateMax = ((!(fInaccurateMax < 0f)) ? fInaccurateMax : float.PositiveInfinity);
			dic[nSeq].fAccurateSpread = ((!(fAccurateSpread < 0f)) ? fAccurateSpread : float.PositiveInfinity);
			dic[nSeq].fAccurateCenter = ((!(fAccurateCenter < 0f)) ? fAccurateCenter : float.PositiveInfinity);
			dic[nSeq].fInaccurateSpread = ((!(fInaccurateSpread < 0f)) ? fInaccurateSpread : float.PositiveInfinity);
			dic[nSeq].fInaccurateCenter = ((!(fInaccurateCenter < 0f)) ? fInaccurateCenter : float.PositiveInfinity);
			dic[nSeq].fMoveInaccuracyFactor = ((!(fMoveInaccuracyFactor < 0f)) ? fMoveInaccuracyFactor : float.PositiveInfinity);
			dic[nSeq].fZAccuracy = ((!(fZAccuracy < 0f)) ? fZAccuracy : float.PositiveInfinity);
			dic[nSeq].fZAccurateMin = ((!(fZAccurateMin < 0f)) ? fZAccurateMin : float.PositiveInfinity);
			dic[nSeq].fZAccurateMax = ((!(fZAccurateMax < 0f)) ? fZAccurateMax : float.PositiveInfinity);
			dic[nSeq].fZInaccurateMin = ((!(fZInaccurateMin < 0f)) ? fZInaccurateMin : float.PositiveInfinity);
			dic[nSeq].fZInaccurateMax = ((!(fZInaccurateMax < 0f)) ? fZInaccurateMax : float.PositiveInfinity);
			dic[nSeq].fZAccurateSpread = ((!(fZAccurateSpread < 0f)) ? fZAccurateSpread : float.PositiveInfinity);
			dic[nSeq].fZAccurateCenter = ((!(fZAccurateCenter < 0f)) ? fZAccurateCenter : float.PositiveInfinity);
			dic[nSeq].fZInaccurateSpread = ((!(fZInaccurateSpread < 0f)) ? fZInaccurateSpread : float.PositiveInfinity);
			dic[nSeq].fZInaccurateCenter = ((!(fZInaccurateCenter < 0f)) ? fZInaccurateCenter : float.PositiveInfinity);
			dic[nSeq].fZMoveInaccuracyFactor = ((!(fZMoveInaccuracyFactor < 0f)) ? fZMoveInaccuracyFactor : float.PositiveInfinity);
			dic[nSeq].fZFov = ((!(fZFov < 0f)) ? fZFov : float.PositiveInfinity);
			dic[nSeq].fZCamSpeed = ((!(fZCamSpeed < 0f)) ? fZCamSpeed : float.PositiveInfinity);
			dic[nSeq].fSlashSpeed = ((!(fSlashSpeed < 0f)) ? fSlashSpeed : float.PositiveInfinity);
			dic[nSeq].maxAmmo = ((maxAmmo >= 0) ? maxAmmo : 0);
			dic[nSeq].maxMagazine = ((maxMagazine >= 0) ? maxMagazine : 0);
			dic[nSeq].explosionTime = ((!(fExplosionTime < 0f)) ? fExplosionTime : float.PositiveInfinity);
			dic[nSeq].effectiveRange = ((!(fEffectiveRange < 0f)) ? Mathf.Min(fEffectiveRange, fRange) : float.PositiveInfinity);
			dic[nSeq].recoilYaw = fRecoilYaw;
			dic[nSeq].brokenRatio = brokenRatio;
			dic[nSeq].radius = ((!(radius < 0f)) ? radius : float.PositiveInfinity);
		}
	}

	public WpnModEx GetEx(int weaponBy)
	{
		if (dicEx.ContainsKey(weaponBy))
		{
			return dicEx[weaponBy];
		}
		return null;
	}

	public void UpdateWpnModEx(int nSeq, float misSpeed, float throwForce, int maxLauncherAmmo, float radius2ndWpn, int damage2ndWpn, float recoilPitch2ndWpn, float recoilYaw2ndWpn, float Radius1stWpn, int semiAutoMaxCyclicAmmo, int minBuckShot, int maxBuckShot, float persistTime, float continueTime)
	{
		if (!dicEx.ContainsKey(nSeq))
		{
			dicEx.Add(nSeq, new WpnModEx());
		}
		if (dicEx.ContainsKey(nSeq))
		{
			dicEx[nSeq].nSeq = nSeq;
			dicEx[nSeq].misSpeed = ((!(misSpeed < 0f)) ? misSpeed : float.PositiveInfinity);
			dicEx[nSeq].throwForce = ((!(throwForce < 0f)) ? throwForce : float.PositiveInfinity);
			dicEx[nSeq].maxLauncherAmmo = ((maxLauncherAmmo >= 0) ? maxLauncherAmmo : 0);
			dicEx[nSeq].radius2ndWpn = ((!(radius2ndWpn < 0f)) ? radius2ndWpn : float.PositiveInfinity);
			dicEx[nSeq].damage2ndWpn = ((damage2ndWpn >= 0) ? damage2ndWpn : 0);
			dicEx[nSeq].recoilPitch2ndWpn = ((!(recoilPitch2ndWpn < 0f)) ? recoilPitch2ndWpn : float.PositiveInfinity);
			dicEx[nSeq].recoilYaw2ndWpn = recoilYaw2ndWpn;
			dicEx[nSeq].Radius1stWpn = ((!(Radius1stWpn < 0f)) ? Radius1stWpn : float.PositiveInfinity);
			dicEx[nSeq].semiAutoMaxCyclicAmmo = ((semiAutoMaxCyclicAmmo >= 0) ? semiAutoMaxCyclicAmmo : 0);
			dicEx[nSeq].minBuckShot = ((minBuckShot >= 0) ? minBuckShot : 0);
			dicEx[nSeq].maxBuckShot = ((maxBuckShot >= 0) ? maxBuckShot : 0);
			dicEx[nSeq].persistTime = ((!(persistTime < 0f)) ? persistTime : float.PositiveInfinity);
			dicEx[nSeq].continueTime = ((!(continueTime < 0f)) ? continueTime : float.PositiveInfinity);
		}
	}

	public void Awake()
	{
		dic = new Dictionary<int, WpnMod>();
		dicEx = new Dictionary<int, WpnModEx>();
		Object.DontDestroyOnLoad(this);
	}

	public void Clear()
	{
		dic.Clear();
		dicEx.Clear();
	}
}
