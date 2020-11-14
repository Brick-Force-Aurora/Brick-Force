using UnityEngine;

public class Shutgun : Gun
{
	public int minBuckShot = 3;

	public int maxBuckShot = 7;

	public int MinBuckShot
	{
		get
		{
			return minBuckShot;
		}
		set
		{
			minBuckShot = value;
		}
	}

	public int MaxBuckShot
	{
		get
		{
			return maxBuckShot;
		}
		set
		{
			maxBuckShot = value;
		}
	}

	protected override void Fire()
	{
		if (!IsCoolDown())
		{
			if (!magazine.Fire())
			{
				bool flag = true;
				if (CanReload())
				{
					GameObject gameObject = GameObject.Find("Main");
					ShooterTools component = gameObject.GetComponent<ShooterTools>();
					if (BuildOption.Instance.Props.useDefaultAutoReload || (null != component && component.Use("auto_reload")))
					{
						flag = false;
						localCtrl.AutoReload();
					}
				}
				if (flag)
				{
					base.animation.Play("empty");
					P2PManager.Instance.SendPEER_GUN_ANIM(MyInfoManager.Instance.Seq, (sbyte)GetComponent<Weapon>().slot, 1);
					EmptySound();
				}
				cyclic = false;
			}
			else
			{
				NoCheat.Instance.Sync(wdMagazine, magazine.Cur);
				localCtrl.DoFireAnimation(fireAnimation);
				DoFireAnimation("fire");
				FireSound();
				deltaTime = 0f;
				int num = Random.Range(minBuckShot, maxBuckShot);
				for (int i = 0; i < num; i++)
				{
					CreateMuzzleFire();
					Shoot();
				}
				camCtrl.Pitchup(recoilPitch, recoilYaw);
				GetComponent<Aim>().Inaccurate(localCtrl.CanAimAccuratelyMore());
				Scope component2 = GetComponent<Scope>();
				if (null != component2)
				{
					component2.HandleFireEvent(localCtrl.CanAimAccuratelyMore());
				}
			}
		}
	}

	protected override void Modify()
	{
		base.Modify();
		WpnModEx ex = WeaponModifier.Instance.GetEx((int)weaponBy);
		if (ex != null)
		{
			minBuckShot = ex.minBuckShot;
			maxBuckShot = ex.maxBuckShot;
		}
	}
}
