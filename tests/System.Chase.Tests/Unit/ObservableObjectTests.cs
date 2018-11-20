using System.Chase.Tests.Fixtures;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace System.Chase.Tests.Unit
{
    
    [TestFixture]
    public class ObservableObjectTests
    {
        [Test]
        public void ShouldSafelyRunVolatileCode()
        {
            // setup
            var vm = new TestViewModel();
            const string expectedErrorMessage = "barf";
            void ThrowError() => throw new Exception(expectedErrorMessage);

            // execute
            Action action = () => vm.SafeActionRunner(ThrowError);
            
            // assert
            action.Should().NotThrow();
            vm.LastErrorMessage.Should().Be(expectedErrorMessage);
        }
        
        [Test]
        public void ShouldFailToRunVolatileCode()
        {
            // setup
            var vm = new TestViewModel();
            const string expectedErrorMessage = "barf";
            void ThrowError() => throw new Exception(expectedErrorMessage);

            // execute
            Action action = () => vm.UnsafeActionRunner(ThrowError);
            
            // assert
            action.Should().Throw<Exception>().WithMessage(expectedErrorMessage);
        }
        
        [Test]
        public void ShouldCallPropertyChangedOnlyOnce()
        {
            // setup
            var propertyChangedCount = 0;
            var vm = new TestViewModel();
            vm.PropertyChanged += (sender, args) => propertyChangedCount++;
            
            const int loopCount = 100;
            const int expectedRaiseCount = 1;
            const string nameFormat = "Name {0}";
            var actualNumberOfChanges = 0;
        
            // execute
            using (vm.SuppressChangeNotifications())
            for (var index = 1; index <= loopCount; index++)
            {
                vm.Name = string.Format(nameFormat, index);
                actualNumberOfChanges = index;
            }
            
        
            // assert
            vm.Name.Should().Be(string.Format(nameFormat, loopCount));
            propertyChangedCount.Should().Be(expectedRaiseCount);
            actualNumberOfChanges.Should().Be(loopCount);
        }
        
        [Test]
        public void ShouldSetBusyDirect()
        {
            // the default/old way
            var vm = new TestViewModel();
            vm.IsBusy.Should().BeFalse();
            vm.IsNotBusy.Should().BeTrue();
            vm.IsBusy = true;
            vm.IsBusy.Should().BeTrue();
            vm.IsNotBusy.Should().BeFalse();
            vm.IsBusy = true;
            vm.IsBusy.Should().BeTrue();
            vm.IsNotBusy.Should().BeFalse();
            vm.IsBusy = false;
            vm.IsBusy.Should().BeFalse();
            vm.IsNotBusy.Should().BeTrue();
        }

        [Test]
        public void ShouldManageMultipleBusy()
        {
            const int expectedChangeCount = 2; // one for IsBusy and one for IsNotBusy
            var propertyChangedCount = 0;
            var vm = new TestViewModel();
            vm.PropertyChanged += (sender, args) => propertyChangedCount++;
            
            vm.IsBusy.Should().BeFalse();
            vm.IsNotBusy.Should().BeTrue();

            using (vm.SuppressChangeNotifications())
            using (vm.Busy())
            {
                vm.IsBusy.Should().BeTrue();
                vm.IsNotBusy.Should().BeFalse();
                using (vm.Busy())
                {
                    vm.IsBusy.Should().BeTrue();
                    vm.IsNotBusy.Should().BeFalse();
                }
                vm.IsBusy.Should().BeTrue();
                vm.IsNotBusy.Should().BeFalse();
            }

            vm.IsBusy.Should().BeFalse();
            vm.IsNotBusy.Should().BeTrue();
            propertyChangedCount.Should().Be(expectedChangeCount);
        }

        [Test]
        public void ShouldManageMultipleBusyWithDirectSetAndOnlyRaiseTwoPropertyChanges()
        {
            var propertyChangedCount = 0;
            const int expectedRaiseCount = 2; // one for busy and one for not busy
            
            var vm = new TestViewModel();
            vm.PropertyChanged += (sender, args) => propertyChangedCount++;
            
            using (vm.SuppressChangeNotifications())
            {
                vm.IsBusy.Should().BeFalse();
                vm.IsNotBusy.Should().BeTrue();
                vm.IsBusy = true;
                vm.IsBusy.Should().BeTrue();
                vm.IsNotBusy.Should().BeFalse();
                using (vm.Busy())
                {
                    vm.IsBusy.Should().BeTrue();
                    vm.IsNotBusy.Should().BeFalse();
                    using (vm.Busy())
                    {
                        vm.IsBusy.Should().BeTrue();
                        vm.IsNotBusy.Should().BeFalse();
                        using (vm.Busy())
                        {
                            vm.IsBusy.Should().BeTrue();
                            vm.IsNotBusy.Should().BeFalse();
                            using (vm.Busy())
                            {
                                vm.IsBusy = false;
                                vm.IsBusy.Should().BeTrue();
                                vm.IsNotBusy.Should().BeFalse();
                                using (vm.Busy())
                                {
                                    vm.IsBusy.Should().BeTrue();
                                    vm.IsNotBusy.Should().BeFalse();
                                }
                            }
                        }
                    }

                    vm.IsBusy.Should().BeTrue();
                    vm.IsNotBusy.Should().BeFalse();
                }

                vm.IsBusy.Should().BeFalse();
                vm.IsNotBusy.Should().BeTrue();
                vm.IsBusy = true;
                vm.IsBusy.Should().BeTrue();
                vm.IsNotBusy.Should().BeFalse();
                vm.IsBusy = false;
                vm.IsBusy.Should().BeFalse();
                vm.IsNotBusy.Should().BeTrue();
            }

            propertyChangedCount.Should().Be(expectedRaiseCount);
        }

        [Test]
        public void ShouldHandleMultipleAsyncBusy()
        {
            const int shortDelay = 1000;
            const int longDelay = 2000;
            var vm = new TestViewModel();
            vm.IsBusy.Should().BeFalse();
            vm.IsNotBusy.Should().BeTrue();
            Task.Run(async () =>
            {
                using (vm.Busy())
                {
                    vm.IsBusy.Should().BeTrue();
                    await Task.Delay(shortDelay);
                }

                vm.IsBusy.Should().BeTrue(because: "because the second method runs longer.");
            });
            Task.Run(async () =>
            {
                using (vm.Busy())
                {
                    vm.IsBusy.Should().BeTrue();
                    vm.IsNotBusy.Should().BeFalse();
                    await Task.Delay(longDelay);
                }
                vm.IsBusy.Should().BeFalse();
            });
            vm.IsBusy.Should().BeFalse();
            vm.IsNotBusy.Should().BeTrue();
        }
    }
}
