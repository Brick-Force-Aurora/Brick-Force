using CodeStage.AntiCheat.ObscuredTypes;
using System.Diagnostics;
using UnityEngine;

public class PerformanceObscuredTests : MonoBehaviour
{
	public bool boolTest = true;

	public int boolIterations = 2500000;

	public bool byteTest = true;

	public int byteIterations = 2500000;

	public bool shortTest = true;

	public int shortIterations = 2500000;

	public bool ushortTest = true;

	public int ushortIterations = 2500000;

	public bool intTest = true;

	public int intIterations = 2500000;

	public bool uintTest = true;

	public int uintIterations = 2500000;

	public bool longTest = true;

	public int longIterations = 2500000;

	public bool floatTest = true;

	public int floatIterations = 2500000;

	public bool doubleTest = true;

	public int doubleIterations = 2500000;

	public bool stringTest = true;

	public int stringIterations = 250000;

	public bool vector3Test = true;

	public int vector3Iterations = 2500000;

	public bool prefsTest = true;

	public int prefsIterations = 2500;

	private void Start()
	{
		Invoke("StartTests", 1f);
	}

	private void StartTests()
	{
		UnityEngine.Debug.Log("--- OBSCURED TYPES PERFORMANCE TESTS ---\n");
		if (boolTest)
		{
			TestBool();
		}
		if (byteTest)
		{
			TestByte();
		}
		if (shortTest)
		{
			TestShort();
		}
		if (ushortTest)
		{
			TestUShort();
		}
		if (intTest)
		{
			TestInt();
		}
		if (uintTest)
		{
			TestUInt();
		}
		if (longTest)
		{
			TestLong();
		}
		if (floatTest)
		{
			TestFloat();
		}
		if (doubleTest)
		{
			TestDouble();
		}
		if (stringTest)
		{
			TestString();
		}
		if (vector3Test)
		{
			TestVector3();
		}
		if (prefsTest)
		{
			TestPrefs();
		}
	}

	private void TestBool()
	{
		UnityEngine.Debug.Log("  Testing ObscuredBool vs bool preformance:\n  " + boolIterations + " iterations for read and same for write");
		Stopwatch stopwatch = Stopwatch.StartNew();
		ObscuredBool value = true;
		bool flag = value;
		bool flag2 = false;
		for (int i = 0; i < boolIterations; i++)
		{
			flag2 = value;
		}
		for (int j = 0; j < boolIterations; j++)
		{
			value = flag2;
		}
		UnityEngine.Debug.Log("    ObscuredBool:\n    " + stopwatch.ElapsedMilliseconds + " ms ");
		stopwatch.Reset();
		stopwatch.Start();
		for (int k = 0; k < boolIterations; k++)
		{
			flag2 = flag;
		}
		for (int l = 0; l < boolIterations; l++)
		{
			flag = flag2;
		}
		stopwatch.Stop();
		UnityEngine.Debug.Log("    bool:\n    " + stopwatch.ElapsedMilliseconds + "  ms ");
		if (flag2)
		{
		}
		if ((bool)value)
		{
		}
		if (!flag)
		{
		}
	}

	private void TestByte()
	{
		UnityEngine.Debug.Log("  Testing ObscuredByte vs byte preformance:\n  " + byteIterations + " iterations for read and same for write");
		Stopwatch stopwatch = Stopwatch.StartNew();
		ObscuredByte value = (byte)100;
		byte b = value;
		byte b2 = 0;
		for (int i = 0; i < byteIterations; i++)
		{
			b2 = value;
		}
		for (int j = 0; j < byteIterations; j++)
		{
			value = b2;
		}
		UnityEngine.Debug.Log("    ObscuredByte:\n    " + stopwatch.ElapsedMilliseconds + " ms ");
		stopwatch.Reset();
		stopwatch.Start();
		for (int k = 0; k < byteIterations; k++)
		{
			b2 = b;
		}
		for (int l = 0; l < byteIterations; l++)
		{
			b = b2;
		}
		stopwatch.Stop();
		UnityEngine.Debug.Log("    byte:\n    " + stopwatch.ElapsedMilliseconds + "  ms ");
		if (b2 != 0)
		{
		}
		if ((byte)value != 0)
		{
		}
		if (b == 0)
		{
		}
	}

	private void TestShort()
	{
		UnityEngine.Debug.Log("  Testing ObscuredShort vs short preformance:\n  " + shortIterations + " iterations for read and same for write");
		Stopwatch stopwatch = Stopwatch.StartNew();
		ObscuredShort value = (short)100;
		short num = value;
		short num2 = 0;
		for (int i = 0; i < shortIterations; i++)
		{
			num2 = value;
		}
		for (int j = 0; j < shortIterations; j++)
		{
			value = num2;
		}
		UnityEngine.Debug.Log("    ObscuredShort:\n    " + stopwatch.ElapsedMilliseconds + " ms ");
		stopwatch.Reset();
		stopwatch.Start();
		for (int k = 0; k < shortIterations; k++)
		{
			num2 = num;
		}
		for (int l = 0; l < shortIterations; l++)
		{
			num = num2;
		}
		stopwatch.Stop();
		UnityEngine.Debug.Log("    short:\n    " + stopwatch.ElapsedMilliseconds + "  ms ");
		if (num2 != 0)
		{
		}
		if ((short)value != 0)
		{
		}
		if (num == 0)
		{
		}
	}

	private void TestUShort()
	{
		UnityEngine.Debug.Log("  Testing ObscuredUShort vs ushort preformance:\n  " + ushortIterations + " iterations for read and same for write");
		Stopwatch stopwatch = Stopwatch.StartNew();
		ObscuredUShort value = (ushort)100;
		ushort num = value;
		ushort num2 = 0;
		for (int i = 0; i < ushortIterations; i++)
		{
			num2 = value;
		}
		for (int j = 0; j < ushortIterations; j++)
		{
			value = num2;
		}
		UnityEngine.Debug.Log("    ObscuredUShort:\n    " + stopwatch.ElapsedMilliseconds + " ms ");
		stopwatch.Reset();
		stopwatch.Start();
		for (int k = 0; k < ushortIterations; k++)
		{
			num2 = num;
		}
		for (int l = 0; l < ushortIterations; l++)
		{
			num = num2;
		}
		stopwatch.Stop();
		UnityEngine.Debug.Log("    ushort:\n    " + stopwatch.ElapsedMilliseconds + "  ms ");
		if (num2 != 0)
		{
		}
		if ((ushort)value != 0)
		{
		}
		if (num == 0)
		{
		}
	}

	private void TestDouble()
	{
		UnityEngine.Debug.Log("  Testing ObscuredDouble vs double preformance:\n  " + doubleIterations + " iterations for read and same for write");
		Stopwatch stopwatch = Stopwatch.StartNew();
		ObscuredDouble value = 100.0;
		double num = value;
		double num2 = 0.0;
		for (int i = 0; i < doubleIterations; i++)
		{
			num2 = value;
		}
		for (int j = 0; j < doubleIterations; j++)
		{
			value = num2;
		}
		UnityEngine.Debug.Log("    ObscuredDouble:\n    " + stopwatch.ElapsedMilliseconds + " ms ");
		stopwatch.Reset();
		stopwatch.Start();
		for (int k = 0; k < doubleIterations; k++)
		{
			num2 = num;
		}
		for (int l = 0; l < doubleIterations; l++)
		{
			num = num2;
		}
		stopwatch.Stop();
		UnityEngine.Debug.Log("    double:\n    " + stopwatch.ElapsedMilliseconds + "  ms ");
		if (num2 != 0.0)
		{
		}
		if ((double)value != 0.0)
		{
		}
		if (num == 0.0)
		{
		}
	}

	private void TestFloat()
	{
		UnityEngine.Debug.Log("  Testing ObscuredFloat vs float preformance:\n  " + floatIterations + " iterations for read and same for write");
		Stopwatch stopwatch = Stopwatch.StartNew();
		ObscuredFloat value = 100f;
		float num = value;
		float num2 = 0f;
		for (int i = 0; i < floatIterations; i++)
		{
			num2 = value;
		}
		for (int j = 0; j < floatIterations; j++)
		{
			value = num2;
		}
		UnityEngine.Debug.Log("    ObscuredFloat:\n    " + stopwatch.ElapsedMilliseconds + " ms ");
		stopwatch.Reset();
		stopwatch.Start();
		for (int k = 0; k < floatIterations; k++)
		{
			num2 = num;
		}
		for (int l = 0; l < floatIterations; l++)
		{
			num = num2;
		}
		stopwatch.Stop();
		UnityEngine.Debug.Log("    float:\n    " + stopwatch.ElapsedMilliseconds + "  ms ");
		if (num2 != 0f)
		{
		}
		if ((float)value != 0f)
		{
		}
		if (num == 0f)
		{
		}
	}

	private void TestInt()
	{
		UnityEngine.Debug.Log("  Testing ObscuredInt vs int preformance:\n  " + intIterations + " iterations for read and same for write");
		Stopwatch stopwatch = Stopwatch.StartNew();
		ObscuredInt value = 100;
		int num = value;
		int num2 = 0;
		for (int i = 0; i < intIterations; i++)
		{
			num2 = value;
		}
		for (int j = 0; j < intIterations; j++)
		{
			value = num2;
		}
		UnityEngine.Debug.Log("    ObscuredInt:\n    " + stopwatch.ElapsedMilliseconds + " ms ");
		stopwatch.Reset();
		stopwatch.Start();
		for (int k = 0; k < intIterations; k++)
		{
			num2 = num;
		}
		for (int l = 0; l < intIterations; l++)
		{
			num = num2;
		}
		stopwatch.Stop();
		UnityEngine.Debug.Log("    int:\n    " + stopwatch.ElapsedMilliseconds + "  ms ");
		if (num2 != 0)
		{
		}
		if ((int)value != 0)
		{
		}
		if (num == 0)
		{
		}
	}

	private void TestLong()
	{
		UnityEngine.Debug.Log("  Testing ObscuredLong vs long preformance:\n  " + longIterations + " iterations for read and same for write");
		Stopwatch stopwatch = Stopwatch.StartNew();
		ObscuredLong value = 100L;
		long num = value;
		long num2 = 0L;
		for (int i = 0; i < longIterations; i++)
		{
			num2 = value;
		}
		for (int j = 0; j < longIterations; j++)
		{
			value = num2;
		}
		UnityEngine.Debug.Log("    ObscuredLong:\n    " + stopwatch.ElapsedMilliseconds + " ms ");
		stopwatch.Reset();
		stopwatch.Start();
		for (int k = 0; k < longIterations; k++)
		{
			num2 = num;
		}
		for (int l = 0; l < longIterations; l++)
		{
			num = num2;
		}
		stopwatch.Stop();
		UnityEngine.Debug.Log("    long:\n    " + stopwatch.ElapsedMilliseconds + "  ms ");
		if (num2 != 0L)
		{
		}
		if ((long)value != 0L)
		{
		}
		if (num == 0L)
		{
		}
	}

	private void TestString()
	{
		UnityEngine.Debug.Log("  Testing ObscuredString vs string preformance:\n  " + stringIterations + " iterations for read and same for write");
		Stopwatch stopwatch = Stopwatch.StartNew();
		ObscuredString obscuredString = "abcd";
		string text = obscuredString;
		string text2 = string.Empty;
		for (int i = 0; i < stringIterations; i++)
		{
			text2 = obscuredString;
		}
		for (int j = 0; j < stringIterations; j++)
		{
			obscuredString = text2;
		}
		UnityEngine.Debug.Log("    ObscuredString:\n    " + stopwatch.ElapsedMilliseconds + " ms ");
		stopwatch.Reset();
		stopwatch.Start();
		for (int k = 0; k < stringIterations; k++)
		{
			text2 = text;
		}
		for (int l = 0; l < stringIterations; l++)
		{
			text = text2;
		}
		stopwatch.Stop();
		UnityEngine.Debug.Log("    string:\n    " + stopwatch.ElapsedMilliseconds + "  ms ");
		if (text2 != string.Empty)
		{
		}
		if (obscuredString != string.Empty)
		{
		}
		if (!(text != string.Empty))
		{
		}
	}

	private void TestUInt()
	{
		UnityEngine.Debug.Log("  Testing ObscuredUInt vs uint preformance:\n  " + uintIterations + " iterations for read and same for write");
		Stopwatch stopwatch = Stopwatch.StartNew();
		ObscuredUInt value = 100u;
		uint num = value;
		uint num2 = 0u;
		for (int i = 0; i < uintIterations; i++)
		{
			num2 = value;
		}
		for (int j = 0; j < uintIterations; j++)
		{
			value = num2;
		}
		UnityEngine.Debug.Log("    ObscuredUInt:\n    " + stopwatch.ElapsedMilliseconds + " ms ");
		stopwatch.Reset();
		stopwatch.Start();
		for (int k = 0; k < uintIterations; k++)
		{
			num2 = num;
		}
		for (int l = 0; l < uintIterations; l++)
		{
			num = num2;
		}
		stopwatch.Stop();
		UnityEngine.Debug.Log("    uint:\n    " + stopwatch.ElapsedMilliseconds + "  ms ");
		if (num2 != 0)
		{
		}
		if ((uint)value != 0)
		{
		}
		if (num == 0)
		{
		}
	}

	private void TestVector3()
	{
		UnityEngine.Debug.Log("  Testing ObscuredVector3 vs Vector3 preformance:\n  " + vector3Iterations + " iterations for read and same for write");
		Stopwatch stopwatch = Stopwatch.StartNew();
		ObscuredVector3 value = new Vector3(1f, 2f, 3f);
		Vector3 vector = value;
		Vector3 vector2 = new Vector3(0f, 0f, 0f);
		for (int i = 0; i < vector3Iterations; i++)
		{
			vector2 = value;
		}
		for (int j = 0; j < vector3Iterations; j++)
		{
			value = vector2;
		}
		UnityEngine.Debug.Log("    ObscuredVector3:\n    " + stopwatch.ElapsedMilliseconds + " ms ");
		stopwatch.Reset();
		stopwatch.Start();
		for (int k = 0; k < vector3Iterations; k++)
		{
			vector2 = vector;
		}
		for (int l = 0; l < vector3Iterations; l++)
		{
			vector = vector2;
		}
		stopwatch.Stop();
		UnityEngine.Debug.Log("    Vector3:\n    " + stopwatch.ElapsedMilliseconds + "  ms ");
		if (vector2 != Vector3.zero)
		{
		}
		if (value != Vector3.zero)
		{
		}
		if (!(vector != Vector3.zero))
		{
		}
	}

	private void TestPrefs()
	{
		UnityEngine.Debug.Log("  Testing ObscuredPrefs vs PlayerPrefs preformance:\n  " + prefsIterations + " iterations for read and same for write");
		Stopwatch stopwatch = Stopwatch.StartNew();
		for (int i = 0; i < prefsIterations; i++)
		{
			ObscuredPrefs.SetInt("__a", 1);
			ObscuredPrefs.SetFloat("__b", 2f);
			ObscuredPrefs.SetString("__c", "3");
		}
		for (int j = 0; j < prefsIterations; j++)
		{
			ObscuredPrefs.GetInt("__a", 1);
			ObscuredPrefs.GetFloat("__b", 2f);
			ObscuredPrefs.GetString("__c", "3");
		}
		UnityEngine.Debug.Log("    ObscuredPrefs:\n    " + stopwatch.ElapsedMilliseconds + " ms ");
		stopwatch.Reset();
		stopwatch.Start();
		for (int k = 0; k < prefsIterations; k++)
		{
			PlayerPrefs.SetInt("__a", 1);
			PlayerPrefs.SetFloat("__b", 2f);
			PlayerPrefs.SetString("__c", "3");
		}
		for (int l = 0; l < prefsIterations; l++)
		{
			PlayerPrefs.GetInt("__a", 1);
			PlayerPrefs.GetFloat("__b", 2f);
			PlayerPrefs.GetString("__c", "3");
		}
		stopwatch.Stop();
		UnityEngine.Debug.Log("    PlayerPrefs:\n    " + stopwatch.ElapsedMilliseconds + "  ms ");
		PlayerPrefs.DeleteKey("__a");
		PlayerPrefs.DeleteKey("__b");
		PlayerPrefs.DeleteKey("__c");
	}
}
