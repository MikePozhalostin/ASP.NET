using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Промокоды
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PromocodesController
        : ControllerBase
    {
        private readonly IRepository<Customer> _customersRepository;
        private readonly IRepository<PromoCode> _promoRepository;
        private readonly IRepository<Preference> _prefRepository;

        public PromocodesController(IRepository<Customer> customersRepository, IRepository<PromoCode> promoRepository, IRepository<Preference> prefRepository)
        {
            _customersRepository = customersRepository ?? throw new ArgumentNullException(nameof(customersRepository));
            _promoRepository = promoRepository ?? throw new ArgumentNullException(nameof(promoRepository));
            _prefRepository = prefRepository ?? throw new ArgumentNullException(nameof(prefRepository));
        }

        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync()
        {
            var promoCodes = await _promoRepository.GetAllAsync();

            return Ok(promoCodes.Select(p => new PromoCodeShortResponse
            {
                BeginDate = p.BeginDate.ToString(),
                Code = p.Code,
                EndDate = p.EndDate.ToString(),
                Id = p.Id,
                PartnerName = p.PartnerName,
                ServiceInfo = p.ServiceInfo
            }));
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
        {
            var preferences = await _prefRepository.GetAllAsync();
            var currentPref = preferences.FirstOrDefault(p => request.Preference == p.Name);

            var newPromoCode = await _promoRepository.CreateAsync(new PromoCode
            {
                ServiceInfo = request.ServiceInfo,
                PartnerName = request.PartnerName,
                Code = request.PromoCode,
                BeginDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                Preference = currentPref
            });

            var customers = await _customersRepository.GetAllAsync();

            var customersForUpdate = customers.Where(c => c.CustomerPreferences.FirstOrDefault(cp => cp.PreferenceId == currentPref.Id) != null);

            if (customersForUpdate.Any())
            {
                foreach (var customer in customersForUpdate)
                {
                    customer.PromoCodes.Add(newPromoCode);
                    await _customersRepository.UpdateAsync(customer);
                }
            }

            return Ok();
        }
    }
}