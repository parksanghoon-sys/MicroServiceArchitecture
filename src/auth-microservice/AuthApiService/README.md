# AuthApiService

C#/.NET 8 기반의 JWT 인증 서비스 API입니다. 객체지향 설계 원칙을 적용하여 구현되었으며, 액세스 토큰과 리프레시 토큰을 지원하는 완전한 인증 시스템을 제공합니다.

## 🚀 주요 특징

- **JWT 기반 인증**: 액세스 토큰과 리프레시 토큰을 사용한 안전한 인증
- **객체지향 설계**: SOLID 원칙을 적용한 깔끔한 아키텍처
- **최신 C# 문법**: .NET 8, Record types, Pattern matching 등 활용
- **구조화된 로깅**: Serilog를 활용한 체계적인 로그 관리
- **Docker 지원**: 컨테이너화된 배포 환경
- **API 문서화**: Swagger/OpenAPI 지원
- **보안 강화**: BCrypt 비밀번호 해싱, CORS, HTTPS 지원

## 🏗️ 아키텍처

```
AuthApiService/
├── Controllers/          # API 컨트롤러
├── Services/            # 비즈니스 로직 서비스
├── Interfaces/          # 서비스 인터페이스
├── Models/              # 데이터 모델 및 DTOs
├── Middleware/          # 커스텀 미들웨어
├── Configuration/       # 설정 클래스들
└── Docker/              # Docker 관련 파일들
```

### 핵심 컴포넌트

- **AuthService**: 사용자 인증 및 토큰 관리
- **TokenService**: JWT 토큰 생성 및 검증
- **UserService**: 사용자 관리 및 비밀번호 처리
- **LoggingService**: 구조화된 로깅
- **RefreshTokenService**: 리프레시 토큰 관리

## 🛠️ 기술 스택

- **.NET 8**: 최신 .NET 플랫폼
- **ASP.NET Core**: 웹 API 프레임워크
- **JWT**: JSON Web Token 인증
- **Serilog**: 구조화된 로깅
- **BCrypt**: 비밀번호 해싱
- **Swagger**: API 문서화
- **Docker**: 컨테이너화
- **Redis**: 캐싱 (선택적)
- **Nginx**: 리버스 프록시

## 🚦 시작하기

### 사전 요구사항

- .NET 8 SDK
- Docker & Docker Compose (선택적)
- Git

### 로컬 개발 환경 설정

1. **저장소 클론**
```bash
git clone <repository-url>
cd AuthApiService
```

2. **의존성 복원**
```bash
dotnet restore
```

3. **설정 파일 확인**
```bash
# appsettings.Development.json에서 개발 환경 설정 확인
# 필요시 JWT SecretKey 등 수정
```

4. **애플리케이션 실행**
```bash
dotnet run
```

5. **API 문서 확인**
```
브라우저에서 http://localhost:5000 접속
Swagger UI에서 API 문서 확인
```

### Docker를 사용한 실행

1. **Docker Compose로 전체 스택 실행**
```bash
docker-compose up -d
```

2. **개발 환경으로 실행**
```bash
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d
```

3. **서비스 확인**
```bash
# API 서비스
curl http://localhost:8080/health

# Grafana 대시보드
http://localhost:3000 (admin/admin123)

# Prometheus 모니터링
http://localhost:9090
```

## 📡 API 엔드포인트

### 인증 관련

| 메서드 | 엔드포인트 | 설명 | 인증 필요 |
|--------|------------|------|-----------|
| POST | `/api/auth/register` | 사용자 등록 | ❌ |
| POST | `/api/auth/login` | 로그인 | ❌ |
| POST | `/api/auth/refresh` | 토큰 갱신 | ❌ |
| POST | `/api/auth/logout` | 로그아웃 | ✅ |
| POST | `/api/auth/revoke-all` | 모든 토큰 무효화 | ✅ |
| GET | `/api/auth/me` | 현재 사용자 정보 | ✅ |

### 시스템 관련

| 메서드 | 엔드포인트 | 설명 |
|--------|------------|------|
| GET | `/health` | 헬스체크 |
| GET | `/` | 서비스 정보 |

## 🔧 사용 예제

### 1. 사용자 등록

```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "email": "test@example.com",
    "password": "Test123!"
  }'
```

### 2. 로그인

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "password": "Test123!"
  }'
```

응답:
```json
{
  "success": true,
  "message": "로그인에 성공했습니다.",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "base64-encoded-refresh-token",
    "expiresIn": 900,
    "tokenType": "Bearer"
  }
}
```

### 3. 인증이 필요한 API 호출

```bash
curl -X GET http://localhost:5000/api/auth/me \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

### 4. 토큰 갱신

```bash
curl -X POST http://localhost:5000/api/auth/refresh \
  -H "Content-Type: application/json" \
  -d '{
    "refreshToken": "YOUR_REFRESH_TOKEN"
  }'
```

## ⚙️ 설정

### JWT 설정

```json
{
  "JwtSettings": {
    "SecretKey": "your-secret-key-min-256-bits",
    "Issuer": "AuthApiService",
    "Audience": "AuthApiServiceClients",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  }
}
```

### 보안 설정

```json
{
  "SecuritySettings": {
    "MinPasswordLength": 8,
    "RequireSpecialCharacter": true,
    "RequireDigit": true,
    "RequireUppercase": true,
    "RequireLowercase": true,
    "MaxLoginAttempts": 5,
    "AccountLockoutMinutes": 30
  }
}
```

### 로깅 설정

```json
{
  "LoggingSettings": {
    "MinimumLevel": "Information",
    "EnableConsoleLogging": true,
    "EnableFileLogging": true,
    "EnableStructuredLogging": true,
    "PreventSensitiveDataLogging": true
  }
}
```

## 🔒 보안 고려사항

- **강력한 JWT 시크릿 키**: 최소 256비트 이상의 안전한 키 사용
- **HTTPS 사용**: 운영 환경에서는 반드시 HTTPS 활성화
- **토큰 만료 시간**: 적절한 액세스 토큰 만료 시간 설정 (15분 권장)
- **리프레시 토큰 관리**: 한 번 사용 후 폐기 정책
- **비밀번호 정책**: 강력한 비밀번호 요구사항 설정
- **로그인 시도 제한**: 무차별 대입 공격 방지
- **민감한 데이터 로깅 방지**: 비밀번호, 토큰 등 로그에서 제외

## 📊 모니터링

### 로그 확인

```bash
# 실시간 로그 확인
tail -f logs/app-.log

# Docker 컨테이너 로그
docker-compose logs -f auth-api
```

### 메트릭 확인

- **Prometheus**: http://localhost:9090
- **Grafana**: http://localhost:3000
- **헬스체크**: http://localhost:8080/health

## 🧪 테스트

### 기본 테스트 사용자

개발 환경에서는 다음 테스트 사용자들이 자동으로 생성됩니다:

| 사용자명 | 이메일 | 비밀번호 |
|----------|--------|----------|
| admin | admin@example.com | Admin123! |
| testuser | test@example.com | Test123! |
| demo | demo@example.com | Demo123! |

### API 테스트

```bash
# 헬스체크
curl http://localhost:5000/health

# 서비스 정보
curl http://localhost:5000/

# 테스트 사용자로 로그인
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username": "admin", "password": "Admin123!"}'
```

## 🐛 문제 해결

### 일반적인 문제들

1. **JWT 시크릿 키 오류**
   - `appsettings.json`에서 `JwtSettings.SecretKey`가 충분히 긴지 확인 (최소 256비트)

2. **포트 충돌**
   - 다른 서비스가 5000번 포트를 사용 중인지 확인
   - `launchSettings.json`에서 포트 변경 가능

3. **Docker 빌드 실패**
   - Docker가 실행 중인지 확인
   - `docker-compose down && docker-compose up --build`로 재빌드

4. **로그 파일 권한 문제**
   - `logs` 디렉토리 권한 확인
   - Docker 환경에서는 볼륨 마운트 권한 확인

## 📝 개발 가이드

### 새로운 서비스 추가

1. `Interfaces/` 폴더에 인터페이스 정의
2. `Services/` 폴더에 구현체 작성
3. `Program.cs`에서 DI 등록
4. 필요시 `Controllers/`에 컨트롤러 추가

### 로깅 활용

```csharp
// 구조화된 로깅 예제
await _loggingService.LogInformationAsync(
    "사용자 로그인 성공",
    "Authentication",
    userId,
    new Dictionary<string, object>
    {
        ["Username"] = username,
        ["IPAddress"] = ipAddress,
        ["UserAgent"] = userAgent
    });
```

### 새로운 엔드포인트 추가

```csharp
[HttpGet("example")]
[Authorize] // 인증 필요한 경우
public async Task<ActionResult<ApiResponse<object>>> ExampleAsync()
{
    // 비즈니스 로직
    return Ok(ApiResponse<object>.SuccessResponse(data, "성공"));
}
```

## 🤝 기여하기

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 라이선스

이 프로젝트는 MIT 라이선스 하에 배포됩니다. 자세한 내용은 `LICENSE` 파일을 참조하세요.

## 📞 지원

- **Issues**: GitHub Issues 탭에서 버그 리포트 및 기능 요청
- **Documentation**: 코드 내 XML 문서 및 Swagger UI 참조
- **Email**: dev@example.com

---

**참고**: 이 프로젝트는 교육 및 개발 목적으로 제작되었습니다. 운영 환경에서 사용하기 전에 보안 검토를 수행하시기 바랍니다.
