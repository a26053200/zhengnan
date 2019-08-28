@echo off

::set BUILD_TARGET = "-buildTarget windows64"
::#set LANGUAGE = "-language zh_CN"
set UNITY_LOG_PATH=%cd%\unity_log.txt
set UNITY_EDITOR_PATH="D:\Program Files\Unity2018.4.0f1\Editor\Unity.exe"
set PROJECT_PATH=D:\work\WorkSpace_Unity\mrpg_trunk
set METHOD_NAME=BM.BundlerManager.BuildAssetBundle
echo Lunch unity.exe,Please wait a moment...

echo %UNITY_EDITOR_PATH% -batchmode -quit -logFile %UNITY_LOG_PATH% -projectPath %PROJECT_PATH% -executeMethod %METHOD_NAME%
%UNITY_EDITOR_PATH% -batchmode -quit -logFile %UNITY_LOG_PATH% -projectPath %PROJECT_PATH% -executeMethod %METHOD_NAME%

echo Build Asset Bundle Done
pause