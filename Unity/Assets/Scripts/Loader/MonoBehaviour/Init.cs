﻿using System;
using System.Collections;
using CommandLine;
using UnityEngine;
using YooAsset;

namespace ET
{
	public class Init: MonoBehaviour
	{
		// /// <summary>
		// /// 资源系统运行模式
		// /// </summary>
		// public bool IsUseEditorMode = true;
		public EPlayMode PlayMode;
		private IEnumerator Start()
		{
			DontDestroyOnLoad(gameObject);
			
			AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
			{
				Log.Error(e.ExceptionObject.ToString());
			};
				
			Game.AddSingleton<MainThreadSynchronizationContext>();

			// 命令行参数
			string[] args = "".Split(" ");
			Parser.Default.ParseArguments<Options>(args)
				.WithNotParsed(error => throw new Exception($"命令行格式错误! {error}"))
				.WithParsed(Game.AddSingleton);
			
			Game.AddSingleton<TimeInfo>();
			Game.AddSingleton<Logger>().ILog = new UnityLogger();
			Game.AddSingleton<ObjectPool>();
			Game.AddSingleton<IdGenerater>();
			Game.AddSingleton<EventSystem>();
			Game.AddSingleton<TimerComponent>();
			Game.AddSingleton<CoroutineLockComponent>();
			
			ETTask.ExceptionHandler += Log.Error;

			if (!Application.isEditor && PlayMode == EPlayMode.EditorSimulateMode)
			{
				PlayMode = EPlayMode.HostPlayMode;
			}
			yield return Game.AddSingleton<MonoResComponent>().InitAsync(PlayMode);
			Game.AddSingleton<CodeLoader>().Start();
		}

		private void Update()
		{
			Game.Update();
		}

		private void LateUpdate()
		{
			Game.LateUpdate();
			Game.FrameFinishUpdate();
		}

		private void OnApplicationQuit()
		{
			Game.Close();
		}
	}
}