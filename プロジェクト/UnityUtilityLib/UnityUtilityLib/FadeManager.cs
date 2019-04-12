/// <summary>
/// 番場宥輝
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Yuuki.Fade
{
    /// <summary>
    /// フェード管理クラス
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(CanvasScaler))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public class FadeManager : SingletonMonoBehaviour<FadeManager>
    {

        //Hide variable
        private CanvasGroup canvasGroupEntity;

        //accessor
        public CanvasGroup canvasGroup
        {
            get
            {
                //null check!
                if (canvasGroupEntity == null)
                {
                    //GetComponent
                    canvasGroupEntity = GetComponent<CanvasGroup>();
                    //null check!
                    if (canvasGroupEntity == null)
                    {
                        //AddComponent
                        canvasGroupEntity = gameObject.AddComponent<CanvasGroup>();
                    }
                }
                return canvasGroupEntity;
            }
        }

        public float Alpha
        {
            get
            {
                return canvasGroup.alpha;
            }
            set
            {
                canvasGroup.alpha = value;
            }
        }

        public bool IsFade
        {
            get
            {
                return state != FadeState.None;
            }
        }

        //enum
        private enum FadeState { None, FadeIn, FadeOut }
        private FadeState state;

        //CallBack
        public delegate void FadeInFinishedFunc();
        public delegate void FadeOutFinishedFunc();

        /// <summary>
        /// 初期化
        /// </summary>
        [ContextMenu("Initialize!")]
        public void Initialize()
        {
            //get reference!
            Canvas canvas = GetComponent<Canvas>();
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            CanvasScaler canvasScaler = GetComponent<CanvasScaler>();

            //create child!
            Image child = GetComponentInChildren<Image>() ?? null;
            if (!child)
            {
                CreateChildImage();
            }

            //  param initialize! 
            state = FadeState.None;
            Alpha = 0;
            canvasGroup.blocksRaycasts = false;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            Image image = GetComponentInChildren<Image>();
            image.color = Color.black;
            image.GetComponent<RectTransform>().sizeDelta = new Vector2(2000, 2000);
        }

        /// <summary>
        /// フェードインするコルーチン(暗くなる方)
        /// </summary>
        /// <param name="frame">フェードさせるフレーム</param>
        /// <param name="fadeInFinished">フェードイン終了後に行う関数</param>
        /// <returns></returns>
        public IEnumerator FadeInCoroutine(float frame, FadeInFinishedFunc fadeInFinished = null)
        {
            state = FadeState.FadeIn;
            float speed = 1 / frame;

            for (int i = 0; i < frame; i++)
            {
                Alpha += speed;
                yield return null;
            }

            if (fadeInFinished != null)
            {
                fadeInFinished();
            }
            state = FadeState.None;
            yield break;
        }

        /// <summary>
        /// フェードアウトするコルーチン(明るくなる方)
        /// </summary>
        /// <param name="frame">フェードさせるフレーム</param>
        /// <param name="fadeInFinished">フェードアウト終了後に行う関数</param>
        /// <returns></returns>
        public IEnumerator FadeOutCoroutine(float frame, FadeOutFinishedFunc fadeOutFinished = null)
        {
            state = FadeState.FadeOut;
            float speed = 1 / frame;

            for (int i = 0; i < frame; i++)
            {
                Alpha -= speed;
                yield return null;
            }

            if (fadeOutFinished != null)
            {
                fadeOutFinished();
            }
            state = FadeState.None;
            yield break;
        }

        /// <summary>
        /// 子オブジェクトにイメージ生成
        /// </summary>
        private void CreateChildImage()
        {
            GameObject image = new GameObject("FadeImage");
            image.transform.SetParent(Instance.transform, false);
            image.AddComponent<Image>();
        }
    }
}
