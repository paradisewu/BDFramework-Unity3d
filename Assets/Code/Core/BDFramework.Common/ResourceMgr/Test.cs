﻿using UnityEngine;
using System.Collections;
using BDFramework.ResourceMgr;
using UnityEngine.UI;
using System.Collections.Generic;
public class Test : MonoBehaviour
{
    IVersionControl versionControl;
    void Start()
    {
        ///*
        //* 版本控制设置
        //* 
        //*/
        //versionControl = new VersionControl(); //step.1 新建增量包版本控制对象
        //versionControl.ResServerAddress = "http://api-resource.ptdev.cn/v1/res";  //step.2  设置接入服务器
        //versionControl.LocalHotUpdateResPath = Application.persistentDataPath + "/hot";  //step.3设置

        ////c盘有权限问题 更改如下
        //versionControl.LocalHotUpdateResPath = "E:/Hot";

        ///*
        //* 资源管理设置
        //* 
        //*/
        //JResources.SetLocalPath(versionControl.LocalHotUpdateResPath); //step.1 设置本地目录 如不设置，以后全部传入绝对路径
    }
    bool isLoadCanvas = false;
    void OnGUI()
    {
		if (GUILayout.Button("测试拷贝streamingasset", GUILayout.Height(50),GUILayout.Width(300)))
        {
            //多层目录

            //versionControl.CopyStreamingAsset(1.8f, () =>
            //{

            //     JDeBug.I.Log("拷贝完成!");
            //});
        }

		if (GUILayout.Button("下载测试",  GUILayout.Height(50),GUILayout.Width(300)))
        {
            ////启动版本控制
            //versionControl.Start("3", "100", "V0uFhE2GRNnRipS0hery9OhY", (float state, string info, bool issuccess) =>
            //{

            //     JDeBug.I.Log("业务逻辑层通知：" + "下载完成");
            //     JDeBug.I.Log("本地目录：" + versionControl.LocalHotUpdateResPath);
            //});
        }

        if (GUILayout.Button("editor模式:[异步]加载依测试", GUILayout.Height(50), GUILayout.Width(300)))
        {
            BResources.IsAssetBundleModel = false; //切换为 resource.load加载模式
            //单层目录
            BResources.LoadAsync<GameObject>("Canvas", (bool issuccess, GameObject o) =>
            {
                if (issuccess)
                    GameObject.Instantiate(o);
            });
        }

		if (GUILayout.Button ("ab模式:加载依赖文件",  GUILayout.Height(50),GUILayout.Width(300)))
		{
            BResources.IsAssetBundleModel = true; //切换为 ab加载模式
			BResources.LoadManifestAsync("AllResources", (bool _issuccess) => //step.2 全局加载依赖 只需加载一次。
				{
					if (_issuccess)
					{
						 BDeBug.I.Log("依赖加载成功!");
					}
				});
		}
		if (GUILayout.Button("ab模式:[异步]加载1级目录", GUILayout.Height(50),GUILayout.Width(300)))
        {
            BResources.IsAssetBundleModel = true; //切换为 ab加载模式

            //单层目录
            BResources.LoadAsync<GameObject>("Canvas", (bool issuccess, GameObject o) =>
            {
                if (issuccess)
                {
                    isLoadCanvas = true;
                    GameObject.Instantiate(o);
                }                
            });
        }

        if (isLoadCanvas)
        {
            if (GUILayout.Button("ab模式:卸载canvas", GUILayout.Height(50), GUILayout.Width(300)))
            {
                BResources.UnloadAsset("Canvas");
            }
        }
		if (GUILayout.Button("ab模式:[异步]加载多级目录",  GUILayout.Height(50),GUILayout.Width(300)))
        {
            BResources.IsAssetBundleModel = true; //切换为 ab加载模式
            //多层目录
            BResources.LoadAsync<GameObject>("test1/test", (bool issuccess, GameObject o) =>
            {
                if (issuccess)
                {
                    GameObject.Instantiate(o);
                }
            });
        }


        if (GUILayout.Button("ab模式:[异步]加载一组", GUILayout.Height(50), GUILayout.Width(300)))
        {
            BResources.IsAssetBundleModel = true; //切换为 ab加载模式

            IList<string> list = new List<string>() { "Canvas", "test1/test" };
            //多层目录
            var task =   BResources.LoadAsync(list, (IDictionary<string, UnityEngine.Object> resmap) =>
            {

                var o1 = GameObject.Instantiate(resmap["Canvas"]);
                var o2 = GameObject.Instantiate(resmap["test1/test"]);

            });

            BResources.LoadCancel(task);
        }

        if (GUILayout.Button("ab模式:[异步]取消加载请求", GUILayout.Height(50), GUILayout.Width(300)))
        {
            BResources.IsAssetBundleModel = true; //切换为 ab加载模式
            //多层目录
           var taskid =  BResources.LoadAsync<GameObject>("test1/test", (bool issuccess, GameObject o) =>
            {
                if (issuccess)
                    GameObject.Instantiate(o);
            });

           BResources.LoadCancel(taskid);
        }

    }

}
