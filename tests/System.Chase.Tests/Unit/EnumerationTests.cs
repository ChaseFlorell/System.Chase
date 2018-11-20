using System.Chase.Tests.Fixtures;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
// ReSharper disable UnusedVariable

namespace System.Chase.Tests.Unit
{
    
    [TestFixture]
    public class EnumerationTests
    {
        [Test]
        public void ShouldTestEqualityBetweenSameTypes()
        {
            // setup
            var emp1 = EmployeeType.RegionalManager;
            var emp2 = EmployeeType.RegionalManager;
            
            // assert
            emp1.Should().Be(emp2);
        }

        [Test]
        public void ShouldTestInequalityBetweenSameTypes()
        {
            // setup
            var emp1 = EmployeeType.RegionalManager;
            var emp2 = EmployeeType.AssistantToTheRegionalManager;
            
            // assert
            emp1.Should().NotBe(emp2);
        }

        [Test]
        public void ShouldTestEqualityWhereRightIsInt()
        {
            // setup
            var emp1 = EmployeeType.RegionalManager;
            var emp2 = EmployeeType.RegionalManager.Value;
            
            // execute
            var result = emp1 == emp2;
            // assert
            result.Should().BeTrue();
        }

        [Test]
        public void ShouldTestInequalityWhereRightIsInt()
        {
            // setup
            var emp1 = EmployeeType.RegionalManager;
            var emp2 = EmployeeType.AssistantToTheRegionalManager.Value;
            
            // execute
            var result = emp1 == emp2;
            // assert
            result.Should().BeFalse();
        }

        [Test]
        public void ShouldTestEqualityWhereRightIsString()
        {
            // setup
            var emp1 = EmployeeType.RegionalManager;
            var emp2 = EmployeeType.RegionalManager.DisplayName;
            
            // execute
            var result = emp1 == emp2;
            // assert
            result.Should().BeTrue();
        }

        [Test]
        public void ShouldTestInequalityWhereRightIsString()
        {
            // setup
            var emp1 = EmployeeType.RegionalManager;
            var emp2 = EmployeeType.AssistantToTheRegionalManager.DisplayName;
            
            // execute
            var result = emp1 == emp2;
            // assert
            result.Should().BeFalse();
        }

        [Test]
        public void ShouldTestEqualityWhereLeftIsInt()
        {
            // setup
            var emp1 = EmployeeType.RegionalManager.Value;
            var emp2 = EmployeeType.RegionalManager;
            
            // execute
            var result = emp1 == emp2;
            // assert
            result.Should().BeTrue();
        }

        [Test]
        public void ShouldTestInequalityWhereLeftsInt()
        {
            // setup
            var emp1 = EmployeeType.Sales.Value;
            var emp2 = EmployeeType.RegionalManager;
            
            // execute
            var result = emp1 == emp2;
            // assert
            result.Should().BeFalse();
        }

        [Test]
        public void ShouldTestEqualityWhereLeftIsString()
        {
            // setup
            var emp1 = EmployeeType.RegionalManager.DisplayName;
            var emp2 = EmployeeType.RegionalManager;
            
            // execute
            var result = emp1 == emp2;
            // assert
            result.Should().BeTrue();
        }

        [Test]
        public void ShouldTestInequalityWhereLeftsString()
        {
            // setup
            var emp1 = EmployeeType.AssistantToTheRegionalManager.DisplayName;
            var emp2 = EmployeeType.RegionalManager;
            
            // execute
            var result = emp1 == emp2;
            // assert
            result.Should().BeFalse();
        }

        [Test]
        public void ShouldGetEmployeeTypeFromDisplayName()
        {
            // setup
            var displayName = EmployeeType.AssistantToTheRegionalManager.DisplayName;
            var expectedResult = EmployeeType.AssistantToTheRegionalManager;
            
            // execute
            var result = Enumeration.FromDisplayName<EmployeeType>(displayName);
            
            // assert
            result.Value.Should().Be(expectedResult.Value);
            result.DisplayName.Should().Be(expectedResult.DisplayName);
        }

        [Test]
        public void ShouldGetEmployeeTypeFromValue()
        {
            // setup
            var value = EmployeeType.AssistantToTheRegionalManager.Value;
            var expectedResult = EmployeeType.AssistantToTheRegionalManager;
            
            // execute
            var result = Enumeration.FromValue<EmployeeType>(value);
            
            // assert
            result.Value.Should().Be(expectedResult.Value);
            result.DisplayName.Should().Be(expectedResult.DisplayName);
        }

        [Test]
        public void ShouldGetAllEmployeeTypes()
        {
            // setup
            const int expectedCount = 3;
            var first = (EmployeeType)EmployeeType.RegionalManager.Value;
            var second = (EmployeeType)EmployeeType.Sales.DisplayName;
            var third = (EmployeeType)EmployeeType.AssistantToTheRegionalManager.Value;
            
            // execute
            var results = Enumeration.GetAll<EmployeeType>().ToList();
            
            // assert
            results.Count.Should().Be(expectedCount);
            results[0].Should().Be(first);
            results[1].Should().Be(second);
            results[2].Should().Be(third);
        }

        [Test]
        public void ShouldThrowOnInvalidValue()
        {
            // setup
            const int invalidType = 10;
            
            // execute
            Action action = () =>
            {
                var x = Enumeration.FromValue<EmployeeType>(invalidType);
            };

            action.Should().Throw<Exception>().WithMessage($"Failed to parse {typeof(EmployeeType).FullName} because '{invalidType}' is not a valid value");
        }

        [Test]
        public void ShouldThrowOnInvalidDisplayName()
        {
            // setup
            const string invalidType = "gopher";
            
            // execute
            Action action = () =>
            {
                var x = Enumeration.FromDisplayName<EmployeeType>(invalidType);
            };

            action.Should().Throw<ArgumentException>().WithMessage($"Failed to parse {typeof(EmployeeType).FullName} because '{invalidType}' is not a valid display name");
        }

        [Test]
        public void ShouldRetrieveDefaultValue()
        {
            // setup
            const string expectedDisplayName = null;
            const int expectedValue = 0;
            
            // execute
            var result = (EmployeeType)0;
            
            // assert
            result.Value.Should().Be(expectedValue);
            result.DisplayName.Should().Be(expectedDisplayName);
        }

        [Test]
        public void ShouldRetrieveUnknownValue()
        {
            // setup
            var expectedDisplayName = VehicleType.Unknown.DisplayName;
            const int expectedValue = 0;
            
            // execute
            var result = (VehicleType)0;
            
            // assert
            result.Value.Should().Be(expectedValue);
            result.DisplayName.Should().Be(expectedDisplayName);  
        }
    }
}
