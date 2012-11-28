using NUnit.Framework;

namespace Federation.Core.Tests
{
    [SetUpFixture]
    public class AssemblyInitializer
    {
        public AssemblyInitializer()
        {
            new Bootstrapper().Run();
        }
    }
}
