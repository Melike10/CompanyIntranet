# ?? Unit Test README � AnnouncementController

Bu dosya, CompanyIntranet projesindeki `AnnouncementController` �zerinden ger�ekle�tirilen **Unit Test** uygulamas�n� ad�m ad�m anlat�r. Hedefimiz, `Create`, `Edit` ve `DeleteConfirmed` metotlar�n�n do�ru �al���p �al��mad���n� otomatik testlerle kontrol etmektir.

---

## ? Ad�m Ad�m Nas�l Yap�ld�?

### 1. Test Projesi Olu�turuldu

* Visual Studio'da **Solution'a sa� t�klanarak > Add > New Project** se�ildi.
* `xUnit Test Project (.NET Core)` �ablonu ile `CompanyIntranet.Tests` adl� proje eklendi.

### 2. Gerekli NuGet Paketleri Y�klendi

Testlerde kullanmak i�in a�a��daki paketler y�klendi:

* `xunit` � test framework�
* `Moq` � sahte nesneler (mock) olu�turmak i�in
* `Microsoft.EntityFrameworkCore.InMemory` � testlerde ger�ek veritaban� yerine haf�zada ge�ici veri kullanmak i�in
* `FluentAssertions` � daha okunabilir assert ifadeleri i�in

### 3. `AnnouncementControllerTests.cs` Dosyas� Olu�turuldu

* `Tests/Controllers` klas�r� a��ld� ve bu s�n�f eklendi.

```csharp
public class AnnouncementControllerTests
{
    // Test setup'lar� ve metotlar buraya geldi
}
```

### 4. Sahte Ortam Kuruldu

* `DbContext`, `IMapper`, ve `ILogger` mock olarak kuruldu
* HTTP context i�in sahte `ClaimsPrincipal` tan�mland�

```csharp
var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
{
    new Claim(ClaimTypes.Name, "TestUser")
}, "mock"));
```

### 5. Test Metotlar� Yaz�ld� (`[Fact]`)

#### ? Create Testi:

```csharp
[Fact]
public async Task Create_AddsAnnouncement_WhenModelIsValid()
```

* `AnnouncementDto` nesnesi verildi.
* Mapper yard�m�yla `Announcement` nesnesine �evrildi.
* Controller'da sahte kullan�c� bilgisi sa�land� (`CreatedBy`).
* Kay�t eklendi mi, title do�ru mu kontrol edildi.

#### ? Edit Testi:

```csharp
[Fact]
public async Task Edit_UpdatesAnnouncement_WhenIdIsValid()
```

* Haf�zada eski bir duyuru yarat�ld�
* DTO verileri ile g�ncelleme yap�ld�
* Yeni de�erlerin veritaban�na yans�d��� kontrol edildi

#### ? DeleteConfirmed Testi:

```csharp
[Fact]
public async Task DeleteConfirmed_RemovesAnnouncement_WhenIdIsValid()
```

* Var olan duyuru silindi mi kontrol edildi
* `FindAsync` ile null olup olmad��� test edildi

---

## ?? Sonu�:

Testler �u senaryolar� ge�ti:

* Duyuru do�ru kaydediliyor mu?
* Duyuru do�ru g�ncelleniyor mu?
* Duyuru siliniyor mu?

---


