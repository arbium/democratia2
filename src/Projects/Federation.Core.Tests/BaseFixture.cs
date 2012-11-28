using System;
using System.ComponentModel.Composition.Hosting;
using NUnit.Framework;

namespace Federation.Core.Tests
{
    [TestFixture]
    public abstract class BaseFixture
    {
        protected CompositionContainer Container { get; set; }

        [SetUp]
        public void TestInitialize()
        {
            var testContainer = new CompositionContainer(TestHelper.Container);
            Container = testContainer;
            TestHelper.TestContainer = testContainer;
            var locator = new MEFServiceLocator(testContainer);
            ServiceLocator.SetProvider(() => locator);
            BeforeTest();
        }

        [TearDown]
        public void TestCleanup()
        {
            AfterTest();
            var locator = new MEFServiceLocator(TestHelper.Container);
            ServiceLocator.SetProvider(() => locator);
        }

        protected virtual void BeforeTest()
        {
        }

        protected virtual void AfterTest()
        {
        }
    }
}
