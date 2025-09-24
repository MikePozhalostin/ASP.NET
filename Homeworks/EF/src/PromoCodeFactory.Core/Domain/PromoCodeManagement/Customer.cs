using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class Customer
        : BaseEntity
    {
        [MaxLength(50)]
        public string FirstName { get; set; }
        [MaxLength(100)]
        public string LastName { get; set; }
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
        [MaxLength(50)]
        public string Email { get; set; }

        public ICollection<PromoCode> PromoCodes { get; set; }

        public ICollection<CustomerPreference> CustomerPreferences { get; set; }
    }
}