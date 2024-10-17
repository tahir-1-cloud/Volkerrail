using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using OfficeOpenXml.ConditionalFormatting;
using PRISM.DTO;
using PRISM.DTO.AbsencesFolder;
using PRISM.DTO.AppUsersModel;
using PRISM.DTO.Employeedto;
using PRISM.Models;
using PRISM.Services.Interfaces;
using System.Data;
using System.Text;

namespace PRISM.Services
{
    public class EmployeesServices : IEmployees
    {
		private readonly PRISMContext dBContext;
        private readonly IConfiguration configuration;
        public EmployeesServices(PRISMContext _dbContext, IConfiguration _configuration)
		{
			dBContext = _dbContext;
			this.configuration = _configuration;
		}

		public async Task<List<Employee>> GetData(int type,string SearchText="")
		{
			List<Employee> list=new List<Employee>();
			if (type == 0)
			{
                if(string.IsNullOrEmpty(SearchText))
				    list = await dBContext.Employees.Where(x => x.RecordStatus != "Deleted").OrderBy(x => x.LastName).ToListAsync();
                else
                    list = await dBContext.Employees.Where(x => x.RecordStatus != "Deleted" && (x.FirstName.Contains(SearchText) ||
                     x.LastName.Contains(SearchText))).OrderBy(x => x.LastName).ToListAsync();
            }
			else
            {
                if (string.IsNullOrEmpty(SearchText))
                    list = await dBContext.Employees.Where(x => x.RecordStatus != "Deleted" && x.EmployeeTypeId==type).OrderBy(x => x.LastName).ToListAsync();
                else
                    list = await dBContext.Employees.Where(x => x.RecordStatus != "Deleted" && x.EmployeeTypeId == type && (x.FirstName.Contains(SearchText) ||
                     x.LastName.Contains(SearchText))).OrderBy(x => x.LastName).ToListAsync();
            }

			return list;
		}
        public async Task<List<Role>> GetRoles()
        {
            
              var  list = await dBContext.Roles.ToListAsync();
            
            return list;
        }
        public async Task<List<ModuleRoleModels>> GetRolesAndRights(int RoleId)
        {
			

            List<ModuleRoleModels> list = new List<ModuleRoleModels>();

            using (SqlConnection con = new SqlConnection(configuration.GetConnectionString("ConnStr")))
            {
                SqlCommand cmd = new SqlCommand("SpGetModuleRoles", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RoleId", RoleId);
                con.Open();
                SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                while (rdr.Read())
                {
                    var cvs = new ModuleRoleModels()
                    {
                        ActionRoles =new ActionRoleModels{IsDelete= Convert.ToBoolean(rdr["IsDelete"]),
							IsEdit=Convert.ToBoolean(rdr["IsEdit"]),
							IsInsert=Convert.ToBoolean(rdr["IsInsert"]),
							IsRead=Convert.ToBoolean(rdr["IsRead"]) },
                        ModuleId = Convert.ToInt32(rdr["Id"]),
                        ModuleName = Convert.ToString(rdr["ModuleName"]),
                        RoleId = Convert.ToInt32(rdr["RoleId"])
                    };

                    list.Add(cvs);
                }
                con.Close();
            }

            return list;
        }
        public async Task<List<ModuleRoleModels>> GetRolesAndRightsByUserId(string UserId)
        {

            int RoleId = 0;
            var userObject=await dBContext.AppUsers.Where(x => x.UserId== UserId).FirstOrDefaultAsync();
            if (userObject != null)
            {
                RoleId = userObject.RoleId??0;
            }

            List<ModuleRoleModels> list = new List<ModuleRoleModels>();

            using (SqlConnection con = new SqlConnection(configuration.GetConnectionString("ConnStr")))
            {
                SqlCommand cmd = new SqlCommand("SpGetModuleRoles", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RoleId", RoleId);
                con.Open();
                SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                while (rdr.Read())
                {
                    var cvs = new ModuleRoleModels()
                    {
                        ActionRoles = new ActionRoleModels
                        {
                            IsDelete = Convert.ToBoolean(rdr["IsDelete"]),
                            IsEdit = Convert.ToBoolean(rdr["IsEdit"]),
                            IsInsert = Convert.ToBoolean(rdr["IsInsert"]),
                            IsRead = Convert.ToBoolean(rdr["IsRead"])
                        },
                        ModuleId = Convert.ToInt32(rdr["Id"]),
                        ModuleName = Convert.ToString(rdr["ModuleName"]),
                        RoleId = Convert.ToInt32(rdr["RoleId"]),
						ModuleIdentity = Convert.ToString(rdr["ModuleId"])
					};

                    list.Add(cvs);
                }
                con.Close();
            }

            return list;
        }
        public async Task<Employee> GetDataById(int Id)
		{
			var list = await dBContext.Employees.Where(x => x.Id== Id).FirstOrDefaultAsync();
			return list;
		}

		public async Task<Employee> Insert(Employee param)
		{
			try
			{
				if (param.Id > 0)
				{
					var obj = await dBContext.Employees.FindAsync(param.Id);
					if (obj != null)
					{
						obj.Department = param.Department;
						obj.FirstName = param.FirstName;
						obj.LastName = param.LastName;
						obj.Initials = param.Initials;
						obj.EmployeeId = param.EmployeeId;
						obj.JobTitle = param.JobTitle;
						obj.Manager = param.Manager;
						obj.ReportsTo = param.ReportsTo;
                        obj.UserName = param.UserName;
                        obj.Location = param.Location;
                        obj.ContactNumber = param.ContactNumber;
                        obj.EmployeeTypeId = param.EmployeeTypeId;
                        obj.Gang = param.Gang;
                        obj.ModifiedDate = DateTime.UtcNow;
						//obj.ModifiedBy = 1;
						dBContext.Employees.Update(obj);
						await dBContext.SaveChangesAsync();
					}

					return param;
				}
				else
				{
					param.RecordStatus = "Active";
					//param.CreatedBy = 1;
					param.CreatedDate = DateTime.UtcNow;
					dBContext.Employees.Add(param);
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
				var obj = dBContext.Employees.FirstOrDefault(x => x.Id == Id);
				if (obj != null)
				{
					obj.RecordStatus = "Deleted";
					dBContext.Employees.Update(obj);
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
        public async Task<string> InsertModuleRole(List<RoleModule> param)
        {
            try
            {
				var objdel=await dBContext.RoleModules.Where(x => x.RoleId == param[0].RoleId).ToListAsync();
				if (objdel != null)
				{
					 dBContext.RoleModules.RemoveRange(objdel);
				}
				if(param.Count > 0)
				{
					
                    await dBContext.RoleModules.AddRangeAsync(param);
					
                }

                await dBContext.SaveChangesAsync();


                return "Success";//  return param;
                
            }
            catch (Exception ex)
            {

                return ex.Message;
            }

        }
        public async Task<string> InsertRole(Role param)
        {
            try
            {
                var objdel = await dBContext.Roles.Where(x => x.Id == param.Id).FirstOrDefaultAsync();
                if (objdel != null)
                {
                    dBContext.Roles.Update(objdel);
                }
                else
                {
                    dBContext.Roles.Add(param);
                }

                await dBContext.SaveChangesAsync();


                return "Success";//  return param;

            }
            catch (Exception ex)
            {

                return ex.Message;
            }

        }
        public async Task<string> InsertAppUser(AppUser param)
        {
            try
            {
                var objDeafult=await dBContext.AppUsers.Where(x => x.UserId==param.UserId).FirstOrDefaultAsync();
                if (objDeafult == null)
                {
                    await dBContext.AppUsers.AddAsync(param);
                    await dBContext.SaveChangesAsync();

                    var objuserl = new UserLog()
                    {
                        ActionType = "Insert",
                        AppName = "RoleLog",
                        UserId = param.UserId,
                        CreatedDate = DateTime.Now,
                        ShiftId=param.Id,
                        Description = objDeafult.FullName + ": Added "
					};
					await dBContext.UserLogs.AddAsync(objuserl);
					await dBContext.SaveChangesAsync();
				}
                else
                {
					objDeafult.FullName=param.FullName;
				 dBContext.AppUsers.Update(objDeafult);
					await dBContext.SaveChangesAsync();

					

				}

				return "success";
            }
            catch (Exception ex)
            {

                return ex.Message;
            }

        }
        public async Task<string> UpdateRole(AppUser param,string UserId)
        {
            try
            {
                var objDeafult = await dBContext.AppUsers.Where(x => x.Id == param.Id).FirstOrDefaultAsync();
                if (objDeafult != null)
                {
                    objDeafult.RoleId = param.RoleId;
                    dBContext.AppUsers.Update(objDeafult);
                    await dBContext.SaveChangesAsync();


                    var roleobj=await dBContext.Roles.Where(x => x.Id == param.RoleId).FirstOrDefaultAsync();
					var objuserl = new UserLog()
					{
						ActionType = "Update",
						AppName = "RoleLog",
						UserId = UserId,
						ShiftId = param.Id,
						CreatedDate = DateTime.Now,
						Description = objDeafult.FullName+ ": Role changed to "+ roleobj?.Name
					};
					await dBContext.UserLogs.AddAsync(objuserl);
					await dBContext.SaveChangesAsync();
				}

                return "success";
            }
            catch (Exception ex)
            {

                return ex.Message;
            }

        }
        public async Task<List<AppUserViewModel>> GetAppUsers(int RoleId)
        {


            List<AppUserViewModel> list = new List<AppUserViewModel>();

            using (SqlConnection con = new SqlConnection(configuration.GetConnectionString("ConnStr")))
            {
                SqlCommand cmd = new SqlCommand("SpGetAppUsers", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RoleId", RoleId);
                con.Open();
                SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                while (rdr.Read())
                {
                    var cvs = new AppUserViewModel()
                    {
                        FullName = string.IsNullOrEmpty(Convert.ToString(rdr["FullName"]))? Convert.ToString(rdr["EmailAddress"]): Convert.ToString(rdr["FullName"]),
                        EmailAddress = Convert.ToString(rdr["EmailAddress"]),
                        CreatedDateString = Convert.ToString(rdr["CreatedDateString"]),
						ModifiedDateString = Convert.ToString(rdr["ModifiedDateString"]),
						RecordStatus = Convert.ToString(rdr["RecordStatus"]),
                        Id = Convert.ToInt32(rdr["Id"]),
						RoleId= Convert.ToInt32(rdr["RoleId"]),
                        RoleName = Convert.ToString(rdr["RoleName"]),
                        UserId = Convert.ToString(rdr["UserId"]),
                    };

                    list.Add(cvs);
                }
                con.Close();
            }

            return list;
        }

        public async Task<List<UserLogModel>> GetUserLog(string UserId, int ShiftId)
        {


            List<UserLogModel> list = new List<UserLogModel>();

            using (SqlConnection con = new SqlConnection(configuration.GetConnectionString("ConnStr")))
            {
                SqlCommand cmd = new SqlCommand("SpGetUserLog", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@ShiftId", ShiftId);
                con.Open();
                SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                while (rdr.Read())
                {
                    var cvs = new UserLogModel()
                    {
                        FullName = string.IsNullOrEmpty(Convert.ToString(rdr["FullName"])) ? Convert.ToString(rdr["EmailAddress"]) : Convert.ToString(rdr["FullName"]),
                        EmailAddress = Convert.ToString(rdr["EmailAddress"]),
                        CreatedDateString = Convert.ToDateTime(rdr["CreatedDateString"]).ToString("dd-MM-yyyy HH:mm"),
                        RecordStatus = Convert.ToString(rdr["RecordStatus"]),
                        Id = Convert.ToInt32(rdr["Id"]),
                        AppName = Convert.ToString(rdr["AppName"]),
                        ActionType = Convert.ToString(rdr["ActionType"]),
                        UserId = Convert.ToString(rdr["UserId"]),
						Description = Convert.ToString(rdr["Description"]),
					};

                    list.Add(cvs);
                }
                con.Close();
            }

            return list;
        }

		public async Task<List<UserLogModel>> GetUserLogForRole(string UserId, int  UserIdFrom)
		{


			List<UserLogModel> list = new List<UserLogModel>();

			using (SqlConnection con = new SqlConnection(configuration.GetConnectionString("ConnStr")))
			{
				SqlCommand cmd = new SqlCommand("SpGetUserLog", con);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@UserId", UserId);
				cmd.Parameters.AddWithValue("@ShiftId", UserIdFrom);
				con.Open();
				SqlDataReader rdr = await cmd.ExecuteReaderAsync();

				while (rdr.Read())
				{
					var cvs = new UserLogModel()
					{
						FullName = string.IsNullOrEmpty(Convert.ToString(rdr["FullName"])) ? Convert.ToString(rdr["EmailAddress"]) : Convert.ToString(rdr["FullName"]),
						EmailAddress = Convert.ToString(rdr["EmailAddress"]),
						CreatedDateString = Convert.ToDateTime(rdr["CreatedDateString"]).ToString("dd-MM-yyyy"),
						RecordStatus = Convert.ToString(rdr["RecordStatus"]),
						Id = Convert.ToInt32(rdr["Id"]),
						AppName = Convert.ToString(rdr["AppName"]),
						ActionType = Convert.ToString(rdr["ActionType"]),
						UserId = Convert.ToString(rdr["UserId"]),
						Description = Convert.ToString(rdr["Description"]),
					};

					list.Add(cvs);
				}
				con.Close();
			}

			return list;
		}
	}
}
