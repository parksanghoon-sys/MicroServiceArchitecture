@echo off
setlocal EnableDelayedExpansion

REM AuthApiService 개발 환경 설정 스크립트 (Windows)

echo ==========================================
echo    AuthApiService 개발 환경 설정
echo ==========================================
echo.

REM .NET SDK 확인
echo [INFO] .NET SDK 확인 중...
where dotnet >nul 2>nul
if %ERRORLEVEL% neq 0 (
    echo [ERROR] .NET SDK가 설치되지 않았습니다.
    echo [INFO] https://dotnet.microsoft.com/download 에서 .NET 8 SDK를 다운로드하세요.
    pause
    exit /b 1
)

for /f "tokens=*" %%i in ('dotnet --version') do set DOTNET_VERSION=%%i
echo [SUCCESS] .NET SDK 버전: !DOTNET_VERSION!

REM Docker 확인
echo [INFO] Docker 확인 중...
where docker >nul 2>nul
if %ERRORLEVEL% neq 0 (
    echo [WARNING] Docker가 설치되지 않았습니다. Docker 기능은 사용할 수 없습니다.
    set DOCKER_AVAILABLE=0
) else (
    docker info >nul 2>nul
    if !ERRORLEVEL! neq 0 (
        echo [WARNING] Docker가 실행되지 않았습니다. Docker 기능은 사용할 수 없습니다.
        set DOCKER_AVAILABLE=0
    ) else (
        echo [SUCCESS] Docker가 정상적으로 실행 중입니다.
        set DOCKER_AVAILABLE=1
    )
)

REM 필요한 디렉토리 생성
echo [INFO] 필요한 디렉토리 생성 중...
if not exist "logs" mkdir logs
if not exist "data" mkdir data
echo [SUCCESS] 디렉토리 설정 완료

REM 개발 환경 설정 파일 확인
echo [INFO] 개발 환경 설정 파일 확인 중...
if not exist "appsettings.Development.json" (
    echo [WARNING] appsettings.Development.json 파일이 없습니다.
)
echo [SUCCESS] 개발 환경 설정 완료

REM NuGet 패키지 복원
echo [INFO] NuGet 패키지 복원 중...
dotnet restore
if %ERRORLEVEL% neq 0 (
    echo [ERROR] 패키지 복원에 실패했습니다.
    pause
    exit /b 1
)
echo [SUCCESS] 의존성 복원 완료

REM 프로젝트 빌드
echo [INFO] 프로젝트 빌드 중...
dotnet build --configuration Debug --no-restore
if %ERRORLEVEL% neq 0 (
    echo [ERROR] 빌드에 실패했습니다.
    pause
    exit /b 1
)
echo [SUCCESS] 빌드 완료

echo.
echo ==========================================
echo            설정 완료!
echo ==========================================
echo.
echo 사용 가능한 명령어:
echo   scripts\run-dev.bat        - 개발 서버 실행
echo   scripts\test.bat           - 테스트 실행
echo   scripts\build.bat          - 프로덕션 빌드

if !DOCKER_AVAILABLE! equ 1 (
    echo   scripts\docker-dev.bat     - Docker 개발 환경 실행
    echo   scripts\docker-prod.bat    - Docker 프로덕션 환경 실행
)

echo.
echo 개발 서버를 시작하려면 다음 명령어를 실행하세요:
echo   scripts\run-dev.bat
echo.

pause
