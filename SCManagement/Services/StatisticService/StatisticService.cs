using Microsoft.EntityFrameworkCore;
using SCManagement.Data;
using SCManagement.Models;
using SCManagement.Services.PaymentService.Models;
using SCManagement.Services.StatisticsService.Models;

namespace SCManagement.Services.StatisticsService
{
    public class StatisticService : IStatisticService
    {
        private readonly ApplicationDbContext _context;
        public StatisticService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateClubUserStatistic(int clubId)
        {
            List<int> filterIds = new() { 10, 20 };
            var prevDay = DateTime.Now.Date.AddDays(-1);
            var result = await _context
                .ClubUserStatistic
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

            userCountByRole.ForEach(user =>
            {
                var f = result.Find(r => r.RoleId == user.RoleId);
                if (f != null)
                {
                    f.Value = user.Count;
                    _context.ClubUserStatistic.Update(f);
                }
                else
                {
                    var stat = new ClubUserStatistic
                    {
                        Value = user.Count,
                        RoleId = user.RoleId,
                        ClubId = clubId,
                        StatisticRange = StatisticRange.Month,
                        Timestamp = new DateTime(prevDay.Year, prevDay.Month, DateTime.DaysInMonth(prevDay.Year, prevDay.Month)),
                    };
                    _context.ClubUserStatistic.Add(stat);
                }
            });
            await _context.SaveChangesAsync();
        }

        public async Task CreateClubPaymentStatistic(int clubId)
        {
            var prevDay = DateTime.Now.Date.AddDays(-1);
            var result = await _context
                .ClubPaymentStatistic
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
                    f.Value = prod.Total;
                    _context.ClubPaymentStatistic.Update(f);
                }
                else if (prod.Total > 0)
                {
                    var stat = new ClubPaymentStatistic
                    {
                        Value = prod.Total,
                        ProductId = prod.ProductId,
                        ClubId = clubId,
                        StatisticRange = StatisticRange.Month,
                        ProductType = prod.ProductType,
                        Timestamp = new DateTime(prevDay.Year, prevDay.Month, DateTime.DaysInMonth(prevDay.Year, prevDay.Month)),
                    };
                    _context.ClubPaymentStatistic.Add(stat);
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
                    f.Value = prod.Total;
                    _context.ClubPaymentStatistic.Update(f);
                }
                else if (prod.Total > 0)
                {
                    var stat = new ClubPaymentStatistic
                    {
                        Value = prod.Total,
                        ClubId = clubId,
                        StatisticRange = StatisticRange.Month,
                        ProductType = prod.ProductType,
                        Timestamp = new DateTime(prevDay.Year, prevDay.Month, DateTime.DaysInMonth(prevDay.Year, prevDay.Month)),
                    };
                    _context.ClubPaymentStatistic.Add(stat);
                }
            });

            await _context.SaveChangesAsync();
        }

        public async Task CreateClubModalityStatistic(int clubId)
        {
            var prevDay = DateTime.Now.Date.AddDays(-1);
            var result = await _context
                .ClubModalityStatistic
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
                    _context.ClubModalityStatistic.Update(f);
                }
                else
                {
                    var stat = new ClubModalityStatistic
                    {
                        Value = modality.Count,
                        ModalityId = modality.ModalityId,
                        ClubId = clubId,
                        StatisticRange = StatisticRange.Month,
                        Timestamp = new DateTime(prevDay.Year, prevDay.Month, DateTime.DaysInMonth(prevDay.Year, prevDay.Month)),
                    };
                    _context.ClubModalityStatistic.Add(stat);
                }
            });
            await _context.SaveChangesAsync();
        }
    }
}
