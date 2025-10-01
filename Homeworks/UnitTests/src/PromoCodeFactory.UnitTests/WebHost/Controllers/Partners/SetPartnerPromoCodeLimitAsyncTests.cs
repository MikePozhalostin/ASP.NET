using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.UnitTests.Helpers;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitAsyncTests
    {
        private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
        private readonly PartnersController _partnersController;

        public SetPartnerPromoCodeLimitAsyncTests()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            _partnersRepositoryMock = fixture.Freeze<Mock<IRepository<Partner>>>();
            _partnersController = fixture.Build<PartnersController>().OmitAutoProperties().Create();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerIsNotFound_ReturnsNotFound()
        {
            // Arrange
            var partnerId = Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");
            Partner partner = null;
            var request = new SetPartnerPromoCodeLimitRequest();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

            // Assert
            result.Result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerIsNotActive_ReturnsBadRequest()
        {
            // Arrange
            var partner = new PartnerBuilder().FillIsActive(false).FillId(Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165")).Build();
            var request = new SetPartnerPromoCodeLimitRequest();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();

            result.Result
                .As<BadRequestObjectResult>()
                .Value.Should().Be("Данный партнер не активен");
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_SetNotValidLimit_ReturnsBadRequest()
        {
            // Arrange
            var partner = new PartnerBuilder().FillIsActive(true).FillId(Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165")).Build();
            var request = new SetPartnerPromoCodeLimitRequest { EndDate = DateTime.Now.AddDays(1), Limit = 0 };

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();

            result.Result
                .As<BadRequestObjectResult>()
                .Value.Should().Be("Лимит должен быть больше 0");
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_SetValidLimit_ReturnsPartnerWithZeroNumberPromoCodes()
        {
            // Arrange
            var partner = new PartnerBuilder()
                .FillIsActive(true)
                .FillId(Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165"))
                .FillNumberIssuedPromoCodes(5)
                .FillPartnerLimits(new List<PartnerPromoCodeLimit>
                {
                    new() { Limit = 2, CreateDate = DateTime.Now.AddDays(-1), EndDate = DateTime.Now.AddDays(1) },
                })
                .Build();

            var request = new SetPartnerPromoCodeLimitRequest { EndDate = DateTime.Now.AddDays(1), Limit = 1 };

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(partner);

            _partnersRepositoryMock.Setup(repo => repo.UpdateAsync(partner));

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            // Assert
            result.Value.NumberIssuedPromoCodes.Should().Be(0);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_SetValidLimit_ReturnsPartnerWithOneActiveLimit()
        {
            // Arrange
            var partner = new PartnerBuilder()
                .FillIsActive(true)
                .FillId(Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165"))
                .FillNumberIssuedPromoCodes(5)
                .FillPartnerLimits(new List<PartnerPromoCodeLimit>
                {
                    new() { Limit = 2, CreateDate = DateTime.Now.AddDays(-1), EndDate = DateTime.Now.AddDays(1) },
                })
                .Build();

            var request = new SetPartnerPromoCodeLimitRequest { EndDate = DateTime.Now.AddDays(1), Limit = 1 };

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(partner);

            _partnersRepositoryMock.Setup(repo => repo.UpdateAsync(partner));

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            // Assert
            result.Value.PartnerLimits.Where(p => string.IsNullOrEmpty(p.CancelDate)).Count().Should().Be(1);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_SetValidLimit_ReturnsUpdateEntityException()
        {
            // Arrange
            var partner = new PartnerBuilder()
                .FillIsActive(true)
                .FillId(Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165"))
                .FillNumberIssuedPromoCodes(5)
                .FillPartnerLimits(new List<PartnerPromoCodeLimit>
                {
                    new() { Limit = 2, CreateDate = DateTime.Now.AddDays(-1), EndDate = DateTime.Now.AddDays(1) },
                })
                .Build();
            var request = new SetPartnerPromoCodeLimitRequest { EndDate = DateTime.Now.AddDays(1), Limit = 1 };

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(partner);

            _partnersRepositoryMock.Setup(repo => repo.UpdateAsync(partner)).Throws(new InvalidOperationException());

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();

            result.Result
                .As<BadRequestObjectResult>()
                .Value.Should().Be("Ошибка обновления данных");
        }
    }
}