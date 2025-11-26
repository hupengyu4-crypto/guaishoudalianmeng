using System;
using UnityEngine;
using UnityEngine.UI;

namespace YoukiaSDKSpace
{
    /// <summary>
    /// 该类用于创建预制体和gameobject
    /// </summary>
    public class FastUI
    {
        private static FastUI instance;
        private GameObject rootCanvas;
        private const string ROOT_CANVAS_NAME = "FastUICanvas";
        public static FastUI GetInstance()
        {
            if (instance == null)
            {
                instance = new FastUI();
                instance.CreateRootCanvas();
            }
            return instance;
        }

        public T Init<T>() where T : MonoBehaviour
        {
            string resPath = GetResPath(typeof(T));
            FastResType resType = GetResType(typeof(T));
            switch (resType)
            {
                case FastResType.GameObject:
                    return InitGameObject<T>(resPath);
                default:
                case FastResType.Prefab:
                    return InitPrefab<T>(resPath);
            }
        }
        //实例化预制体
        private T InitPrefab<T>(string path, bool dontDestroy = false) where T : MonoBehaviour
        {
            GameObject resObj = Resources.Load<GameObject>(path);
            GameObject gameObj = UnityEngine.Object.Instantiate(resObj);
            if (dontDestroy)
            {
                UnityEngine.Object.DontDestroyOnLoad(gameObj);
            }
            resObj = null;
            Resources.UnloadUnusedAssets();
            T t = gameObj.AddComponent<T>();
            gameObj.transform.SetParent(rootCanvas.transform, true);
            gameObj.transform.localPosition = Vector3.zero;
            gameObj.transform.localScale = Vector3.one;
            return t;
        }
        //实例化gameobject
        private T InitGameObject<T>(string path, bool dontDestroy = false) where T : MonoBehaviour
        {
            GameObject gameObj = new GameObject(path);
            if (dontDestroy)
            {
                UnityEngine.Object.DontDestroyOnLoad(gameObj);
            }
            T t = gameObj.AddComponent<T>();
            gameObj.transform.SetParent(rootCanvas.transform, true);
            gameObj.transform.localPosition = Vector3.zero;
            gameObj.transform.localScale = Vector3.one;
            return t;
        }
        //通过注解获取资源的路径
        public string GetResPath(Type type)
        {
            string path = "";
            object[] attrs = type.GetCustomAttributes(true);
            foreach (object attr in attrs)
            {
                if (attr is FastResPathAttr)
                {
                    path = (attr as FastResPathAttr).Path;
                    return path;
                }
            }
            Debug.Log($"{type} this Class is not config res path");
            return path;
        }
        //通过注解获取资源的种类
        public FastResType GetResType(Type type)
        {
            FastResType resType = FastResType.Prefab;
            object[] attrs = type.GetCustomAttributes(true);
            foreach (object attr in attrs)
            {
                if (attr is FastResTypeAttr)
                {
                    resType = (attr as FastResTypeAttr).Type;
                    return resType;
                }
            }
            Debug.Log($"{type} this Class is not config res type");
            return resType;
        }


        //获取根Canvas
        public GameObject GetRootCanvas()
        {
            return rootCanvas;
        }

        //创建根Canvas，用来放置预制体
        private GameObject CreateRootCanvas()
        {
            rootCanvas = new GameObject(ROOT_CANVAS_NAME);
            GameObject.DontDestroyOnLoad(rootCanvas);
            rootCanvas.transform.localPosition = Vector3.zero;
            rootCanvas.transform.localScale = Vector3.one;
            Canvas canvas = rootCanvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 20;
            CanvasScaler canvasScaler = rootCanvas.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1280, 720);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            GraphicRaycaster graphicRaycaster = rootCanvas.AddComponent<GraphicRaycaster>();
            graphicRaycaster.ignoreReversedGraphics = true;
            return rootCanvas;
        }
    }
}

