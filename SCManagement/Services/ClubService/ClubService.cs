﻿using System;
using System.Security.Policy;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SCManagement.Data;
using SCManagement.Models;
using SCManagement.Services.AzureStorageService;
using SCManagement.Services.AzureStorageService.Models;

namespace SCManagement.Services.ClubService
{
    public class ClubService : IClubService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _httpContext;
        private readonly SharedResourceService _sharedResource;
        private readonly IAzureStorage _azureStorage;

        public ClubService(
            ApplicationDbContext context,
            IEmailSender emailSender,
            IHttpContextAccessor httpContext,
            SharedResourceService sharedResource,
            IAzureStorage azureStorage)
        {
            _context = context;
            _emailSender = emailSender;
            _httpContext = httpContext;
            _sharedResource = sharedResource;
            _azureStorage = azureStorage;
        }

        /// <summary>
        /// Allow to create a club
        /// </summary>
        /// <param name="club">club to be created</param>
        /// <param name="userId">user who created the club</param>
        /// <returns>A new club</returns>
        public async Task<Club> CreateClub(Club club, string userId)
        {

            //Create a new club
            Club c = new Club
            {
                Name = club.Name,
                Modalities = GetModalities().Result.Where(m => club.ModalitiesIds.Contains(m.Id)).ToList(),
                CreationDate = DateTime.Now
            };

            //with this implementation, the user can only create 1 club (1 user per clube atm, might change later)
            List<UsersRoleClub> roles = new();

            //add a role whit the user that create the club
            roles.Add(new UsersRoleClub { UserId = userId, RoleId = 50, JoinDate = DateTime.Now });
            c.UsersRoleClub = roles;

            _context.Club.Add(c);
            await _context.SaveChangesAsync();

            return c;
        }

        public Task<Club> DeleteClub(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Allows to get a desired club
        /// </summary>
        /// <param name="id">club id to find</param>
        /// <returns>A wanted club</returns>
        public async Task<Club> GetClub(int id)
        {
            return await _context.Club.Include(c => c.Modalities).Include(c => c.Photography).FirstOrDefaultAsync(m => m.Id == id);
        }

        /// <summary>
        /// Allows to get club id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>id of the club</returns>
        public int GetClubId(int id)
        {
            return _context.UsersRoleClub.Where(u => u.Id == id).FirstOrDefault().ClubId;
        }

        /// <summary>
        /// Allows to get all clubs
        /// </summary>
        /// <returns>All clubs</returns>
        public async Task<IEnumerable<Club>> GetClubs()
        {
            return await _context.Club.Include(c => c.Modalities).ToListAsync();
        }

        public async Task<Club> UpdateClub(Club club)
        {
            _context.Club.Update(club);
            await _context.SaveChangesAsync();
            return club;
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
        /// <param name="userId">that club's user id</param>
        /// <param name="clubId">club id viewing user roles</param>
        /// <returns>user role list</returns>
        public List<int> UserRolesInClub(string userId, int clubId)
        {
            List<int> rolesId = new List<int>();
            rolesId.AddRange(_context.UsersRoleClub.Where(f => f.UserId == userId && f.ClubId == clubId).Select(r => r.RoleId).ToList());
            return rolesId;
        }


        /// <summary>
        /// Allow to get the admin of the club
        /// </summary>
        /// <param name="clubId">id of the club to get the admin</param>
        /// <returns>admin user of the club</returns>
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

        /// <summary>
        /// Allow to get all roles of the club
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<RoleClub>> GetRoles()
        {
            return Task.FromResult(_context.RoleClub.Where(r => r.Id > 10 && r.Id < 50).AsEnumerable()); ;
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

        /// <summary>
        /// Allow to know if a user are in the club
        /// </summary>
        /// <param name="userId">user id to be verified</param>
        /// <param name="clubId">club verifying user</param>
        /// <returns>Returns a boolean value, true if in the club and false if not </returns>
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

        /// <summary>
        /// Allow to know if the user is a admin of the club
        /// </summary>
        /// <param name="userId">id of the user</param>
        /// <param name="clubId">id of the club</param>
        /// <returns>a boolean value, true is the user is admin of the club, false if not</returns>
        public bool IsClubAdmin(string userId, int clubId)
        {
            return UserRolesInClub(userId, clubId).Contains(50);
        }

        /// <summary>
        /// Allow to know if the user is a secretary of the club
        /// </summary>
        /// <param name="userId">id of the user</param>
        /// <param name="clubId">id of the club</param>
        /// <returns>a boolean value, true is the user is secretary of the club, false if not</returns>
        public bool IsClubSecretary(string userId, int clubId)
        {
            return UserRolesInClub(userId, clubId).Contains(40);
        }

        /// <summary>
        /// Allow to know if the user is a manager of the club
        /// </summary>
        /// <param name="userId">id of the user</param>
        /// <param name="clubId">id of the club</param>
        /// <returns>a boolean value, true is the user is manager of the club, false if not</returns>
        public bool IsClubManager(string userId, int clubId)
        {
            return UserRolesInClub(userId, clubId).Any(r => r == 40 || r == 50);
        }

        /// <summary>
        /// Allow to know if the user is a Trainer of the club
        /// </summary>
        /// <param name="userId">id of the user</param>
        /// <param name="clubId">id of the club</param>
        /// <returns>a boolean value, true is the user is Trainer of the club, false if not</returns>
        public bool IsClubTrainer(string userId, int clubId)
        {
            return UserRolesInClub(userId, clubId).Any(r => r == 30);
        }

        /// <summary>
        /// Allow to know if the user is a Staff of the club
        /// </summary>
        /// <param name="userId">id of the user</param>
        /// <param name="clubId">id of the club</param>
        /// <returns>a boolean value, true is the user is Staff of the club, false if not</returns>
        public bool IsClubStaff(string userId, int clubId)
        {
            return (UserHasRoleInClub(userId, clubId, 50) || UserHasRoleInClub(userId, clubId, 40));
        }

        /// <summary>
        /// Allow to know if the user is a Athlete of the club
        /// </summary>
        /// <param name="userId">id of the user</param>
        /// <param name="clubId">id of the club</param>
        /// <returns>a boolean value, true is the user is a Athlete of the club, false if not</returns>
        public bool IsClubAthlete(string userId, int clubId)
        {
            return UserRolesInClub(userId, clubId).Any(r => r == 20);
        }

        /// <summary>
        /// Allow to know if the user is a Member of the club
        /// </summary>
        /// <param name="userId">id of the user</param>
        /// <param name="clubId">id of the club</param>
        /// <returns>a boolean value, true is the user is a Member of the club, false if not</returns>
        public bool IsClubMember(string userId, int clubId)
        {
            return UserRolesInClub(userId, clubId).Any(r => r == 20 || r == 30 || r == 40);
        }

        /// <summary>
        /// Allow to know if the user is a Member of the club
        /// </summary>
        /// <param name="userId">id of the user</param>
        /// <param name="clubId">id of the club</param>
        /// <returns>a boolean value, true is the user is a Member of the club, false if not</returns>
        public bool IsClubPartner(string userId, int clubId)
        {
            return UserRolesInClub(userId, clubId).Any(r => r == 10);
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


        /// <summary>
        /// Allows to get all the modalities
        /// </summary>
        /// <returns>All modalities</returns>
        public async Task<IEnumerable<Modality>> GetModalities()
        {
            return await _context.Modality.ToListAsync();
        }

        /// <summary>
        /// Allow to know if a user have a role in the club
        /// </summary>
        /// <param name="userId">user id to check</param>
        /// <param name="clubId">club id verifying user</param>
        /// <param name="roleId">role to be verified</param>
        /// <returns>If the user has the desired role, false if not</returns>
        public bool UserHasRoleInClub(string userId, int clubId, int roleId)
        {
            var r = _context.UsersRoleClub.FirstOrDefault(f => f.UserId == userId && f.ClubId == clubId && f.RoleId == roleId);
            return r != null;
        }

        public async Task<IEnumerable<UsersRoleClub>> GetClubPartners(int clubId)
        {
            //when the payment module is done, this should return the status of the partner, based on the payment status
            return await _context.UsersRoleClub.Where(u => u.ClubId == clubId && u.RoleId == 10).Include(r => r.User).ToListAsync();
        }

        public async Task RemoveClubUser(int userRoleClubId)
        {
            _context.UsersRoleClub.Remove(await _context.UsersRoleClub.FindAsync(userRoleClubId));
            await _context.SaveChangesAsync();
        }

        public async Task RemoveClubUser(string userId, int clubId, int roleId)
        {
            _context.UsersRoleClub.Remove(await _context.UsersRoleClub.Where(r => r.UserId == userId && r.ClubId == clubId && r.RoleId == roleId).FirstAsync());
            await _context.SaveChangesAsync();
        }

        public async Task UpdateClubPhoto(Club club, bool remove = false, IFormFile? file = null)
        {

            //new profile picture provided, delete old from storage and update club to new one
            if (file != null)
            {
                BlobResponseDto uploadResult = await _azureStorage.UploadAsync(file);
                await deletePhoto(club);
                club.Photography = uploadResult.Blob;
                return;
            }

            if (remove)
            {
                await deletePhoto(club);
                return;
            }
        }

        private async Task deletePhoto(Club club)
        {
            if (club.Photography != null)
            {
                await _azureStorage.DeleteAsync(club.Photography.Uuid);
                _context.BlobDto.Remove(club.Photography);
                club.Photography = null;
            }
        }

        public void UpdateClubModalities(Club club, IEnumerable<int> ModalitiesIds)
        {
            //new modalities choosed
            List<Modality> newModalities = GetModalities().Result.Where(m => ModalitiesIds.Contains(m.Id)).ToList();

            //remove from club modalities which are not in the new modalities list
            foreach (Modality m in club.Modalities.ToList())
            {
                if (!newModalities.Contains(m))
                {
                    club.Modalities.Remove(m);
                }
            }

            //add to club modalities the modalities that are in the new modalities list and aren't on club modalities list already
            foreach (Modality m in newModalities.ToList())
            {
                if (!club.Modalities.Contains(m))
                {
                    club.Modalities.Add(m);
                }
            }
        }

        public async Task<UsersRoleClub?> GetUserRoleClubFromId(int userRoleClubId)
        {
            return await _context.UsersRoleClub.FindAsync(userRoleClubId);
        }

        public async Task AddUserToClub(string userId, int clubId, int roleId)
        {
            var a = _context.UsersRoleClub.Add(new UsersRoleClub { UserId = userId, ClubId = clubId, RoleId = roleId, JoinDate = DateTime.Now });
            await _context.SaveChangesAsync();
        }
    }
}
