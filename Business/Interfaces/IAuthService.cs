using Business.Models;

namespace Business.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> SignInAsync(SignInFormData formData);
        Task SignOutAsync();
        Task<AuthResult> SignUpAsync(SignUpFormData formData);
    }
}