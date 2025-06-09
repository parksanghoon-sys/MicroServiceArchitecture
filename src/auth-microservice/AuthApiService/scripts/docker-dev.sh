#!/bin/bash

# Docker 개발 환경 실행 스크립트

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

# Docker 확인
check_docker() {
    if ! command -v docker &> /dev/null; then
        print_error "Docker가 설치되지 않았습니다."
        exit 1
    fi
    
    if ! docker info &> /dev/null; then
        print_error "Docker가 실행되지 않았습니다."
        exit 1
    fi
}

# Docker Compose 확인
check_docker_compose() {
    if ! command -v docker-compose &> /dev/null; then
        print_error "Docker Compose가 설치되지 않았습니다."
        exit 1
    fi
}

# 환경 변수 파일 확인
check_env_file() {
    if [ ! -f ".env" ]; then
        print_warning ".env 파일이 없습니다. 기본값으로 실행됩니다."
    fi
}

# 로그 디렉토리 생성
setup_directories() {
    print_status "필요한 디렉토리 생성 중..."
    mkdir -p logs/nginx
    mkdir -p data
    print_success "디렉토리 설정 완료"
}

# 기존 컨테이너 정리
cleanup_containers() {
    print_status "기존 컨테이너 정리 중..."
    docker-compose down --remove-orphans 2>/dev/null || true
    print_success "컨테이너 정리 완료"
}

# 개발 환경 시작
start_dev_environment() {
    print_status "Docker 개발 환경 시작 중..."
    
    # 개발 환경용 오버라이드 파일 사용
    docker-compose \
        -f docker-compose.yml \
        -f docker-compose.override.yml \
        up --build -d
    
    print_success "Docker 개발 환경이 시작되었습니다."
}

# 서비스 상태 확인
check_services() {
    print_status "서비스 상태 확인 중..."
    
    # 잠시 대기 (서비스 시작 시간)
    sleep 10
    
    # API 서비스 확인
    if curl -f http://localhost:5000/health &> /dev/null; then
        print_success "✅ API 서비스: 정상"
    else
        print_error "❌ API 서비스: 응답 없음"
    fi
    
    # Redis 확인
    if docker-compose ps redis | grep -q "Up"; then
        print_success "✅ Redis: 정상"
    else
        print_warning "⚠️ Redis: 상태 확인 필요"
    fi
    
    # Nginx 확인
    if curl -f http://localhost:8000/health &> /dev/null; then
        print_success "✅ Nginx: 정상"
    else
        print_warning "⚠️ Nginx: 상태 확인 필요"
    fi
}

# 메인 실행 함수
main() {
    echo "=========================================="
    echo "   Docker 개발 환경 시작"
    echo "=========================================="
    
    check_docker
    check_docker_compose
    check_env_file
    setup_directories
    cleanup_containers
    start_dev_environment
    check_services
    
    echo ""
    echo "=========================================="
    echo "        개발 환경 시작 완료!"
    echo "=========================================="
    echo ""
    echo "🌐 서비스 URL:"
    echo "  • API 서비스:      http://localhost:5000"
    echo "  • Nginx (프록시):  http://localhost:8000"
    echo "  • API 문서:        http://localhost:5000"
    echo "  • 헬스체크:        http://localhost:5000/health"
    echo ""
    echo "🛠️ 개발 도구:"
    echo "  • MailHog:         http://localhost:8025"
    echo "  • Adminer:         http://localhost:8080"
    echo "  • Grafana:         http://localhost:3000 (admin/admin123)"
    echo "  • Prometheus:      http://localhost:9090"
    echo ""
    echo "📋 유용한 명령어:"
    echo "  • 로그 확인:       docker-compose logs -f auth-api"
    echo "  • 서비스 중지:     docker-compose down"
    echo "  • 재시작:          docker-compose restart auth-api"
    echo ""
    echo "기본 테스트 계정:"
    echo "  • admin / Admin123!"
    echo "  • testuser / Test123!"
    echo "  • demo / Demo123!"
    echo ""
}

# 스크립트 실행
main "$@"
