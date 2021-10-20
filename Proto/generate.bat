@echo off
DEL /F/Q/S "%~dp0\Cs\Generated\Source" > NUL

if not exist "%~dp0\Cs\Generated\Source" mkdir "%~dp0\Cs\Generated\Source"

%~dp0\protoc.exe --proto_path="%~dp0\Definitions" --csharp_out="%~dp0\Cs\Generated\Source" "%~dp0\Definitions\*.proto"
echo Generated Cs source files