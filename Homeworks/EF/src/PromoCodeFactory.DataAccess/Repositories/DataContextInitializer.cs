using PromoCodeFactory.DataAccess.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public static class DataContextInitializer
    {
        public async static Task SeedAsync(DataContext context, CancellationToken ct)
        {
            try
            {
                await context.Preferences.AddRangeAsync(FakeDataFactory.Preferences, ct);
                await context.SaveChangesAsync(ct);

                await context.Roles.AddRangeAsync(FakeDataFactory.Roles, ct);
                await context.SaveChangesAsync(ct);

                var employees = FakeDataFactory.Employees.ToList();
                foreach (var employee in employees)
                {
                    employee.Role = context.Roles.First(r => r.Name == employee.Role.Name);
                }
                await context.Employees.AddRangeAsync(employees, ct);
                await context.SaveChangesAsync(ct);

                var customers = FakeDataFactory.Customers.ToList();
                foreach (var customer in customers)
                {
                    foreach (var pref in customer.CustomerPreferences)
                    {
                        pref.Preference = context.Preferences.First(p => p.Name == pref.Preference.Name);
                    }
                }
                await context.Customers.AddRangeAsync(customers, ct);
                await context.SaveChangesAsync(ct);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
