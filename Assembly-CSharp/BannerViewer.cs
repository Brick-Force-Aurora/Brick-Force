using System;
using UnityEngine;

[Serializable]
public class BannerViewer
{
	public Texture2D def;

	private float deltaFade;

	private Texture2D fadeTexture;

	private Lobby main;

	private int id = -1;

	private float deltaTimeChangeBanner;

	private float deltaTimeChangeBannerMax = 10f;

	private Rect crdBanner = new Rect(3f, 611f, 238f, 152f);

	public void Start()
	{
		fadeTexture = null;
		id = -1;
		if (BannerManager.Instance.Count() > 0)
		{
			id = 0;
		}
	}

	public void SetupMain(Lobby _main)
	{
		main = _main;
	}

	public void OnGUI()
	{
		if (id < 0)
		{
			TextureUtil.DrawTexture(crdBanner, def, ScaleMode.ScaleToFit);
		}
		else
		{
			Texture2D bnnr = BannerManager.Instance.GetBnnr(id + 1);
			if (null == bnnr)
			{
				bnnr = def;
			}
			TextureUtil.DrawTexture(crdBanner, bnnr, ScaleMode.ScaleToFit);
			if (fadeTexture != null)
			{
				Color color = GUI.color;
				GUI.color = Color.Lerp(new Color(1f, 1f, 1f, 1f), GlobalVars.txtEmptyColor, deltaFade * 2f);
				TextureUtil.DrawTexture(crdBanner, fadeTexture, ScaleMode.ScaleToFit);
				GUI.color = color;
			}
			string[] array = new string[BannerManager.Instance.Count()];
			for (int i = 0; i < BannerManager.Instance.Count(); i++)
			{
				array[i] = string.Empty;
			}
			int num = id;
			id = GUI.SelectionGrid(new Rect(crdBanner.x + 5f, crdBanner.y + 5f, 24f, (float)(24 * BannerManager.Instance.Count())), id, array, 1, "BtnChat");
			if (id != num)
			{
				OnChangeBanner(num);
			}
			if (GlobalVars.Instance.MyButton(crdBanner, string.Empty, "BannerButton"))
			{
				Banner banner = BannerManager.Instance.GetBanner(id + 1);
				if (banner != null)
				{
					switch (banner.ActionType)
					{
					case 1:
						BuildOption.OpenURL(banner.ActionParam);
						break;
					case 2:
						main.OpenShopTree(StringMgr.Instance.Get(banner.ActionParam));
						break;
					case 3:
						main.DirectBuyItem(banner.ActionParam);
						break;
					case 4:
						CSNetManager.Instance.Sock.SendCS_TC_OPEN_REQ();
						break;
					}
				}
			}
		}
	}

	private void OnChangeBanner(int prev)
	{
		deltaFade = 0f;
		fadeTexture = BannerManager.Instance.GetBnnr(prev + 1);
	}

	public void Update()
	{
		if (fadeTexture != null)
		{
			deltaFade += Time.deltaTime;
			if (deltaFade > 0.5f)
			{
				fadeTexture = null;
			}
		}
		if (BannerManager.Instance.Count() > 0)
		{
			Vector2 point = GlobalVars.Instance.PixelToGUIPoint(MouseUtil.ScreenToPixelPoint(Input.mousePosition));
			if (!GlobalVars.Instance.ToGUIRect(crdBanner).Contains(point))
			{
				deltaTimeChangeBanner += Time.deltaTime;
				if (deltaTimeChangeBanner > deltaTimeChangeBannerMax)
				{
					int num = id;
					deltaTimeChangeBanner = 0f;
					id++;
					if (id >= BannerManager.Instance.Count())
					{
						id = 0;
					}
					if (id != num)
					{
						OnChangeBanner(num);
					}
				}
			}
		}
	}
}
