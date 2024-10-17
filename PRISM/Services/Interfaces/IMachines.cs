using PRISM.Models;
using System.Linq;

namespace PRISM.Services.Interfaces
{
    public interface IMachines
    {
        Task<List<Machine>> GetMachines();
        Task<Machine> Insert(Machine param);
        Task<bool> delete(int Id);
    }
}
