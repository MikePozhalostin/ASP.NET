using HotChocolate;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using System;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.WebHost.GraphQL
{
    public class Query
    {
        public Task<Customer> GetCustomer(Guid id, [Service] IRepository<Customer> repository)
        {
            return repository.GetByIdAsync(id);
        }
    }
}