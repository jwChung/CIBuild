@ECHO OFF
IF "%VS120COMNTOOLS%"=="" (
    SET VsVersionProperty=""
) ELSE (
    SET VsVersionProperty="/p:VisualStudioVersion=12.0"
)

"%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild" "%~dp0Build.proj" /v:minimal /maxcpucount /nodeReuse:false %VsVersionProperty% %*
