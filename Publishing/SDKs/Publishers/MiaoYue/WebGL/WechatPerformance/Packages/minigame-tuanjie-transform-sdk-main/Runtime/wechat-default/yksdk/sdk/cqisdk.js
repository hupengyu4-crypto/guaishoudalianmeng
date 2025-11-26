(function(l, g) {
  typeof exports == "object" && typeof module < "u" ? g(exports) : typeof define == "function" && define.amd ? define(["exports"], g) : (l = typeof globalThis < "u" ? globalThis : l || self, g(l.CQI = {}))
})(this, function(l) {
  "use strict";
  var se = Object.defineProperty;
  var ie = (l, g, w) => g in l ? se(l, g, {
      enumerable: !0,
      configurable: !0,
      writable: !0,
      value: w
  }) : l[g] = w;
  var h = (l, g, w) => (ie(l, typeof g != "symbol" ? g + "" : g, w), w);
  typeof navigator < "u" && navigator.product === void 0 && (navigator.product = "NativeScript");
  class g {
      setStorage(t) {
          return wx.setStorage(t)
      }
      getStorage(t) {
          return new Promise(e => {
              let s = {};
              s.data = wx.getStorageSync(t.key), e(s)
          })
      }
  }

  function w(r) {
      const {
          url: t,
          params: e,
          baseURL: s
      } = r;
      let i = "";
      e && (Object.keys(e).forEach(n => {
          let c = encodeURIComponent(n),
              d = encodeURIComponent(e[n]);
          i += `${c}=${d}&`
      }), i.endsWith("&") && (i = i.slice(0, -1)));
      let o = t || "";
      return !o.startsWith("http://") && !o.startsWith("https://") && (o = `${s}${t}`), i ? `${o}?${i}` : o
  }

  function L() {
      const r = {
          get: (t, e) => (e = {
              ...e,
              url: t,
              method: "get"
          }, r.request(e)),
          post: (t, e, s) => {
              const i = (s == null ? void 0 : s.headers) || {};
              return !i["Content-Type"] && (i["Content-Type"] = "application/x-www-form-urlencoded"), s = {
                  ...s,
                  url: t,
                  data: e,
                  method: "post",
                  headers: i
              }, r.request(s)
          },
          request: t => new Promise((e, s) => {
              wx.request({
                  method: t.method.toUpperCase(),
                  url: w(t),
                  header: t.headers,
                  data: t.data,
                  dataType: "json",
                  responseType: t.responseType,
                  success: i => {
                      console.log("request success ↓↓"), console.log(i), e(i.data)
                  },
                  fail: i => {
                      s(i)
                  }
              })
          })
      };
      return r
  }
  class G {
      md5Encrypt(t) {
          return t
      }
      rsaEncrypt(t) {
          return t
      }
      uniqid() {
          let t = Date.now(),
              e = Math.floor(Math.random() * 1e11);
          return (t + e).toString(32)
      }
  }
  class M extends G {
      async getSystemInfo() {
          const t = await wx.getSystemInfo();
          switch (t.platform) {
              case "android":
                  t._os = 0;
                  break;
              case "ios":
                  t._os = 1;
                  break;
              case "windows":
              case "mac":
                  t._os = 3;
                  break;
              default:
                  t._os = 2;
                  break
          }
          return t
      }
      getLaunchOption() {
          return wx.getLaunchOptionsSync()
      }
  }
  const b = ({
          method: r,
          url: t,
          query: e = {},
          data: s = {},
          sdk: i,
          dm: o
      }) => {
          const n = {},
              c = i.getUserInfo();
          n.Token = c.token;
          const d = i.getPkgConfig();
          let a;
          switch (o) {
              case "sdk":
                  a = (d == null ? void 0 : d.domain.sdk) || "https://myi-sdk.gzmywl.cn";
                  break;
              case "pay":
                  a = (d == null ? void 0 : d.domain.pay) || "https://myi-pay.gzmywl.cn";
                  break;
              case "log":
                  a = (d == null ? void 0 : d.domain.report) || "https://myi-log.gzmywl.cn";
                  break
          }
          if (t = a + t, r === "GET") {
              const m = Object.assign(i.getGlobalParams(), e);
              return i.getRequest().get(t, {
                  params: m,
                  headers: n
              })
          } else return s = Object.assign(i.getGlobalParams(), s), i.getRequest().post(t, s, {
              headers: n
          })
      },
      x = (r, t, e, s) => b({
          method: "GET",
          url: r,
          query: t,
          sdk: e,
          dm: s
      }),
      S = (r, t, e, s) => b({
          method: "POST",
          url: r,
          data: t,
          sdk: e,
          dm: s
      }),
      R = (r, t, e) => x(r, t, e, "sdk"),
      I = (r, t, e) => S(r, t, e, "sdk"),
      P = (r, t, e) => S(r, t, e, "pay"),
      _ = (r, t, e) => x(r, t, e, "log"),
      f = r => {
          if (r.code != 200) {
              const t = new Error;
              throw t.message = r.msg, t.name = "" + r.code, t
          }
          return r.result
      },
      U = async (r, t) => {
          const e = await I("/users/wxLogin", r, t);
          return f(e)
      }, E = async (r, t) => {
          const e = await I("/users/msgSecCheck", r, t);
          return f(e)
      }, N = async (r, t) => {
          const e = await I("/users/wxDecryptData", r, t);
          return f(e)
      }, T = async (r, t) => {
          const e = await I("/minGame/userRiskRank", r, t);
          return f(e)
      }, O = async (r, t) => {
          const e = await P("/orders/createWxOrder", r, t);
          return f(e)
      }, z = async (r, t) => {
          const e = await P("/orders/wxPay", r, t);
          return f(e)
      }, V = async (r, t) => {
          const e = await I("/packages/detail", r, t);
          return f(e)
      }, K = async (r, t) => {
          const e = await R("/packages/getConf", r, t);
          return f(e)
      }, B = async (r, t) => {
          r.act = "createRole", r.appv = "1.2";
          const e = await _("/comapi", r, t);
          return f(e)
      }, W = async (r, t) => {
          r.act = "enterGame", r.appv = "1.2";
          const e = await _("/comapi", r, t);
          return f(e)
      }, j = async (r, t) => {
          r.act = "roleUp", r.appv = "1.2";
          const e = await _("/comapi", r, t);
          return f(e)
      };

  function u(r) {
      return r.then(t => [null, t]).catch(t => [t, null])
  }
  const C = "1.0.7",
      q = "cqisdk_device",
      $ = "cqisdk_user";
  class F {
      constructor(t) {
          h(this, "storageAdapter");
          h(this, "requestAdapter");
          h(this, "systemAdapter");
          h(this, "csl");
          h(this, "gameId", 0);
          h(this, "pkgbnd", "");
          h(this, "pkgid", 0);
          h(this, "adid", "");
          h(this, "promotionId", "");
          h(this, "query", {
              invite: ""
          });
          h(this, "deviceInfo", {
              _is: !1,
              isActived: !1,
              deviceId: "",
              system: "",
              brand: "",
              model: "",
              os: 2,
              platform: "",
              clue_token: "",
              sdk_version: C
          });
          h(this, "userInfo", {
              uid: "",
              username: "",
              token: "",
              sign: "",
              wechat: "",
              wechat_sk: ""
          });
          h(this, "pkgConfig");
          h(this, "isInitCompleted", !1);
          h(this, "isLogined", !1);
          h(this, "rewardedVideoAd");
          this.gameId = t.gameid, this.pkgid = t.pkgid, this.pkgbnd = t.pkgbnd, this.csl = Object.assign({}, console)
      }
      err(t, e) {
          return console.error(`(${t}) ${e}`), {
              errcode: t,
              errmsg: e
          }
      }
      log(t, e = "", s = "info") {
          t = `[SDK:${s}] ${t}`, e = e ?? "", this.csl.log(t, e)
      }
      logCheck(t, e = "") {
          let s = "";
          [1, 2, 3, 4, 7].includes(t) ? s += "(must) " : [8].includes(t) && (s += "(recommend) "), s += `2.${t} Connected.`, this.log(s, e, "check")
      }
      setStorage(t) {
          this.storageAdapter = t
      }
      setRequest(t) {
          this.requestAdapter = t
      }
      setSystem(t) {
          this.systemAdapter = t
      }
      getRequest() {
          return this.requestAdapter
      }
      roleCreate(t) {
          return this.logCheck(3, t), this.checkLogined(), new Promise(async (e, s) => {
              const i = {
                  uid: this.userInfo.uid,
                  roleid: t.roleId,
                  rolename: t.roleName,
                  sid: t.serverId,
                  servername: t.serverName,
                  gameid: this.gameId,
                  pkgbnd: this.pkgbnd,
                  adid: this.adid,
                  pkgid: this.pkgid,
                  devid: this.deviceInfo.deviceId
              };
              await B(i, this), e({})
          })
      }
      enterGame(t) {
          return this.logCheck(4, t), this.checkLogined(), new Promise(async (e, s) => {
              const i = {
                  uid: this.userInfo.uid,
                  roleid: t.roleId,
                  rolename: t.roleName,
                  sid: t.serverId,
                  servername: t.serverName,
                  gameid: this.gameId,
                  pkgbnd: this.pkgbnd,
                  adid: this.adid,
                  pkgid: this.pkgid,
                  promotion_id: this.promotionId,
                  devid: this.deviceInfo.deviceId
              };
              await W(i, this), e({})
          })
      }
      roleUp(t) {
          return this.logCheck(5, t), this.checkLogined(), new Promise(async (e, s) => {
              const i = {
                  uid: this.userInfo.uid,
                  roleid: t.roleId,
                  rolename: t.roleName,
                  sid: t.serverId,
                  servername: t.serverName,
                  rolelevel: t.roleLevel || 0,
                  gameid: this.gameId,
                  pkgbnd: this.pkgbnd,
                  adid: this.adid,
                  pkgid: this.pkgid,
                  promotion_id: this.promotionId,
                  devid: this.deviceInfo.deviceId
              };
              await j(i, this), e({})
          })
      }
      exitGame(t) {
          return this.logCheck(6, t), this.checkLogined(), new Promise(async (e, s) => {
              e({})
          })
      }
      async initialize() {
          var i;
          this.log(`SDK_VERSION: ${C}`), await this.getDeviceInfo(), await this.readLogined();
          const t = {
                  actived: 1,
                  gameid: this.gameId,
                  pkgbnd: this.pkgbnd,
                  pkgid: this.pkgid
              },
              [e, s] = await u(V(t, this));
          return e ? (this.err(4e3, `Initialize Error: ${e.message}`), !1) : (this.pkgConfig = s, ((i = this.pkgConfig) == null ? void 0 : i.watch_ad.status) === 1 && this._initAd(), this.isInitCompleted = !0, !0)
      }
      checkInit() {
          if (!this.isInitCompleted) throw console.error("SDK 未初始化"), "SDK 未初始化"
      }
      checkLogined() {
          if (!this.isLogined) throw console.error("SDK 未登录"), "SDK 未登录"
      }
      getGlobalParams() {
          return {
              devid: this.deviceInfo.deviceId,
              gameid: this.gameId,
              pkgid: this.pkgid,
              pkgbnd: this.pkgbnd,
              adid: this.adid,
              promotion_id: this.promotionId,
              oaid: "",
              imei: "",
              androidid: "",
              osver: this.deviceInfo.system,
              exmodel: `${this.deviceInfo.brand} ${this.deviceInfo.model}`,
              versioncode: 1,
              os: this.deviceInfo.os,
              lang: "zh",
              sdkver: C
          }
      }
      getPkgConfig() {
          return this.pkgConfig
      }
      async getDeviceInfo() {
          if (this.deviceInfo && this.deviceInfo._is) return this.deviceInfo;
          console.log("getDeviceInfo~");
          const t = await this.storageAdapter.getStorage({
                  key: q
              }),
              e = t && t.data ? t.data : {};
          if (Object.keys(e).length > 0 && e.sdk_version === this.deviceInfo.sdk_version) this.deviceInfo = e;
          else {
              const s = await this.systemAdapter.getSystemInfo();
              console.log("getSystemInfo返回", s), this.deviceInfo.system = s.system || "", this.deviceInfo.brand = s.brand || "", this.deviceInfo.model = s.model || "", this.deviceInfo.os = s._os, this.deviceInfo.platform = s.platform || "", this.deviceInfo.deviceId = s._devid || e.deviceId || "", this.deviceInfo.deviceId || (this.deviceInfo.deviceId = this.systemAdapter.uniqid()), this.deviceInfo.clue_token = e.clue_token || this.query.clue_token || "", this.deviceInfo.isActived = e.isActived || !1
          }
          return this.deviceInfo._is = !0, console.log("deviceInfo", this.deviceInfo), this.deviceInfo
      }
      async setDeviceInfo(t) {
          this.deviceInfo = t, await this.storageAdapter.setStorage({
              key: q,
              data: this.deviceInfo
          })
      }
      async setLogined(t) {
          this.userInfo = t, await this.storageAdapter.setStorage({
              key: $,
              data: this.userInfo
          }), this.isLogined = !0
      }
      async readLogined() {
          if (!this.userInfo.token) {
              const t = await this.storageAdapter.getStorage({
                      key: $
                  }),
                  e = t && t.data ? t.data : {};
              Object.keys(e).length > 0 && (this.userInfo = e)
          }
      }
      getUserInfo() {
          return this.userInfo
      }
      getCloudUserInfo() {}
      showRewardedVideoAd(t) {
          return this.logCheck(12, t), this.checkLogined(), new Promise(async (e, s) => {
              var o;
              if (((o = this.pkgConfig) == null ? void 0 : o.watch_ad.status) !== 1) return s(this.err(4023, "广告未配置"));
              const i = n => {
                  n && n.isEnded || n === void 0 ? e({
                      isEnded: 1
                  }) : e({
                      isEnded: 0
                  }), this.rewardedVideoAd.offClose(i)
              };
              this.rewardedVideoAd.show().then(() => {
                  this.rewardedVideoAd.onClose(i)
              }).catch(n => {
                  this.rewardedVideoAd.load().then(() => {
                      this.rewardedVideoAd.show().then(() => {
                          this.rewardedVideoAd.onClose(i)
                      }).catch(c => {
                          console.log("激励视频 广告错误"), console.log(c), s(this.err(4021, "广告展示失败"))
                      })
                  }).catch(c => {
                      console.log("激励视频 广告错误"), console.log(c), s(this.err(4022, "广告加载失败"))
                  })
              })
          })
      }
      wxGetGameClubData(t) {
          return console.warn("微信专有接口: wxGetGameClubData", t), new Promise(() => {})
      }
      wxRequestSubscribeMessage(t) {
          return console.warn("微信专有接口: wxRequestSubscribeMessage", t), new Promise(() => {})
      }
      getShareConf() {
          return new Promise(async (t, e) => {
              const [s, i] = await u(K({
                  module: "share"
              }, this));
              s && e(this.err(4041, `getShareConf err: ${s.message}`)), t({
                  title: i.title,
                  imgurl: i.img,
                  imgid: i.id
              })
          })
      }
  }
  class Q extends F {
      constructor() {
          super(...arguments);
          h(this, "reloginNum", 0);
          h(this, "gameClubBtn");
          h(this, "wxUserProfile")
      }
      init() {
          return this.logCheck(1), new Promise(async (e, s) => {
              var o, n;
              const {
                  query: i = {}
              } = this.systemAdapter.getLaunchOption();
              this.promotionId = i.promotion_id || "", this.adid = i.adid || "", this.query = i, await this.initialize(), (o = this.pkgConfig) != null && o.wxgame && ((n = this.pkgConfig) == null ? void 0 : n.wxgame.club_btn) === 1 && (this._initGameClubBtn(), this.gameClubBtn.hide()), e({
                  query: this.query
              })
          })
      }
      _initAd() {
          var e;
          this.rewardedVideoAd = wx.createRewardedVideoAd({
              adUnitId: (e = this.pkgConfig) == null ? void 0 : e.watch_ad.placementid
          }), this.rewardedVideoAd.onError(s => {
              console.log("激励视频广告异常"), console.log(s)
          })
      }
      _initGameClubBtn() {
          var e;
          this.gameClubBtn = wx.createGameClubButton({
              type: "image",
              style: {
                  left: 10,
                  top: 80,
                  width: 40,
                  height: 40
              },
              icon: "light",
              openlink: ((e = this.pkgConfig) == null ? void 0 : e.wxgame.club_link) || ""
          })
      }
      wxLogin() {
          return new Promise((e, s) => {
              wx.login({
                  success: i => {
                      i.code ? e(i) : s(this.err(4001, "wx.login Error"))
                  },
                  fail: (i, o) => {
                      s(this.err(4001, `wx.login err: ${i} code: ${o}`))
                  }
              })
          })
      }
      wxCheckSession() {
          return new Promise((e, s) => {
              wx.checkSession({
                  success: () => {
                      e(!0)
                  },
                  fail: () => {
                      this.userInfo.token = "", e(!1)
                  }
              })
          })
      }
      showMessage(e, s = 0) {
          wx.showModal({
              title: "提示",
              content: e,
              showCancel: !1,
              success: () => {
                  s && this.showMessage(e, --s)
              }
          })
      }
      login() {
          return this.logCheck(2), this.checkInit(), new Promise(async (e, s) => {
              await u(this.wxCheckSession());
              let i = "";
              if (!this.userInfo.token) {
                  const [a, m] = await u(this.wxLogin());
                  if (a) return s(a);
                  i = m.code
              }
              const o = await this.getDeviceInfo(),
                  n = {
                      code: i,
                      type: "wechat",
                      pkgid: this.pkgid,
                      gameid: this.gameId,
                      actived: o.isActived ? 1 : 0,
                      pkgbnd: this.pkgbnd,
                      adid: this.adid,
                      promotion_id: this.promotionId,
                      devid: o.deviceId,
                      clue_token: o.clue_token,
                      request_id: this.query.req_id || ""
                  };
              this.userInfo.uid && (n.uid = this.userInfo.uid);
              const [c, d] = await u(U(Object.assign(this.query, n), this));
              if (c) {
                  if (c.name === "421") {
                      if (this.reloginNum += 1, this.reloginNum >= 3) return s(this.err(4002, `Login Error: ${c.message}`));
                      this.userInfo.token = "";
                      const [a, m] = await u(this.login());
                      return a ? s(this.err(4002, `Login Error: ${c.message}`)) : e(m)
                  }
                  this.showMessage(c.message, 999);
                  return
              }
              o.deviceId = d.devid, o.isActived = !0, await this.setDeviceInfo(o), await this.setLogined(d), e({
                  uid: this.userInfo.uid,
                  username: this.userInfo.username,
                  sign: this.userInfo.sign
              })
          })
      }
      pay(e) {
          return this.logCheck(7, e), this.checkLogined(), new Promise(async (s, i) => {
              const o = await this.getDeviceInfo(),
                  n = {
                      uid: this.userInfo.uid,
                      money: e.amount,
                      cburl: e.cburl,
                      attach: e.attach,
                      sid: e.serverId,
                      servername: e.serverName,
                      roleid: e.roleId,
                      rolename: e.roleName,
                      productid: e.productId,
                      productdesc: e.productDesc,
                      productname: e.productName,
                      vip: e.vipLevel,
                      istest: 0,
                      gameid: this.gameId,
                      pkgid: this.pkgid,
                      pkgbnd: this.pkgbnd,
                      adid: this.adid,
                      promotion_id: this.promotionId,
                      devid: o.deviceId,
                      os: o.os,
                      platform: o.platform,
                      wechat: this.userInfo.wechat,
                      wechat_sk: this.userInfo.wechat_sk
                  },
                  [c, d] = await u(O(n, this));
              if (c) return i(this.err(4011, `Create Order Error: ${c}`));
              if (d.is_qie === 0)
                  if (d.status === 0) {
                      let a = d.money;
                      this.pkgConfig && this.pkgConfig.channel.param6 && (a = a / this.pkgConfig.channel.param6), wx.requestMidasPayment({
                          mode: "game",
                          env: 0,
                          offerId: d.offer_id,
                          currencyType: "CNY",
                          platform: o.platform,
                          buyQuantity: a,
                          outTradeNo: d.out_trade_no,
                          success: async ({
                              errMsg: m
                          }) => {
                              console.log(m);
                              const p = {
                                      out_trade_no: d.out_trade_no,
                                      gameid: this.gameId,
                                      uid: this.userInfo.uid,
                                      wechat: this.userInfo.wechat,
                                      wechat_sk: this.userInfo.wechat_sk
                                  },
                                  [v, k] = await u(z(p, this));
                              if (v) return i(this.err(4013, `Midas Pay Error: ${v}`));
                              k.status === 2 ? (console.log("支付成功，返回游戏"), s({})) : (console.error("充值失败", k), i(this.err(4014, `Pay Status Error: ${k.status} --- ` + JSON.stringify(k))))
                          },
                          fail: ({
                              errMsg: m,
                              errCode: p
                          }) => {
                              console.log("充值失败"), console.error(m, p), i(this.err(4012, `wx.requestMidasPayment err: ${m} code: ${p}`))
                          }
                      })
                  } else console.log("支付成功，返回游戏"), s({});
              else switch (d.qzfType) {
                  case "customer":
                      this.customerPay(e, d).then(a => s(a)).catch(a => i(a));
                      break;
                  case "clipboard":
                      this.clipboardPay(e, d).then(a => s(a)).catch(a => i(a));
                      break;
                  case "qrcode":
                      this.qrcodePay(e, d).then(a => s(a)).catch(a => i(a));
                      break;
                  default:
                      this.customerPay(e, d).then(a => s(a)).catch(a => i(a));
                      break
              }
          })
      }
      customerPay(e, s) {
          return new Promise((i, o) => {
              const n = "https://bzj-oss-sdk.bzkj1.cn/sdk/image/sendpay.jpg";
              wx.showModal({
                  title: e.productName,
                  content: s.money / 100 + "元",
                  showCancel: !1,
                  confirmText: "点击充值",
                  success: ({
                      confirm: c
                  }) => {
                      c && wx.openCustomerServiceConversation({
                          sessionFrom: "order_" + s.out_trade_no,
                          showMessageCard: !0,
                          sendMessageTitle: `订单号：${s.out_trade_no}`,
                          sendMessagePath: "order_" + s.out_trade_no,
                          sendMessageImg: n,
                          success: () => {
                              i({})
                          },
                          fail: () => {
                              wx.showModal({
                                  title: "温馨提示",
                                  content: "因版本限制，需通过【客服会话】进行充值，请您谅解",
                                  confirmText: "前往充值",
                                  success: ({
                                      confirm: d
                                  }) => {
                                      console.log(d), !d && o(this.err(4016, "cancel pay")), d && wx.openCustomerServiceConversation({
                                          sessionFrom: "order_" + s.out_trade_no,
                                          showMessageCard: !0,
                                          sendMessageTitle: `订单号：${s.out_trade_no}`,
                                          sendMessagePath: "order_" + s.out_trade_no,
                                          sendMessageImg: n,
                                          success: () => {
                                              i({})
                                          },
                                          fail: a => {
                                              o(this.err(4015, `wx.openCustomerServiceConversation err: ${a.errMsg}`))
                                          }
                                      })
                                  }
                              })
                          }
                      })
                  }
              })
          })
      }
      clipboardPay(e, s) {
          return new Promise(async (i, o) => {
              const [n, c] = await u(wx.setClipboardData({
                  data: s.qzfUrl
              }));
              n && o(this.err(4018, `wx.setClipboardData err: ${n.message}`)), wx.showModal({
                  title: "提示",
                  content: "已复制充值链接，请进入客服后粘贴，然后点击链接进行充值！",
                  success: ({
                      confirm: d
                  }) => {
                      d && wx.openCustomerServiceConversation({
                          success: () => {
                              i({})
                          },
                          fail: a => {
                              o(this.err(4018, `wx.openCustomerServiceConversation err: ${a.errMsg}`))
                          }
                      })
                  }
              })
          })
      }
      qrcodePay(e, s) {
          return new Promise(async (i, o) => {
              const [n, c] = await u(wx.previewImage({
                  urls: [s.qzfQrurl]
              }));
              n && o(this.err(4019, `wx.previewImage err: ${n.message}`)), i({})
          })
      }
      msgSecCheck(e) {
          return this.logCheck(8, e), this.checkLogined(), new Promise(async (s, i) => {
              const o = {
                      pkgid: this.pkgid,
                      uid: this.userInfo.uid,
                      scene: e.scene,
                      content: e.content,
                      nickname: e.roleName,
                      sid: e.serverId,
                      servername: e.serverName,
                      roleid: e.roleId,
                      rolename: e.roleName
                  },
                  [n, c] = await u(E(o, this));
              if (n) return i(this.err(4017, `msgSecCheck api error: ${n}`));
              s(c)
          })
      }
      async checkAuth(e) {
          const [s, i] = await u(wx.getSetting());
          if (s) throw new Error("检查权限失败");
          if (!i.authSetting[e]) {
              const [n, c] = await u(wx.authorize({
                  scope: e
              }));
              if (n) return console.error(n), !1
          }
          return !0
      }
      wxGetGameClubData(e) {
          return this.logCheck(13, e), this.checkLogined(), new Promise(async (s, i) => {
              const [o, n] = await u(this.checkAuth("scope.gameClubData"));
              if (o || !n) return i(this.err(4031, "wxGetGameClubData errr"));
              const c = {
                      1: "join_time",
                      3: "status",
                      4: "like_num",
                      5: "comment_num",
                      6: "post_num",
                      7: "video_num",
                      8: "like_official_num",
                      9: "comment_official_num",
                      10: "topic_post_num"
                  },
                  d = Object.keys(c).map(a => a === "10" ? {
                      type: parseInt(a),
                      subKey: e || ""
                  } : {
                      type: parseInt(a)
                  });
              wx.getGameClubData({
                  dataTypeList: d,
                  success: async a => {
                      if (a.data.errCode !== 0) return console.log(a), i(this.err(4032, "wxGetGameClubData errr"));
                      const m = {
                              pkgid: this.pkgid,
                              wechat_sk: this.userInfo.wechat_sk,
                              iv: a.iv,
                              encrypted_data: a.encryptedData
                          },
                          [p, v] = await u(N(m, this));
                      if (p) return i(this.err(4033, "wxGetGameClubData errr"));
                      const k = v.dataList,
                          A = {};
                      k.forEach(D => {
                          const Z = D.dataType.type,
                              ee = D.value,
                              te = c[Z];
                          A[te] = ee
                      }), s(A)
                  },
                  fail: a => {
                      console.log(a), i(this.err(4032, "wxGetGameClubData errr22"))
                  }
              })
          })
      }
      openCustomerService(e) {
          return this.logCheck(11), this.checkInit(), new Promise((s, i) => {
              wx.openCustomerServiceConversation({
                  success: () => {
                      s({})
                  },
                  fail: o => {
                      i(this.err(4051, `wx.openCustomerServiceConversation err: ${o.errMsg}`))
                  }
              })
          })
      }
      share(e = {}, s = !1) {
          return this.logCheck(10), this.checkLogined(), new Promise(async (i, o) => {
              if (!s) {
                  const [n, c] = await u(this.getShareConf());
                  n && o(this.err(4042, `share err: ${n.message}`)), e = {
                      title: c.title,
                      imageUrl: c.imgurl,
                      imageUrlId: c.imgid,
                      ...e
                  }
              }
              wx.shareAppMessage(e), i({})
          })
      }
      wxRequestSubscribeMessage(e) {
          return this.logCheck(13), this.checkLogined(), new Promise(async (s, i) => {
              wx.onTouchEnd(async () => {
                  const [o, n] = await u(wx.requestSubscribeMessage({
                      tmplIds: e
                  }));
                  o && (console.log(o), i(this.err(4061, `wx.requestSubscribeMessage err: ${o.message}`))), s(n)
              })
          })
      }
      getPfUserInfo(e = "完善信息") {
          return new Promise((s, i) => {
              if (this.wxUserProfile) return this.log("[缓存]userInfo:", this.wxUserProfile.userInfo), s(this.wxUserProfile);
              wx.getUserProfile({
                  desc: e,
                  success: o => {
                      this.wxUserProfile = o, this.log("userInfo:", this.wxUserProfile.userInfo), s(this.wxUserProfile)
                  },
                  fail: o => {
                      const n = o.errMsg || "获取用户信息失败";
                      i(this.err(4062, n))
                  }
              })
          })
      }
      getUserRiskRank(e) {
          return this.checkLogined(), new Promise(async (s, i) => {
              const o = {
                      pkgid: this.pkgid,
                      uid: this.userInfo.uid,
                      scene: e.scene
                  },
                  [n, c] = await u(T(o, this));
              if (n) return i(this.err(4017, `getUserRiskRank api error: ${n}`));
              s(c)
          })
      }
  }
  const {
      gameid: J,
      pkgbnd: Y,
      pkgid: H
  } = require("./config.js"), y = new Q({
      gameid: J,
      pkgbnd: Y,
      pkgid: H
  });
  y.setStorage(new g), y.setRequest(L()), y.setSystem(new M);
  const X = y;
  l.cqisdk = X, Object.defineProperty(l, Symbol.toStringTag, {
      value: "Module"
  })
});