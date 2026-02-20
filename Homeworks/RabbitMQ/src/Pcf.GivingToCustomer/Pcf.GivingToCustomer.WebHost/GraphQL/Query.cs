using HotChocolate;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using System;
using System.Threading.Tasks;

public class Query
{
    public Task<Customer> GetCustomer(Guid id, [Service] IRepository<Customer> repository)
    {
        return repository.GetByIdAsync(id);
    }
}
