/// <summary>
/// 番場 宥輝
/// </summary>
using System;
using UnityEngine;

/// <summary>
/// 拡張メソッドの名前空間
/// </summary>
namespace Yuuki.MethodExpansions
{
    /// <summary>
    /// System.Actionの拡張クラス
    /// </summary>
    public static class ActionExpansion
    {
        /// <summary>
        /// Actionメソッドの安全な呼び出し
        /// @NOTE nullなら処理しない
        ///       ?.Invoke()のラッパー
        /// </summary>
        /// <param name="action"></param>
        public static void SafeInvoke(this Action action)
        {
            action?.Invoke();
        }
    }
}
