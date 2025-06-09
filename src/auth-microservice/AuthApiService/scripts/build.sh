#!/bin/bash

# 프로덕션 빌드 스크립트

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

# 빌드 설정
BUILD_CONFIGURATION=${1:-Release}
OUTPUT_DIR="./publish"
RUNTIME=${2:-linux-x64}

print_status "AuthApiService 프로덕션 빌드를 시작합니다..."
print_status "빌드 구성: $BUILD_CONFIGURATION"
print_status "타겟 런타임: $RUNTIME"
print_status "출력 디렉토리: $OUTPUT_DIR"

echo ""
echo "=========================================="
echo "        프로덕션 빌드 시작"
echo "=========================================="
echo ""

# 출력 디렉토리 정리
if [ -d "$OUTPUT_DIR" ]; then
    print_status "기존 빌드 결과물 정리 중..."
    rm -rf "$OUTPUT_DIR"
fi

# 의존성 복원
print_status "NuGet 패키지 복원 중..."
dotnet restore
print_success "의존성 복원 완료"

# 빌드
print_status "프로젝트 빌드 중..."
dotnet build \
    --configuration "$BUILD_CONFIGURATION" \
    --no-restore \
    --verbosity minimal

if [ $? -ne 0 ]; then
    print_error "빌드에 실패했습니다."
    exit 1
fi
print_success "빌드 완료"

# 게시
print_status "애플리케이션 게시 중..."
dotnet publish \
    --configuration "$BUILD_CONFIGURATION" \
    --runtime "$RUNTIME" \
    --self-contained false \
    --no-build \
    --output "$OUTPUT_DIR" \
    --verbosity minimal

if [ $? -ne 0 ]; then
    print_error "게시에 실패했습니다."
    exit 1
fi
print_success "게시 완료"

# 빌드 결과 검증
print_status "빌드 결과 검증 중..."
if [ ! -f "$OUTPUT_DIR/AuthApiService.dll" ]; then
    print_error "빌드 결과물을 찾을 수 없습니다."
    exit 1
fi

# 파일 크기 계산
BUILD_SIZE=$(du -sh "$OUTPUT_DIR" | cut -f1)
print_success "빌드 결과물 크기: $BUILD_SIZE"

# 추가 파일 복사
print_status "추가 파일 복사 중..."

# 설정 파일 복사 (운영 환경용)
if [ -f "appsettings.Production.json" ]; then
    cp "appsettings.Production.json" "$OUTPUT_DIR/"
    print_success "운영 환경 설정 파일 복사 완료"
fi

# Docker 파일 복사
if [ -f "Dockerfile" ]; then
    cp "Dockerfile" "$OUTPUT_DIR/"
    print_success "Dockerfile 복사 완료"
fi

# 필요한 디렉토리 생성
mkdir -p "$OUTPUT_DIR/logs"
mkdir -p "$OUTPUT_DIR/data"
print_success "런타임 디렉토리 생성 완료"

# 실행 권한 설정 (Linux/Mac)
if [[ "$OSTYPE" == "linux-gnu"* ]] || [[ "$OSTYPE" == "darwin"* ]]; then
    chmod +x "$OUTPUT_DIR/AuthApiService"
    print_success "실행 권한 설정 완료"
fi

echo ""
echo "=========================================="
echo "        빌드 완료!"
echo "=========================================="
echo ""
echo "📁 빌드 결과물: $OUTPUT_DIR"
echo "📦 빌드 크기: $BUILD_SIZE"
echo "🏃 타겟 런타임: $RUNTIME"
echo "⚙️ 빌드 구성: $BUILD_CONFIGURATION"
echo ""
echo "🚀 실행 방법:"
echo "  cd $OUTPUT_DIR"
echo "  dotnet AuthApiService.dll"
echo ""
echo "🐳 Docker 빌드:"
echo "  docker build -t authapi:latest $OUTPUT_DIR"
echo ""
echo "📋 배포 체크리스트:"
echo "  ✅ 환경 변수 설정 확인"
echo "  ✅ 데이터베이스 연결 문자열 확인"
echo "  ✅ JWT 시크릿 키 설정"
echo "  ✅ HTTPS 인증서 준비"
echo "  ✅ 방화벽 설정 확인"
echo ""

print_success "빌드가 성공적으로 완료되었습니다!"
