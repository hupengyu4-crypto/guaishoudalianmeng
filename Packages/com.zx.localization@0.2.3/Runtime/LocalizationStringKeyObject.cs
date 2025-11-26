using Engine.LayoutViewer;
using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LaunchSetpConfig", menuName = "ScriptableObject/LocalizationStringKeyObject", order = 3)]
[LayoutEditor]
public class LocalizationStringKeyObject : ScriptableObject
{
    #region Properties
    [SerializeField, ReorderableList, HideHeader(Key = "T")]
    private KeyValuePair[] list;

    public string this[string key]
    {
        get
        {
            for (int i = 0, length = list.Length; i < length; i++)
            {
                var item = list[i];
                if (item.Key == key)
                {
                    return item.Value;
                }
            }

            return string.Empty;
        }
    }

#if UNITY_EDITOR
    [Display]
    public TextAsset JsonText
    {
        get
        {
            return null;
        }
        set
        {
            if (value != null)
            {
                UnityEditor.Undo.RegisterCompleteObjectUndo(this, "UpdateData");
                UnityEditor.EditorUtility.SetDirty(this);

                List<KeyValuePair> list = new List<KeyValuePair>();
                var lines = value.text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                var len = lines.Length;
                for (int i = 0; i < len; i++)
                {
                    string line = lines[i];
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    switch (i)
                    {
                        case 0:
                        case 1:
                        case 2:
                            break;
                        default:
                            var items = line.Split('\t');
                            list.Add(new KeyValuePair() { Key = items[0], Value = items[1] });
                            break;
                    }
                }

                this.list = list.ToArray();
            }
        }
    }
#endif
    #endregion

    #region Public Methods
    public Dictionary<string, string> ToDictionary()
    {
        int length = list.Length;
        Dictionary<string, string> map = new Dictionary<string, string>(length);
        for (int i = 0; i < length; i++)
        {
            var item = list[i];
            map.Add(item.Key, item.Value);
        }

        return map;
    }
    #endregion

    #region Declarations
    [Serializable, HorizontalGroup("")]
    private class KeyValuePair
    {
        [Title("")]
        public string Key;
        [Title("")]
        public string Value;
    }
    #endregion
}
