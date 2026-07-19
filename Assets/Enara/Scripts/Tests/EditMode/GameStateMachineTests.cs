using System;
using NUnit.Framework;
using Enara.Core;

namespace Enara.Tests
{
    /// <summary>
    /// EditMode tests for <see cref="GameStateMachine"/>. The state machine is a plain class
    /// with no Unity dependencies, so it tests cleanly.
    /// </summary>
    [TestFixture]
    public class GameStateMachineTests
    {
        private GameStateMachine _machine;

        [SetUp]
        public void SetUp()
        {
            _machine = new GameStateMachine();
        }

        [Test]
        public void Constructor_DefaultsToBootState()
        {
            Assert.AreEqual(GameState.Boot, _machine.Current);
        }

        [Test]
        public void TransitionTo_NewState_ChangesCurrent()
        {
            _machine.TransitionTo(GameState.Exploration);

            Assert.AreEqual(GameState.Exploration, _machine.Current);
        }

        [Test]
        public void TransitionTo_SameState_ReturnsFalseAndDoesNotFireEvent()
        {
            _machine.TransitionTo(GameState.Exploration);
            bool eventFired = false;
            _machine.OnStateChanged += (prev, next) => eventFired = true;

            bool result = _machine.TransitionTo(GameState.Exploration);

            Assert.IsFalse(result, "TransitionTo should return false for same-state transitions.");
            Assert.IsFalse(eventFired, "OnStateChanged should not fire for same-state transitions.");
        }

        [Test]
        public void TransitionTo_DifferentState_ReturnsTrue()
        {
            bool result = _machine.TransitionTo(GameState.Cutscene);

            Assert.IsTrue(result);
        }

        [Test]
        public void OnStateChanged_FiresWithCorrectPreviousAndNext()
        {
            GameState? firedPrev = null;
            GameState? firedNext = null;
            _machine.OnStateChanged += (prev, next) => { firedPrev = prev; firedNext = next; };

            _machine.TransitionTo(GameState.Qte);

            Assert.AreEqual(GameState.Boot, firedPrev);
            Assert.AreEqual(GameState.Qte, firedNext);
        }

        [Test]
        public void OnStateChanged_CanHaveMultipleSubscribers()
        {
            int callCount = 0;
            _machine.OnStateChanged += (prev, next) => callCount++;
            _machine.OnStateChanged += (prev, next) => callCount++;

            _machine.TransitionTo(GameState.Dialogue);

            Assert.AreEqual(2, callCount);
        }

        [Test]
        public void IsInputAllowed_TrueForExploration()
        {
            _machine.TransitionTo(GameState.Exploration);
            Assert.IsTrue(_machine.IsInputAllowed);
        }

        [Test]
        public void IsInputAllowed_TrueForQte()
        {
            _machine.TransitionTo(GameState.Qte);
            Assert.IsTrue(_machine.IsInputAllowed);
        }

        [Test]
        public void IsInputAllowed_TrueForDialogue()
        {
            _machine.TransitionTo(GameState.Dialogue);
            Assert.IsTrue(_machine.IsInputAllowed);
        }

        [Test]
        public void IsInputAllowed_FalseForBoot()
        {
            // Already in Boot state from constructor.
            Assert.IsFalse(_machine.IsInputAllowed);
        }

        [Test]
        public void IsInputAllowed_FalseForCutscene()
        {
            _machine.TransitionTo(GameState.Cutscene);
            Assert.IsFalse(_machine.IsInputAllowed);
        }

        [Test]
        public void IsInputAllowed_FalseForMenu()
        {
            _machine.TransitionTo(GameState.Menu);
            Assert.IsFalse(_machine.IsInputAllowed);
        }

        [Test]
        public void IsInputAllowed_FalseForPaused()
        {
            _machine.TransitionTo(GameState.Paused);
            Assert.IsFalse(_machine.IsInputAllowed);
        }

        [Test]
        public void IsInputAllowed_FalseForEnding()
        {
            _machine.TransitionTo(GameState.Ending);
            Assert.IsFalse(_machine.IsInputAllowed);
        }

        [Test]
        public void TransitionTo_CanTraverseAllStatesInOrder()
        {
            _machine.TransitionTo(GameState.Cutscene);
            Assert.AreEqual(GameState.Cutscene, _machine.Current);
            _machine.TransitionTo(GameState.Exploration);
            Assert.AreEqual(GameState.Exploration, _machine.Current);
            _machine.TransitionTo(GameState.Qte);
            Assert.AreEqual(GameState.Qte, _machine.Current);
            _machine.TransitionTo(GameState.Dialogue);
            Assert.AreEqual(GameState.Dialogue, _machine.Current);
            _machine.TransitionTo(GameState.Menu);
            Assert.AreEqual(GameState.Menu, _machine.Current);
            _machine.TransitionTo(GameState.Paused);
            Assert.AreEqual(GameState.Paused, _machine.Current);
            _machine.TransitionTo(GameState.Ending);
            Assert.AreEqual(GameState.Ending, _machine.Current);
        }
    }
}
