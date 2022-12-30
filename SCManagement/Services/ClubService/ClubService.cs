using System.Security.Policy;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using SCManagement.Data;
using SCManagement.Models;

namespace SCManagement.Services.ClubService
{
    public class ClubService : IClubService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _httpContext;

        public ClubService(ApplicationDbContext context, IEmailSender emailSender, IHttpContextAccessor httpContext)
        {
            _context = context;
            _emailSender = emailSender;
            _httpContext = httpContext;
        }

        public Task<Club> CreateClub(Club club)
        {
            throw new NotImplementedException();
        }

        public Task<Club> DeleteClub(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Club> GetClub(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Club>> GetClubs()
        {
            throw new NotImplementedException();
        }

        public Task<Club> UpdateClub(Club club)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generate a join club code (the code in string) with 10 chars
        /// but first check if the code is unique on the database
        /// </summary>
        /// <returns></returns>
        private string generateCode()
        {
            string code;
            do
            {
                Guid g = Guid.NewGuid();
                string GuidString = Convert.ToBase64String(g.ToByteArray());
                GuidString = GuidString.Replace("=", "");
                GuidString = GuidString.Replace("+", "");
                GuidString = GuidString.Replace("/", "");
                GuidString = GuidString.ToUpper();
                code = GuidString.Substring(0, 8);
            } while (codeAlreadyExists(code));
            return code;
        }

        private bool codeAlreadyExists(string code)
        {
            return _context.CodeClub.Any(c => c.Code == code);
        }

        /// <summary>
        /// Generates a code to access a club and saves it
        /// </summary>
        /// <param name="clubId"></param>
        /// <param name="creatorId"></param>
        /// <param name="roleId"></param>
        /// <param name="expireDate"></param>
        /// <returns></returns>
        public async Task<CodeClub> GenerateCode(int clubId, string creatorId, int roleId, DateTime? expireDate)
        {
            // if trying to generate a code to a secretary role and the user
            // is another secretary, the code must be validated by the club admin first
            bool isApproved = true;
            if (roleId == 4 && userHasRoleInClub(creatorId, clubId, 4))
            {
                isApproved = false;
            }

            string code = generateCode();
            CodeClub cc = new CodeClub { ClubId = clubId, RoleId = roleId, CreatedByUserId = creatorId, ExpireDate = expireDate, Code = code, Approved = isApproved };
            _context.CodeClub.Add(cc);
            await _context.SaveChangesAsync();

            //send an email to club admin to validated the code
            if (!isApproved)
            {
                string hostUrl = $"{_httpContext.HttpContext.Request.Scheme}://{_httpContext.HttpContext.Request.Host}";
                string url = $"${hostUrl}/Clubs/ValidateCode?code=${code}";
                await sendEmailToClubAdmin(clubId, "Approve code creation", $"Approve creation of the code {cc.Code} to the Secretary Role. <a href=\"{url}\">");
            }

            return cc;
        }

        /// <summary>
        /// Obtain a list of rolesId that the user has in the club
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clubId"></param>
        /// <returns></returns>
        private List<int> userRolesInClub(string userId, int clubId)
        {
            List<int> rolesId = new List<int>();
            rolesId.AddRange(_context.UsersRoleClub.Where(f => f.UserId == userId && f.ClubId == clubId).Select(r => r.RoleId).ToList());
            return rolesId;
        }

        /// <summary>
        /// Check if the user has a specific role in a club
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clubId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        private bool userHasRoleInClub(string userId, int clubId, int roleId)
        {
            return userRolesInClub(userId, clubId).Contains(roleId);
        }

        /// <summary>
        /// Allow to know if a user are in the club
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clubId"></param>
        /// <returns></returns>
        private bool userAlreadyInAClub(string userId, int? clubId = null)
        {
            if (clubId == null)
            {
                return _context.UsersRoleClub.Any(f => f.UserId == userId);
            }
            return _context.UsersRoleClub.Any(f => f.UserId == userId && f.ClubId == clubId);
        }

        private User getClubAdmin(int clubId)
        {
            return _context.UsersRoleClub.Where(r => r.ClubId == clubId && r.RoleId == 5).Include(r => r.User).Select(r => r.User).FirstOrDefault()!;
        }

        private async Task sendEmailToClubAdmin(int clubId, string subject, string content)
        {
            User admin = getClubAdmin(clubId);
            await _emailSender.SendEmailAsync(admin.Email, subject, content);
        }

        public Task<IEnumerable<RoleClub>> GetRoles()
        {
            return Task.FromResult(_context.RoleClub.Where(r => r.Id != 1 && r.Id != 5).AsEnumerable());
        }

        public bool IsClubManager(string userId, int clubId)
        {
            return userRolesInClub(userId, clubId).Any(r => r == 4 || r == 5);
        }

        public Task<IEnumerable<CodeClub>> GetCodes(int clubId)
        {
            return Task.FromResult(_context.CodeClub
                .Where(c => c.ClubId == clubId)
                .Include(c => c.Role)
                .Include(c => c.CreatedByUser)
                .Include(c => c.UsedByUser)
                .Select(c => new CodeClub
                {
                    Id = c.Id,
                    Code = c.Code,
                    Approved = c.Approved,
                    CreationDate = c.CreationDate,
                    ExpireDate = c.ExpireDate,
                    UsedDate = c.UsedDate,
                    CreatedByUser = new User { FirstName = c.CreatedByUser.FirstName, LastName = c.CreatedByUser.LastName },
                    UsedByUser = new User { FirstName = c.UsedByUser.FirstName, LastName = c.UsedByUser.LastName },
                    Role = new RoleClub { RoleName = c.Role.RoleName }
                })
                .AsEnumerable());
        }

        public async Task<CodeClub> GetCodeWithInfos(int id)
        {
            return await _context.CodeClub
                .Where(c => c.Id == id)
                .Include(c => c.Role)
                .Include(c => c.CreatedByUser)
                .Include(c => c.UsedByUser)
                .Select(c => new CodeClub
                {
                    Id = c.Id,
                    Code = c.Code,
                    Approved = c.Approved,
                    CreationDate = c.CreationDate,
                    ExpireDate = c.ExpireDate,
                    UsedDate = c.UsedDate,
                    CreatedByUser = new User { FirstName = c.CreatedByUser.FirstName, LastName = c.CreatedByUser.LastName },
                    UsedByUser = new User { FirstName = c.UsedByUser.FirstName, LastName = c.UsedByUser.LastName },
                    Role = new RoleClub { RoleName = c.Role.RoleName }
                })
                .FirstAsync();
        }

        public bool IsAlreadyInAClub(string userId)
        {
            return userAlreadyInAClub(userId);
        }

        public bool UseCode(string userId, string code)
        {
            CodeClub cc = _context.CodeClub.Where(c => c.Code == code).FirstOrDefault()!;

            //code not found, not approved, already used or expired, return false and dont join
            if (cc == null || !cc.Approved || cc.UsedByUserId != null || cc.ExpireDate < DateTime.Now) return false;

            _context.UsersRoleClub.Add(new UsersRoleClub { UserId = userId, ClubId = cc.ClubId, RoleId = cc.RoleId });
            cc.UsedByUserId = userId;
            cc.UsedDate = DateTime.Now;
            _context.CodeClub.Update(cc);

            _context.SaveChanges();
            return true;
        }
    }
}
