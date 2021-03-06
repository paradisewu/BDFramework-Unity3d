﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
public class IEnumeratorTool : MonoBehaviour
{
    /// <summary>
    /// 压入的action任务
    /// </summary>
    public class ActionTask
    {
        public Action m_willDoAction;
        public Action m_callBackAction;
    }
    /// <summary>
    /// 任务队列
    /// </summary>
    static Queue<ActionTask> m_actionTaskQueue = new Queue<ActionTask>();

    /// <summary>
    /// 执行任务
    /// </summary>
    /// <param name="action"></param>
    /// <param name="callBack"></param>
    static public void ExecAction(Action action,Action callBack =null)
    {
        var task = new ActionTask()
        {
            m_willDoAction = action,
            m_callBackAction = callBack
        };

        
        m_actionTaskQueue.Enqueue(task);
    }


    /// <summary>
    /// 任务队列
    /// </summary>
    static Queue<ActionTask> m_actionTaskQueueImmediately = new Queue<ActionTask>();
    /// <summary>
    /// 立即执行
    /// </summary>

    static public void ExecActionImmediately(Action action, Action callBack = null)
    {
        var task = new ActionTask()
        {
            m_willDoAction = action,
            m_callBackAction = callBack
        };


        m_actionTaskQueueImmediately.Enqueue(task);
    }

    //
    static Dictionary<int, IEnumerator> m_IEnumeratorDictionary = new Dictionary<int, IEnumerator>();
    static Dictionary<int, Coroutine> m_CoroutineDictionary = new Dictionary<int, Coroutine>();
    static Queue<int> m_IEnumeratorQueue = new Queue<int>();
    static int counter = -1;
    static public new int StartCoroutine (IEnumerator ie)
    {
        counter++;
        m_IEnumeratorQueue.Enqueue(counter);
        m_IEnumeratorDictionary[counter] = ie;
        return counter;
    }

    static Queue<int> m_StopIEIdQueue = new Queue<int>();
    static public void StopCroutine(int id)
    {
        m_StopIEIdQueue.Enqueue(id);
    }
       
   static private bool isStopAllCroutine = false;
   /// <summary>
   /// 停止携程
   /// </summary>
    static public void StopAllCroutine()
    {
        isStopAllCroutine = true;
    }

#region Tools

    /// <summary>
    /// 等待一段时间后执行
    /// </summary>
    /// <param name="f"></param>
    /// <param name="action"></param>
   static public void WaitingForExec(float f, Action action)
    {
        StartCoroutine(IE_WaitingForExec(f, action));
    }

  static  private IEnumerator IE_WaitingForExec(float f, Action action)
    {
        yield return new WaitForSeconds(f);
        if(action!=null)
          action();
        yield break;
    }
    #endregion
    /// <summary>
    /// 主循环
    /// </summary>
    void Update()
    {
        //停止所有携程
        if (isStopAllCroutine) {
             BDeBug.I.Log("停止所有携程");
            StopAllCoroutines();
            isStopAllCroutine = false;
        }
        //优先停止携程
        while (m_StopIEIdQueue.Count > 0) {
            var id = m_StopIEIdQueue.Dequeue();
            if (m_CoroutineDictionary.ContainsKey(id)) {
                var coroutine = m_CoroutineDictionary[id];
                base.StopCoroutine(coroutine);
                //
                m_CoroutineDictionary.Remove(id);
            } else {
                Debug.LogErrorFormat("此id协程不存在,无法停止:{0}", id);
            }
        }

        //携程循环
        if (m_IEnumeratorQueue.Count > 0) {
            var id = m_IEnumeratorQueue.Dequeue();
            //取出携程
            var ie = m_IEnumeratorDictionary[id];
            m_IEnumeratorDictionary.Remove(id);
            //执行携程
            var coroutine = base.StartCoroutine(ie);

            //存入coroutine
            m_CoroutineDictionary[id] = coroutine;
        }

        //主线程循环 立即执行
        while (m_actionTaskQueueImmediately.Count > 0) {

            var task = m_actionTaskQueueImmediately.Dequeue();
            task.m_willDoAction();
            if (task.m_callBackAction != null) {
                task.m_callBackAction();
            }
        }

        //主线程循环
        if (m_actionTaskQueue.Count > 0) {

            var task = m_actionTaskQueue.Dequeue();
            task.m_willDoAction();
            if (task.m_callBackAction != null) {
                task.m_callBackAction();
            }
        }
    }

}
