using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace CoEditor.Tests.Helper;

public static class TestOutputHelperExtensions
{
    public static string GetTestName(this ITestOutputHelper output)
    {
        var type = output.GetType();
        var testMember = type.GetField("test", BindingFlags.Instance | BindingFlags.NonPublic);
        if (testMember == null)
            throw FailException.ForFailure("Could not get test name as there is no field 'test' on ITestOutputHelper");
        if (testMember.GetValue(output) is not ITest test)
            throw FailException.ForFailure("Could not get test name");
        return test.DisplayName;
    }
}
