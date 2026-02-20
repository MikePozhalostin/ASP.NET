using Grpc.Core;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.Grpc;
using System;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.WebHost.Grpc
{
    public class CustomerGrpcService : CustomerService.CustomerServiceBase
    {
        private readonly IRepository<Customer> _repository;

        public CustomerGrpcService(IRepository<Customer> repository)
        {
            _repository = repository;
        }

        public override async Task<CustomerReply> GetCustomer(
            GetCustomerRequest request,
            ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var customerId))
            {
                throw new RpcException(new Status(
                    StatusCode.InvalidArgument,
                    "Invalid GUID format"));
            }

            var customer = await _repository.GetByIdAsync(customerId);

            if (customer == null)
            {
                throw new RpcException(new Status(
                    StatusCode.NotFound,
                    "Customer not found"));
            }

            return new CustomerReply
            {
                Id = customer.Id.ToString(),
                Name = customer.FullName,
                Email = customer.Email
            };
        }
    }
}