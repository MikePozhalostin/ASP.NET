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
    /// Клиенты
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CustomersController
        : ControllerBase
    {
        private readonly IRepository<Customer> _repository;
        public CustomersController(IRepository<Customer> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerShortResponse>>> GetCustomersAsync()
        {
            var customerEntities = await _repository.GetAllAsync();
            return Ok(customerEntities.Select(c => new CustomerShortResponse
            {
                Email = c.Email,
                FirstName = c.FirstName,
                Id = c.Id,
                LastName = c.LastName
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            var customer = await _repository.GetByIdAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(new CustomerResponse
            {
                LastName = customer.LastName,
                Email = customer.Email,
                FirstName = customer.FirstName,
                Id = customer.Id,
                PromoCodes = customer.PromoCodes.Select(p => new PromoCodeShortResponse
                {
                    Id = p.Id,
                    BeginDate = p.BeginDate.ToShortDateString(),
                    Code = p.Code,
                    EndDate = p.EndDate.ToShortDateString(),
                    PartnerName = p.PartnerName,
                    ServiceInfo = p.ServiceInfo
                }).ToList(),
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        {
            await _repository.CreateAsync(new Customer
            {
                Email = request.Email,
                FirstName= request.FirstName,
                LastName = request.LastName,
                CustomerPreferences = request.PreferenceIds.Select(p => new CustomerPreference
                {
                    PreferenceId = p
                }).ToList()
            });
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request)
        {
            try
            {
                await _repository.UpdateAsync(new Customer
                {
                    Id = id,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName= request.LastName,
                    CustomerPreferences = request.PreferenceIds.Select(p => new CustomerPreference
                    {
                        PreferenceId = p
                    }).ToList()
                });
                return Ok();
            }
            catch (KeyNotFoundException) { return NotFound(); }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            try
            {
                await _repository.DeleteByIdAsync(id);
                return Ok();
            }
            catch (KeyNotFoundException) { return NotFound(); }
        }
    }
}