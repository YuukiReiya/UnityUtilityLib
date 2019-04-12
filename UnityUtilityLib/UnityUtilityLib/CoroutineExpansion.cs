using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

/// <summary>
/// 拡張メソッドの名前空間
/// </summary>
namespace Yuuki.MethodExpansions
{
    /// <summary>
    /// コルーチンの拡張
    /// </summary>
    public static class CoroutineExpansion
    {

        public delegate void CoroutineFinishedFunc();

        /// <summary>
        /// StartCoroutineのオーバーロード
        /// コルーチンの終了後に関数を実行
        /// </summary>
        /// <param name="self"></param>
        /// <param name="routine"></param>
        /// <param name="func"></param>
        public static void StartCoroutine(this MonoBehaviour self, IEnumerator routine, CoroutineFinishedFunc func)
        {
            self.StartCoroutine(DelayRoutine(self, routine, func));
        }

        /// <summary>
        /// コルーチンの終了後に関数を実行するためのコルーチン
        /// </summary>
        /// <param name="mono"></param>
        /// <param name="routine"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        private static IEnumerator DelayRoutine(MonoBehaviour mono, IEnumerator routine, CoroutineFinishedFunc func)
        {
            yield return mono.StartCoroutine(routine);
            if (func != null) { func(); }
        }
    }
}
