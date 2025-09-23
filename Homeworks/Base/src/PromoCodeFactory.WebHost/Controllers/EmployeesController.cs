using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepository<Employee> _employeeRepository;

        public EmployeesController(IRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Создать нового сотрудника
        /// </summary>
        /// <param name="employeeRequest">Данные по сотруднику</param>
        /// <returns>Новый сотрудник</returns>
        [HttpPost]
        public async Task<ActionResult<EmployeeResponse>> CreateEmployeeAsync([FromBody] EmployeeRequest employeeRequest)
        {
            // Mapper EmployeeRequest -> Employee
            // var entity = Mapper(employeeRequest);
            // return await _employeeRepository.Add(entity);

            var newEmployee = (await _employeeRepository.GetAllAsync()).First();

            return StatusCode(201, new EmployeeResponse {
                Id = newEmployee.Id,
                Email = newEmployee.Email,
                FullName = newEmployee.FullName,
            });
        }

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            var employeesModelList = employees.Select(x =>
                new EmployeeShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                }).ToList();

            return employeesModelList;
        }

        /// <summary>
        /// Получить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            var employeeModel = new EmployeeResponse()
            {
                Id = employee.Id,
                Email = employee.Email,
                Roles = employee.Roles.Select(x => new RoleItemResponse()
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };

            return employeeModel;
        }

        /// <summary>
        /// Обновление данных сотрудника
        /// </summary>
        /// <param name="updateEmployeeRequest">Данные для обновления</param>
        [HttpPut]
        public async Task<IActionResult> UpdateEmployeeAsync(UpdateEmployeeRequest updateEmployeeRequest)
        {
            var employee = await _employeeRepository.GetByIdAsync(updateEmployeeRequest.Id);

            if (employee == null)
                return NotFound();

            // Mapper: updateEmployeeRequest -> Employee
            // await _employeeRepository.Update(Mapper(updateEmployeeRequest))

            return Ok();
        }

        /// <summary>
        /// Удалить сотрудника
        /// </summary>
        /// <param name="id">Id сотрудника</param>
        [HttpDelete]
        public async Task<IActionResult> DeleteEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            // await _employeeRepository.Delete(id))

            return Ok();
        }
    }
}