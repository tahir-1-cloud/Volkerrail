using PRISM.Models;
using System.Linq;

namespace PRISM.Services.Interfaces
{
    public interface IRoutes
    {
        Task<List<PRISM.Models.Route>> GetData();
        Task<PRISM.Models.Route> Insert(PRISM.Models.Route param);
        Task<bool> delete(int Id);
    }
}
