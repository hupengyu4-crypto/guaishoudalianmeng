using UnityEngine.UI;

namespace YoukiaSDKSpace
{
    [FastResPathAttr("YoukiaSDK/Prefabs/FastDialogView")]
    [FastResTypeAttr(FastResType.Prefab)]
    public class FastDialog : FastSingleUI<FastDialog>
    {

        //标题
        public Text titleText;
        //内容
        public Text messageText;
        //左按钮
        public Button leftBtn;
        //右按钮
        public Button rightBtn;

        private OnClickLeftBtn onClickLeft;
        private OnClickRightBtn onClickRight;

        public delegate void OnClickLeftBtn();
        public delegate void OnClickRightBtn();

        private FastDialog()
        {

        }

        private void Awake()
        {
            titleText = transform.Find("DialogPanel/Title").GetComponent<Text>();
            messageText = transform.Find("DialogPanel/Message").GetComponent<Text>();
            leftBtn = transform.Find("DialogPanel/LeftBtn").GetComponent<Button>();
            rightBtn = transform.Find("DialogPanel/RightBtn").GetComponent<Button>();
            this.gameObject.SetActive(false);
        }

        private void Hide()
        {
            this.gameObject.SetActive(false);
        }

        public void Show()
        {
            this.gameObject.SetActive(true);
        }

        public void SetAll(string title, string msg, string leftBtnText, string rightBtnText)
        {
  
            SetTitle(title);
            SetMessage(msg);
            SetLeftBtnText(leftBtnText);
            SetRightBtnText(rightBtnText);
        }

        public void SetCallback(OnClickLeftBtn onClickleft, OnClickRightBtn onClickRight)
        {
            this.onClickLeft = onClickleft;
            this.onClickRight = onClickRight;
        }

        public void SetTitle(string title)
        {
            this.titleText.text = title;
        }

        public void SetMessage(string msg)
        {
            this.messageText.text = msg;
        }

        public void SetLeftBtnText(string text)
        {

            Text tv = this.leftBtn.transform.GetChild(0).GetComponent<Text>();
            tv.text = text;
        }

        public void SetRightBtnText(string text)
        {
            Text tv = this.rightBtn.transform.GetChild(0).GetComponent<Text>();
            tv.text = text;
        }

        void Start()
        {
            leftBtn.onClick.AddListener(OnClickLeft);
            rightBtn.onClick.AddListener(OnClickRight);
        }

        void OnClickLeft()
        {
            gameObject.SetActive(false);
            DestroySelf();
            if (onClickLeft != null)
            {
                onClickLeft();
            }
        }

        void OnClickRight()
        {
            gameObject.SetActive(false);
            DestroySelf();
            if (onClickRight != null)
            {
                onClickRight();
            }
        }
    }
}
