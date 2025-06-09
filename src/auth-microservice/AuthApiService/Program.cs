using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using AuthApiService.Configuration;
using AuthApiService.Interfaces;
using AuthApiService.Services;
using AuthApiService.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Serilog 설정
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/app-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 31,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog(Log.Logger);
// Serilog.ILogger를 DI에 등록
//builder.Services.AddSingleton(Log.Logger);
// 설정 바인딩
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection(JwtSettings.SectionName));

builder.Services.Configure<LoggingSettings>(
    builder.Configuration.GetSection(LoggingSettings.SectionName));

builder.Services.Configure<SecuritySettings>(
    builder.Configuration.GetSection(SecuritySettings.SectionName));

builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection(DatabaseSettings.SectionName));

builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection(AppSettings.SectionName));

// JWT 인증 설정
var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
    ?? throw new InvalidOperationException("JWT 설정을 찾을 수 없습니다.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // 개발 환경에서는 false
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(5),
        RequireExpirationTime = true
    };

    // JWT 이벤트 핸들러
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggingService>();
            _ = Task.Run(async () =>
            {
                await logger.LogWarningAsync(
                    $"JWT 인증 실패: {context.Exception.Message}",
                    "JwtAuthentication",
                    properties: new Dictionary<string, object>
                    {
                        ["Exception"] = context.Exception.Message,
                        ["Path"] = context.HttpContext.Request.Path,
                        ["Method"] = context.HttpContext.Request.Method
                    });
            });
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggingService>();
            var userIdClaim = context.Principal?.FindFirst("user_id");
            
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
            {
                _ = Task.Run(async () =>
                {
                    await logger.LogDebugAsync(
                        "JWT 토큰 검증 성공",
                        "JwtAuthentication",
                        userId,
                        new Dictionary<string, object>
                        {
                            ["UserId"] = userId,
                            ["Path"] = context.HttpContext.Request.Path,
                            ["Method"] = context.HttpContext.Request.Method
                        });
                });
            }
            return Task.CompletedTask;
        }
    };
});

// 권한 부여 설정
builder.Services.AddAuthorization();

// 서비스 등록
builder.Services.AddSingleton<ILoggingService, LoggingService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// 컨트롤러 설정
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // 모델 검증 실패 시 사용자 정의 응답
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors)
                .Select(x => x.ErrorMessage);

            var response = new
            {
                Success = false,
                Message = "입력 데이터가 올바르지 않습니다.",
                Errors = errors
            };

            return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(response);
        };
    });

// API 탐색기 설정
builder.Services.AddEndpointsApiExplorer();

// Swagger 설정
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AuthApiService",
        Version = "v1",
        Description = "JWT 기반 인증 서비스 API",
        Contact = new OpenApiContact
        {
            Name = "개발팀",
            Email = "dev@example.com"
        }
    });

    // JWT 인증 설정
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // XML 문서 파일 경로 (API 문서화용)
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// CORS 설정
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// HTTP 클라이언트 팩토리
builder.Services.AddHttpClient();

// 메모리 캐시
builder.Services.AddMemoryCache();

// 상태 검사
builder.Services.AddHealthChecks();

// 응답 압축
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

var app = builder.Build();

// 개발 환경 설정
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Swagger UI (개발 및 스테이징 환경에서만)
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthApiService v1");
        options.RoutePrefix = string.Empty; // Swagger UI를 루트 경로에서 제공
        options.DisplayRequestDuration();
        options.EnableDeepLinking();
        options.EnableFilter();
        options.ShowExtensions();
        options.EnableValidator();
    });
}

// 미들웨어 파이프라인 구성
app.UseResponseCompression();

// HTTPS 리디렉션
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// CORS
app.UseCors("DefaultCorsPolicy");

// 글로벌 예외 처리 (인증보다 먼저)
app.UseGlobalExceptionHandling();

// 인증 및 권한 부여
app.UseAuthentication();
app.UseAuthorization();

// 요청 로깅 (인증 후에)
app.UseRequestLogging();

// 컨트롤러 매핑
app.MapControllers();

// 상태 검사 엔드포인트
app.MapHealthChecks("/health");

// 기본 정보 엔드포인트
app.MapGet("/", () => new
{
    Application = "AuthApiService",
    Version = "1.0.0",
    Status = "Running",
    Timestamp = DateTime.UtcNow,
    Environment = app.Environment.EnvironmentName
});

// 애플리케이션 시작 로그
Log.Information("AuthApiService가 시작되었습니다. Environment: {Environment}", app.Environment.EnvironmentName);

try
{
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "애플리케이션 시작 중 오류가 발생했습니다");
}
finally
{
    Log.CloseAndFlush();
}
