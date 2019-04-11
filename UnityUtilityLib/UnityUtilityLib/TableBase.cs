/// <summary>
/// 番場 宥輝
//https://qiita.com/k_yanase/items/fb64ccfe1c14567a907d
//https://doruby.jp/users/ino/entries/%E3%80%90C--Unity%E3%80%91LINQ%E3%82%92%E4%BD%BF%E3%81%A3%E3%81%A6%E3%81%BF%E3%81%9F%E3%82%89%E3%81%A8%E3%81%A6%E3%82%82%E3%82%B9%E3%83%9E%E3%83%BC%E3%83%88%E3%81%A0%E3%81%A3%E3%81%9F
//上記のサイト参考
/// </summary>
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



namespace Yuuki
{
    /// <summary>
    /// 視覚化したいテーブルの基底クラス
    /// </summary>
    [System.Serializable]
    public class TableBase<TKey, TValue, Type> where Type : KeyAndValue<TKey, TValue>
    {
        //private param
        [SerializeField] List<Type> list;

        //accessory
        Dictionary<TKey, TValue> table { get; set; }

        //property
        public List<Type> List { get { return list; } }
        public Dictionary<TKey, TValue> Table
        {
            get
            {
                //null check
                if (table == null)
                {
                    //convert List to Dictionary
                    table = ConvertListToDictionary(list);
                }
                return table;
            }
        }

        /// <summary>
        /// ListからDictionaryへ変換
        /// </summary>
        /// <param name="list">変換するリスト</param>
        /// <returns>変換したDictionary</returns>
        static private Dictionary<TKey, TValue> ConvertListToDictionary(List<Type> list)
        {
            Dictionary<TKey, TValue> dic = new Dictionary<TKey, TValue>();
            foreach(KeyAndValue<TKey,TValue> it in list) { dic.Add(it.key, it.value); }
            return dic;
        }

    }

    /// <summary>
    /// シリアル化して視覚化するテーブルの要素(キーと値)
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [System.Serializable]
    public class KeyAndValue<TKey, TValue>
    {
        public TKey key;
        public TValue value;

        /// <summary>
        /// 空コンストラクタ
        /// </summary>
//        public KeyAndValue() { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="key">登録キー</param>
        /// <param name="value">登録値</param>
        public KeyAndValue(TKey key,TValue value)
        {
            this.key = key;
            this.value = value;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="pair">登録するペア</param>
        public KeyAndValue(KeyAndValue<TKey, TValue> pair)
        {
            key = pair.key;
            value = pair.value;
        }
    }
}

////////////////////////////////////////////////////////////
//使い方
////////////////////////////////////////////////////////////
/*
    ・使用先でKeyAndValueを継承したクラスを作成(コンストラクタをオーバーライドする)
    ・使用先でTableBaseを継承した空のクラスを作成(Typeに上記のクラスを渡す) 
    ・使用するクラスでTableBaseを継承したクラスを宣言
*/
