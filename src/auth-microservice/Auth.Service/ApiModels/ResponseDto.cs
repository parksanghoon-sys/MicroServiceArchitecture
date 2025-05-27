namespace Auth.Service.ApiModels;

public record ResponseDto (object? Result, bool IsSuccess, string Message);

public static class Response
{
    public static ResponseDto Success(string message = "Success") => new ResponseDto(Result: default, IsSuccess: true, Message: message);
    public static ResponseDto Fail(string message) => new ResponseDto(Result: default, IsSuccess: false, Message: message);
}