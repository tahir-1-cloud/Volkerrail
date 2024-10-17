using Microsoft.EntityFrameworkCore;
using PRISM.Models;
using PRISM.Services.Interfaces;

namespace PRISM.Services
{
    public class MachinesServices : IMachines
    {
        private readonly PRISMContext dBContext;

        public MachinesServices(PRISMContext _dbContext)
        {
            dBContext = _dbContext;
        }

        public async Task<List<Machine>> GetMachines()
        {
            var list = await dBContext.Machines.Where(x => x.RecordStatus=="Active").OrderBy(x => x.Number).ToListAsync();
            return list;
        }

        public async Task<Machine> Insert(Machine param)
        {
            try
            {
                if (param.Id > 0)
                {
                    var obj = await dBContext.Machines.FindAsync(param.Id);
                    if (obj != null)
                    {
                        obj.Number = param.Number;
                        obj.Specification = param.Specification;
                        obj.Speed = param.Speed;
                        obj.StatusId = param.StatusId;
                        obj.Area = param.Area;
                        obj.Type = param.Type;
                        obj.CategoryId = param.CategoryId;
                        obj.OwnerName = param.OwnerName;
                        obj.ManagerName = param.ManagerName;
                        obj.HeadCode = param.HeadCode;
                        obj.Weight = param.Weight;
                        obj.Capabilities = param.Capabilities;
                        obj.Nrn1 = param.Nrn1;
                        obj.Nrn2 = param.Nrn2;
                        //obj.ModifiedBy = 1;
                        obj.ModifiedDate = DateTime.UtcNow;
                        obj.Description = param.Description;
                        dBContext.Machines.Update(obj);
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
                    dBContext.Machines.Add(param);
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
                var obj = dBContext.Machines.FirstOrDefault(x => x.Id == Id);
                if (obj != null)
                {
                    obj.RecordStatus = "Deleted";
                    dBContext.Machines.Update(obj);
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
