#!/bin/bash

# API 테스트 스크립트

set -e

# 색상 정의
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

print_status() {
    echo -e "${BLUE}[TEST]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[PASS]${NC} $1"
}

print_error() {
    echo -e "${RED}[FAIL]${NC} $1"
}

# 설정
API_BASE_URL=${1:-"http://localhost:5000"}
TEST_USERNAME="testuser"
TEST_PASSWORD="Test123!"
TEST_EMAIL="test@example.com"

print_status "AuthApiService API 테스트를 시작합니다..."
print_status "API 기본 URL: $API_BASE_URL"

echo ""
echo "=========================================="
echo "        API 테스트 시작"
echo "=========================================="
echo ""

# curl 확인
if ! command -v curl &> /dev/null; then
    print_error "curl이 설치되지 않았습니다."
    exit 1
fi

# jq 확인 (JSON 파싱용)
if ! command -v jq &> /dev/null; then
    print_status "jq가 설치되지 않았습니다. JSON 파싱 없이 진행합니다."
    USE_JQ=false
else
    USE_JQ=true
fi

# 테스트 카운터
TOTAL_TESTS=0
PASSED_TESTS=0

# 테스트 함수
run_test() {
    local test_name="$1"
    local expected_status="$2"
    local response_file="/tmp/api_test_response"
    
    TOTAL_TESTS=$((TOTAL_TESTS + 1))
    
    # HTTP 상태 코드와 응답 본문을 분리해서 저장
    http_status=$(curl -s -w "%{http_code}" -o "$response_file" "$@" 2>/dev/null || echo "000")
    
    if [ "$http_status" = "$expected_status" ]; then
        print_success "$test_name (HTTP $http_status)"
        PASSED_TESTS=$((PASSED_TESTS + 1))
        
        # JSON 응답이면 예쁘게 출력
        if [ "$USE_JQ" = true ] && [ -f "$response_file" ]; then
            if jq . "$response_file" >/dev/null 2>&1; then
                echo "    Response: $(jq -c . "$response_file")"
            fi
        fi
        return 0
    else
        print_error "$test_name (Expected: $expected_status, Got: $http_status)"
        if [ -f "$response_file" ]; then
            echo "    Response: $(cat "$response_file")"
        fi
        return 1
    fi
}

# 변수 저장용
ACCESS_TOKEN=""
REFRESH_TOKEN=""

# 1. 헬스체크 테스트
echo "1. 헬스체크 테스트"
run_test "헬스체크" "200" \
    -X GET "$API_BASE_URL/health"

echo ""

# 2. 서비스 정보 테스트
echo "2. 서비스 정보 테스트"
run_test "서비스 정보" "200" \
    -X GET "$API_BASE_URL/"

echo ""

# 3. 사용자 등록 테스트
echo "3. 사용자 등록 테스트"
REGISTER_RESPONSE=$(mktemp)
if run_test "사용자 등록" "201" \
    -X POST "$API_BASE_URL/api/auth/register" \
    -H "Content-Type: application/json" \
    -d "{\"username\":\"${TEST_USERNAME}_$(date +%s)\",\"email\":\"test_$(date +%s)@example.com\",\"password\":\"$TEST_PASSWORD\"}" \
    -o "$REGISTER_RESPONSE"; then
    
    if [ "$USE_JQ" = true ]; then
        ACCESS_TOKEN=$(jq -r '.data.accessToken // empty' "$REGISTER_RESPONSE" 2>/dev/null || echo "")
        REFRESH_TOKEN=$(jq -r '.data.refreshToken // empty' "$REGISTER_RESPONSE" 2>/dev/null || echo "")
    fi
fi

echo ""

# 4. 기존 사용자 로그인 테스트
echo "4. 사용자 로그인 테스트"
LOGIN_RESPONSE=$(mktemp)
if run_test "사용자 로그인" "200" \
    -X POST "$API_BASE_URL/api/auth/login" \
    -H "Content-Type: application/json" \
    -d "{\"username\":\"admin\",\"password\":\"Admin123!\"}" \
    -o "$LOGIN_RESPONSE"; then
    
    if [ "$USE_JQ" = true ]; then
        ACCESS_TOKEN=$(jq -r '.data.accessToken // empty' "$LOGIN_RESPONSE" 2>/dev/null || echo "")
        REFRESH_TOKEN=$(jq -r '.data.refreshToken // empty' "$LOGIN_RESPONSE" 2>/dev/null || echo "")
        print_status "액세스 토큰을 획득했습니다."
    fi
fi

echo ""

# 5. 인증이 필요한 엔드포인트 테스트
if [ -n "$ACCESS_TOKEN" ]; then
    echo "5. 인증된 사용자 정보 조회 테스트"
    run_test "사용자 정보 조회" "200" \
        -X GET "$API_BASE_URL/api/auth/me" \
        -H "Authorization: Bearer $ACCESS_TOKEN"
    
    echo ""
fi

# 6. 잘못된 로그인 테스트
echo "6. 잘못된 로그인 테스트"
run_test "잘못된 비밀번호" "401" \
    -X POST "$API_BASE_URL/api/auth/login" \
    -H "Content-Type: application/json" \
    -d "{\"username\":\"admin\",\"password\":\"wrongpassword\"}"

echo ""

# 7. 토큰 갱신 테스트
if [ -n "$REFRESH_TOKEN" ]; then
    echo "7. 토큰 갱신 테스트"
    run_test "토큰 갱신" "200" \
        -X POST "$API_BASE_URL/api/auth/refresh" \
        -H "Content-Type: application/json" \
        -d "{\"refreshToken\":\"$REFRESH_TOKEN\"}"
    
    echo ""
fi

# 8. 로그아웃 테스트
if [ -n "$ACCESS_TOKEN" ] && [ -n "$REFRESH_TOKEN" ]; then
    echo "8. 로그아웃 테스트"
    run_test "로그아웃" "200" \
        -X POST "$API_BASE_URL/api/auth/logout" \
        -H "Content-Type: application/json" \
        -H "Authorization: Bearer $ACCESS_TOKEN" \
        -d "{\"refreshToken\":\"$REFRESH_TOKEN\"}"
    
    echo ""
fi

# 9. 인증 없이 보호된 엔드포인트 접근 테스트
echo "9. 인증 없이 보호된 엔드포인트 접근 테스트"
run_test "인증 없는 접근" "401" \
    -X GET "$API_BASE_URL/api/auth/me"

echo ""

# 결과 요약
echo "=========================================="
echo "           테스트 결과 요약"
echo "=========================================="
echo "총 테스트: $TOTAL_TESTS"
echo "성공: $PASSED_TESTS"
echo "실패: $((TOTAL_TESTS - PASSED_TESTS))"

if [ $PASSED_TESTS -eq $TOTAL_TESTS ]; then
    print_success "모든 테스트가 통과했습니다! 🎉"
    exit 0
else
    print_error "일부 테스트가 실패했습니다. 로그를 확인해주세요."
    exit 1
fi
