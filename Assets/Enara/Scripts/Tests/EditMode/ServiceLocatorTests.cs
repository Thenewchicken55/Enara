using NUnit.Framework;
using Enara.Core;

namespace Enara.Tests
{
    /// <summary>
    /// EditMode tests for <see cref="ServiceLocator"/>. Static state - each test clears
    /// in TearDown.
    /// </summary>
    [TestFixture]
    public class ServiceLocatorTests
    {
        // A concrete service type for testing.
        private sealed class FakeService { public string Value { get; set; } }
        private sealed class OtherService { }

        [TearDown]
        public void TearDown() => ServiceLocator.Clear();

        [Test]
        public void Get_WhenNotRegistered_ReturnsNull()
        {
            Assert.IsNull(ServiceLocator.Get<FakeService>());
        }

        [Test]
        public void Has_WhenNotRegistered_ReturnsFalse()
        {
            Assert.IsFalse(ServiceLocator.Has<FakeService>());
        }

        [Test]
        public void Register_ThenGet_ReturnsSameInstance()
        {
            var service = new FakeService { Value = "hello" };

            ServiceLocator.Register(service);
            var fetched = ServiceLocator.Get<FakeService>();

            Assert.AreSame(service, fetched);
            Assert.AreEqual("hello", fetched.Value);
        }

        [Test]
        public void Register_ThenHas_ReturnsTrue()
        {
            ServiceLocator.Register(new FakeService());

            Assert.IsTrue(ServiceLocator.Has<FakeService>());
        }

        [Test]
        public void Register_OverwritesPreviousRegistration()
        {
            var first = new FakeService { Value = "first" };
            var second = new FakeService { Value = "second" };

            ServiceLocator.Register(first);
            ServiceLocator.Register(second);

            var fetched = ServiceLocator.Get<FakeService>();
            Assert.AreSame(second, fetched, "Last registration should win.");
        }

        [Test]
        public void Unregister_RemovesService()
        {
            ServiceLocator.Register(new FakeService());
            Assert.IsTrue(ServiceLocator.Has<FakeService>());

            ServiceLocator.Unregister<FakeService>();

            Assert.IsFalse(ServiceLocator.Has<FakeService>());
            Assert.IsNull(ServiceLocator.Get<FakeService>());
        }

        [Test]
        public void Unregister_WhenNotRegistered_IsSafe()
        {
            Assert.DoesNotThrow(() => ServiceLocator.Unregister<FakeService>());
        }

        [Test]
        public void Clear_RemovesAllServices()
        {
            ServiceLocator.Register(new FakeService());
            ServiceLocator.Register(new OtherService());

            ServiceLocator.Clear();

            Assert.IsFalse(ServiceLocator.Has<FakeService>());
            Assert.IsFalse(ServiceLocator.Has<OtherService>());
        }

        [Test]
        public void Register_NullService_ThrowsArgumentNullException()
        {
            Assert.Throws<System.ArgumentNullException>(() => ServiceLocator.Register<FakeService>(null));
        }

        [Test]
        public void DifferentServiceTypes_RegisteredIndependently()
        {
            var fake = new FakeService();
            var other = new OtherService();

            ServiceLocator.Register(fake);
            ServiceLocator.Register(other);

            Assert.AreSame(fake, ServiceLocator.Get<FakeService>());
            Assert.AreSame(other, ServiceLocator.Get<OtherService>());
        }
    }
}
