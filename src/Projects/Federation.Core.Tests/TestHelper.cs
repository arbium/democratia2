using System;
using System.ComponentModel.Composition.Hosting;

namespace Federation.Core.Tests
{
    public static class TestHelper
    {
        public static CompositionContainer Container { get; set; }
        public static CompositionContainer TestContainer { get; set; }
    }
}
