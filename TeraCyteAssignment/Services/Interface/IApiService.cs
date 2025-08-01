using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeraCyteAssignment.Models;

namespace TeraCyteAssignment.Services.Interface
{
    public interface IApiService
    {
        Task<ImageResponse?> GetImageAsync();
        Task<ResultsResponse?> GetResultsAsync();
    }
}
