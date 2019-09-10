@echo off

set USERNAME="zhengnan262@qq.com"
set PASSWORD="415161059Thj"

set PROJ_DIR="D:\work\WorkSpace_Unity"

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

echo Start checkout - %PROJ_URL% ...
D:
cd %PROJ_DIR%
svn checkout %PROJ_URL% %PROJ_NAME% --username %USERNAME% --password %PASSWORD%
echo Project framework checkout out complete! -- %PROJ_URL%

echo Start checkout - %GAME_CODE_URL% ...
cd %PROJ_NAME%\Assets
svn checkout %GAME_CODE_URL% %GAME_CODE_DIR% --username %USERNAME% --password %PASSWORD%
echo Game C# code checkout out complete! -- %GAME_CODE_URL%

echo Start checkout - %RES_URL% ...
svn checkout %RES_URL% %RES_DIR% --username %USERNAME% --password %PASSWORD%
echo Game C# code checkout out complete! -- %RES_URL%

echo Start checkout - %LUA_FRAMEWORK_URL% ...
svn checkout %LUA_FRAMEWORK_URL% %LUA_FRAMEWORK_DIR% --username %USERNAME% --password %PASSWORD%
echo Game lua framework checkout out complete! -- %LUA_FRAMEWORK_URL%

cd %LUA_FRAMEWORK_DIR%
echo Start checkout - %LUA_URL% ...
svn checkout %LUA_URL% %LUA_DIR% --username %USERNAME% --password %PASSWORD%
echo Game lua checkout out complete! -- %LUA_URL%
pause