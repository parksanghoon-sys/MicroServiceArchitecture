
using Auth.Service.ApiModels;
using Auth.Service.Infrastructure.Data;
using Auth.Service.Services;
using Auth.Service.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;

namespace Auth.Service.Endpoints
{
    public static class AuthApiEndpoints
    {
        public static void RegisterEndpoints(this IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("/users", GetUsers);
            routeBuilder.MapGet("/UserRoles", GetUserRoles);
            routeBuilder.MapPost("/login", Login);
            routeBuilder.MapPost("/register", Register);
            routeBuilder.MapPost("/assignlole", AssignRole);
        }

        private static async Task<IResult> GetUserRoles(IAuthStore authStore)
        {
            return Results.Ok(await authStore.GatIdentityRoleAll());
        }
    
        //[Authorize]
        private static async Task<IResult> GetUsers(IAuthStore authStore)
        {
            return Results.Ok(await authStore.GatUserAll());
        }
        private static async Task<IResult> Register(IAuthService authService,[FromBody] RegistrationRequestDto registrationRequestDto)
        {
            var errorMessage = await authService.Register(registrationRequestDto);
            
            if(string.IsNullOrEmpty(errorMessage) == false)
            {                
                return Results.BadRequest(Response.Fail(errorMessage));

            }
            //// 현재 메서드명과 클래스명 가져오기
            //var method = MethodBase.GetCurrentMethod();
            //string className = method.DeclaringType?.Name ?? "UnknownClass";
            //string methodName = method.Name;
            var logName = GetExecutionPoint();

            return Results.Ok(Response.Success($"{logName.ClassName}.{logName.MethodName}_Success"));
        }
        private static async Task<IResult> AssignRole(IAuthService authService, [FromBody] RegistrationRequestDto requestDto)
        {
            var assignRoleSuccessful = await authService.AssignRole(requestDto.Email, requestDto.Role.ToUpper());
            if (!assignRoleSuccessful)
            {
                return Results.BadRequest(Response.Fail(""));
            }
            var logName = GetExecutionPoint();

            return Results.Ok(Response.Success($"{logName.ClassName}.{logName.MethodName}_Success"));
        }
        private static async Task<IResult> Login([FromBody] LoginRequestDto model, IAuthService authService)
        {
            var loginResponse = await authService.Login(model);
            if (loginResponse.User == null)
            {
                var errorMessage = "Username or password is incorrect";
                return Results.BadRequest(Response.Fail(errorMessage));
            }
            var logName = GetExecutionPoint();
            return Results.Ok(Response.Success($"{loginResponse.User}_{MethodBase.GetCurrentMethod()?.Name}_Success"));
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
}
