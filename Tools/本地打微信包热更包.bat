@echo off 


:: Unity本地路径
set UnityPath="D:\Program Files\Unity 2021.3.23f1\Editor\Unity.exe"
set CDNRoot="http://192.168.10.69:8099/"
set LOCAL_IIS="D:\zhongshengIIS"
set LastResVersion=0.0.1
set ResVersion=0.0.2

set ReleaseProject=%~dp0..\..\MirrorRelease
set DevProjectPath=%~dp0..\
set ProjectPath=%~dp0..\..\
set GameVersion=4.0.2
set IsClearAB=false
set OutDir=OutWeixin
set ABGuid=1f7d66e16d83ab74da779c4fc04e8f95
set HotFixGuid=a1792c146d6ddc84293ac438dd48f791
set BuildType=1
set IsCloseGuide=false
set IsDevelopmentBuild=false
set AppId=wxa48e8546732b5089
set OutDirGameVersion=%ProjectPath%%OutDir%\%GameVersion%

:: goto copyAsset

::判断Unity是否运行中
TASKLIST /V /S localhost /U %username%>tmp_process_list.txt
TYPE tmp_process_list.txt |FIND "Unity.exe"

IF ERRORLEVEL 0 (GOTO UNITY_IS_RUNNING)
ELSE (GOTO START_UNITY)


:: 新建/更新工程
call %~dp0新建Release工程.bat noPause


:: 关闭Unity工程
:UNITY_IS_RUNNING
::杀掉Unity
TASKKILL /F /IM Unity.exe
::停1秒
PING 127.0.0.1 -n 1 >NUL

echo ----------------------------------------------------
echo DevProjectPath %DevProjectPath%
echo ReleaseProject %ReleaseProject%
echo ----------------------------------------------------

:: goto endDo

:: 热更代码包处理

echo Begin Build HotFix Update Log:%ProjectPath%\HotFix_Update.log

call %UnityPath% -quit ^
-batchmode ^
-projectPath "%ReleaseProject%" ^
-executeMethod TaskManagement.TaskEditorWindow.JenkinsStart ^
-logFile "%ProjectPath%\HotFixAll.log" ^
guid=%HotFixGuid% ^


:: AB包打包

echo Begin Build AB Log:%ProjectPath%\AB_Update_Output.log

call %UnityPath% -quit ^
-batchmode ^
-projectPath %DevProjectPath% ^
-executeMethod TaskManagement.TaskEditorWindow.JenkinsStart ^
-logFile "%ProjectPath%\AB_Update_Output.log" ^
guid=%ABGuid% ^
projectPath=%ProjectPath% ^
gameVersion=%GameVersion% ^
resVersion=%ResVersion% ^
isClearAB=%IsClearAB% ^
outDir=%OutDir% ^
lastResVersion=%LastResVersion% ^

::停1秒
PING 127.0.0.1 -n 1 >NUL

:: goto endDo

::停1秒
PING 127.0.0.1 -n 1 >NUL

:copyAsset
echo "Begin Copy AB File"
:: 拷ab文件

set binPath=%ProjectPath%%OutDir%\%GameVersion%\Out\webgl
echo %binPath%
for /r %binPath% %%a in (*.bin.txt) do copy %%a %LOCAL_IIS%
xcopy %OutDirGameVersion%\%ResVersion%\%ResVersion%\ %LOCAL_IIS%\%ResVersion% /s/e/i/y/q
xcopy %OutDirGameVersion%\%ResVersion%\StreamingAssets\ %LOCAL_IIS%\StreamingAssets /s/e/i/y/q
xcopy %OutDirGameVersion%\%ResVersion%\Assets\ %LOCAL_IIS%\Assets /s/e/i/y/q

:endDo

pause