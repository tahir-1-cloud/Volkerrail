using Microsoft.EntityFrameworkCore;
using PRISM.Models;
using PRISM.Services.Interfaces;

namespace PRISM.Services
{
    public class ContactsServices : IContacts
    {
        private readonly PRISMContext dBContext;

        public ContactsServices(PRISMContext _dbContext)
        {
            dBContext = _dbContext;
        }

        public async Task<List<Contact>> GetData(string alphabat)
        {
            if (string.IsNullOrEmpty(alphabat))
            {
                var list = await dBContext.Contacts.Where(x => x.RecordStatus == "Active").OrderBy(x=>x.Name).ToListAsync();
                return list;
            }
            else
            {
                var list = await dBContext.Contacts.Where(x => x.RecordStatus == "Active" && x.Name.StartsWith(alphabat)).OrderBy(x=>x.Name).ToListAsync();
                return list;
            }
        }

        public async Task<Contact> Insert(Contact param)
        {
            try
            {
                if (param.Id > 0)
                {
                    var obj = await dBContext.Contacts.FindAsync(param.Id);
                    if (obj != null)
                    {
                        obj.Name = param.Name;
                        obj.Address = param.Address;
                        obj.PhoneNumber = param.PhoneNumber;
                        obj.FaxNumber = param.FaxNumber;
                        obj.Notes = param.Notes;
                        obj.MobileNumber = param.MobileNumber;
                        obj.Company = param.Company;
                        obj.Email = param.Email;
                        obj.JobTitle = param.JobTitle;
                        //obj.ModifiedBy = 1;
                        obj.ModifiedDate = DateTime.UtcNow;
                        dBContext.Contacts.Update(obj);
                        await dBContext.SaveChangesAsync();
                    }

                    return param;
                }
                else
                {
                    param.CreatedDate = DateTime.UtcNow;
                    param.ModifiedDate = DateTime.UtcNow;
                    param.RecordStatus = "Active";
                    //param.CreatedBy = 1;
                    dBContext.Contacts.Add(param);
                    await dBContext.SaveChangesAsync();

                    return param;
                }
            }
            catch (Exception ex)
            {

                return null;
            }

        }
        public async Task<bool> delete(int Id)
        {
            try
            {
                var obj = dBContext.Contacts.FirstOrDefault(x => x.Id == Id);
                if (obj != null)
                {
                    obj.RecordStatus = "Deleted";
                    dBContext.Contacts.Update(obj);
                    await dBContext.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

    }
}
