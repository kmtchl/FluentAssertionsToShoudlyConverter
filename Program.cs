using System.Text.RegularExpressions;

namespace FluentAssertionsToShoudlyConverter
{
    class Program
    {
        private static readonly Dictionary<string, string> AssertionMappings = new()
        {
            // Special cases first - order is important as more specific patterns need to match first
            { ".Should().HaveCount(", ".Count().ShouldBe(" },
            { "HaveCount(", "Count().ShouldBe(" },
            { ".Should().NotContain(", ".ShouldNotContain(" },
            { ".Should().Contain(", ".ShouldContain(" },
            { "Contain(", "ShouldContain(" },

            { ".Should().Be(", ".ShouldBe(" },
            { ".Should().NotBe(", ".ShouldNotBe(" },
            { ".Should().BeNull()", ".ShouldBeNull()" },
            { ".Should().BeNullOrEmpty()", ".ShouldBeEmpty()" },
            { ".Should().NotBeNull()", ".ShouldNotBeNull()" },
            { ".Should().BeTrue()", ".ShouldBeTrue()" },
            { ".Should().BeFalse()", ".ShouldBeFalse()" },
            { ".Should().BeOfType<", ".ShouldBeOfType<" },
            { ".Should().NotBeOfType<", ".ShouldNotBeOfType<" },
            { ".Should().BeEquivalentTo(", ".ShouldBeEquivalentTo(" },
            { ".Should().NotBeEquivalentTo(", ".ShouldNotBeEquivalentTo(" },
            { ".Should().BeSameAs(", ".ShouldBeSameAs(" },
            { ".Should().NotBeSameAs(", ".ShouldNotBeSameAs(" },
            { ".Should().BeEmpty()", ".ShouldBeEmpty()" },
            { ".Should().NotBeEmpty()", ".ShouldNotBeEmpty()" },
            { ".Should().OnlyHaveUniqueItems()", ".ShouldAllBeUnique()" },
            { ".Should().ContainInOrder(", ".ShouldBe(" },
            { ".Should().ContainSingle()", ".ShouldContainSingle()" },
            { ".Should().BeSubsetOf(", ".ShouldBeSubsetOf(" },
            { ".Should().NotBeSubsetOf(", ".ShouldNotBeSubsetOf(" },
            { ".Should().IntersectWith(", ".ShouldIntersectWith(" },
            { ".Should().StartWith(", ".ShouldStartWith(" },
            { ".Should().EndWith(", ".ShouldEndWith(" },
            { ".Should().Throw<", ".ShouldThrow<" },
            { ".Should().NotThrow()", ".ShouldNotThrow()" },
            { ".Should().ThrowExactly<", ".ShouldThrowExactly<" },
            { ".Should().BeGreaterThan(", ".ShouldBeGreaterThan(" },
            { ".Should().BeLessThan(", ".ShouldBeLessThan(" },
            { ".Should().BeGreaterThanOrEqualTo(", ".ShouldBeGreaterThanOrEqualTo(" },
            { ".Should().BeLessThanOrEqualTo(", ".ShouldBeLessThanOrEqualTo(" },
            { ".Should().BeInRange(", ".ShouldBeInRange(" },
            { ".Should().NotBeInRange(", ".ShouldNotBeInRange(" },
            { ".Should().CompleteWithin(", ".ShouldCompleteIn(" },
            { ".Should().NotCompleteWithin(", ".ShouldNotCompleteIn(" }
        };

        static void Main(string?[] args)
        {
            try
            {
                string? directoryPath = args.Length > 0 ? args[0] : PromptForDirectoryPath();

                if (string.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(directoryPath))
                {
                    Console.WriteLine("Invalid directory path.");
                    return;
                }

                Console.WriteLine($"Converting Fluent Assertions to Shouldly in {directoryPath}...");
                var files = Directory.GetFiles(directoryPath, "*.cs", SearchOption.AllDirectories);

                int convertedFiles = files.Count(ConvertFile);

                Console.WriteLine($"Conversion complete. Converted {convertedFiles} of {files.Length} files.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        private static string? PromptForDirectoryPath()
        {
            Console.WriteLine("Enter the directory containing the files to convert:");
            return Console.ReadLine()?.Trim();
        }

        private static bool ConvertFile(string filePath)
        {
            try
            {
                var content = File.ReadAllText(filePath);
                if (!content.Contains("FluentAssertions") && !content.Contains(".Should()"))
                {
                    return false;
                }

                List<string?> lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
                var result = new List<string?>();
                bool hasChanges = false;
                bool isTestFile = false;

                for (int i = 0; i < lines.Count; i++)
                {
                    var line = lines[i];
                    var trimmedLine = line.TrimEnd();

                    if (line.Contains("[Test]") || line.Contains("[Fact]") || line.Contains("[Theory]"))
                    {
                        isTestFile = true;
                    }

                    if (trimmedLine.Contains(".Should()"))
                    {
                        if (IsStartOfChain(lines, i))
                        {
                            var (convertedLines, newIndex) = ProcessChainedAssertion(lines, i);
                            result.AddRange(convertedLines);
                            i = newIndex;
                            hasChanges = true;
                            continue;
                        }

                        result.Add(ConvertSingleAssertion(line));
                        hasChanges = true;
                        continue;
                    }

                    result.Add(line);
                }

                if (hasChanges && isTestFile)
                {
                    AddAndRemoveImports(result);

                    while (result.Count > 1 && string.IsNullOrWhiteSpace(result[^1]))
                    {
                        result.RemoveAt(result.Count - 1);
                    }

                    File.WriteAllLines(filePath, result!);
                    Console.WriteLine($"Converted: {filePath}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing file {filePath}: {ex.Message}");
            }

            return false;
        }

        private static bool IsStartOfChain(List<string> lines, int currentIndex)
        {
            if (currentIndex + 1 >= lines.Count) return false;

            var nextLine = lines[currentIndex + 1].Trim();
            return nextLine.StartsWith(".And");
        }

        private static (List<string?>, int) ProcessChainedAssertion(List<string> lines, int startIndex)
        {
            var result = new List<string?>();
            var indentation = new string(' ', lines[startIndex].TakeWhile(c => c == ' ').Count());

            var firstLine = lines[startIndex];
            var subject = firstLine.Substring(0, firstLine.IndexOf(".Should()", StringComparison.Ordinal)).Trim();

            var firstAssertion = ConvertSingleAssertion(firstLine);
            result.Add(firstAssertion);

            int currentIndex = startIndex + 1;

            while (currentIndex < lines.Count)
            {
                var line = lines[currentIndex].Trim();

                if (string.IsNullOrWhiteSpace(line))
                {
                    currentIndex++;
                    continue;
                }

                if (!line.StartsWith(".And"))
                {
                    break;
                }

                var assertion = line.Substring(4).TrimStart();

                if (assertion.StartsWith("."))
                {
                    assertion = assertion.Substring(1);
                }

                var convertedLine = $"{indentation}{subject}.{assertion}";
                result.Add(ConvertSingleAssertion(convertedLine));

                currentIndex++;
            }

            return (result, currentIndex - 1);
        }

        private static string ConvertSingleAssertion(string line)
        {
            string result = line;
            var indentation = new string(' ', result.TakeWhile(c => c == ' ').Count());

            // Handle arrays
            if (result.Contains("Contain(new") && result.Contains("[]"))
            {
                var match = Regex.Match(result, @"Contain\(new\s+\w+\[\]\s*{\s*(.*?)\s*}\)");
                if (match.Success)
                {
                    var items = match.Groups[1].Value.Split(',').Select(x => x.Trim());
                    var subject = result.Trim().Substring(0, result.Trim().IndexOf(".", StringComparison.Ordinal));
                    var assertions = items.Select(item => $"{subject}.ShouldContain({item});");

                    return string.Join(Environment.NewLine, assertions.Select(a => indentation + a));
                }
            }

            foreach (var mapping in AssertionMappings.Where(mapping => result.Contains(mapping.Key)))
            {
                result = result.Replace(mapping.Key, mapping.Value);
                break;
            }

            if (!result.Contains("=>") && !result.TrimEnd().EndsWith(";") && !result.TrimEnd().EndsWith(".And"))
            {
                result = result.TrimEnd() + ";";
            }

            result = result.Replace("..", ".");

            if (!result.StartsWith(indentation))
            {
                result = indentation + result.TrimStart();
            }

            return result;
        }

        private static void AddAndRemoveImports(List<string?> lines)
        {
            bool shouldlyAdded = false;
            int lastUsingIndex = -1;
            int firstUsingIndex = -1;

            for (int i = 0; i < lines.Count; i++)
            {
                var trimmedLine = lines[i]?.Trim();
                if (trimmedLine == null || !trimmedLine.StartsWith("using ")) continue;
        
                if (firstUsingIndex == -1) firstUsingIndex = i;
                lastUsingIndex = i;

                if (trimmedLine == "using FluentAssertions;")
                {
                    lines[i] = null;
                }
                else if (trimmedLine == "using Shouldly;")
                {
                    shouldlyAdded = true;
                    break;
                }
            }
            
            if (!shouldlyAdded && lastUsingIndex >= 0)
            {
                lines.Insert(lastUsingIndex + 1, "using Shouldly;");
            }

            lines.RemoveAll(line => line == null);
        }
    }
}
