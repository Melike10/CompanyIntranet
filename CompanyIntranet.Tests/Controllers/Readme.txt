# ?? Unit Test README – AnnouncementController

Bu dosya, CompanyIntranet projesindeki `AnnouncementController` üzerinden gerçekleþtirilen **Unit Test** uygulamasýný adým adým anlatýr. Hedefimiz, `Create`, `Edit` ve `DeleteConfirmed` metotlarýnýn doðru çalýþýp çalýþmadýðýný otomatik testlerle kontrol etmektir.

---

## ? Adým Adým Nasýl Yapýldý?

### 1. Test Projesi Oluþturuldu

* Visual Studio'da **Solution'a sað týklanarak > Add > New Project** seçildi.
* `xUnit Test Project (.NET Core)` þablonu ile `CompanyIntranet.Tests` adlý proje eklendi.

### 2. Gerekli NuGet Paketleri Yüklendi

Testlerde kullanmak için aþaðýdaki paketler yüklendi:

* `xunit` – test frameworkü
* `Moq` – sahte nesneler (mock) oluþturmak için
* `Microsoft.EntityFrameworkCore.InMemory` – testlerde gerçek veritabaný yerine hafýzada geçici veri kullanmak için
* `FluentAssertions` – daha okunabilir assert ifadeleri için

### 3. `AnnouncementControllerTests.cs` Dosyasý Oluþturuldu

* `Tests/Controllers` klasörü açýldý ve bu sýnýf eklendi.

```csharp
public class AnnouncementControllerTests
{
    // Test setup'larý ve metotlar buraya geldi
}
```

### 4. Sahte Ortam Kuruldu

* `DbContext`, `IMapper`, ve `ILogger` mock olarak kuruldu
* HTTP context için sahte `ClaimsPrincipal` tanýmlandý

```csharp
var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
{
    new Claim(ClaimTypes.Name, "TestUser")
}, "mock"));
```

### 5. Test Metotlarý Yazýldý (`[Fact]`)

#### ? Create Testi:

```csharp
[Fact]
public async Task Create_AddsAnnouncement_WhenModelIsValid()
```

* `AnnouncementDto` nesnesi verildi.
* Mapper yardýmýyla `Announcement` nesnesine çevrildi.
* Controller'da sahte kullanýcý bilgisi saðlandý (`CreatedBy`).
* Kayýt eklendi mi, title doðru mu kontrol edildi.

#### ? Edit Testi:

```csharp
[Fact]
public async Task Edit_UpdatesAnnouncement_WhenIdIsValid()
```

* Hafýzada eski bir duyuru yaratýldý
* DTO verileri ile güncelleme yapýldý
* Yeni deðerlerin veritabanýna yansýdýðý kontrol edildi

#### ? DeleteConfirmed Testi:

```csharp
[Fact]
public async Task DeleteConfirmed_RemovesAnnouncement_WhenIdIsValid()
```

* Var olan duyuru silindi mi kontrol edildi
* `FindAsync` ile null olup olmadýðý test edildi

---

## ?? Sonuç:

Testler þu senaryolarý geçti:

* Duyuru doðru kaydediliyor mu?
* Duyuru doðru güncelleniyor mu?
* Duyuru siliniyor mu?

---


