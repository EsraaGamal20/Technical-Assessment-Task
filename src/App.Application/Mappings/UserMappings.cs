using App.Application.Features.Auth.Dtos;
using App.Domain.Entities;

namespace App.Application.Mappings;

public static class UserMappings
{
    public static UserDto ToDto(this ApplicationUser user) => new(
        user.Id,
        user.FullName,
        user.Email,
        user.PhoneNumber,
        user.IsPhoneVerified,
        user.IsEmailVerified,
        user.CreatedAt);
}
