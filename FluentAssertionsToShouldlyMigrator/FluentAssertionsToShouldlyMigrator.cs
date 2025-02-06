using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace FluentAssertionsToShouldly
{
    public class FluentAssertionsToShouldlyMigrator
    {
        public static readonly Dictionary<string, string> AssertionReplacements = new()
        {
            { @"using FluentAssertions;", "using Shouldly;" },
            
            // Handle Dictionary with strict ordering - most specific first
            { @"\.Should\(\)\s*\.BeEquivalentTo\((new Dictionary<[^>]+>[^;]+?),\s*options\s*=>\s*options\.WithStrictOrdering\(\)\)", ".ShouldBe($1)" },
            
            // Handle Dictionary without strict ordering
            { @"(.*?)\.Should\(\)\.BeEquivalentTo\((new Dictionary<[^>]+>\s*\{[^}]+\})\)", "$1.ShouldBeEquivalentTo($2)" },
            
            // Handle array with strict ordering
            { @"\.Should\(\)\.BeEquivalentTo\((new\[\][^\)]+?)\s*,\s*options\s*=>\s*options\.WithStrictOrdering\(\)\)", ".ShouldBe($1)" },
            { @"\.Should\(\)\.BeEquivalentTo\((\[[^\]]+\])\s*,\s*options\s*=>\s*options\.WithStrictOrdering\(\)\)", ".ShouldBe($1)" },
            
            // Handle variable with strict ordering
            { @"\.Should\(\)\.BeEquivalentTo\(([^,\)]+?)\s*,\s*options\s*=>\s*options\.WithStrictOrdering\(\)\)", ".ShouldBe($1)" },
            
            // Handle collection expressions
            { @"\.Should\(\)\.BeEquivalentTo\((\[(?:[^\[\]]|\[(?:[^\[\]]|\[[^\[\]]*\])*\])*\])\)", ".ShouldBeEquivalentTo($1)" },
            
            // Handle empty BeEquivalentTo calls
            { @"\.Should\(\)\.BeEquivalentTo\(\)", ".ShouldBeEquivalentTo()" },
            
            // Generic BeEquivalentTo pattern
            { @"\.Should\(\)\.BeEquivalentTo\(([^,\)]+)\)", ".ShouldBeEquivalentTo($1)" },
            
            // Rest of the patterns
            { @"\.Should\(\)\.NotBeEquivalentTo\(", ".ShouldNotBeEquivalentTo(" },
            { @"\.Should\(\)\.ContainInOrder\(", ".ShouldBe(" },
            { @"\.Should\(\)\.ContainSingle\(\)", ".ShouldHaveSingleItem()" },
            { @"\.Should\(\)\.OnlyHaveUniqueItems\(\)", ".ShouldAllBeUnique()" },
            { @"\.Should\(\)\.BeNullOrEmpty\(\)", ".ShouldBeNullOrEmpty()" },
            { @"\.Should\(\)\.NotBeNullOrWhiteSpace\(\)", ".ShouldNotBeNullOrWhiteSpace()" },
            { @"\.Should\(\)\.NotBeOfType<", ".ShouldNotBeOfType<" },
            { @"\.Should\(\)\.BeOfType<([^>]+)>\(\)", ".ShouldBeOfType<$1>()" },
            { @"\.Should\(\)\.BeSameAs\(", ".ShouldBeSameAs(" },
            { @"\.Should\(\)\.NotBeSameAs\(", ".ShouldNotBeSameAs(" },
            { @"\.Should\(\)\.BeSubsetOf\(", ".ShouldBeSubsetOf(" },
            { @"\.Should\(\)\.NotBeSubsetOf\(", ".ShouldNotBeSubsetOf(" },
            { @"\.Should\(\)\.IntersectWith\(", ".ShouldIntersectWith(" },
            { @"\.Should\(\)\.BeGreaterThanOrEqualTo\(", ".ShouldBeGreaterThanOrEqualTo(" },
            { @"\.Should\(\)\.BeLessThanOrEqualTo\(", ".ShouldBeLessThanOrEqualTo(" },
            { @"\.Should\(\)\.BeGreaterThan\(", ".ShouldBeGreaterThan(" },
            { @"\.Should\(\)\.BeLessThan\(", ".ShouldBeLessThan(" },
            { @"\.Should\(\)\.BeInRange\(", ".ShouldBeInRange(" },
            { @"\.Should\(\)\.NotBeInRange\(", ".ShouldNotBeInRange(" },
            { @"\.Should\(\)\.CompleteWithin\(", ".ShouldCompleteIn(" },
            { @"\.Should\(\)\.NotCompleteWithin\(", ".ShouldNotCompleteIn(" },
            { @"\.Should\(\)\.HaveCount\(", ".Count().ShouldBe(" },
            { @"\.Should\(\)\.StartWith\(", ".ShouldStartWith(" },
            { @"\.Should\(\)\.EndWith\(", ".ShouldEndWith(" },
            { @"\.Should\(\)\.ThrowExactly<", ".ShouldThrowExactly<" },
            { @"\.Should\(\)\.ThrowAsync<", "ShouldThrowAsync<" },
            { @"\.Should\(\)\.Throw<([^>]+)>\(\)", "ShouldThrow<$1>(() => " },
            { @"\.Should\(\)\.NotThrow\(\)", ".ShouldNotThrow()" },
            { @"\.Should\(\)\.NotContain\(", ".ShouldNotContain(" },
            { @"\.Should\(\)\.Contain\(", ".ShouldContain(" },
            { @"\.Should\(\)\.BeEmpty\(\)", ".ShouldBeEmpty()" },
            { @"\.Should\(\)\.NotBeEmpty\(\)", ".ShouldNotBeEmpty()" },
            { @"\.Should\(\)\.BeNull\(\)", ".ShouldBeNull()" },
            { @"\.Should\(\)\.NotBeNull\(\)", ".ShouldNotBeNull()" },
            { @"\.Should\(\)\.BeTrue\(\)", ".ShouldBeTrue()" },
            { @"\.Should\(\)\.BeFalse\(\)", ".ShouldBeFalse()" },
            { @"\.Should\(\)\.NotBe\(", ".ShouldNotBe(" },
            // Handle Be with parameters
            { @"\.Should\(\)\.Be\(([^)]+)\)", ".ShouldBe($1)" },
            // Handle empty Be calls
            { @"\.Should\(\)\.Be\(\)", ".ShouldBeEquivalentTo()" }
        };

        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide the solution directory path.");
                return;
            }

            var solutionDir = args[0];
            var migrator = new FluentAssertionsToShouldlyMigrator();
            
            Console.WriteLine("Starting migration process...");
            
            // Update project files first
            migrator.UpdateProjectFiles(solutionDir);
            
            // Then update test files
            migrator.UpdateTestFiles(solutionDir);
            
            Console.WriteLine("Migration complete. Please review changes, restore packages, and run tests.");
        }

        public void UpdateProjectFiles(string solutionDir)
        {
            Console.WriteLine("Updating project files...");
            
            var testProjects = Directory.GetFiles(solutionDir, "*.csproj", SearchOption.AllDirectories)
                .Where(file => file.Contains("Test") || file.Contains("Tests"));

            foreach (var projectFile in testProjects)
            {
                try
                {
                    var doc = XDocument.Load(projectFile);
                    var itemGroups = doc.Root?.Elements("ItemGroup");
                    var modified = false;

                    if (itemGroups == null) continue;

                    foreach (var itemGroup in itemGroups)
                    {
                        // Remove FluentAssertions if present
                        var fluentAssertions = itemGroup.Elements("PackageReference")
                            .FirstOrDefault(x => x.Attribute("Include")?.Value == "FluentAssertions");
                        
                        if (fluentAssertions != null)
                        {
                            fluentAssertions.Remove();
                            modified = true;
                        }

                        // Add Shouldly if not present
                        var shouldly = itemGroup.Elements("PackageReference")
                            .Any(x => x.Attribute("Include")?.Value == "Shouldly");
                        
                        if (!shouldly && itemGroup.Elements("PackageReference").Any())
                        {
                            itemGroup.AddFirst(new XElement("PackageReference",
                                new XAttribute("Include", "Shouldly"),
                                new XAttribute("Version", "4.3.0")));
                            modified = true;
                            break;
                        }
                    }

                    if (modified)
                    {
                        doc.Save(projectFile);
                        Console.WriteLine($"Updated project file: {Path.GetFileName(projectFile)}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating project file {projectFile}: {ex.Message}");
                }
            }
        }

        public void UpdateTestFiles(string solutionDir)
        {
            Console.WriteLine("Updating test files...");
            
            var testFiles = Directory.GetFiles(solutionDir, "*.cs", SearchOption.AllDirectories)
                .Where(file => file.Contains("Test") || file.Contains("Tests") || file.Contains("Should"))
                .ToList();

            Console.WriteLine($"Found {testFiles.Count} test files to process.");
            var processedCount = 0;

            foreach (var file in testFiles)
            {
                try
                {
                    Console.WriteLine($"Processing file {++processedCount} of {testFiles.Count}: {Path.GetFileName(file)}");
                    
                    var content = File.ReadAllText(file);
                    var originalContent = content;

                    // Create a task to process the file with a timeout
                    var processTask = Task.Run(() =>
                    {
                        var iterations = 0;
                        var madeChanges = true;
                        var currentContent = content;

                        while (madeChanges)
                        {
                            madeChanges = false;
                            iterations++;

                            foreach (var replacement in AssertionReplacements)
                            {
                                try
                                {
                                    var newContent = Regex.Replace(currentContent, replacement.Key, replacement.Value, 
                                        RegexOptions.Multiline, TimeSpan.FromSeconds(5));
                                    
                                    if (newContent != currentContent)
                                    {
                                        currentContent = newContent;
                                        madeChanges = true;
                                    }
                                }
                                catch (RegexMatchTimeoutException)
                                {
                                    Console.WriteLine($"    Warning: Regex timeout in {Path.GetFileName(file)} for pattern: {replacement.Key.Substring(0, Math.Min(50, replacement.Key.Length))}...");
                                    continue;
                                }
                            }

                            if (iterations % 5 == 0 && madeChanges)
                            {
                                Console.WriteLine($"    Warning: File {Path.GetFileName(file)} required {iterations} iterations so far...");
                            }
                        }

                        return (currentContent, iterations);
                    });

                    // Wait for the processing to complete with a timeout
                    if (processTask.Wait(TimeSpan.FromSeconds(30)))
                    {
                        var (processedContent, iterations) = processTask.Result;
                        
                        if (processedContent != originalContent)
                        {
                            File.WriteAllText(file, processedContent);
                            Console.WriteLine($"  Updated file: {Path.GetFileName(file)} (took {iterations} iterations)");
                        }
                        else
                        {
                            Console.WriteLine($"  No changes needed for: {Path.GetFileName(file)}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"  Warning: Skipped file {Path.GetFileName(file)} due to timeout");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating test file {file}: {ex.Message}");
                }
            }
            
            Console.WriteLine($"Completed processing {processedCount} files.");
        }

        public static string TestConversion(string input)
        {
            var currentContent = input;
            var madeChanges = true;
            var iterations = 0;

            while (madeChanges && iterations < 10)
            {
                madeChanges = false;
                iterations++;

                foreach (var replacement in AssertionReplacements)
                {
                    var match = Regex.Match(currentContent, replacement.Key);
                    if (match.Success)
                    {
                        Console.WriteLine($"Pattern matched: {replacement.Key}");
                        Console.WriteLine($"Groups: {string.Join(", ", match.Groups.Cast<Group>().Select(g => g.Value))}");
                        var newContent = Regex.Replace(currentContent, replacement.Key, replacement.Value);
                        if (newContent != currentContent)
                        {
                            Console.WriteLine($"Replaced with: {newContent}");
                            currentContent = newContent;
                            madeChanges = true;
                        }
                    }
                }
            }

            return currentContent;
        }
    }
} 