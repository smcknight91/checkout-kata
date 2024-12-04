using checkout_kata;
using checkout_kata.Service;
using NUnit.Framework;
using System;
using checkout_kata.Models;
using checkout_kata.Interface;
using Microsoft.Extensions.Logging;
using Moq;

namespace checkout_kataTests
{
    [TestFixture]
    public class CheckoutApplicationTests
    {
        private Checkout _checkout;
        private Mock<ILogger<Checkout>> _mockLogger;

        [SetUp]
        public void Init()
        {
            _mockLogger = new Mock<ILogger<Checkout>>();
            _checkout = new Checkout(_mockLogger.Object);
            _checkout.AddConfigurations([
                new PriceConfigurationModel
                {
                    SKU = "A", Price = 50,
                    Multibuy =
                    [
                        new MultibuyModel { ItemCount = 3, TotalPrice = 130 },
                        new MultibuyModel { ItemCount = 6, TotalPrice = 200 }
                    ]
                },
                new PriceConfigurationModel
                {
                    SKU = "B", Price = 30,
                    Multibuy =
                    [
                        new MultibuyModel { ItemCount = 4, TotalPrice = 30 }
                    ]
                },
                new PriceConfigurationModel
                    { SKU = "C", Price = 20, Multibuy = 
                        [
                            new MultibuyModel { ItemCount = 6, TotalPrice = 100 }
                        ]
                    },
                new PriceConfigurationModel { SKU = "D", Price = 15, Multibuy = null }
            ]);
            _checkout.AddMultibuyConfigurations("B", new MultibuyModel { ItemCount = 2, TotalPrice = 45 });
        }

        [Test]
        [TestCase("ABCM")]
        [TestCase("ABCZ")]
        [TestCase("TCA")]
        public void GivenInvalidSku_WhenScan_ThenThrowKeyNotFoundException(string skus)
        {
            //Arrange
            //Act
            //Assert
            Assert.That(() =>
            {
                HelperExtensions.ScanSkus(_checkout, skus);
            },
                Throws.TypeOf<KeyNotFoundException>()
                    .With.Message.EqualTo("Scanned SKU does not exist."));

            _mockLogger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Test]
        [TestCase("AB", ExpectedResult = 2)]
        [TestCase("ABC", ExpectedResult = 3)]
        [TestCase("AAAAAA", ExpectedResult = 1)]
        public int GivenValidSku_WhenScan_ThenAddToScannedItems(string skus)
        {
            //Arrange
            HelperExtensions.ScanSkus(_checkout, skus);

            //Act
            //Assert
            Assert.That(_checkout.ScannedItemDetails, Is.Not.Null);
            return _checkout.ScannedItemDetails.Count;
        }

        [TestCase("AAA", ExpectedResult = 130, TestName = "GivenValidSkuOffer_WhenGetTotalPrice_ThenReturnCorrectTotalPrice")]
        [TestCase("AAAA", ExpectedResult = 180, TestName = "GivenValidSkuOfferWthAdditionalSku_WhenGetTotalPrice_ThenReturnCorrectTotalPrice")]
        [TestCase("AAAAAA", ExpectedResult = 200, TestName = "GivenValidDoubleSkuOffer_WhenGetTotalPrice_ThenReturnCorrectTotalPrice")]
        [TestCase("AAAB", ExpectedResult = 160, TestName = "GivenValidSkuOfferWithNonOfferSku_WhenGetTotalPrice_ThenReturnCorrectTotalPrice")]
        [TestCase("DDDDD", ExpectedResult = 75, TestName = "GivenSkuWithNullOffer_WhenGetTotalPrice_ThenReturnCorrectTotalPrice")]
        [TestCase("ABABAABBAAA", ExpectedResult = 280, TestName = "GivenSkuWithMultipleOffersAndOneRemaining_WhenGetTotalPrice_ThenReturnCorrectTotalPrice")]
        public int GivenValidSkuAndOffers_WhenGetTotalPrice_ThenReturnCorrectTotalPrice(string skus)
        {
            //Arrange
            HelperExtensions.ScanSkus(_checkout, skus);

            //Act
            var totalPrice = _checkout.GetTotalPrice();

            //Assert
            Assert.That(totalPrice, Is.Not.Null);
            return totalPrice;
        }

        [Test]
        public void GivenDuplicateSkuAndOffer_WhenGetTotalPrice_ThenThrowException()
        {
            //Arrange
            //Act
            //Assert
            Assert.That(() =>
                {
                    _checkout.AddMultibuyConfigurations("A", new MultibuyModel { ItemCount = 3, TotalPrice = 30 });
                },
                Throws.TypeOf<Exception>()
                    .With.Message.EqualTo("Multibuy model exists already for item count for sku: A"));

            _mockLogger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Test]
        public void GivenDuplicatePriceConfiguration_WhenGetTotalPrice_ThenThrowException()
        {
            //Arrange
            //Act
            //Assert
            Assert.That(() =>
                {
                    _checkout.AddConfigurations(new PriceConfigurationModel
                    {
                        SKU = "A",
                        Price = 10,
                        Multibuy =
                        [
                            new MultibuyModel { ItemCount = 3, TotalPrice = 130 },
                            new MultibuyModel { ItemCount = 6, TotalPrice = 200 }
                        ]
                    });
                },
                Throws.TypeOf<Exception>()
                    .With.Message.EqualTo("Error - SKU already exists within configuration."));

            _mockLogger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }
    }
}