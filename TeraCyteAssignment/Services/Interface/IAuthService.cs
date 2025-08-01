using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeraCyteAssignment.Services.Interface
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(string username, string password);
        Task<string?> GetAccessTokenAsync();
        Task<bool> RefreshTokenAsync();
        void Logout();
        bool IsLoggedIn { get; }
    }
}
