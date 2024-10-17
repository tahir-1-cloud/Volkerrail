using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;
using PRISM.DTO;
using PRISM.Models;
using PRISM.Services.Interfaces;
using System;
using System.Globalization;
using System.Linq;

namespace PRISM.Services
{
    public class LookupServices : ILookupServices
    {
        private readonly PRISMContext dBContext;

        public LookupServices(PRISMContext _dbContext)
        {
            dBContext = _dbContext;
        }

        public async Task<List<LookupEntity>> GetLookups(List<string> type)
        {
            if (type.Count > 0)
            {
                var list = await dBContext.LookupEntities.Where(x => type.Contains(x.LookupType)).ToListAsync();
                return list;
            }
            else
            {
                var list = await dBContext.LookupEntities.ToListAsync();
                return list;
            }
        }
        public async Task<List<Role>> GetRoles()
        {

            var list = await dBContext.Roles.ToListAsync();
            return list;
        }
        public async Task<LookupEntity> Insert(LookupEntity param)
        {
            try
            {
                if (param.LookupType.ToLower() == "roles")
                {
                    if (param.Id > 0)
                    {
                        var obj = await dBContext.Roles.FindAsync(param.Id);
                        if (obj != null)
                        {
                            obj.Name = param.Name;
                            obj.Description = param.Description;
                            dBContext.Roles.Update(obj);
                            await dBContext.SaveChangesAsync();
                        }

                        return param;
                    }
                    else
                    {
                        var paramsd = new Role();
                        paramsd.Name = param.Name;
                        paramsd.Description = param.Description;
                        dBContext.Roles.Add(paramsd);
                        await dBContext.SaveChangesAsync();

                        return param;
                    }
                }
                else
                {
                    if (param.Id > 0)
                    {
                        var obj = await dBContext.LookupEntities.FindAsync(param.Id);
                        if (obj != null)
                        {
                            obj.Name = param.Name;
                            obj.Description = param.Description;
                            dBContext.LookupEntities.Update(obj);
                            await dBContext.SaveChangesAsync();
                        }

                        return param;
                    }
                    else
                    {
                        param.Name = param.Name;
                        param.Description = param.Description;
                        param.LookupType = param.LookupType;
                        dBContext.LookupEntities.Add(param);
                        await dBContext.SaveChangesAsync();

                        return param;
                    }
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
                var obj = dBContext.LookupEntities.FirstOrDefault(x => x.Id == Id);
                if (obj != null)
                {
                    dBContext.LookupEntities.Remove(obj);
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
        public async Task<List<Week>> GetWeeks()
        {
            var list = await dBContext.Weeks.ToListAsync();
            return list;
        }
        public async Task<List<ShiftTemplate>> GetTemplateData()
        {
            var list = await dBContext.ShiftTemplates.ToListAsync();
            return list;
        }
        public async Task<List<ShiftTemplate>> GetTemplates()
        {
            var list = await dBContext.ShiftTemplates.ToListAsync();
            return list;
        }
        public async Task<Week> Insert(WeeksModel param)
        {
            try
            {
                Week obj = new Week();
                if (!string.IsNullOrEmpty(param.StartDate))
                {
                    if (DateTime.TryParseExact(param.StartDate + " 00:00", "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                    {
                        obj.StartDate = result;
                    }

                }
                if (!string.IsNullOrEmpty(param.EndDate))
                {
                    if (DateTime.TryParseExact(param.EndDate + " 23:59", "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                    {
                        obj.EndDate = result;
                    }

                }

                if (param.Id > 0)
                {
                    obj = await dBContext.Weeks.FindAsync(param.Id);
                    if (obj != null)
                    {
                        obj.Title = param.Title;
                        dBContext.Weeks.Update(obj);
                        await dBContext.SaveChangesAsync();
                    }

                    return obj;
                }
                else
                {
                    var weeknumbers = GetWeeks(Convert.ToDateTime(obj.StartDate), Convert.ToDateTime(obj.EndDate));
                    int count = 1;
                    foreach (var w in weeknumbers)
                    {
                        var newparam = new Week()
                        {
                            Title = obj.StartDate.Value.Year + " " + param.Title + " " + count,
                            StartDate = w.Item1,
                            EndDate = w.Item2,
                            WeekNo = Convert.ToInt32(Convert.ToDateTime(obj.StartDate).Year % 10 + "" + (count < 10 ? "0" + count : count))
                        };
                        dBContext.Weeks.Add(newparam);
                        await dBContext.SaveChangesAsync();

                        try
                        {
                            DateTime StartDatetime = DateTime.Now;

                            DateTime EndDateTime = DateTime.Now;
                            if (DateTime.TryParseExact(w.Item1.ToString("dd-MM-yyyy") + " 00:00", "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startresult))
                            {
                                StartDatetime = startresult;
                            }


                            //if (DateTime.TryParseExact(w.Item2.ToString("dd-MM-yyyy") + " 23:59", "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endresult))
                            //{
                            //    EndDateTime = endresult;
                            //}
                            long maxId = 0;
                            var maxObj = dBContext.ShiftNumbers.OrderByDescending(x => x.Id).FirstOrDefault();
                            if (maxObj != null)
                            {
                                maxId = maxObj.Id;
                            }
                            for (int i = 0; i < 14; i++)
                            {
                                maxId = maxId + 1;
                                var _startdatetime = StartDatetime.AddHours(12 * i);
                                var _enddatetime = _startdatetime.AddHours(11.99);
                                var shiftNumberObj = new ShiftNumber()
                                {
                                    Id = maxId,
                                    ShiftNo = i + 1,
                                    StartShiftDateTime = _startdatetime,
                                    EndShiftDateTime = _enddatetime,
                                    WeekNo = newparam.WeekNo
                                };

                                dBContext.ShiftNumbers.Add(shiftNumberObj);
                                dBContext.SaveChanges();
                            }

                            //var ShiftNumberWithDatetime = GenerateData(StartDatetime, EndDateTime, 14);
                            //if (ShiftNumberWithDatetime?.Count > 0)
                            //{

                            //    foreach (var shiftno in ShiftNumberWithDatetime)
                            //    {
                            //        var shiftNumberObj = new ShiftNumber()
                            //        {
                            //            ShiftNo = shiftno.Number,
                            //            DateTime = shiftno.DateTime
                            //        };

                            //        dBContext.ShiftNumbers.Add(shiftNumberObj);
                            //        await dBContext.SaveChangesAsync();
                            //    }

                            //}
                        }
                        catch (Exception ex)
                        {

                            throw;
                        }


                        count++;
                    }

                    return obj;
                }
            }
            catch (Exception ex)
            {

                return null;
            }

        }
        static List<(DateTime DateTime, int Number)> GenerateData(DateTime startDate, DateTime endDate, int numberOfPoints)
        {
            List<(DateTime DateTime, int Number)> data = new List<(DateTime DateTime, int Number)>();

            TimeSpan span = endDate - startDate;
            double daysBetween = span.TotalDays;

            for (int i = 0; i < numberOfPoints; i++)
            {
                double portionOfDay = i / (double)(numberOfPoints - 1);
                double daysToAdd = portionOfDay * daysBetween;
                DateTime currentDate = startDate.AddDays(daysToAdd);
                int currentNumber = i + 1; // Numbers start from 1
                data.Add((currentDate, currentNumber));
            }

            return data;
        }
        static List<(DateTime, DateTime)> GetWeeks(DateTime fromDate, DateTime toDate)
        {
            //List<(DateTime, DateTime)> weeks = new List<(DateTime, DateTime)>();

            //DateTime startOfWeek = fromDate.AddDays(-(int)fromDate.DayOfWeek);
            //DateTime endOfWeek = startOfWeek.AddDays(6);

            //while (startOfWeek <= toDate)
            //{
            //    if (endOfWeek > toDate)
            //    {
            //        endOfWeek = toDate;
            //    }
            //    weeks.Add((startOfWeek, endOfWeek));
            //    startOfWeek = startOfWeek.AddDays(7);
            //    endOfWeek = startOfWeek.AddDays(6);
            //}

            //return weeks;

            List<(DateTime, DateTime)> weeks = new List<(DateTime, DateTime)>();

            // Set the start of the week to the first Saturday within the specified range
            DateTime startOfWeek = fromDate.AddDays(-(int)fromDate.DayOfWeek);
            startOfWeek = startOfWeek.AddDays((int)DayOfWeek.Saturday);

            DateTime endOfWeek = startOfWeek.AddDays(6);

            while (startOfWeek <= toDate)
            {
                if (endOfWeek > toDate)
                {
                    endOfWeek = toDate;
                }
                weeks.Add((startOfWeek, endOfWeek));
                startOfWeek = startOfWeek.AddDays(7);
                endOfWeek = startOfWeek.AddDays(6);
            }

            return weeks;
        }
        public async Task<bool> deleteWeek(int Id)
        {
            try
            {
                var obj = dBContext.Weeks.FirstOrDefault(x => x.Id == Id);
                if (obj != null)
                {
                    dBContext.Weeks.Remove(obj);
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
        public async Task<List<LeaveType>> GetLeaves()
        {
            var list = await dBContext.LeaveTypes.ToListAsync();
            return list;
        }

        public async Task<LeaveType> Insert(LeaveType param)
        {
            try
            {
                if (param.Id > 0)
                {
                    var obj = await dBContext.LeaveTypes.FindAsync(param.Id);
                    if (obj != null)
                    {
                        obj.Title = param.Title;
                        obj.ColorCode = param.ColorCode;
                        dBContext.LeaveTypes.Update(obj);
                        await dBContext.SaveChangesAsync();
                    }

                    return param;
                }
                else
                {
                    param.Title = param.Title;
                    param.ColorCode = param.ColorCode;
                    dBContext.LeaveTypes.Add(param);
                    await dBContext.SaveChangesAsync();

                    return param;
                }
            }
            catch (Exception ex)
            {

                return null;
            }

        }
        public async Task<bool> deleteLeave(int Id)
        {
            try
            {
                var obj = dBContext.LeaveTypes.FirstOrDefault(x => x.Id == Id);
                if (obj != null)
                {
                    dBContext.LeaveTypes.Remove(obj);
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
        public async Task<ShiftTemplate> InsertTemplate(ShiftTemplate param)
        {

            if (param.Id > 0)
            {
                var obj = await dBContext.ShiftTemplates.FindAsync(param.Id);
                if (obj != null)
                {
                    obj.TemplateName = param.TemplateName;
                    obj.Columns = param.Columns;
                    obj.UserId = param.UserId;
                    dBContext.ShiftTemplates.Update(obj);
                    await dBContext.SaveChangesAsync();
                }

                return param;
            }
            else
            {
                dBContext.ShiftTemplates.Add(param);
                await dBContext.SaveChangesAsync();

                return param;
            }


        }
        public async Task<bool> DeleteTemplate(int Id)
        {
            try
            {
                var obj = dBContext.ShiftTemplates.FirstOrDefault(x => x.Id == Id);
                if (obj != null)
                {
                    dBContext.ShiftTemplates.Remove(obj);
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
