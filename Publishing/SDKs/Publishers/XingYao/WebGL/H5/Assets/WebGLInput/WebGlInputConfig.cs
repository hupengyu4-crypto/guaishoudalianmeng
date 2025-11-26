using UnityEngine;
using YoYo.Unity.SDK;

[CreateAssetMenu(fileName = "WebGlConfig", menuName = "ScriptableObject/WebGlInputConfig´´½¨", order = 0)]
public class WebGlInputConfig : WebGlConfig 
{
    public override  void Init()
    {
        WebGLSupport.WebGLInput.SetSdkAction();
    }

}