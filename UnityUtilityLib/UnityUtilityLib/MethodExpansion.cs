/// <summary>
/// 番場 宥輝
//  参考：http://kan-kikuchi.hatenablog.com/entry/DelayMethod
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

/// <summary>
/// 拡張メソッドの名前空間
/// </summary>
namespace Yuuki.MethodExpansions
{
    /// <summary>
    /// 関数の拡張
    /// </summary>
    public static class MethodExpansion
    {
        /// <summary>
        /// コールバック
        /// </summary>
        public delegate void Function();

        /// <summary>
        /// 指定フレーム遅延後に関数の実行を行う
        /// </summary>
        /// <param name="self"></param>
        /// <param name="func"></param>
        /// <param name="frame"></param>
        public static void DelayMethod(this MonoBehaviour self, Function func, float frame)
        {
            CoroutineExpansion.CoroutineFinishedFunc cast = () => { func(); };
            CoroutineExpansion.StartCoroutine(self, WaitCoroutine(self, frame), cast);
        }

        /// <summary>
        /// 指定フレーム何もしないコルーチン
        /// </summary>
        /// <param name="self"></param>
        /// <param name="waitFrame"></param>
        /// <returns></returns>
        public static IEnumerator WaitCoroutine(this MonoBehaviour self,float waitFrame)
        {
            for(int i = 0; i < waitFrame; ++i) { yield return null; }
        }

        /// <summary>
        /// Reflectionを使った関数の呼び出し
        /// ※呼び出し側の関数がpublicでないと呼び出しは出来ない
        /// </summary>
        /// <param name="self"></param>
        /// <param name="funcName">呼び出す関数名</param>
        public static void CallMethod(this object self,string funcName)
        {
            //  メソッドデータの作成
            MethodInfo info = self.GetType().GetMethod(funcName);

            //  メソッドが読み込めれば実行
            if (info != null)
            {
                info.Invoke(self, null);
            }
            //  読み込めなければエラーを表示
            else
            {
                Debug.LogError("The function could not be called!\nFunction name is \"" + "<color=red>" + funcName + "</color>" + "\"");
            }
        }

        /// <summary>
        /// Reflectionを使った関数の呼び出し
        /// ※呼び出し側の関数がpublicでないと呼び出しは出来ない
        /// </summary>
        /// <param name="self"></param>
        /// <param name="funcName">呼び出す関数名</param>
        /// <param name="argument">関数の引数</param>
        public static void CallMethod(this object self,string funcName, object[] argument)
        {
            //  メソッドデータの作成
            MethodInfo info = self.GetType().GetMethod(funcName);

            //  メソッドが読み込めれば実行
            //  TargetParameterCountException が表示された場合引数が不適切
            if (info != null)
            {
                info.Invoke(self, argument);
            }
            else
            {
                Debug.LogError("The function could not be called!\nFunction name is \"" + "<color=red>" + funcName + "</color>" + "\"");
            }
        }

        /// <summary>
        /// Reflectionを使った関数の呼び出し
        /// (ジェネリック版)
        /// サンプルを記載
        /// </summary>
        /// <typeparam name="Type">引数の型を明示的に示す省略可能</typeparam>
        /// <param name="funcName">呼び出す関数名</param>
        /// <param name="argument">関数の引数</param>
        public static void CallMethod<Type>(this object self, string funcName, Type[] argument)
        {
            //  メソッドデータの作成
            MethodInfo info = self.GetType().GetMethod(funcName);

            //  メソッドが読み込めれば実行
            //  TargetParameterCountException が表示された場合引数が不適切
            if (info != null)
            {
                object[] tmp = new object[argument.Length];
                argument.CopyTo(tmp, 0);
                info.Invoke(self, tmp);
            }
            else
            {
                Debug.LogError("The function could not be called!\nFunction name is \"" + "<color=red>" + funcName + "</color>" + "\"");
            }
        }
        /// <example>サンプルコード
        /// <code>
        /// class sample
        /// {
        ///     void main()
        ///     {
        ///         //    
        ///         this.CallMethod<int>("hoge",new int[]{0});//出力結果 0
        /// 
        ///         //↓省略版
        ///         this.CallMethod("hoge",new []{0});//出力結果 0
        ///     }
        ///     
        ///     public void hoge(int param)
        ///     {
        ///         Debug.Log(param);
        ///     }
        /// }
        /// </code>
        /// </example>

        /// <summary>
        /// Reflectionを使った関数の呼び出し
        /// ※呼び出す関数がpublicである必要がある
        /// </summary>
        /// <param name="obj">バインドするオブジェクト</param>
        /// <param name="funcName">呼び出す関数名</param>
        public static void CallMethod(this object self, object obj, string funcName)
        {
            //  メソッドデータの作成
            MethodInfo info = obj.GetType().GetMethod(funcName);

            //  メソッドが読み込めれば実行
            if (info != null)
            {
                info.Invoke(obj, null);
            }
            //  読み込めなければエラーを表示
            else
            {
                Debug.LogError("The function could not be called!\nFunction name is \"" + "<color=red>" + funcName + "</color>" + "\"");
            }
        }

        /// <summary>
        /// Reflectionを使った関数の呼び出し
        /// ※呼び出す関数がpublicである必要がある
        /// </summary>
        /// <param name="obj">バインドするオブジェクト</param>
        /// <param name="funcName">呼び出す関数名</param>
        /// <param name="argument">関数の引数</param>
        public static void CallMethod(this object self,object obj,string funcName,object[] argument)
        {
            //  メソッドデータの作成
            MethodInfo info = obj.GetType().GetMethod(funcName);

            //  メソッドが読み込めれば実行
            //  TargetParameterCountException が表示された場合引数が不適切
            if (info != null)
            {
                info.Invoke(obj, argument);
            }
            //  読み込めなければエラーを表示
            else
            {
                Debug.LogError("The function could not be called!\nFunction name is \"" + "<color=red>" + funcName + "</color>" + "\"");
            }

        }

        /// <summary>
        /// Reflectionを使った関数の呼び出し
        /// (ジェネリック版)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Type">引数の型を明示的に示す省略可能</typeparam>
        /// <param name="obj">バインドするオブジェクト</param>
        /// <param name="funcName">呼び出す関数名</param>
        /// <param name="argument">関数の引数</param>
        public static void CallMethod<T>(this object self, object obj, string funcName, T[] argument)
        {
            //  メソッドデータの作成
            MethodInfo info = obj.GetType().GetMethod(funcName);

            //  メソッドが読み込めれば実行
            //  TargetParameterCountException が表示された場合引数が不適切
            if (info != null)
            {
                object[] tmp = new object[argument.Length];
                argument.CopyTo(tmp, 0);
                info.Invoke(obj, tmp);
            }
            //  読み込めなければエラーを表示
            else
            {
                Debug.LogError("The function could not be called!\nFunction name is \"" + "<color=red>" + funcName + "</color>" + "\"");
            }

        }

    }
}
