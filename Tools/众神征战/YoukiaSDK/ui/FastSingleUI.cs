using UnityEngine;

namespace YoukiaSDKSpace
{
    public abstract class FastSingleUI<T> : MonoBehaviour where T : FastSingleUI<T>
    {
        private static T m_instance;

        public static T Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = FastUI.GetInstance().Init<T>();
                }
                //Debug.Log($"{typeof(T)} instance {m_instance.GetHashCode()}");
                return m_instance;
            }
        }

        /*
         * 没有任何实现的函数，用于保证MonoSingleton在使用前已创建
         */
        public virtual void Startup()
        {

        }

        protected virtual void Awake()
        {
            if (m_instance == null)
            {
                m_instance = this as T;
                DontDestroyOnLoad(gameObject);
                InitData();
            }
            else if (m_instance != this)
            {
                DestroyImmediate(gameObject);
            }
        }

        protected virtual void InitData()
        {

        }

        public void DestroySelf()
        {
            m_instance = null;
            DestroyImmediate(gameObject);
        }

    }

}
