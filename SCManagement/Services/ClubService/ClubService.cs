using FluentEmail.Core;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using SCManagement.Data;
using SCManagement.Models;
using SCManagement.Services.AzureStorageService;
using SCManagement.Services.AzureStorageService.Models;
using SCManagement.Services.Location;

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
            IAzureStorage azureStorage,
            ILocationService locationService)
        {
            _context = context;
            _emailSender = emailSender;
            _httpContext = httpContext;
            _sharedResource = sharedResource;
            _azureStorage = azureStorage;
        }

        /// <summary>
        /// Saves the created club and assign the user to the club
        /// with the club admin role
        /// </summary>
        /// <param name="club">club to be created</param>
        /// <param name="userId">user who created the club</param>
        /// <returns>A new club</returns>
        public async Task<Club> CreateClub(Club club, string userId)
        {
            //with this implementation, the user can only create 1 club (1 user per clube atm, might change later)
            List<UsersRoleClub> roles = new();

            //add a role whit the user that create the club
            roles.Add(new UsersRoleClub { UserId = userId, RoleId = 50, JoinDate = DateTime.Now });
            club.UsersRoleClub = roles;
            club.ClubPaymentSettings = new ClubPaymentSettings();

            _context.Club.Add(club);
            await _context.SaveChangesAsync();

            return club;
        }

        public Task<Club> DeleteClub(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a club by its id
        /// </summary>
        /// <param name="id">club id to find</param>
        /// <returns>Found club or null</returns>
        public async Task<Club?> GetClub(int id)
        {
            return await _context.Club
                .Include(c => c.Modalities)
                .Include(c => c.Photography)
                //.Include(c => c.Address)
                //.Include(c => c.Address.County)
                //.ThenInclude(c => c.District)
                //.ThenInclude(c => c.Country)
                //.Select(s => new Club
                //{
                //    Id = s.Id,
                //    Name = s.Name,
                //    Email = s.Email,
                //    PhoneNumber = s.PhoneNumber,
                //    About = s.About,
                //    Photography = s.Photography,
                //    Modalities = s.Modalities,
                //    //Address = new Address
                //    //{
                //    //    Street = s.Address.Street,
                //    //    Number = s.Address.Number,
                //    //    ZipCode = s.Address.ZipCode,
                //    //    County = new County
                //    //    {
                //    //        Name = $"{s.Address.County.Name}, {s.Address.County.District!.Name}, {s.Address.County.District.Country!.Name}"
                //    //    }
                //    //}
                //})
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        /// <summary>
        /// Gets all clubs available
        /// </summary>
        /// <returns>All clubs</returns>
        public async Task<IEnumerable<Club>> GetClubs()
        {
            return await _context.Club
               .Include(c => c.Modalities)
               .Include(c => c.Photography)
               //.Include(c => c.Address)
               //.Include(c => c.Address.County)
               //.ThenInclude(c => c.District)
               //.ThenInclude(c => c.Country)
               .Select(s =>
               new Club
               {
                   Id = s.Id,
                   Name = s.Name,
                   Email = s.Email,
                   PhoneNumber = s.PhoneNumber,
                   About = s.About,
                   Photography = s.Photography,
                   Modalities = s.Modalities,
                   //Address = new Address
                   //{
                   //    Street = s.Address.Street,
                   //    Number = s.Address.Number,
                   //    ZipCode = s.Address.ZipCode,
                   //    County = new County
                   //    {
                   //        Name = $"{s.Address.County.Name}, {s.Address.County.District!.Name}, {s.Address.County.District.Country!.Name}"
                   //    }
                   //}
               })
               .ToListAsync();
        }

        /// <summary>
        /// Persists the changes made to a club in the database
        /// </summary>
        /// <param name="club"></param>
        /// <returns>Updated Club</returns>
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
        /// If the code is created by a secretary user for another
        /// secretary user, an email is sent to the club admin
        /// to ask for approve
        /// </summary>
        /// <param name="codeToCreate"></param>
        /// <returns>Created code to access the club</returns>
        public async Task<CodeClub> GenerateCode(CodeClub codeToCreate)
        {
            string code = generateCode();
            CodeClub cc = codeToCreate;
            cc.Code = code;
            _context.CodeClub.Add(cc);
            await _context.SaveChangesAsync();

            //send an email to club admin to validated the code
            if (!cc.Approved)
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
                await sendEmailToClubAdmin(cc.ClubId, "Subject_ApproveCode", "Email_ApproveCode", values);
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
        /// Gets the role of a given user in a given club
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clubId"></param>
        /// <returns></returns>
        public int GetUserRoleInClub(string userId, int clubId)
        {
            return _context.UsersRoleClub.Where(f => f.UserId == userId && f.ClubId == clubId).Select(r => r.RoleId).FirstOrDefault();
        }


        /// <summary>
        /// Gets the admin user of a given club
        /// </summary>
        /// <param name="clubId">id of the club to get the admin</param>
        /// <returns>admin user of the club</returns>
        private User getClubAdmin(int clubId)
        {
            return _context.UsersRoleClub.Where(r => r.ClubId == clubId && r.RoleId == 50).Include(r => r.User).Select(r => r.User).FirstOrDefault()!;
        }

        /// <summary>
        /// Sends an email to the club admin with a given subject and body
        /// and replaces the values in the body with the values in the dictionary
        /// when the email content is dinamic
        /// </summary>
        /// <param name="clubId"></param>
        /// <param name="keySubject"></param>
        /// <param name="keyContent"></param>
        /// <param name="values"></param>
        /// <returns></returns>
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
        /// Gets all the roles available to be assigned 
        /// to a user in a club
        /// </summary>
        /// <returns>list of roles</returns>
        public Task<IEnumerable<RoleClub>> GetRoles()
        {
            return Task.FromResult(_context.RoleClub.Where(r => r.Id > 10 && r.Id < 50).AsEnumerable()); ;
        }

        /// <summary>
        /// Gets all the generated codes for a given club
        /// with the respective status
        /// </summary>
        /// <param name="clubId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets a code by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
        /// Checks if a given user has any role in a given club
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

        /// <summary>
        /// Assigns the usage of a code to a user and performs the verifications
        /// needed to check if the code is valid and assigns the role to the user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<KeyValuePair<bool, string>> UseCode(string userId, CodeClub code)
        {
            if (code == null)
            {
                return await Task.FromResult(new KeyValuePair<bool, string>(false, "Code_NotFound"));
            }

            CodeClub cc = _context.CodeClub.Where(c => c.Code == code.Code).FirstOrDefault()!;

            if (cc == null)
            {
                return await Task.FromResult(new KeyValuePair<bool, string>(false, "Code_NotFound"));
            }

            if (!cc.Approved)
            {
                return await Task.FromResult(new KeyValuePair<bool, string>(false, "Code_NotApproved"));
            }

            if (cc.UsedByUserId != null)
            {
                return await Task.FromResult(new KeyValuePair<bool, string>(false, "Code_AlreadyUsed"));
            }

            if (cc.ExpireDate < DateTime.Now)
            {
                return await Task.FromResult(new KeyValuePair<bool, string>(false, "Code_Expired"));
            }

            if (UserAlreadyInAClub(userId, cc.ClubId))
            {
                return await Task.FromResult(new KeyValuePair<bool, string>(false, "Code_AlreadyPart"));
            }

            _context.UsersRoleClub.Add(new UsersRoleClub { UserId = userId, ClubId = cc.ClubId, RoleId = cc.RoleId });
            cc.UsedByUserId = userId;
            cc.UsedDate = DateTime.Now;
            _context.CodeClub.Update(cc);
            await _context.SaveChangesAsync();
            return await Task.FromResult(new KeyValuePair<bool, string>(true, "Success"));
        }


        /// <summary>
        /// Given a role in a club, check if is admin
        /// </summary>
        /// <param name="userRoleClub">role object to eval</param>
        /// <returns>a boolean value, true is the user is admin of the club, false if not</returns>
        public bool IsClubAdmin(UsersRoleClub userRoleClub)
        {
            return userRoleClub.RoleId == 50;
        }

        /// <summary>
        /// Given a role in a club, check if is secretary
        /// </summary>
        /// <param name="userRoleClub">role object to eval</param>
        /// <returns>a boolean value, true is the user is secretary of the club, false if not</returns>
        public bool IsClubSecretary(UsersRoleClub userRoleClub)
        {
            return userRoleClub.RoleId == 40;

        }
        /// <summary>
        /// Check if a give user is secreatary in a given club
        /// </summary>
        /// <param name="userRoleClub">role object to eval</param>
        /// <returns>a boolean value, true is the user is secretary of the club, false if not</returns>
        public bool IsClubSecretary(string userId, int clubId)
        {
            return UserRolesInClub(userId, clubId).Any(r => r >= 20);
        }

        /// <summary>
        /// Given a role in a club, check if is admin or secretary
        /// </summary>
        /// <param name="userRoleClub">role object to eval</param>
        /// <returns>a boolean value, true is the user is manager of the club, false if not</returns>
        public bool IsClubManager(UsersRoleClub userRoleClub)
        {
            return userRoleClub.RoleId == 40 || userRoleClub.RoleId == 50;
        }

        /// <summary>
        /// Given a role in a club, check if is trainer
        /// </summary>
        /// <param name="userRoleClub">role object to eval</param>
        /// <returns>a boolean value, true is the user is trainer of the club, false if not</returns>
        public bool IsClubTrainer(UsersRoleClub userRoleClub)
        {
            return userRoleClub.RoleId == 30;
        }

        /// <summary>
        /// Given a role in a club, check if is admin or secretary or trainer
        /// </summary>
        /// <param name="userRoleClub">role object to eval</param>
        /// <returns>a boolean value, true is the user is Staff (admin, secretary, trainer) of the club, false if not</returns>
        public bool IsClubStaff(UsersRoleClub userRoleClub)
        {
            return userRoleClub.RoleId == 30 || userRoleClub.RoleId == 40 || userRoleClub.RoleId == 50;
        }

        /// <summary>
        /// Given a role in a club, check if is athlete
        /// </summary>
        /// <param name="userRoleClub">role object to eval</param>
        /// <returns>a boolean value, true is the user is a Athlete of the club, false if not</returns>
        public bool IsClubAthlete(UsersRoleClub userRoleClub)
        {
            return userRoleClub.RoleId == 20;
        }

        /// <summary>
        /// Given a user and a club, check if the user is a member 
        /// (athlete, trainer, secretary or admin)
        /// </summary>
        /// <param name="userId">id of the user</param>
        /// <param name="clubId">id of the club</param>
        /// <returns>a boolean value, true is the user is a Member of the club, false if not</returns>
        public bool IsClubMember(string userId, int clubId)
        {
            return UserRolesInClub(userId, clubId).Any(r => r >= 20);
        }

        /// <summary>
        /// Given a user and a club, check if the user is partner
        /// </summary>
        /// <param name="userId">id of the user</param>
        /// <param name="clubId">id of the club</param>
        /// <returns>a boolean value, true is the user is a Member of the club, false if not</returns>
        public bool IsClubPartner(string userId, int clubId)
        {
            return UserRolesInClub(userId, clubId).Contains(10);
        }

        /// <summary>
        /// Approve the utilization of a code (to be used by club admin)
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool ApproveCode(string code)
        {
            CodeClub? cc = _context.CodeClub.Where(c => c.Code == code).FirstOrDefault();
            if (cc == null) return false;
            cc.Approved = true;
            _context.Update(cc);
            _context.SaveChanges();
            return true;
        }

        /// <summary>
        /// Sends a email with the access code to a user to join a club
        /// </summary>
        /// <param name="codeId"></param>
        /// <param name="email"></param>
        /// <param name="clubId"></param>
        /// <returns></returns>
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
                    { "_INVITER_", code.CreatedByUser.FullName },
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

        /// <summary>
        /// Gets a user by an email address
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Gets all club partners (users role object with join date)
        /// </summary>
        /// <param name="clubId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<UsersRoleClub>> GetClubPartners(int clubId)
        {
            //when the payment module is done, this should return the status of the partner, based on the payment status
            return await _context.UsersRoleClub.Where(u => u.ClubId == clubId && u.RoleId == 10).Include(r => r.User).ToListAsync();
        }

        /// <summary>
        /// Removes a user by a given role from the club assigned to that role
        /// </summary>
        /// <param name="userRoleClubId"></param>
        /// <returns></returns>
        public async Task RemoveClubUser(int userRoleClubId)
        {
            _context.UsersRoleClub.Remove(await _context.UsersRoleClub.FindAsync(userRoleClubId));
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Removes a user with a role, from a club by a given userid, clubid and roleid
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clubId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task RemoveClubUser(string userId, int clubId, int roleId)
        {
            _context.UsersRoleClub.Remove(await _context.UsersRoleClub.Where(r => r.UserId == userId && r.ClubId == clubId && r.RoleId == roleId).FirstAsync());
            await _context.SaveChangesAsync();
        }


        /// <summary>
        /// Updates the photo for a club, can either upload a new one and
        /// remove the old one or just the remove the photo
        /// </summary>
        /// <param name="club"></param>
        /// <param name="remove"></param>
        /// <param name="file"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Helper function to remove the club photo
        /// </summary>
        /// <param name="club"></param>
        /// <returns></returns>
        private async Task deletePhoto(Club club)
        {
            if (club.Photography != null)
            {
                await _azureStorage.DeleteAsync(club.Photography.Uuid);
                _context.BlobDto.Remove(club.Photography);
                club.Photography = null;
            }
        }

        /// <summary>
        /// Updates the modalities of a given club (remove from club the ids that
        /// are not present and add the new ones)
        /// </summary>
        /// <param name="club"></param>
        /// <param name="ModalitiesIds"></param>
        /// <returns></returns>
        public async Task UpdateClubModalities(Club club, IEnumerable<int> ModalitiesIds)
        {
            //new modalities choosed
            List<Modality> newModalities = (await GetModalities()).Where(m => ModalitiesIds.Contains(m.Id)).ToList();

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

        /// <summary>
        /// Gets a user role + club by a userRoleClub id
        /// </summary>
        /// <param name="userRoleClubId"></param>
        /// <returns></returns>
        public async Task<UsersRoleClub?> GetUserRoleClubFromId(int userRoleClubId)
        {
            return await _context.UsersRoleClub.FindAsync(userRoleClubId);
        }

        /// <summary>
        /// Adds a user to a given club and role
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clubId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task AddUserToClub(string userId, int clubId, int roleId)
        {
            var a = _context.UsersRoleClub.Add(new UsersRoleClub { UserId = userId, ClubId = clubId, RoleId = roleId, JoinDate = DateTime.Now });
            await _context.SaveChangesAsync();
        }

        //public async Task<int> GetAddressAsync(int countyId, string street, string zipCode, string number)
        //{
        //    Address address = new Address
        //    {
        //        CountyId = countyId,
        //        Street = street,
        //        ZipCode = zipCode,
        //        Number = number
        //    };

        //    _context.Address.Add(address);
        //    await _context.SaveChangesAsync();

        //    return address.Id;
        //}

        //public void UpdateClubAddress(int addressId, int CountyId, string Street, string ZipCode, string Number)
        //{
        //    Address address = _context.Address.Find(addressId);
        //    address.Street = Street;
        //    address.ZipCode = ZipCode;
        //    address.Number = Number;
        //    address.County = null;
        //    address.CountyId = CountyId;

        //    _context.Address.Update(address);
        //    _context.SaveChanges();
        //}

        /// <summary>
        /// Gets all club staff (admin, secretary and trainer) (users role object with join date)
        /// </summary>
        /// <param name="clubId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<UsersRoleClub>> GetClubStaff(int clubId)
        {
            return await _context.UsersRoleClub.Where(u => u.ClubId == clubId && (u.RoleId == 30 || u.RoleId == 40 || u.RoleId == 50))
                .Include(r => r.User)
                .Include(r => r.Role)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all athletes of a given club (users role object with join date)
        /// </summary>
        /// <param name="clubId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<UsersRoleClub>> GetClubAthletes(int clubId)
        {
            return await _context.UsersRoleClub.Where(u => u.ClubId == clubId && u.RoleId == 20)
                .Include(r => r.User)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all the modalities of a given club
        /// </summary>
        /// <param name="clubId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Modality>> GetClubModalities(int clubId)
        {
            return await _context.Club.Where(c => c.Id == clubId).SelectMany(c => c.Modalities).ToListAsync();
        }

        /// <summary>
        /// Gets all athletes of a given club (users object)
        /// </summary>
        /// <param name="clubId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<User>> GetAthletes(int clubId)
        {
            return await _context.UsersRoleClub.Where(u => u.ClubId == clubId && u.RoleId == 20).Include(u => u.User).Select(u => u.User).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetClubTrainers(int clubId)
        {
            return await _context.UsersRoleClub.Where(u => u.ClubId == clubId && u.RoleId == 30).Include(u => u.User).Select(u => u.User).ToListAsync();
        }

        public async Task<ClubStatus> GetClubStatus(int clubId)
        {
            return await _context.Club.Where(c => c.Id == clubId).Select(c => c.Status).FirstAsync();
        }

        public async Task<ClubPaymentSettings> GetClubPaymentSettings(int clubId)
        {
            return await _context.ClubPaymentSettings.FirstAsync(c => c.ClubPaymentSettingsId == clubId);
        }

        public async Task<ClubPaymentSettings> UpdateClubPaymentSettings(ClubPaymentSettings settings)
        {
            var currentSettings = await GetClubPaymentSettings(settings.ClubPaymentSettingsId);
            currentSettings.AccountId = settings.AccountId;
            currentSettings.AccountKey = settings.AccountKey;

            //check if quota settings where updated and notify users
            if ((currentSettings.QuotaFrequency != settings.QuotaFrequency) || (currentSettings.QuotaFee != settings.QuotaFee))
            {
                currentSettings.QuotaFrequency = settings.QuotaFrequency;
                currentSettings.QuotaFee = settings.QuotaFee;
                await notifyPartnersQuotaChange(currentSettings.ClubPaymentSettingsId, settings);
            }

            _context.ClubPaymentSettings.Update(currentSettings);
            await _context.SaveChangesAsync();
            return currentSettings;
        }

        private async Task notifyPartnersQuotaChange(int clubId, ClubPaymentSettings settings)
        {
            var partners = await GetClubPartners(clubId);
            var club = await GetClub(clubId);

            string hostUrl = $"{_httpContext.HttpContext.Request.Scheme}://{_httpContext.HttpContext.Request.Host}";

            foreach (var partner in partners)
            {
                string lang = partner.User.Language;

                string emailBody = _sharedResource.Get("Email_ClubFees", lang);

                Dictionary<string, string> values = new Dictionary<string, string>
                {
                    { "_FREQUENCY_", _sharedResource.Get(settings.QuotaFrequency.Value.ToString(), lang) },
                    { "_PRICE_", $"{settings.QuotaFee.ToString()}€"},
                    { "_CLUB_", club.Name },
                    { "_SUBSCRIPTION_", $"{hostUrl}/Subscription"},
                };

                foreach (KeyValuePair<string, string> entry in values)
                {
                    emailBody = emailBody.Replace(entry.Key, entry.Value);
                }

                await _emailSender.SendEmailAsync(partner.User.Email, _sharedResource.Get("Subject_ClubFees", lang), emailBody);
            }
            return;
        }
    }
}
