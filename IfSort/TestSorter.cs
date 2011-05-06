using System;
using NUnit.Framework;

namespace IfSort
{
    [TestFixture]
    public class TestSorter
    {
        Sorter sorter;

        [SetUp]
        public void SetUp()
        {
            sorter = new Sorter();
        }

        [Test]
        public void CanOrderInstances()
        {
            var objects = new object[]
                              {
                                  new FirstScenario.SecondClass(),
                                  new FirstScenario.FirstClass()
                              };
            
            sorter.Sort(objects);

            AssertType<FirstScenario.FirstClass>(objects[0]);
            AssertType<FirstScenario.SecondClass>(objects[1]);
        }

        class FirstScenario
        {
            public class FirstClass : IExecuteBefore<SecondClass>{}
            public class SecondClass{}
        }

        [Test]
        public void CanStillOrderThem()
        {
            var objects = new object[]
                              {
                                  new SecondScenario.ThirdClass(),
                                  new SecondScenario.FourthClass(),
                                  new SecondScenario.SecondClass(),
                                  new SecondScenario.FirstClass()
                              };

            sorter.Sort(objects);

            AssertType<SecondScenario.FirstClass>(objects[0]);
            AssertType<SecondScenario.SecondClass>(objects[1]);
            AssertType<SecondScenario.ThirdClass>(objects[2]);
            AssertType<SecondScenario.FourthClass>(objects[3]);
        }

        [Test]
        public void DetectsCycles()
        {
            var objects = new object[]
                              {
                                  new ThirdScenario.ThirdClass(),
                                  new ThirdScenario.FourthClass(),
                                  new ThirdScenario.SecondClass(),
                                  new ThirdScenario.FirstClass()
                              };

            var ex = Assert.Throws<InvalidOperationException>(() => sorter.Sort(objects));
            Console.WriteLine(ex);
        }

        class SecondScenario
        {
            public class FirstClass : IExecuteBefore<SecondClass> {}
            public class SecondClass : IExecuteBefore<ThirdClass> {}
            public class ThirdClass : IExecuteBefore<FourthClass> {}
            public class FourthClass : IExecuteAfter<FirstClass> {}
        }

        class ThirdScenario
        {
            public class FirstClass : IExecuteBefore<SecondClass> {}
            public class SecondClass : IExecuteBefore<ThirdClass> {}
            public class ThirdClass : IExecuteBefore<FourthClass> {}
            public class FourthClass : IExecuteBefore<FirstClass> {}
        }

        void AssertType<T>(object obj)
        {
            Assert.AreEqual(typeof (T), obj.GetType());
        }
    }
}