/// <summary>
/// 番場 宥輝
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yuuki.Sound
{
    [System.Serializable] public class SoundTable : TableBase<string, AudioClip, SoundPair> { };
    [System.Serializable]
    public class SoundPair : KeyAndValue<string, AudioClip>
    {

        public SoundPair(string key, AudioClip value) : base(key, value) { }
    }

    /// <summary>
    /// 再生する音のリスト
    /// </summary>
    public class SoundList : MonoBehaviour
    {

        //  serialize param!
        [SerializeField, Tooltip("AudioClipを格納するテーブル")] SoundTable table;
        [SerializeField, Tooltip("ONにするとAwake関数でSoundManagerにSoundListを登録する")] bool isDefaultSet = true;

        //  property!
        public SoundTable Table { get { return table; } }

        // Use this for initialization
        void Awake()
        {

            //リストセット
            if (isDefaultSet)
            {
                SoundManager.Instance.ClearSoundsList();
                SoundManager.Instance.SetSoundList(this);
            }
        }
    }
}
