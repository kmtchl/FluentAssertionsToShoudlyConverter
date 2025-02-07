using System.Text.RegularExpressions;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace FluentAssertionsToShouldlyConverterTests
{
    [TestFixture]
    public class FluentAssertionsToShouldlyConverterTests
    {
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
            var input = @"items[0].Values.Should().BeEquivalentTo();";
            var expected = @"items[0].Values.ShouldBeEquivalentTo();";

            // Act
            var result = ApplyConversion(input);

            // Assert
            Assert.That(NormalizeString(result), Is.EqualTo(NormalizeString(expected)));
        }

        [Test]
        public void ShouldConvertDictionaryWithStrictOrdering()
        {
            // Arrange
            var input = @"result.Data.Should().BeEquivalentTo(new Dictionary<int, IEnumerable<int>> { { 0, new[] { 5 } } }, options => options.WithStrictOrdering());";
            var expected = @"result.Data.ShouldBe(new Dictionary<int, IEnumerable<int>> { { 0, new[] { 5 } } });";

            // Act
            var result = ApplyConversion(input);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldConvertArrayWithStrictOrdering()
        {
            // Arrange
            var input = @"items[0].Values.Should().BeEquivalentTo(new[] { 1, 2 }, options => options.WithStrictOrdering());";
            var expected = @"items[0].Values.ShouldBe(new[] { 1, 2 });";

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
                public async Task Execute_ShouldReturnExpectedResults()
                {
                    excludedItems.AddRange(new[] { 1, 4, 6 });

                    var result = (await dataFilter.Apply(request, items)).ToList();

                    result.Count.Should().Be(2);

                    result[0].Values.Should().BeEquivalentTo(new[] { 2 }, options => options.WithStrictOrdering());
                    result[0].Mappings.Should()
                        .BeEquivalentTo(new Dictionary<int, IEnumerable<int>> { { 0, new[] { 5 } } }, options => options.WithStrictOrdering());

                    result[1].Values.Should().BeEquivalentTo(new[] { 3 }, options => options.WithStrictOrdering());
                    result[1].Mappings.Should()
                        .BeEquivalentTo(new Dictionary<int, IEnumerable<int>> { { 0, new[] { 7 } } }, options => options.WithStrictOrdering());
                }";

            var expected = @"
                [Test]
                public async Task Execute_ShouldReturnExpectedResults()
                {
                    excludedItems.AddRange(new[] { 1, 4, 6 });

                    var result = (await dataFilter.Apply(request, items)).ToList();

                    result.Count.ShouldBe(2);

                    result[0].Values.ShouldBe(new[] { 2 });
                    result[0].Mappings.ShouldBe(new Dictionary<int, IEnumerable<int>> { { 0, new[] { 5 } } });

                    result[1].Values.ShouldBe(new[] { 3 });
                    result[1].Mappings.ShouldBe(new Dictionary<int, IEnumerable<int>> { { 0, new[] { 7 } } });
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
                action.ShouldThrow<Exception>();
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

        [Test]
        public void ShouldConvertBeWithValue_SingleAndMultiLine()
        {
            // Arrange
            var input = @"
                result.Count.Should().Be(2);
                result.Count
                    .Should()
                    .Be(2);
            ";
            var expected = @"
                result.Count.ShouldBe(2);
                result.Count
                    .ShouldBe(2);
            ";

            // Act
            var result = ApplyConversion(input);

            // Assert
            Assert.That(NormalizeString(result), Is.EqualTo(NormalizeString(expected)));
        }

        [Test]
        public void ShouldConvertDictionaryWithStrictOrdering_SingleAndMultiLine()
        {
            // Arrange
            var input = @"
                result.Data.Should().BeEquivalentTo(new Dictionary<int, IEnumerable<int>> { { 0, new[] { 5 } } }, options => options.WithStrictOrdering());
                result.Data
                    .Should()
                    .BeEquivalentTo(
                        new Dictionary<int, IEnumerable<int>> { { 0, new[] { 5 } } },
                        options => 
                            options.WithStrictOrdering()
                    );
            ";
            var expected = @"
                result.Data.ShouldBe(new Dictionary<int, IEnumerable<int>> { { 0, new[] { 5 } } });
                result.Data
                    .ShouldBe(new Dictionary<int, IEnumerable<int>> { { 0, new[] { 5 } } });
            ";

            // Act
            var result = ApplyConversion(input);

            // Assert
            Assert.That(NormalizeString(result), Is.EqualTo(NormalizeString(expected)));
        }

        [Test]
        public void ShouldConvertExceptionAssertions_SingleAndMultiLine()
        {
            // Arrange
            var input = @"
                action.Should().ThrowExactly<ArgumentException>();
                action
                    .Should()
                    .ThrowExactly<ArgumentException>();

                asyncAction.Should().ThrowAsync<InvalidOperationException>();
                asyncAction
                    .Should()
                    .ThrowAsync<InvalidOperationException>();

                action.Should().Throw<Exception>();
                action
                    .Should()
                    .Throw<Exception>();

                action.Should().NotThrow();
                action
                    .Should()
                    .NotThrow();
            ";
            var expected = @"
                action.ShouldThrowExactly<ArgumentException>();
                action
                    .ShouldThrowExactly<ArgumentException>();

                asyncAction.ShouldThrowAsync<InvalidOperationException>();
                asyncAction
                    .ShouldThrowAsync<InvalidOperationException>();

                action.ShouldThrow<Exception>();
                action
                    .ShouldThrow<Exception>();

                action.ShouldNotThrow();
                action
                    .ShouldNotThrow();
            ";

            // Act
            var result = ApplyConversion(input);

            // Assert
            Assert.That(NormalizeString(result), Is.EqualTo(NormalizeString(expected)));
        }

        [Test]
        public void ShouldConvertCollectionAssertions_SingleAndMultiLine()
        {
            // Arrange
            var input = @"
                list.Should().HaveCount(3);
                list
                    .Should()
                    .HaveCount(3);

                list.Should().BeEmpty();
                list
                    .Should()
                    .BeEmpty();

                list.Should().Contain(item);
                list
                    .Should()
                    .Contain(item);
            ";
            var expected = @"
                list.Count().ShouldBe(3);
                list
                    .Count().ShouldBe(3);

                list.ShouldBeEmpty();
                list
                    .ShouldBeEmpty();

                list.ShouldContain(item);
                list
                    .ShouldContain(item);
            ";

            // Act
            var result = ApplyConversion(input);

            // Assert
            Assert.That(NormalizeString(result), Is.EqualTo(NormalizeString(expected)));
        }

        [Test]
        public void ShouldConvertBooleanAssertions_SingleAndMultiLine()
        {
            // Arrange
            var input = @"
                condition.Should().BeTrue();
                condition
                    .Should()
                    .BeTrue();

                condition.Should().BeFalse();
                condition
                    .Should()
                    .BeFalse();
            ";
            var expected = @"
                condition.ShouldBeTrue();
                condition
                    .ShouldBeTrue();

                condition.ShouldBeFalse();
                condition
                    .ShouldBeFalse();
            ";

            // Act
            var result = ApplyConversion(input);

            // Assert
            Assert.That(NormalizeString(result), Is.EqualTo(NormalizeString(expected)));
        }

        [Test]
        public void ShouldConvertComplexArrayWithNestedDictionariesAndStrictOrdering()
        {
            // Arrange
            var input = @"
                var result = await query.GetData(new Criteria());

                result.Items[""A""].Should().BeEquivalentTo(new[]
                {
                    new ItemDetails(""ID1"", 0, ""CODE1"", ""Name1""),
                    new ItemDetails(""ID2"", 1, ""CODE2"", ""Name2"", new Dictionary<string, IDictionary<int, IEnumerable<int>>>
                    {
                        { string.Empty, new Dictionary<int, IEnumerable<int>> { { default, new[] { 1, 2 } } } },
                        { ""X"", new Dictionary<int, IEnumerable<int>> { { default, new[] { 4 } }, { 3, new[] { 5 } } } }
                    }, ItemType.TypeA, true),
                    new ItemDetails(""ID3"", 2, ""CODE3"", ""Name3"", ItemType.TypeB),
                    new ItemDetails(""ID4"", 2, ""CODE4"", ""Name4"", ItemType.TypeB)
                }, options => options.WithStrictOrdering());

                result.Items[""B""].Should().BeEquivalentTo(new[]
                {
                    new ItemDetails(""ID5"", 0, ""CODE5"", ""Name5""),
                    new ItemDetails(""ID6"", 1, ""CODE6"", ""Name6"", new Dictionary<string, IDictionary<int, IEnumerable<int>>>
                    {
                        { string.Empty, new Dictionary<int, IEnumerable<int>> { { default, new[] { 13 } }, { 1, new[] { 14 } } } }
                    }, ItemType.TypeA, true)
                }, options => options.WithStrictOrdering());
            ";

            var expected = @"
                var result = await query.GetData(new Criteria());

                result.Items[""A""].ShouldBe(new[]
                {
                    new ItemDetails(""ID1"", 0, ""CODE1"", ""Name1""),
                    new ItemDetails(""ID2"", 1, ""CODE2"", ""Name2"", new Dictionary<string, IDictionary<int, IEnumerable<int>>>
                    {
                        { string.Empty, new Dictionary<int, IEnumerable<int>> { { default, new[] { 1, 2 } } } },
                        { ""X"", new Dictionary<int, IEnumerable<int>> { { default, new[] { 4 } }, { 3, new[] { 5 } } } }
                    }, ItemType.TypeA, true),
                    new ItemDetails(""ID3"", 2, ""CODE3"", ""Name3"", ItemType.TypeB),
                    new ItemDetails(""ID4"", 2, ""CODE4"", ""Name4"", ItemType.TypeB)
                });

                result.Items[""B""].ShouldBe(new[]
                {
                    new ItemDetails(""ID5"", 0, ""CODE5"", ""Name5""),
                    new ItemDetails(""ID6"", 1, ""CODE6"", ""Name6"", new Dictionary<string, IDictionary<int, IEnumerable<int>>>
                    {
                        { string.Empty, new Dictionary<int, IEnumerable<int>> { { default, new[] { 13 } }, { 1, new[] { 14 } } } }
                    }, ItemType.TypeA, true)
                });
            ";

            // Act
            var result = ApplyConversion(input);

            // Assert
            Assert.That(NormalizeString(result), Is.EqualTo(NormalizeString(expected)));
        }

        [Test]
        public void ShouldConvertSimpleStringArrayWithoutStrictOrdering()
        {
            // Arrange
            var input = @"model.Include.Should().BeEquivalentTo(new[] { ""dts"", ""dth"" });";
            var expected = @"model.Include.ShouldBeEquivalentTo(new[] { ""dts"", ""dth"" });";

            // Act
            var result = ApplyConversion(input);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldConvertExceptionWithMessageAssertion()
        {
            // Arrange
            var input = @"
                featureToggleConfiguration
                    .Invoking(x => x[featureName])
                    .Should()
                    .Throw<FeatureToggleNotSupportedException>()
                    .WithMessage($""*'{featureName}'*"");";

            var expected = @"
                var exception = Should.Throw<FeatureToggleNotSupportedException>(
                    () => featureToggleConfiguration[featureName]);
                exception.Message.ShouldContain($""*'{featureName}'*"");";

            // Act
            var result = ApplyConversion(input);

            // Debug
            Console.WriteLine("Input:");
            Console.WriteLine(input);
            Console.WriteLine("\nExpected:");
            Console.WriteLine(expected);
            Console.WriteLine("\nActual:");
            Console.WriteLine(result);
            Console.WriteLine("\nNormalized Expected:");
            Console.WriteLine(NormalizeString(expected));
            Console.WriteLine("\nNormalized Actual:");
            Console.WriteLine(NormalizeString(result));

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
                foreach (var replacement in FluentAssertionsToShouldly.Converter.AssertionReplacements)
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