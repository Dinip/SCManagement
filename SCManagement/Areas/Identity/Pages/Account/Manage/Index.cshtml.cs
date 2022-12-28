// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FluentEmail.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SCManagement.Data;
using SCManagement.Models;
using SCManagement.Models.Validations;
using SCManagement.Services.AzureStorageService;
using SCManagement.Services.AzureStorageService.Models;

namespace SCManagement.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IStringLocalizer<SharedResource> _stringLocalizer;
        private readonly ApplicationDbContext _context;
        private readonly IAzureStorage _azureStorage;

        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IStringLocalizer<SharedResource> stringLocalizer,
            ApplicationDbContext context,
            IAzureStorage azureStorage)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _stringLocalizer = stringLocalizer;
            _context = context;
            _azureStorage = azureStorage;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        public string ProfilePictureUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>

            [Required(ErrorMessage = "Error_Required")]
            [StringLength(100, ErrorMessage = "Error_Legth", MinimumLength = 2)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Error_Required")]
            [StringLength(100, ErrorMessage = "Error_Legth", MinimumLength = 2)]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            public string Email { get; set; }

            [Display(Name = "Date Of Birth")]
            [DataType(DataType.Date)]
            [DateOfBirth(MinAge = 6, MaxAge = 100, ErrorMessage = "Error_DateOfBirth")]
            public DateTime? DateOfBirth { get; set; }

            public IFormFile? File { get; set; }
            public bool RemoveImage { get; set; } = false;


            //public int? AddressId { get; set; }
            //public Address? Address { get; set; }
        }

        private async Task LoadAsync(User user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var userWithPFP = await _context.Users.Include(u => u.ProfilePicture).FirstOrDefaultAsync(u => u.Id == user.Id);

            Username = userName;
            ProfilePictureUrl = userWithPFP.ProfilePicture == null ? "https://cdn.scmanagement.me/public/user_placeholder.png" : userWithPFP.ProfilePicture.Uri;

            Input = new InputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = phoneNumber,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            //override user with information about profile picture
            user = await _context.Users.Include(c => c.ProfilePicture).FirstAsync(u => u.Id == user.Id);

            //Checks if the user first name is diferent from the user first name saved, and if so trie to update 
            if (user.FirstName != Input.FirstName)
            {
                user.FirstName = Input.FirstName;
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    StatusMessage = $"{_stringLocalizer["StatusMessage_ErrorUpdate"]} {_stringLocalizer["First Name"]}";
                    return RedirectToPage();
                }
            }

            //Checks if the user last name is diferent from the user last name saved, and if so trie to update 
            if (user.LastName != Input.LastName)
            {
                user.LastName = Input.LastName;
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    StatusMessage = $"{_stringLocalizer["StatusMessage_ErrorUpdate"]} {_stringLocalizer["Last Name"]}";
                    return RedirectToPage();
                }
            }

            //Checks if the user phone number is diferent from the user phone number saved, and if so trie to update 
            if (user.PhoneNumber != Input.PhoneNumber)
            {
                user.PhoneNumber = Input.PhoneNumber;
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    StatusMessage = $"{_stringLocalizer["StatusMessage_ErrorUpdate"]} {_stringLocalizer["Phone number"]}";
                    return RedirectToPage();
                }
            }

            //Checks if the user date of birth is diferent from the user date of birth saved, and if so trie to update 
            if (user.DateOfBirth != Input.DateOfBirth)
            {
                user.DateOfBirth = Input.DateOfBirth;
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    StatusMessage = $"{_stringLocalizer["StatusMessage_ErrorUpdate"]} {_stringLocalizer["Date Of Birth"]}";
                    return RedirectToPage();
                }
            }

            if (Input.RemoveImage)
            {
                await CheckAndDeleteProfilePicture(user);
            }

            if (Input.File != null)
            {
                BlobResponseDto uploadResult = await _azureStorage.UploadAsync(Input.File);
                if (uploadResult.Error)
                {
                    StatusMessage = $"{_stringLocalizer["StatusMessage_ErrorUpdate"]} {_stringLocalizer["Profile Picure"]}";
                    return RedirectToPage();
                }
                await CheckAndDeleteProfilePicture(user);
                user.ProfilePicture = uploadResult.Blob;
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    StatusMessage = $"{_stringLocalizer["StatusMessage_ErrorUpdate"]} {_stringLocalizer["Profile Picure"]}";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = _stringLocalizer["StatusMessage_ProfileUpdate"];
            return RedirectToPage();
        }

        public string ShowRemoveButton() => ProfilePictureUrl.Contains("uploads") ? "" : "d-none";

        private async Task CheckAndDeleteProfilePicture(User user)
        {
            if (user.ProfilePicture != null)
            {
                await _azureStorage.DeleteAsync(user.ProfilePicture.Uuid);
                _context.BlobDto.Remove(user.ProfilePicture);
                user.ProfilePicture = null;
                //update user and save changes
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
