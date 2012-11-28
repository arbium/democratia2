using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using Federation.Core;

namespace Federation.MailService
{
        public class Bootstrapper : AbstractBootstrapper
        {
            protected override IEnumerable<ComposablePartCatalog> Catalogs
            {
                get
                {
                    yield return new AssemblyCatalog(typeof(IServiceLocator).Assembly);
                    yield return new AssemblyCatalog(typeof(DailyMailTask).Assembly);
                }
            }

            protected override void ApplicationLoaded()
            {

            }
        }
    
}