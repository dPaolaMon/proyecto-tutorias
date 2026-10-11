using tutorias.Models;
using tutorias.Models.ViewModels;

namespace tutorias.Services
{
    public interface IUserProfileService
    {
        Task<List<Carrera>> GetCarrerasAsync();
        Task<Microsoft.AspNetCore.Mvc.Rendering.SelectList> GetCarrerasSelectListAsync(int? selectedId = null);
        Task<ProfileViewModel?> GetProfileAsync(ApplicationUser user);
        Task<EditProfileViewModel?> GetEditProfileAsync(ApplicationUser user);
        Task<(bool Succeeded, string? ErrorMessage)> UpdateProfileAsync(ApplicationUser user, EditProfileViewModel model);
        Task<(bool Succeeded, string? ErrorMessage)> CompleteProfileAsync(ApplicationUser user, CompleteProfileViewModel model);
        Task<(bool Succeeded, string? ErrorMessage)> LogicalDeleteUserAsync(ApplicationUser user, string? motivoBaja);
    }
}
