using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace Federation.Core
{
    public abstract class AbstractBootstrapper
    {
        protected abstract IEnumerable<ComposablePartCatalog> Catalogs { get; }

        public CompositionContainer Container { get; set; }

        public virtual void Run()
        {
            Container = new CompositionContainer(new AggregateCatalog(Catalogs));
            var mefServiceLocator = new MEFServiceLocator(Container);
            ServiceLocator.SetProvider(() => mefServiceLocator);
            Container.ComposeExportedValue(Container);
            Container.ComposeExportedValue(ServiceLocator.Current);
            ApplicationLoaded();
        }

        protected virtual void ApplicationLoaded()
        {
        }
    }
}
