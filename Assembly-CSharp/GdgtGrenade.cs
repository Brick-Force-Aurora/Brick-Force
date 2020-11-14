using System.Collections.Generic;
using UnityEngine;

public class GdgtGrenade : WeaponGadget
{
	private Dictionary<int, ProjectileWrap> dic;

	public GameObject selfExpolsion;

	private void Start()
	{
		if (BuildOption.Instance.Props.useUskWeaponTex && applyUsk)
		{
			MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				if (meshRenderer.material.mainTexture != null && UskManager.Instance.Get(meshRenderer.material.mainTexture.name) != null)
				{
					meshRenderer.material.mainTexture = UskManager.Instance.Get(meshRenderer.material.mainTexture.name);
				}
			}
		}
		dic = new Dictionary<int, ProjectileWrap>();
	}

	public void EnableHandbomb(bool enable)
	{
		MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
		MeshRenderer[] array = componentsInChildren;
		foreach (MeshRenderer meshRenderer in array)
		{
			meshRenderer.enabled = enable;
		}
		SkinnedMeshRenderer[] componentsInChildren2 = GetComponentsInChildren<SkinnedMeshRenderer>();
		SkinnedMeshRenderer[] array2 = componentsInChildren2;
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in array2)
		{
			skinnedMeshRenderer.enabled = enable;
		}
	}

	private void OnDisable()
	{
		if (dic != null)
		{
			foreach (KeyValuePair<int, ProjectileWrap> item in dic)
			{
				Object.DestroyImmediate(item.Value.projectile);
			}
			dic.Clear();
		}
	}

	public Weapon.BY GetWeaponBY()
	{
		Weapon component = GetComponent<Weapon>();
		if (null == component)
		{
			return Weapon.BY.FALLOUT;
		}
		WeaponFunction component2 = component.GetComponent<WeaponFunction>();
		if (component2 == null)
		{
			return Weapon.BY.FALLOUT;
		}
		return component2.weaponBy;
	}

	public ProjectileWrap[] ToProjectileWrap()
	{
		if (dic == null || dic.Count == 0)
		{
			return null;
		}
		List<ProjectileWrap> list = new List<ProjectileWrap>();
		foreach (KeyValuePair<int, ProjectileWrap> item in dic)
		{
			list.Add(item.Value);
		}
		return list.ToArray();
	}

	public override void Throw(int index, Vector3 initPos, Vector3 pos, Vector3 rot, bool bSoundvoc, bool IsYang)
	{
		Weapon component = GetComponent<Weapon>();
		if (!(null == component) && dic != null && !dic.ContainsKey(index))
		{
			GameObject gameObject = Object.Instantiate((Object)component.BulletOrBody, initPos, Quaternion.Euler(rot)) as GameObject;
			if (null != gameObject)
			{
				if (BuildOption.Instance.Props.useUskWeaponTex && applyUsk)
				{
					MeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<MeshRenderer>();
					foreach (MeshRenderer meshRenderer in componentsInChildren)
					{
						if (meshRenderer.material.mainTexture != null && UskManager.Instance.Get(meshRenderer.material.mainTexture.name) != null)
						{
							meshRenderer.material.mainTexture = UskManager.Instance.Get(meshRenderer.material.mainTexture.name);
						}
					}
				}
				if (bSoundvoc)
				{
					WeaponFunction component2 = component.GetComponent<WeaponFunction>();
					if (component2 != null)
					{
						if (!IsYang)
						{
							if (component2.weaponBy == Weapon.BY.GRENADE || component2.weaponBy == Weapon.BY.KG400)
							{
								VoiceManager.Instance.Play0("BKG400_throw_5");
							}
							else if (component2.weaponBy == Weapon.BY.KG440)
							{
								VoiceManager.Instance.Play0("BKGS440_throw_2");
							}
							else if (component2.weaponBy == Weapon.BY.FLASHBANG)
							{
								VoiceManager.Instance.Play0("KG409_throw_3");
							}
							else
							{
								VoiceManager.Instance.Play0("BKG400_throw_5");
							}
						}
						else if (component2.weaponBy == Weapon.BY.GRENADE || component2.weaponBy == Weapon.BY.KG400)
						{
							VoiceManager.Instance.Play2("BKG400_throw_5");
						}
						else if (component2.weaponBy == Weapon.BY.KG440)
						{
							VoiceManager.Instance.Play2("BKGS440_throw_2");
						}
						else if (component2.weaponBy == Weapon.BY.FLASHBANG)
						{
							VoiceManager.Instance.Play2("KG409_throw_3");
						}
						else
						{
							VoiceManager.Instance.Play2("BKG400_throw_5");
						}
					}
				}
				Rigidbody component3 = gameObject.GetComponent<Rigidbody>();
				if (null != component3)
				{
					component3.isKinematic = true;
				}
				Projectile component4 = gameObject.GetComponent<Projectile>();
				if (null != component4)
				{
					component4.enabled = false;
				}
				dic.Add(index, new ProjectileWrap(gameObject));
			}
		}
	}

	public void SelfKaboom(Vector3 pos)
	{
		if (!BuildOption.Instance.IsNetmarble && !BuildOption.Instance.IsDeveloper)
		{
			if (!BuildOption.Instance.Props.useUskMuzzleEff || !applyUsk)
			{
				if (selfExpolsion != null)
				{
					Object.Instantiate((Object)selfExpolsion, pos, Quaternion.Euler(0f, 0f, 0f));
				}
			}
			else if (GlobalVars.Instance.explosionUsk != null)
			{
				Object.Instantiate((Object)GlobalVars.Instance.explosionUsk, pos, Quaternion.Euler(0f, 0f, 0f));
			}
		}
		else if (MyInfoManager.Instance.IsBelow12())
		{
			Object.Instantiate((Object)GlobalVars.Instance.selfExpolsion11, pos, Quaternion.Euler(0f, 0f, 0f));
		}
		else
		{
			Object.Instantiate((Object)selfExpolsion, pos, Quaternion.Euler(0f, 0f, 0f));
		}
	}

	public void Kaboom(int index)
	{
		if (dic != null && dic.Count != 0 && dic.ContainsKey(index))
		{
			if (dic[index].projectile != null)
			{
				GameObject explosion = dic[index].projectile.GetComponent<Projectile>().explosion;
				Vector3 position = dic[index].projectile.transform.position;
				if (!BuildOption.Instance.IsNetmarble && !BuildOption.Instance.IsDeveloper)
				{
					if (!BuildOption.Instance.Props.useUskMuzzleEff || !applyUsk)
					{
						if (explosion != null)
						{
							Object.Instantiate((Object)explosion, position, Quaternion.Euler(0f, 0f, 0f));
						}
					}
					else if (GlobalVars.Instance.explosionUsk != null)
					{
						Object.Instantiate((Object)GlobalVars.Instance.explosionUsk, position, Quaternion.Euler(0f, 0f, 0f));
					}
				}
				else if (explosion != null)
				{
					Object.Instantiate((Object)explosion, position, Quaternion.Euler(0f, 0f, 0f));
				}
				Object.DestroyImmediate(dic[index].projectile);
			}
			dic.Remove(index);
		}
	}

	public void LetProjectileFly()
	{
		if (dic != null)
		{
			foreach (KeyValuePair<int, ProjectileWrap> item in dic)
			{
				item.Value.Fly();
			}
		}
	}

	public void Fly(int index, Vector3 pos, Vector3 rot, float range)
	{
		if (dic != null && dic.ContainsKey(index))
		{
			dic[index].targetPos = pos;
			dic[index].targetRot = rot;
			dic[index].range = range;
		}
	}
}
