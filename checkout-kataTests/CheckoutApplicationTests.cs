using checkout_kata;
using checkout_kata.Service;
using NUnit.Framework;
using System;

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
        [TestCase("")]
        public void GivenNoSkus_WhenProcessRan_ThenThrowNullArgumentException(string skus)
        {
            //Arrange
            //Act
            //Assert
            Assert.That(() =>
                {
                    CheckoutApplication.Process(skus);
                },
                Throws.TypeOf<ArgumentNullException>()
                    .With.Message.EqualTo("Skus can't be null or empty."));
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
        }

        [Test]
        [TestCase("AB")]
        [TestCase("ABC")]
        [TestCase("AAAAAA")]
        public void GivenValidSku_WhenScan_ThenAddToScannedItems(string skus)
        {
            //Arrange
            HelperExtensions.ScanSkus(_checkout, skus);

            //Act
            //Assert
            Assert.That(_checkout.ScannedItemDetails, Is.Not.Null);
        }

        [Test]
        [TestCase("ABC")]
        public void GivenValidSku_WhenGetTotalPrice_ThenReturnCorrectTotalPrice(string skus)
        {
            //Arrange
            HelperExtensions.ScanSkus(_checkout, skus);

            //Act
            var totalPrice = _checkout.GetTotalPrice();

            //Assert
            Assert.That(totalPrice, Is.Not.Null);
            Assert.That(totalPrice, Is.EqualTo(100));
        }

        [Test]
        [TestCase("AAA")]
        public void GivenValidSkuOffer_WhenGetTotalPrice_ThenReturnCorrectTotalPrice(string skus)
        {
            //Arrange
            HelperExtensions.ScanSkus(_checkout, skus);

            //Act
            var totalPrice = _checkout.GetTotalPrice();

            //Assert
            Assert.That(totalPrice, Is.Not.Null);
            Assert.That(totalPrice, Is.EqualTo(130));
        }

        [Test]
        [TestCase("AAAA")]
        public void GivenValidSkuOfferWthAdditionalSku_WhenGetTotalPrice_ThenReturnCorrectTotalPrice(string skus)
        {
            //Arrange
            HelperExtensions.ScanSkus(_checkout, skus);

            //Act
            var totalPrice = _checkout.GetTotalPrice();

            //Assert
            Assert.That(totalPrice, Is.Not.Null);
            Assert.That(totalPrice, Is.EqualTo(180));
        }

        [Test]
        [TestCase("AAAAAA")]
        public void GivenValidDoubleSkuOffer_WhenGetTotalPrice_ThenReturnCorrectTotalPrice(string skus)
        {
            //Arrange
            HelperExtensions.ScanSkus(_checkout, skus);

            //Act
            var totalPrice = _checkout.GetTotalPrice();

            //Assert
            Assert.That(totalPrice, Is.Not.Null);
            Assert.That(totalPrice, Is.EqualTo(260));
        }

        [Test]
        [TestCase("AAAB")]
        public void GivenValidSkuOfferWithNonOfferSku_WhenGetTotalPrice_ThenReturnCorrectTotalPrice(string skus)
        {
            //Arrange
            HelperExtensions.ScanSkus(_checkout, skus);

            //Act
            var totalPrice = _checkout.GetTotalPrice();

            //Assert
            Assert.That(totalPrice, Is.Not.Null);
            Assert.That(totalPrice, Is.EqualTo(160));
        }

        [Test]
        [TestCase("DDDDD")]
        public void GivenSkuWithNullOffer_WhenGetTotalPrice_ThenReturnCorrectTotalPrice(string skus)
        {
            //Arrange
            HelperExtensions.ScanSkus(_checkout, skus);

            //Act
            var totalPrice = _checkout.GetTotalPrice();

            //Assert
            Assert.That(totalPrice, Is.Not.Null);
            Assert.That(totalPrice, Is.EqualTo(75));
        }
    }
}