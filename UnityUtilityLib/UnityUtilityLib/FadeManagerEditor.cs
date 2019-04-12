using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Yuuki.Fade;

namespace Yuuki.Editor
{
    [CustomEditor(typeof(FadeManager))]
    public class FadeManagerEditor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            FadeManager fade = target as FadeManager;

            if (GUILayout.Button("Create Need Components!"))
            {
                if (!fade.gameObject.GetComponent<Canvas>())
                {
                    Debug.Log("Canvasがありません。生成します。");
                    fade.gameObject.AddComponent<Canvas>();
                }

                if (!fade.gameObject.GetComponent<CanvasGroup>())
                {
                    Debug.Log("CanvasGroupがありません。生成します。");
                    fade.gameObject.AddComponent<CanvasGroup>();
                }

                if (!fade.gameObject.GetComponent<CanvasScaler>())
                {
                    Debug.Log("CanvasScalerがありません。生成します。");
                    fade.gameObject.AddComponent<CanvasScaler>();
                }

                if (!fade.gameObject.GetComponent<GraphicRaycaster>())
                {
                    Debug.Log("GraphicRaycasterがありません。生成します。");
                    fade.gameObject.AddComponent<GraphicRaycaster>();
                }

                fade.Initialize();
                Debug.Log("コンポーネントの初期化を行いました。");
            }

        }

    }
}