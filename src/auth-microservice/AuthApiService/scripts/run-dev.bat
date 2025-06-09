@echo off
setlocal

REM AuthApiService 개발 서버 실행 스크립트 (Windows)

REM 환경 변수 설정
set ASPNETCORE_ENVIRONMENT=Development
set DOTNET_WATCH_RESTART_ON_RUDE_EDIT=true

echo [INFO] AuthApiService 개발 서버를 시작합니다...
echo [INFO] 환경: %ASPNETCORE_ENVIRONMENT%
echo [INFO] Hot Reload: 활성화
echo.

echo ==========================================
echo         개발 서버 시작 중...
echo ==========================================
echo.

REM 로그 디렉토리 확인
if not exist "logs" (
    mkdir logs
    echo [INFO] 로그 디렉토리가 생성되었습니다.
)

REM 개발 서버 시작 정보 출력
echo [INFO] 서버 시작 중... (Ctrl+C로 중지)
echo [INFO] API 문서: http://localhost:5000
echo [INFO] 헬스체크: http://localhost:5000/health
echo.

echo ==========================================
echo    기본 테스트 사용자 계정
echo ==========================================
echo 사용자명: admin     ^| 비밀번호: Admin123!
echo 사용자명: testuser  ^| 비밀번호: Test123!
echo 사용자명: demo      ^| 비밀번호: Demo123!
echo ==========================================
echo.

REM 개발 서버 실행 (Hot Reload 포함)
dotnet watch run --urls "http://localhost:5000;https://localhost:5001"

if %ERRORLEVEL% neq 0 (
    echo.
    echo [ERROR] 서버 시작에 실패했습니다.
    pause
    exit /b 1
)
