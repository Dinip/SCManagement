using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using SCManagement.Services.AzureStorageService.Models;
using Xunit.Abstractions;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace SCManagement.Models
{
    public class Club
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Club name is required")]
        [StringLength(60, ErrorMessage = "Club name cannot contain more than 60 characters and less than 2 characters", MinimumLength = 2)]
        [Display(Name = "Clube Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [Phone]
        [Required(ErrorMessage = "Phone number is required")]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "About")]
        public string? About { get; set; }
        
        public DateTime CreationDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? PhotographyId { get; set; }
        public BlobDto? Photography { get; set; }

        //public string? PhotographyUrl { get; set; }
        public Address Address { get; set; }
        public ICollection<Modality>? Modalities { get; set; }

        //public bool RemoveImage { get; set; } = false;

        //public async Task<IActionResult> OnPostAsync()
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    if (user == null)
        //    {
        //        return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        await LoadAsync(user);
        //        return Page();
        //    }

        //    //override user with information about profile picture
        //    user = await _context.Users.Include(c => c.ProfilePicture).FirstAsync(u => u.Id == user.Id);

        //    //Checks if the user first name is diferent from the user first name saved, and if so trie to update 
        //    if (user.FirstName != Input.FirstName)
        //    {
        //        user.FirstName = Input.FirstName;
        //        var result = await _userManager.UpdateAsync(user);
        //        if (!result.Succeeded)
        //        {
        //            StatusMessage = $"{_stringLocalizer["StatusMessage_ErrorUpdate"]} {_stringLocalizer["First Name"]}";
        //            return RedirectToPage();
        //        }
        //    }

        //    //Checks if the user last name is diferent from the user last name saved, and if so trie to update 
        //    if (user.LastName != Input.LastName)
        //    {
        //        user.LastName = Input.LastName;
        //        var result = await _userManager.UpdateAsync(user);
        //        if (!result.Succeeded)
        //        {
        //            StatusMessage = $"{_stringLocalizer["StatusMessage_ErrorUpdate"]} {_stringLocalizer["Last Name"]}";
        //            return RedirectToPage();
        //        }
        //    }

        //    //Checks if the user phone number is diferent from the user phone number saved, and if so trie to update 
        //    if (user.PhoneNumber != Input.PhoneNumber)
        //    {
        //        user.PhoneNumber = Input.PhoneNumber;
        //        var result = await _userManager.UpdateAsync(user);
        //        if (!result.Succeeded)
        //        {
        //            StatusMessage = $"{_stringLocalizer["StatusMessage_ErrorUpdate"]} {_stringLocalizer["Phone number"]}";
        //            return RedirectToPage();
        //        }
        //    }

        //    //Checks if the user date of birth is diferent from the user date of birth saved, and if so trie to update 
        //    if (user.DateOfBirth != Input.DateOfBirth)
        //    {
        //        user.DateOfBirth = Input.DateOfBirth;
        //        var result = await _userManager.UpdateAsync(user);
        //        if (!result.Succeeded)
        //        {
        //            StatusMessage = $"{_stringLocalizer["StatusMessage_ErrorUpdate"]} {_stringLocalizer["Date Of Birth"]}";
        //            return RedirectToPage();
        //        }
        //    }

        //    if (Input.RemoveImage)
        //    {
        //        await CheckAndDeleteProfilePicture(user);
        //    }

        //    if (Input.File != null)
        //    {
        //        BlobResponseDto uploadResult = await _azureStorage.UploadAsync(Input.File);
        //        if (uploadResult.Error)
        //        {
        //            StatusMessage = $"{_stringLocalizer["StatusMessage_ErrorUpdate"]} {_stringLocalizer["Profile Picure"]}";
        //            return RedirectToPage();
        //        }
        //        await CheckAndDeleteProfilePicture(user);
        //        user.ProfilePicture = uploadResult.Blob;
        //        var result = await _userManager.UpdateAsync(user);
        //        if (!result.Succeeded)
        //        {
        //            StatusMessage = $"{_stringLocalizer["StatusMessage_ErrorUpdate"]} {_stringLocalizer["Profile Picure"]}";
        //            return RedirectToPage();
        //        }
        //    }

        //    await _signInManager.RefreshSignInAsync(user);
        //    StatusMessage = _stringLocalizer["StatusMessage_ProfileUpdate"];
        //    return RedirectToPage();
        //}

        //public string ShowRemoveButton() => ProfilePictureUrl.Contains("uploads") ? "" : "d-none";


        

        //private async Task CheckAndDeleteProfilePicture(User user)
        //{
        //    if (user.ProfilePicture != null)
        //    {
        //        await _azureStorage.DeleteAsync(user.ProfilePicture.Uuid);
        //        _context.BlobDto.Remove(user.ProfilePicture);
        //        user.ProfilePicture = null;
        //        //update user and save changes
        //        _context.Update(user);
        //        await _context.SaveChangesAsync();
        //    }
        //}
    }
}
