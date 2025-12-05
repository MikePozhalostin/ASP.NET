using MassTransit;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Domain.Administration;
using System;
using System.Threading.Tasks;

namespace Pcf.Administration.WebHost.Consumers
{
    public class PromoConsumer : IConsumer<string>
    {
        private readonly IRepository<Employee> _employeeRepository;

        public PromoConsumer(IRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task Consume(ConsumeContext<string> context)
        {
            if (Guid.TryParse(context.Message, out var id))
            {
                Console.WriteLine(context.Message);

                var employee = await _employeeRepository.GetByIdAsync(id);

                if (employee == null)
                    return;

                employee.AppliedPromocodesCount++;

                await _employeeRepository.UpdateAsync(employee);
            }
            else
            {
                Console.WriteLine($"Message is not valid: {context.Message}");
            }
        }
    }
}
