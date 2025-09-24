using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class Preference
        : BaseEntity
    {
        [MaxLength(100)]
        public string Name { get; set; }

        public ICollection<CustomerPreference> CustomerPreferences { get; set; }
    }
}