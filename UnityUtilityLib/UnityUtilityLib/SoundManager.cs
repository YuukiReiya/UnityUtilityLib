using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// サウンドマネージャー
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SoundManager : SingletonMonoBehaviour<SoundManager> {

    //  serialize param
    [Header("Sound Manager Parameter")]
    [SerializeField, Tooltip("チャネル数"), Range(1, 16)] int channelNum = 1;
    [SerializeField, Tooltip("ONならメインカメラではなく、このオブジェクトがListenerを担当する")] bool isAttachedListener = false;

    //  private param!
    AudioSource[] channel;
    SoundTable sounds;

    //  accessor
    public bool isFade { get { return state != FadeState.None; } }

    //  enum
    private enum FadeState { None, FadeIn, FadeOut }
    private FadeState state;

    //  CallBack
    public delegate void FadeInFinishedFunc();
    public delegate void FadeOutFinishedFunc();

    /// <summary>
    /// Awake
    /// </summary>
    protected override void Awake()
    {
        //基底クラスの処理
        base.Awake();

        //Listenerのセット
        SetListener();
        //チャンネルの生成
        CreateChannel();
        //チャンネルの取得
        SetChannel();
        //フェード状況の初期化
        state = FadeState.None;
    }

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /********************/
    /* private function */
    /********************/

    /// <summary>
    /// AudioListenerのセット
    /// </summary>
    [ContextMenu("SetAudioListener")]
    void SetListener()
    {
        //AudioListenerを設定

        //メインカメラの参照が取得できない場合(カメラのタグをMainCameraに変えてしまうので注意)
        if (Camera.main == null)
        {
            GameObject.Find("Main Camera").tag = "MainCamera";
        }

        AudioListener destroy   = null;//破棄させるListener
        GameObject add          = null;//Listenerを追加するGameObject

        //Listenerをこのオブジェクトに付属させる
        if (isAttachedListener)
        {
            //カメラにListenerが付いていたら破棄オブジェクトにする
            destroy = Camera.main.GetComponent<AudioListener>() ?? null;

            //オブジェクトにListenerが付いていなかったら、このオブジェクトをListenerを付与するオブジェクトにする
            add = GetComponent<AudioListener>() ? null : this.gameObject;
        }
        //Listenerをメインカメラに付属させる
        else
        {
            //このオブジェクトにListenerが付いていたら破棄オブジェクトにする
            destroy = GetComponent<AudioListener>() ?? null;

            //カメラにListenerが付いていなかったら、メインカメラをListenerを付与するオブジェクトにする
            add = Camera.main.GetComponent<AudioListener>() ? null : Camera.main.gameObject;
        }

        //破棄するListenerが存在すればListenerを破棄する
        if (destroy)
        {
#if UNITY_EDITOR
            DestroyImmediate(destroy);
#else
            Destroy(destroy);
#endif
        }

        //Listenerが付いていなかったらオブジェクトに追加する
        if (add) { add.AddComponent<AudioListener>(); }
    }

    /// <summary>
    /// チャンネルの用意
    /// </summary>
    [ContextMenu("SetChannel")]
    void CreateChannel()
    {
        AudioSource[] sources = this.gameObject.GetComponents<AudioSource>();
        int createCount = 0;

        //一つもAudioSourceがアタッチされてない場合
        if (sources == null)
        {
            createCount = channelNum;
        }
        //AudioSourceの個数がチャネル数と一致しない場合
        else if (sources.Length != channelNum)
        {
            //コンポーネントの個数を1個にする(RequireComponentで1個は付いてしまうため)
            for (int i = 0; i < sources.Length - 1; ++i)
            {
#if UNITY_EDITOR
                GameObject.DestroyImmediate(sources[i]);
#else
                Destroy(sources[i]);
#endif
            }

            //チャネルの1番目のPlayOnAwakeをOFF
            GetComponent<AudioSource>().playOnAwake = false;

            //"チャネル生成数 - 1" を追加数とする
            createCount = channelNum - 1;
        }

        //コンポーネントを追加数分だけ追加する
        for (int i = 0; i < createCount; ++i)
        {
            //追加したコンポーネントを取得
            var audioSource = this.gameObject.AddComponent<AudioSource>();


            //PlayOnAwakeをOFFにする
            audioSource.playOnAwake = false;
        }
    }

    /// <summary>
    /// 現在のチャネル(AudioSource)コンポーネントをセット
    /// </summary>
    void SetChannel()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        channel = sources;
    }

    /// <summary>
    /// チャンネルの範囲外参照を確認し、必要であれば値を丸める
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    int CheckChannelOutOfRange(int index)
    {
        //下回った場合 0 に丸める
        if (index < 0)
        {
            Debug.LogError(
                "channel index is out-of range(min over)" +
                "\n" + 0 + " <= " + index + " < " + channel.Length
                );
            index = 0;
        }
        //上回った場合 チャネルの最大インデックス に丸める
        else if (index >= channel.Length)
        {
            Debug.LogError(
                "channel index is out-of range(max over)" +
                "\n" + 0 + " <= " + index + " < " + channel.Length
                );
            index = channel.Length - 1;
        }

        //チェックし終えた値を返す
        return index;
    }


    /// <summary>
    /// サウンドのインデックスのチェック
    /// </summary>
    /// <param name="index">検査する値</param>
    /// <returns>値が有効かどうか</returns>
    bool CheckSoundsKey(int index)
    {
        //下回った場合
        if (index < 0) {
            Debug.LogError(
                "sounds index is out-of range(min over)" +
                "\n" + 0 + " <= " + index + " < " + sounds.List.Count
                );
            return false;
        }
        //上回った場合
        else if (sounds.List.Count<=index) {

            Debug.LogError(
                "sounds index is out-of range(max over)" +
                "\n" + 0 + " <= " + index + " < " + sounds.List.Count
                );
            return false;
        }

        //値は正常
        return true;
    }

    /// <summary>
    /// サウンドのハッシュキーのチェック
    /// </summary>
    /// <param name="key">検査する値</param>
    /// <returns>値が有効かどうか</returns>
    bool CheckSoundsKey(string key)
    {
        //LINQを使って検査
        bool ret = sounds.Table.Keys.Contains(key);

        if (!ret)
        {
            //検査したハッシュキーが保持するリストに存在しなかった
            Debug.LogError(
                "sounds hash key is not found of this list!" +
                "\n" + "\"" + key + "\"");
        }

        return ret;
    }

    /*******************/
    /* public function */
    /*******************/

    /// <summary>
    /// リストのクリア
    /// </summary>
    public void ClearSoundsList()
    {
        if (sounds==null) { return; }
        sounds.List.Clear();
        sounds.Table.Clear();
    }

    /// <summary>
    /// リストのセット
    /// </summary>
    /// <param name="list"></param>
    public void SetSoundsList(SoundTable list)
    {
        sounds = list;
    }

    /// <summary>
    /// リストのセット
    /// </summary>
    /// <param name="list"></param>
    public void SetSoundList(SoundList list)
    {
        sounds = list.Table;
    }

    /// <summary>
    /// 再生
    /// </summary>
    /// <param name="key">登録されたサウンドリストのハッシュキー</param>
    /// <param name="channelIndex">鳴らすチャネルの番号</param>
    /// <param name="isLoop">ループ有無</param>
    public void Play(string key, int channelIndex = 0, bool isLoop = false)
    {
        //インデックスの範囲外参照をしていた場合値を丸める処理
        channelIndex = CheckChannelOutOfRange(channelIndex);

        //ハッシュのチェック
        if (!CheckSoundsKey(key)) {
            
            //ハッシュキーが見つからなかった場合は関数を実行しない
            Debug.LogError(
                "not found hash key!" + "\nSo not run this function"
                );
            return;
        }

        //再生処理
        channel[channelIndex].clip = sounds.Table[key];
        channel[channelIndex].Play();
        channel[channelIndex].loop = isLoop;
    }

    /// <summary>
    /// 再生
    /// </summary>
    /// <param name="soundIndex">登録されたサウンドリストのインデックス</param>
    /// <param name="channelIndex">鳴らすチャネルの番号</param>
    /// <param name="isLoop">ループ有無</param>
    public void Play(int soundIndex,int channelIndex = 0, bool isLoop = false)
    {
        //インデックスの範囲外参照をしていた場合値を丸める処理
        channelIndex = CheckChannelOutOfRange(channelIndex);

        //ハッシュのチェック
        if (!CheckSoundsKey(soundIndex))
        {
            //インデックスが見つからなかった場合は関数を実行しない
            Debug.LogError(
                "not found index!" + "\nSo not run this function"
                );
            return;
        }

        //再生処理
        channel[channelIndex].clip = sounds.List[soundIndex].value;
        channel[channelIndex].Play();
        channel[channelIndex].loop = isLoop;
    }

    /// <summary>
    /// BGM用の再生(ループ)
    /// </summary>
    /// <param name="key">登録されたサウンドリストのハッシュキー</param>
    /// <param name="channelIndex">鳴らすチャネルの番号</param>
    public void PlayOnBGM(string key, int channelIndex = 0)
    {
        Play(key, channelIndex, true);
    }

    /// <summary>
    /// BGM用の再生(ループ) 
    /// </summary>
    /// <param name="soundIndex">登録されたサウンドリストのインデックス</param>
    /// <param name="channelIndex">鳴らすチャネルの番号</param>
    public void PlayOnBGM(int soundIndex, int channelIndex = 0)
    {
        Play(soundIndex, channelIndex, true);
    }

    /// <summary>
    /// SE用の再生(一度)
    /// </summary>
    /// <param name="key">登録されたサウンドリストのハッシュキー</param>
    /// <param name="volume">音量(0.0～1.0)</param>
    public void PlayOnSE(string key, float volume = 1.0f)
    {
        //ハッシュのチェック
        if (!CheckSoundsKey(key))
        {
            //ハッシュキーが見つからなかった場合は関数を実行しない
            Debug.LogError(
                "not found hash key!" + "\nSo not run this function"
                );
            return;
        }

        //再生処理
        AudioClip clip = sounds.Table[key];
        channel[0].PlayOneShot(clip, volume);
    }

    /// <summary>
    /// SE用の再生(一度) 
    /// </summary>
    /// <param name="soundIndex">登録されたサウンドリストのインデックス</param>
    /// <param name="volume">音量(0.0～1.0)</param>
    public void PlayOnSE(int soundIndex, float volume = 1.0f)
    {
        //ハッシュのチェック
        if (!CheckSoundsKey(soundIndex))
        {
            //インデックスが見つからなかった場合は関数を実行しない
            Debug.LogError(
                "not found index!" + "\nSo not run this function"
                );
            return;
        }

        //再生処理
        AudioClip clip = sounds.List[soundIndex].value;
        channel[0].PlayOneShot(clip, volume);
    }

    /// <summary>
    /// 一時停止
    /// </summary>
    /// <param name="channelIndex">一時停止するチャネルの番号</param>
    public void Pause(int channelIndex)
    {
        //インデックスの範囲外参照をしていた場合値を丸める処理
        channelIndex = CheckChannelOutOfRange(channelIndex);

        //一時停止
        if (channel[channelIndex].isPlaying) { channel[channelIndex].Pause(); }
    }

    /// <summary>
    /// 一時停止の解除
    /// </summary>
    /// <param name="channelIndex">一時停止を解除するチャネルの番号</param>
    public void UnPause(int channelIndex)
    {
        //インデックスの範囲外参照をしていた場合値を丸める処理
        channelIndex = CheckChannelOutOfRange(channelIndex);

        //鳴っていたら処理しない
        if (channel[channelIndex].isPlaying) { return; }

        //一時停止の解除
        if (channel[channelIndex].time != 0.0f) { channel[channelIndex].UnPause(); }
    }

    /// <summary>
    /// 停止
    /// </summary>
    /// <param name="channelIndex">停止するチャネルの番号</param>
    public void Stop(int channelIndex)
    {
        //インデックスの範囲外参照をしていた場合値を丸める処理
        channelIndex = CheckChannelOutOfRange(channelIndex);

        //停止
        if (channel[channelIndex].isPlaying) { channel[channelIndex].Stop(); }

    }

    /// <summary>
    /// 登録したチャネル(AudioSource)の取得
    /// </summary>
    /// <param name="channelIndex">チャネルの番号</param>
    /// <returns></returns>
    public AudioSource GetChannel(int channelIndex)
    {
        //インデックスの範囲外参照をしていた場合値を丸める処理
        channelIndex = CheckChannelOutOfRange(channelIndex);
        return channel[channelIndex];
    }

    /// <summary>
    /// フェードインするコルーチン(小さくなる方)
    /// </summary>
    /// <param name="frame">フェードさせるフレーム</param>
    /// <param name="volume">フェード先のボリューム(0.0～1.0)</param>
    /// <param name="fadeInFinished">フェードイン終了後に行う関数</param>
    /// <returns></returns>
    public IEnumerator FadeInCoroutine(float frame, float volume = 0.0f, int channelIndex = 0, FadeInFinishedFunc func = null)
    {
        //フェード状態更新
        state = FadeState.FadeIn;

        //1フレーム当たりの増減させる量
        float value = (1 - volume) / frame;

        for (int i = 0; i < frame; ++i)
        {
            GetChannel(channelIndex).volume -= value;
            yield return null;
        }

        //補正
        GetChannel(channelIndex).volume = volume;

        //コールバック
        if (func != null) { func(); }

        //フェード状態更新
        state = FadeState.None;
        yield break;
    }

    /// <summary>
    /// フェードアウトするコルーチン(大きくなる方)
    /// </summary>
    /// <param name="frame">フェードさせるフレーム</param>
    /// <param name="volume">フェード先のボリューム(0.0～1.0)</param>
    /// <param name="fadeInFinished">フェードアウト終了後に行う関数</param>
    /// <returns></returns>
    public IEnumerator FadeOutCoroutine(float frame, float volume = 1.0f, int channelIndex = 0, FadeOutFinishedFunc func = null)
    {
        //フェード状態更新
        state = FadeState.FadeOut;

        //1フレーム当たりの増減させる量
        float value = volume / frame;

        for (int i = 0; i < frame; i++)
        {
            GetChannel(channelIndex).volume += value;
            yield return null;
        }

        //補正
        GetChannel(channelIndex).volume = volume;

        //コールバック
        if (func != null) { func(); }

        //フェード状態更新
        state = FadeState.None;
        yield break;
    }

    /// <summary>
    /// フェードイン(小さくなる)の開始命令
    /// </summary>
    /// <param name="frame">フェードさせるフレーム</param>
    /// <param name="volume">フェード先のボリューム(0.0～1.0)</param>
    /// <param name="channelIndex">フェードさせるチャネルの番号</param>
    /// <param name="func">フェードイン終了後に行う関数</param>
    public void StartSoundFadeIn(float frame, float volume = 0.0f, int channelIndex = 0, FadeInFinishedFunc func = null)
    {
        var coroutine = FadeInCoroutine(frame, volume, channelIndex, func);
        StartCoroutine(coroutine);
    }

    /// <summary>
    /// フェードアウト(大きくなる)の開始命令
    /// </summary>
    /// <param name="frame">フェードさせるフレーム</param>
    /// <param name="volume">フェード先のボリューム(0.0～1.0)</param>
    /// <param name="channelIndex">フェードさせるチャネルの番号</param>
    /// <param name="func">フェードアウト終了後に行う関数</param>
    public void StartSoundFadeOut(float frame, float volume = 1.0f, int channelIndex = 0, FadeOutFinishedFunc func = null)
    {
        var coroutine = FadeOutCoroutine(frame, volume, channelIndex, func);
        StartCoroutine(coroutine);
    }

    /// <summary>
    /// 全チャネルをフェードインさせるコルーチン(小さくする)
    /// </summary>
    /// <param name="frame">フェードさせるフレーム</param>
    /// <param name="volume">フェード先のボリューム(0.0～1.0)</param>
    /// <param name="func">フェードイン終了後に行う関数</param>
    /// <returns></returns>
    public IEnumerator SoundFadeInAllCoroutine(float frame, float volume = 0.0f, FadeInFinishedFunc func = null)
    {
        //状態更新
        state = FadeState.FadeIn;

        //1フレーム当たりの増減させる量
        float value = (1 - volume) / frame;

        //フェードフレーム分繰り返す
        for (int f = 0; f < frame; ++f)
        {
            //毎フレームチャネル数分繰り返す
            for (int c = 0; c < channel.Length; ++c)
            {
                GetChannel(c).volume -= value;
            }
            yield return null;
        }

        //補正
        for (int c = 0; c < channel.Length; ++c)
        {
            GetChannel(c).volume = volume;
        }

        //コールバック
        if (func != null) { func(); }

        //状態更新
        state = FadeState.None;
        yield break;
    }

    /// <summary>
    /// 全チャネルをフェードアウトさせるコルーチン(大きくする)
    /// </summary>
    /// <param name="frame">フェードさせるフレーム</param>
    /// <param name="volume">フェード先のボリューム(0.0～1.0)</param>
    /// <param name="func">フェードアウト終了後に行う関数</param>
    /// <returns></returns>
    public IEnumerator SoundFadeOutAllCoroutine(float frame, float volume = 1.0f, FadeOutFinishedFunc func = null)
    {
        //状態更新
        state = FadeState.FadeOut;

        //1フレーム当たりの増減させる量
        float value = volume / frame;

        //フェードフレーム分繰り返す
        for (int f = 0; f < frame; ++f)
        {
            //毎フレームチャネル数分繰り返す
            for (int c = 0; c < channel.Length; ++c)
            {
                GetChannel(c).volume += value;
            }
            yield return null;
        }

        //補正
        for(int c = 0; c < channel.Length; ++c)
        {
            GetChannel(c).volume = volume;
        }

        //コールバック
        if (func != null) { func(); }

        //状態更新
        state = FadeState.None;
        yield break;
    }

    /// <summary>
    /// 全チャネルのフェードイン開始命令
    /// </summary>
    /// <param name="frame">フェードさせるフレーム</param>
    /// <param name="volume">フェード先のボリューム</param>
    /// <param name="func">フェードイン終了後に行う関数</param>
    public void StartSoundFadeInAll(float frame, float volume = 0.0f, FadeInFinishedFunc func = null)
    {
        var coroutine = SoundFadeInAllCoroutine(frame, volume, func);
        StartCoroutine(coroutine);
    }

    /// <summary>
    /// 全チャネルのフェードアウト開始命令
    /// </summary>
    /// <param name="frame">フェードさせるフレーム</param>
    /// <param name="volume">フェード先のボリューム</param>
    /// <param name="func">フェードアウト終了後に行う関数</param>
    public void StartSoundFadeOutAll(float frame, float volume = 1.0f, FadeOutFinishedFunc func = null)
    {
        var coroutine = SoundFadeOutAllCoroutine(frame, volume, func);
        StartCoroutine(coroutine);
    }
}
