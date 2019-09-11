@echo off

set PROJ_DIR="D:\work\WorkSpace_Unity\X-Bullet"
set PROJ_NAME="X-Bullet"
set GAME_CODE_DIR="Game"
set RES_DIR="Res"
set LUA_FRAMEWORK_DIR="Lua"
set LUA_DIR="Game"

echo Start update - %PROJ_DIR% ...
D:
cd %PROJ_DIR%
svn status
svn update
echo Update Complete! -- %PROJ_DIR%

cd Assets\%GAME_CODE_DIR%
svn status
svn update
echo Update Complete! -- %GAME_CODE_DIR%

cd ../
cd %RES_DIR%
svn status
svn update
echo Update Complete! -- %RES_DIR%

cd ../
cd %LUA_FRAMEWORK_DIR%
svn status
svn update
echo Update Complete! -- %LUA_FRAMEWORK_DIR%

cd %LUA_DIR%
svn status
svn update
echo Update Complete! -- %LUA_DIR%

pause