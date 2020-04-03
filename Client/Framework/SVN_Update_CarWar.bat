@echo off

set PROJ_DIR="D:\work\WorkSpace_Unity\CardWar"
set PROJ_NAME="CardWar"
set RES_DIR="Res"
set LUA_FRAMEWORK_DIR="Lua"
set LUA_DIR="Game"

echo Start update - %PROJ_DIR% ...
D:
cd %PROJ_DIR%
svn status
svn update
echo Update Complete! -- %PROJ_DIR%

cd Assets/
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