function cqi(){
  var Lt = Object.defineProperty, Mt = Object.defineProperties;
var qt = Object.getOwnPropertyDescriptors;
var Be = Object.getOwnPropertySymbols;
var $t = Object.prototype.hasOwnProperty, jt = Object.prototype.propertyIsEnumerable;
var fe = (r, e, t) => e in r ? Lt(r, e, { enumerable: !0, configurable: !0, writable: !0, value: t }) : r[e] = t, M = (r, e) => {
  for (var t in e || (e = {}))
    $t.call(e, t) && fe(r, t, e[t]);
  if (Be)
    for (var t of Be(e))
      jt.call(e, t) && fe(r, t, e[t]);
  return r;
}, j = (r, e) => Mt(r, qt(e));
var x = (r, e, t) => (fe(r, typeof e != "symbol" ? e + "" : e, t), t);
var E = (r, e, t) => new Promise((n, i) => {
  var s = (c) => {
    try {
      a(t.next(c));
    } catch (u) {
      i(u);
    }
  }, o = (c) => {
    try {
      a(t.throw(c));
    } catch (u) {
      i(u);
    }
  }, a = (c) => c.done ? n(c.value) : Promise.resolve(c.value).then(s, o);
  a((t = t.apply(r, e)).next());
});
typeof navigator != "undefined" && navigator.product === void 0 && (navigator.product = "NativeScript");
class Ht {
  setStorage(e) {
    return tt.setStorage({
      key: e.key,
      data: e.data
    });
  }
  getStorage(e) {
    return new Promise((t) => {
      tt.getStorage({
        key: e.key,
        default: {},
        success: function(n) {
          t(n);
        },
        fail: function() {
          t({});
        }
      });
    });
  }
}
const ye = typeof global != "undefined" ? global : typeof self != "undefined" ? self : typeof window != "undefined" ? window : {};
function Ze(r, e) {
  return function() {
    return r.apply(e, arguments);
  };
}
const { toString: Yt } = Object.prototype, { getPrototypeOf: Ie } = Object, te = ((r) => (e) => {
  const t = Yt.call(e);
  return r[t] || (r[t] = t.slice(8, -1).toLowerCase());
})(/* @__PURE__ */ Object.create(null)), C = (r) => (r = r.toLowerCase(), (e) => te(e) === r), re = (r) => (e) => typeof e === r, { isArray: $ } = Array, Y = re("undefined");
function zt(r) {
  return r !== null && !Y(r) && r.constructor !== null && !Y(r.constructor) && k(r.constructor.isBuffer) && r.constructor.isBuffer(r);
}
const et = C("ArrayBuffer");
function Gt(r) {
  let e;
  return typeof ArrayBuffer != "undefined" && ArrayBuffer.isView ? e = ArrayBuffer.isView(r) : e = r && r.buffer && et(r.buffer), e;
}
const Vt = re("string"), k = re("function"), rt = re("number"), ne = (r) => r !== null && typeof r == "object", Jt = (r) => r === !0 || r === !1, J = (r) => {
  if (te(r) !== "object")
    return !1;
  const e = Ie(r);
  return (e === null || e === Object.prototype || Object.getPrototypeOf(e) === null) && !(Symbol.toStringTag in r) && !(Symbol.iterator in r);
}, Wt = C("Date"), Kt = C("File"), Xt = C("Blob"), Qt = C("FileList"), Zt = (r) => ne(r) && k(r.pipe), er = (r) => {
  let e;
  return r && (typeof FormData == "function" && r instanceof FormData || k(r.append) && ((e = te(r)) === "formdata" || // detect form-data instance
  e === "object" && k(r.toString) && r.toString() === "[object FormData]"));
}, tr = C("URLSearchParams"), rr = (r) => r.trim ? r.trim() : r.replace(/^[\s\uFEFF\xA0]+|[\s\uFEFF\xA0]+$/g, "");
function z(r, e, { allOwnKeys: t = !1 } = {}) {
  if (r === null || typeof r == "undefined")
    return;
  let n, i;
  if (typeof r != "object" && (r = [r]), $(r))
    for (n = 0, i = r.length; n < i; n++)
      e.call(null, r[n], n, r);
  else {
    const s = t ? Object.getOwnPropertyNames(r) : Object.keys(r), o = s.length;
    let a;
    for (n = 0; n < o; n++)
      a = s[n], e.call(null, r[a], a, r);
  }
}
function nt(r, e) {
  e = e.toLowerCase();
  const t = Object.keys(r);
  let n = t.length, i;
  for (; n-- > 0; )
    if (i = t[n], e === i.toLowerCase())
      return i;
  return null;
}
const it = (() => typeof globalThis != "undefined" ? globalThis : typeof self != "undefined" ? self : typeof window != "undefined" ? window : ye)(), st = (r) => !Y(r) && r !== it;
function Ee() {
  const { caseless: r } = st(this) && this || {}, e = {}, t = (n, i) => {
    const s = r && nt(e, i) || i;
    J(e[s]) && J(n) ? e[s] = Ee(e[s], n) : J(n) ? e[s] = Ee({}, n) : $(n) ? e[s] = n.slice() : e[s] = n;
  };
  for (let n = 0, i = arguments.length; n < i; n++)
    arguments[n] && z(arguments[n], t);
  return e;
}
const nr = (r, e, t, { allOwnKeys: n } = {}) => (z(e, (i, s) => {
  t && k(i) ? r[s] = Ze(i, t) : r[s] = i;
}, { allOwnKeys: n }), r), ir = (r) => (r.charCodeAt(0) === 65279 && (r = r.slice(1)), r), sr = (r, e, t, n) => {
  r.prototype = Object.create(e.prototype, n), r.prototype.constructor = r, Object.defineProperty(r, "super", {
    value: e.prototype
  }), t && Object.assign(r.prototype, t);
}, or = (r, e, t, n) => {
  let i, s, o;
  const a = {};
  if (e = e || {}, r == null)
    return e;
  do {
    for (i = Object.getOwnPropertyNames(r), s = i.length; s-- > 0; )
      o = i[s], (!n || n(o, r, e)) && !a[o] && (e[o] = r[o], a[o] = !0);
    r = t !== !1 && Ie(r);
  } while (r && (!t || t(r, e)) && r !== Object.prototype);
  return e;
}, ar = (r, e, t) => {
  r = String(r), (t === void 0 || t > r.length) && (t = r.length), t -= e.length;
  const n = r.indexOf(e, t);
  return n !== -1 && n === t;
}, cr = (r) => {
  if (!r)
    return null;
  if ($(r))
    return r;
  let e = r.length;
  if (!rt(e))
    return null;
  const t = new Array(e);
  for (; e-- > 0; )
    t[e] = r[e];
  return t;
}, ur = ((r) => (e) => r && e instanceof r)(typeof Uint8Array != "undefined" && Ie(Uint8Array)), lr = (r, e) => {
  const n = (r && r[Symbol.iterator]).call(r);
  let i;
  for (; (i = n.next()) && !i.done; ) {
    const s = i.value;
    e.call(r, s[0], s[1]);
  }
}, fr = (r, e) => {
  let t;
  const n = [];
  for (; (t = r.exec(e)) !== null; )
    n.push(t);
  return n;
}, hr = C("HTMLFormElement"), dr = (r) => r.toLowerCase().replace(
  /[-_\s]([a-z\d])(\w*)/g,
  function(t, n, i) {
    return n.toUpperCase() + i;
  }
), Ne = (({ hasOwnProperty: r }) => (e, t) => r.call(e, t))(Object.prototype), pr = C("RegExp"), ot = (r, e) => {
  const t = Object.getOwnPropertyDescriptors(r), n = {};
  z(t, (i, s) => {
    let o;
    (o = e(i, s, r)) !== !1 && (n[s] = o || i);
  }), Object.defineProperties(r, n);
}, mr = (r) => {
  ot(r, (e, t) => {
    if (k(r) && ["arguments", "caller", "callee"].indexOf(t) !== -1)
      return !1;
    const n = r[t];
    if (k(n)) {
      if (e.enumerable = !1, "writable" in e) {
        e.writable = !1;
        return;
      }
      e.set || (e.set = () => {
        throw Error("Can not rewrite read-only method '" + t + "'");
      });
    }
  });
}, wr = (r, e) => {
  const t = {}, n = (i) => {
    i.forEach((s) => {
      t[s] = !0;
    });
  };
  return $(r) ? n(r) : n(String(r).split(e)), t;
}, gr = () => {
}, yr = (r, e) => (r = +r, Number.isFinite(r) ? r : e), he = "abcdefghijklmnopqrstuvwxyz", Le = "0123456789", at = {
  DIGIT: Le,
  ALPHA: he,
  ALPHA_DIGIT: he + he.toUpperCase() + Le
}, Er = (r = 16, e = at.ALPHA_DIGIT) => {
  let t = "";
  const { length: n } = e;
  for (; r--; )
    t += e[Math.random() * n | 0];
  return t;
};
function xr(r) {
  return !!(r && k(r.append) && r[Symbol.toStringTag] === "FormData" && r[Symbol.iterator]);
}
const Ar = (r) => {
  const e = new Array(10), t = (n, i) => {
    if (ne(n)) {
      if (e.indexOf(n) >= 0)
        return;
      if (!("toJSON" in n)) {
        e[i] = n;
        const s = $(n) ? [] : {};
        return z(n, (o, a) => {
          const c = t(o, i + 1);
          !Y(c) && (s[a] = c);
        }), e[i] = void 0, s;
      }
    }
    return n;
  };
  return t(r, 0);
}, Sr = C("AsyncFunction"), Rr = (r) => r && (ne(r) || k(r)) && k(r.then) && k(r.catch), l = {
  isArray: $,
  isArrayBuffer: et,
  isBuffer: zt,
  isFormData: er,
  isArrayBufferView: Gt,
  isString: Vt,
  isNumber: rt,
  isBoolean: Jt,
  isObject: ne,
  isPlainObject: J,
  isUndefined: Y,
  isDate: Wt,
  isFile: Kt,
  isBlob: Xt,
  isRegExp: pr,
  isFunction: k,
  isStream: Zt,
  isURLSearchParams: tr,
  isTypedArray: ur,
  isFileList: Qt,
  forEach: z,
  merge: Ee,
  extend: nr,
  trim: rr,
  stripBOM: ir,
  inherits: sr,
  toFlatObject: or,
  kindOf: te,
  kindOfTest: C,
  endsWith: ar,
  toArray: cr,
  forEachEntry: lr,
  matchAll: fr,
  isHTMLForm: hr,
  hasOwnProperty: Ne,
  hasOwnProp: Ne,
  // an alias to avoid ESLint no-prototype-builtins detection
  reduceDescriptors: ot,
  freezeMethods: mr,
  toObjectSet: wr,
  toCamelCase: dr,
  noop: gr,
  toFiniteNumber: yr,
  findKey: nt,
  global: it,
  isContextDefined: st,
  ALPHABET: at,
  generateString: Er,
  isSpecCompliantForm: xr,
  toJSONObject: Ar,
  isAsyncFn: Sr,
  isThenable: Rr
};
var F = [], I = [], Ir = typeof Uint8Array != "undefined" ? Uint8Array : Array, ke = !1;
function ct() {
  ke = !0;
  for (var r = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/", e = 0, t = r.length; e < t; ++e)
    F[e] = r[e], I[r.charCodeAt(e)] = e;
  I["-".charCodeAt(0)] = 62, I["_".charCodeAt(0)] = 63;
}
function kr(r) {
  ke || ct();
  var e, t, n, i, s, o, a = r.length;
  if (a % 4 > 0)
    throw new Error("Invalid string. Length must be a multiple of 4");
  s = r[a - 2] === "=" ? 2 : r[a - 1] === "=" ? 1 : 0, o = new Ir(a * 3 / 4 - s), n = s > 0 ? a - 4 : a;
  var c = 0;
  for (e = 0, t = 0; e < n; e += 4, t += 3)
    i = I[r.charCodeAt(e)] << 18 | I[r.charCodeAt(e + 1)] << 12 | I[r.charCodeAt(e + 2)] << 6 | I[r.charCodeAt(e + 3)], o[c++] = i >> 16 & 255, o[c++] = i >> 8 & 255, o[c++] = i & 255;
  return s === 2 ? (i = I[r.charCodeAt(e)] << 2 | I[r.charCodeAt(e + 1)] >> 4, o[c++] = i & 255) : s === 1 && (i = I[r.charCodeAt(e)] << 10 | I[r.charCodeAt(e + 1)] << 4 | I[r.charCodeAt(e + 2)] >> 2, o[c++] = i >> 8 & 255, o[c++] = i & 255), o;
}
function _r(r) {
  return F[r >> 18 & 63] + F[r >> 12 & 63] + F[r >> 6 & 63] + F[r & 63];
}
function br(r, e, t) {
  for (var n, i = [], s = e; s < t; s += 3)
    n = (r[s] << 16) + (r[s + 1] << 8) + r[s + 2], i.push(_r(n));
  return i.join("");
}
function Me(r) {
  ke || ct();
  for (var e, t = r.length, n = t % 3, i = "", s = [], o = 16383, a = 0, c = t - n; a < c; a += o)
    s.push(br(r, a, a + o > c ? c : a + o));
  return n === 1 ? (e = r[t - 1], i += F[e >> 2], i += F[e << 4 & 63], i += "==") : n === 2 && (e = (r[t - 2] << 8) + r[t - 1], i += F[e >> 10], i += F[e >> 4 & 63], i += F[e << 2 & 63], i += "="), s.push(i), s.join("");
}
function ie(r, e, t, n, i) {
  var s, o, a = i * 8 - n - 1, c = (1 << a) - 1, u = c >> 1, h = -7, d = t ? i - 1 : 0, y = t ? -1 : 1, m = r[e + d];
  for (d += y, s = m & (1 << -h) - 1, m >>= -h, h += a; h > 0; s = s * 256 + r[e + d], d += y, h -= 8)
    ;
  for (o = s & (1 << -h) - 1, s >>= -h, h += n; h > 0; o = o * 256 + r[e + d], d += y, h -= 8)
    ;
  if (s === 0)
    s = 1 - u;
  else {
    if (s === c)
      return o ? NaN : (m ? -1 : 1) * (1 / 0);
    o = o + Math.pow(2, n), s = s - u;
  }
  return (m ? -1 : 1) * o * Math.pow(2, s - n);
}
function ut(r, e, t, n, i, s) {
  var o, a, c, u = s * 8 - i - 1, h = (1 << u) - 1, d = h >> 1, y = i === 23 ? Math.pow(2, -24) - Math.pow(2, -77) : 0, m = n ? 0 : s - 1, p = n ? 1 : -1, w = e < 0 || e === 0 && 1 / e < 0 ? 1 : 0;
  for (e = Math.abs(e), isNaN(e) || e === 1 / 0 ? (a = isNaN(e) ? 1 : 0, o = h) : (o = Math.floor(Math.log(e) / Math.LN2), e * (c = Math.pow(2, -o)) < 1 && (o--, c *= 2), o + d >= 1 ? e += y / c : e += y * Math.pow(2, 1 - d), e * c >= 2 && (o++, c /= 2), o + d >= h ? (a = 0, o = h) : o + d >= 1 ? (a = (e * c - 1) * Math.pow(2, i), o = o + d) : (a = e * Math.pow(2, d - 1) * Math.pow(2, i), o = 0)); i >= 8; r[t + m] = a & 255, m += p, a /= 256, i -= 8)
    ;
  for (o = o << i | a, u += i; u > 0; r[t + m] = o & 255, m += p, o /= 256, u -= 8)
    ;
  r[t + m - p] |= w * 128;
}
var Fr = {}.toString, lt = Array.isArray || function(r) {
  return Fr.call(r) == "[object Array]";
};
/*!
 * The buffer module from node.js, for the browser.
 *
 * @author   Feross Aboukhadijeh <feross@feross.org> <http://feross.org>
 * @license  MIT
 */
var Tr = 50;
f.TYPED_ARRAY_SUPPORT = ye.TYPED_ARRAY_SUPPORT !== void 0 ? ye.TYPED_ARRAY_SUPPORT : !0;
Q();
function Q() {
  return f.TYPED_ARRAY_SUPPORT ? 2147483647 : 1073741823;
}
function O(r, e) {
  if (Q() < e)
    throw new RangeError("Invalid typed array length");
  return f.TYPED_ARRAY_SUPPORT ? (r = new Uint8Array(e), r.__proto__ = f.prototype) : (r === null && (r = new f(e)), r.length = e), r;
}
function f(r, e, t) {
  if (!f.TYPED_ARRAY_SUPPORT && !(this instanceof f))
    return new f(r, e, t);
  if (typeof r == "number") {
    if (typeof e == "string")
      throw new Error(
        "If encoding is specified then the first argument must be a string"
      );
    return _e(this, r);
  }
  return ft(this, r, e, t);
}
f.poolSize = 8192;
f._augment = function(r) {
  return r.__proto__ = f.prototype, r;
};
function ft(r, e, t, n) {
  if (typeof e == "number")
    throw new TypeError('"value" argument must not be a number');
  return typeof ArrayBuffer != "undefined" && e instanceof ArrayBuffer ? Or(r, e, t, n) : typeof e == "string" ? Pr(r, e, t) : Ur(r, e);
}
f.from = function(r, e, t) {
  return ft(null, r, e, t);
};
f.TYPED_ARRAY_SUPPORT && (f.prototype.__proto__ = Uint8Array.prototype, f.__proto__ = Uint8Array);
function ht(r) {
  if (typeof r != "number")
    throw new TypeError('"size" argument must be a number');
  if (r < 0)
    throw new RangeError('"size" argument must not be negative');
}
function Cr(r, e, t, n) {
  return ht(e), e <= 0 ? O(r, e) : t !== void 0 ? typeof n == "string" ? O(r, e).fill(t, n) : O(r, e).fill(t) : O(r, e);
}
f.alloc = function(r, e, t) {
  return Cr(null, r, e, t);
};
function _e(r, e) {
  if (ht(e), r = O(r, e < 0 ? 0 : be(e) | 0), !f.TYPED_ARRAY_SUPPORT)
    for (var t = 0; t < e; ++t)
      r[t] = 0;
  return r;
}
f.allocUnsafe = function(r) {
  return _e(null, r);
};
f.allocUnsafeSlow = function(r) {
  return _e(null, r);
};
function Pr(r, e, t) {
  if ((typeof t != "string" || t === "") && (t = "utf8"), !f.isEncoding(t))
    throw new TypeError('"encoding" must be a valid string encoding');
  var n = dt(e, t) | 0;
  r = O(r, n);
  var i = r.write(e, t);
  return i !== n && (r = r.slice(0, i)), r;
}
function xe(r, e) {
  var t = e.length < 0 ? 0 : be(e.length) | 0;
  r = O(r, t);
  for (var n = 0; n < t; n += 1)
    r[n] = e[n] & 255;
  return r;
}
function Or(r, e, t, n) {
  if (e.byteLength, t < 0 || e.byteLength < t)
    throw new RangeError("'offset' is out of bounds");
  if (e.byteLength < t + (n || 0))
    throw new RangeError("'length' is out of bounds");
  return t === void 0 && n === void 0 ? e = new Uint8Array(e) : n === void 0 ? e = new Uint8Array(e, t) : e = new Uint8Array(e, t, n), f.TYPED_ARRAY_SUPPORT ? (r = e, r.__proto__ = f.prototype) : r = xe(r, e), r;
}
function Ur(r, e) {
  if (T(e)) {
    var t = be(e.length) | 0;
    return r = O(r, t), r.length === 0 || e.copy(r, 0, 0, t), r;
  }
  if (e) {
    if (typeof ArrayBuffer != "undefined" && e.buffer instanceof ArrayBuffer || "length" in e)
      return typeof e.length != "number" || Qr(e.length) ? O(r, 0) : xe(r, e);
    if (e.type === "Buffer" && lt(e.data))
      return xe(r, e.data);
  }
  throw new TypeError("First argument must be a string, Buffer, ArrayBuffer, Array, or array-like object.");
}
function be(r) {
  if (r >= Q())
    throw new RangeError("Attempt to allocate Buffer larger than maximum size: 0x" + Q().toString(16) + " bytes");
  return r | 0;
}
f.isBuffer = Zr;
function T(r) {
  return !!(r != null && r._isBuffer);
}
f.compare = function(e, t) {
  if (!T(e) || !T(t))
    throw new TypeError("Arguments must be Buffers");
  if (e === t)
    return 0;
  for (var n = e.length, i = t.length, s = 0, o = Math.min(n, i); s < o; ++s)
    if (e[s] !== t[s]) {
      n = e[s], i = t[s];
      break;
    }
  return n < i ? -1 : i < n ? 1 : 0;
};
f.isEncoding = function(e) {
  switch (String(e).toLowerCase()) {
    case "hex":
    case "utf8":
    case "utf-8":
    case "ascii":
    case "latin1":
    case "binary":
    case "base64":
    case "ucs2":
    case "ucs-2":
    case "utf16le":
    case "utf-16le":
      return !0;
    default:
      return !1;
  }
};
f.concat = function(e, t) {
  if (!lt(e))
    throw new TypeError('"list" argument must be an Array of Buffers');
  if (e.length === 0)
    return f.alloc(0);
  var n;
  if (t === void 0)
    for (t = 0, n = 0; n < e.length; ++n)
      t += e[n].length;
  var i = f.allocUnsafe(t), s = 0;
  for (n = 0; n < e.length; ++n) {
    var o = e[n];
    if (!T(o))
      throw new TypeError('"list" argument must be an Array of Buffers');
    o.copy(i, s), s += o.length;
  }
  return i;
};
function dt(r, e) {
  if (T(r))
    return r.length;
  if (typeof ArrayBuffer != "undefined" && typeof ArrayBuffer.isView == "function" && (ArrayBuffer.isView(r) || r instanceof ArrayBuffer))
    return r.byteLength;
  typeof r != "string" && (r = "" + r);
  var t = r.length;
  if (t === 0)
    return 0;
  for (var n = !1; ; )
    switch (e) {
      case "ascii":
      case "latin1":
      case "binary":
        return t;
      case "utf8":
      case "utf-8":
      case void 0:
        return Z(r).length;
      case "ucs2":
      case "ucs-2":
      case "utf16le":
      case "utf-16le":
        return t * 2;
      case "hex":
        return t >>> 1;
      case "base64":
        return xt(r).length;
      default:
        if (n)
          return Z(r).length;
        e = ("" + e).toLowerCase(), n = !0;
    }
}
f.byteLength = dt;
function vr(r, e, t) {
  var n = !1;
  if ((e === void 0 || e < 0) && (e = 0), e > this.length || ((t === void 0 || t > this.length) && (t = this.length), t <= 0) || (t >>>= 0, e >>>= 0, t <= e))
    return "";
  for (r || (r = "utf8"); ; )
    switch (r) {
      case "hex":
        return Yr(this, e, t);
      case "utf8":
      case "utf-8":
        return wt(this, e, t);
      case "ascii":
        return jr(this, e, t);
      case "latin1":
      case "binary":
        return Hr(this, e, t);
      case "base64":
        return qr(this, e, t);
      case "ucs2":
      case "ucs-2":
      case "utf16le":
      case "utf-16le":
        return zr(this, e, t);
      default:
        if (n)
          throw new TypeError("Unknown encoding: " + r);
        r = (r + "").toLowerCase(), n = !0;
    }
}
f.prototype._isBuffer = !0;
function N(r, e, t) {
  var n = r[e];
  r[e] = r[t], r[t] = n;
}
f.prototype.swap16 = function() {
  var e = this.length;
  if (e % 2 !== 0)
    throw new RangeError("Buffer size must be a multiple of 16-bits");
  for (var t = 0; t < e; t += 2)
    N(this, t, t + 1);
  return this;
};
f.prototype.swap32 = function() {
  var e = this.length;
  if (e % 4 !== 0)
    throw new RangeError("Buffer size must be a multiple of 32-bits");
  for (var t = 0; t < e; t += 4)
    N(this, t, t + 3), N(this, t + 1, t + 2);
  return this;
};
f.prototype.swap64 = function() {
  var e = this.length;
  if (e % 8 !== 0)
    throw new RangeError("Buffer size must be a multiple of 64-bits");
  for (var t = 0; t < e; t += 8)
    N(this, t, t + 7), N(this, t + 1, t + 6), N(this, t + 2, t + 5), N(this, t + 3, t + 4);
  return this;
};
f.prototype.toString = function() {
  var e = this.length | 0;
  return e === 0 ? "" : arguments.length === 0 ? wt(this, 0, e) : vr.apply(this, arguments);
};
f.prototype.equals = function(e) {
  if (!T(e))
    throw new TypeError("Argument must be a Buffer");
  return this === e ? !0 : f.compare(this, e) === 0;
};
f.prototype.inspect = function() {
  var e = "", t = Tr;
  return this.length > 0 && (e = this.toString("hex", 0, t).match(/.{2}/g).join(" "), this.length > t && (e += " ... ")), "<Buffer " + e + ">";
};
f.prototype.compare = function(e, t, n, i, s) {
  if (!T(e))
    throw new TypeError("Argument must be a Buffer");
  if (t === void 0 && (t = 0), n === void 0 && (n = e ? e.length : 0), i === void 0 && (i = 0), s === void 0 && (s = this.length), t < 0 || n > e.length || i < 0 || s > this.length)
    throw new RangeError("out of range index");
  if (i >= s && t >= n)
    return 0;
  if (i >= s)
    return -1;
  if (t >= n)
    return 1;
  if (t >>>= 0, n >>>= 0, i >>>= 0, s >>>= 0, this === e)
    return 0;
  for (var o = s - i, a = n - t, c = Math.min(o, a), u = this.slice(i, s), h = e.slice(t, n), d = 0; d < c; ++d)
    if (u[d] !== h[d]) {
      o = u[d], a = h[d];
      break;
    }
  return o < a ? -1 : a < o ? 1 : 0;
};
function pt(r, e, t, n, i) {
  if (r.length === 0)
    return -1;
  if (typeof t == "string" ? (n = t, t = 0) : t > 2147483647 ? t = 2147483647 : t < -2147483648 && (t = -2147483648), t = +t, isNaN(t) && (t = i ? 0 : r.length - 1), t < 0 && (t = r.length + t), t >= r.length) {
    if (i)
      return -1;
    t = r.length - 1;
  } else if (t < 0)
    if (i)
      t = 0;
    else
      return -1;
  if (typeof e == "string" && (e = f.from(e, n)), T(e))
    return e.length === 0 ? -1 : qe(r, e, t, n, i);
  if (typeof e == "number")
    return e = e & 255, f.TYPED_ARRAY_SUPPORT && typeof Uint8Array.prototype.indexOf == "function" ? i ? Uint8Array.prototype.indexOf.call(r, e, t) : Uint8Array.prototype.lastIndexOf.call(r, e, t) : qe(r, [e], t, n, i);
  throw new TypeError("val must be string, number or Buffer");
}
function qe(r, e, t, n, i) {
  var s = 1, o = r.length, a = e.length;
  if (n !== void 0 && (n = String(n).toLowerCase(), n === "ucs2" || n === "ucs-2" || n === "utf16le" || n === "utf-16le")) {
    if (r.length < 2 || e.length < 2)
      return -1;
    s = 2, o /= 2, a /= 2, t /= 2;
  }
  function c(m, p) {
    return s === 1 ? m[p] : m.readUInt16BE(p * s);
  }
  var u;
  if (i) {
    var h = -1;
    for (u = t; u < o; u++)
      if (c(r, u) === c(e, h === -1 ? 0 : u - h)) {
        if (h === -1 && (h = u), u - h + 1 === a)
          return h * s;
      } else
        h !== -1 && (u -= u - h), h = -1;
  } else
    for (t + a > o && (t = o - a), u = t; u >= 0; u--) {
      for (var d = !0, y = 0; y < a; y++)
        if (c(r, u + y) !== c(e, y)) {
          d = !1;
          break;
        }
      if (d)
        return u;
    }
  return -1;
}
f.prototype.includes = function(e, t, n) {
  return this.indexOf(e, t, n) !== -1;
};
f.prototype.indexOf = function(e, t, n) {
  return pt(this, e, t, n, !0);
};
f.prototype.lastIndexOf = function(e, t, n) {
  return pt(this, e, t, n, !1);
};
function Dr(r, e, t, n) {
  t = Number(t) || 0;
  var i = r.length - t;
  n ? (n = Number(n), n > i && (n = i)) : n = i;
  var s = e.length;
  if (s % 2 !== 0)
    throw new TypeError("Invalid hex string");
  n > s / 2 && (n = s / 2);
  for (var o = 0; o < n; ++o) {
    var a = parseInt(e.substr(o * 2, 2), 16);
    if (isNaN(a))
      return o;
    r[t + o] = a;
  }
  return o;
}
function Br(r, e, t, n) {
  return ae(Z(e, r.length - t), r, t, n);
}
function mt(r, e, t, n) {
  return ae(Kr(e), r, t, n);
}
function Nr(r, e, t, n) {
  return mt(r, e, t, n);
}
function Lr(r, e, t, n) {
  return ae(xt(e), r, t, n);
}
function Mr(r, e, t, n) {
  return ae(Xr(e, r.length - t), r, t, n);
}
f.prototype.write = function(e, t, n, i) {
  if (t === void 0)
    i = "utf8", n = this.length, t = 0;
  else if (n === void 0 && typeof t == "string")
    i = t, n = this.length, t = 0;
  else if (isFinite(t))
    t = t | 0, isFinite(n) ? (n = n | 0, i === void 0 && (i = "utf8")) : (i = n, n = void 0);
  else
    throw new Error(
      "Buffer.write(string, encoding, offset[, length]) is no longer supported"
    );
  var s = this.length - t;
  if ((n === void 0 || n > s) && (n = s), e.length > 0 && (n < 0 || t < 0) || t > this.length)
    throw new RangeError("Attempt to write outside buffer bounds");
  i || (i = "utf8");
  for (var o = !1; ; )
    switch (i) {
      case "hex":
        return Dr(this, e, t, n);
      case "utf8":
      case "utf-8":
        return Br(this, e, t, n);
      case "ascii":
        return mt(this, e, t, n);
      case "latin1":
      case "binary":
        return Nr(this, e, t, n);
      case "base64":
        return Lr(this, e, t, n);
      case "ucs2":
      case "ucs-2":
      case "utf16le":
      case "utf-16le":
        return Mr(this, e, t, n);
      default:
        if (o)
          throw new TypeError("Unknown encoding: " + i);
        i = ("" + i).toLowerCase(), o = !0;
    }
};
f.prototype.toJSON = function() {
  return {
    type: "Buffer",
    data: Array.prototype.slice.call(this._arr || this, 0)
  };
};
function qr(r, e, t) {
  return e === 0 && t === r.length ? Me(r) : Me(r.slice(e, t));
}
function wt(r, e, t) {
  t = Math.min(r.length, t);
  for (var n = [], i = e; i < t; ) {
    var s = r[i], o = null, a = s > 239 ? 4 : s > 223 ? 3 : s > 191 ? 2 : 1;
    if (i + a <= t) {
      var c, u, h, d;
      switch (a) {
        case 1:
          s < 128 && (o = s);
          break;
        case 2:
          c = r[i + 1], (c & 192) === 128 && (d = (s & 31) << 6 | c & 63, d > 127 && (o = d));
          break;
        case 3:
          c = r[i + 1], u = r[i + 2], (c & 192) === 128 && (u & 192) === 128 && (d = (s & 15) << 12 | (c & 63) << 6 | u & 63, d > 2047 && (d < 55296 || d > 57343) && (o = d));
          break;
        case 4:
          c = r[i + 1], u = r[i + 2], h = r[i + 3], (c & 192) === 128 && (u & 192) === 128 && (h & 192) === 128 && (d = (s & 15) << 18 | (c & 63) << 12 | (u & 63) << 6 | h & 63, d > 65535 && d < 1114112 && (o = d));
      }
    }
    o === null ? (o = 65533, a = 1) : o > 65535 && (o -= 65536, n.push(o >>> 10 & 1023 | 55296), o = 56320 | o & 1023), n.push(o), i += a;
  }
  return $r(n);
}
var $e = 4096;
function $r(r) {
  var e = r.length;
  if (e <= $e)
    return String.fromCharCode.apply(String, r);
  for (var t = "", n = 0; n < e; )
    t += String.fromCharCode.apply(
      String,
      r.slice(n, n += $e)
    );
  return t;
}
function jr(r, e, t) {
  var n = "";
  t = Math.min(r.length, t);
  for (var i = e; i < t; ++i)
    n += String.fromCharCode(r[i] & 127);
  return n;
}
function Hr(r, e, t) {
  var n = "";
  t = Math.min(r.length, t);
  for (var i = e; i < t; ++i)
    n += String.fromCharCode(r[i]);
  return n;
}
function Yr(r, e, t) {
  var n = r.length;
  (!e || e < 0) && (e = 0), (!t || t < 0 || t > n) && (t = n);
  for (var i = "", s = e; s < t; ++s)
    i += Wr(r[s]);
  return i;
}
function zr(r, e, t) {
  for (var n = r.slice(e, t), i = "", s = 0; s < n.length; s += 2)
    i += String.fromCharCode(n[s] + n[s + 1] * 256);
  return i;
}
f.prototype.slice = function(e, t) {
  var n = this.length;
  e = ~~e, t = t === void 0 ? n : ~~t, e < 0 ? (e += n, e < 0 && (e = 0)) : e > n && (e = n), t < 0 ? (t += n, t < 0 && (t = 0)) : t > n && (t = n), t < e && (t = e);
  var i;
  if (f.TYPED_ARRAY_SUPPORT)
    i = this.subarray(e, t), i.__proto__ = f.prototype;
  else {
    var s = t - e;
    i = new f(s, void 0);
    for (var o = 0; o < s; ++o)
      i[o] = this[o + e];
  }
  return i;
};
function S(r, e, t) {
  if (r % 1 !== 0 || r < 0)
    throw new RangeError("offset is not uint");
  if (r + e > t)
    throw new RangeError("Trying to access beyond buffer length");
}
f.prototype.readUIntLE = function(e, t, n) {
  e = e | 0, t = t | 0, n || S(e, t, this.length);
  for (var i = this[e], s = 1, o = 0; ++o < t && (s *= 256); )
    i += this[e + o] * s;
  return i;
};
f.prototype.readUIntBE = function(e, t, n) {
  e = e | 0, t = t | 0, n || S(e, t, this.length);
  for (var i = this[e + --t], s = 1; t > 0 && (s *= 256); )
    i += this[e + --t] * s;
  return i;
};
f.prototype.readUInt8 = function(e, t) {
  return t || S(e, 1, this.length), this[e];
};
f.prototype.readUInt16LE = function(e, t) {
  return t || S(e, 2, this.length), this[e] | this[e + 1] << 8;
};
f.prototype.readUInt16BE = function(e, t) {
  return t || S(e, 2, this.length), this[e] << 8 | this[e + 1];
};
f.prototype.readUInt32LE = function(e, t) {
  return t || S(e, 4, this.length), (this[e] | this[e + 1] << 8 | this[e + 2] << 16) + this[e + 3] * 16777216;
};
f.prototype.readUInt32BE = function(e, t) {
  return t || S(e, 4, this.length), this[e] * 16777216 + (this[e + 1] << 16 | this[e + 2] << 8 | this[e + 3]);
};
f.prototype.readIntLE = function(e, t, n) {
  e = e | 0, t = t | 0, n || S(e, t, this.length);
  for (var i = this[e], s = 1, o = 0; ++o < t && (s *= 256); )
    i += this[e + o] * s;
  return s *= 128, i >= s && (i -= Math.pow(2, 8 * t)), i;
};
f.prototype.readIntBE = function(e, t, n) {
  e = e | 0, t = t | 0, n || S(e, t, this.length);
  for (var i = t, s = 1, o = this[e + --i]; i > 0 && (s *= 256); )
    o += this[e + --i] * s;
  return s *= 128, o >= s && (o -= Math.pow(2, 8 * t)), o;
};
f.prototype.readInt8 = function(e, t) {
  return t || S(e, 1, this.length), this[e] & 128 ? (255 - this[e] + 1) * -1 : this[e];
};
f.prototype.readInt16LE = function(e, t) {
  t || S(e, 2, this.length);
  var n = this[e] | this[e + 1] << 8;
  return n & 32768 ? n | 4294901760 : n;
};
f.prototype.readInt16BE = function(e, t) {
  t || S(e, 2, this.length);
  var n = this[e + 1] | this[e] << 8;
  return n & 32768 ? n | 4294901760 : n;
};
f.prototype.readInt32LE = function(e, t) {
  return t || S(e, 4, this.length), this[e] | this[e + 1] << 8 | this[e + 2] << 16 | this[e + 3] << 24;
};
f.prototype.readInt32BE = function(e, t) {
  return t || S(e, 4, this.length), this[e] << 24 | this[e + 1] << 16 | this[e + 2] << 8 | this[e + 3];
};
f.prototype.readFloatLE = function(e, t) {
  return t || S(e, 4, this.length), ie(this, e, !0, 23, 4);
};
f.prototype.readFloatBE = function(e, t) {
  return t || S(e, 4, this.length), ie(this, e, !1, 23, 4);
};
f.prototype.readDoubleLE = function(e, t) {
  return t || S(e, 8, this.length), ie(this, e, !0, 52, 8);
};
f.prototype.readDoubleBE = function(e, t) {
  return t || S(e, 8, this.length), ie(this, e, !1, 52, 8);
};
function R(r, e, t, n, i, s) {
  if (!T(r))
    throw new TypeError('"buffer" argument must be a Buffer instance');
  if (e > i || e < s)
    throw new RangeError('"value" argument is out of bounds');
  if (t + n > r.length)
    throw new RangeError("Index out of range");
}
f.prototype.writeUIntLE = function(e, t, n, i) {
  if (e = +e, t = t | 0, n = n | 0, !i) {
    var s = Math.pow(2, 8 * n) - 1;
    R(this, e, t, n, s, 0);
  }
  var o = 1, a = 0;
  for (this[t] = e & 255; ++a < n && (o *= 256); )
    this[t + a] = e / o & 255;
  return t + n;
};
f.prototype.writeUIntBE = function(e, t, n, i) {
  if (e = +e, t = t | 0, n = n | 0, !i) {
    var s = Math.pow(2, 8 * n) - 1;
    R(this, e, t, n, s, 0);
  }
  var o = n - 1, a = 1;
  for (this[t + o] = e & 255; --o >= 0 && (a *= 256); )
    this[t + o] = e / a & 255;
  return t + n;
};
f.prototype.writeUInt8 = function(e, t, n) {
  return e = +e, t = t | 0, n || R(this, e, t, 1, 255, 0), f.TYPED_ARRAY_SUPPORT || (e = Math.floor(e)), this[t] = e & 255, t + 1;
};
function se(r, e, t, n) {
  e < 0 && (e = 65535 + e + 1);
  for (var i = 0, s = Math.min(r.length - t, 2); i < s; ++i)
    r[t + i] = (e & 255 << 8 * (n ? i : 1 - i)) >>> (n ? i : 1 - i) * 8;
}
f.prototype.writeUInt16LE = function(e, t, n) {
  return e = +e, t = t | 0, n || R(this, e, t, 2, 65535, 0), f.TYPED_ARRAY_SUPPORT ? (this[t] = e & 255, this[t + 1] = e >>> 8) : se(this, e, t, !0), t + 2;
};
f.prototype.writeUInt16BE = function(e, t, n) {
  return e = +e, t = t | 0, n || R(this, e, t, 2, 65535, 0), f.TYPED_ARRAY_SUPPORT ? (this[t] = e >>> 8, this[t + 1] = e & 255) : se(this, e, t, !1), t + 2;
};
function oe(r, e, t, n) {
  e < 0 && (e = 4294967295 + e + 1);
  for (var i = 0, s = Math.min(r.length - t, 4); i < s; ++i)
    r[t + i] = e >>> (n ? i : 3 - i) * 8 & 255;
}
f.prototype.writeUInt32LE = function(e, t, n) {
  return e = +e, t = t | 0, n || R(this, e, t, 4, 4294967295, 0), f.TYPED_ARRAY_SUPPORT ? (this[t + 3] = e >>> 24, this[t + 2] = e >>> 16, this[t + 1] = e >>> 8, this[t] = e & 255) : oe(this, e, t, !0), t + 4;
};
f.prototype.writeUInt32BE = function(e, t, n) {
  return e = +e, t = t | 0, n || R(this, e, t, 4, 4294967295, 0), f.TYPED_ARRAY_SUPPORT ? (this[t] = e >>> 24, this[t + 1] = e >>> 16, this[t + 2] = e >>> 8, this[t + 3] = e & 255) : oe(this, e, t, !1), t + 4;
};
f.prototype.writeIntLE = function(e, t, n, i) {
  if (e = +e, t = t | 0, !i) {
    var s = Math.pow(2, 8 * n - 1);
    R(this, e, t, n, s - 1, -s);
  }
  var o = 0, a = 1, c = 0;
  for (this[t] = e & 255; ++o < n && (a *= 256); )
    e < 0 && c === 0 && this[t + o - 1] !== 0 && (c = 1), this[t + o] = (e / a >> 0) - c & 255;
  return t + n;
};
f.prototype.writeIntBE = function(e, t, n, i) {
  if (e = +e, t = t | 0, !i) {
    var s = Math.pow(2, 8 * n - 1);
    R(this, e, t, n, s - 1, -s);
  }
  var o = n - 1, a = 1, c = 0;
  for (this[t + o] = e & 255; --o >= 0 && (a *= 256); )
    e < 0 && c === 0 && this[t + o + 1] !== 0 && (c = 1), this[t + o] = (e / a >> 0) - c & 255;
  return t + n;
};
f.prototype.writeInt8 = function(e, t, n) {
  return e = +e, t = t | 0, n || R(this, e, t, 1, 127, -128), f.TYPED_ARRAY_SUPPORT || (e = Math.floor(e)), e < 0 && (e = 255 + e + 1), this[t] = e & 255, t + 1;
};
f.prototype.writeInt16LE = function(e, t, n) {
  return e = +e, t = t | 0, n || R(this, e, t, 2, 32767, -32768), f.TYPED_ARRAY_SUPPORT ? (this[t] = e & 255, this[t + 1] = e >>> 8) : se(this, e, t, !0), t + 2;
};
f.prototype.writeInt16BE = function(e, t, n) {
  return e = +e, t = t | 0, n || R(this, e, t, 2, 32767, -32768), f.TYPED_ARRAY_SUPPORT ? (this[t] = e >>> 8, this[t + 1] = e & 255) : se(this, e, t, !1), t + 2;
};
f.prototype.writeInt32LE = function(e, t, n) {
  return e = +e, t = t | 0, n || R(this, e, t, 4, 2147483647, -2147483648), f.TYPED_ARRAY_SUPPORT ? (this[t] = e & 255, this[t + 1] = e >>> 8, this[t + 2] = e >>> 16, this[t + 3] = e >>> 24) : oe(this, e, t, !0), t + 4;
};
f.prototype.writeInt32BE = function(e, t, n) {
  return e = +e, t = t | 0, n || R(this, e, t, 4, 2147483647, -2147483648), e < 0 && (e = 4294967295 + e + 1), f.TYPED_ARRAY_SUPPORT ? (this[t] = e >>> 24, this[t + 1] = e >>> 16, this[t + 2] = e >>> 8, this[t + 3] = e & 255) : oe(this, e, t, !1), t + 4;
};
function gt(r, e, t, n, i, s) {
  if (t + n > r.length)
    throw new RangeError("Index out of range");
  if (t < 0)
    throw new RangeError("Index out of range");
}
function yt(r, e, t, n, i) {
  return i || gt(r, e, t, 4), ut(r, e, t, n, 23, 4), t + 4;
}
f.prototype.writeFloatLE = function(e, t, n) {
  return yt(this, e, t, !0, n);
};
f.prototype.writeFloatBE = function(e, t, n) {
  return yt(this, e, t, !1, n);
};
function Et(r, e, t, n, i) {
  return i || gt(r, e, t, 8), ut(r, e, t, n, 52, 8), t + 8;
}
f.prototype.writeDoubleLE = function(e, t, n) {
  return Et(this, e, t, !0, n);
};
f.prototype.writeDoubleBE = function(e, t, n) {
  return Et(this, e, t, !1, n);
};
f.prototype.copy = function(e, t, n, i) {
  if (n || (n = 0), !i && i !== 0 && (i = this.length), t >= e.length && (t = e.length), t || (t = 0), i > 0 && i < n && (i = n), i === n || e.length === 0 || this.length === 0)
    return 0;
  if (t < 0)
    throw new RangeError("targetStart out of bounds");
  if (n < 0 || n >= this.length)
    throw new RangeError("sourceStart out of bounds");
  if (i < 0)
    throw new RangeError("sourceEnd out of bounds");
  i > this.length && (i = this.length), e.length - t < i - n && (i = e.length - t + n);
  var s = i - n, o;
  if (this === e && n < t && t < i)
    for (o = s - 1; o >= 0; --o)
      e[o + t] = this[o + n];
  else if (s < 1e3 || !f.TYPED_ARRAY_SUPPORT)
    for (o = 0; o < s; ++o)
      e[o + t] = this[o + n];
  else
    Uint8Array.prototype.set.call(
      e,
      this.subarray(n, n + s),
      t
    );
  return s;
};
f.prototype.fill = function(e, t, n, i) {
  if (typeof e == "string") {
    if (typeof t == "string" ? (i = t, t = 0, n = this.length) : typeof n == "string" && (i = n, n = this.length), e.length === 1) {
      var s = e.charCodeAt(0);
      s < 256 && (e = s);
    }
    if (i !== void 0 && typeof i != "string")
      throw new TypeError("encoding must be a string");
    if (typeof i == "string" && !f.isEncoding(i))
      throw new TypeError("Unknown encoding: " + i);
  } else
    typeof e == "number" && (e = e & 255);
  if (t < 0 || this.length < t || this.length < n)
    throw new RangeError("Out of range index");
  if (n <= t)
    return this;
  t = t >>> 0, n = n === void 0 ? this.length : n >>> 0, e || (e = 0);
  var o;
  if (typeof e == "number")
    for (o = t; o < n; ++o)
      this[o] = e;
  else {
    var a = T(e) ? e : Z(new f(e, i).toString()), c = a.length;
    for (o = 0; o < n - t; ++o)
      this[o + t] = a[o % c];
  }
  return this;
};
var Gr = /[^+\/0-9A-Za-z-_]/g;
function Vr(r) {
  if (r = Jr(r).replace(Gr, ""), r.length < 2)
    return "";
  for (; r.length % 4 !== 0; )
    r = r + "=";
  return r;
}
function Jr(r) {
  return r.trim ? r.trim() : r.replace(/^\s+|\s+$/g, "");
}
function Wr(r) {
  return r < 16 ? "0" + r.toString(16) : r.toString(16);
}
function Z(r, e) {
  e = e || 1 / 0;
  for (var t, n = r.length, i = null, s = [], o = 0; o < n; ++o) {
    if (t = r.charCodeAt(o), t > 55295 && t < 57344) {
      if (!i) {
        if (t > 56319) {
          (e -= 3) > -1 && s.push(239, 191, 189);
          continue;
        } else if (o + 1 === n) {
          (e -= 3) > -1 && s.push(239, 191, 189);
          continue;
        }
        i = t;
        continue;
      }
      if (t < 56320) {
        (e -= 3) > -1 && s.push(239, 191, 189), i = t;
        continue;
      }
      t = (i - 55296 << 10 | t - 56320) + 65536;
    } else
      i && (e -= 3) > -1 && s.push(239, 191, 189);
    if (i = null, t < 128) {
      if ((e -= 1) < 0)
        break;
      s.push(t);
    } else if (t < 2048) {
      if ((e -= 2) < 0)
        break;
      s.push(
        t >> 6 | 192,
        t & 63 | 128
      );
    } else if (t < 65536) {
      if ((e -= 3) < 0)
        break;
      s.push(
        t >> 12 | 224,
        t >> 6 & 63 | 128,
        t & 63 | 128
      );
    } else if (t < 1114112) {
      if ((e -= 4) < 0)
        break;
      s.push(
        t >> 18 | 240,
        t >> 12 & 63 | 128,
        t >> 6 & 63 | 128,
        t & 63 | 128
      );
    } else
      throw new Error("Invalid code point");
  }
  return s;
}
function Kr(r) {
  for (var e = [], t = 0; t < r.length; ++t)
    e.push(r.charCodeAt(t) & 255);
  return e;
}
function Xr(r, e) {
  for (var t, n, i, s = [], o = 0; o < r.length && !((e -= 2) < 0); ++o)
    t = r.charCodeAt(o), n = t >> 8, i = t % 256, s.push(i), s.push(n);
  return s;
}
function xt(r) {
  return kr(Vr(r));
}
function ae(r, e, t, n) {
  for (var i = 0; i < n && !(i + t >= e.length || i >= r.length); ++i)
    e[i + t] = r[i];
  return i;
}
function Qr(r) {
  return r !== r;
}
function Zr(r) {
  return r != null && (!!r._isBuffer || At(r) || en(r));
}
function At(r) {
  return !!r.constructor && typeof r.constructor.isBuffer == "function" && r.constructor.isBuffer(r);
}
function en(r) {
  return typeof r.readFloatLE == "function" && typeof r.slice == "function" && At(r.slice(0, 0));
}
function g(r, e, t, n, i) {
  Error.call(this), Error.captureStackTrace ? Error.captureStackTrace(this, this.constructor) : this.stack = new Error().stack, this.message = r, this.name = "AxiosError", e && (this.code = e), t && (this.config = t), n && (this.request = n), i && (this.response = i);
}
l.inherits(g, Error, {
  toJSON: function() {
    return {
      // Standard
      message: this.message,
      name: this.name,
      // Microsoft
      description: this.description,
      number: this.number,
      // Mozilla
      fileName: this.fileName,
      lineNumber: this.lineNumber,
      columnNumber: this.columnNumber,
      stack: this.stack,
      // Axios
      config: l.toJSONObject(this.config),
      code: this.code,
      status: this.response && this.response.status ? this.response.status : null
    };
  }
});
const St = g.prototype, Rt = {};
[
  "ERR_BAD_OPTION_VALUE",
  "ERR_BAD_OPTION",
  "ECONNABORTED",
  "ETIMEDOUT",
  "ERR_NETWORK",
  "ERR_FR_TOO_MANY_REDIRECTS",
  "ERR_DEPRECATED",
  "ERR_BAD_RESPONSE",
  "ERR_BAD_REQUEST",
  "ERR_CANCELED",
  "ERR_NOT_SUPPORT",
  "ERR_INVALID_URL"
  // eslint-disable-next-line func-names
].forEach((r) => {
  Rt[r] = { value: r };
});
Object.defineProperties(g, Rt);
Object.defineProperty(St, "isAxiosError", { value: !0 });
g.from = (r, e, t, n, i, s) => {
  const o = Object.create(St);
  return l.toFlatObject(r, o, function(c) {
    return c !== Error.prototype;
  }, (a) => a !== "isAxiosError"), g.call(o, r.message, e, t, n, i), o.cause = r, o.name = r.name, s && Object.assign(o, s), o;
};
const tn = null;
function Ae(r) {
  return l.isPlainObject(r) || l.isArray(r);
}
function It(r) {
  return l.endsWith(r, "[]") ? r.slice(0, -2) : r;
}
function je(r, e, t) {
  return r ? r.concat(e).map(function(i, s) {
    return i = It(i), !t && s ? "[" + i + "]" : i;
  }).join(t ? "." : "") : e;
}
function rn(r) {
  return l.isArray(r) && !r.some(Ae);
}
const nn = l.toFlatObject(l, {}, null, function(e) {
  return /^is[A-Z]/.test(e);
});
function ce(r, e, t) {
  if (!l.isObject(r))
    throw new TypeError("target must be an object");
  e = e || new FormData(), t = l.toFlatObject(t, {
    metaTokens: !0,
    dots: !1,
    indexes: !1
  }, !1, function(w, P) {
    return !l.isUndefined(P[w]);
  });
  const n = t.metaTokens, i = t.visitor || h, s = t.dots, o = t.indexes, c = (t.Blob || typeof Blob != "undefined" && Blob) && l.isSpecCompliantForm(e);
  if (!l.isFunction(i))
    throw new TypeError("visitor must be a function");
  function u(p) {
    if (p === null)
      return "";
    if (l.isDate(p))
      return p.toISOString();
    if (!c && l.isBlob(p))
      throw new g("Blob is not supported. Use a Buffer instead.");
    return l.isArrayBuffer(p) || l.isTypedArray(p) ? c && typeof Blob == "function" ? new Blob([p]) : f.from(p) : p;
  }
  function h(p, w, P) {
    let _ = p;
    if (p && !P && typeof p == "object") {
      if (l.endsWith(w, "{}"))
        w = n ? w : w.slice(0, -2), p = JSON.stringify(p);
      else if (l.isArray(p) && rn(p) || (l.isFileList(p) || l.endsWith(w, "[]")) && (_ = l.toArray(p)))
        return w = It(w), _.forEach(function(V, Nt) {
          !(l.isUndefined(V) || V === null) && e.append(
            // eslint-disable-next-line no-nested-ternary
            o === !0 ? je([w], Nt, s) : o === null ? w : w + "[]",
            u(V)
          );
        }), !1;
    }
    return Ae(p) ? !0 : (e.append(je(P, w, s), u(p)), !1);
  }
  const d = [], y = Object.assign(nn, {
    defaultVisitor: h,
    convertValue: u,
    isVisitable: Ae
  });
  function m(p, w) {
    if (!l.isUndefined(p)) {
      if (d.indexOf(p) !== -1)
        throw Error("Circular reference detected in " + w.join("."));
      d.push(p), l.forEach(p, function(_, L) {
        (!(l.isUndefined(_) || _ === null) && i.call(
          e,
          _,
          l.isString(L) ? L.trim() : L,
          w,
          y
        )) === !0 && m(_, w ? w.concat(L) : [L]);
      }), d.pop();
    }
  }
  if (!l.isObject(r))
    throw new TypeError("data must be an object");
  return m(r), e;
}
function He(r) {
  const e = {
    "!": "%21",
    "'": "%27",
    "(": "%28",
    ")": "%29",
    "~": "%7E",
    "%20": "+",
    "%00": "\0"
  };
  return encodeURIComponent(r).replace(/[!'()~]|%20|%00/g, function(n) {
    return e[n];
  });
}
function Fe(r, e) {
  this._pairs = [], r && ce(r, this, e);
}
const kt = Fe.prototype;
kt.append = function(e, t) {
  this._pairs.push([e, t]);
};
kt.toString = function(e) {
  const t = e ? function(n) {
    return e.call(this, n, He);
  } : He;
  return this._pairs.map(function(i) {
    return t(i[0]) + "=" + t(i[1]);
  }, "").join("&");
};
function sn(r) {
  return encodeURIComponent(r).replace(/%3A/gi, ":").replace(/%24/g, "$").replace(/%2C/gi, ",").replace(/%20/g, "+").replace(/%5B/gi, "[").replace(/%5D/gi, "]");
}
function _t(r, e, t) {
  if (!e)
    return r;
  const n = t && t.encode || sn, i = t && t.serialize;
  let s;
  if (i ? s = i(e, t) : s = l.isURLSearchParams(e) ? e.toString() : new Fe(e, t).toString(n), s) {
    const o = r.indexOf("#");
    o !== -1 && (r = r.slice(0, o)), r += (r.indexOf("?") === -1 ? "?" : "&") + s;
  }
  return r;
}
class on {
  constructor() {
    this.handlers = [];
  }
  /**
   * Add a new interceptor to the stack
   *
   * @param {Function} fulfilled The function to handle `then` for a `Promise`
   * @param {Function} rejected The function to handle `reject` for a `Promise`
   *
   * @return {Number} An ID used to remove interceptor later
   */
  use(e, t, n) {
    return this.handlers.push({
      fulfilled: e,
      rejected: t,
      synchronous: n ? n.synchronous : !1,
      runWhen: n ? n.runWhen : null
    }), this.handlers.length - 1;
  }
  /**
   * Remove an interceptor from the stack
   *
   * @param {Number} id The ID that was returned by `use`
   *
   * @returns {Boolean} `true` if the interceptor was removed, `false` otherwise
   */
  eject(e) {
    this.handlers[e] && (this.handlers[e] = null);
  }
  /**
   * Clear all interceptors from the stack
   *
   * @returns {void}
   */
  clear() {
    this.handlers && (this.handlers = []);
  }
  /**
   * Iterate over all the registered interceptors
   *
   * This method is particularly useful for skipping over any
   * interceptors that may have become `null` calling `eject`.
   *
   * @param {Function} fn The function to call for each interceptor
   *
   * @returns {void}
   */
  forEach(e) {
    l.forEach(this.handlers, function(n) {
      n !== null && e(n);
    });
  }
}
const Ye = on, bt = {
  silentJSONParsing: !0,
  forcedJSONParsing: !0,
  clarifyTimeoutError: !1
}, an = typeof URLSearchParams != "undefined" ? URLSearchParams : Fe, cn = typeof FormData != "undefined" ? FormData : null, un = typeof Blob != "undefined" ? Blob : null, ln = (() => {
  let r;
  return typeof navigator != "undefined" && ((r = navigator.product) === "ReactNative" || r === "NativeScript" || r === "NS") ? !1 : typeof window != "undefined" && typeof document != "undefined";
})(), fn = (() => typeof WorkerGlobalScope != "undefined" && // eslint-disable-next-line no-undef
self instanceof WorkerGlobalScope && typeof self.importScripts == "function")(), b = {
  isBrowser: !0,
  classes: {
    URLSearchParams: an,
    FormData: cn,
    Blob: un
  },
  isStandardBrowserEnv: ln,
  isStandardBrowserWebWorkerEnv: fn,
  protocols: ["http", "https", "file", "blob", "url", "data"]
};
function hn(r, e) {
  return ce(r, new b.classes.URLSearchParams(), Object.assign({
    visitor: function(t, n, i, s) {
      return b.isNode && l.isBuffer(t) ? (this.append(n, t.toString("base64")), !1) : s.defaultVisitor.apply(this, arguments);
    }
  }, e));
}
function dn(r) {
  return l.matchAll(/\w+|\[(\w*)]/g, r).map((e) => e[0] === "[]" ? "" : e[1] || e[0]);
}
function pn(r) {
  const e = {}, t = Object.keys(r);
  let n;
  const i = t.length;
  let s;
  for (n = 0; n < i; n++)
    s = t[n], e[s] = r[s];
  return e;
}
function Ft(r) {
  function e(t, n, i, s) {
    let o = t[s++];
    const a = Number.isFinite(+o), c = s >= t.length;
    return o = !o && l.isArray(i) ? i.length : o, c ? (l.hasOwnProp(i, o) ? i[o] = [i[o], n] : i[o] = n, !a) : ((!i[o] || !l.isObject(i[o])) && (i[o] = []), e(t, n, i[o], s) && l.isArray(i[o]) && (i[o] = pn(i[o])), !a);
  }
  if (l.isFormData(r) && l.isFunction(r.entries)) {
    const t = {};
    return l.forEachEntry(r, (n, i) => {
      e(dn(n), i, t, 0);
    }), t;
  }
  return null;
}
function mn(r, e, t) {
  if (l.isString(r))
    try {
      return (e || JSON.parse)(r), l.trim(r);
    } catch (n) {
      if (n.name !== "SyntaxError")
        throw n;
    }
  return (t || JSON.stringify)(r);
}
const Te = {
  transitional: bt,
  adapter: b.isNode ? "http" : "xhr",
  transformRequest: [function(e, t) {
    const n = t.getContentType() || "", i = n.indexOf("application/json") > -1, s = l.isObject(e);
    if (s && l.isHTMLForm(e) && (e = new FormData(e)), l.isFormData(e))
      return i && i ? JSON.stringify(Ft(e)) : e;
    if (l.isArrayBuffer(e) || l.isBuffer(e) || l.isStream(e) || l.isFile(e) || l.isBlob(e))
      return e;
    if (l.isArrayBufferView(e))
      return e.buffer;
    if (l.isURLSearchParams(e))
      return t.setContentType("application/x-www-form-urlencoded;charset=utf-8", !1), e.toString();
    let a;
    if (s) {
      if (n.indexOf("application/x-www-form-urlencoded") > -1)
        return hn(e, this.formSerializer).toString();
      if ((a = l.isFileList(e)) || n.indexOf("multipart/form-data") > -1) {
        const c = this.env && this.env.FormData;
        return ce(
          a ? { "files[]": e } : e,
          c && new c(),
          this.formSerializer
        );
      }
    }
    return s || i ? (t.setContentType("application/json", !1), mn(e)) : e;
  }],
  transformResponse: [function(e) {
    const t = this.transitional || Te.transitional, n = t && t.forcedJSONParsing, i = this.responseType === "json";
    if (e && l.isString(e) && (n && !this.responseType || i)) {
      const o = !(t && t.silentJSONParsing) && i;
      try {
        return JSON.parse(e);
      } catch (a) {
        if (o)
          throw a.name === "SyntaxError" ? g.from(a, g.ERR_BAD_RESPONSE, this, null, this.response) : a;
      }
    }
    return e;
  }],
  /**
   * A timeout in milliseconds to abort a request. If set to 0 (default) a
   * timeout is not created.
   */
  timeout: 0,
  xsrfCookieName: "XSRF-TOKEN",
  xsrfHeaderName: "X-XSRF-TOKEN",
  maxContentLength: -1,
  maxBodyLength: -1,
  env: {
    FormData: b.classes.FormData,
    Blob: b.classes.Blob
  },
  validateStatus: function(e) {
    return e >= 200 && e < 300;
  },
  headers: {
    common: {
      Accept: "application/json, text/plain, */*",
      "Content-Type": void 0
    }
  }
};
l.forEach(["delete", "get", "head", "post", "put", "patch"], (r) => {
  Te.headers[r] = {};
});
const Ce = Te, wn = l.toObjectSet([
  "age",
  "authorization",
  "content-length",
  "content-type",
  "etag",
  "expires",
  "from",
  "host",
  "if-modified-since",
  "if-unmodified-since",
  "last-modified",
  "location",
  "max-forwards",
  "proxy-authorization",
  "referer",
  "retry-after",
  "user-agent"
]), gn = (r) => {
  const e = {};
  let t, n, i;
  return r && r.split(`
`).forEach(function(o) {
    i = o.indexOf(":"), t = o.substring(0, i).trim().toLowerCase(), n = o.substring(i + 1).trim(), !(!t || e[t] && wn[t]) && (t === "set-cookie" ? e[t] ? e[t].push(n) : e[t] = [n] : e[t] = e[t] ? e[t] + ", " + n : n);
  }), e;
}, ze = Symbol("internals");
function H(r) {
  return r && String(r).trim().toLowerCase();
}
function W(r) {
  return r === !1 || r == null ? r : l.isArray(r) ? r.map(W) : String(r);
}
function yn(r) {
  const e = /* @__PURE__ */ Object.create(null), t = /([^\s,;=]+)\s*(?:=\s*([^,;]+))?/g;
  let n;
  for (; n = t.exec(r); )
    e[n[1]] = n[2];
  return e;
}
const En = (r) => /^[-_a-zA-Z0-9^`|~,!#$%&'*+.]+$/.test(r.trim());
function de(r, e, t, n, i) {
  if (l.isFunction(n))
    return n.call(this, e, t);
  if (i && (e = t), !!l.isString(e)) {
    if (l.isString(n))
      return e.indexOf(n) !== -1;
    if (l.isRegExp(n))
      return n.test(e);
  }
}
function xn(r) {
  return r.trim().toLowerCase().replace(/([a-z\d])(\w*)/g, (e, t, n) => t.toUpperCase() + n);
}
function An(r, e) {
  const t = l.toCamelCase(" " + e);
  ["get", "set", "has"].forEach((n) => {
    Object.defineProperty(r, n + t, {
      value: function(i, s, o) {
        return this[n].call(this, e, i, s, o);
      },
      configurable: !0
    });
  });
}
class ue {
  constructor(e) {
    e && this.set(e);
  }
  set(e, t, n) {
    const i = this;
    function s(a, c, u) {
      const h = H(c);
      if (!h)
        throw new Error("header name must be a non-empty string");
      const d = l.findKey(i, h);
      (!d || i[d] === void 0 || u === !0 || u === void 0 && i[d] !== !1) && (i[d || c] = W(a));
    }
    const o = (a, c) => l.forEach(a, (u, h) => s(u, h, c));
    return l.isPlainObject(e) || e instanceof this.constructor ? o(e, t) : l.isString(e) && (e = e.trim()) && !En(e) ? o(gn(e), t) : e != null && s(t, e, n), this;
  }
  get(e, t) {
    if (e = H(e), e) {
      const n = l.findKey(this, e);
      if (n) {
        const i = this[n];
        if (!t)
          return i;
        if (t === !0)
          return yn(i);
        if (l.isFunction(t))
          return t.call(this, i, n);
        if (l.isRegExp(t))
          return t.exec(i);
        throw new TypeError("parser must be boolean|regexp|function");
      }
    }
  }
  has(e, t) {
    if (e = H(e), e) {
      const n = l.findKey(this, e);
      return !!(n && this[n] !== void 0 && (!t || de(this, this[n], n, t)));
    }
    return !1;
  }
  delete(e, t) {
    const n = this;
    let i = !1;
    function s(o) {
      if (o = H(o), o) {
        const a = l.findKey(n, o);
        a && (!t || de(n, n[a], a, t)) && (delete n[a], i = !0);
      }
    }
    return l.isArray(e) ? e.forEach(s) : s(e), i;
  }
  clear(e) {
    const t = Object.keys(this);
    let n = t.length, i = !1;
    for (; n--; ) {
      const s = t[n];
      (!e || de(this, this[s], s, e, !0)) && (delete this[s], i = !0);
    }
    return i;
  }
  normalize(e) {
    const t = this, n = {};
    return l.forEach(this, (i, s) => {
      const o = l.findKey(n, s);
      if (o) {
        t[o] = W(i), delete t[s];
        return;
      }
      const a = e ? xn(s) : String(s).trim();
      a !== s && delete t[s], t[a] = W(i), n[a] = !0;
    }), this;
  }
  concat(...e) {
    return this.constructor.concat(this, ...e);
  }
  toJSON(e) {
    const t = /* @__PURE__ */ Object.create(null);
    return l.forEach(this, (n, i) => {
      n != null && n !== !1 && (t[i] = e && l.isArray(n) ? n.join(", ") : n);
    }), t;
  }
  [Symbol.iterator]() {
    return Object.entries(this.toJSON())[Symbol.iterator]();
  }
  toString() {
    return Object.entries(this.toJSON()).map(([e, t]) => e + ": " + t).join(`
`);
  }
  get [Symbol.toStringTag]() {
    return "AxiosHeaders";
  }
  static from(e) {
    return e instanceof this ? e : new this(e);
  }
  static concat(e, ...t) {
    const n = new this(e);
    return t.forEach((i) => n.set(i)), n;
  }
  static accessor(e) {
    const n = (this[ze] = this[ze] = {
      accessors: {}
    }).accessors, i = this.prototype;
    function s(o) {
      const a = H(o);
      n[a] || (An(i, o), n[a] = !0);
    }
    return l.isArray(e) ? e.forEach(s) : s(e), this;
  }
}
ue.accessor(["Content-Type", "Content-Length", "Accept", "Accept-Encoding", "User-Agent", "Authorization"]);
l.reduceDescriptors(ue.prototype, ({ value: r }, e) => {
  let t = e[0].toUpperCase() + e.slice(1);
  return {
    get: () => r,
    set(n) {
      this[t] = n;
    }
  };
});
l.freezeMethods(ue);
const U = ue;
function pe(r, e) {
  const t = this || Ce, n = e || t, i = U.from(n.headers);
  let s = n.data;
  return l.forEach(r, function(a) {
    s = a.call(t, s, i.normalize(), e ? e.status : void 0);
  }), i.normalize(), s;
}
function Tt(r) {
  return !!(r && r.__CANCEL__);
}
function G(r, e, t) {
  g.call(this, r == null ? "canceled" : r, g.ERR_CANCELED, e, t), this.name = "CanceledError";
}
l.inherits(G, g, {
  __CANCEL__: !0
});
function Sn(r, e, t) {
  const n = t.config.validateStatus;
  !t.status || !n || n(t.status) ? r(t) : e(new g(
    "Request failed with status code " + t.status,
    [g.ERR_BAD_REQUEST, g.ERR_BAD_RESPONSE][Math.floor(t.status / 100) - 4],
    t.config,
    t.request,
    t
  ));
}
const Rn = b.isStandardBrowserEnv ? (
  // Standard browser envs support document.cookie
  function() {
    return {
      write: function(t, n, i, s, o, a) {
        const c = [];
        c.push(t + "=" + encodeURIComponent(n)), l.isNumber(i) && c.push("expires=" + new Date(i).toGMTString()), l.isString(s) && c.push("path=" + s), l.isString(o) && c.push("domain=" + o), a === !0 && c.push("secure"), document.cookie = c.join("; ");
      },
      read: function(t) {
        const n = document.cookie.match(new RegExp("(^|;\\s*)(" + t + ")=([^;]*)"));
        return n ? decodeURIComponent(n[3]) : null;
      },
      remove: function(t) {
        this.write(t, "", Date.now() - 864e5);
      }
    };
  }()
) : (
  // Non standard browser env (web workers, react-native) lack needed support.
  function() {
    return {
      write: function() {
      },
      read: function() {
        return null;
      },
      remove: function() {
      }
    };
  }()
);
function In(r) {
  return /^([a-z][a-z\d+\-.]*:)?\/\//i.test(r);
}
function kn(r, e) {
  return e ? r.replace(/\/+$/, "") + "/" + e.replace(/^\/+/, "") : r;
}
function Ct(r, e) {
  return r && !In(e) ? kn(r, e) : e;
}
const _n = b.isStandardBrowserEnv ? (
  // Standard browser envs have full support of the APIs needed to test
  // whether the request URL is of the same origin as current location.
  function() {
    const e = /(msie|trident)/i.test(navigator.userAgent), t = document.createElement("a");
    let n;
    function i(s) {
      let o = s;
      return e && (t.setAttribute("href", o), o = t.href), t.setAttribute("href", o), {
        href: t.href,
        protocol: t.protocol ? t.protocol.replace(/:$/, "") : "",
        host: t.host,
        search: t.search ? t.search.replace(/^\?/, "") : "",
        hash: t.hash ? t.hash.replace(/^#/, "") : "",
        hostname: t.hostname,
        port: t.port,
        pathname: t.pathname.charAt(0) === "/" ? t.pathname : "/" + t.pathname
      };
    }
    return n = i(window.location.href), function(o) {
      const a = l.isString(o) ? i(o) : o;
      return a.protocol === n.protocol && a.host === n.host;
    };
  }()
) : (
  // Non standard browser envs (web workers, react-native) lack needed support.
  function() {
    return function() {
      return !0;
    };
  }()
);
function bn(r) {
  const e = /^([-+\w]{1,25})(:?\/\/|:)/.exec(r);
  return e && e[1] || "";
}
function Fn(r, e) {
  r = r || 10;
  const t = new Array(r), n = new Array(r);
  let i = 0, s = 0, o;
  return e = e !== void 0 ? e : 1e3, function(c) {
    const u = Date.now(), h = n[s];
    o || (o = u), t[i] = c, n[i] = u;
    let d = s, y = 0;
    for (; d !== i; )
      y += t[d++], d = d % r;
    if (i = (i + 1) % r, i === s && (s = (s + 1) % r), u - o < e)
      return;
    const m = h && u - h;
    return m ? Math.round(y * 1e3 / m) : void 0;
  };
}
function Ge(r, e) {
  let t = 0;
  const n = Fn(50, 250);
  return (i) => {
    const s = i.loaded, o = i.lengthComputable ? i.total : void 0, a = s - t, c = n(a), u = s <= o;
    t = s;
    const h = {
      loaded: s,
      total: o,
      progress: o ? s / o : void 0,
      bytes: a,
      rate: c || void 0,
      estimated: c && o && u ? (o - s) / c : void 0,
      event: i
    };
    h[e ? "download" : "upload"] = !0, r(h);
  };
}
const Tn = typeof XMLHttpRequest != "undefined", Cn = Tn && function(r) {
  return new Promise(function(t, n) {
    let i = r.data;
    const s = U.from(r.headers).normalize(), o = r.responseType;
    let a;
    function c() {
      r.cancelToken && r.cancelToken.unsubscribe(a), r.signal && r.signal.removeEventListener("abort", a);
    }
    l.isFormData(i) && (b.isStandardBrowserEnv || b.isStandardBrowserWebWorkerEnv ? s.setContentType(!1) : s.setContentType("multipart/form-data;", !1));
    let u = new XMLHttpRequest();
    if (r.auth) {
      const m = r.auth.username || "", p = r.auth.password ? unescape(encodeURIComponent(r.auth.password)) : "";
      s.set("Authorization", "Basic " + btoa(m + ":" + p));
    }
    const h = Ct(r.baseURL, r.url);
    u.open(r.method.toUpperCase(), _t(h, r.params, r.paramsSerializer), !0), u.timeout = r.timeout;
    function d() {
      if (!u)
        return;
      const m = U.from(
        "getAllResponseHeaders" in u && u.getAllResponseHeaders()
      ), w = {
        data: !o || o === "text" || o === "json" ? u.responseText : u.response,
        status: u.status,
        statusText: u.statusText,
        headers: m,
        config: r,
        request: u
      };
      Sn(function(_) {
        t(_), c();
      }, function(_) {
        n(_), c();
      }, w), u = null;
    }
    if ("onloadend" in u ? u.onloadend = d : u.onreadystatechange = function() {
      !u || u.readyState !== 4 || u.status === 0 && !(u.responseURL && u.responseURL.indexOf("file:") === 0) || setTimeout(d);
    }, u.onabort = function() {
      u && (n(new g("Request aborted", g.ECONNABORTED, r, u)), u = null);
    }, u.onerror = function() {
      n(new g("Network Error", g.ERR_NETWORK, r, u)), u = null;
    }, u.ontimeout = function() {
      let p = r.timeout ? "timeout of " + r.timeout + "ms exceeded" : "timeout exceeded";
      const w = r.transitional || bt;
      r.timeoutErrorMessage && (p = r.timeoutErrorMessage), n(new g(
        p,
        w.clarifyTimeoutError ? g.ETIMEDOUT : g.ECONNABORTED,
        r,
        u
      )), u = null;
    }, b.isStandardBrowserEnv) {
      const m = (r.withCredentials || _n(h)) && r.xsrfCookieName && Rn.read(r.xsrfCookieName);
      m && s.set(r.xsrfHeaderName, m);
    }
    i === void 0 && s.setContentType(null), "setRequestHeader" in u && l.forEach(s.toJSON(), function(p, w) {
      u.setRequestHeader(w, p);
    }), l.isUndefined(r.withCredentials) || (u.withCredentials = !!r.withCredentials), o && o !== "json" && (u.responseType = r.responseType), typeof r.onDownloadProgress == "function" && u.addEventListener("progress", Ge(r.onDownloadProgress, !0)), typeof r.onUploadProgress == "function" && u.upload && u.upload.addEventListener("progress", Ge(r.onUploadProgress)), (r.cancelToken || r.signal) && (a = (m) => {
      u && (n(!m || m.type ? new G(null, r, u) : m), u.abort(), u = null);
    }, r.cancelToken && r.cancelToken.subscribe(a), r.signal && (r.signal.aborted ? a() : r.signal.addEventListener("abort", a)));
    const y = bn(h);
    if (y && b.protocols.indexOf(y) === -1) {
      n(new g("Unsupported protocol " + y + ":", g.ERR_BAD_REQUEST, r));
      return;
    }
    u.send(i || null);
  });
}, K = {
  http: tn,
  xhr: Cn
};
l.forEach(K, (r, e) => {
  if (r) {
    try {
      Object.defineProperty(r, "name", { value: e });
    } catch (t) {
    }
    Object.defineProperty(r, "adapterName", { value: e });
  }
});
const Pt = {
  getAdapter: (r) => {
    r = l.isArray(r) ? r : [r];
    const { length: e } = r;
    let t, n;
    for (let i = 0; i < e && (t = r[i], !(n = l.isString(t) ? K[t.toLowerCase()] : t)); i++)
      ;
    if (!n)
      throw n === !1 ? new g(
        `Adapter ${t} is not supported by the environment`,
        "ERR_NOT_SUPPORT"
      ) : new Error(
        l.hasOwnProp(K, t) ? `Adapter '${t}' is not available in the build` : `Unknown adapter '${t}'`
      );
    if (!l.isFunction(n))
      throw new TypeError("adapter is not a function");
    return n;
  },
  adapters: K
};
function me(r) {
  if (r.cancelToken && r.cancelToken.throwIfRequested(), r.signal && r.signal.aborted)
    throw new G(null, r);
}
function Ve(r) {
  return me(r), r.headers = U.from(r.headers), r.data = pe.call(
    r,
    r.transformRequest
  ), ["post", "put", "patch"].indexOf(r.method) !== -1 && r.headers.setContentType("application/x-www-form-urlencoded", !1), Pt.getAdapter(r.adapter || Ce.adapter)(r).then(function(n) {
    return me(r), n.data = pe.call(
      r,
      r.transformResponse,
      n
    ), n.headers = U.from(n.headers), n;
  }, function(n) {
    return Tt(n) || (me(r), n && n.response && (n.response.data = pe.call(
      r,
      r.transformResponse,
      n.response
    ), n.response.headers = U.from(n.response.headers))), Promise.reject(n);
  });
}
const Je = (r) => r instanceof U ? r.toJSON() : r;
function q(r, e) {
  e = e || {};
  const t = {};
  function n(u, h, d) {
    return l.isPlainObject(u) && l.isPlainObject(h) ? l.merge.call({ caseless: d }, u, h) : l.isPlainObject(h) ? l.merge({}, h) : l.isArray(h) ? h.slice() : h;
  }
  function i(u, h, d) {
    if (l.isUndefined(h)) {
      if (!l.isUndefined(u))
        return n(void 0, u, d);
    } else
      return n(u, h, d);
  }
  function s(u, h) {
    if (!l.isUndefined(h))
      return n(void 0, h);
  }
  function o(u, h) {
    if (l.isUndefined(h)) {
      if (!l.isUndefined(u))
        return n(void 0, u);
    } else
      return n(void 0, h);
  }
  function a(u, h, d) {
    if (d in e)
      return n(u, h);
    if (d in r)
      return n(void 0, u);
  }
  const c = {
    url: s,
    method: s,
    data: s,
    baseURL: o,
    transformRequest: o,
    transformResponse: o,
    paramsSerializer: o,
    timeout: o,
    timeoutMessage: o,
    withCredentials: o,
    adapter: o,
    responseType: o,
    xsrfCookieName: o,
    xsrfHeaderName: o,
    onUploadProgress: o,
    onDownloadProgress: o,
    decompress: o,
    maxContentLength: o,
    maxBodyLength: o,
    beforeRedirect: o,
    transport: o,
    httpAgent: o,
    httpsAgent: o,
    cancelToken: o,
    socketPath: o,
    responseEncoding: o,
    validateStatus: a,
    headers: (u, h) => i(Je(u), Je(h), !0)
  };
  return l.forEach(Object.keys(Object.assign({}, r, e)), function(h) {
    const d = c[h] || i, y = d(r[h], e[h], h);
    l.isUndefined(y) && d !== a || (t[h] = y);
  }), t;
}
const Ot = "1.5.0", Pe = {};
["object", "boolean", "number", "function", "string", "symbol"].forEach((r, e) => {
  Pe[r] = function(n) {
    return typeof n === r || "a" + (e < 1 ? "n " : " ") + r;
  };
});
const We = {};
Pe.transitional = function(e, t, n) {
  function i(s, o) {
    return "[Axios v" + Ot + "] Transitional option '" + s + "'" + o + (n ? ". " + n : "");
  }
  return (s, o, a) => {
    if (e === !1)
      throw new g(
        i(o, " has been removed" + (t ? " in " + t : "")),
        g.ERR_DEPRECATED
      );
    return t && !We[o] && (We[o] = !0, console.warn(
      i(
        o,
        " has been deprecated since v" + t + " and will be removed in the near future"
      )
    )), e ? e(s, o, a) : !0;
  };
};
function Pn(r, e, t) {
  if (typeof r != "object")
    throw new g("options must be an object", g.ERR_BAD_OPTION_VALUE);
  const n = Object.keys(r);
  let i = n.length;
  for (; i-- > 0; ) {
    const s = n[i], o = e[s];
    if (o) {
      const a = r[s], c = a === void 0 || o(a, s, r);
      if (c !== !0)
        throw new g("option " + s + " must be " + c, g.ERR_BAD_OPTION_VALUE);
      continue;
    }
    if (t !== !0)
      throw new g("Unknown option " + s, g.ERR_BAD_OPTION);
  }
}
const Se = {
  assertOptions: Pn,
  validators: Pe
}, v = Se.validators;
class ee {
  constructor(e) {
    this.defaults = e, this.interceptors = {
      request: new Ye(),
      response: new Ye()
    };
  }
  /**
   * Dispatch a request
   *
   * @param {String|Object} configOrUrl The config specific for this request (merged with this.defaults)
   * @param {?Object} config
   *
   * @returns {Promise} The Promise to be fulfilled
   */
  request(e, t) {
    typeof e == "string" ? (t = t || {}, t.url = e) : t = e || {}, t = q(this.defaults, t);
    const { transitional: n, paramsSerializer: i, headers: s } = t;
    n !== void 0 && Se.assertOptions(n, {
      silentJSONParsing: v.transitional(v.boolean),
      forcedJSONParsing: v.transitional(v.boolean),
      clarifyTimeoutError: v.transitional(v.boolean)
    }, !1), i != null && (l.isFunction(i) ? t.paramsSerializer = {
      serialize: i
    } : Se.assertOptions(i, {
      encode: v.function,
      serialize: v.function
    }, !0)), t.method = (t.method || this.defaults.method || "get").toLowerCase();
    let o = s && l.merge(
      s.common,
      s[t.method]
    );
    s && l.forEach(
      ["delete", "get", "head", "post", "put", "patch", "common"],
      (p) => {
        delete s[p];
      }
    ), t.headers = U.concat(o, s);
    const a = [];
    let c = !0;
    this.interceptors.request.forEach(function(w) {
      typeof w.runWhen == "function" && w.runWhen(t) === !1 || (c = c && w.synchronous, a.unshift(w.fulfilled, w.rejected));
    });
    const u = [];
    this.interceptors.response.forEach(function(w) {
      u.push(w.fulfilled, w.rejected);
    });
    let h, d = 0, y;
    if (!c) {
      const p = [Ve.bind(this), void 0];
      for (p.unshift.apply(p, a), p.push.apply(p, u), y = p.length, h = Promise.resolve(t); d < y; )
        h = h.then(p[d++], p[d++]);
      return h;
    }
    y = a.length;
    let m = t;
    for (d = 0; d < y; ) {
      const p = a[d++], w = a[d++];
      try {
        m = p(m);
      } catch (P) {
        w.call(this, P);
        break;
      }
    }
    try {
      h = Ve.call(this, m);
    } catch (p) {
      return Promise.reject(p);
    }
    for (d = 0, y = u.length; d < y; )
      h = h.then(u[d++], u[d++]);
    return h;
  }
  getUri(e) {
    e = q(this.defaults, e);
    const t = Ct(e.baseURL, e.url);
    return _t(t, e.params, e.paramsSerializer);
  }
}
l.forEach(["delete", "get", "head", "options"], function(e) {
  ee.prototype[e] = function(t, n) {
    return this.request(q(n || {}, {
      method: e,
      url: t,
      data: (n || {}).data
    }));
  };
});
l.forEach(["post", "put", "patch"], function(e) {
  function t(n) {
    return function(s, o, a) {
      return this.request(q(a || {}, {
        method: e,
        headers: n ? {
          "Content-Type": "multipart/form-data"
        } : {},
        url: s,
        data: o
      }));
    };
  }
  ee.prototype[e] = t(), ee.prototype[e + "Form"] = t(!0);
});
const X = ee;
class Oe {
  constructor(e) {
    if (typeof e != "function")
      throw new TypeError("executor must be a function.");
    let t;
    this.promise = new Promise(function(s) {
      t = s;
    });
    const n = this;
    this.promise.then((i) => {
      if (!n._listeners)
        return;
      let s = n._listeners.length;
      for (; s-- > 0; )
        n._listeners[s](i);
      n._listeners = null;
    }), this.promise.then = (i) => {
      let s;
      const o = new Promise((a) => {
        n.subscribe(a), s = a;
      }).then(i);
      return o.cancel = function() {
        n.unsubscribe(s);
      }, o;
    }, e(function(s, o, a) {
      n.reason || (n.reason = new G(s, o, a), t(n.reason));
    });
  }
  /**
   * Throws a `CanceledError` if cancellation has been requested.
   */
  throwIfRequested() {
    if (this.reason)
      throw this.reason;
  }
  /**
   * Subscribe to the cancel signal
   */
  subscribe(e) {
    if (this.reason) {
      e(this.reason);
      return;
    }
    this._listeners ? this._listeners.push(e) : this._listeners = [e];
  }
  /**
   * Unsubscribe from the cancel signal
   */
  unsubscribe(e) {
    if (!this._listeners)
      return;
    const t = this._listeners.indexOf(e);
    t !== -1 && this._listeners.splice(t, 1);
  }
  /**
   * Returns an object that contains a new `CancelToken` and a function that, when called,
   * cancels the `CancelToken`.
   */
  static source() {
    let e;
    return {
      token: new Oe(function(i) {
        e = i;
      }),
      cancel: e
    };
  }
}
const On = Oe;
function Un(r) {
  return function(t) {
    return r.apply(null, t);
  };
}
function vn(r) {
  return l.isObject(r) && r.isAxiosError === !0;
}
const Re = {
  Continue: 100,
  SwitchingProtocols: 101,
  Processing: 102,
  EarlyHints: 103,
  Ok: 200,
  Created: 201,
  Accepted: 202,
  NonAuthoritativeInformation: 203,
  NoContent: 204,
  ResetContent: 205,
  PartialContent: 206,
  MultiStatus: 207,
  AlreadyReported: 208,
  ImUsed: 226,
  MultipleChoices: 300,
  MovedPermanently: 301,
  Found: 302,
  SeeOther: 303,
  NotModified: 304,
  UseProxy: 305,
  Unused: 306,
  TemporaryRedirect: 307,
  PermanentRedirect: 308,
  BadRequest: 400,
  Unauthorized: 401,
  PaymentRequired: 402,
  Forbidden: 403,
  NotFound: 404,
  MethodNotAllowed: 405,
  NotAcceptable: 406,
  ProxyAuthenticationRequired: 407,
  RequestTimeout: 408,
  Conflict: 409,
  Gone: 410,
  LengthRequired: 411,
  PreconditionFailed: 412,
  PayloadTooLarge: 413,
  UriTooLong: 414,
  UnsupportedMediaType: 415,
  RangeNotSatisfiable: 416,
  ExpectationFailed: 417,
  ImATeapot: 418,
  MisdirectedRequest: 421,
  UnprocessableEntity: 422,
  Locked: 423,
  FailedDependency: 424,
  TooEarly: 425,
  UpgradeRequired: 426,
  PreconditionRequired: 428,
  TooManyRequests: 429,
  RequestHeaderFieldsTooLarge: 431,
  UnavailableForLegalReasons: 451,
  InternalServerError: 500,
  NotImplemented: 501,
  BadGateway: 502,
  ServiceUnavailable: 503,
  GatewayTimeout: 504,
  HttpVersionNotSupported: 505,
  VariantAlsoNegotiates: 506,
  InsufficientStorage: 507,
  LoopDetected: 508,
  NotExtended: 510,
  NetworkAuthenticationRequired: 511
};
Object.entries(Re).forEach(([r, e]) => {
  Re[e] = r;
});
const Dn = Re;
function Ut(r) {
  const e = new X(r), t = Ze(X.prototype.request, e);
  return l.extend(t, X.prototype, e, { allOwnKeys: !0 }), l.extend(t, e, null, { allOwnKeys: !0 }), t.create = function(i) {
    return Ut(q(r, i));
  }, t;
}
const A = Ut(Ce);
A.Axios = X;
A.CanceledError = G;
A.CancelToken = On;
A.isCancel = Tt;
A.VERSION = Ot;
A.toFormData = ce;
A.AxiosError = g;
A.Cancel = A.CanceledError;
A.all = function(e) {
  return Promise.all(e);
};
A.spread = Un;
A.isAxiosError = vn;
A.mergeConfig = q;
A.AxiosHeaders = U;
A.formToJSON = (r) => Ft(l.isHTMLForm(r) ? new FormData(r) : r);
A.getAdapter = Pt.getAdapter;
A.HttpStatusCode = Dn;
A.default = A;
const Bn = A;
function Nn() {
  return Ke.defaults.adapter = (r) => {
    const { method: e, data: t, headers: n } = r, i = {};
    return Object.keys(n).forEach((s) => {
      n[s] && (i[s] = n[s]);
    }), new Promise((s, o) => {
      tt.request({
        method: e.toUpperCase(),
        url: Ln(r),
        header: i,
        data: t,
        dataType: "json",
        responseType: r.responseType,
        success: (a) => {
          console.log("request success ↓↓"), console.log(a), s(a.data);
        },
        fail: (a) => {
          o(a);
        }
      });
    });
  }, Ke;
}
const Ue = Bn.create({
  baseURL: "https://myi-sdk.gzmywl.cn"
});
Ue.interceptors.request.use((r) => {
  const { method: e, params: t, headers: n } = r;
  return e === "post" && !n["Content-Type"] && (n["Content-Type"] = "application/x-www-form-urlencoded"), e === "delete" && (n["Content-Type"] = "application/json;", Object.assign(r, {
    data: t,
    params: {}
  })), j(M({}, r), {
    headers: n
  });
});
Ue.interceptors.response.use((r) => {
  var e;
  return ((e = r.data) == null ? void 0 : e.code) === 401 || (r.status || r.statusCode) === 200 ? r.data : Object.keys(r.result).length > 0 ? Promise.resolve(r) : Promise.reject(r);
});
const Ke = Ue;
function Ln(r) {
  const { url: e, params: t, baseURL: n } = r;
  let i = "";
  t && (Object.keys(t).forEach((o) => {
    let a = encodeURIComponent(o), c = encodeURIComponent(t[o]);
    i += `${a}=${c}&`;
  }), i.endsWith("&") && (i = i.slice(0, -1)));
  let s = e || "";
  return !s.startsWith("http://") && !s.startsWith("https://") && (s = `${n}${e}`), i ? `${s}?${i}` : s;
}
class Mn {
  /**
   * md5加密
   * @param str 明文字符串
   * @returns 密文字符串
   */
  md5Encrypt(e) {
    return e;
  }
  /**
   * rsa加密
   * @param str 明文字符串
   * @returns 密文字符串
   */
  rsaEncrypt(e) {
    return e;
  }
  /**
   * 生成唯一字符串
   * @returns 唯一字符串
   */
  uniqid() {
    let e = Date.now(), t = Math.floor(Math.random() * 1e11);
    return (e + t).toString(32);
  }
}
class qn extends Mn {
  getSystemInfo() {
    return new Promise((e, t) => {
      tt.getSystemInfo({
        success: (n) => {
          switch (console.log("TODO::检查platform的值", n), n.platform) {
            case "Android":
            case "android":
            case "ANDROIDOS":
              n.platform = "android", n._os = 0;
              break;
            case "ios":
              n._os = 1;
              break;
            case "windows":
            case "mac":
              n._os = 3;
              break;
            default:
              n._os = 2;
              break;
          }
          e(n);
        },
        fail: (n) => t(n)
      });
    });
  }
  getLaunchOption() {
    const e = tt.getLaunchOptionsSync();
    return console.log(e), {
      query: e == null ? void 0 : e.query
    };
  }
}
const vt = ({ method: r, url: e, query: t = {}, data: n = {}, sdk: i, dm: s }) => {
  const o = {}, a = i.getUserInfo();
  o.Token = a.token;
  const c = i.getPkgConfig();
  let u;
  switch (s) {
    case "sdk":
      u = (c == null ? void 0 : c.domain.sdk) || "https://myi-sdk.gzmywl.cn";
      break;
    case "pay":
      u = (c == null ? void 0 : c.domain.pay) || "https://myi-pay.gzmywl.cn";
      break;
    case "log":
      u = (c == null ? void 0 : c.domain.report) || "https://myi-log.gzmywl.cn";
      break;
    case "media":
      u = (c == null ? void 0 : c.domain.media) || "https://myi-log.gzmywl.cn";
      break;
  }
  if (e = u + e, r === "GET") {
    const h = Object.assign(i.getGlobalParams(), t);
    return i.getRequest().get(e, { params: h, headers: o });
  } else
    return n = Object.assign(i.getGlobalParams(), n), i.getRequest().post(e, n, { headers: o });
}, ve = (r, e, t, n) => vt({ method: "GET", url: r, query: e, sdk: t, dm: n }), Dt = (r, e, t, n) => vt({ method: "POST", url: r, data: e, sdk: t, dm: n }), $n = (r, e, t) => ve(r, e, t, "sdk"), Bt = (r, e, t) => Dt(r, e, t, "sdk"), jn = (r, e, t) => Dt(r, e, t, "pay"), De = (r, e, t) => ve(r, e, t, "log"), Hn = (r, e, t) => ve(r, e, t, "media"), D = (r) => {
  if (r.code != 200) {
    const e = new Error();
    throw e.message = r.msg, e.name = "" + r.code, e;
  }
  return r.result;
}, Yn = (r, e) => E(void 0, null, function* () {
  const t = yield Bt("/users/mgLogin", r, e);
  return D(t);
}), zn = (r, e) => E(void 0, null, function* () {
  const t = yield jn("/orders/createMgOrder", r, e);
  return D(t);
}), Gn = (r, e) => E(void 0, null, function* () {
  const t = yield Bt("/packages/detail", r, e);
  return D(t);
}), Vn = (r, e) => E(void 0, null, function* () {
  const t = yield $n("/packages/getConf", r, e);
  return D(t);
}), Jn = (r, e) => E(void 0, null, function* () {
  r.act = "createRole", r.appv = "1.2";
  const t = yield De("/comapi", r, e);
  return D(t);
}), Wn = (r, e) => E(void 0, null, function* () {
  r.act = "enterGame", r.appv = "1.2";
  const t = yield De("/comapi", r, e);
  return D(t);
}), Kn = (r, e) => E(void 0, null, function* () {
  r.act = "roleUp", r.appv = "1.2";
  const t = yield De("/comapi", r, e);
  return D(t);
}), Xe = (r, e, t) => E(void 0, null, function* () {
  try {
    const n = {
      act: "mediaReport",
      appv: "1.2",
      media: t.getMediakey(),
      report: r,
      extension: JSON.stringify(e || {})
    }, i = yield Hn("/comapi", n, t);
    return D(i);
  } catch (n) {
    console.info(n);
  }
});
function B(r) {
  return r.then((e) => [null, e]).catch((e) => [e, null]);
}
const we = "1.0.7", ge = "cqisdk_device", Qe = "cqisdk_user";
class Xn {
  constructor(e) {
    // Adapter
    x(this, "storageAdapter");
    x(this, "requestAdapter");
    x(this, "systemAdapter");
    // 保存console对象，以防研发修改该对象无法输出
    x(this, "csl");
    // 游戏ID，发行配置获取
    x(this, "gameId", 0);
    // 包名，发行配置获取
    x(this, "pkgbnd", "");
    // 包ID，发行配置获取
    x(this, "pkgid", 0);
    // 广告位ID，启动参数获取
    x(this, "adid", "");
    // 场景值，启动参数获取
    x(this, "scene", 0);
    // 头条端广告位ID，启动参数获取
    x(this, "promotionId", "");
    // 媒体标识
    x(this, "mediakey", "");
    // 媒体类型（判断是否自投）
    x(this, "mdaces", "");
    //判断是否引力（使用引力会带上）
    x(this, "turbo_promoted_object_id", "");
    //初始化引力，带值gravity
    x(this, "from_platform", "");
    // 启动参数
    x(this, "query", {
      // 邀请人
      invite: ""
    });
    // 设备信息
    x(this, "deviceInfo", {
      _is: !1,
      // 是否已激活
      isActived: !1,
      // 设备ID，随机生成
      deviceId: "",
      // 操作系统及版本
      system: "",
      // 设备品牌
      brand: "",
      // 设备型号
      model: "",
      // 运行系统，0-Android 1-IOS 2-未知(交给后端判断) 3-其他(PC/...)
      os: 2,
      // 运行平台类型，android | ios | devtool
      platform: "",
      // 头条下发的 clue_token
      clue_token: "",
      // sdk版本
      sdk_version: we,
      // 媒体标识
      mediakey: "",
      query: {}
    });
    // 用户信息
    x(this, "userInfo", {
      uid: "",
      username: "",
      token: "",
      sign: "",
      // 用户 openid
      wechat: "",
      // 用户 session_key
      wechat_sk: "",
      //注册时间
      regtime: 0
    });
    // 包参数配置
    x(this, "pkgConfig");
    // 是否初始化完成
    x(this, "isInitCompleted", !1);
    // 是否已登录
    x(this, "isLogined", !1);
    // 广告对象实例
    x(this, "rewardedVideoAd");
    // 引力SDK
    x(this, "geSdk");
    // 广点通SDK
    x(this, "gdtSdk");
    this.gameId = e.gameid, this.pkgid = e.pkgid, this.pkgbnd = e.pkgbnd, this.csl = Object.assign({}, console);
  }
  /**
   * 统一返回SDK错误对象
   * @param code 错误码
   * @param msg 错误信息
   * @returns
   */
  err(e, t) {
    return console.error(`(${e}) ${t}`), {
      errcode: e,
      errmsg: t
    };
  }
  warn(e, t) {
    return console.warn(`(${e}) ${t}`), {
      errcode: e,
      errmsg: t
    };
  }
  /**
   * 统一SDK日志输出打印
   * @param msg 输出信息
   * @param data 输出数据
   * @param level 输出级别
   */
  log(e, t = "", n = "info") {
    e = `[SDK:${n}] ${e}`, t = t != null ? t : "", this.csl.log(e, t);
  }
  /**
   * 检查点日志输出
   * @param point 检查点，参考文档
   * @param data 接口参数
   */
  logCheck(e, t = "") {
    let n = "";
    [1, 2, 3, 4, 7].includes(e) ? n += "(must) " : [8].includes(e) && (n += "(recommend) "), n += `2.${e} Connected.`, this.log(n, t, "check");
  }
  setStorage(e) {
    this.storageAdapter = e;
  }
  setRequest(e) {
    this.requestAdapter = e;
  }
  setSystem(e) {
    this.systemAdapter = e;
  }
  getRequest() {
    return this.requestAdapter;
  }
  /**
   * 创角上报接口
   * @param param
   * @returns
   */
  roleCreate(e) {
    return this.logCheck(3, e), this.checkLogined(), new Promise((t, n) => E(this, null, function* () {
      var s, o;
      const i = {
        uid: this.userInfo.uid,
        roleid: e.roleId,
        rolename: e.roleName,
        sid: e.serverId,
        servername: e.serverName,
        gameid: this.gameId,
        pkgbnd: this.pkgbnd,
        adid: this.adid,
        pkgid: this.pkgid,
        devid: this.deviceInfo.deviceId,
        from_platform: this.from_platform
      };
      yield Jn(i, this), t({}), (s = this.geSdk) == null || s.track(
        "roleCreate",
        //追踪事件的名称
        e
      ), (o = this.gdtSdk) == null || o.onCreateRole("SuperMan"), Xe("gdtRoleCreate", { report_os: "wxMiniGame", action: "gdtRoleCreate" }, this);
    }));
  }
  /**
   * 进入游戏上报接口
   * @param param
   * @returns
   */
  enterGame(e) {
    return this.logCheck(4, e), this.checkLogined(), new Promise((t, n) => E(this, null, function* () {
      var s;
      const i = {
        uid: this.userInfo.uid,
        roleid: e.roleId,
        rolename: e.roleName,
        sid: e.serverId,
        servername: e.serverName,
        gameid: this.gameId,
        pkgbnd: this.pkgbnd,
        adid: this.adid,
        pkgid: this.pkgid,
        promotion_id: this.promotionId,
        devid: this.deviceInfo.deviceId,
        from_platform: this.from_platform
      };
      yield Wn(i, this), t({}), (s = this.geSdk) == null || s.track(
        "enterGame",
        //追踪事件的名称
        e
      );
    }));
  }
  /**
   * 角色升级上报接口
   * @param param
   * @returns
   */
  roleUp(e) {
    return this.logCheck(5, e), this.checkLogined(), new Promise((t, n) => E(this, null, function* () {
      var s, o;
      const i = {
        uid: this.userInfo.uid,
        roleid: e.roleId,
        rolename: e.roleName,
        sid: e.serverId,
        servername: e.serverName,
        rolelevel: e.roleLevel || 0,
        gameid: this.gameId,
        pkgbnd: this.pkgbnd,
        adid: this.adid,
        pkgid: this.pkgid,
        promotion_id: this.promotionId,
        devid: this.deviceInfo.deviceId,
        from_platform: this.from_platform
      };
      yield Kn(i, this), t({}), (s = this.geSdk) == null || s.track(
        "roleUp",
        //追踪事件的名称
        e
      ), (o = this.gdtSdk) == null || o.track("UPDATE_LEVEL", {
        level: e.roleLevel || 0
      }), Xe("gdtRoleUp", { report_os: "wxMiniGame", action: "gdtRoleUp" }, this);
    }));
  }
  /**
   * 退出游戏上报接口
   * @param param
   * @returns
   */
  exitGame(e) {
    return this.logCheck(6, e), this.checkLogined(), new Promise((t, n) => E(this, null, function* () {
      var i;
      t({}), (i = this.geSdk) == null || i.track(
        "exitGame",
        //追踪事件的名称
        e
      );
    }));
  }
  /**
   * SDK 初始化
   */
  initialize() {
    return E(this, null, function* () {
      var i;
      this.log(`SDK_VERSION: ${we}`), yield this.getDeviceInfo();
      const e = {
        // 激活不在此接口激活，只拿包的配置参数，写死1
        actived: 1,
        gameid: this.gameId,
        pkgbnd: this.pkgbnd,
        pkgid: this.pkgid,
        from_platform: this.from_platform
      }, [t, n] = yield B(Gn(e, this));
      return t ? (this.err(4e3, `Initialize Error: ${t.message}`), !1) : (this.pkgConfig = n, ((i = this.pkgConfig) == null ? void 0 : i.watch_ad.status) === 1 && this._initAd(), this.isInitCompleted = !0, !0);
    });
  }
  /**
   * 检查SDK是否初始化完成
   * @throws
   */
  checkInit() {
    if (!this.isInitCompleted)
      throw console.error("SDK 未初始化"), "SDK 未初始化";
  }
  /**
   * 检查用户是否登录
   */
  checkLogined() {
    if (!this.isLogined)
      throw console.error("SDK 未登录"), "SDK 未登录";
  }
  /**
   * 获取公共请求参数
   * @returns
   */
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
      sdkver: we
    };
  }
  getPkgConfig() {
    return this.pkgConfig;
  }
  /**
   * 获取query启动参数
   * @returns
   */
  getQuery(e) {
    return E(this, null, function* () {
      if (Object.keys(e).length !== 0)
        return e;
      const t = yield this.storageAdapter.getStorage({ key: ge }), n = t && t.data ? t.data : {};
      return Object.keys(n).length > 0 && n.sdk_version === this.deviceInfo.sdk_version ? n.query || {} : {};
    });
  }
  /**
   * 获取设备信息
   * @returns
   */
  getDeviceInfo() {
    return E(this, null, function* () {
      if (this.deviceInfo && this.deviceInfo._is)
        return this.deviceInfo;
      console.log("getDeviceInfo~");
      const e = yield this.storageAdapter.getStorage({ key: ge }), t = e && e.data ? e.data : {};
      if (Object.keys(t).length > 0 && t.sdk_version === this.deviceInfo.sdk_version)
        this.deviceInfo = t;
      else {
        const n = yield this.systemAdapter.getSystemInfo();
        console.log("getSystemInfo返回", n), this.deviceInfo.system = n.system || "", this.deviceInfo.brand = n.brand || "", this.deviceInfo.model = n.model || "", this.deviceInfo.os = n._os, this.deviceInfo.platform = n.platform || "", this.deviceInfo.deviceId = n._devid || t.deviceId || "", this.deviceInfo.deviceId || (this.deviceInfo.deviceId = this.systemAdapter.uniqid()), this.deviceInfo.clue_token = t.clue_token || this.query.clue_token || "", this.deviceInfo.isActived = t.isActived || !1, this.deviceInfo.mediakey = t.mediakey || "";
      }
      return this.deviceInfo._is = !0, console.log("deviceInfo", this.deviceInfo), this.deviceInfo;
    });
  }
  setDeviceInfo(e) {
    return E(this, null, function* () {
      this.deviceInfo = e, this.deviceInfo.mediakey = this.mediakey, this.deviceInfo.query = this.query, yield this.storageAdapter.setStorage({ key: ge, data: this.deviceInfo });
    });
  }
  /**
   * 设置登录信息
   * 子类在login()后必须调用此方法设置登录信息
   * 否则后续接口无法调用
   * @param result 登录API返回结果
   */
  setLogined(e) {
    return E(this, null, function* () {
      this.userInfo = e, yield this.storageAdapter.setStorage({ key: Qe, data: this.userInfo }), this.isLogined = !0;
    });
  }
  /**
   * 读取本地缓存的登录信息
   */
  readLogined() {
    return E(this, null, function* () {
      if (!this.userInfo.token) {
        const e = yield this.storageAdapter.getStorage({ key: Qe }), t = e && e.data ? e.data : {};
        Object.keys(t).length > 0 && (this.userInfo = t);
      }
    });
  }
  getUserInfo() {
    return this.userInfo;
  }
  /**
   * 展示激励视频广告接口
   * @param param
   * @returns
   */
  showRewardedVideoAd(e) {
    return this.logCheck(12, e), this.checkLogined(), new Promise((t, n) => E(this, null, function* () {
      var s;
      if (((s = this.pkgConfig) == null ? void 0 : s.watch_ad.status) !== 1)
        return n(this.err(4023, "广告未配置"));
      const i = (o) => {
        var a, c;
        o && o.isEnded || o === void 0 ? t({ isEnded: 1 }) : t({ isEnded: 0 }), (c = this.geSdk) == null || c.adShowEvent("reward", (a = this.pkgConfig) == null ? void 0 : a.watch_ad.placementid, { custom_param: e }), this.rewardedVideoAd.offClose(i);
      };
      this.rewardedVideoAd.show().then(() => {
        this.rewardedVideoAd.onClose(i);
      }).catch((o) => {
        this.rewardedVideoAd.load().then(() => {
          this.rewardedVideoAd.show().then(() => {
            this.rewardedVideoAd.onClose(i);
          }).catch((a) => {
            console.log("激励视频 广告错误"), console.log(a), n(this.err(4021, "广告展示失败"));
          });
        }).catch((a) => {
          console.log("激励视频 广告错误"), console.log(a), n(this.err(4022, "广告加载失败"));
        });
      });
    }));
  }
  /**
   * 获取微信游戏圈数据（微信专有接口，在微信子类重写）
   * @param topic
   * @returns
   */
  wxGetGameClubData(e) {
    return console.warn("微信专有接口: wxGetGameClubData", e), new Promise(() => {
    });
  }
  /**
   * 调起客户端小游戏订阅消息界面（微信专有接口，在微信子类重写）
   * @param tmplIds
   * @returns
   */
  wxRequestSubscribeMessage(e) {
    return console.warn("微信专有接口: wxRequestSubscribeMessage", e), new Promise(() => {
    });
  }
  /**
   * 获取分享配置接口
   * @returns
   */
  getShareConf() {
    return new Promise((e, t) => E(this, null, function* () {
      const [n, i] = yield B(Vn({ module: "share" }, this));
      n && t(this.err(4041, `getShareConf err: ${n.message}`)), e({
        title: i.title,
        imgurl: i.img,
        imgid: i.id
      });
    }));
  }
  getMediakey() {
    return this.mediakey;
  }
}
class Qn extends Xn {
  constructor() {
    super(...arguments);
    x(this, "reloginNum", 0);
    //tt.login登录后用户信息
    x(this, "dyUserProfile");
  }
  init() {
    return this.logCheck(1), new Promise((t, n) => E(this, null, function* () {
      const { query: i = {}, scene: s = 0 } = this.systemAdapter.getLaunchOption();
      this.promotionId = i.promotion_id || "", this.adid = i.origin_adid || "", this.query = i, this.scene = s, yield this.initialize(), t({
        query: this.query
      });
    }));
  }
  _initAd() {
    var t;
    this.rewardedVideoAd = tt.createRewardedVideoAd({ adUnitId: (t = this.pkgConfig) == null ? void 0 : t.watch_ad.placementid }), this.rewardedVideoAd.onError((n) => {
      console.log("激励视频广告异常", n);
    });
  }
  qgLogin() {
    return new Promise((t, n) => E(this, null, function* () {
      tt.login({
        success: (i) => {
          i ? (this.dyUserProfile = i, t(i)) : n(this.err(4001, "tt.login Error"));
        },
        fail: (i) => {
          n(this.err(4001, `tt.login err: ${i}`));
        }
      });
    }));
  }
  showMessage(t, n = 0) {
    tt.showModal({
      title: "提示",
      content: t,
      showCancel: !1,
      success: () => {
        n && this.showMessage(t, --n);
      }
    });
  }
  login() {
    return this.logCheck(2), this.checkInit(), new Promise((t, n) => E(this, null, function* () {
      var d;
      let i = {};
      const [s, o] = yield B(this.qgLogin());
      if (s)
        return n(s);
      i = o;
      const a = yield this.getDeviceInfo(), c = {
        data: i,
        type: "dygame",
        mgType: "dygame",
        pkgid: this.pkgid,
        gameid: this.gameId,
        actived: a.isActived ? 1 : 0,
        pkgbnd: this.pkgbnd,
        adid: this.adid,
        scene: this.scene,
        devid: a.deviceId
      }, [u, h] = yield B(Yn(Object.assign(this.query, c), this));
      if (u) {
        if (u.name === "421") {
          if (this.reloginNum += 1, this.reloginNum >= 3)
            return n(this.err(4002, `Login Error: ${u.message}`));
          this.userInfo.token = "";
          const [y, m] = yield B(this.login());
          return y ? n(this.err(4002, `Login Error: ${u.message}`)) : t(m);
        }
        this.showMessage(u.name + u.message, 999);
        return;
      }
      a.deviceId = (d = h.devid) != null ? d : a.deviceId, a.isActived = !0, yield this.setDeviceInfo(a), yield this.setLogined(h), this.dyUserProfile.openid = this.userInfo.username, t({
        uid: this.userInfo.uid,
        username: this.userInfo.username,
        sign: this.userInfo.sign
      });
    }));
  }
  pay(t) {
    return this.logCheck(7, t), this.checkLogined(), new Promise((n, i) => E(this, null, function* () {
      const s = yield this.getDeviceInfo(), o = {
        data: this.dyUserProfile,
        mgType: "dygame",
        uid: this.userInfo.uid,
        money: t.amount,
        cburl: t.cburl,
        attach: t.attach,
        sid: t.serverId,
        servername: t.serverName,
        roleid: t.roleId,
        rolename: t.roleName,
        productid: t.productId,
        productdesc: t.productDesc,
        productname: t.productName,
        vip: t.vipLevel,
        istest: 0,
        gameid: this.gameId,
        pkgid: this.pkgid,
        pkgbnd: this.pkgbnd,
        adid: this.adid,
        promotion_id: this.promotionId,
        devid: s.deviceId,
        os: s.os,
        platform: s.platform
      }, [a, c] = yield B(zn(o, this));
      if (a)
        return i(this.err(4011, `Create Order Error: ${a}`));
      if (c.status === 0) {
        let u = c.money;
        this.pkgConfig && this.pkgConfig.channel.param6 && (u = u / this.pkgConfig.channel.param6), s.os === 0 ? (console.log("安卓"), tt.requestGamePayment({
          env: 0,
          // 支付环境
          mode: c.channel_data.mode,
          // 支付类型
          customId: c.channel_data.customId,
          //订单号
          buyQuantity: Number(c.channel_data.buyQuantity),
          // 购买数量，必须满足：金币数量*金币单价 = 限定价格等级（详见金币限定等级）
          currencyType: "CNY",
          zoneId: String(c.channel_data.zoneId),
          //服区id
          extraInfo: c.channel_data.extraInfo,
          platform: c.channel_data.platform,
          success: () => {
            console.log("支付成功，返回游戏"), n({});
          },
          fail: (h) => {
            console.log("支付失败"), i(this.err(4012, `tt.requestGamePayment拉起支付失败  ${JSON.stringify(h)}`));
          }
        })) : (console.log("ios"), tt.openAwemeCustomerService({
          currencyType: "DIAMOND",
          // 币种：目前仅为"DIAMOND"
          buyQuantity: Number(c.channel_data.buyQuantity),
          // 购买游戏币数量
          zoneId: String(c.channel_data.zoneId),
          customId: c.channel_data.customId,
          //开发者自定义唯一订单号。如不填，支付结果回调将不包含此字段，将导致游戏开发者无法发放游戏道具, 基础库版本低于1.55.0没有此字段
          extraInfo: c.channel_data.extraInfo,
          success() {
            console.log("支付成功，返回游戏"), n({});
          },
          fail(h) {
            console.log("支付失败"), i(this.err(4013, `tt.openAwemeCustomerService拉起支付失败  ${JSON.stringify(h)}`));
          }
        }));
      } else
        console.log("支付成功，返回游戏"), n({});
    }));
  }
  //创建桌面图标
  createShortcut() {
    return new Promise((t, n) => E(this, null, function* () {
      tt.addShortcut({
        success() {
          console.log("添加桌面成功"), t({});
        },
        fail(i) {
          n(this.err(4031, `创建桌面图标失败${i.errMsg}`));
        }
      });
    }));
  }
  /**
     * 主动拉起分享窗口
     * @param param 自定义参数
     * @param custom 自定义参数[true时：会自动补上缺失的字段：title、imageUrl]
     * 参考：https://developer.open-douyin.com/docs/resource/zh-CN/mini-game/develop/api/retweet/tt-share-app-message
  */
  share(t = {}, n = !1) {
    return this.logCheck(10), this.checkLogined(), new Promise((i, s) => E(this, null, function* () {
      if (!n) {
        const [o, a] = yield B(this.getShareConf());
        t = M({
          title: a.title,
          imageUrl: a.imgurl
        }, t);
      }
      tt.shareAppMessage(j(M({}, t), {
        success() {
          console.log("分享成功"), i({});
        },
        fail(o) {
          s(this.err(4042, `share err: ${o.message}`));
        }
      }));
    }));
  }
  // 拉起客服
  //参考：https://developer.open-douyin.com/docs/resource/zh-CN/mini-game/develop/api/open-capacity/customer-contact/openCustomerServiceConversation
  openCustomerService(t = 1) {
    return this.logCheck(11), this.checkInit(), new Promise((n, i) => {
      tt.openCustomerServiceConversation({
        type: t,
        success: () => {
          n({});
        },
        fail: (s) => {
          i(this.err(4051, `tt.openCustomerServiceConversation err: ${s.errMsg}`));
        }
      });
    });
  }
  //进入游戏的场景
  checkScene() {
    return new Promise((t, n) => {
      tt.onShow((i) => {
        let s = !1, o = !1;
        (i.scene == "021001" || i.scene == "021036" || i.scene == "101001") && (s = !0), i.scene == "021020" && (o = !0), t({
          isSidebar: s,
          isDeskEnter: o
        });
      });
    });
  }
  //跳转侧边栏
  navigateToScene(t = "sidebar") {
    return new Promise((n, i) => {
      tt.navigateToScene({
        scene: t,
        success: () => {
          n({});
        },
        fail: (s) => {
          i(this.err(4062, `tt.navigateToScene err: ${s.errMsg}`));
        }
      });
    });
  }
  //推荐流直出授权请求
  requestFeedSubscribe(t) {
    return this.logCheck(14), this.checkLogined(), new Promise((n, i) => {
      const s = {
        type: t.type,
        allScene: t.allScene || !1
      };
      t.allScene || (s.scene = t.scene), tt.checkFeedSubscribeStatus(j(M({}, s), {
        success: (o) => {
          if (!o.status) {
            i(this.err(4071, `tt.checkFeedSubscribeStatus err: ${o.errMsg}`));
            return;
          }
          const a = {
            type: t.type,
            allScene: t.allScene || !1
          };
          t.allScene || (s.scene = t.scene, s.contentIDs = t.contentIDs), tt.requestFeedSubscribe(j(M({}, a), {
            success(c) {
              if (!c.success) {
                i(this.err(4072, `tt.requestFeedSubscribe err: ${c.errMsg}`));
                return;
              }
              n({});
            },
            fail(c) {
              i(this.err(4072, `tt.requestFeedSubscribe err: ${c.errMsg}`));
            }
          }));
        },
        fail: (o) => {
          i(this.err(4071, `tt.checkFeedSubscribeStatus err: ${o.errMsg}`));
        }
      }));
    });
  }
  //关注游戏
  showFavoriteGuide(t) {
    return this.logCheck(15), this.checkLogined(), new Promise((n, i) => {
      tt.showFavoriteGuide({
        type: t.type,
        content: t.content || "一键添加到我的小程序",
        position: t.position || "bottom",
        success(s) {
          if (s.isFavorited) {
            i(this.err(4082, "tt.showFavoriteGuide err: 已收藏过"));
            return;
          }
          n({});
        },
        fail(s) {
          i(this.err(4081, `tt.showFavoriteGuide err: ${s.errMsg}`));
        }
      });
    });
  }
  //订阅消息
  subscribeMessage(t) {
    return this.logCheck(16), this.checkLogined(), new Promise((n, i) => {
      tt.requestSubscribeMessage({
        tmplIds: t,
        success(r) {
          n(r);
        },
        fail(s) {
          i(this.err(4091, `tt.requestSubscribeMessage err: ${s.errMsg}`));
        }
      });
    });
  }
  msgSecCheck() {
    throw new Error("Method not implemented.");
  }
}
const le = new Qn({ gameid: "12", pkgbnd: "com.dyxyx.yybs", pkgid: "275" });
le.setStorage(new Ht());
le.setRequest(Nn());
le.setSystem(new qn());
return le

}

window.cqisdk = cqi()