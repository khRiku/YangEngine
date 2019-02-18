using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AppLauncher : MonoBehaviour
{

	public static AppLauncher Instance = null;

    public Action mUpdateAction;
	// Use this for initialization
	void Start ()
	{
		Init();

		//启动游戏
		ManagerIndex.StartupManagr.RunLogic();
	}

	private void Init()
	{
		Instance = this;
	}

    private void Update()
    {
        if (mUpdateAction != null)
            mUpdateAction();
    }

}
