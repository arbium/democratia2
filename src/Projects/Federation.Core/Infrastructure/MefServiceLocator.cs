using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;

namespace Federation.Core
{
    public class MEFServiceLocator : IServiceLocator //TODO: refactoring
    {
        private readonly CompositionContainer _container;
        public MEFServiceLocator(CompositionContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            _container = container;
        }

        public T GetInstance<T>()
        {
            var export = _container.GetExport<T>();
            if (export == null)
                return default(T);
            return export.Value;
        }

        public T GetNewInstance<T>() where T : class
        {
            var _emptyMetadata =
                Enumerable.Empty<KeyValuePair<string, Type>>();

            var contractName = AttributedModelServices.GetContractName(typeof(T));

            var dif = new ContractBasedImportDefinition(
                contractName,
                AttributedModelServices.GetTypeIdentity(typeof(T)),
                _emptyMetadata,
                ImportCardinality.ZeroOrOne,
                false,
                true,
                CreationPolicy.NonShared);

            var exports = _container.GetExports(dif);

            if (exports == null)
                return default(T);

            if (exports.Count() == 0)
                return default(T);

            return (T)exports.First().Value;
        }

        public T GetNewInstance<T>(string key) where T : class
        {
            List<KeyValuePair<string, Type>> keys = new List<KeyValuePair<string, Type>>();
            keys.Add(new KeyValuePair<string, Type>(key, typeof(T)));
                
            var _emptyMetadata = Enumerable.Empty<KeyValuePair<string, Type>>();

            var contractName = AttributedModelServices.GetContractName(typeof(T));

            var def = new ContractBasedImportDefinition(
                key,
                AttributedModelServices.GetTypeIdentity(typeof(T)),
                _emptyMetadata,
                ImportCardinality.ZeroOrOne,
                false,
                true,
                CreationPolicy.NonShared);

            var exports = _container.GetExports(def);

            if (exports == null)
                return default(T);

            if (exports.Count() == 0)
                return default(T);

            return (T)exports.First().Value;
        }

        public IEnumerable<T> GetAllInstances<T>()
        {
            var exports = _container.GetExports<T>();
            foreach (var export in exports)
            {
                yield return export.Value;
            }
        }

        public T GetInstance<T>(string key)
        {
            var export = _container.GetExport<T>(key);
            if (export == null)
                return default(T);
            return export.Value;
        }
    }
}
