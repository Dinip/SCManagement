using System;
using System.Security.Policy;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SCManagement.Data;
using SCManagement.Models;

namespace SCManagement.Services.ClubService
{
    public class ClubService : IClubService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _httpContext;
        private readonly SharedResourceService _sharedResource;

        public ClubService(
            ApplicationDbContext context,
            IEmailSender emailSender,
            IHttpContextAccessor httpContext,
            SharedResourceService sharedResource)
        {
            _context = context;
            _emailSender = emailSender;
            _httpContext = httpContext;
            _sharedResource = sharedResource;
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
        /// <returns>a code with 8 chars</returns>
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

        /// <summary>
        /// Auxiliary method to check if the code is unique on the database
        /// </summary>
        /// <param name="code">code to check</param>
        /// <returns></returns>
        private bool codeAlreadyExists(string code)
        {
            return _context.CodeClub.Any(c => c.Code == code);
        }

        /// <summary>
        /// Generates a code to access a club and saves it
        /// </summary>
        /// <param name="clubId">id of the club to generate the code</param>
        /// <param name="creatorId">id of the person who created the code</param>
        /// <param name="roleId">id of the role that the code will assign</param>
        /// <param name="expireDate">expire date of the code</param>
        /// <returns>code</returns>
        public async Task<CodeClub> GenerateCode(int clubId, string creatorId, int roleId, DateTime? expireDate)
        {
            // if trying to generate a code to a secretary role and the user
            // is another secretary, the code must be validated by the club admin first
            bool isApproved = true;
            if (roleId == 40 && IsClubSecretary(creatorId, clubId))
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
                string url = $"{hostUrl}/Clubs/Codes/{cc.ClubId}?approveCode={cc.Code}";
                string roleName = _context.RoleClub.Where(r => r.Id == cc.RoleId).Select(r => r.RoleName).First();
                Dictionary<string, string> values = new Dictionary<string, string>
                {
                    { "_CODE_", cc.Code },
                    { "_ROLE_", roleName },
                    { "_CALLBACK_URL_", url }
                };
                await sendEmailToClubAdmin(clubId, "Subject_ApproveCode", "Email_ApproveCode", values);
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

        private User getClubAdmin(int clubId)
        {
            return _context.UsersRoleClub.Where(r => r.ClubId == clubId && r.RoleId == 50).Include(r => r.User).Select(r => r.User).FirstOrDefault()!;
        }

        private async Task sendEmailToClubAdmin(int clubId, string keySubject, string keyContent, Dictionary<string, string>? values = null)
        {
            User admin = getClubAdmin(clubId);
            string emailBody = _sharedResource.Get(keyContent, admin.Language);

            if (values != null)
            {
                foreach (KeyValuePair<string, string> entry in values)
                {
                    emailBody = emailBody.Replace(entry.Key, entry.Value);
                }
            }
            await _emailSender.SendEmailAsync(admin.Email, _sharedResource.Get(keySubject, admin.Language), emailBody);
        }

        public Task<IEnumerable<RoleClub>> GetRoles()
        {
            return Task.FromResult(_context.RoleClub.Where(r => r.Id > 10 && r.Id < 50).AsEnumerable());;
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
                    ClubId = c.ClubId,
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

        public async Task<CodeClub?> GetCodeWithInfos(int id)
        {
            return await _context.CodeClub
                .Where(c => c.Id == id)
                .Include(c => c.Role)
                .Include(c => c.CreatedByUser)
                .Include(c => c.UsedByUser)
                .Include(c => c.Club)
                .Select(c => new CodeClub
                {
                    Id = c.Id,
                    Code = c.Code,
                    ClubId = c.ClubId,
                    Approved = c.Approved,
                    CreationDate = c.CreationDate,
                    ExpireDate = c.ExpireDate,
                    UsedDate = c.UsedDate,
                    Club = new Club { Name = c.Club.Name },
                    CreatedByUser = new User { FirstName = c.CreatedByUser.FirstName, LastName = c.CreatedByUser.LastName },
                    UsedByUser = new User { FirstName = c.UsedByUser.FirstName, LastName = c.UsedByUser.LastName },
                    Role = new RoleClub { RoleName = c.Role.RoleName }
                })
                .FirstOrDefaultAsync();
        }

        public bool UserAlreadyInAClub(string userId, int? clubId)
        {
            if (clubId == null)
            {
                return _context.UsersRoleClub.Any(f => f.UserId == userId);
            }
            return _context.UsersRoleClub.Any(f => f.UserId == userId && f.ClubId == clubId);
        }

        public KeyValuePair<bool, string> UseCode(string userId, CodeClub code)
        {
            if (code == null)
            {
                return new KeyValuePair<bool, string>(false, "Code not found");
            }

            CodeClub cc = _context.CodeClub.Where(c => c.Code == code.Code).FirstOrDefault()!;

            if (cc == null)
            {
                return new KeyValuePair<bool, string>(false, "Code not found");
            }

            if (!cc.Approved)
            {
                return new KeyValuePair<bool, string>(false, "Code not approved");
            }

            if (cc.UsedByUserId != null)
            {
                return new KeyValuePair<bool, string>(false, "Code already used");
            }

            if (cc.ExpireDate < DateTime.Now)
            {
                return new KeyValuePair<bool, string>(false, "Code expired");
            }

            if (UserAlreadyInAClub(userId, cc.ClubId))
            {
                return new KeyValuePair<bool, string>(false, "You are already part of the club with another role");
            }

            _context.UsersRoleClub.Add(new UsersRoleClub { UserId = userId, ClubId = cc.ClubId, RoleId = cc.RoleId });
            cc.UsedByUserId = userId;
            cc.UsedDate = DateTime.Now;
            _context.CodeClub.Update(cc);
            _context.SaveChanges();
            return new KeyValuePair<bool, string>(true, "Success");

        }

        public bool IsClubAdmin(string userId, int clubId)
        {
            return userRolesInClub(userId, clubId).Contains(50);
        }

        public bool IsClubSecretary(string userId, int clubId)
        {
            return userRolesInClub(userId, clubId).Contains(40);
        }

        public bool IsClubManager(string userId, int clubId)
        {
            return userRolesInClub(userId, clubId).Any(r => r == 40 || r == 50);
        }

        public bool IsClubTrainer(string userId, int clubId)
        {
            return userRolesInClub(userId, clubId).Contains(30);
        }

        public bool IsClubStaff(string userId, int clubId)
        {
            return userRolesInClub(userId, clubId).Any(r => r == 30 || r == 40 || r == 50);
        }

        public bool IsClubAthlete(string userId, int clubId)
        {
            return userRolesInClub(userId, clubId).Contains(20);
        }

        public bool IsClubMember(string userId, int clubId)
        {
            return userRolesInClub(userId, clubId).Any(r => r == 10 || r == 20 || r == 30 || r == 40 || r == 50);
        }

        public bool IsClubPartner(string userId, int clubId)
        {
            return userRolesInClub(userId, clubId).Contains(10);
        }

        public bool ApproveCode(string code)
        {
            CodeClub? cc = _context.CodeClub.Where(c => c.Code == code).FirstOrDefault();
            if (cc == null) return false;
            cc.Approved = true;
            _context.Update(cc);
            _context.SaveChanges();
            return true;
        }

        public Task SendCodeEmail(int codeId, string email, int clubId)
        {
            CodeClub? code = GetCodeWithInfos(codeId).Result;
            if (code == null || code.ClubId != clubId) return Task.CompletedTask;
            User? user = GetUser(email);
            string lang = "en-US";
            if (user != null) lang = user.Language;

            string emailBody = _sharedResource.Get("Email_InviteClub", lang);

            string hostUrl = $"{_httpContext.HttpContext.Request.Scheme}://{_httpContext.HttpContext.Request.Host}";

            Dictionary<string, string> values = new Dictionary<string, string>
                {
                    { "_CODE_", code.Code },
                    { "_ROLE_", code.Role.RoleName },
                    { "_CLUB_", code.Club.Name },
                    { "_INVITER_", code.CreatedByUser.FirstName },
                    { "_CALLBACK_URL_", $"{hostUrl}/Clubs/Join/?code={code.Code}" },
                    { "_PAGE_URL_", $"{hostUrl}/Clubs/Join" }
                };

            foreach (KeyValuePair<string, string> entry in values)
            {
                emailBody = emailBody.Replace(entry.Key, entry.Value);
            }

            _emailSender.SendEmailAsync(email.ToLower(), _sharedResource.Get("Subject_InviteClub", lang), emailBody);
            return Task.CompletedTask;
        }

        private User? GetUser(string email)
        {
            return _context.Users.Where(u => u.NormalizedEmail == email.ToUpper()).FirstOrDefault();
        }
    }
}
