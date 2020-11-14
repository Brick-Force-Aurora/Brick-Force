using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerManager : MonoBehaviour
{
	private SortedDictionary<int, Banner> ads;

	private static BannerManager _instance;

	public static BannerManager Instance
	{
		get
		{
			if (null == _instance)
			{
				_instance = (Object.FindObjectOfType(typeof(BannerManager)) as BannerManager);
				if (null == _instance)
				{
					Debug.LogError("ERROR, Fail to get the BannerManager Instance");
				}
			}
			return _instance;
		}
	}

	private void Awake()
	{
		ads = new SortedDictionary<int, Banner>();
		Object.DontDestroyOnLoad(this);
	}

	public void Clear()
	{
		ads.Clear();
	}

	public void AddAd(int _row, string _imagePath, int _actionType, string _actionParam)
	{
		if (!ads.ContainsKey(_row))
		{
			ads.Add(_row, new Banner(_row, _imagePath, _actionType, _actionParam));
		}
		else
		{
			ads[_row].ImagePath = _imagePath;
			ads[_row].ActionType = _actionType;
			ads[_row].ActionParam = _actionParam;
			ads[_row].Bnnr = null;
			ads[_row].CDN = null;
		}
		Banner banner = GetBanner(_row);
		if (banner != null)
		{
			StartCoroutine(LoadBannerTexture(banner));
		}
	}

	public Texture2D GetBnnr(int _row)
	{
		Texture2D result = null;
		if (ads.ContainsKey(_row))
		{
			result = ads[_row].Bnnr;
		}
		return result;
	}

	public Banner GetBanner(int _row)
	{
		if (ads.ContainsKey(_row))
		{
			return ads[_row];
		}
		return null;
	}

	public int Count()
	{
		return ads.Count;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private IEnumerator LoadBannerTexture(Banner banner)
	{
		Property prop = BuildOption.Instance.Props;
		string url = "http://" + prop.GetResourceServer + "/BfData/ads/" + banner.ImagePath;
		banner.CDN = new WWW(url);
		yield return (object)banner.CDN;
		banner.Bnnr = new Texture2D(128, 128, TextureFormat.RGBA32, mipmap: false);
		banner.Bnnr.wrapMode = TextureWrapMode.Clamp;
		if (!banner.Bnnr.LoadImage(banner.CDN.bytes))
		{
			banner.Bnnr = null;
		}
		else
		{
			banner.Bnnr.Apply();
		}
		banner.CDN.Dispose();
		banner.CDN = null;
	}
}
