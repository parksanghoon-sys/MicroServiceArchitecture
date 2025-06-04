using System.ComponentModel.DataAnnotations;

namespace Auth.Service.ApiModels;

public record TokenRequestDto([property: Required] string Email);
