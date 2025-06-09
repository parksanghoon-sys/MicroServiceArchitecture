#!/bin/bash

# 개발 서버 실행 스크립트

set -e

# 색상 정의
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# 환경 변수 설정
export ASPNETCORE_ENVIRONMENT=Development
export DOTNET_WATCH_RESTART_ON_RUDE_EDIT=true

print_status "AuthApiService 개발 서버를 시작합니다..."
print_status "환경: $ASPNETCORE_ENVIRONMENT"
print_status "Hot Reload: 활성화"

echo ""
echo "=========================================="
echo "        개발 서버 시작 중..."
echo "=========================================="
echo ""

# 로그 디렉토리 확인
if [ ! -d "logs" ]; then
    mkdir -p logs
    print_status "로그 디렉토리가 생성되었습니다."
fi

# 개발 서버 시작
print_status "서버 시작 중... (Ctrl+C로 중지)"
print_status "API 문서: http://localhost:5000"
print_status "헬스체크: http://localhost:5000/health"

echo ""
echo "=========================================="
echo "   기본 테스트 사용자 계정"
echo "=========================================="
echo "사용자명: admin     | 비밀번호: Admin123!"
echo "사용자명: testuser  | 비밀번호: Test123!"
echo "사용자명: demo      | 비밀번호: Demo123!"
echo "=========================================="
echo ""

# 개발 서버 실행 (Hot Reload 포함)
dotnet watch run --urls "http://localhost:5000;https://localhost:5001"
