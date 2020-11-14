using System;
using System.Collections.Generic;

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
	}
}
