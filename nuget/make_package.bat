if not exist "build" mkdir "build"

copy /Y "..\build\llvm\Release\bin\libclang.dll" "build\"
copy /Y "..\build\sealang\Release\sealang.dll" "build\"

if not exist "lib\net40" mkdir "lib\net40"

copy /Y "..\SealangSharp\bin\Release\ClangSharp.dll" "lib\net40\"
copy /Y "..\SealangSharp\bin\Release\SealangSharp.dll" "lib\net40\"

nuget pack SealangSharp.nuspec