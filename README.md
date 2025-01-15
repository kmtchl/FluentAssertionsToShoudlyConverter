# FluentAssertionsToShouldlyConverter

## Overview

FluentAssertions v8 onwards will require a license (https://github.com/fluentassertions/fluentassertions/pull/2943) and you may wish to move away from it. 
`FluentAssertionsToShouldlyConverter` is a tool designed to convert assertions written using Fluent Assertions to Shouldly assertions in C# projects. 
This tool scans through C# files in a specified directory and replaces Fluent Assertions syntax with the equivalent Shouldly syntax (https://docs.shouldly.org/).

#### Note: You should check all conversions are correct after running the tool. 
For example, some dictionary/list equivalence comparisons may need to be handled manually since they can need different approaches depending on the scenario.

## Features

- Converts a wide range of Fluent Assertions to Shouldly assertions.
- Handles both single and chained assertions.
- Automatically updates `using` directives to remove `FluentAssertions` and add `Shouldly`.

## Usage

1. **Clone the repository:**

2. **Install Shouldy via Nuget if it's not already installed**.

3. **Build the project:**

   Open the project in your preferred IDE (e.g., Visual Studio, JetBrains Rider) and build the solution.

4. **Run the converter:**

   You can run the converter from the command line or within your IDE.

   **Command Line:**

    ```sh
    dotnet run -- <directory-path>
    ```

   Replace `<directory-path>` with the path to the directory containing the C# files you want to convert.

   **IDE:**

    - Set the directory path as a command-line argument in your IDE's run configuration.
    - Run the `Program` class.

5. **Follow the prompts:**

   If no directory path is provided as an argument, the program will prompt you to enter the directory path.

## Example

Given a C# file with the following Fluent Assertions:

```csharp
myList.Should().HaveCount(3);
myString.Should().Be("Hello");
```

After running the converter, it will be transformed to:

```csharp
myList.Count().ShouldBe(3);
myString.ShouldBe("Hello");
```