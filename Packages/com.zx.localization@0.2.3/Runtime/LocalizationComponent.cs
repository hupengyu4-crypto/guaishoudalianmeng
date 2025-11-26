using UnityEngine;
using UObject = UnityEngine.Object;

namespace Localization
{
    [ExecuteInEditMode]
    public abstract class LocalizationComponent : MonoBehaviour
    {
        #region Properties
        private static bool isRegisted;

        public virtual UObject Target { get; }
        #endregion

        #region Public Methods
        /// <summary>
        /// 刷新当前显示
        /// </summary>
        public abstract void Refresh();

        /// <summary>
        /// 清理显示
        /// </summary>
        /// <returns>清理成功返回true</returns>
        public abstract bool ClearDisplay();
        #endregion

        #region Internal Methods
        protected virtual void Awake()
        {
            if (!isRegisted)
            {
                isRegisted = true;
                LocalizationUtility.LoadReplaceRegist();
            }

            Refresh();
        }
        #endregion
    }
}