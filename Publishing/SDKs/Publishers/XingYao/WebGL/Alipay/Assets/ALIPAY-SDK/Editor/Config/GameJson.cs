using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameJson
{
    public string screenOrientation;
    public Plugins plugins;
    public Subpackage[] subpackages;
}

[Serializable]
public class Plugins
{
    public UnityLoader UnityLoader;
}

[Serializable]
public class UnityLoader
{
    public string unityVersion;
    public string exporterVersion;
    public string frameworkModulePath;
    public string frameworkWasmBinaryPath;
    public string wasmBinaryMd5;
}

[Serializable]
public class Subpackage
{
    public string name;
    public string root;
}
