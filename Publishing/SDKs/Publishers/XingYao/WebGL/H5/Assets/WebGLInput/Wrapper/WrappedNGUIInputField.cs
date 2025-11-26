#if H5_NGUI
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
using WebGLSupport.Detail;

namespace WebGLSupport
{
    /// <summary>
    /// Wrapper for NGUI
    /// </summary>
    class WrappedNGUIInputField : IInputField
    {
        UIInput input;
        RebuildChecker checker;

        public bool ReadOnly { get { return false; } }

        public string text
        {
            get { return input.text; }
            set { input.text = value; }
        }

        public string placeholder
        {
            get
            {
                if (!input.label) return "";
                var text = input.label.GetComponent<UILabel>();
                return text ? text.text : "";
            }
        }

        public int fontSize
        {
            get { return input.label.fontSize; }
        }

        public ContentType contentType
        {
            get { return (ContentType)input.inputType; }
        }

        public LineType lineType
        {
            get { return (LineType)input.onReturnKey; }
        }

        public int characterLimit
        {
            get { return input.characterLimit; }
        }

        public int caretPosition
        {
            get { return input.cursorPosition; }
        }

        public bool isFocused
        {
            get { return true; }
        }

        public int selectionFocusPosition
        {
            get { return input.cursorPosition; }
            set { input.cursorPosition = value; }
        }

        public int selectionAnchorPosition
        {
            get { return input.cursorPosition; }
            set { input.cursorPosition = value; }
        }

        public bool OnFocusSelectAll
        {
            get { return input.isSelected; }
        }

        public bool EnableMobileSupport
        {
            get
            {
                // return false to use unity mobile keyboard support
                return true;
            }
        }

        public WrappedNGUIInputField(UIInput input)
        {
            this.input = input;
            checker = new RebuildChecker(this);
        }

        public void ActivateInputField()
        {
            //input.ActivateInputField();
        }

        public void DeactivateInputField()
        {
            //input.DeactivateInputField();
        }

        public void Rebuild()
        {
            if (checker.NeedRebuild())
            {
                input.UpdateLabel();
  
            }
        }

        public Rect GetScreenCoordinates()
        {
            var worldCorners = new Vector3[4];
            var camera = NGUITools.FindCameraForLayer(input.gameObject.layer);
            for (var i = 0; i < worldCorners.Length; i++)
            {
                worldCorners[i] = camera.WorldToScreenPoint(input.label.worldCorners[i]);
            }
            var min = new Vector3(float.MaxValue, float.MaxValue);
            var max = new Vector3(float.MinValue, float.MinValue);
            for (var i = 0; i < worldCorners.Length; i++)
            {
                min.x = Mathf.Min(min.x, worldCorners[i].x);
                min.y = Mathf.Min(min.y, worldCorners[i].y);
                max.x = Mathf.Max(max.x, worldCorners[i].x);
                max.y = Mathf.Max(max.y, worldCorners[i].y);
            }
            return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
        }
    }
}

#endif