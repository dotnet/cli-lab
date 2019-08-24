using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging.Query.Interpret;
using Microsoft.Build.Logging.Query.Result;
using Xunit;

namespace Microsoft.Build.Logging.Query.Tests.Interpret
{
    public class InterpreterTests
    {
        public static IEnumerable<object[]> GetDataForTestFilter()
        {
            var testBuild1 = new Result.Build();

            yield return new object[]
            {
                "/Project",
                testBuild1,
                new List<IQueryResult>()
            };

            yield return new object[]
            {
                "/Task",
                testBuild1,
                new List<IQueryResult>()
            };

            var testBuild2 = new Result.Build();
            var testMessage1_Build2 = new Message("build message 1", testBuild2, MessageImportance.High);
            var testMessage2_Build2 = new Message("build message 2", testBuild2, MessageImportance.Low);
            testBuild2.AddMessage(testMessage1_Build2);
            testBuild2.AddMessage(testMessage2_Build2);

            var testProject1_Build2 = testBuild2.AddProject(
                1,
                "./Proj1.csproj",
                Enumerable.Empty<DictionaryEntry>(),
                Enumerable.Empty<DictionaryEntry>(),
                new Dictionary<string, string>());
            var testMessage3_Build2 = new Message("project message 1", testProject1_Build2, MessageImportance.Normal);
            var testMessage4_Build2 = new Message("project message 2", testProject1_Build2, MessageImportance.Low);
            testProject1_Build2.AddMessage(testMessage3_Build2);
            testProject1_Build2.AddMessage(testMessage4_Build2);

            var testProject2_Build2 = testBuild2.AddProject(
                2,
                "./Proj2.csproj",
                Enumerable.Empty<DictionaryEntry>(),
                Enumerable.Empty<DictionaryEntry>(),
                new Dictionary<string, string>());
            var testMessage5_Build2 = new Message("project message 3", testProject2_Build2, MessageImportance.Normal);
            testProject2_Build2.AddMessage(testMessage5_Build2);

            var testTarget1_Project2_Build2 = testProject2_Build2.AddTarget(10, "CoreCompile", true);
            var testMessage6_Build2 = new Message("target message 1", testTarget1_Project2_Build2, MessageImportance.Low);
            testTarget1_Project2_Build2.AddMessage(testMessage6_Build2);

            var testTarget2_Project2_Build2 = testProject2_Build2.AddTarget(11, "Optimize", false);
            var testMessage7_Build2 = new Message("target message 2", testTarget2_Project2_Build2, MessageImportance.High);
            var testMessage8_Build2 = new Message("target message 3", testTarget2_Project2_Build2, MessageImportance.High);
            testTarget2_Project2_Build2.AddMessage(testMessage7_Build2);
            testTarget2_Project2_Build2.AddMessage(testMessage8_Build2);

            yield return new object[]
            {
                "/Message",
                testBuild2,
                new[] { testMessage1_Build2, testMessage2_Build2 }
            };

            yield return new object[]
            {
                "//Message",
                testBuild2,
                new[]
                {
                    testMessage1_Build2,
                    testMessage2_Build2,
                    testMessage3_Build2,
                    testMessage4_Build2,
                    testMessage5_Build2,
                    testMessage6_Build2,
                    testMessage7_Build2,
                    testMessage8_Build2
                }
            };

            yield return new object[]
            {
                "/Project/Message",
                testBuild2,
                new[] { testMessage3_Build2, testMessage4_Build2, testMessage5_Build2 }
            };

            yield return new object[]
            {
                "/Project[Id=1]/Message",
                testBuild2,
                new[] { testMessage3_Build2, testMessage4_Build2 }
            };

            yield return new object[]
            {
                "/Project[Id=2]/Message",
                testBuild2,
                new[] { testMessage5_Build2 }
            };

            yield return new object[]
            {
                "/Project[Id=2]//Message",
                testBuild2,
                new[] { testMessage5_Build2, testMessage6_Build2, testMessage7_Build2, testMessage8_Build2 }
            };

            yield return new object[]
            {
                "/Target[Id=11]/Message",
                testBuild2,
                new[] { testMessage7_Build2, testMessage8_Build2 }
            };

            yield return new object[]
            {
                "/Project",
                testBuild2,
                new[] { testProject1_Build2, testProject2_Build2 }
            };

            yield return new object[]
            {
                "/Project[Name=\"Proj1\"]",
                testBuild2,
                new[] { testProject1_Build2 }
            };

            yield return new object[]
            {
                "/Project[Name=\"Proj2\"]/Target[Name=\"Optimize\"]",
                testBuild2,
                new[] { testTarget2_Project2_Build2 }
            };
        }

        [Theory]
        [MemberData(nameof(GetDataForTestFilter))]
        public void TestFilter(string expression, Result.Build build, IList<IQueryResult> expected)
        {
            var interpreter = new Interpreter(expression);
            var actual = interpreter.Filter(build).ToList();

            actual.Count.Should().Be(expected.Count);

            for (var i = 0; i < expected.Count; i++)
            {
                actual[i].Should().Be(expected[i]);
            }
        }
    }
}