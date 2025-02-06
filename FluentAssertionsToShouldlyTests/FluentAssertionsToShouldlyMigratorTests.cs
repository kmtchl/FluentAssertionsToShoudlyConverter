using NUnit.Framework;
using System.Text.RegularExpressions;
using FluentAssertionsToShouldly;
using Assert = NUnit.Framework.Assert;

namespace FluentAssertionsToShouldlyMigrator.Tests
{
    [TestFixture]
    public class FluentAssertionsToShouldlyMigratorTests
    {
        private FluentAssertionsToShouldly.FluentAssertionsToShouldlyMigrator _migrator;

        [SetUp]
        public void Setup()
        {
            _migrator = new FluentAssertionsToShouldly.FluentAssertionsToShouldlyMigrator();
        }

        private string NormalizeString(string input)
        {
            // Remove whitespace and normalize line endings
            return Regex.Replace(input, @"\s+", "");
        }

        [Test]
        public void ShouldConvertBeWithValue()
        {
            // Arrange
            var input = @"result.Count.Should().Be(2);";
            var expected = @"result.Count.ShouldBe(2);";

            // Act
            var result = ApplyConversion(input);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldConvertEmptyBeEquivalentTo()
        {
            // Arrange
            var input = @"result[0].DeliveryOptionTypeIds.Should().BeEquivalentTo();";
            var expected = @"result[0].DeliveryOptionTypeIds.ShouldBeEquivalentTo();";

            // Act
            var result = ApplyConversion(input);

            // Assert
            Assert.That(NormalizeString(result), Is.EqualTo(NormalizeString(expected)));
        }

        [Test]
        public void ShouldConvertDictionaryWithStrictOrdering()
        {
            // Arrange
            var input = @"result[0].DeliveryOptionTypeOverrideIds.Should().BeEquivalentTo(new Dictionary<int, IEnumerable<int>> { { 0, new[] { 5 } } }, options => options.WithStrictOrdering());";
            var expected = @"result[0].DeliveryOptionTypeOverrideIds.ShouldBe(new Dictionary<int, IEnumerable<int>> { { 0, new[] { 5 } } });";

            // Act
            var result = FluentAssertionsToShouldly.FluentAssertionsToShouldlyMigrator.TestConversion(input);
            Console.WriteLine($"Input: {input}");
            Console.WriteLine($"Result: {result}");
            Console.WriteLine($"Expected: {expected}");

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldConvertArrayWithStrictOrdering()
        {
            // Arrange
            var input = @"result[0].DeliveryOptionTypeIds.Should().BeEquivalentTo(new[] { 1, 2 }, options => options.WithStrictOrdering());";
            var expected = @"result[0].DeliveryOptionTypeIds.ShouldBe(new[] { 1, 2 });";

            // Act
            var result = ApplyConversion(input);

            // Assert
            Assert.That(NormalizeString(result), Is.EqualTo(NormalizeString(expected)));
        }

        [Test]
        public void ShouldConvertMultipleAssertionsInSingleMethod()
        {
            // Arrange
            var input = @"
            [Test]
            public async Task Execute_ShouldReturnDeliverySourcesBasedOnPostCodeAndDeliveryOptionExclusions()
            {
                excludedDeliveryOptions.AddRange(new[] { 1, 4, 6 });

                var result = (await postalCodeFilter.Apply(address, deliverySources)).ToList();

                result.Count.Should().Be(2);

                result[0].DeliveryOptionTypeIds.Should().BeEquivalentTo(new[] { 2 }, options => options.WithStrictOrdering());
                result[0].DeliveryOptionTypeOverrideIds.Should()
                    .BeEquivalentTo(new Dictionary<int, IEnumerable<int>> { { 0, new[] { 5 } } }, options => options.WithStrictOrdering());

                result[1].DeliveryOptionTypeIds.Should().BeEquivalentTo(new[] { 3 }, options => options.WithStrictOrdering());
                result[1].DeliveryOptionTypeOverrideIds.Should()
                    .BeEquivalentTo(new Dictionary<int, IEnumerable<int>> { { 0, new[] { 7 } } }, options => options.WithStrictOrdering());
            }";

            var expected = @"
            [Test]
            public async Task Execute_ShouldReturnDeliverySourcesBasedOnPostCodeAndDeliveryOptionExclusions()
            {
                excludedDeliveryOptions.AddRange(new[] { 1, 4, 6 });

                var result = (await postalCodeFilter.Apply(address, deliverySources)).ToList();

                result.Count.ShouldBe(2);

                result[0].DeliveryOptionTypeIds.ShouldBe(new[] { 2 });
                result[0].DeliveryOptionTypeOverrideIds.ShouldBe(new Dictionary<int, IEnumerable<int>> { { 0, new[] { 5 } } });

                result[1].DeliveryOptionTypeIds.ShouldBe(new[] { 3 });
                result[1].DeliveryOptionTypeOverrideIds.ShouldBe(new Dictionary<int, IEnumerable<int>> { { 0, new[] { 7 } } });
            }";

            // Act
            var result = ApplyConversion(input);

            // Assert
            Assert.That(NormalizeString(result), Is.EqualTo(NormalizeString(expected)));
        }

        private string ApplyConversion(string input)
        {
            var currentContent = input;
            var madeChanges = true;

            while (madeChanges)
            {
                madeChanges = false;
                foreach (var replacement in FluentAssertionsToShouldly.FluentAssertionsToShouldlyMigrator.AssertionReplacements)
                {
                    var newContent = Regex.Replace(currentContent, replacement.Key, replacement.Value, RegexOptions.Multiline);
                    if (newContent != currentContent)
                    {
                        currentContent = newContent;
                        madeChanges = true;
                    }
                }
            }

            return currentContent;
        }
    }
}