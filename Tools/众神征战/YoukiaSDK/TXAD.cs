using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YoukiaSDKSpace;
using WeChatWASM;

namespace YoukiaSDKSpace
{
    static public class TXAD
    {

        static long next = 0;
        static System.Random rand = new System.Random();

        public static string WX_OPENID = "";
        public static string CLICK_ID = "";


        private static string act_url = YoukiaSDKConfig.TXAD.URL +
            "user_actions/add?access_token=" + UrlEncodeUtil.UrlEncode(YoukiaSDKConfig.TXAD.TOKEN) +
            "&timestamp=<timestamp>&nonce=<nonce>";



        public class ActonType
        {
            public const string CUSTOM = "CUSTOM";
            public const string REGISTER = "REGISTER";
            public const string VIEW_CONTENT = "VIEW_CONTENT";
            public const string CONSULT = "CONSULT";
            public const string ADD_TO_CART = "ADD_TO_CART";
            public const string PURCHASE = "PURCHASE";
            public const string ACTIVATE_APP = "ACTIVATE_APP";
            public const string SEARCH = "SEARCH"; public const string ADD_TO_WISHLIST = "ADD_TO_WISHLIST"; public const string INITIATE_CHECKOUT = "INITIATE_CHECKOUT"; public const string COMPLETE_ORDER = "COMPLETE_ORDER"; public const string DOWNLOAD_APP = "DOWNLOAD_APP"; public const string START_APP = "START_APP"; public const string RATE = "RATE"; public const string PAGE_VIEW = "PAGE_VIEW"; public const string RESERVATION = "RESERVATION"; public const string SHARE = "SHARE"; public const string APPLY = "APPLY"; public const string CLAIM_OFFER = "CLAIM_OFFER"; public const string NAVIGATE = "NAVIGATE"; public const string PRODUCT_RECOMMEND = "PRODUCT_RECOMMEND"; public const string VISIT_STORE = "VISIT_STORE"; public const string TRY_OUT = "TRY_OUT"; public const string DELIVER = "DELIVER"; public const string CONFIRM_EFFECTIVE_LEADS = "CONFIRM_EFFECTIVE_LEADS"; public const string CONFIRM_POTENTIAL_CUSTOMER = "CONFIRM_POTENTIAL_CUSTOMER"; public const string CREATE_ROLE = "CREATE_ROLE"; public const string AUTHORIZE = "AUTHORIZE"; public const string TUTORIAL_FINISH = "TUTORIAL_FINISH"; public const string SCANCODE = "SCANCODE"; public const string ENTER_BACKGROUND = "ENTER_BACKGROUND"; public const string ENTER_FOREGROUND = "ENTER_FOREGROUND"; public const string TICKET = "TICKET"; public const string LOGIN = "LOGIN"; public const string QUEST = "QUEST"; public const string UPDATE_LEVEL = "UPDATE_LEVEL"; public const string CREATE = "CREATE"; public const string PAUSE = "PAUSE"; public const string RESUME = "RESUME"; public const string APP_QUIT = "APP_QUIT"; public const string BIND_ACCOUNT = "BIND_ACCOUNT"; public const string ADD_PAYMENT = "ADD_PAYMENT"; public const string PRE_CREDIT = "PRE_CREDIT"; public const string CREDIT = "CREDIT"; public const string WITHDRAW_DEPOSITS = "WITHDRAW_DEPOSITS"; public const string LANDING_PAGE_CLICK = "LANDING_PAGE_CLICK"; public const string SELECT_COURSE = "SELECT_COURSE"; public const string RE_FUND = "RE_FUND"; public const string PLATFORM_VIEW = "PLATFORM_VIEW"; public const string ONE_DAY_LEAVE = "ONE_DAY_LEAVE"; public const string PRODUCT_VIEW = "PRODUCT_VIEW"; public const string PURCHASE_MEMBER_CARD = "PURCHASE_MEMBER_CARD"; public const string ONLINE_CONSULT = "ONLINE_CONSULT"; public const string MAKE_PHONE_CALL = "MAKE_PHONE_CALL"; public const string FOLLOW = "FOLLOW"; public const string ADD_DESKTOP = "ADD_DESKTOP"; public const string RETURN = "RETURN"; public const string LEAVE_INFORMATION = "LEAVE_INFORMATION"; public const string PURCHASE_COUPON = "PURCHASE_COUPON"; public const string ADD_GROUP = "ADD_GROUP"; public const string ADD_CUSTOMER_PAGE_VIEW = "ADD_CUSTOMER_PAGE_VIEW"; public const string ADD_CUSTOMER_PAGE_INTERACTIVE = "ADD_CUSTOMER_PAGE_INTERACTIVE"; public const string CUSTOMER_PROMOTION_PAGE_VIEW = "CUSTOMER_PROMOTION_PAGE_VIEW"; public const string CUSTOMER_PROMOTION_PAGE_INTERACTIVE = "CUSTOMER_PROMOTION_PAGE_INTERACTIVE"; public const string ABNORMAL_ACTION = "ABNORMAL_ACTION"; public const string LIVE_STREAM = "LIVE_STREAM"; public const string SCANCODE_WX = "SCANCODE_WX"; public const string STAY_PAY_7 = "STAY_PAY_7"; public const string STAY_PAY_15 = "STAY_PAY_15"; public const string STAY_PAY_30 = "STAY_PAY_30"; public const string INSURANCE_PAY = "INSURANCE_PAY"; public const string RESERVATION_CHECK = "RESERVATION_CHECK"; public const string PARTICIPATED = "PARTICIPATED"; public const string COMPLETED = "COMPLETED"; public const string REGULAR_PRICE_COURSE = "REGULAR_PRICE_COURSE"; public const string DROP_OUT = "DROP_OUT"; public const string CONFIRM_DELIVERY_ORDER = "CONFIRM_DELIVERY_ORDER"; public const string CANCEL_DELIVERY_ORDER = "CANCEL_DELIVERY_ORDER"; public const string OPEN_ACCOUNT = "OPEN_ACCOUNT"; public const string DEPOSIT = "DEPOSIT"; public const string TRADE = "TRADE"; public const string SECURITY_NEGATIVE = "SECURITY_NEGATIVE"; public const string AD_CLICK = "AD_CLICK"; public const string AD_IMPRESSION = "AD_IMPRESSION"; public const string SIGN_IN = "SIGN_IN"; public const string TRY_OUT_INTENTION = "TRY_OUT_INTENTION"; public const string INEFFECTIVE_LEADS = "INEFFECTIVE_LEADS"; public const string READ_ARTICLE = "READ_ARTICLE"; public const string COMMENT = "COMMENT"; public const string CARD_CLICK = "CARD_CLICK"; public const string WECOM_CONSULT = "WECOM_CONSULT"; public const string BIND_CARD = "BIND_CARD"; public const string LOW_PRICE_COURSE = "LOW_PRICE_COURSE"; public const string ADD_WECHAT = "ADD_WECHAT"; public const string PRE_PAY = "PRE_PAY"; public const string QUIT_GROUP = "QUIT_GROUP"; public const string PHONE_CONNECTED = "PHONE_CONNECTED"; public const string RE_ACTIVE = "RE_ACTIVE"; public const string CLAIM_COURSE = "CLAIM_COURSE"; public const string VIEW_ACQUISITION_CONTENT = "VIEW_ACQUISITION_CONTENT"; public const string TERMINATION = "TERMINATION"; public const string RENEWAL = "RENEWAL";
            public const string CONSULT_INTENTION = "CONSULT_INTENTION";
        }

        private static string GetTimeStamp()
        {
            return ((System.DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000).ToString();
        }

        public static void DoAction(string type, Dictionary<string, object> act = null)
        {


            if (!YoukiaSDKConfig.TXAD.ENABLED || String.IsNullOrEmpty(CLICK_ID) || String.IsNullOrEmpty(WX_OPENID))
            {
                return;
            }
            var post = new Dictionary<string, object>();

            // TX文件有错 非子账号不能传入account_id
            //post.Add("account_id", YoukiaSDKConfig.TXAD_ACCOUNT);


            post.Add("user_action_set_id", YoukiaSDKConfig.TXAD.ACTION_SET_ID);


            var user = new Dictionary<string, object>();
            user.Add("wechat_app_id", YoukiaSDKConfig.WX_APP_ID);
            user.Add("wechat_openid", WX_OPENID);
            post.Add("user_id", user);
            Dictionary<string, object> action = act;
            if (act == null)
            {
                action = new Dictionary<string, object>();
            }
            action.Add("action_type", type);
            var trace = new Dictionary<string, string>();
            trace.Add("click_id", CLICK_ID);
            action.Add("trace", trace);
            var acts = new Dictionary<string, object>[1];
            acts[0] = action;
            post.Add("actions", acts);

            next++;
            var url = act_url.Replace("<timestamp>", GetTimeStamp())
            .Replace("<nonce>", MD5Utils.MD5String(Guid.NewGuid().ToString() + next.ToString()));

            var data = Json.Serialize(post);
            Debug.Log("TXAD\t" + next + "\t" + url);
            Debug.Log("TXAD\t" + next + "\n" + data);

            FastNetwork.Instance.DoPostJson(url, data, (res) =>
            {
                Debug.Log("TXAD\t" + next + "\t" + res);
                try
                {
                    var r = Json.Deserialize(res) as Dictionary<string, object>;
                    if (r.ContainsKey("code") && r["code"].ToString().Equals("0"))
                    {
                        Debug.Log("TXAD\tupdate SUCCESS");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("TXAD\t update faild " + next + "\t" + res);
                }

            }, (err) =>
            {
                Debug.LogError("TXAD\t" + next + "\t" + err);
            });
        }

    }

}
