@echo off 

set DevProject=%~dp0..\
set ReleaseProject=%~dp0..\..\MirrorReleaseAllZSZZ

::链接到工程资源
mklink /J %ReleaseProject%\ProjectSettings %DevProject%\ProjectSettings
mklink /J %ReleaseProject%\Packages\com.muyuan.devcore %DevProject%\Packages\com.muyuan.devcore
mklink /J %ReleaseProject%\Packages\com.muyuan.devtool %DevProject%\Packages\com.muyuan.devtool
mklink /J %ReleaseProject%\Packages\com.muyuan.minigame %DevProject%\Packages\com.muyuan.minigame
mklink /J %ReleaseProject%\Packages\com.muyuan.remotepackage %DevProject%\Packages\com.muyuan.remotepackage
mklink /J %ReleaseProject%\Packages\com.muyuan.xcore@0.1.303101416-preview %DevProject%\Packages\com.muyuan.xcore@0.1.303101416-preview
mklink /J %ReleaseProject%\Packages\com.muyuan.xunitycore@0.1.303101452-preview %DevProject%\Packages\com.muyuan.xunitycore@0.1.303101452-preview
mklink /H %ReleaseProject%\Packages\manifest.json %DevProject%\Packages\manifest.json
mklink /H %ReleaseProject%\Packages\packages-lock.json %DevProject%\Packages\packages-lock.json

mklink /J %ReleaseProject%\Assets\common %DevProject%\Assets\common
mklink /J %ReleaseProject%\Assets\Editor %DevProject%\Assets\Editor
mklink /J %ReleaseProject%\Assets\WebGLTemplates %DevProject%\Assets\WebGLTemplates
mklink /J %ReleaseProject%\Assets\iTween %DevProject%\Assets\iTween

mklink /J %ReleaseProject%\Assets\NGUI\Editor %DevProject%\Assets\NGUI\Editor
mklink /J %ReleaseProject%\Assets\NGUI\Resources %DevProject%\Assets\NGUI\Resources
mklink /J %ReleaseProject%\Assets\NGUI\Scripts %DevProject%\Assets\NGUI\Scripts

mklink /J %ReleaseProject%\Assets\own %DevProject%\Assets\own

mklink /J %ReleaseProject%\Assets\Pixelplacement %DevProject%\Assets\Pixelplacement
mklink /J %ReleaseProject%\Assets\plugins %DevProject%\Assets\plugins

::mklink /J %ReleaseProject%\Assets\StreamingAssets %DevProject%\Assets\StreamingAssets
mklink /J %ReleaseProject%\Assets\screen %DevProject%\Assets\screen

mklink /H %ReleaseProject%\Assets\RootAssembly.asmdef.meta %DevProject%\Assets\RootAssembly.asmdef.meta
mklink /H %ReleaseProject%\Assets\RootAssembly.asmdef %DevProject%\Assets\RootAssembly.asmdef

mklink /J %ReleaseProject%\Assets\Spine %DevProject%\Assets\Spine
mklink /J %ReleaseProject%\Assets\script %DevProject%\Assets\script
mklink /J %ReleaseProject%\Assets\RootScript %DevProject%\Assets\RootScript
mklink /J %ReleaseProject%\Assets\YoukiaSDK %DevProject%\Tools\众神征战\YoukiaSDK
mklink /J %ReleaseProject%\Assets\WX-WASM-SDK-V2 %DevProject%\Tools\众神征战\WX-WASM-SDK-V2
mklink /J %ReleaseProject%\Assets\XDevCoreEditor %DevProject%\Assets\XDevCoreEditor


echo "链接到工程资源完成"

set str=%1
if "%str%"=="" (
    pause
)