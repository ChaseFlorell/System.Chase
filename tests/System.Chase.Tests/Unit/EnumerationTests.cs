using System.Chase.Tests.Fixtures;
using System.Collections.Generic;
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
            var emp2 = 1;
            
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
            var emp2 = 2;
            
            // execute
            var result = emp1 == emp2;
            // assert
            result.Should().BeFalse();
        }

        [Test]
        public void ShouldTestEqualityWhereLeftIsInt()
        {
            // setup
            var emp1 = 1;
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
            var emp1 = 2;
            var emp2 = EmployeeType.RegionalManager;
            
            // execute
            var result = emp1 == emp2;
            // assert
            result.Should().BeFalse();
        }

        [Test]
        public void ShouldCastKeyValuePairToEnumeration()
        {
            // setup
            var value = EmployeeType.RegionalManager.Value;
            var displayName = EmployeeType.RegionalManager.DisplayName;
            var item = new KeyValuePair<int, string>(value,displayName);
            var enumeration = EmployeeType.RegionalManager;
            
            // execute
            var cast = (Enumeration) item;
            
            // assert
            cast.Value.Should().Be(enumeration.Value);
            cast.DisplayName.Should().Be(enumeration.DisplayName);
        }

        [Test]
        public void ShouldGetEmployeeTypeFromDisplayName()
        {
            // setup
            const string displayName = "Assistant to the Regional Manager";
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
            const int value = 3;
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
            var first = (EmployeeType)1;
            var second = (EmployeeType)"Sales";
            var third = (EmployeeType)3;
            
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
            const int invalidType = 4;
            
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

            action.Should().Throw<Exception>().WithMessage($"Failed to parse {typeof(EmployeeType).FullName} because '{invalidType}' is not a valid display name");
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
            const string expectedDisplayName = "Unknown Vehicle Type";
            const int expectedValue = 0;
            
            // execute
            var result = (VehicleType)0;
            
            // assert
            result.Value.Should().Be(expectedValue);
            result.DisplayName.Should().Be(expectedDisplayName);  
        }
    }
}
