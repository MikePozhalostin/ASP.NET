using MassTransit;
using Microsoft.Extensions.Logging;
using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Domain.Administration;
using Pcf.Common;
using System.Threading.Tasks;

namespace Pcf.Administration.WebHost.Consumers
{
    public class PromoConsumer : IConsumer<GivedPromoForPartnerMessage>
    {
        private readonly IRepository<Employee> _employeeRepository;
        private readonly ILogger<PromoConsumer> _logger;

        public PromoConsumer(IRepository<Employee> employeeRepository, ILogger<PromoConsumer> logger)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GivedPromoForPartnerMessage> context)
        {
            _logger.LogInformation($"Receive parnter id: {context.Message.ParnterId}");

            var employee = await _employeeRepository.GetByIdAsync(context.Message.ParnterId);

            if (employee == null)
                return;

            employee.AppliedPromocodesCount++;

            await _employeeRepository.UpdateAsync(employee);
        }
    }
}
