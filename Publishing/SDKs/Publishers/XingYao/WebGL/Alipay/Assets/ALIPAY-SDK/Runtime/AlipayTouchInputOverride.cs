#if UNITY_WEBGL || UNITY_EDITOR
using System;
using System.Collections.Generic;
using AlipaySdk;
using LitJson;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Touch = UnityEngine.Touch;

internal class TouchData
{
    public Touch touch;
    public long timeStamp;
}

[RequireComponent(typeof(StandaloneInputModule))]
public class AlipayTouchInputOverride : BaseInput
{
    private readonly List<TouchData> _changedTouches = new List<TouchData>();
    private StandaloneInputModule _standaloneInputModule = null;

    #region InputMono
    protected override void Awake()
    {
        base.Awake();
        _standaloneInputModule = GetComponent<StandaloneInputModule>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        Debug.Log("Enable  AlipayTouchInputOverride");
        InitAlipayTouchEvents();
        if (_standaloneInputModule)
        {
            _standaloneInputModule.inputOverride = this;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UnregisterAlipayTouchEvents();
        if (_standaloneInputModule)
        {
            _standaloneInputModule.inputOverride = null;
        }
    }

    public void OnTouchStart(int identifier, Vector2 position, long timeStamp)
    {
        var data = FindOrCreateTouchData(identifier);
        data.touch.phase = TouchPhase.Began;
        data.touch.position = position;
        data.touch.rawPosition = data.touch.position;
        data.timeStamp = timeStamp;
    }

    public void OnTouchMove(int identifier, Vector2 position, long timeStamp)
    {
        var data = FindOrCreateTouchData(identifier);
        UpdateTouchData(data, position, timeStamp, TouchPhase.Moved);
    }

    public void OnTouchEnd(int identifier, Vector2 position, long timeStamp)
    {
        TouchData data = FindTouchData(identifier);
        if (data == null)
            return;

        UpdateTouchData(data, position, timeStamp, TouchPhase.Ended);
    }

    public void OnTouchCancel(int identifier, Vector2 position, long timeStamp)
    {
        TouchData data = FindTouchData(identifier);
        if (data == null)
            return;

        UpdateTouchData(data, position, timeStamp, TouchPhase.Canceled);
    }

    private void LateUpdate()
    {
        foreach (var t in _changedTouches)
        {
            if (t.touch.phase == TouchPhase.Began)
            {
                t.touch.phase = TouchPhase.Stationary;
            }
        }

        RemoveEndedchangedTouches();
    }

    private void RemoveEndedchangedTouches()
    {
        if (_changedTouches.Count > 0)
        {
            _changedTouches.RemoveAll(touchData =>
            {
                var touch = touchData.touch;
                return touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled;
            });
        }
    }

    #endregion

    private void InitAlipayTouchEvents()
    {
        AlipaySDK.API.OnTouchStart(OnAlipayTouchStart);
        AlipaySDK.API.OnTouchMove(OnAlipayTouchMove);
        AlipaySDK.API.OnTouchEnd(OnAlipayTouchEnd);
        AlipaySDK.API.OnTouchCancel(OnAlipayTouchCancel);
    }

    private void UnregisterAlipayTouchEvents()
    {
        AlipaySDK.API.OffTouchStart(OnAlipayTouchStart);
        AlipaySDK.API.OffTouchMove(OnAlipayTouchMove);
        AlipaySDK.API.OffTouchEnd(OnAlipayTouchEnd);
        AlipaySDK.API.OffTouchCancel(OnAlipayTouchCancel);
    }

    private void OnAlipayTouchStart(OnTouchListenerResult touchEvent)
    {
        foreach (var alipayTouch in touchEvent.changedTouches)
        {
            Vector2 pos = new Vector2(alipayTouch.clientX, alipayTouch.clientY);
            pos = FixTouchPos(pos);
            OnTouchStart(alipayTouch.identifier, pos, touchEvent.timeStamp);
        }
    }

    private void OnAlipayTouchMove(OnTouchListenerResult touchEvent)
    {
        foreach (var alipayTouch in touchEvent.changedTouches)
        {
            Vector2 pos = new Vector2(alipayTouch.clientX, alipayTouch.clientY);
            pos = FixTouchPos(pos);
            OnTouchMove(alipayTouch.identifier, pos, touchEvent.timeStamp);
        }
    }

    private void OnAlipayTouchEnd(OnTouchListenerResult touchEvent)
    {
        foreach (var alipayTouch in touchEvent.changedTouches)
        {
            Vector2 pos = new Vector2(alipayTouch.clientX, alipayTouch.clientY);
            pos = FixTouchPos(pos);
            OnTouchEnd(alipayTouch.identifier, pos, touchEvent.timeStamp);
        }
    }

    private void OnAlipayTouchCancel(OnTouchListenerResult touchEvent)
    {
        foreach (var alipayTouch in touchEvent.changedTouches)
        {
            Vector2 pos = new Vector2(alipayTouch.clientX, alipayTouch.clientY);
            pos = FixTouchPos(pos);
            OnTouchCancel(alipayTouch.identifier, pos, touchEvent.timeStamp);
        }
    }

    private TouchData FindTouchData(int identifier)
    {
        foreach (var touchData in _changedTouches)
        {
            var touch = touchData.touch;
            if (touch.fingerId == identifier)
            {
                return touchData;
            }
        }

        return null;
    }

    private TouchData FindOrCreateTouchData(int identifier)
    {
        var touchData = FindTouchData(identifier);
        if (touchData != null)
        {
            return touchData;
        }

        var data = new TouchData();
        data.touch.pressure = 1.0f;
        data.touch.maximumPossiblePressure = 1.0f;
        data.touch.type = TouchType.Direct;
        data.touch.tapCount = 1;
        data.touch.fingerId = identifier;
        data.touch.radius = 0;
        data.touch.radiusVariance = 0;
        data.touch.altitudeAngle = 0;
        data.touch.azimuthAngle = 0;
        data.touch.deltaTime = 0;
        _changedTouches.Add(data);
        return data;
    }

    private static void UpdateTouchData(TouchData data, Vector2 pos, long timeStamp, TouchPhase phase)
    {
        data.touch.phase = phase;
        data.touch.deltaPosition = pos - data.touch.position;
        data.touch.position = pos;
        data.touch.deltaTime = (timeStamp - data.timeStamp) / 1000000.0f;
    }

    private static Vector2 FixTouchPos(Vector2 pos)
    {
        JsonData systemInfo = JsonMapper.ToObject(AlipaySDK.API.GetSystemInfoSync());
        float curRatio = float.Parse(systemInfo["pixelRatio"].ToString());
        float height = float.Parse(systemInfo["windowHeight"].ToString());
        pos.x = Mathf.RoundToInt(pos.x * curRatio);
        pos.y = Mathf.RoundToInt(height * curRatio - pos.y * curRatio);
        return pos;
    }

#if !UNITY_EDITOR
    public override bool touchSupported
    {
        get
        {
            return true;
        }
    }
    public override bool mousePresent
    {
        get
        {
            return false;
        }
    }
    public override int touchCount
    {
        get { return _changedTouches.Count; }
    }

    public override Touch GetTouch(int index)
    {
        return _changedTouches[index].touch;
    }
#endif
}

#endif