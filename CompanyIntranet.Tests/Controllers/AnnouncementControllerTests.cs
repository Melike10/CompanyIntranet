using AutoMapper;
using CompanyIntranet.Controllers;
using CompanyIntranet.Data;
using CompanyIntranet.DTOs;
using CompanyIntranet.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using CompanyIntranet.Services;

namespace CompanyIntranet.Tests.Controllers
{
    public class AnnouncementControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AnnouncementController _controller;
        private readonly Mock<ILogger<AnnouncementController>> _loggerMock;
        private readonly Mock<IEmailSender> _emailSenderMock;


        public AnnouncementControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // her test için ayrı DB
                .Options;

            _context = new ApplicationDbContext(options);
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<AnnouncementController>>();
            _emailSenderMock = new Mock<IEmailSender>();

            _controller = new AnnouncementController(
                _context,
                _mapperMock.Object,
                _loggerMock.Object,
                _emailSenderMock.Object 
            );

        }

        [Fact]
        public async Task Create_AddsAnnouncement_WhenModelIsValid()
        {
            // Arrange
            var dto = new AnnouncementDto { Title = "Test Başlık", Content = "Test içerik" };
            var mapped = new Announcement { Title = dto.Title, Content = dto.Content };

            _mapperMock.Setup(m => m.Map<Announcement>(dto)).Returns(mapped);

            // Fake user
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Name, "TestUser")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var list = await _context.Announcements.ToListAsync();
            list.Should().HaveCount(1);
            list[0].Title.Should().Be("Test Başlık");
            list[0].CreatedBy.Should().Be("TestUser");
            result.Should().BeOfType<RedirectToActionResult>();
        }


        [Fact]
        public async Task Edit_UpdatesAnnouncement_WhenIdIsValid()
        {
            // Arrange
            var existing = new Announcement
            {
                Id = 1,
                Title = "Old",
                Content = "Old content",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                CreatedBy = "admin"
            };
            _context.Announcements.Add(existing);
            await _context.SaveChangesAsync();

            var dto = new AnnouncementDto { Id = 1, Title = "New", Content = "New content" };

            _mapperMock.Setup(m => m.Map(dto, existing)).Callback<AnnouncementDto, Announcement>((src, dest) =>
            {
                dest.Title = src.Title;
                dest.Content = src.Content;
            });

            // Act
            var result = await _controller.Edit(1, dto);

            // Assert
            var updated = await _context.Announcements.FindAsync(1);
            updated.Title.Should().Be("New");
            updated.Content.Should().Be("New content");
            result.Should().BeOfType<RedirectToActionResult>();
        }
        [Fact]
        public async Task DeleteConfirmed_RemovesAnnouncement_WhenIdIsValid()
        {
            // Arrange
            var announcement = new Announcement
            {
                Title = "Silinecek",
                Content = "...",
                CreatedBy = "TestUser",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();

            var toDelete = await _context.Announcements.FirstOrDefaultAsync();

            // Act
            var result = await _controller.DeleteConfirmed(toDelete.Id);

            // Assert
            var deleted = await _context.Announcements.FindAsync(toDelete.Id);
            deleted.Should().BeNull();
            result.Should().BeOfType<RedirectToActionResult>();
        }


    }
}
