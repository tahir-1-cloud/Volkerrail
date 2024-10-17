using Microsoft.EntityFrameworkCore;
using PRISM.DTO;
using PRISM.Models;
using PRISM.Services.Interfaces;

namespace PRISM.Services
{
    public class DistributionsServices : IDistributions
    {
        private readonly PRISMContext dBContext;

        public DistributionsServices(PRISMContext _dbContext)
        {
            dBContext = _dbContext;
        }

        public async Task<List<DistributionViewModel>> GetData(string type="original")
        {
            List<DistributionViewModel> listResult = new List<DistributionViewModel>();
            var list = await dBContext.Distributions.ToListAsync();
            foreach (var item in list)
            {
                listResult.Add(new DistributionViewModel()
                {
                    ActiveStatus = item.ActiveStatus,
                    EmailAddress = item.EmailAddress,
                    Id = item.Id,
                    TypeId = item.TypeId,
                    TypeName=dBContext.LookupEntities.Where(x => x.LookupType== "Distribution" && x.Id==item.TypeId).FirstOrDefault()?.Name
                });
            }


            if (type == "orignal")
            {

            }
            else
            {
				listResult = (from row in listResult
							 group row by row.TypeName into g
								  select new DistributionViewModel
								  {
									  TypeName = g.Key,
									  EmailAddress = string.Join(";", g.Select(x => x.EmailAddress))
								  }).ToList();
			}


            return listResult;
        }

        public async Task<Distribution> Insert(Distribution param)
        {
            try
            {
                if (param.Id > 0)
                {
                    var obj = await dBContext.Distributions.FindAsync(param.Id);
                    if (obj != null)
                    {
                        obj.EmailAddress = param.EmailAddress;
                        obj.TypeId = param.TypeId;
                        obj.ActiveStatus = param.ActiveStatus;

                        dBContext.Distributions.Update(obj);
                        await dBContext.SaveChangesAsync();
                    }

                    return param;
                }
                else
                {
                  
                    dBContext.Distributions.Add(param);
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
                var obj = dBContext.Distributions.FirstOrDefault(x => x.Id == Id);
                if (obj != null)
                {
                    dBContext.Distributions.Remove(obj);
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
