using PRISM.DTO;
using PRISM.Models;
using System.Linq;

namespace PRISM.Services.Interfaces
{
    public interface IDistributions
    {
        Task<List<DistributionViewModel>> GetData(string type="orignal");
        Task<Distribution> Insert(Distribution param);
        Task<bool> delete(int Id);
    }
}
