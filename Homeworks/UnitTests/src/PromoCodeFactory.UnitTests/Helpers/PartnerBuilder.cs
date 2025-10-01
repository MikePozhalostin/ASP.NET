using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;
using System.Collections.Generic;

namespace PromoCodeFactory.UnitTests.Helpers
{
    public class PartnerBuilder
    {
        private readonly Partner _partner = new();

        public PartnerBuilder FillName(string name)
        {
            _partner.Name = name;
            return this;
        }

        public PartnerBuilder FillNumberIssuedPromoCodes(int numberIssuedPromoCodes)
        {
            _partner.NumberIssuedPromoCodes = numberIssuedPromoCodes;
            return this;
        }

        public PartnerBuilder FillIsActive(bool isActive)
        {
            _partner.IsActive = isActive;
            return this;
        }

        public PartnerBuilder FillPartnerLimits(ICollection<PartnerPromoCodeLimit> partnerLimits)
        {
            _partner.PartnerLimits = partnerLimits;
            return this;
        }

        public PartnerBuilder FillId(Guid id)
        {
            _partner.Id = id;
            return this;
        }

        public Partner Build()
        {
            return _partner;
        }
    }
}
