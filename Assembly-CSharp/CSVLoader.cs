using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class CSVLoader
{
	private const string sSecretKey = "Password";

	private bool _loaded;

	public List<string[]> _rows;

	private char crypt = 'E';

	private int _noCols;

	public int Cols => _noCols;

	public int Rows
	{
		get
		{
			if (_rows == null)
			{
				return 0;
			}
			return _rows.Count;
		}
	}

	public CSVLoader()
	{
		_loaded = false;
		_rows = null;
	}

	public CSVLoader(List<string[]> data)
	{
		_rows = data;
		_loaded = true;
		for (int row = 0; row < _rows.Count; row++)
		{
			if (_rows[row].Length > _noCols)
				_noCols = _rows[row].Length;
		}
	}

	public bool ReadValue(int col, int row, long def, out long Value)
	{
		Value = def;
		if (!_loaded)
		{
			return false;
		}
		if (_rows == null)
		{
			return false;
		}
		if (row >= _rows.Count)
		{
			return false;
		}
		if (col >= _noCols)
		{
			return false;
		}
		Value = long.Parse(_rows[row][col]);
		return true;
	}

	public bool ReadValue(int col, int row, bool def, out bool Value)
	{
		Value = def;
		if (!_loaded)
		{
			return false;
		}
		if (_rows == null)
		{
			return false;
		}
		if (row >= _rows.Count)
		{
			return false;
		}
		if (col >= _noCols)
		{
			return false;
		}
		Value = bool.Parse(_rows[row][col]);
		return true;
	}

	public bool ReadValue(int col, int row, int def, out int Value)
	{
		Value = def;
		if (!_loaded)
		{
			return false;
		}
		if (_rows == null)
		{
			return false;
		}
		if (row >= _rows.Count)
		{
			return false;
		}
		if (col >= _noCols)
		{
			return false;
		}
		Value = int.Parse(_rows[row][col]);
		return true;
	}

	public bool ReadValue(int col, int row, string def, out string Value)
	{
		Value = def;
		if (!_loaded)
		{
			return false;
		}
		if (_rows == null)
		{
			return false;
		}
		if (row >= _rows.Count)
		{
			return false;
		}
		if (col >= _noCols)
		{
			return false;
		}
		Value = _rows[row][col];
		return true;
	}

	public bool ReadValue(int col, int row, float def, out float Value)
	{
		Value = def;
		if (!_loaded)
		{
			return false;
		}
		if (_rows == null)
		{
			return false;
		}
		if (row >= _rows.Count)
		{
			return false;
		}
		if (col >= _noCols)
		{
			return false;
		}
		Value = float.Parse(_rows[row][col]);
		return true;
	}

	public T GetValue<T>(string findRow, int col = 1)
	{
		string[] row = _rows.Find(x => x[0] == findRow);
		if (row != null && row.Length > col)
		{
			return (T)Convert.ChangeType(row[col], typeof(T));
		}

		Debug.LogError("CSVLoader.GetValue: row or column does not exist. " + findRow + ", " + col);
		return default;
	}

	public void SetValue<T>(string findRow, T value, int col = 1, bool extend = true)
	{
		string[] row = _rows.Find(x => x[0] == findRow);
		if (row != null)
		{
			if (row.Length > col)
				row[col] = Convert.ToString(value);
			return;
		}

		if (extend)
		{
			string[] addRow = new string[Cols];
			addRow[0] = findRow;
			if (addRow.Length > col)
				addRow[col] = Convert.ToString(value);
			_rows.Add(addRow);
			return;
		}
		Debug.LogError("CSVLoader.SetValue: row or column does not exist. " + findRow + ", " + col);
	}

	public bool SecuredLoadFromBinaryReader(BinaryReader reader)
	{
		try
		{
			_SecuredLoadFromBinaryReader(reader);
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message.ToString());
			return false;
			IL_0024:;
		}
		_loaded = true;
		return true;
	}

	private void _SecuredLoadFromBinaryReader(BinaryReader reader)
	{
		int num = reader.ReadInt32();
		_rows = new List<string[]>();
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			num2 = reader.ReadInt32();
			string[] array = new string[num2];
			for (int j = 0; j < num2; j++)
			{
				int num3 = reader.ReadInt32();
				if (num3 > 0)
				{
					char[] array2 = reader.ReadChars(num3);
					for (int k = 0; k < num3; k++)
					{
						array2[k] ^= crypt;
					}
					array[j] = new string(array2, 0, num3);
				}
				else
				{
					array[j] = string.Empty;
				}
			}
			_rows.Add(array);
		}
		_noCols = num2;
	}

	public bool SecuredLoad(string pathName)
	{
		pathName += ".cooked";
		try
		{
			_noCols = 0;
			FileStream fileStream = File.Open(pathName, FileMode.Open, FileAccess.Read);
			BinaryReader binaryReader = new BinaryReader(fileStream);
			_SecuredLoadFromBinaryReader(binaryReader);
			binaryReader.Close();
			fileStream.Close();
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message.ToString());
			return false;
			IL_0054:;
		}
		_loaded = true;
		return true;
	}

	public bool SecuredSave(string pathName)
	{
		if (!_loaded || _rows == null)
		{
			Debug.LogError("Fail to save, CSV is not loaded yet");
			return false;
		}
		pathName += ".cooked";
		try
		{
			FileStream fileStream = File.Open(pathName, FileMode.Create, FileAccess.Write);
			BinaryWriter binaryWriter = new BinaryWriter(fileStream);
			binaryWriter.Write(_rows.Count);
			for (int i = 0; i < _rows.Count; i++)
			{
				binaryWriter.Write(_rows[i].Length);
				for (int j = 0; j < _rows[i].Length; j++)
				{
					char[] array = _rows[i][j].ToCharArray();
					int num = (array != null) ? array.Length : 0;
					for (int k = 0; k < num; k++)
					{
						array[k] ^= crypt;
					}
					binaryWriter.Write(num);
					if (num > 0)
					{
						binaryWriter.Write(array, 0, num);
					}
				}
			}
			binaryWriter.Close();
			fileStream.Close();
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message.ToString());
			return false;
			IL_0139:;
		}
		return true;
	}

	public bool Save(string pathName, string header = "x\tCode")
	{
		if (!_loaded || _rows == null)
		{
			Debug.LogError("Fail to save, CSV is not loaded yet");
			return false;
		}
		try
		{
			File.WriteAllText(pathName, "");
			FileStream fileStream = File.Open(pathName, FileMode.Create, FileAccess.Write);
			StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.ASCII);
			streamWriter.BaseStream.Seek(0L, SeekOrigin.Begin);
			if (header != "")
				streamWriter.WriteLine(header);
			for (int row = 0; row < Rows; row++)
			{
				string rowString = "";
				for (int col = 0; col < Cols; col++)
				{
					rowString += _rows[row][col];
					if (col < Cols - 1)
						rowString += "\t";
				}
				streamWriter.WriteLine(rowString);
			}
			streamWriter.Close();
			fileStream.Close();
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message.ToString());
			return false;
		IL_0139:;
		}
		return true;
	}

	public bool Load(string pathName)
	{
		try
		{
			FileStream fileStream = File.OpenRead(pathName);
			StreamReader streamReader = new StreamReader(fileStream, Encoding.ASCII);
			_rows = new List<string[]>();
			streamReader.BaseStream.Seek(0L, SeekOrigin.Begin);
			_noCols = 0;
			HashSet<int> hashSet = new HashSet<int>();
			int num = 0;
			while (streamReader.Peek() > -1)
			{
				string text = streamReader.ReadLine();
				text.Trim();
				if (text.Length > 0)
				{
					string[] array = text.Split('\t');
					if (num++ == 0)
					{
						for (int i = 0; i < array.Length; i++)
						{
							if (array[i].Length > 0 && !array[i].StartsWith("*"))
							{
								hashSet.Add(i);
								_noCols++;
							}
						}
						if (_noCols == 0)
						{
							throw new Exception("There is no valid columns");
						}
					}
					else
					{
						int num3 = 0;
						string[] array2 = new string[_noCols];
						for (int j = 0; j < array.Length; j++)
						{
							if (hashSet.Contains(j))
							{
								array[j] = array[j].Replace('"', ' ');
								array[j] = array[j].Trim();
								array2[num3++] = array[j];
							}
						}
						_rows.Add(array2);
					}
				}
			}
			streamReader.Close();
			fileStream.Close();
			_loaded = true;
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message.ToString());
			return false;
			IL_01a3:;
		}
		return true;
	}

	public bool cmsLoad(string Filename)
	{
		string text = Filename + ".encrypt";
		string sOutputFilename = Filename + ".decrypt";
		EncryptFile(Filename, text, "Password");
		DecryptFile(text, sOutputFilename, "Password");
		return false;
	}

	public void EncryptFile(string sInputFilename, string sOutputFilename, string sKey)
	{
		FileStream fileStream = new FileStream(sInputFilename, FileMode.Open, FileAccess.Read);
		FileStream fileStream2 = new FileStream(sOutputFilename, FileMode.Create, FileAccess.Write);
		DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
		dESCryptoServiceProvider.Key = Encoding.ASCII.GetBytes(sKey);
		dESCryptoServiceProvider.IV = Encoding.ASCII.GetBytes(sKey);
		ICryptoTransform transform = dESCryptoServiceProvider.CreateEncryptor();
		CryptoStream cryptoStream = new CryptoStream(fileStream2, transform, CryptoStreamMode.Write);
		byte[] array = new byte[fileStream.Length];
		fileStream.Read(array, 0, array.Length);
		cryptoStream.Write(array, 0, array.Length);
		cryptoStream.Close();
		fileStream.Close();
		fileStream2.Close();
	}

	public void DecryptFile(string sInputFilename, string sOutputFilename, string sKey)
	{
		DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
		dESCryptoServiceProvider.Key = Encoding.ASCII.GetBytes(sKey);
		dESCryptoServiceProvider.IV = Encoding.ASCII.GetBytes(sKey);
		FileStream stream = new FileStream(sInputFilename, FileMode.Open, FileAccess.Read);
		ICryptoTransform transform = dESCryptoServiceProvider.CreateDecryptor();
		CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read);
		StreamWriter streamWriter = new StreamWriter(sOutputFilename);
		streamWriter.Write(new StreamReader(stream2).ReadToEnd());
		streamWriter.Flush();
		streamWriter.Close();
	}
}
