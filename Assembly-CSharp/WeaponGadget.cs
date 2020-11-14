using UnityEngine;

[RequireComponent(typeof(Weapon))]
public class WeaponGadget : MonoBehaviour
{
	public bool applyUsk = true;

	public bool ApplyUsk
	{
		get
		{
			return applyUsk;
		}
		set
		{
			applyUsk = value;
		}
	}

	public virtual void ClipOut()
	{
	}

	public virtual void ClipIn()
	{
	}

	public virtual void BoltUp()
	{
	}

	public virtual void FireAction()
	{
	}

	public virtual void GunAnim(int anim)
	{
	}

	public virtual void setFever(bool isOn)
	{
	}

	public virtual void Fire(int projectile, Vector3 origin, Vector3 direction)
	{
	}

	public virtual void Fire2(int ammoId, int launcher, Vector3 pos, Vector3 rot)
	{
	}

	public virtual void Fly(int projectile, Vector3 pos, Vector3 rot)
	{
	}

	public virtual void KaBoom(int projectile, Vector3 pos, Vector3 rot, bool viewColeff = false)
	{
	}

	public virtual void Throw(int projectile, Vector3 initPos, Vector3 pos, Vector3 rot, bool bSoundvoc, bool IsYang)
	{
	}

	public virtual void SetSenseBeam(int playerSlot, int projectile, Vector3 initPos, Vector3 pos, Vector3 normal, bool bSoundvoc)
	{
	}

	public virtual void Compose(bool isDel)
	{
	}

	public virtual void Install(bool install)
	{
	}
}
