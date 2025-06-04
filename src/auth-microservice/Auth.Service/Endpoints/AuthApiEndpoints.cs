
using Auth.Service.ApiModels;
using Auth.Service.Infrastructure.Data;
using Auth.Service.Services.IService;
using System.Diagnostics;
using System.Reflection;


namespace Auth.Service.Endpoints;

public static class AuthApiEndpoints
{
    public static void RegisterEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("/users", GetUsersAsync);
        routeBuilder.MapGet("/UserRoles", GetUserRolesAsync);
        routeBuilder.MapPost("/login", LoginAsync);
        routeBuilder.MapPost("/register", RegisterAsync);
        routeBuilder.MapPost("/assignlole", AssignRoleAsync);
        routeBuilder.MapPost("/token", GetTokenAsync);
    }

    private static async Task<IResult> GetTokenAsync(TokenRequestDto model, IJwtTokenGenerator jwtTokenGenerator)
    {
        var result = await jwtTokenGenerator.GetTokenAsync(model);

        return Results.Ok(result);
    }

    private static async Task<IResult> GetUserRolesAsync(IAuthStore authStore)
    {
        return Results.Ok(await authStore.GatIdentityRoleAll());
    }

    //[Authorize]
    private static async Task<IResult> GetUsersAsync(IAuthStore authStore)
    {
        return Results.Ok(await authStore.GatUserAll());
    }
    private static async Task<IResult> RegisterAsync(IAuthService authService,RegistrationRequestDto registrationRequestDto)
    {
        var errorMessage = await authService.Register(registrationRequestDto);
        
        if(string.IsNullOrEmpty(errorMessage) == false)
        {                
            return Results.BadRequest(ResponseResult.Fail(errorMessage));

        }
        //// 현재 메서드명과 클래스명 가져오기
        //var method = MethodBase.GetCurrentMethod();
        //string className = method.DeclaringType?.Name ?? "UnknownClass";
        //string methodName = method.Name;
        var logName = GetExecutionPoint();

        return Results.Ok(ResponseResult.Success($"{logName.ClassName}.{logName.MethodName}_Success"));
    }
    private static async Task<IResult> AssignRoleAsync(IAuthService authService, RegistrationRequestDto requestDto)
    {
        var assignRoleSuccessful = await authService.AssignRole(requestDto.Email, requestDto.Role.ToUpper());
        if (!assignRoleSuccessful)
        {
            return Results.BadRequest(ResponseResult.Fail(""));
        }
        var logName = GetExecutionPoint();

        return Results.Ok(ResponseResult.Success($"{logName.ClassName}.{logName.MethodName}_Success"));
    }
    private static async Task<IResult> LoginAsync(LoginRequestDto model, IAuthService authService)
    {
        var loginResponse = await authService.Login(model);
        if (loginResponse.User == null)
        {
            var errorMessage = "Username or password is incorrect";
            return Results.BadRequest(ResponseResult.Fail(errorMessage));
        }
        var logName = GetExecutionPoint();
        return Results.Ok(ResponseResult.Success($"{loginResponse.User}_{MethodBase.GetCurrentMethod()?.Name}_Success"));
    }
    private static (string ClassName, string MethodName) GetExecutionPoint()
    {
        var stackTrace = new StackTrace();
        // 1: 현재 메서드(GetCallerInfo), 2: GetCallerInfo를 호출한 메서드
        var frame = stackTrace.GetFrame(2);
        var method = frame.GetMethod();
        var className = method.DeclaringType?.Name ?? "UnknownClass";
        var methodName = method.Name;
        return (className, methodName);
    }
 
}
