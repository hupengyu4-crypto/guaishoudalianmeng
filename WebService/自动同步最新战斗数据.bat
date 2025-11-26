xcopy ..\Assets\Resources\ConfigByteNew\Battle\System %~dp0ConfigByteNew\Battle\System /e /y
xcopy ..\Assets\Resources\ConfigByteNew\ConfigMap.bytes %~dp0ConfigByteNew /y

xcopy ..\Assets\plugins\ProtoBuf\Google.Protobuf.dll %~dp0bin\ /y
xcopy ..\Assets\plugins\ProtoBuf\System.Buffers.dll %~dp0bin /y
xcopy ..\Assets\plugins\ProtoBuf\System.Memory.dll %~dp0bin /y
xcopy ..\Assets\plugins\ProtoBuf\System.Runtime.CompilerServices.Unsafe.dll %~dp0bin /y

xcopy ..\Assets\script\Modules\BattleSystem %~dp0Scripts\BattleSystem /e /y

xcopy ..\Assets\script\Modules\ProtoBuf %~dp0Scripts\ProtoBuf /e /y

xcopy ..\Assets\script\ProtoBuff\PubFighter.cs %~dp0Scripts\ProtoBuff /e /y
xcopy ..\Assets\script\ProtoBuff\PubProp.cs %~dp0Scripts\ProtoBuff /e /y
xcopy ..\Assets\script\ProtoBuff\PubProto.cs %~dp0Scripts\ProtoBuff /e /y
xcopy ..\Assets\script\ConfigHashCodeDefine.cs %~dp0Scripts\Config /s /y

rd /s /q %~dp0Scripts\BattleSystem\Editor

del /F /Q /A /S %~dp0*.meta

pause