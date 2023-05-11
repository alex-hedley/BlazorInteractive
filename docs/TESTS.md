# Tests

## coverlet

```shell
cd src/BlazorInteractive.Compilation.Tests
```

```shell
`dotnet test --collect:"XPlat Code Coverage"
```

`TestResults/[GUID]/coverage.cobertura.xml`

```shell
ReportGenerator -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:coveragereport
```

- [index.html](../src/BlazorInteractive.Compilation.Tests/coveragereport/index.html)

## coverlet.console

```shell
dotnet tool install --global coverlet.console
```

> `coverlet /path/to/test-assembly.dll --target "dotnet" --targetargs "test /path/to/test-project --no-build"`

- [.NET Core Code Coverage as a Global Tool with coverlet](https://www.hanselman.com/blog/net-core-code-coverage-as-a-global-tool-with-coverlet)

> `coverlet .\bin\Debug\netcoreapp2.1\hanselminutes.core.tests.dll --target "dotnet" --targetargs "test --no-build"`

Run this from the `src` directory:

`coverlet BlazorInteractive.Compilation.Tests/bin/Debug/net7.0/BlazorInteractive.Compilation.dll --target "dotnet" --targetargs "test --no-build"`

## AltCover

Change directory to `BlazorInteractive.Tests`.

`cd BlazorInteractive.Tests`

Install **AltCover**.

```shell
dotnet tool install --global altcover.global
dotnet add package AltCover
```

Install ReportGenerator

```shell
dotnet tool install -g dotnet-reportgenerator-globaltool
```

Run this command to generate a html report from a `coverage.xml` file.

```shell
dotnet reportgenerator -reports:coverage.xml -targetdir:./coverage -assemblyfilters:'+BlazorInteractive*;-*.Tests' -classfilters:'+BlazorInteractive.*';
```

Load the [report file](../src/BlazorInteractive.Tests/coverage/index.html) at `BlazorInteractive.Tests/coverage/index.html`
