using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SCManagement.Data;
using SCManagement.Models;

namespace SCManagement.Controllers {
    /// <summary>
    /// This class represents the Clubs Controller
    /// </summary>
    /// 

    public class ClubsController : Controller {

        private readonly ApplicationDbContext _context;

        public ClubsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Clubs
        public async Task<IActionResult> Index()
        {
            return View(await _context.Club.Include(c => c.Modalities).ToListAsync());
        }

        // GET: Clubs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Club == null)
            {
                return NotFound();
            }

            var club = await _context.Club.Include(c => c.Modalities).FirstOrDefaultAsync(m => m.Id == id);

            ViewBag.Modalities = new SelectList(club.Modalities, "Id", "Name");

            if (club == null)
            {
                return NotFound();
            }

            return View(club);
        }



        // GET: Clubs/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewBag.Modalities = new SelectList(_context.Modalities.ToList(), "Id", "Name");
            return View();
        }

        // POST: Clubs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ModalitiesIds")] Club club)
        {
            List<Modality> modalities = await _context.Modalities.Where(m => club.ModalitiesIds.Contains(m.Id)).ToListAsync();

            if (!ModelState.IsValid) return View();

            Club c = new Club
            {
                Name = club.Name,
                Modalities = modalities,
                CreationDate = DateTime.Now
            };

            _context.Club.Add(c);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        // GET: Clubs/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Club == null) return NotFound();

            var club = await _context.Club.Include(c => c.Modalities).FirstOrDefaultAsync(c => c.Id == id);

            List<int> ClubModalitiesIds = club.Modalities.Select(m => m.Id).ToList();

            ViewBag.Modalities = new MultiSelectList(_context.Modalities.ToList(), "Id", "Name", ClubModalitiesIds);

            if (club == null) return NotFound();

            return View(club);
        }

        // POST: Clubs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,PhoneNumber,About,CreationDate,EndDate,Photography,ModalitiesIds")] Club club)
        {
            if (id != club.Id) return NotFound();
            if (!ModelState.IsValid) return View(club);
            var actualClub = _context.Club.Include(c => c.Modalities).FirstOrDefault(c => c.Id == id);
            if (actualClub == null) return NotFound();

            List<Modality> newModalities = await _context.Modalities.Where(m => club.ModalitiesIds.Contains(m.Id)).ToListAsync();
            //remove from club modalities which are not in the new modalities list
            if (actualClub.Modalities != null)
            {
                foreach (Modality m in actualClub.Modalities.ToList())
                {
                    if (!newModalities.Contains(m))
                    {
                        actualClub.Modalities.Remove(m);
                    }
                }

                //add to club modalities the modalities that are in the new modalities list and aren't on club modalities list already
                foreach (Modality m in newModalities.ToList())
                {
                    if (!actualClub.Modalities.Contains(m))
                    {
                        actualClub.Modalities.Add(m);
                    }
                }
            }
            else
            {
                actualClub.Modalities = newModalities;
            }

            actualClub.Name = club.Name;
            actualClub.Email = club.Email;
            actualClub.PhoneNumber = club.PhoneNumber;
            actualClub.About = club.About;

            _context.Club.Update(actualClub);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

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
