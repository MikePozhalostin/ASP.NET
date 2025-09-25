using PromoCodeFactory.DataAccess.Data;
using System;
using System.Linq;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public static class DataContextInitializer
    {
        public static void Seed()
        {
            using var context = new DataContext();

            var transaction = context.Database.BeginTransaction();

            try
            {
                context.Preferences.AddRange(FakeDataFactory.Preferences);
                context.SaveChanges();

                context.Roles.AddRange(FakeDataFactory.Roles);
                context.SaveChanges();

                var employees = FakeDataFactory.Employees.ToList();
                foreach (var employee in employees)
                {
                    employee.Role = context.Roles.First(r => r.Name == employee.Role.Name);
                }
                context.Employees.AddRange(employees);
                context.SaveChanges();

                var customers = FakeDataFactory.Customers.ToList();
                foreach (var customer in customers)
                {
                    foreach (var pref in customer.CustomerPreferences)
                    {
                        pref.Preference = context.Preferences.First(p => p.Name == pref.Preference.Name);
                    }
                }
                context.Customers.AddRange(customers);
                context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
