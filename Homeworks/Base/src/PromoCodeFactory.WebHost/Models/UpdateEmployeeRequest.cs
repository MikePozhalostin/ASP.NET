using System;

namespace PromoCodeFactory.WebHost.Models
{
    public class UpdateEmployeeRequest : EmployeeRequest
    {
        public Guid Id { get; set; }
    }
}
