using System;
using System.Collections.Generic;
using System.Composition.Hosting.Core;

namespace RPGM.Notes.Extensions
{
    public sealed class InstanceExportDescriptorProvider<TContract> : ExportDescriptorProvider
    {
        private readonly Func<IEnumerable<CompositionDependency>, ExportDescriptor> getDescriptor;
        private readonly TContract instance;
        private readonly string origin;

        public InstanceExportDescriptorProvider(TContract instance)
        {
            if (instance == null) throw new ArgumentNullException("instance");

            this.getDescriptor = c => ExportDescriptor.Create((a, m) => instance, new Dictionary<string, object>());
            this.instance = instance;
            this.origin = instance.ToString();
        }

        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(CompositionContract contract, DependencyAccessor descriptorAccessor)
        {
            if (typeof(TContract) == contract.ContractType)
            {
                yield return new ExportDescriptorPromise(contract, origin, true, NoDependencies, getDescriptor);
            }
        }
    }
}
