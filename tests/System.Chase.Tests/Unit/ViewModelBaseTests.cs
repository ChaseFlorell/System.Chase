using System.Chase.Mvvm;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace System.Chase.Tests.Unit
{
    internal class TestViewModel : ObservableObject
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
    }
    
    [TestFixture]
    public class ObservableObjectTests
    {
        
        [Test]
        public void ShouldCallPropertyChangedOnlyOnce()
        {
            // setup
            var vm = new TestViewModel();
            const int loopCount = 100;
            const int expectedRaiseCount = 1;
            const string nameFormat = "Name {0}";
            var numberOfTimesNameIsChanged = 0;
            var raiseCount = 0;
            vm.PropertyChanged += (sender, args) => raiseCount++;
        
            // execute
            using (vm.SuppressChangeNotifications())
            {
                for (var i = 1; i <= loopCount; i++)
                {
                    vm.Name = string.Format(nameFormat, i);
                    numberOfTimesNameIsChanged++;
                }
            }
        
            // assert
            vm.Name.Should().Be(string.Format(nameFormat, loopCount));
            raiseCount.Should().Be(expectedRaiseCount);
            numberOfTimesNameIsChanged.Should().Be(loopCount);
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
            vm.IsBusy = false;
            vm.IsBusy.Should().BeFalse();
            vm.IsNotBusy.Should().BeTrue();
        }

        [Test]
        public void ShouldManageMultipleBusy()
        {
            var vm = new TestViewModel();
            vm.IsBusy.Should().BeFalse();
            vm.IsNotBusy.Should().BeTrue();

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
        }

        [Test]
        public void ShouldManageMultipleBusyWithDirectSetAndOnlyRaiseTwoPropertyChanges()
        {
            var raiseCount = 0;
            const int expectedRaiseCount = 2; // one for busy and one for not busy
            
            var vm = new TestViewModel();
            vm.PropertyChanged += (sender, args) => raiseCount++;
            
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

            raiseCount.Should().Be(expectedRaiseCount);
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