@echo off 

set ReleaseProject=%~dp0..\..\MirrorReleaseAllZSZZ
	
if exist %ReleaseProject% ( 
	rd /s /q %ReleaseProject% 
)


::创建目录
md %ReleaseProject%\Assets
md %ReleaseProject%\Assets\NGUI
md %ReleaseProject%\Packages

copy %~dp0\众神征战\堆栈\csc.rsp %ReleaseProject%\Assets\csc.rsp
::拷贝ProjectSettings
::xcopy %~dp0..\ProjectSettings\*.* %ReleaseProject%\ProjectSettings\ /e /y /q /s

::调用软连接资源bat
call %~dp0更新众神征战Release全堆栈工程.bat noPause

echo 新建工程完成:%ReleaseProject%

set str=%1
if "%str%"=="" (
    pause
)