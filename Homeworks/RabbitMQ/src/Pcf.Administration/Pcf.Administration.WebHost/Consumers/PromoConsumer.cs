using MassTransit;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Domain.Administration;
using Pcf.Administration.WebHost.Models;
using System;
using System.Threading.Tasks;

namespace Pcf.Administration.WebHost.Consumers
{
    public class PromoConsumer : IConsumer<AdminPartnerMessage>
    {
        private readonly IRepository<Employee> _employeeRepository;

        public PromoConsumer(IRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task Consume(ConsumeContext<AdminPartnerMessage> context)
        {
            Console.WriteLine($"Receive parnter id: {context.Message.ParnterId}");

            var employee = await _employeeRepository.GetByIdAsync(context.Message.ParnterId);

            if (employee == null)
                return;

            employee.AppliedPromocodesCount++;

            await _employeeRepository.UpdateAsync(employee);
        }
    }
}
