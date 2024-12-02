using checkout_kata;
using NUnit.Framework;
using System;
using checkout_kata.DataModels;

namespace checkout_kataTests
{
    [TestFixture]
    public class CheckoutApplicationTests
    {
        private Checkout _checkout;

        [SetUp]
        public void Init()
        {
            _checkout = new Checkout();
        }

        [Test]
        [TestCase("ABCM")]
        [TestCase("ABCZ")]
        [TestCase("TCA")]
        public void GivenInvalidSku_WhenProcessRan_ThenThrowKeyNotFoundException(string skus)
        {
            //Arrange
            //Act
            //Assert
            Assert.That(() =>
            {
                foreach (var sku in skus)
                {
                    _checkout.Scan(sku.ToString());
                }
            },
                Throws.TypeOf<KeyNotFoundException>()
                    .With.Message.EqualTo($"Scanned SKU does not exist."));
        }

        [Test]
        [TestCase("AB")]
        [TestCase("ABC")]
        [TestCase("AAAAAA")]
        public void GivenValidSku_WhenScan_ThenAddToScannedItems(string skus)
        {
            //Arrange
            //Act
            foreach (var sku in skus)
            {
                _checkout.Scan(sku.ToString());
            }

            //Assert
            Assert.That(_checkout.scannedItemCount, Is.Not.Null);
        }

        [Test]
        [TestCase("ABC")]
        public void GivenValidSku_WhenGetTotalCount_ThenReturnCorrectTotalPrice(string skus)
        {
            //Arrange
            //Act
            foreach (var sku in skus)
            {
                _checkout.Scan(sku.ToString());
            }

            var totalPrice = _checkout.GetTotalPrice();

            //Assert
            Assert.That(totalPrice, Is.Not.Null);
            Assert.That(totalPrice, Is.EqualTo(100));
        }
    }
}