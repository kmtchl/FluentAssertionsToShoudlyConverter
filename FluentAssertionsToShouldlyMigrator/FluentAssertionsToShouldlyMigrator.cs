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
            { @"\.Should\(\)(?:\s*\n*\s*|\s+)\.BeEquivalentTo\((\s*new Dictionary<[^>]+>[^;]+?),(?:\s*\n*\s*|\s+)options(?:\s*\n*\s*|\s+)=>(?:\s*\n*\s*|\s+)options\.WithStrictOrdering\(\)\s*\)", ".ShouldBe($1)" },
            
            // Handle Dictionary without strict ordering
            { @"(.*?)\.Should\(\)(?:\s*\n*\s*|\s+)\.BeEquivalentTo\((new Dictionary<[^>]+>\s*\{[^}]+\})\)", "$1.ShouldBeEquivalentTo($2)" },
            
            // Handle array with strict ordering
            { @"\.Should\(\)(?:\s*\n*\s*|\s+)\.BeEquivalentTo\((new\[\][^\)]+?)\s*,(?:\s*\n*\s*|\s+)options(?:\s*\n*\s*|\s+)=>(?:\s*\n*\s*|\s+)options\.WithStrictOrdering\(\)\)", ".ShouldBe($1)" },
            { @"\.Should\(\)(?:\s*\n*\s*|\s+)\.BeEquivalentTo\((\[[^\]]+\])\s*,(?:\s*\n*\s*|\s+)options(?:\s*\n*\s*|\s+)=>(?:\s*\n*\s*|\s+)options\.WithStrictOrdering\(\)\)", ".ShouldBe($1)" },
            
            // Handle variable with strict ordering
            { @"\.Should\(\)\s*\n*\s*\.BeEquivalentTo\(([^,\)]+?)\s*,\s*\n*\s*options\s*=>\s*\n*\s*options\.WithStrictOrdering\(\)\)", ".ShouldBe($1)" },
            
            // Handle collection expressions
            { @"\.Should\(\)\s*\n*\s*\.BeEquivalentTo\((\[(?:[^\[\]]|\[(?:[^\[\]]|\[[^\[\]]*\])*\])*\])\)", ".ShouldBeEquivalentTo($1)" },
            
            // Handle empty BeEquivalentTo calls
            { @"\.Should\(\)\s*\n*\s*\.BeEquivalentTo\(\)", ".ShouldBeEquivalentTo()" },
            
            // Generic BeEquivalentTo pattern
            { @"\.Should\(\)\s*\n*\s*\.BeEquivalentTo\(([^,\)]+)\)", ".ShouldBeEquivalentTo($1)" },
            
            // Rest of the patterns
            { @"\.Should\(\)\s*\n*\s*\.NotBeEquivalentTo\(", ".ShouldNotBeEquivalentTo(" },
            { @"\.Should\(\)\s*\n*\s*\.ContainInOrder\(", ".ShouldBe(" },
            { @"\.Should\(\)\s*\n*\s*\.ContainSingle\(\)", ".ShouldHaveSingleItem()" },
            { @"\.Should\(\)\s*\n*\s*\.OnlyHaveUniqueItems\(\)", ".ShouldAllBeUnique()" },
            { @"\.Should\(\)\s*\n*\s*\.BeNullOrEmpty\(\)", ".ShouldBeNullOrEmpty()" },
            { @"\.Should\(\)\s*\n*\s*\.NotBeNullOrWhiteSpace\(\)", ".ShouldNotBeNullOrWhiteSpace()" },
            { @"\.Should\(\)\s*\n*\s*\.NotBeOfType<", ".ShouldNotBeOfType<" },
            { @"\.Should\(\)\s*\n*\s*\.BeOfType<([^>]+)>\(\)", ".ShouldBeOfType<$1>()" },
            { @"\.Should\(\)\s*\n*\s*\.BeSameAs\(", ".ShouldBeSameAs(" },
            { @"\.Should\(\)\s*\n*\s*\.NotBeSameAs\(", ".ShouldNotBeSameAs(" },
            { @"\.Should\(\)\s*\n*\s*\.BeSubsetOf\(", ".ShouldBeSubsetOf(" },
            { @"\.Should\(\)\s*\n*\s*\.NotBeSubsetOf\(", ".ShouldNotBeSubsetOf(" },
            { @"\.Should\(\)\s*\n*\s*\.IntersectWith\(", ".ShouldIntersectWith(" },
            { @"\.Should\(\)\s*\n*\s*\.BeGreaterThanOrEqualTo\(", ".ShouldBeGreaterThanOrEqualTo(" },
            { @"\.Should\(\)\s*\n*\s*\.BeLessThanOrEqualTo\(", ".ShouldBeLessThanOrEqualTo(" },
            { @"\.Should\(\)\s*\n*\s*\.BeGreaterThan\(", ".ShouldBeGreaterThan(" },
            { @"\.Should\(\)\s*\n*\s*\.BeLessThan\(", ".ShouldBeLessThan(" },
            { @"\.Should\(\)\s*\n*\s*\.BeInRange\(", ".ShouldBeInRange(" },
            { @"\.Should\(\)\s*\n*\s*\.NotBeInRange\(", ".ShouldNotBeInRange(" },
            { @"\.Should\(\)\s*\n*\s*\.CompleteWithin\(", ".ShouldCompleteIn(" },
            { @"\.Should\(\)\s*\n*\s*\.NotCompleteWithin\(", ".ShouldNotCompleteIn(" },
            { @"\.Should\(\)\s*\n*\s*\.HaveCount\(", ".Count().ShouldBe(" },
            { @"\.Should\(\)\s*\n*\s*\.StartWith\(", ".ShouldStartWith(" },
            { @"\.Should\(\)\s*\n*\s*\.EndWith\(", ".ShouldEndWith(" },
            { @"\.Should\(\)\s*\n*\s*\.ThrowExactly<", ".ShouldThrowExactly<" },
            { @"\.Should\(\)\s*\n*\s*\.ThrowAsync<", ".ShouldThrowAsync<" },
            { @"\.Should\(\)\s*\n*\s*\.Throw<([^>]+)>\(\)", ".ShouldThrow<$1>(() =>)" },
            { @"\.Should\(\)\s*\n*\s*\.NotThrow\(\)", ".ShouldNotThrow()" },
            { @"\.Should\(\)\s*\n*\s*\.NotContain\(", ".ShouldNotContain(" },
            { @"\.Should\(\)\s*\n*\s*\.Contain\(", ".ShouldContain(" },
            { @"\.Should\(\)\s*\n*\s*\.BeEmpty\(\)", ".ShouldBeEmpty()" },
            { @"\.Should\(\)\s*\n*\s*\.NotBeEmpty\(\)", ".ShouldNotBeEmpty()" },
            { @"\.Should\(\)\s*\n*\s*\.BeNull\(\)", ".ShouldBeNull()" },
            { @"\.Should\(\)\s*\n*\s*\.NotBeNull\(\)", ".ShouldNotBeNull()" },
            { @"\.Should\(\)\s*\n*\s*\.BeTrue\(\)", ".ShouldBeTrue()" },
            { @"\.Should\(\)\s*\n*\s*\.BeFalse\(\)", ".ShouldBeFalse()" },
            { @"\.Should\(\)\s*\n*\s*\.NotBe\(", ".ShouldNotBe(" },
            // Handle Be with parameters
            { @"\.Should\(\)\s*\n*\s*\.Be\(([^)]+)\)", ".ShouldBe($1)" },
            // Handle empty Be calls
            { @"\.Should\(\)\s*\n*\s*\.Be\(\)", ".ShouldBeEquivalentTo()" }
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

        private void UpdateProjectFiles(string solutionDir)
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

        private void UpdateTestFiles(string solutionDir)
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

                                    if (newContent == currentContent) continue;
                                    
                                    currentContent = newContent;
                                    madeChanges = true;
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
    }
} 