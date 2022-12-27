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

namespace SCManagement.Controllers
{
    /// <summary>
    /// This class represents the Clubs Controller
    /// </summary>
    /// 

    [Authorize]
    public class ClubsController : Controller
    {
        
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

            var club = await _context.Club
                .FirstOrDefaultAsync(m => m.Id == id);
            if (club == null)
            {
                return NotFound();
            }

            return View(club);
        }

        // GET: Clubs/Create
        public IActionResult Create()
        {
            ViewBag.Modalities = new SelectList(_context.Modalities.ToList(), "Id", "Name");
            return View();
        }

        // POST: Clubs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ModalitiesIds")] Club club)
        { 
            
            if (ModelState.IsValid)
            {
                club.CreationDate = DateTime.Now;
                _context.Add(club);

                await _context.SaveChangesAsync();

                foreach (int id in club.ModalitiesIds)
                {
                    _context.ModalitiesClubs.Add(new ModalitiesClub { ClubId = club.Id, ModalityId = id });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(club);
        }

        // GET: Clubs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Club == null)
            {
                return NotFound();
            }
            var club = await _context.Club.FindAsync(id);

            //Modalities of the club
            var clubModalities = _context.ModalitiesClubs.Where(c => c.ClubId == club.Id).ToList();
            var modalities = _context.Modalities.ToList();
            //List<Modality> modalities1 = new List<Modality>();

            //modalities that the club in clubModalities doesnt have
            foreach (ModalitiesClub m in clubModalities)
            {
                modalities.Remove(_context.Modalities.Find(m.ModalityId));
                //modalities1.Add(_context.Modalities.Find(m.ModalityId));
            }

            ViewBag.Modalities = new SelectList(modalities, "Id", "Name");
            //ViewBag.removeModalities = new SelectList(modalities1, "Id", "Name");

            if (club == null)
            {
                return NotFound();
            }
            return View(club);
        }

        // POST: Clubs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,PhoneNumber,About,CreationDate,EndDate,Photography,ModalitiesIds")] Club club)
        {
            if (id != club.Id)
            {
                return NotFound();
            }

            foreach (int mId in club.ModalitiesIds)
            {
                _context.ModalitiesClubs.Add(new ModalitiesClub { ClubId = club.Id, ModalityId = mId });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(club);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClubExists(club.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(club);
        }

        // GET: Clubs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Club == null)
            {
                return NotFound();
            }

            var club = await _context.Club
                .FirstOrDefaultAsync(m => m.Id == id);
            if (club == null)
            {
                return NotFound();
            }

            return View(club);
        }

        // POST: Clubs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Club == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Club'  is null.");
            }
            var club = await _context.Club.FindAsync(id);
            if (club != null)
            {
                _context.Club.Remove(club);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClubExists(int id)
        {
          return _context.Club.Any(e => e.Id == id);
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
