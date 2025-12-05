using MassTransit;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.Mappers;
using Pcf.GivingToCustomer.WebHost.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.WebHost.Consumers
{
    public class PromoConsumer : IConsumer<GivePromoCodeRequest>
    {
        private readonly IRepository<PromoCode> _promoCodesRepository;
        private readonly IRepository<Preference> _preferencesRepository;
        private readonly IRepository<Customer> _customersRepository;

        public PromoConsumer(IRepository<PromoCode> promoCodesRepository,
            IRepository<Preference> preferencesRepository, IRepository<Customer> customersRepository)
        {
            _promoCodesRepository = promoCodesRepository;
            _preferencesRepository = preferencesRepository;
            _customersRepository = customersRepository;
        }

        public async Task Consume(ConsumeContext<GivePromoCodeRequest> context)
        {
            Console.WriteLine($"Receive message for preference: {context.Message.PreferenceId}");
            //Получаем предпочтение по имени
            var preference = await _preferencesRepository.GetByIdAsync(context.Message.PreferenceId);

            if (preference == null)
            {
                return;
            }

            //  Получаем клиентов с этим предпочтением:
            var customers = await _customersRepository
                .GetWhere(d => d.Preferences.Any(x =>
                    x.Preference.Id == preference.Id));

            PromoCode promoCode = PromoCodeMapper.MapFromModel(context.Message, preference, customers);

            await _promoCodesRepository.AddAsync(promoCode);
        }
    }
}
