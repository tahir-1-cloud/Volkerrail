using PRISM.Models;
using System.Linq;

namespace PRISM.Services.Interfaces
{
    public interface IContacts
    {
        Task<List<Contact>> GetData(string alphabat);
        Task<Contact> Insert(Contact param);
        Task<bool> delete(int Id);
    }
}
