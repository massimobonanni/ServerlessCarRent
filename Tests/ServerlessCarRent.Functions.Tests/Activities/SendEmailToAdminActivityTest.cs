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
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILogger<SendEmailToAdminActivity>> _mockLogger;

        public SendEmailToAdminActivityTest()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<SendEmailToAdminActivity>>();
        }

        private SendEmailToAdminActivity CreateInstance()
        {
            return new SendEmailToAdminActivity(_mockConfiguration.Object, _mockLogger.Object);
        }

        //[Fact]
        //public async Task TestRun()
        //{
        //    // Arrange
        //    var context = new RentalStatusChangeOrchestratorDto()
        //    {
        //        CarPlate = "testCarPlate",
        //        CarModel = "testCarModel",
        //        NewRentalStatus = CarRentalState.Free,
        //        CurrentRenter = new RenterData()
        //        {
        //            LastName = "testLastName",
        //            FirstName = "testFirstName",
        //            Email = "testEmail"
        //        }
        //    };

        //    var mockMessageCollector = new Mock<IAsyncCollector<SendGridMessage>>();
        //    var instance = CreateInstance();

        //    _mockConfiguration.Setup(x => x.GetValue<string>("AdminEmail")).Returns("test_admin_email");

        //    // Act
        //    await instance.Run(context, mockMessageCollector.Object);

        //    // Assert
        //    mockMessageCollector.Verify(x => x.AddAsync(It.IsAny<SendGridMessage>()), Times.Once());
        //}

        [Fact]
        public async Task TestRun_WithNullAdminEmail_ShouldLogWarning()
        {
            // Arrange
            var context = new RentalStatusChangeOrchestratorDto()
            {
                CarPlate = "testCarPlate",
                CarModel = "testCarModel",
                NewRentalStatus = CarRentalState.Free,
                CurrentRenter = new RenterData()
                {
                    LastName = "testLastName",
                    FirstName = "testFirstName",
                    Email = "testEmail"
                }
            };

            var mockMessageCollector = new Mock<IAsyncCollector<SendGridMessage>>();
            var instance = CreateInstance();

            _mockConfiguration.Setup(x => x.GetValue<string>("AdminEmail")).Returns((string)null);

            // Act
            await instance.Run(context, mockMessageCollector.Object);

            // Assert
            _mockLogger.Verify(x => x.LogWarning(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public async Task TestCreateSendGridMessageAsync()
        {
            // Arrange
            var instance = CreateInstance();
            var context = new RentalStatusChangeOrchestratorDto()
            {
                CarPlate = "testCarPlate",
                CarModel = "testCarModel",
                NewRentalStatus = CarRentalState.Free,
                CurrentRenter = new RenterData()
                {
                    LastName = "testLastName",
                    FirstName = "testFirstName",
                    Email = "testmail@mail.something"
                }
            };
            var mockMessageCollector = new Mock<IAsyncCollector<SendGridMessage>>();
            _mockConfiguration.Setup(x => x.GetValue<string>("AdminEmail")).Returns("admin@mail.something");

            // Act
            await instance.Run(context, mockMessageCollector.Object);

            // Assert
           mockMessageCollector.Verify( x=> x.AddAsync(It.IsNotNull<SendGridMessage>(),default));
            
        }
    }
}

