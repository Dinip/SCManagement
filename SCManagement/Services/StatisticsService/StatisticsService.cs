using System.Data;
using Microsoft.EntityFrameworkCore;
using SCManagement.Data;
using SCManagement.Models;
using SCManagement.Services.ClubService;
using SCManagement.Services.PaymentService.Models;
using SCManagement.Services.PlansService.Models;
using SCManagement.Services.StatisticsService.Models;

namespace SCManagement.Services.StatisticsService
{
    public class StatisticsService : IStatisticsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IClubService _clubService;
        private readonly IStringLocalizer<SharedResource> _stringLocalizer;

        public StatisticsService(
            ApplicationDbContext context,
            IClubService clubService,
            IStringLocalizer<SharedResource> stringLocalizer
            )
        {
            _context = context;
            _clubService = clubService;
            _stringLocalizer = stringLocalizer;
        }

        /// <summary>
        /// Creates statistics about users (partners and athletes) from a specified club.
        /// Only creates monthly stats and can be run once a day. Updates the existing stats if they exist
        /// with the new values.
        /// </summary>
        /// <param name="clubId"></param>
        /// <returns></returns>
        public async Task CreateClubUserStatistics(int clubId)
        {
            List<int> filterIds = new() { 10, 20 };
            var prevDay = DateTime.Now.Date.AddDays(-1);
            var result = await _context
                .ClubUserStatistics
                .Where(f => f.Timestamp.Month == prevDay.Month && f.Timestamp.Year == prevDay.Year && f.ClubId == clubId)
                .ToListAsync();

            var userCountByRole = await _context
                .UsersRoleClub
                .Where(f => f.ClubId == clubId && filterIds.Contains(f.RoleId))
                .GroupBy(f => f.RoleId)
                .Select(f => new
                {
                    RoleId = f.Key,
                    Count = f.Count()
                })
                .ToListAsync();

            filterIds.ForEach(id =>
            {
                var f = result.Find(r => r.RoleId == id);
                if (f != null)
                {
                    f.Value = userCountByRole.FirstOrDefault(s => s.RoleId == id)?.Count ?? 0;
                    _context.ClubUserStatistics.Update(f);
                }
                else
                {
                    var stat = new ClubUserStatistics
                    {
                        Value = userCountByRole.FirstOrDefault(s => s.RoleId == id)?.Count ?? 0,
                        RoleId = id,
                        ClubId = clubId,
                        StatisticsRange = StatisticsRange.Month,
                        Timestamp = new DateTime(prevDay.Year, prevDay.Month, DateTime.DaysInMonth(prevDay.Year, prevDay.Month)),
                    };
                    _context.ClubUserStatistics.Add(stat);
                }
            });

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Creates statistics about payments (events and memberships) from a specified club.
        /// Only creates monthly stats and MUST be only run once a day. Gets the payments received from
        /// the previous day and updates (sum) the existing stats if they exist or creates an new one.
        /// </summary>
        /// <param name="clubId"></param>
        /// <returns></returns>
        public async Task CreateClubPaymentStatistics(int clubId)
        {
            var prevDay = DateTime.Now.Date.AddDays(-1);
            var result = await _context
                .ClubPaymentStatistics
                .Where(f => f.Timestamp.Month == prevDay.Month && f.Timestamp.Year == prevDay.Year && f.ClubId == clubId)
                .ToListAsync();

            var sumValueByProduct = await _context
                .Product
                .Join(
                    _context.Payment,
                    product => product.Id,
                    payment => payment.ProductId,
                    (product, payment) => new { product, payment }
                )
                .Where(
                    f => f.product.ClubId == clubId &&
                    f.payment.PayedAt.Value.Date == prevDay // get the payed payments from last day
                    )
                .GroupBy(f => f.product.Id)
                .Select(f => new
                {
                    ProductId = f.Key,
                    Total = f.Sum(s => s.product.Value),
                    ProductType = f.Select(s => s.product.ProductType).First()
                })
                .ToListAsync();

            sumValueByProduct.ForEach(prod =>
            {
                var f = result.Find(r => r.ProductId == prod.ProductId);
                if (f != null)
                {
                    f.Value += prod.Total;
                    _context.ClubPaymentStatistics.Update(f);
                }
                else if (prod.Total > 0)
                {
                    var stat = new ClubPaymentStatistics
                    {
                        Value = prod.Total,
                        ProductId = prod.ProductId,
                        ClubId = clubId,
                        StatisticsRange = StatisticsRange.Month,
                        ProductType = prod.ProductType,
                        Timestamp = new DateTime(prevDay.Year, prevDay.Month, DateTime.DaysInMonth(prevDay.Year, prevDay.Month)),
                    };
                    _context.ClubPaymentStatistics.Add(stat);
                }
            });

            var sumValueByType = sumValueByProduct
                .GroupBy(f => f.ProductType)
                .Select(f => new
                {
                    ProductType = f.Key,
                    Total = f.Sum(s => s.Total)
                })
                .ToList();

            sumValueByType.ForEach(prod =>
            {
                var f = result.Find(r => r.ProductId == null && r.ProductType == prod.ProductType);
                if (f != null)
                {
                    f.Value += prod.Total;
                    _context.ClubPaymentStatistics.Update(f);
                }
                else if (prod.Total > 0)
                {
                    var stat = new ClubPaymentStatistics
                    {
                        Value = prod.Total,
                        ClubId = clubId,
                        StatisticsRange = StatisticsRange.Month,
                        ProductType = prod.ProductType,
                        Timestamp = new DateTime(prevDay.Year, prevDay.Month, DateTime.DaysInMonth(prevDay.Year, prevDay.Month)),
                    };
                    _context.ClubPaymentStatistics.Add(stat);
                }
            });

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Creates statistics about modalities from a specified club.
        /// Only creates monthly stats and can be run once a day. Updates the existing stats if they exist
        /// with the new values.
        /// </summary>
        /// <param name="clubId"></param>
        /// <returns></returns>
        public async Task CreateClubModalityStatistics(int clubId)
        {
            var prevDay = DateTime.Now.Date.AddDays(-1);
            var result = await _context
                .ClubModalityStatistics
                .Where(f => f.Timestamp.Month == prevDay.Month && f.Timestamp.Year == prevDay.Year && f.ClubId == clubId)
                .ToListAsync();

            var groupByModality = await _context
                .Team
                .Include(f => f.Athletes)
                .Where(f => f.ClubId == clubId)
                .GroupBy(f => f.ModalityId)
                .Select(f => new
                {
                    ModalityId = f.Key,
                    Count = f.Count()
                })
                .ToListAsync();

            groupByModality.ForEach(modality =>
            {
                var f = result.Find(r => r.ModalityId == modality.ModalityId);
                if (f != null)
                {
                    f.Value = modality.Count;
                    _context.ClubModalityStatistics.Update(f);
                }
                else
                {
                    var stat = new ClubModalityStatistics
                    {
                        Value = modality.Count,
                        ModalityId = modality.ModalityId,
                        ClubId = clubId,
                        StatisticsRange = StatisticsRange.Month,
                        Timestamp = new DateTime(prevDay.Year, prevDay.Month, DateTime.DaysInMonth(prevDay.Year, prevDay.Month)),
                    };
                    _context.ClubModalityStatistics.Add(stat);
                }
            });

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Gets the current user stats (number of athletes / total slots, staff, active partners / all partners)
        /// for a specified club. Returns realtime data.
        /// </summary>
        /// <param name="clubId"></param>
        /// <returns></returns>
        public async Task<ICollection<ClubCurrentUsers>> GetCurrentClubUsersStatistics(int clubId)
        {
            List<ClubCurrentUsers> users = new();
            var slots = await _clubService.ClubAthleteSlots(clubId);
            users.Add(new ClubCurrentUsers
            {
                RoleName = _stringLocalizer["Athletes"].ToString(),
                Value = slots.UsedSlots,
                MaxValue = slots.TotalSlots
            });

            var staff = await _context
                .UsersRoleClub
                .Where(u => u.ClubId == clubId && (u.RoleId == 30 || u.RoleId == 40))
                .GroupBy(r => r.ClubId)
                .Select(r =>
                new ClubCurrentUsers
                {
                    RoleName = _stringLocalizer["Staff"].ToString(),
                    Value = r.Count()
                })
                .FirstOrDefaultAsync() ?? new ClubCurrentUsers { RoleName = _stringLocalizer["Staff"].ToString(), Value = 0 };

            users.Add(staff);

            var partnersValues = await _context
                .UsersRoleClub
                .Where(u => u.ClubId == clubId && u.RoleId == 10)
                .GroupBy(r => r.Status)
                .Select(r => new
                {
                    Status = r.Key,
                    Value = r.Count(),
                })
                .ToListAsync();


            var partners = new ClubCurrentUsers
            {
                RoleName = _stringLocalizer["Partners"].ToString(),
                Value = partnersValues.Count != 0 ? partnersValues?.First(p => p.Status == UserRoleStatus.Active)?.Value ?? 0 : 0,
                MaxValue = partnersValues.Count != 0 ? partnersValues?.Sum(p => p.Value) ?? 0 : 0
            };

            users.Add(partners);

            return users;
        }

        /// <summary>
        /// Gets the payment statistics for a specified club. If year is not specified, uses current year.
        /// Returns 12 entries (1 for each month) with the respective data.
        /// </summary>
        /// <param name="clubId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public async Task<ICollection<ClubPaymentStatistics>> GetClubPaymentStatistics(int clubId, int? year = null, int? month = null)
        {
            year ??= DateTime.Now.Year;

            if (month != null && month > 0 && month < 13)
            {
                return await _context
                    .ClubPaymentStatistics
                    .Where(c =>
                        c.Timestamp.Year == year &&
                        c.Timestamp.Month == month &&
                        c.ClubId == clubId &&
                        c.StatisticsRange == StatisticsRange.Day &&
                        c.ProductId == null)
                    .ToListAsync();
            }
            else
            {
                return await _context
                    .ClubPaymentStatistics
                    .Where(c =>
                        c.Timestamp.Year == year &&
                        c.ClubId == clubId &&
                        c.StatisticsRange == StatisticsRange.Month &&
                        c.ProductId == null)
                    .ToListAsync();
            }
        }

        /// <summary>
        /// Gets the payment details statistics for a specified club. If year is not specified, uses current year.
        /// Returns X entries for each month corresponding to each product that was "sold" that month.
        /// </summary>
        /// <param name="clubId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public async Task<ICollection<ClubPaymentStatistics>> GetClubPaymentDetailsStatistics(int clubId, int? year = null, int? month = null)
        {
            if (year == null) year = DateTime.Now.Year;

            if (month != null && month > 0 && month < 13)
            {
                return await _context
                    .ClubPaymentStatistics
                    .Include(p => p.Product)
                    .Where(c =>
                        c.Timestamp.Year == year &&
                        c.Timestamp.Month == month &&
                        c.ClubId == clubId &&
                        c.StatisticsRange == StatisticsRange.Day &&
                        c.ProductId != null)
                    .ToListAsync();
            }
            else
            {
                return await _context
                    .ClubPaymentStatistics
                    .Include(p => p.Product)
                    .Where(c =>
                        c.Timestamp.Year == year &&
                        c.ClubId == clubId &&
                        c.StatisticsRange == StatisticsRange.Month &&
                        c.ProductId != null)
                    .ToListAsync();
            }
        }

        /// <summary>
        /// Gets the user statistics for a specified club. If year is not specified, uses current year.
        /// User type must be specified (id from database). Mostly used to get the number of athletes 
        /// and partners.
        /// </summary>
        /// <param name="clubId"></param>
        /// <param name="userTypeId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public async Task<ICollection<ClubUserStatistics>> GetClubUserStatistics(int clubId, int userTypeId, int? year = null, int? month = null)
        {
            if (year == null) year = DateTime.Now.Year;

            if (month != null && month > 0 && month < 13)
            {
                return await _context
                    .ClubUserStatistics
                    .Where(c =>
                        c.Timestamp.Year == year &&
                        c.Timestamp.Month == month &&
                        c.ClubId == clubId &&
                        c.StatisticsRange == StatisticsRange.Day &&
                        c.RoleId == userTypeId)
                    .ToListAsync();
            }
            else
            {
                return await _context
                    .ClubUserStatistics
                    .Where(c =>
                        c.Timestamp.Year == year &&
                        c.ClubId == clubId &&
                        c.StatisticsRange == StatisticsRange.Month &&
                        c.RoleId == userTypeId)
                    .ToListAsync();
            }
        }

        /// <summary>
        /// Gets the modality statistics for a specified club. If year is not specified, uses current year.
        /// </summary>
        /// <param name="clubId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public async Task<ICollection<ClubModalityStatistics>> GetClubModalityStatistics(int clubId, int? year = null, int? month = null)
        {
            if (year == null) year = DateTime.Now.Year;

            if (month != null && month > 0 && month < 13)
            {
                return await _context
                    .ClubModalityStatistics
                    .Include(c => c.Modality)
                    .Where(c =>
                        c.Timestamp.Year == year &&
                        c.Timestamp.Month == month &&
                        c.ClubId == clubId &&
                        c.StatisticsRange == StatisticsRange.Day)
                    .ToListAsync();
            }
            else
            {
                return await _context
                    .ClubModalityStatistics
                    .Include(c => c.Modality)
                    .Where(c =>
                        c.Timestamp.Year == year &&
                        c.ClubId == clubId &&
                        c.StatisticsRange == StatisticsRange.Month)
                    .ToListAsync();
            }
        }

        public async Task CreateSystemPaymentStatistics()
        {
            var prevDay = DateTime.Now.Date.AddDays(-1);
            var result = await _context
                .SystemPaymentStatistics
                .Where(f => f.Timestamp.Month == prevDay.Month && f.Timestamp.Year == prevDay.Year)
                .ToListAsync();

            var sumValueByProduct = await _context
                .Product
                .Join(
                    _context.Payment,
                    product => product.Id,
                    payment => payment.ProductId,
                    (product, payment) => new { product, payment }
                )
                .Where(
                    f => f.product.ProductType == PaymentService.Models.ProductType.ClubSubscription &&
                    f.payment.PayedAt.Value.Date == prevDay // get the payed payments from last day
                    )
                .GroupBy(f => f.product.Id)
                .Select(f => new
                {
                    ProductId = f.Key,
                    Total = f.Sum(s => s.product.Value),
                    ProductType = f.Select(s => s.product.ProductType).First()
                })
                .ToListAsync();

            sumValueByProduct.ForEach(prod =>
            {
                var f = result.Find(r => r.ProductId == prod.ProductId);
                if (f != null)
                {
                    f.Value += prod.Total;
                    _context.SystemPaymentStatistics.Update(f);
                }
                else if (prod.Total > 0)
                {
                    var stat = new SystemPaymentStatistics
                    {
                        Value = prod.Total,
                        ProductId = prod.ProductId,
                        StatisticsRange = StatisticsRange.Month,
                        ProductType = prod.ProductType,
                        Timestamp = new DateTime(prevDay.Year, prevDay.Month, DateTime.DaysInMonth(prevDay.Year, prevDay.Month)),
                    };
                    _context.SystemPaymentStatistics.Add(stat);
                }
            });

            await _context.SaveChangesAsync();
        }

        public async Task CreateSystemPlansStatistics()
        {
            var prevDay = DateTime.Now.Date.AddDays(-1);

            var plansIds = await _context
                .Product
                .Where(f => f.ProductType == PaymentService.Models.ProductType.ClubSubscription)
                .Select(f => f.Id)
                .ToListAsync();

            var prevDayMonthResults = await _context
                    .SystemPlansStatistics
                    .Where(f => f.Timestamp.Month == prevDay.Month && f.Timestamp.Year == prevDay.Year)
                    .ToListAsync();

            foreach (var id in plansIds)
            {
                //new planId that is not present on prevDayMonthResults
                //(created yesterday so it wasn't "found" on that month or new month start)
                if (!prevDayMonthResults.Any(r => r.ProductId == id))
                {
                    _context.SystemPlansStatistics.Add(new SystemPlansStatistics
                    {
                        ProductId = id,
                        StatisticsRange = StatisticsRange.Month,
                        Timestamp = new DateTime(prevDay.Year, prevDay.Month, DateTime.DaysInMonth(prevDay.Year, prevDay.Month)),
                        Active = 0
                    });
                }
            }

            await _context.SaveChangesAsync();

            //get all plan subs that expired yesterday
            var endedSubsYesterday = await _context
                    .Subscription
                    .Where(f => plansIds.Contains(f.ProductId) && f.EndTime.Value.Date == prevDay)
                    .GroupBy(f => f.ProductId)
                    .Select(f => new { Id = f.Key, Canceled = f.Count() })
                    .ToListAsync();

            //get all plan subs that started yesterday
            var startedSubsYesterday = await _context
                .Subscription
                .Where(f => plansIds.Contains(f.ProductId) && f.StartTime.Date == prevDay)
                .GroupBy(f => f.ProductId)
                .Select(f => new { Id = f.Key, Canceled = f.Count() })
                .ToListAsync();

            //if plans were started or canceled yesterday, get the old values and update them
            if (endedSubsYesterday.Any() || startedSubsYesterday.Any())
            {
                var prevDayMonthResultsNew = await _context
                    .SystemPlansStatistics
                    .Where(f => f.Timestamp.Month == prevDay.Month && f.Timestamp.Year == prevDay.Year)
                    .ToListAsync();

                prevDayMonthResultsNew.ForEach(plan =>
                {
                    //add count of new active plans from previous day (is assumes that was created before)
                    var startedPlans = startedSubsYesterday.Where(e => e.Id == plan.ProductId);
                    if (startedPlans.Any())
                    {
                        plan.Active += startedPlans.Count();
                    }

                    //subtract from active and add to canceled (is assumes that was created before)
                    var endedPlans = endedSubsYesterday.Where(e => e.Id == plan.ProductId);
                    if (endedPlans.Any())
                    {
                        plan.Active -= endedPlans.Count();
                        plan.Canceled += endedPlans.Count();
                    }

                    _context.SystemPlansStatistics.Update(plan);
                });
            }
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<SystemPaymentStatistics>> GetSystemPaymentStatistics(int? year = null)
        {
            year ??= DateTime.Now.Year;

            return await _context
                .SystemPaymentStatistics
                .Where(c =>
                    c.Timestamp.Year == year &&
                    c.StatisticsRange == StatisticsRange.Month)
                .ToListAsync();
        }

        public async Task<ICollection<SystemPlansStatistics>> GetSystemPlansStatistics(int? year = null)
        {
            year ??= DateTime.Now.Year;
            return await _context
                .SystemPlansStatistics
                .Where(c =>
                    c.Timestamp.Year == year &&
                    c.StatisticsRange == StatisticsRange.Month)
                .ToListAsync();
        }

        public async Task<ICollection<SystemPlansShortStatistics>> GetSystemPlansShortStatistics()
        {
            var plansIds = await _context
                .Product
                .Where(f => f.ProductType == ProductType.ClubSubscription)
                .Select(f => f.Id)
                .ToListAsync();

            var stats = await _context
                .Subscription
                .Include(f => f.Product)
                .Where(f => plansIds.Contains(f.ProductId))
                .GroupBy(f => new { f.ProductId, f.Product.Name })
                .Select(f => new SystemPlansShortStatistics
                {
                    Id = f.Key.ProductId,
                    Count = f.Count(),
                    Name = f.Key.Name
                })
                .ToListAsync();

            return stats;
        }
    }
}
