using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CompanyIntranet.Data;
using CompanyIntranet.Models;
using AutoMapper;
using CompanyIntranet.DTOs;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using CompanyIntranet.Services;
using Hangfire;

namespace CompanyIntranet.Controllers
{
   
    public class AnnouncementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AnnouncementController> _logger;
        private readonly IEmailSender _emailSender;

        public AnnouncementController(ApplicationDbContext context, IMapper mapper, ILogger<AnnouncementController> logger, IEmailSender emailSender)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _emailSender = emailSender;
        }

        // GET: Announcement
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var announcements = await _context.Announcements.ToListAsync();
            var dtoList = _mapper.Map<List<AnnouncementDto>>(announcements);
            return View(dtoList);
        }

        // GET: Announcement/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var announcement = await _context.Announcements
                .FirstOrDefaultAsync(m => m.Id == id);
            if (announcement == null)
            {
                return NotFound();
            }

            
            return View(announcement);
        }

        // GET: Announcement/Create
        [Authorize(Roles = "IK")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Announcement/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "IK")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AnnouncementDto dto)
        {
            if (ModelState.IsValid)
            {
                var announcement = _mapper.Map<Announcement>(dto);
                announcement.CreatedAt = DateTime.Now;
                announcement.CreatedBy = User.Identity?.Name ?? "anonymous";
                announcement.UpdatedAt = DateTime.Now;

                _context.Add(announcement);
                await _context.SaveChangesAsync();

                // email gönderme
                var allUserEmails = await _context.Users.Select(u => u.Email).ToListAsync();
                string subject = "Yeni Duyuru";
                string body = $"<h3>{announcement.Title}</h3><p>{announcement.Content}</p>";

                foreach (var email in allUserEmails)
                {
                    // Hangfire arka planda mail gönderme işi oluşturuyor
                    BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(email, subject, body));
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var modelState in ModelState)
                {
                    foreach (var error in modelState.Value.Errors)
                    {
                        Console.WriteLine($"{modelState.Key}: {error.ErrorMessage}");
                    }
                }

                
            }
                return View(dto);
        }

        // GET: Announcement/Edit/5
        [Authorize(Roles = "IK")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null)
            {
                return NotFound();
            }
            var dto = _mapper.Map<AnnouncementDto>(announcement);
            return View(dto);
        }

        // POST: Announcement/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "IK")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AnnouncementDto dto)
        {
            if (id != dto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var entity = await _context.Announcements.FindAsync(id);
                    if (entity == null) return NotFound();

                    _mapper.Map(dto, entity);
                    entity.UpdatedAt = DateTime.Now;
                    _context.Update(entity);
                    await _context.SaveChangesAsync();
                    
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!await AnnouncementExists(dto.Id))
                    {
                        _logger.LogWarning("Concurrency exception: Announcement with Id {Id} not found", dto.Id);
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError(ex, "Error while updating Announcement with Id {Id}", dto.Id);
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        // GET: Announcement/Delete/5
        [Authorize(Roles = "IK")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var announcement = await _context.Announcements
                .FirstOrDefaultAsync(m => m.Id == id);
            if (announcement == null)
            {
                return NotFound();
            }

            var dto = _mapper.Map<AnnouncementDto>(announcement);
            return View(dto);
        }

        // POST: Announcement/Delete/5
        [Authorize(Roles = "IK")]
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement != null)
            {
                _context.Announcements.Remove(announcement);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> AnnouncementExists(int id)
        {
            return await _context.Announcements.AnyAsync(e => e.Id == id);
        }
    }
}
