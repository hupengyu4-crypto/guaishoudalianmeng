using UnityEngine;

namespace YoukiaSDKSpace
{
    [FastResPathAttr("YoukiaSDK/Prefabs/FastLoadingView")]
    [FastResTypeAttr(FastResType.Prefab)]
    public class FastLoading : FastSingleUI<FastLoading>
    {

        private FastLoading()
        {

        }

        public void show()
        {
            this.gameObject.SetActive(true);
        }

        public void hide()
        {
            gameObject.SetActive(false);
            DestroySelf();
        }

        void Update()
        {
            if (this.gameObject.activeSelf)
            {
                this.gameObject.transform.Rotate(-Vector3.forward * 200 * Time.deltaTime);
            }
        }
    }
}

