using Microsoft.EntityFrameworkCore;
using PRISM.Models;
using PRISM.Services.Interfaces;

namespace PRISM.Services
{
    public class RoutesServices : IRoutes
    {
        private readonly PRISMContext dBContext;

        public RoutesServices(PRISMContext _dbContext)
        {
            dBContext = _dbContext;
        }

        public async Task<List<PRISM.Models.Route>> GetData()
        {
            var list = await dBContext.Routes.Where(x => x.RecordStatus == "Active").OrderBy(x => x.Name).ToListAsync();
            return list;
        }

        public async Task<PRISM.Models.Route> Insert(PRISM.Models.Route param)
        {
            try
            {
                if (param.Id > 0)
                {
                    var obj = await dBContext.Routes.FindAsync(param.Id);
                    if (obj != null)
                    {
                        obj.Name = param.Name;
                        obj.StablingPoints = param.StablingPoints;
                        obj.Comments = param.Comments;
                        obj.ShortCode = param.ShortCode;
                        obj.Stanox = param.Stanox;
                       // obj.ModifiedBy = 1;
                        obj.ModifiedDate = DateTime.UtcNow;
                        dBContext.Routes.Update(obj);
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
                    dBContext.Routes.Add(param);
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
                var obj = dBContext.Routes.FirstOrDefault(x => x.Id == Id);
                if (obj != null)
                {
                    obj.RecordStatus = "Deleted";
                    dBContext.Routes.Update(obj);
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
