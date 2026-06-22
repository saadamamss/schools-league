using Common;
using Config;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Data;

public class DbSeedr
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;
        var db = sp.GetRequiredService<AppDbContext>();
        var userManager = sp.GetRequiredService<UserManager<User>>();
        var roleManager = sp.GetRequiredService<RoleManager<Role>>();
        var seedSettings = sp.GetRequiredService<IOptions<SeedSettings>>().Value;

        if (await db.Users.AnyAsync())
            return;

        // ── Roles ────────────────────────────────────────────────
        foreach (var role in new[] { "Admin", "User", "Supervisor" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new Role { Name = role });
        }

        // ── Lookup tables ────────────────────────────────────────
        if (!await db.Cities.AnyAsync())
        {
            db.Cities.AddRange(
                new City { NameAr = "الرياض", NameEn = "Riyadh" },
                new City { NameAr = "جدة", NameEn = "Jeddah" },
                new City { NameAr = "مكة المكرمة", NameEn = "Makkah" },
                new City { NameAr = "المدينة المنورة", NameEn = "Madinah" },
                new City { NameAr = "الدمام", NameEn = "Dammam" },
                new City { NameAr = "الخبر", NameEn = "Khobar" },
                new City { NameAr = "تبوك", NameEn = "Tabuk" },
                new City { NameAr = "أبها", NameEn = "Abha" }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.UserTypes.AnyAsync())
        {
            db.UserTypes.AddRange(
                new UserType { Code = "admin", NameAr = "مدير", NameEn = "Admin", Key = "admin", IsActive = true },
                new UserType { Code = "supervisor", NameAr = "مشرف", NameEn = "Supervisor", Key = "supervisor", IsActive = true },
                new UserType { Code = "employee", NameAr = "موظف", NameEn = "Employee", Key = "employee", IsActive = true }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.Nationalities.AnyAsync())
        {
            db.Nationalities.AddRange(
                new Nationality { NameAr = "سعودي", NameEn = "Saudi", Code = "SA" },
                new Nationality { NameAr = "مصري", NameEn = "Egyptian", Code = "EG" },
                new Nationality { NameAr = "أردني", NameEn = "Jordanian", Code = "JO" },
                new Nationality { NameAr = "سوري", NameEn = "Syrian", Code = "SY" },
                new Nationality { NameAr = "يمني", NameEn = "Yemeni", Code = "YE" }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.PaymentTypes.AnyAsync())
        {
            db.PaymentTypes.AddRange(
                new PaymentType { Name = "راتب" },
                new PaymentType { Name = "مكافأة" },
                new PaymentType { Name = "خصم" },
                new PaymentType { Name = "سلفة" }
            );
            await db.SaveChangesAsync();
        }

        var riyadh = await db.Cities.FirstAsync(c => c.NameEn == "Riyadh");
        var jeddah = await db.Cities.FirstAsync(c => c.NameEn == "Jeddah");
        var makkah = await db.Cities.FirstAsync(c => c.NameEn == "Makkah");
        var madinah = await db.Cities.FirstAsync(c => c.NameEn == "Madinah");
        var dammam = await db.Cities.FirstAsync(c => c.NameEn == "Dammam");
        var khobar = await db.Cities.FirstAsync(c => c.NameEn == "Khobar");
        var tabuk = await db.Cities.FirstAsync(c => c.NameEn == "Tabuk");
        var abha = await db.Cities.FirstAsync(c => c.NameEn == "Abha");

        var adminType = await db.UserTypes.FirstAsync(t => t.Code == "admin");
        var supervisorType = await db.UserTypes.FirstAsync(t => t.Code == "supervisor");
        var empType = await db.UserTypes.FirstAsync(t => t.Code == "employee");

        var natSa = await db.Nationalities.FirstAsync(n => n.Code == "SA");
        var natEg = await db.Nationalities.FirstAsync(n => n.Code == "EG");
        var natJo = await db.Nationalities.FirstAsync(n => n.Code == "JO");
        var natSy = await db.Nationalities.FirstAsync(n => n.Code == "SY");
        var natYe = await db.Nationalities.FirstAsync(n => n.Code == "YE");

        // ── Users ────────────────────────────────────────────────
        var allUsers = new List<User>();

        // Admin
        var admin = new User
        {
            UserName = "admin@schools-league.com",
            Email = "admin@schools-league.com",
            FullName = "مدير النظام",
            FirstName = "مدير",
            LastName = "النظام",
            PhoneNumber = "0500000000",
            SaId = "1000000000",
            Gender = "male",
            CityId = riyadh.Id,
            UserTypeId = adminType.Id,
            NationalityId = natSa.Id,
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
        };
        if (!await db.Users.AnyAsync(u => u.Email == admin.Email))
        {
            var r = await userManager.CreateAsync(admin, seedSettings.AdminPassword);
            if (r.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
                await userManager.AddToRoleAsync(admin, "User");
                allUsers.Add(admin);
            }
        }

        // ── Supervisors ──────────────────────────────────────────
        var superData = new[]
        {
            ("supervisor", "أحمد السلمي", "أحمد", "", "السلمي", "0506666661", "1000000006", "male", natSa, riyadh, 6000m, "الرياض", "SA2030000012345678", "SA0380000000012345678001", "RIYBBANK"),
            ("supervisor2", "فاطمة الزهراني", "فاطمة", "سعيد", "الزهراني", "0506666662", "1000000007", "female", natSa, jeddah, 5500m, "الراجحي", "SA1530000012345679", "SA1580000000012345679001", "RAJHBANK"),
        };
        var supervisors = new List<User>();
        foreach (var (uname, full, first, middle, last, phone, saId, gender, nat, city, rate, bank, acct, iban, swift) in superData)
        {
            var email = $"{uname}@schools-league.com";
            if (!await db.Users.AnyAsync(u => u.Email == email))
            {
                var u = new User
                {
                    UserName = email, Email = email, FullName = full,
                    FirstName = first, MiddleName = middle, LastName = last,
                    PhoneNumber = phone, SaId = saId, Gender = gender,
                    CityId = city.Id, UserTypeId = supervisorType.Id,
                    NationalityId = nat.Id, DailyRate = rate,
                    BankName = bank, AccountNumber = acct, Iban = iban, SwiftCode = swift,
                    EmailConfirmed = true, PhoneNumberConfirmed = true,
                };
                var r = await userManager.CreateAsync(u, seedSettings.UserPassword);
                if (r.Succeeded)
                {
                    await userManager.AddToRoleAsync(u, "Supervisor");
                    await userManager.AddToRoleAsync(u, "User");
                    supervisors.Add(u);
                    allUsers.Add(u);
                }
            }
        }

        // ── Employees ────────────────────────────────────────────
        var empData = new[]
        {
            ("emp1", "محمد العوفي", "محمد", "سالم", "العوفي", "0501111111", "1000000008", "male", natSa, riyadh, 4500m, "الأهلي", "SA6530000012345680", "SA6580000000012345680001", "NCBKSAJE"),
            ("emp2", "نورة الدوسري", "نورة", "", "الدوسري", "0502222222", "1000000009", "female", natSa, riyadh, 4200m, "الرياض", "SA1230000012345681", null, "RIYBBANK"),
            ("emp3", "خالد الأحمدي", "خالد", "عبدالله", "الأحمدي", "0503333333", "1000000010", "male", natSa, makkah, 4800m, "الإنماء", "SA9830000012345682", "SA9880000000012345682001", "INMASISA"),
            ("emp4", "سارة الحربي", "سارة", "عمر", "الحربي", "0504444444", "1000000011", "female", natSa, jeddah, 4000m, "الأهلي", "SA6530000012345683", "SA6580000000012345683001", "NCBKSAJE"),
            ("emp5", "عمر الغامدي", "عمر", "حسن", "الغامدي", "0505555555", "1000000012", "male", natSa, dammam, 5000m, "سامبا", "SA3430000012345684", "SA3480000000012345684001", "SAMBSARI"),
            ("emp6", "ليلى الشمراني", "ليلى", "مبارك", "الشمراني", "0507777777", "1000000013", "female", natEg, jeddah, 3800m, null, null, null, null),
            ("emp7", "ماجد القحطاني", "ماجد", "فهد", "القحطاني", "0508888888", "1000000014", "male", natSa, abha, 5200m, "البلاد", "SA4330000012345685", null, "ALBLSAJE"),
            ("emp8", "هند الزهراني", "هند", "علي", "الزهراني", "0509999999", "1000000015", "female", natJo, tabuk, 3600m, null, null, null, null),
            ("emp9", "فيصل المطيري", "فيصل", "نايف", "المطيري", "0510000001", "1000000016", "male", natSy, khobar, 4600m, "الأول", "SA7630000012345686", "SA7680000000012345686001", "AUBSISJE"),
            ("emp10", "ريم الجهني", "ريم", "محمد", "الجهني", "0510000002", "1000000017", "female", natSa, madinah, 4100m, "الراجحي", "SA1530000012345687", "SA1580000000012345687001", "RAJHBANK"),
        };
        var employees = new List<User>();
        foreach (var (uname, full, first, middle, last, phone, saId, gender, nat, city, rate, bank, acct, iban, swift) in empData)
        {
            var email = $"{uname}@schools-league.com";
            if (!await db.Users.AnyAsync(u => u.Email == email))
            {
                var u = new User
                {
                    UserName = email, Email = email, FullName = full,
                    FirstName = first, MiddleName = middle, LastName = last,
                    PhoneNumber = phone, SaId = saId, Gender = gender,
                    CityId = city.Id, UserTypeId = empType.Id,
                    NationalityId = nat.Id, DailyRate = rate,
                    BankName = bank, AccountNumber = acct, Iban = iban, SwiftCode = swift,
                    EmailConfirmed = true, PhoneNumberConfirmed = true,
                };
                var r = await userManager.CreateAsync(u, seedSettings.UserPassword);
                if (r.Succeeded)
                {
                    await userManager.AddToRoleAsync(u, "User");
                    employees.Add(u);
                    allUsers.Add(u);
                }
            }
        }

        // ── Locations ────────────────────────────────────────────
        if (!await db.Locations.AnyAsync())
        {
            db.Locations.AddRange(
                new Location
                {
                    Name = "مدرسة النور", Type = "school", IsActive = true,
                    CityId = riyadh.Id, License = "SCH-RUH-001", Phone = "0112001001",
                    Address = "حي النور، شارع الملك فهد، الرياض",
                    Latitude = 24.7136, Longitude = 46.6753,
                    JoinedEmployee = 8, ObserverCount = 1, WorkHours = "8:00-15:00", WorkType = "morning",
                },
                new Location
                {
                    Name = "مدرسة الفلاح", Type = "school", IsActive = true,
                    CityId = jeddah.Id, License = "SCH-JED-002", Phone = "0122002002",
                    Address = "حي الفلاح، شارع الأمير سلطان، جدة",
                    Latitude = 21.4858, Longitude = 39.1925,
                    JoinedEmployee = 6, ObserverCount = 1, WorkHours = "7:30-14:30", WorkType = "morning",
                },
                new Location
                {
                    Name = "معهد الإبداع", Type = "institute", IsActive = true,
                    CityId = makkah.Id, License = "INS-MAK-003", Phone = "0122003003",
                    Address = "حي العوالي، شارع إبراهيم الخليل، مكة",
                    Latitude = 21.3891, Longitude = 39.8579,
                    JoinedEmployee = 5, ObserverCount = 1, WorkHours = "9:00-16:00", WorkType = "morning",
                },
                new Location
                {
                    Name = "مركز التدريب", Type = "center", IsActive = true,
                    CityId = dammam.Id, License = "CTR-DMM-004", Phone = "0132004004",
                    Address = "حي الشاطئ، شارع الملك عبدالله، الدمام",
                    Latitude = 26.4207, Longitude = 50.0888,
                    JoinedEmployee = 4, ObserverCount = 1, WorkHours = "8:00-16:00", WorkType = "full",
                },
                new Location
                {
                    Name = "مكتب الخبر", Type = "office", IsActive = true,
                    CityId = khobar.Id, License = "OFF-KHB-005", Phone = "0132005005",
                    Address = "حي العقربية، شارع الظهران، الخبر",
                    Latitude = 26.2793, Longitude = 50.2079,
                    JoinedEmployee = 3, ObserverCount = 0, WorkHours = "8:30-15:30", WorkType = "morning",
                },
                new Location
                {
                    Name = "مركز أبها", Type = "center", IsActive = true,
                    CityId = abha.Id, License = "CTR-ABH-006", Phone = "0172006006",
                    Address = "حي المنسك، طريق الملك عبدالعزيز، أبها",
                    Latitude = 18.2164, Longitude = 42.5053,
                    JoinedEmployee = 5, ObserverCount = 1, WorkHours = "8:00-15:00", WorkType = "morning",
                }
            );
            await db.SaveChangesAsync();
        }

        var locations = await db.Locations.Where(l => !l.IsDeleted).ToListAsync();

        // ── UserLocationAssignments ──────────────────────────────
        if (!await db.UserLocationAssignments.AnyAsync())
        {
            var assignments = new List<UserLocationAssignment>();
            var now = DateTime.UtcNow;

            // Supervisor 1 → مدرسة النور (مشرف)
            assignments.Add(new UserLocationAssignment
            { UserId = supervisors[0].Id, LocationId = locations[0].Id, Role = "supervisor", AssignedAt = now.AddMonths(-6) });
            // Supervisor 1 also → مركز التدريب (مشرف)
            assignments.Add(new UserLocationAssignment
            { UserId = supervisors[0].Id, LocationId = locations[3].Id, Role = "supervisor", AssignedAt = now.AddMonths(-6) });

            // Supervisor 2 → مدرسة الفلاح (مشرف)
            assignments.Add(new UserLocationAssignment
            { UserId = supervisors[1].Id, LocationId = locations[1].Id, Role = "supervisor", AssignedAt = now.AddMonths(-4) });

            // Assign employees to locations
            // مدرسة النور (loc[0]): emp1, emp2
            assignments.Add(new UserLocationAssignment
            { UserId = employees[0].Id, LocationId = locations[0].Id, Role = "employee", AssignedAt = now.AddMonths(-6) });
            assignments.Add(new UserLocationAssignment
            { UserId = employees[1].Id, LocationId = locations[0].Id, Role = "employee", AssignedAt = now.AddMonths(-3) });

            // مدرسة الفلاح (loc[1]): emp3, emp4
            assignments.Add(new UserLocationAssignment
            { UserId = employees[2].Id, LocationId = locations[1].Id, Role = "employee", AssignedAt = now.AddMonths(-5) });
            assignments.Add(new UserLocationAssignment
            { UserId = employees[3].Id, LocationId = locations[1].Id, Role = "employee", AssignedAt = now.AddMonths(-4) });

            // معهد الإبداع (loc[2]): emp5, emp6
            assignments.Add(new UserLocationAssignment
            { UserId = employees[4].Id, LocationId = locations[2].Id, Role = "employee", AssignedAt = now.AddMonths(-3) });
            assignments.Add(new UserLocationAssignment
            { UserId = employees[5].Id, LocationId = locations[2].Id, Role = "employee", AssignedAt = now.AddMonths(-2) });

            // مركز التدريب (loc[3]): emp7, emp8
            assignments.Add(new UserLocationAssignment
            { UserId = employees[6].Id, LocationId = locations[3].Id, Role = "employee", AssignedAt = now.AddMonths(-4) });
            assignments.Add(new UserLocationAssignment
            { UserId = employees[7].Id, LocationId = locations[3].Id, Role = "employee", AssignedAt = now.AddMonths(-3) });

            // مكتب الخبر (loc[4]): emp9
            assignments.Add(new UserLocationAssignment
            { UserId = employees[8].Id, LocationId = locations[4].Id, Role = "employee", AssignedAt = now.AddMonths(-2) });

            // مركز أبها (loc[5]): emp10
            assignments.Add(new UserLocationAssignment
            { UserId = employees[9].Id, LocationId = locations[5].Id, Role = "employee", AssignedAt = now.AddMonths(-1) });

            db.UserLocationAssignments.AddRange(assignments);
            await db.SaveChangesAsync();
        }

        // ── Shifts ───────────────────────────────────────────────
        if (!await db.Shifts.AnyAsync())
        {
            db.Shifts.AddRange(
                new Shift { Name = "الفترة الصباحية - النور", StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(15, 0, 0), LocationId = locations[0].Id, IsActive = true },
                new Shift { Name = "الفترة المسائية - النور", StartTime = new TimeSpan(15, 0, 0), EndTime = new TimeSpan(20, 0, 0), LocationId = locations[0].Id, IsActive = true },
                new Shift { Name = "الفترة الصباحية - الفلاح", StartTime = new TimeSpan(7, 30, 0), EndTime = new TimeSpan(14, 30, 0), LocationId = locations[1].Id, IsActive = true },
                new Shift { Name = "الفترة الصباحية - الإبداع", StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(16, 0, 0), LocationId = locations[2].Id, IsActive = true },
                new Shift { Name = "الفترة الكاملة - التدريب", StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(16, 0, 0), LocationId = locations[3].Id, IsActive = true },
                new Shift { Name = "الفترة الصباحية - الخبر", StartTime = new TimeSpan(8, 30, 0), EndTime = new TimeSpan(15, 30, 0), LocationId = locations[4].Id, IsActive = true },
                new Shift { Name = "الفترة الصباحية - أبها", StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(15, 0, 0), LocationId = locations[5].Id, IsActive = true }
            );
            await db.SaveChangesAsync();
        }

        // ── Attendances ──────────────────────────────────────────
        if (!await db.Attendances.AnyAsync())
        {
            var rng = new Random(42);
            var today = DateTime.UtcNow.Date;
            var startDate = today.AddDays(-29);
            var assignedLocations = await db.UserLocationAssignments
                .Where(a => a.UnassignedAt == null && !a.IsDeleted)
                .ToListAsync();

            var attendances = new List<Attendance>();
            for (var date = startDate; date <= today; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Friday)
                    continue;

                foreach (var assignment in assignedLocations)
                {
                    var isPresent = rng.NextDouble() < 0.85;
                    var checkInHour = 7 + rng.Next(3);
                    var checkOutHour = 14 + rng.Next(3);

                    attendances.Add(new Attendance
                    {
                        UserId = assignment.UserId,
                        LocationId = assignment.LocationId,
                        Date = date,
                        Status = isPresent ? "attendance" : "absent",
                        CheckIn = isPresent
                            ? date.AddHours(checkInHour).AddMinutes(rng.Next(60))
                            : null,
                        CheckOut = isPresent
                            ? date.AddHours(checkOutHour).AddMinutes(rng.Next(60))
                            : null,
                    });
                }
            }

            db.Attendances.AddRange(attendances);
            await db.SaveChangesAsync();
        }

        // ── FinancialTransactions ────────────────────────────────
        if (!await db.FinancialTransactions.AnyAsync())
        {
            var salaryType = await db.PaymentTypes.FirstAsync(p => p.Name == "راتب");
            var bonusType = await db.PaymentTypes.FirstAsync(p => p.Name == "مكافأة");
            var deductionType = await db.PaymentTypes.FirstAsync(p => p.Name == "خصم");
            var advanceType = await db.PaymentTypes.FirstAsync(p => p.Name == "سلفة");

            var transactions = new List<FinancialTransaction>();
            var now = DateTime.UtcNow;

            // Monthly salary for each employee for last 3 months
            foreach (var emp in allUsers.Where(u => u.UserTypeId == empType.Id))
            {
                for (var m = 0; m < 3; m++)
                {
                    var payDate = new DateTime(now.Year, now.Month, 1).AddMonths(-m).AddDays(25);
                    if (payDate > now) continue;

                    transactions.Add(new FinancialTransaction
                    {
                        UserId = emp.Id,
                        Amount = emp.DailyRate * 22,
                        Type = "income",
                        Description = "راتب شهري",
                        PaymentTypeId = salaryType.Id,
                        TransactionDate = payDate,
                    });
                }
            }

            // Bonuses for some employees
            var bonusRecipients = new[] { employees[0], employees[4], employees[8] };
            foreach (var emp in bonusRecipients)
            {
                transactions.Add(new FinancialTransaction
                {
                    UserId = emp.Id,
                    Amount = 500m,
                    Type = "income",
                    Description = "مكافأة أداء متميز",
                    PaymentTypeId = bonusType.Id,
                    TransactionDate = now.AddDays(-15),
                });
            }

            // Deductions for some
            var deductionRecipients = new[] { employees[2], employees[6] };
            foreach (var emp in deductionRecipients)
            {
                transactions.Add(new FinancialTransaction
                {
                    UserId = emp.Id,
                    Amount = 200m,
                    Type = "deduction",
                    Description = "خصم تأخير",
                    PaymentTypeId = deductionType.Id,
                    DeductionBasis = "تأخير",
                    DeductionValue = "3 أيام",
                    TransactionDate = now.AddDays(-10),
                });
            }

            // Advances
            var advanceRecipients = new[] { employees[1], employees[3] };
            foreach (var emp in advanceRecipients)
            {
                transactions.Add(new FinancialTransaction
                {
                    UserId = emp.Id,
                    Amount = 1000m,
                    Type = "deduction",
                    Description = "سلفة شهرية",
                    PaymentTypeId = advanceType.Id,
                    DeductionBasis = "سلفة",
                    DeductionValue = "شهر",
                    TransactionDate = now.AddDays(-5),
                });
            }

            db.FinancialTransactions.AddRange(transactions);
            await db.SaveChangesAsync();
        }
    }
}
