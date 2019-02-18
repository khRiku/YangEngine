using System.Collections;
using UnityEngine;
using System;
using System.Reflection;
using UnityEngine.Networking;

public class HotFixManager
{
	public HotFixManager CreateInstance()
	{
		HotFixManager tInstance = new HotFixManager();

		return tInstance;
	}


	private Assembly mHotFixAssembly;

	#region 特例外部接口
	/// <summary>
	/// StartupManager 调用的函数
	/// </summary>
	public void CallInStartupManager(Action pToNextParseAction)
	{
		AppLauncher.Instance.StartCoroutine(LoadDll());
	}

	#endregion

	#region 功能函数
	private IEnumerator LoadDll()
	{
		string pDllPath = GetDllPath();
		using (UnityWebRequest tDllRequest = UnityWebRequest.Get(pDllPath))
		{           
			yield return tDllRequest.SendWebRequest();
			byte[] tDllByte = tDllRequest.downloadHandler.data;

			string tPdbPath = GetPdbPath();
			using (UnityWebRequest tPdbRequest = UnityWebRequest.Get(tPdbPath))
			{
				yield return tPdbRequest.SendWebRequest();
				byte[] tPdbByte = tPdbRequest.downloadHandler.data;

				mHotFixAssembly = Assembly.Load(tDllByte, tPdbByte);
			}
		}

		yield return null;


		Type tHotFix = mHotFixAssembly.GetType("Hotfix.Hotfix");
		MethodInfo tMethod = tHotFix.GetMethod("Run", BindingFlags.NonPublic | BindingFlags.Static);

		if (tMethod == null)
		{
			Debug.LogError("获取不到类型");
			yield return null;
		}
		tMethod.Invoke(null, null);
	}

	private string GetDllPath()
	{        
	   return string.Format("{0}/../HotFix/bin/Debug/HotFix.dll", Application.dataPath);
	}

	private string GetPdbPath()
	{
		return string.Format("{0}/../HotFix/bin/Debug/HotFix.dll.mdb", Application.dataPath);
	}

	#endregion
}
