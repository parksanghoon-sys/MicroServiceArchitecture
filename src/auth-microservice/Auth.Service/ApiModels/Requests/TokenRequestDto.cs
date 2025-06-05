using System.ComponentModel.DataAnnotations;

namespace Auth.Service.ApiModels.Requests;

public record TokenRequestDto([property: Required] string Email);
