@echo off

set USERNAME="zhengnan262@qq.com"
set PASSWORD="415161059Thj"

set PROJ_DIR="D:\work\WorkSpace_Unity\X-Bullet"

set PROJ_URL="https://github.com/a26053200/zhengnan.git/trunk/Client/Framework"
set PROJ_NAME="X-Bullet"

set GAME_CODE_URL="https://github.com/a26053200/No.x-Bullet.git/trunk/Client/Game"
set GAME_CODE_DIR="Game"

set RES_URL="https://github.com/a26053200/No.x-Bullet.git/trunk/Client/Res"
set RES_DIR="Res"

set LUA_FRAMEWORK_URL="https://github.com/a26053200/zhengnan.git/trunk/Client/LuaFramework"
set LUA_FRAMEWORK_DIR="Lua"

set LUA_URL="https://github.com/a26053200/No.x-Bullet.git/trunk/Client/Lua"
set LUA_DIR="Game"

echo Start update - %PROJ_DIR% ...
D:
cd %PROJ_DIR%
svn status
::svn update

cd Assets\%GAME_CODE_DIR%
svn status
::svn update

cd ../
cd %RES_DIR%
svn status
::svn update

cd ../
cd %LUA_FRAMEWORK_DIR%
svn status
::svn update

cd %LUA_DIR%
svn status
::svn update

pause