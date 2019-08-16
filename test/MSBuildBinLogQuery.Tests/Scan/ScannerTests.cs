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
        [InlineData("0123456789")]
        [InlineData("/project[]/message?")]
        [InlineData("\\project[]\\message")]
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