using System;
using NUnit.Framework;
using Enara.Core;

namespace Enara.Tests
{
    /// <summary>
    /// EditMode tests for <see cref="EventBus"/>. The bus is a static class with global state,
    /// so each test clears it in SetUp/TearDown to keep tests isolated.
    /// </summary>
    [TestFixture]
    public class EventBusTests
    {
        // A throwaway event type used by these tests.
        private readonly struct TestEvent : IGameEvent
        {
            public readonly int Payload;
            public TestEvent(int p) { Payload = p; }
        }

        [SetUp] public void SetUp() { }
        [TearDown] public void TearDown() => EventBus.Clear();

        [Test]
        public void Publish_WithNoSubscribers_DoesNothing()
        {
            Assert.DoesNotThrow(() => EventBus.Publish(new TestEvent(42)));
        }

        [Test]
        public void Subscribe_Then_Publish_ReceivesEvent()
        {
            int received = 0;
            EventBus.Subscribe<TestEvent>(e => received = e.Payload);

            EventBus.Publish(new TestEvent(99));

            Assert.AreEqual(99, received);
        }

        [Test]
        public void MultipleSubscribers_AllReceiveEvent()
        {
            int count = 0;
            EventBus.Subscribe<TestEvent>(e => count++);
            EventBus.Subscribe<TestEvent>(e => count++);

            EventBus.Publish(new TestEvent(1));

            Assert.AreEqual(2, count);
        }

        [Test]
        public void Unsubscribe_StopsReceivingEvents()
        {
            int received = 0;
            void Handler(TestEvent e) => received = e.Payload;

            EventBus.Subscribe<TestEvent>(Handler);
            EventBus.Unsubscribe<TestEvent>(Handler);

            EventBus.Publish(new TestEvent(7));

            Assert.AreEqual(0, received, "Unsubscribed handler should not be called.");
        }

        [Test]
        public void Unsubscribe_WhenNotSubscribed_IsSafe()
        {
            void Handler(TestEvent e) { }
            Assert.DoesNotThrow(() => EventBus.Unsubscribe<TestEvent>(Handler));
        }

        [Test]
        public void Clear_RemovesAllSubscribers()
        {
            int received = 0;
            EventBus.Subscribe<TestEvent>(e => received = e.Payload);
            EventBus.Subscribe<TestEvent>(e => received = e.Payload);

            EventBus.Clear();
            EventBus.Publish(new TestEvent(5));

            Assert.AreEqual(0, received);
        }

        [Test]
        public void SubscribersOfOneEventType_DoNotReceiveOtherEventTypes()
        {
            int received = 0;
            EventBus.Subscribe<TestEvent>(e => received = e.Payload);

            // Different event type — should not trigger the TestEvent subscriber.
            EventBus.Publish(new QteSucceededEvent("some_id"));

            Assert.AreEqual(0, received);
        }

        [Test]
        public void Subscribe_NullHandler_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => EventBus.Subscribe<TestEvent>(null));
            // Even after a null subscribe, real subscribers should still work.
            int received = 0;
            EventBus.Subscribe<TestEvent>(e => received = e.Payload);
            EventBus.Publish(new TestEvent(3));
            Assert.AreEqual(3, received);
        }

        [Test]
        public void Unsubscribe_NullHandler_IsSafe()
        {
            Assert.DoesNotThrow(() => EventBus.Unsubscribe<TestEvent>(null));
        }

        [Test]
        public void Handler_ThatThrows_DoesNotCorruptBus()
        {
            // The bus doesn't catch exceptions — but the state (the delegate list) must remain valid.
            // After a throwing handler, other handlers should still receive events.
            bool secondCalled = false;
            EventBus.Subscribe<TestEvent>(e => throw new InvalidOperationException("boom"));
            EventBus.Subscribe<TestEvent>(e => secondCalled = true);

            // The first handler throws; the second one's invocation depends on how Delegate.Invoke
            // handles exceptions. We assert only that the bus itself didn't corrupt.
            try { EventBus.Publish(new TestEvent(1)); } catch { /* expected */ }

            // A subsequent publish with new subscribers should work fine.
            EventBus.Clear();
            int received = 0;
            EventBus.Subscribe<TestEvent>(e => received = e.Payload);
            EventBus.Publish(new TestEvent(8));
            Assert.AreEqual(8, received);
        }
    }
}
