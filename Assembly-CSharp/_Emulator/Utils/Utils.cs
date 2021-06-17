using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Emulator
{
    class Utils
    {
		public static List<List<T>> SplitList<T>(List<T> list, int chunkSize)
		{
			if (chunkSize <= 0)
			{
				throw new ArgumentException("chunkSize must be greater than 0.");
			}

			List<List<T>> retVal = new List<List<T>>();
			int index = 0;
			while (index < list.Count)
			{
				int count = list.Count - index > chunkSize ? chunkSize : list.Count - index;
				retVal.Add(list.GetRange(index, count));

				index += chunkSize;
			}

			return retVal;
		}

        public static Color HSVToRGB(float H, float S, float V, bool hdr = false)
        {
            Color white = Color.white;
            if (S == 0f)
            {
                white.r = V;
                white.g = V;
                white.b = V;
            }
            else if (V == 0f)
            {
                white.r = 0f;
                white.g = 0f;
                white.b = 0f;
            }
            else
            {
                white.r = 0f;
                white.g = 0f;
                white.b = 0f;
                float num = H * 6f;
                int num2 = (int)Mathf.Floor(num);
                float num3 = num - (float)num2;
                float num4 = V * (1f - S);
                float num5 = V * (1f - S * num3);
                float num6 = V * (1f - S * (1f - num3));
                switch (num2 + 1)
                {
                    case 0:
                        white.r = V;
                        white.g = num4;
                        white.b = num5;
                        break;
                    case 1:
                        white.r = V;
                        white.g = num6;
                        white.b = num4;
                        break;
                    case 2:
                        white.r = num5;
                        white.g = V;
                        white.b = num4;
                        break;
                    case 3:
                        white.r = num4;
                        white.g = V;
                        white.b = num6;
                        break;
                    case 4:
                        white.r = num4;
                        white.g = num5;
                        white.b = V;
                        break;
                    case 5:
                        white.r = num6;
                        white.g = num4;
                        white.b = V;
                        break;
                    case 6:
                        white.r = V;
                        white.g = num4;
                        white.b = num5;
                        break;
                    case 7:
                        white.r = V;
                        white.g = num6;
                        white.b = num4;
                        break;
                }
                if (!hdr)
                {
                    white.r = Mathf.Clamp(white.r, 0f, 1f);
                    white.g = Mathf.Clamp(white.g, 0f, 1f);
                    white.b = Mathf.Clamp(white.b, 0f, 1f);
                }
            }
            return white;
        }

        public static void RGBToHSV(Color color, out float H, out float S, out float V)
        {
            if (color.b > color.g && color.b > color.r)
            {
                RGBToHSVHelper(4f, color.b, color.r, color.g, out H, out S, out V);
            }
            else if (color.g > color.r)
            {
                RGBToHSVHelper(2f, color.g, color.b, color.r, out H, out S, out V);
            }
            else
            {
                RGBToHSVHelper(0f, color.r, color.g, color.b, out H, out S, out V);
            }
        }

        private static void RGBToHSVHelper(float offset, float dominantcolor, float colorone, float colortwo, out float H, out float S, out float V)
        {
            V = dominantcolor;
            if (V != 0f)
            {
                float num;
                if (colorone > colortwo)
                {
                    num = colortwo;
                }
                else
                {
                    num = colorone;
                }
                float num2 = V - num;
                if (num2 != 0f)
                {
                    S = num2 / V;
                    H = offset + (colorone - colortwo) / num2;
                }
                else
                {
                    S = 0f;
                    H = offset + (colorone - colortwo);
                }
                H /= 6f;
                if (H < 0f)
                {
                    H += 1f;
                }
            }
            else
            {
                S = 0f;
                H = 0f;
            }
        }
    }
}
