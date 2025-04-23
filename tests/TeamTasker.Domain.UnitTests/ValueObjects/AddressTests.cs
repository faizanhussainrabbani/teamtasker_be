using FluentAssertions;
using TeamTasker.Domain.ValueObjects;
using Xunit;

namespace TeamTasker.Domain.UnitTests.ValueObjects
{
    public class AddressTests
    {
        [Fact]
        public void Constructor_ShouldCreateAddress_WithCorrectProperties()
        {
            // Arrange
            var street = "123 Main St";
            var city = "New York";
            var state = "NY";
            var country = "USA";
            var zipCode = "10001";

            // Act
            var address = new Address(street, city, state, country, zipCode);

            // Assert
            address.Street.Should().Be(street);
            address.City.Should().Be(city);
            address.State.Should().Be(state);
            address.Country.Should().Be(country);
            address.ZipCode.Should().Be(zipCode);
        }

        [Fact]
        public void Equals_ShouldReturnTrue_WhenAddressesHaveSameValues()
        {
            // Arrange
            var address1 = new Address("123 Main St", "New York", "NY", "USA", "10001");
            var address2 = new Address("123 Main St", "New York", "NY", "USA", "10001");

            // Act & Assert
            address1.Should().Be(address2);
            address1.GetHashCode().Should().Be(address2.GetHashCode());
        }

        [Fact]
        public void Equals_ShouldReturnFalse_WhenAddressesHaveDifferentValues()
        {
            // Arrange
            var address1 = new Address("123 Main St", "New York", "NY", "USA", "10001");
            var address2 = new Address("456 Elm St", "Boston", "MA", "USA", "02108");

            // Act & Assert
            address1.Should().NotBe(address2);
            address1.GetHashCode().Should().NotBe(address2.GetHashCode());
        }

        [Fact]
        public void Equals_ShouldReturnFalse_WhenComparedWithNull()
        {
            // Arrange
            var address = new Address("123 Main St", "New York", "NY", "USA", "10001");

            // Act & Assert
            address.Equals(null).Should().BeFalse();
        }

        [Fact]
        public void Equals_ShouldReturnFalse_WhenComparedWithDifferentType()
        {
            // Arrange
            var address = new Address("123 Main St", "New York", "NY", "USA", "10001");
            var otherObject = "Not an address";

            // Act & Assert
            address.Equals(otherObject).Should().BeFalse();
        }

        [Theory]
        [InlineData("123 Main St", "New York", "NY", "USA", "10001", "456 Elm St", "New York", "NY", "USA", "10001", false)]
        [InlineData("123 Main St", "New York", "NY", "USA", "10001", "123 Main St", "Boston", "NY", "USA", "10001", false)]
        [InlineData("123 Main St", "New York", "NY", "USA", "10001", "123 Main St", "New York", "MA", "USA", "10001", false)]
        [InlineData("123 Main St", "New York", "NY", "USA", "10001", "123 Main St", "New York", "NY", "Canada", "10001", false)]
        [InlineData("123 Main St", "New York", "NY", "USA", "10001", "123 Main St", "New York", "NY", "USA", "02108", false)]
        [InlineData("123 Main St", "New York", "NY", "USA", "10001", "123 Main St", "New York", "NY", "USA", "10001", true)]
        public void Equals_ShouldCompareAllProperties(
            string street1, string city1, string state1, string country1, string zipCode1,
            string street2, string city2, string state2, string country2, string zipCode2,
            bool expectedResult)
        {
            // Arrange
            var address1 = new Address(street1, city1, state1, country1, zipCode1);
            var address2 = new Address(street2, city2, state2, country2, zipCode2);

            // Act & Assert
            address1.Equals(address2).Should().Be(expectedResult);
        }
    }
}
