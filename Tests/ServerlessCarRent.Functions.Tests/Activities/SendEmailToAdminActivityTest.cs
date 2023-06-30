using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SendGrid.Helpers.Mail;
using ServerlessCarRent.Common.Dtos;
using ServerlessCarRent.Common.Models;
using ServerlessCarRent.Common.Models.CarRental;
using ServerlessCarRent.Functions.Activities;
using Xunit;

namespace ServerlessCarRent.Functions.Tests.Activities
{
    public class SendEmailToAdminActivityTest
    {
        private readonly Mock<ILogger<SendEmailToAdminActivity>> _mockLogger;

        public SendEmailToAdminActivityTest()
        {
            _mockLogger = new Mock<ILogger<SendEmailToAdminActivity>>();
        }

        private SendEmailToAdminActivity CreateInstance(IConfiguration configuration)
        {
            return new SendEmailToAdminActivity(configuration, _mockLogger.Object);
        }

        private RentalStatusChangeOrchestratorDto CreateRentalStatusChangeOrchestratorDto(CarRentalState carRentalState = CarRentalState.Free)
        {
            return new RentalStatusChangeOrchestratorDto()
            {
                CarPlate = Faker.Lorem.Words(1).First(),
                CarModel = Faker.Lorem.Words(1).First(),
                NewRentalStatus = carRentalState,
                CurrentRenter = new RenterData()
                {
                    LastName = Faker.Name.First(),
                    FirstName = Faker.Name.Last(),
                    Email = Faker.Internet.Email()
                }
            };
        }

        [Fact]
        public async Task TestRun_WithNullAdminEmail_ShouldLogWarning()
        {
            // Arrange
            var inMemorySettings = new Dictionary<string, string> {
                {"AdminEmail", null},
            };
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            var instance = CreateInstance(configuration);
            var context = CreateRentalStatusChangeOrchestratorDto();
            var mockMessageCollector = new Mock<IAsyncCollector<SendGridMessage>>();

            // Act
            await instance.Run(context, mockMessageCollector.Object);

            // Assert
            Assert.False(mockMessageCollector.Invocations.Any());
        }

        [Fact]
        public async Task Run_WithNotNullAdminMail_ShouldSendMessage()
        {
            // Arrange
            var inMemorySettings = new Dictionary<string, string> {
                {"AdminEmail", "admin@mail.something"},
            };
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            var instance = CreateInstance(configuration);
            var context = CreateRentalStatusChangeOrchestratorDto();
            var mockMessageCollector = new Mock<IAsyncCollector<SendGridMessage>>();

            // Act
            await instance.Run(context, mockMessageCollector.Object);

            // Assert
           mockMessageCollector.Verify( x=> x.AddAsync(It.IsNotNull<SendGridMessage>(),default));
        }
    }
}

