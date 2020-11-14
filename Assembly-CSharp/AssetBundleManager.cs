using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class AssetBundleManager
{
	private static Dictionary<string, AssetBundleRef> dictAssetBundleRefs;

	static AssetBundleManager()
	{
		dictAssetBundleRefs = new Dictionary<string, AssetBundleRef>();
	}

	public static AssetBundle getAssetBundle(string url, int version)
	{
		string key = url + version.ToString();
		if (dictAssetBundleRefs.TryGetValue(key, out AssetBundleRef value))
		{
			return value.assetBundle;
		}
		return null;
	}

	public static IEnumerator downloadAssetBundle(string file, string url, int version)
	{
		string keyName = url + version.ToString();
		if (dictAssetBundleRefs.ContainsKey(keyName))
		{
			yield return (object)null;
		}
		else
		{
			WWW www = new WWW(file);
			if (www.bytes.Length == 0)
			{
				www = new WWW(url);
				yield return (object)www;
				SaveAssetBundle(www.bytes, file);
			}
			AssetBundleRef abRef = new AssetBundleRef(url, version)
			{
				assetBundle = www.assetBundle
			};
			dictAssetBundleRefs.Add(keyName, abRef);
		}
	}

	public static void Unload(string url, int version, bool allObjects)
	{
		string key = url + version.ToString();
		if (dictAssetBundleRefs.TryGetValue(key, out AssetBundleRef value))
		{
			value.assetBundle.Unload(allObjects);
			value.assetBundle = null;
			dictAssetBundleRefs.Remove(key);
		}
	}

	public static void SaveAssetBundle(byte[] bytes, string fileName)
	{
		string path = Path.Combine(Application.dataPath, "Resources/");
		string[] array = fileName.Split(new char[1]
		{
			'/'
		}, StringSplitOptions.RemoveEmptyEntries);
		string path2 = Path.Combine(path, array[array.Length - 1]);
		FileStream fileStream = File.Open(path2, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream);
		binaryWriter.Write(bytes, 0, bytes.Length);
		binaryWriter.Close();
		fileStream.Close();
	}
}
