using checkout_kata;

namespace checkout_kataTests
{
    [TestClass()]
    public class CheckoutApplicationTests
    {
        [TestMethod()]
        public void GivenInitialSetup_WhenProcessRan_ThenThrowNotImplementedException()
        {
            Assert.ThrowsException<NotImplementedException>(() => CheckoutApplication.Process("ABC"));
        }
    }
}