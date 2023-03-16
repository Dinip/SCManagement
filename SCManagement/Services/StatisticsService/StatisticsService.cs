using System.Data;
using Microsoft.EntityFrameworkCore;
using SCManagement.Data;
using SCManagement.Models;
using SCManagement.Services.ClubService;
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

                //daily stats
                var daily = new ClubUserStatistics
                {
                    Value = userCountByRole.FirstOrDefault(s => s.RoleId == id)?.Count ?? 0,
                    RoleId = id,
                    ClubId = clubId,
                    StatisticsRange = StatisticsRange.Day,
                    Timestamp = prevDay
                };
                _context.ClubUserStatistics.Add(daily);
            });

            await _context.SaveChangesAsync();
        }

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

                //daily stats
                var daily = new ClubPaymentStatistics
                {
                    Value = prod.Total,
                    ProductId = prod.ProductId,
                    ClubId = clubId,
                    StatisticsRange = StatisticsRange.Day,
                    ProductType = prod.ProductType,
                    Timestamp = prevDay
                };
                _context.ClubPaymentStatistics.Add(daily);
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

                //daily stats
                var daily = new ClubPaymentStatistics
                {
                    Value = prod.Total,
                    ClubId = clubId,
                    StatisticsRange = StatisticsRange.Day,
                    ProductType = prod.ProductType,
                    Timestamp = prevDay
                };
                _context.ClubPaymentStatistics.Add(daily);
            });

            await _context.SaveChangesAsync();
        }

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

                //daily stats
                var daily = new ClubModalityStatistics
                {
                    Value = modality.Count,
                    ModalityId = modality.ModalityId,
                    ClubId = clubId,
                    StatisticsRange = StatisticsRange.Day,
                    Timestamp = prevDay
                };

                _context.ClubModalityStatistics.Add(daily);
            });
            await _context.SaveChangesAsync();
        }

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

        public async Task<ICollection<ClubPaymentStatistics>> GetClubPaymentStatistics(int clubId, int? year = null, int? month = null)
        {
            if (year == null) year = DateTime.Now.Year;

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
    }
}
