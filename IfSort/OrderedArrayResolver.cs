using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;

namespace IfSort
{
    public class OrderedArrayResolver : ISubDependencyResolver
    {
        readonly ArrayResolver innerArrayResolver;
        readonly Sorter sorter;

        public OrderedArrayResolver(IKernel kernel)
            : this(kernel, true)
        {
        }

        public OrderedArrayResolver(IKernel kernel, bool allowEmptyList)
        {
            innerArrayResolver = new ArrayResolver(kernel, allowEmptyList);
            sorter = new Sorter();
        }

        public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
        {
            var objects = innerArrayResolver.Resolve(context, contextHandlerResolver, model, dependency);
            
            if (objects != null && objects.GetType().IsArray)
            {
                sorter.Sort((object[]) objects);
            }
            
            return objects;
        }

        public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
        {
            return innerArrayResolver.CanResolve(context, contextHandlerResolver, model, dependency);
        }
    }
}