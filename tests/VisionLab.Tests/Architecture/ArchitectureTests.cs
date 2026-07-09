using NetArchTest.Rules;
using Xunit;
using VisionLab.Application;
using VisionLab.Infrastructure;
using VisionLab.Core;

namespace VisionLab.Tests.Architecture;

public class ArchitectureTests
{
    [Fact]
    public void Application_should_not_depend_on_Infrastructure()
    {
        var result = Types
            .InAssembly(typeof(VisionLab.Application.AssemblyReference).Assembly)
            .ShouldNot()
            .HaveDependencyOn("VisionLab.Infrastructure")
            .GetResult();

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Core_should_not_have_external_dependencies()
    {
        var result = Types
            .InAssembly(typeof(VisionLab.Core.AssemblyReference).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "VisionLab.Application",
                "VisionLab.Infrastructure",
                "VisionLab.Api"
            )
            .GetResult();

        Assert.True(result.IsSuccessful);
    }
}