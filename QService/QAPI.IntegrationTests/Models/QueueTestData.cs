// using Microsoft.EntityFrameworkCore;
// using QDomain.Enums;
// using QDomain.Models;
// using QInfrastructure.Persistence.DataBase;
//
// namespace QAPI.IntegrationTests.Models;
//
// public static class QueueTestData
// {
//     public class SeedData
//     {
//         public int CompanyId { get; set; } = 1;
//         public int BranchId { get; set; } = 1;
//         public int ServiceId { get; set; } = 1;
//         public int EmployeeId { get; set; }
//         public int CustomerId { get; set; }
//         public string Token { get; set; } = string.Empty;
//     }
//
//     public static async Task<SeedData> SeedDatabaseAsync(QueueDbContext db, string token, int? companyId = null)
//     {
//         var user = await db.Users
//             .Include(u => u.Customer)
//             .FirstOrDefaultAsync(u => u.EmailAddress.Contains("test"));
//         
//         if (user == null)
//             throw new Exception("User not found. Make sure GetJwtToken creates a user first.");
//
//         if (user.Customer != null)
//         {
//             var existingEmployee = await CreateOrGetEmployee(db, companyId ?? 1, user);
//             
//             return new SeedData
//             {
//                 CompanyId = companyId ?? 1,
//                 BranchId = 1,
//                 ServiceId = 1,
//                 EmployeeId = existingEmployee.Id,
//                 CustomerId = user.Customer.Id,
//                 Token = token
//             };
//         }
//
//         var customer = new CustomerEntity
//         {
//             FirstName = "Test",
//             LastName = "Customer",
//             PhoneNumber = "+992987654321",
//             CreatedAt = DateTime.UtcNow
//         };
//         
//         db.Customers.Add(customer);
//         await db.SaveChangesAsync();
//
//         user.CustomerId = customer.Id;
//         user.Customer = customer;
//         await db.SaveChangesAsync();
//
//         var employee = await CreateOrGetEmployee(db, companyId ?? 1, user);
//
//         return new SeedData
//         {
//             CompanyId = companyId ?? 1,
//             BranchId = 1,
//             ServiceId = 1,
//             EmployeeId = employee.Id,
//             CustomerId = customer.Id,
//             Token = token
//         };
//     }
//
//     private static async Task<EmployeeEntity> CreateOrGetEmployee(QueueDbContext db, int companyId, UserEntity user)
//     {
//         if (user.EmployeeId.HasValue)
//         {
//             var existingEmployee = await db.Employees
//                 .Include(e => e.AvailabilitySchedules)
//                 .FirstOrDefaultAsync(e => e.Id == user.EmployeeId);
//             
//             if (existingEmployee != null)
//                 return existingEmployee;
//         }
//
//         var employee = new EmployeeEntity
//         {
//             CompanyId = companyId,
//             BranchId = 1,
//             FirstName = "Test",
//             LastName = "Employee",
//             Position = "Receptionist",
//             PhoneNumber = "+992987654322",
//             CreatedAt = DateTime.UtcNow
//         };
//         
//         db.Employees.Add(employee);
//         await db.SaveChangesAsync();
//
//         user.EmployeeId = employee.Id;
//         user.Employee = employee;
//         await db.SaveChangesAsync();
//
//         var schedule = new AvailabilityScheduleEntity
//         {
//             EmployeeId = employee.Id,
//             Description = "Regular working hours",
//             RepeatSlot = RepeatSlot.Weekly,
//             CreatedAt = DateTime.UtcNow,
//             AvailableSlots = new List<Interval<DateTimeOffset>>
//             {
//                 new Interval<DateTimeOffset>(
//                     DateTimeOffset.UtcNow.Date.AddHours(9),
//                     DateTimeOffset.UtcNow.Date.AddHours(12)
//                 ),
//                 new Interval<DateTimeOffset>(
//                     DateTimeOffset.UtcNow.Date.AddHours(13),
//                     DateTimeOffset.UtcNow.Date.AddHours(17)
//                 )
//             }
//         };
//         
//         db.AvailabilitySchedules.Add(schedule);
//         await db.SaveChangesAsync();
//
//         return employee;
//     }
//     
// }