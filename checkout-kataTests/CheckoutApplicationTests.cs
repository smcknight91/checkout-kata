﻿using checkout_kata;
using checkout_kata.Service;
using NUnit.Framework;
using System;
using checkout_kata.Models;

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
    }
}