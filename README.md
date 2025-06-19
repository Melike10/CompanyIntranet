# 📢 Company Intranet

Bu proje, kurum içi bilgilendirme sisteminde **duyuruların yönetimini sağlayan bir intranet uygulamasıdır**. Duyurular hem arayüz üzerinden görüntülenebilir, hem de oluşturulduklarında ilgili kullanıcılara e-posta olarak iletilir. CRUD işlemleri yapılabilir ve işlemler arka planda **Hangfire** ile planlanabilir.

## 🎯 Proje Amacı

> Duyuruların IK (İnsan Kaynakları) tarafından yönetildiği, diğer kullanıcıların ise yalnızca görüntüleyebildiği bir yapı oluşturmak.  
> Her yeni duyuruda ilgili kullanıcılara **otomatik olarak e-posta gönderilmesi** hedeflenmiştir.

---
![Duyuru Listesi](https://github.com/Melike10/CompanyIntranet/blob/47ec00a9741f0206457a7964b8f8bc420dba2899/DuyuruSayfasi.png)
![Mail Gönderimi Kanıtı](https://github.com/Melike10/CompanyIntranet/blob/47ec00a9741f0206457a7964b8f8bc420dba2899/fakesender.png)

## 🛠️ Kullanılan Teknolojiler

| Yapı | Açıklama |
|------|----------|
| .NET 9 (ASP.NET Core MVC) | Ana uygulama çatısı |
| Entity Framework Core | Veritabanı işlemleri |
| MSSQL | Veritabanı yönetimi |
| AutoMapper | Entity-DTO dönüşümleri |
| Hangfire | Arka planda e-posta gönderimi (Background Job) |
| SMTP | E-posta gönderimi için (testlerde fake sender da kullanılmıştır) |
| xUnit & Moq | Unit testler için test altyapısı |
| FluentAssertions | Okunabilir assertion yazımı |
| Seed Data | Varsayılan kullanıcı ve rollerin otomatik oluşturulması |
| Authorization | Rol bazlı yetkilendirme (IK rolü vs.) |

---

## 📌 Özellikler

- ✅ IK rolündeki kullanıcılar duyuru oluşturabilir, güncelleyebilir ve silebilir.
- 👁️ Diğer roller yalnızca duyuruları görüntüleyebilir.
- 📬 Yeni bir duyuru oluşturulduğunda, sistemde tanımlı geçerli e-posta adreslerine otomatik e-posta gönderimi yapılır.
- 🔧 Tüm e-posta gönderim işlemleri **Hangfire** ile arka planda gerçekleştirilir.
- 🧪 CRUD işlemleri için kapsamlı **unit testler** yazılmıştır.
- 🧰 Kod yapısı **clean architecture** mantığına uygun olarak servis katmanlarına bölünmüştür.

---

## 🚀 Başlangıç

### 1. Gerekli NuGet Paketleri

```bash
dotnet add package Hangfire.AspNetCore
dotnet add package Hangfire.SqlServer
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package xunit
dotnet add package Moq
dotnet add package FluentAssertions
```
### 2. appsettings.json SMTP Ayarları
json
Copy
Edit
"Smtp": {
  "Host": "localhost", // veya smtp.gmail.com
  "Port": "1025",
  "User": "",
  "Pass": "",
  "FromEmail": "noreply@company.com",
  "FromName": "Company Intranet"
}
⚠️ Gerçek SMTP yerine test ortamı için MailDev ya da FakeEmailSender kullanılabilir.

### 3. Hangfire Dashboard
Uygulama çalıştığında şu adresten arka plan görevlerini görüntüleyebilirsiniz:
📍 http://localhost:5000/hangfire

## 🧪 Test
Aşağıdaki işlemler için xUnit ile test yazılmıştır:

Duyuru oluşturma

Güncelleme

Silme

Kullanıcı kimliği ile CreatedBy kontrolü

Mapper kullanımı


## Adım Adım Nasıl Yapıldı?

### 1. Test Projesi Oluşturuldu

* Visual Studio'da **Solution'a sağ tıklanarak > Add > New Project** seçildi.
* `xUnit Test Project (.NET Core)` şablonu ile `CompanyIntranet.Tests` adlı proje eklendi.

### 2. Gerekli NuGet Paketleri Yüklendi

Testlerde kullanmak için aşağıdaki paketler yüklendi:

* `xunit` – test frameworkü
* `Moq` – sahte nesneler (mock) oluşturmak için
* `Microsoft.EntityFrameworkCore.InMemory` – testlerde gerçek veritabanı yerine hafızada geçici veri kullanmak için
* `FluentAssertions` – daha okunabilir assert ifadeleri için

### 3. `AnnouncementControllerTests.cs` Dosyası Oluşturuldu

* `Tests/Controllers` klasörü açıldı ve bu sınıf eklendi.

```csharp
public class AnnouncementControllerTests
{
    // Test setup'ları ve metotlar buraya geldi
    
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
}
```

### 4. Sahte Ortam Kuruldu

* `DbContext`, `IMapper`, ve `ILogger` mock olarak kuruldu
* HTTP context için sahte `ClaimsPrincipal` tanımlandı

```csharp
var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
{
    new Claim(ClaimTypes.Name, "TestUser")
}, "mock"));
```

### 5. Test Metotları Yazıldı (`[Fact]`)

####  Create Testi:

```csharp
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
```

* `AnnouncementDto` nesnesi verildi.
* Mapper yardımıyla `Announcement` nesnesine çevrildi.
* Controller'da sahte kullanıcı bilgisi sağlandı (`CreatedBy`).
* Kayıt eklendi mi, title doğru mu kontrol edildi.

#### ? Edit Testi:

```csharp
[Fact]
public async Task Edit_UpdatesAnnouncement_WhenIdIsValid() {
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
```

* Hafızada eski bir duyuru yaratıldı
* DTO verileri ile güncelleme yapıldı
* Yeni değerlerin veritabanına yansıdığı kontrol edildi

#### DeleteConfirmed Testi:

```csharp
[Fact]
public async Task DeleteConfirmed_RemovesAnnouncement_WhenIdIsValid(){
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

```

* Var olan duyuru silindi mi kontrol edildi
* `FindAsync` ile null olup olmadığı test edildi

---

##  Sonuç:

Testler şu senaryoları geçti:

* Duyuru doğru kaydediliyor mu?
* Duyuru doğru güncelleniyor mu?
* Duyuru siliniyor mu?





---




