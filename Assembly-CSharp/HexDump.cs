using System;
using System.IO;

public class HexDump
{
	public static string dataPath = string.Empty;

	public static bool needDump;

	private static void _Dump(TextWriter writer, byte[] buffer, int startIndex, int length, string prefix)
	{
		writer.Write(prefix);
		string value = BitConverter.ToString(buffer, startIndex, length);
		writer.WriteLine(value);
	}

	public static void Dump(string fileName, byte[] buffer, int startIndex, int length, string prefix)
	{
	}
}
