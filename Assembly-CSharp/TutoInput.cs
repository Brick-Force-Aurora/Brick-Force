using UnityEngine;

public class TutoInput : MonoBehaviour
{
	public Texture2D[] texW;

	public Texture2D[] texS;

	public Texture2D[] texA;

	public Texture2D[] texD;

	public Texture2D[] texE;

	public Texture2D[] texSP;

	public Texture2D[] texL;

	public Texture2D[] texR;

	public Texture2D[] texM;

	public Texture2D[] upArrow;

	public Texture2D[] dnArrow;

	public Texture2D blick;

	private TUTO_INPUT tutoInput;

	private TUTO_INPUT tutoClick;

	private int texId;

	private float Elapsed;

	private float MaxElapsed = 0.5f;

	private float ElapsedBlick;

	private float MaxElapsedBlick = 0.2f;

	private TUTO_INPUT st;

	private TUTO_INPUT end;

	private TUTO_INPUT cur;

	private bool repeat;

	private bool press;

	private bool keycheck;

	private bool changepos;

	private bool isBlick;

	private bool drawBlick;

	private int[] skipRepeatIds;

	private Vector2 newpos;

	private Rect crdBlick = new Rect(0f, 0f, 0f, 0f);

	private void Start()
	{
		tutoInput = TUTO_INPUT.NONE;
	}

	private int toIdx(TUTO_INPUT input)
	{
		int[] array = new int[9]
		{
			1,
			2,
			4,
			8,
			16,
			32,
			64,
			128,
			256
		};
		for (int i = 0; i < 9; i++)
		{
			if (array[i] == (int)input)
			{
				return i;
			}
		}
		return -1;
	}

	public void setBlick(bool set)
	{
		isBlick = set;
		if (isBlick)
		{
			drawBlick = true;
		}
		ElapsedBlick = 0f;
	}

	public void setBlickPos(Rect rc)
	{
		crdBlick = rc;
	}

	private void setSkipIndices(TUTO_INPUT[] skips)
	{
		skipRepeatIds = new int[skips.Length];
		for (int i = 0; i < skips.Length; i++)
		{
			TUTO_INPUT input = skips[i];
			skipRepeatIds[i] = toIdx(input);
		}
	}

	private bool getRealNext(int id)
	{
		if (skipRepeatIds != null && skipRepeatIds.Length > 0)
		{
			for (int i = 0; i < skipRepeatIds.Length; i++)
			{
				if (skipRepeatIds[i] == id)
				{
					return false;
				}
			}
		}
		return true;
	}

	private TUTO_INPUT nextInput(TUTO_INPUT curInput)
	{
		int[] array = new int[9]
		{
			1,
			2,
			4,
			8,
			16,
			32,
			64,
			128,
			256
		};
		for (int i = 0; i < 9; i++)
		{
			if (array[i] == (int)curInput && getRealNext(array[i + 1]))
			{
				return (TUTO_INPUT)array[i + 1];
			}
		}
		return TUTO_INPUT.NONE;
	}

	private void Update()
	{
		if (!Application.isLoadingLevel)
		{
			if (keycheck && tutoClick == TUTO_INPUT.SPACE && custom_inputs.Instance.GetButtonDown("K_JUMP"))
			{
				setActive(TUTO_INPUT.WASD);
				setClick(TUTO_INPUT.W);
				keycheck = false;
			}
			if (isBlick)
			{
				ElapsedBlick += Time.deltaTime;
				if (ElapsedBlick > MaxElapsedBlick)
				{
					drawBlick = !drawBlick;
					ElapsedBlick = 0f;
				}
			}
			if (repeat)
			{
				Elapsed += Time.deltaTime;
				if (cur == st)
				{
					if (press)
					{
						if (Elapsed >= MaxElapsed)
						{
							texId = 0;
							Elapsed = 0f;
							press = false;
						}
					}
					else if (Elapsed >= MaxElapsed)
					{
						texId = 1;
						Elapsed = 0f;
						press = true;
						cur = nextInput(st);
						if (cur == TUTO_INPUT.NONE)
						{
							cur = end;
						}
					}
				}
				else if (cur == end)
				{
					if (press)
					{
						if (Elapsed >= MaxElapsed)
						{
							texId = 0;
							Elapsed = 0f;
							press = false;
						}
					}
					else if (Elapsed >= MaxElapsed)
					{
						texId = 1;
						Elapsed = 0f;
						press = true;
						cur = st;
					}
				}
				else if (press)
				{
					if (Elapsed >= MaxElapsed)
					{
						texId = 0;
						Elapsed = 0f;
						press = false;
					}
				}
				else if (Elapsed >= MaxElapsed)
				{
					texId = 1;
					Elapsed = 0f;
					press = true;
					cur = nextInput(cur);
				}
			}
			else
			{
				Elapsed += Time.deltaTime;
				if (press)
				{
					if (Elapsed >= MaxElapsed)
					{
						texId = 0;
						Elapsed = 0f;
						press = false;
					}
				}
				else if (Elapsed >= MaxElapsed)
				{
					texId = 1;
					Elapsed = 0f;
					press = true;
				}
			}
		}
	}

	public void setActive(TUTO_INPUT ti)
	{
		tutoInput = ti;
	}

	public void setClick(TUTO_INPUT ti)
	{
		tutoClick = ti;
		if (tutoClick == TUTO_INPUT.WASD)
		{
			skipRepeatIds = null;
			press = true;
			repeat = true;
			st = TUTO_INPUT.W;
			end = TUTO_INPUT.D;
			cur = TUTO_INPUT.W;
			Elapsed = 0f;
			texId = 1;
		}
		else if (tutoClick == (TUTO_INPUT)17)
		{
			press = true;
			repeat = true;
			st = TUTO_INPUT.W;
			end = TUTO_INPUT.SPACE;
			cur = TUTO_INPUT.W;
			Elapsed = 0f;
			texId = 1;
			TUTO_INPUT[] skipIndices = new TUTO_INPUT[3]
			{
				TUTO_INPUT.A,
				TUTO_INPUT.S,
				TUTO_INPUT.D
			};
			setSkipIndices(skipIndices);
		}
		else if (tutoClick == (TUTO_INPUT)192)
		{
			skipRepeatIds = null;
			press = true;
			repeat = true;
			st = TUTO_INPUT.M_L;
			end = TUTO_INPUT.M_R;
			cur = TUTO_INPUT.M_L;
			Elapsed = 0f;
			texId = 1;
		}
		else
		{
			skipRepeatIds = null;
			press = true;
			repeat = false;
			Elapsed = 0f;
			texId = 1;
		}
	}

	public void setKeyCheck()
	{
		keycheck = true;
	}

	public void setChangePos(bool set, Vector2 p)
	{
		changepos = set;
		newpos = new Vector2(p.x + 50f, p.y + 40f);
	}

	public bool IsClick(TUTO_INPUT input)
	{
		if (repeat && cur != input)
		{
			return false;
		}
		int num = (int)(tutoClick & input);
		if (num == (int)input)
		{
			return true;
		}
		return false;
	}

	public void drawInputs()
	{
		switch (tutoInput)
		{
		case TUTO_INPUT.WASD:
			TextureUtil.DrawTexture(new Rect((float)(Screen.width / 2 - texW[0].width / 2), (float)(Screen.height - 220), (float)texW[0].width, (float)texW[0].height), texW[IsClick(TUTO_INPUT.W) ? texId : 0]);
			TextureUtil.DrawTexture(new Rect((float)(Screen.width / 2 - texA[0].width / 2 - 70), (float)(Screen.height - 150), (float)texA[0].width, (float)texA[0].height), texA[IsClick(TUTO_INPUT.A) ? texId : 0]);
			TextureUtil.DrawTexture(new Rect((float)(Screen.width / 2 - texS[0].width / 2), (float)(Screen.height - 150), (float)texS[0].width, (float)texS[0].height), texS[IsClick(TUTO_INPUT.S) ? texId : 0]);
			TextureUtil.DrawTexture(new Rect((float)(Screen.width / 2 - texD[0].width / 2 + 70), (float)(Screen.height - 150), (float)texD[0].width, (float)texD[0].height), texD[IsClick(TUTO_INPUT.D) ? texId : 0]);
			break;
		case TUTO_INPUT.KEYALL:
			TextureUtil.DrawTexture(new Rect((float)(Screen.width / 2 - texW[0].width / 2), (float)(Screen.height - 220), (float)texW[0].width, (float)texW[0].height), texW[IsClick(TUTO_INPUT.W) ? texId : 0]);
			TextureUtil.DrawTexture(new Rect((float)(Screen.width / 2 - texS[0].width / 2), (float)(Screen.height - 150), (float)texS[0].width, (float)texS[0].height), texS[IsClick(TUTO_INPUT.S) ? texId : 0]);
			TextureUtil.DrawTexture(new Rect((float)(Screen.width / 2 - texA[0].width / 2 - 70), (float)(Screen.height - 150), (float)texA[0].width, (float)texA[0].height), texA[IsClick(TUTO_INPUT.A) ? texId : 0]);
			TextureUtil.DrawTexture(new Rect((float)(Screen.width / 2 - texD[0].width / 2 + 70), (float)(Screen.height - 150), (float)texD[0].width, (float)texD[0].height), texD[IsClick(TUTO_INPUT.D) ? texId : 0]);
			TextureUtil.DrawTexture(new Rect((float)(Screen.width / 2 - texD[0].width / 2 + 140), (float)(Screen.height - 150), (float)texSP[0].width, (float)texSP[0].height), texSP[IsClick(TUTO_INPUT.SPACE) ? texId : 0]);
			break;
		case TUTO_INPUT.MOUSEALL:
			if (cur == TUTO_INPUT.M_L)
			{
				TextureUtil.DrawTexture(new Rect((float)(Screen.width / 2 - texL[0].width / 2), (float)(Screen.height - 150), (float)texL[0].width, (float)texL[0].height), texL[IsClick(TUTO_INPUT.M_L) ? texId : 0]);
			}
			if (cur == TUTO_INPUT.M_R)
			{
				TextureUtil.DrawTexture(new Rect((float)(Screen.width / 2 + texR[0].width / 2), (float)(Screen.height - 150), (float)texR[0].width, (float)texR[0].height), texR[IsClick(TUTO_INPUT.M_R) ? texId : 0]);
			}
			break;
		case TUTO_INPUT.M_L:
		{
			Vector2 vector = default(Vector2);
			if (changepos)
			{
				vector = newpos;
				TextureUtil.DrawTexture(new Rect(vector.x - 50f, vector.y + 10f, (float)upArrow[0].width, (float)upArrow[0].height), upArrow[IsClick(TUTO_INPUT.M_L) ? texId : 0]);
				if (drawBlick)
				{
					TextureUtil.DrawTexture(crdBlick, blick);
				}
			}
			else
			{
				vector.x = (float)(Screen.width / 2 - texL[0].width / 2);
				vector.y = (float)(Screen.height - 200);
			}
			TextureUtil.DrawTexture(new Rect(vector.x, vector.y, (float)texL[0].width, (float)texL[0].height), texL[IsClick(TUTO_INPUT.M_L) ? texId : 0]);
			break;
		}
		case TUTO_INPUT.M_R:
		{
			Vector2 vector2 = default(Vector2);
			if (changepos)
			{
				vector2 = newpos;
			}
			else
			{
				vector2.x = (float)(Screen.width / 2 - texR[0].width / 2);
				vector2.y = (float)(Screen.height - 200);
			}
			TextureUtil.DrawTexture(new Rect(vector2.x, vector2.y, (float)texR[0].width, (float)texR[0].height), texR[IsClick(TUTO_INPUT.M_R) ? texId : 0]);
			break;
		}
		case TUTO_INPUT.E:
			TextureUtil.DrawTexture(new Rect((float)(Screen.width / 2 - texE[0].width / 2), (float)(Screen.height - 150), (float)texE[0].width, (float)texE[0].height), texE[IsClick(TUTO_INPUT.E) ? texId : 0]);
			break;
		case TUTO_INPUT.SPACE:
			TextureUtil.DrawTexture(new Rect((float)(Screen.width / 2 - texSP[0].width / 2), (float)(Screen.height - 150), (float)texSP[0].width, (float)texSP[0].height), texSP[IsClick(TUTO_INPUT.SPACE) ? texId : 0]);
			break;
		case TUTO_INPUT.M:
			TextureUtil.DrawTexture(new Rect((float)(Screen.width / 2 - texM[0].width / 2), (float)(Screen.height - 150), (float)texM[0].width, (float)texM[0].height), texM[IsClick(TUTO_INPUT.M) ? texId : 0]);
			break;
		case TUTO_INPUT.D:
			TextureUtil.DrawTexture(new Rect((float)(Screen.width / 2 - texD[0].width / 2), (float)(Screen.height - 150), (float)texD[0].width, (float)texD[0].height), texD[IsClick(TUTO_INPUT.D) ? texId : 0]);
			break;
		}
	}
}
