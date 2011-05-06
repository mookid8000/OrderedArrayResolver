using System;
using System.Diagnostics;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using NUnit.Framework;

namespace IfSort
{
    [TestFixture]
    public class TestWindsor
    {
        // Initial:
        // 1000 iterations took 0,2 s
        // 1000 iterations took 5,8 s
        //
        // Med cache: string => Dict
        // 1000 iterations took 0,2 s
        // 1000 iterations took 0,2 s
        //
        [Test]
        public void ComparePerformance()
        {
            var iterations = 1000;

            var container = GetContainerWithArrayResolver();
            MakeRegistrations(container);
            Go(container, iterations);

            container = GetContainerWithOrderedArrayResolver();
            MakeRegistrations(container);
            Go(container, iterations);
        }

        WindsorContainer GetContainerWithArrayResolver()
        {
            var container = new WindsorContainer();
            container.Kernel.Resolver.AddSubResolver(new ArrayResolver(container.Kernel, true));
            return container;
        }

        WindsorContainer GetContainerWithOrderedArrayResolver()
        {
            var container = new WindsorContainer();
            container.Kernel.Resolver.AddSubResolver(new OrderedArrayResolver(container.Kernel, true));
            return container;
        }

        void Go(WindsorContainer container, int iterations)
        {
            var stopwatch = Stopwatch.StartNew();
            for(var counter = 0; counter < iterations; counter++)
            {
                var something = container.Resolve<Something>();
                container.Release(something);
            }
            Console.WriteLine("{0} iterations took {1:0.000} s", iterations, stopwatch.Elapsed.TotalSeconds);
        }

        WindsorContainer MakeRegistrations(WindsorContainer container)
        {
            container.Register(AllTypes.FromThisAssembly().IncludeNonPublicTypes()
                                   .BasedOn<ISomeService>()
                                   .WithService.Base()
                                   .Configure(c => c.LifeStyle.Transient));
            
            container.Register(Component.For<Something>().LifeStyle.Transient);
            
            return container;
        }

        class Class1 : ISomeService {}
        class Class2 : ISomeService {}
        class Class3 : ISomeService {}
        class Class4 : ISomeService {}
        class Class5 : ISomeService {}
        class Class6 : ISomeService {}
        class Class7 : ISomeService {}
        class Class8 : ISomeService {}
        class Class9 : ISomeService {}
        class Class10 : ISomeService {}
        class Class11 : ISomeService {}

        [Test]
        public void WorksWithAttributesAsWell()
        {
            var container = new WindsorContainer();
            container.Kernel.Resolver.AddSubResolver(new OrderedArrayResolver(container.Kernel));

            container.Register(Component.For<Something>());

            container.Register(Component.For<ISomeService>().ImplementedBy<Third_Attr>(),
                               Component.For<ISomeService>().ImplementedBy<First_Attr>(),
                               Component.For<ISomeService>().ImplementedBy<Fourth_Attr>(),
                               Component.For<ISomeService>().ImplementedBy<Second_Attr>());

            var something = container.Resolve<Something>();
            var services = something.SomeServices;

            Assert.AreEqual(typeof (First_Attr), services[0].GetType());
            Assert.AreEqual(typeof (Second_Attr), services[1].GetType());
            Assert.AreEqual(typeof (Third_Attr), services[2].GetType());
            Assert.AreEqual(typeof (Fourth_Attr), services[3].GetType());
        }

        class Something
        {
            ISomeService[] someServices;

            public Something(ISomeService[] someServices)
            {
                this.someServices = someServices;
            }

            public ISomeService[] SomeServices
            {
                get { return someServices; }
            }
        }

        class Third_Attr : ISomeService { }
        
        [ExecutesAfter(typeof(Third_Attr))]
        class Fourth_Attr : ISomeService { }

        [ExecutesBefore(typeof(Third_Attr))]
        class Second_Attr : ISomeService {}

        [ExecutesBefore(typeof(Second_Attr))]
        class First_Attr : ISomeService { }

        [Test]
        public void CanResolveServicesInOrder()
        {
            var container = new WindsorContainer();
            container.Kernel.Resolver.AddSubResolver(new OrderedArrayResolver(container.Kernel));

            container.Register(Component.For<ISomeService>().Named("4").ImplementedBy<Fourth>(),
                               Component.For<ISomeService>().Named("3").ImplementedBy<Third>(),
                               Component.For<ISomeService>().Named("1").ImplementedBy<First>(),
                               Component.For<ISomeService>().Named("5").ImplementedBy<Fifth>(),
                               Component.For<ISomeService>().Named("2").ImplementedBy<Second>(),
                               Component.For<NeedsSomeServices>());
            
            var service = container.Resolve<NeedsSomeServices>();
            var services = service.SomeServices;

            Assert.AreEqual(typeof(First), services[0].GetType());
            Assert.AreEqual(typeof(Second), services[1].GetType());
            Assert.AreEqual(typeof(Third), services[2].GetType());
            Assert.AreEqual(typeof(Fourth), services[3].GetType());
            Assert.AreEqual(typeof(Fifth), services[4].GetType());
        }

        void ComponentCreated(ComponentModel model, object instance)
        {
            Console.WriteLine("created!!");
        }

        class NeedsSomeServices
        {
            readonly ISomeService[] someServices;

            public NeedsSomeServices(ISomeService[] someServices)
            {
                this.someServices = someServices;
            }

            public ISomeService[] SomeServices
            {
                get { return someServices; }
            }
        }
        interface ISomeService
        {
        }
        class First : ISomeService, IExecuteBefore<Second>, IExecuteBefore<Third>
        {
        }
        class Second : ISomeService, IExecuteBefore<Third>
        {
        }
        class Third : ISomeService, IExecuteBefore<Fourth>
        {
        }
        class Fourth : ISomeService
        {
        }
        class Fifth : ISomeService, IExecuteAfter<Fourth>, IExecuteAfter<First>
        {
        }
    }
}