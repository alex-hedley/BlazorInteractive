# Tests

## AltCover

Change directory to `BlazorInteractive.Tests`.

Install AltCover

```bash
dotnet tool install --global altcover.global
dotnet add package AltCover
```

Install ReportGenerator

```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
```

Run this command to generate a html report from a `coverage.xml` file.

```bash
dotnet reportgenerator -reports:coverage.xml -targetdir:./coverage -assemblyfilters:'+BlazorInteractive*;-*.Tests' -classfilters:'+BlazorInteractive.*';
```

Load the [report file](../src/BlazorInteractive.Tests/coverage/index.html) at `BlazorInteractive.Tests/coverage/index.html`