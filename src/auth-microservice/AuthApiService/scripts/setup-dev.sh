#!/bin/bash

# 개발 환경 설정 및 실행 스크립트

set -e

echo "🚀 AuthApiService 개발 환경 설정을 시작합니다..."

# 색상 정의
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# 함수 정의
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

# .NET SDK 확인
check_dotnet() {
    print_status "NET SDK 확인 중..."
    if ! command -v dotnet &> /dev/null; then
        print_error ".NET SDK가 설치되지 않았습니다."
        print_status "https://dotnet.microsoft.com/download 에서 .NET 8 SDK를 다운로드하세요."
        exit 1
    fi
    
    DOTNET_VERSION=$(dotnet --version)
    print_success ".NET SDK 버전: $DOTNET_VERSION"
}

# Docker 확인
check_docker() {
    print_status "Docker 확인 중..."
    if ! command -v docker &> /dev/null; then
        print_warning "Docker가 설치되지 않았습니다. Docker 기능은 사용할 수 없습니다."
        return 1
    fi
    
    if ! docker info &> /dev/null; then
        print_warning "Docker가 실행되지 않았습니다. Docker 기능은 사용할 수 없습니다."
        return 1
    fi
    
    print_success "Docker가 정상적으로 실행 중입니다."
    return 0
}

# 의존성 복원
restore_dependencies() {
    print_status "NuGet 패키지 복원 중..."
    dotnet restore
    print_success "의존성 복원 완료"
}

# 빌드
build_project() {
    print_status "프로젝트 빌드 중..."
    dotnet build --configuration Debug --no-restore
    print_success "빌드 완료"
}

# 로그 디렉토리 생성
setup_directories() {
    print_status "필요한 디렉토리 생성 중..."
    mkdir -p logs
    mkdir -p data
    print_success "디렉토리 설정 완료"
}

# 개발용 설정 파일 생성
setup_dev_config() {
    print_status "개발 환경 설정 파일 확인 중..."
    
    if [ ! -f "appsettings.Development.json" ]; then
        print_warning "appsettings.Development.json 파일이 없습니다."
        # 기본 개발 설정 파일은 이미 있다고 가정
    fi
    
    print_success "개발 환경 설정 완료"
}

# 메인 실행 함수
main() {
    echo "=========================================="
    echo "   AuthApiService 개발 환경 설정"
    echo "=========================================="
    
    check_dotnet
    DOCKER_AVAILABLE=0
    if check_docker; then
        DOCKER_AVAILABLE=1
    fi
    
    setup_directories
    setup_dev_config
    restore_dependencies
    build_project
    
    echo ""
    echo "=========================================="
    echo "           설정 완료!"
    echo "=========================================="
    echo ""
    echo "사용 가능한 명령어:"
    echo "  ./scripts/run-dev.sh        - 개발 서버 실행"
    echo "  ./scripts/test.sh           - 테스트 실행"
    echo "  ./scripts/build.sh          - 프로덕션 빌드"
    
    if [ $DOCKER_AVAILABLE -eq 1 ]; then
        echo "  ./scripts/docker-dev.sh     - Docker 개발 환경 실행"
        echo "  ./scripts/docker-prod.sh    - Docker 프로덕션 환경 실행"
    fi
    
    echo ""
    echo "개발 서버를 시작하려면 다음 명령어를 실행하세요:"
    echo "  ./scripts/run-dev.sh"
    echo ""
}

# 스크립트 실행
main "$@"
