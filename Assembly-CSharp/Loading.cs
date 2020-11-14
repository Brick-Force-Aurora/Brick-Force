using UnityEngine;

public class Loading : MonoBehaviour
{
	public Texture2D blackOut;

	public GUIDepth.LAYER guiDepth = GUIDepth.LAYER.LOADING;

	public Texture2D[] loadings;

	public string[] tipKeys;

	public TipDef[] moreTipKeys;

	public Vector2 crdLogoSize = new Vector2(166f, 40f);

	public Vector2 crdLogoOffset = new Vector2(-10f, -10f);

	private Texture2D loading;

	private string tip = string.Empty;

	private void OnGUI()
	{
		if (!BrickManager.Instance.IsLoaded)
		{
			GUISkin skin = GUI.skin;
			GUI.skin = GUISkinFinder.Instance.GetGUISkin();
			GUI.depth = (int)guiDepth;
			GUI.enabled = !DialogManager.Instance.IsModal;
			TextureUtil.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), blackOut);
			if (null != loading)
			{
				int width = loading.width;
				int height = loading.height;
				if (Screen.width <= 800)
				{
					width = Screen.width;
				}
				if (Screen.height <= 600)
				{
					height = Screen.height;
				}
				GUI.BeginGroup(new Rect((float)((Screen.width - width) / 2), (float)((Screen.height - height) / 2), (float)width, (float)height));
				TextureUtil.DrawTexture(new Rect(0f, 0f, (float)width, (float)height), loading);
				Texture2D logo = BuildOption.Instance.Props.logo;
				if (null != logo)
				{
					Vector2 vector = new Vector2((float)width - crdLogoSize.x + crdLogoOffset.x, crdLogoOffset.y);
					TextureUtil.DrawTexture(new Rect(vector.x, vector.y, crdLogoSize.x, crdLogoSize.y), logo, ScaleMode.StretchToFill);
				}
				Vector2 pos = new Vector2((float)(width / 2), (float)(height - 75));
				LabelUtil.TextOut(pos, tip, "Label", Color.white, GlobalVars.txtEmptyColor, TextAnchor.UpperCenter, 500f);
				GUI.EndGroup();
			}
			GUI.enabled = true;
			GUI.skin = skin;
		}
	}

	private void Start()
	{
		loading = loadings[Random.Range(0, loadings.Length)];
		int num = 0;
		int num2 = -1;
		if (moreTipKeys != null && moreTipKeys.Length > 0)
		{
			for (int i = 0; i < moreTipKeys.Length; i++)
			{
				if (BuildOption.Instance.target == moreTipKeys[i].target && moreTipKeys[i].tips.Length > 0)
				{
					num2 = i;
					num += moreTipKeys[i].tips.Length;
					break;
				}
			}
		}
		if (num > 0)
		{
			num += tipKeys.Length;
			string[] array = new string[num];
			for (int j = 0; j < tipKeys.Length; j++)
			{
				array[j] = tipKeys[j];
			}
			for (int k = 0; k < moreTipKeys[num2].tips.Length; k++)
			{
				array[tipKeys.Length + k] = moreTipKeys[num2].tips[k];
			}
			tip = StringMgr.Instance.Get(array[Random.Range(0, array.Length)]);
		}
		else
		{
			tip = StringMgr.Instance.Get(tipKeys[Random.Range(0, tipKeys.Length)]);
		}
	}
}
