using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Build.Logging.Query.Scan;
using Microsoft.Build.Logging.Query.Token;
using Xunit;

namespace Microsoft.Build.Logging.Query.Tests.Scan
{
    public class ScannerTests
    {
        public static IEnumerable<object[]> GenerateDataForTestScannedTokens()
        {
            yield return new object[]
            {
                "message",
                new Token.Token[]
                {
                    MessageToken.Instance
                }
            };

            yield return new object[]
            {
                "Message",
                new Token.Token[]
                {
                    MessageToken.Instance
                }
            };

            yield return new object[]
            {
                "MESSAGE",
                new Token.Token[]
                {
                    MessageToken.Instance
                }
            };

            yield return new object[]
            {
                "warning",
                new Token.Token[]
                {
                    WarningToken.Instance
                }
            };

            yield return new object[]
            {
                "Warning",
                new Token.Token[]
                {
                    WarningToken.Instance
                }
            };

            yield return new object[]
            {
                "WARNING",
                new Token.Token[]
                {
                    WarningToken.Instance
                }
            };

            yield return new object[]
            {
                "error",
                new Token.Token[]
                {
                    ErrorToken.Instance
                }
            };

            yield return new object[]
            {
                "Error",
                new Token.Token[]
                {
                    ErrorToken.Instance
                }
            };

            yield return new object[]
            {
                "ERROR",
                new Token.Token[]
                {
                    ErrorToken.Instance
                }
            };

            yield return new object[]
            {
                "project",
                new Token.Token[]
                {
                    ProjectToken.Instance
                }
            };

            yield return new object[]
            {
                "Project",
                new Token.Token[]
                {
                    ProjectToken.Instance
                }
            };

            yield return new object[]
            {
                "PROJECT",
                new Token.Token[]
                {
                    ProjectToken.Instance
                }
            };

            yield return new object[]
            {
                "target",
                new Token.Token[]
                {
                    TargetToken.Instance
                }
            };

            yield return new object[]
            {
                "Target",
                new Token.Token[]
                {
                    TargetToken.Instance
                }
            };

            yield return new object[]
            {
                "TARGET",
                new Token.Token[]
                {
                    TargetToken.Instance
                }
            };

            yield return new object[]
            {
                "task",
                new Token.Token[]
                {
                    TaskToken.Instance
                }
            };

            yield return new object[]
            {
                "Task",
                new Token.Token[]
                {
                    TaskToken.Instance
                }
            };

            yield return new object[]
            {
                "TASK",
                new Token.Token[]
                {
                    TaskToken.Instance
                }
            };

            yield return new object[]
            {
                "/",
                new Token.Token[]
                {
                    SingleSlashToken.Instance
                }
            };

            yield return new object[]
            {
                "//",
                new Token.Token[]
                {
                    DoubleSlashToken.Instance
                }
            };

            yield return new object[]
            {
                "[",
                new Token.Token[]
                {
                    LeftBracketToken.Instance
                }
            };

            yield return new object[]
            {
                "]",
                new Token.Token[]
                {
                    RightBracketToken.Instance
                }
            };

            yield return new object[]
            {
                "=",
                new Token.Token[]
                {
                    EqualToken.Instance
                }
            };

            yield return new object[]
            {
                ",",
                new Token.Token[]
                {
                    CommaToken.Instance
                }
            };

            yield return new object[]
            {
                "\"\"",
                new Token.Token[]
                {
                    new StringToken("")
                }
            };

            yield return new object[]
            {
                "\"test\"",
                new Token.Token[]
                {
                    new StringToken("test")
                }
            };

            yield return new object[]
            {
                "\"hello world\"",
                new Token.Token[]
                {
                    new StringToken("hello world")
                }
            };

            yield return new object[]
            {
                "\"'hello world'\"",
                new Token.Token[]
                {
                    new StringToken("'hello world'")
                }
            };

            yield return new object[]
            {
                "9",
                new Token.Token[]
                {
                    new IntegerToken(9)
                }
            };

            yield return new object[]
            {
                "19980321",
                new Token.Token[]
                {
                    new IntegerToken(19980321)
                }
            };

            yield return new object[]
            {
                "0321",
                new Token.Token[]
                {
                    new IntegerToken(321)
                }
            };

            yield return new object[]
            {
                "/message",
                new Token.Token[]
                {
                    SingleSlashToken.Instance,
                    MessageToken.Instance
                }
            };

            yield return new object[]
            {
                "/project[]/target/error",
                new Token.Token[]
                {
                    SingleSlashToken.Instance,
                    ProjectToken.Instance,
                    LeftBracketToken.Instance,
                    RightBracketToken.Instance,
                    SingleSlashToken.Instance,
                    TargetToken.Instance,
                    SingleSlashToken.Instance,
                    ErrorToken.Instance
                }
            };

            yield return new object[]
            {
                "   /   target[ ] / warning",
                new Token.Token[]
                {
                    SingleSlashToken.Instance,
                    TargetToken.Instance,
                    LeftBracketToken.Instance,
                    RightBracketToken.Instance,
                    SingleSlashToken.Instance,
                    WarningToken.Instance
                }
            };

            yield return new object[]
            {
                "///message////warning/error//",
                new Token.Token[]
                {
                    DoubleSlashToken.Instance,
                    SingleSlashToken.Instance,
                    MessageToken.Instance,
                    DoubleSlashToken.Instance,
                    DoubleSlashToken.Instance,
                    WarningToken.Instance,
                    SingleSlashToken.Instance,
                    ErrorToken.Instance,
                    DoubleSlashToken.Instance
                }
            };

            yield return new object[]
            {
                "message message",
                new Token.Token[]
                {
                    MessageToken.Instance,
                    MessageToken.Instance
                }
            };

            yield return new object[]
            {
                "messagemessage",
                new Token.Token[]
                {
                    MessageToken.Instance,
                    MessageToken.Instance
                }
            };

            yield return new object[]
            {
                "///////",
                new Token.Token[]
                {
                    DoubleSlashToken.Instance,
                    DoubleSlashToken.Instance,
                    DoubleSlashToken.Instance,
                    SingleSlashToken.Instance
                }
            };

            yield return new object[]
            {
                "/project[task]  // target /message",
                new Token.Token[]
                {
                    SingleSlashToken.Instance,
                    ProjectToken.Instance,
                    LeftBracketToken.Instance,
                    TaskToken.Instance,
                    RightBracketToken.Instance,
                    DoubleSlashToken.Instance,
                    TargetToken.Instance,
                    SingleSlashToken.Instance,
                    MessageToken.Instance
                }
            };

            yield return new object[]
            {
                "/Project[Path=\"./project.csproj\"]/Target[Name=\"CoreCompile\"]//Error",
                new Token.Token[]
                {
                    SingleSlashToken.Instance,
                    ProjectToken.Instance,
                    LeftBracketToken.Instance,
                    PathToken.Instance,
                    EqualToken.Instance,
                    new StringToken("./project.csproj"),
                    RightBracketToken.Instance,
                    SingleSlashToken.Instance,
                    TargetToken.Instance,
                    LeftBracketToken.Instance,
                    NameToken.Instance,
                    EqualToken.Instance,
                    new StringToken("CoreCompile"),
                    RightBracketToken.Instance,
                    DoubleSlashToken.Instance,
                    ErrorToken.Instance
                }
            };

            yield return new object[]
            {
                "/Target[Name=\"Optimize\"]/Task[Id=81]//Warning",
                new Token.Token[]
                {
                    SingleSlashToken.Instance,
                    TargetToken.Instance,
                    LeftBracketToken.Instance,
                    NameToken.Instance,
                    EqualToken.Instance,
                    new StringToken("Optimize"),
                    RightBracketToken.Instance,
                    SingleSlashToken.Instance,
                    TaskToken.Instance,
                    LeftBracketToken.Instance,
                    IdToken.Instance,
                    EqualToken.Instance,
                    new IntegerToken(81),
                    RightBracketToken.Instance,
                    DoubleSlashToken.Instance,
                    WarningToken.Instance
                }
            };

            yield return new object[]
            {
                "/Project[Id=7]/Target[Id=42, Name=\"Link\"]//Message",
                new Token.Token[]
                {
                    SingleSlashToken.Instance,
                    ProjectToken.Instance,
                    LeftBracketToken.Instance,
                    IdToken.Instance,
                    EqualToken.Instance,
                    new IntegerToken(7),
                    RightBracketToken.Instance,
                    SingleSlashToken.Instance,
                    TargetToken.Instance,
                    LeftBracketToken.Instance,
                    IdToken.Instance,
                    EqualToken.Instance,
                    new IntegerToken(42),
                    CommaToken.Instance,
                    NameToken.Instance,
                    EqualToken.Instance,
                    new StringToken("Link"),
                    RightBracketToken.Instance,
                    DoubleSlashToken.Instance,
                    MessageToken.Instance
                }
            };

            yield return new object[]
            {
                "After",
                new Token.Token[]
                {
                    AfterToken.Instance
                }
            };

            yield return new object[]
            {
                "Before",
                new Token.Token[]
                {
                    BeforeToken.Instance
                }
            };

            yield return new object[]
            {
                "DependOn",
                new Token.Token[]
                {
                    DependOnToken.Instance
                }
            };

            yield return new object[]
            {
                "/Project[DependOn=[/Project[Id=1]]]",
                new Token.Token[]
                {
                    SingleSlashToken.Instance,
                    ProjectToken.Instance,
                    LeftBracketToken.Instance,
                    DependOnToken.Instance,
                    EqualToken.Instance,
                    LeftBracketToken.Instance,
                    SingleSlashToken.Instance,
                    ProjectToken.Instance,
                    LeftBracketToken.Instance,
                    IdToken.Instance,
                    EqualToken.Instance,
                    new IntegerToken(1),
                    RightBracketToken.Instance,
                    RightBracketToken.Instance,
                    RightBracketToken.Instance
                }
            };

            yield return new object[]
            {
                "/Project[Name=\"Test\"]/Task[Before=[/Task[Name=\"Compile\"]]]",
                new Token.Token[]
                {
                    SingleSlashToken.Instance,
                    ProjectToken.Instance,
                    LeftBracketToken.Instance,
                    NameToken.Instance,
                    EqualToken.Instance,
                    new StringToken("Test"),
                    RightBracketToken.Instance,
                    SingleSlashToken.Instance,
                    TaskToken.Instance,
                    LeftBracketToken.Instance,
                    BeforeToken.Instance,
                    EqualToken.Instance,
                    LeftBracketToken.Instance,
                    SingleSlashToken.Instance,
                    TaskToken.Instance,
                    LeftBracketToken.Instance,
                    NameToken.Instance,
                    EqualToken.Instance,
                    new StringToken("Compile"),
                    RightBracketToken.Instance,
                    RightBracketToken.Instance,
                    RightBracketToken.Instance
                }
            };
        }

        [Theory]
        [MemberData(nameof(GenerateDataForTestScannedTokens))]
        public void TestScannedTokens(string expression, IList<Token.Token> expectedTokens)
        {
            var scanner = new Scanner(expression);
            var actualTokens = new List<Token.Token>();

            for (; !(scanner.Token is EofToken); scanner.ReadNextToken())
            {
                actualTokens.Add(scanner.Token);
            }

            actualTokens.Count.Should().Be(expectedTokens.Count);

            for (var i = 0; i < actualTokens.Count; i++)
            {
                actualTokens[i].Should().Be(expectedTokens[i]);
            }
        }

        [Theory]
        [InlineData("\\")]
        [InlineData("?")]
        [InlineData("()")]
        [InlineData("<>")]
        [InlineData("hello")]
        [InlineData("messages")]
        [InlineData("-123456")]
        [InlineData("123.456")]
        [InlineData("0x123def")]
        [InlineData("9876543210")]
        [InlineData("/project[]/message?")]
        [InlineData("\\project[]\\message")]
        [InlineData("/Project[Id=536S]")]
        [InlineData("/Project[Identity=123]")]
        [InlineData("/Project[Path=HelloWorld]")]
        [InlineData("/Target[Id=HelloWorld]")]
        [InlineData("/Target[Name=HelloWorld]")]
        [InlineData("/Task[Id<123]")]
        public void TestScannedTokensException(string expression)
        {
            Action action = () =>
            {
                var scanner = new Scanner(expression);

                while (!(scanner.Token is EofToken))
                {
                    scanner.ReadNextToken();
                }
            };

            action.Should().Throw<ScanException>(expression);
        }
    }
}