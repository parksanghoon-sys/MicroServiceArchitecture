# 멀티스테이지 빌드를 사용한 Dockerfile

# 1단계: Build 환경
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 프로젝트 파일 복사 및 패키지 복원
COPY *.csproj ./
RUN dotnet restore

# 소스 코드 복사 및 빌드
COPY . ./
RUN dotnet build -c Release -o /app/build

# 2단계: Publish
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish --no-restore

# 3단계: 런타임 환경
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# 운영 환경에 필요한 패키지 설치
RUN apt-get update && apt-get install -y \
    curl \
    && rm -rf /var/lib/apt/lists/*

# 비root 사용자 생성 및 설정 (보안 강화)
RUN groupadd -r appgroup && useradd -r -g appgroup appuser

# 애플리케이션 디렉토리 생성 및 권한 설정
RUN mkdir -p /app/logs && \
    chown -R appuser:appgroup /app

# 발행된 애플리케이션 복사
COPY --from=publish /app/publish .

# 로그 디렉토리 권한 설정
RUN chown -R appuser:appgroup /app/logs

# 비root 사용자로 전환
USER appuser

# 포트 노출
EXPOSE 8080
EXPOSE 8081

# 환경 변수 설정
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_HTTP_PORTS=8080

# 헬스체크 설정
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# 애플리케이션 실행
ENTRYPOINT ["dotnet", "AuthApiService.dll"]
