using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using Federation.Core;
using System;
using Federation.Web.Controllers;

namespace Federation.Core.Tests
{
        public class Bootstrapper : AbstractBootstrapper
        {
            protected override IEnumerable<ComposablePartCatalog> Catalogs
            {
                get
                {
                    yield return new AssemblyCatalog(typeof(IServiceLocator).Assembly);
                    yield return new AssemblyCatalog(typeof(BaseFixture).Assembly);//BaseFixture
                }
            }

            protected override void ApplicationLoaded()
            {
                TestHelper.Container = Container;
            }
        }
    
}