# GlowBook – Salon Planner (WPF .NET 9)

**Auteur:** Radmila Tantaeva  
**Partnerbedrijf:** Glowy Skin Salon  
**Type:** Individueel examenproject (.NET Frameworks)

---

## Overzicht
GlowBook is een **WPF desktopapplicatie (.NET 9, MVVM)** voor het plannen en beheren van afspraken in schoonheidssalons.  
De app ondersteunt klantenbeheer, behandelingen, medewerkers, afspraken en eenvoudige rapportage.  
Gerealiseerd in samenwerking met **Glowy Skin Salon**.

---

## Techniek
- .NET 9.x – WPF (XAML + C#, MVVM)
- Solution met 2 projecten:
  - **GlowBook.Model** : modellen, Entity Framework, Identity, migraties  
  - **GlowBook.Wpf** : UI, ViewModels, services en logica
- **Entity Framework Core** met eigen `AppDbContext` (afgeleid van `IdentityDbContext<ApplicationUser>`)
- **Seeders** in `Seed.cs` vullen dummydata in voor testgebruik
- **Soft delete** via `BaseEntity`
- **Identity Framework** met aangepaste `ApplicationUser` (extra eigenschappen)
- **Foutafhandeling** via `try/catch` en dialogen
- **Data binding**, **styles**, **RelayCommand**, en **MVVM**-structuur toegepast

---

## Installatie
1. **Clone de repository**  
git clone https://github.com/milatxx/GlowBook.git
2. **Open GlowBook.sln in Visual Studio 2022+**
GlowBook.sln
3. **Herstel packages**
dotnet restore
4. **Maak de database aan**
Update-Database
5. **Stel GlowBook.Wpf in als startup-project**
6. **Start de app met F5**

---

## Toekomstige uitbreidingen
- ASP.NET Web-API en webversie
- MAUI mobiele app
- iCal-export en notificaties

---

© 2025 Radmila Tantaeva – voor educatief gebruik.

---

## Bronnen
- Microsoft Learn – [WPF Documentation](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)
- Microsoft Learn – [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- Stack Overflow – diverse voorbeelden voor data binding en MVVM, ...
- ChatGPT – hulp bij documentatie en code review

