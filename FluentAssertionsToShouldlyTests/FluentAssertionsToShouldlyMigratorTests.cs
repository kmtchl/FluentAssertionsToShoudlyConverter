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

        [Test]
        public void ShouldConvertNotBeEquivalentTo()
        {
            // Arrange
            var input = @"items.Should().NotBeEquivalentTo(expectedItems);";
            var expected = @"items.ShouldNotBeEquivalentTo(expectedItems);";

            // Act
            var result = ApplyConversion(input);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldConvertContainInOrder()
        {
            // Arrange
            var input = @"sequence.Should().ContainInOrder(new[] { 1, 2, 3 });";
            var expected = @"sequence.ShouldBe(new[] { 1, 2, 3 });";

            // Act
            var result = ApplyConversion(input);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldConvertContainSingle()
        {
            // Arrange
            var input = @"collection.Should().ContainSingle();";
            var expected = @"collection.ShouldHaveSingleItem();";

            // Act
            var result = ApplyConversion(input);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldConvertOnlyHaveUniqueItems()
        {
            // Arrange
            var input = @"numbers.Should().OnlyHaveUniqueItems();";
            var expected = @"numbers.ShouldAllBeUnique();";

            // Act
            var result = ApplyConversion(input);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldConvertBeNullOrEmpty()
        {
            // Arrange
            var input = @"text.Should().BeNullOrEmpty();";
            var expected = @"text.ShouldBeNullOrEmpty();";

            // Act
            var result = ApplyConversion(input);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldConvertNotBeNullOrWhiteSpace()
        {
            // Arrange
            var input = @"text.Should().NotBeNullOrWhiteSpace();";
            var expected = @"text.ShouldNotBeNullOrWhiteSpace();";

            // Act
            var result = ApplyConversion(input);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldConvertBeOfType()
        {
            // Arrange
            var input = @"obj.Should().BeOfType<string>();";
            var expected = @"obj.ShouldBeOfType<string>();";

            // Act
            var result = ApplyConversion(input);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldConvertNotBeOfType()
        {
            // Arrange
            var input = @"obj.Should().NotBeOfType<int>();";
            var expected = @"obj.ShouldNotBeOfType<int>();";

            // Act
            var result = ApplyConversion(input);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldConvertBeSameAs()
        {
            // Arrange
            var input = @"instance1.Should().BeSameAs(instance2);";
            var expected = @"instance1.ShouldBeSameAs(instance2);";

            // Act
            var result = ApplyConversion(input);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldConvertBeSubsetOf()
        {
            // Arrange
            var input = @"subset.Should().BeSubsetOf(fullSet);";
            var expected = @"subset.ShouldBeSubsetOf(fullSet);";

            // Act
            var result = ApplyConversion(input);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldConvertIntersectWith()
        {
            // Arrange
            var input = @"set1.Should().IntersectWith(set2);";
            var expected = @"set1.ShouldIntersectWith(set2);";

            // Act
            var result = ApplyConversion(input);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldConvertNumericComparisons()
        {
            // Arrange
            var input = @"
                value.Should().BeGreaterThan(10);
                value.Should().BeLessThan(20);
                value.Should().BeGreaterThanOrEqualTo(15);
                value.Should().BeLessThanOrEqualTo(18);
            ";
            var expected = @"
                value.ShouldBeGreaterThan(10);
                value.ShouldBeLessThan(20);
                value.ShouldBeGreaterThanOrEqualTo(15);
                value.ShouldBeLessThanOrEqualTo(18);
            ";

            // Act
            var result = ApplyConversion(input);

            // Assert
            Assert.That(NormalizeString(result), Is.EqualTo(NormalizeString(expected)));
        }

        [Test]
        public void ShouldConvertRangeChecks()
        {
            // Arrange
            var input = @"
                number.Should().BeInRange(1, 10);
                number.Should().NotBeInRange(20, 30);
            ";
            var expected = @"
                number.ShouldBeInRange(1, 10);
                number.ShouldNotBeInRange(20, 30);
            ";

            // Act
            var result = ApplyConversion(input);

            // Assert
            Assert.That(NormalizeString(result), Is.EqualTo(NormalizeString(expected)));
        }

        [Test]
        public void ShouldConvertCollectionAssertions()
        {
            // Arrange
            var input = @"
                list.Should().HaveCount(3);
                list.Should().BeEmpty();
                list.Should().NotBeEmpty();
                list.Should().Contain(item);
                list.Should().NotContain(item);
            ";
            var expected = @"
                list.Count().ShouldBe(3);
                list.ShouldBeEmpty();
                list.ShouldNotBeEmpty();
                list.ShouldContain(item);
                list.ShouldNotContain(item);
            ";

            // Act
            var result = ApplyConversion(input);

            // Assert
            Assert.That(NormalizeString(result), Is.EqualTo(NormalizeString(expected)));
        }

        [Test]
        public void ShouldConvertStringAssertions()
        {
            // Arrange
            var input = @"
                text.Should().StartWith(""Hello"");
                text.Should().EndWith(""World"");
            ";
            var expected = @"
                text.ShouldStartWith(""Hello"");
                text.ShouldEndWith(""World"");
            ";

            // Act
            var result = ApplyConversion(input);

            // Assert
            Assert.That(NormalizeString(result), Is.EqualTo(NormalizeString(expected)));
        }

        [Test]
        public void ShouldConvertExceptionAssertions()
        {
            // Arrange
            var input = @"
                action.Should().ThrowExactly<ArgumentException>();
                asyncAction.Should().ThrowAsync<InvalidOperationException>();
                action.Should().Throw<Exception>();
                action.Should().NotThrow();
            ";
            var expected = @"
                action.ShouldThrowExactly<ArgumentException>();
                asyncAction.ShouldThrowAsync<InvalidOperationException>();
                action.ShouldThrow<Exception>(() => );
                action.ShouldNotThrow();
            ";

            // Act
            var result = ApplyConversion(input);

            // Assert
            Assert.That(NormalizeString(result), Is.EqualTo(NormalizeString(expected)));
        }

        [Test]
        public void ShouldConvertBooleanAssertions()
        {
            // Arrange
            var input = @"
                condition.Should().BeTrue();
                condition.Should().BeFalse();
            ";
            var expected = @"
                condition.ShouldBeTrue();
                condition.ShouldBeFalse();
            ";

            // Act
            var result = ApplyConversion(input);

            // Assert
            Assert.That(NormalizeString(result), Is.EqualTo(NormalizeString(expected)));
        }

        [Test]
        public void ShouldConvertNullAssertions()
        {
            // Arrange
            var input = @"
                obj.Should().BeNull();
                obj.Should().NotBeNull();
            ";
            var expected = @"
                obj.ShouldBeNull();
                obj.ShouldNotBeNull();
            ";

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