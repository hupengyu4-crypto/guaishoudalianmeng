@echo off 

set DevProject=%~dp0..\
set WebGLProject=C:\Users\admin\Desktop\Test
set CDN=E:\nginx-1.22.1\download

::链接到工程资源
mklink /J %WebGLProject%\webgl\StreamingAssets\StreamingResources %DevProject%\Assets\StreamingAssets\StreamingResources
mklink /J %CDN%\Assets\audio %DevProject%\Assets\Resources\audio
mklink /J %CDN%\Assets\Textures %WebGLProject%\webgl-min\Assets\Textures
::mklink /J %CDN%\Build %WebGLProject%\webgl-min\Build

echo "链接到工程资源完成"

set str=%1
if "%str%"=="" (
    pause
)