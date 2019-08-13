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
                    new MessageToken()
                }
            };

            yield return new object[]
            {
                "Message",
                new Token.Token[]
                {
                    new MessageToken()
                }
            };

            yield return new object[]
            {
                "MESSAGE",
                new Token.Token[]
                {
                    new MessageToken()
                }
            };

            yield return new object[]
            {
                "warning",
                new Token.Token[]
                {
                    new WarningToken()
                }
            };

            yield return new object[]
            {
                "Warning",
                new Token.Token[]
                {
                    new WarningToken()
                }
            };

            yield return new object[]
            {
                "WARNING",
                new Token.Token[]
                {
                    new WarningToken()
                }
            };

            yield return new object[]
            {
                "error",
                new Token.Token[]
                {
                    new ErrorToken()
                }
            };

            yield return new object[]
            {
                "Error",
                new Token.Token[]
                {
                    new ErrorToken()
                }
            };

            yield return new object[]
            {
                "ERROR",
                new Token.Token[]
                {
                    new ErrorToken()
                }
            };

            yield return new object[]
            {
                "project",
                new Token.Token[]
                {
                    new ProjectToken()
                }
            };

            yield return new object[]
            {
                "Project",
                new Token.Token[]
                {
                    new ProjectToken()
                }
            };

            yield return new object[]
            {
                "PROJECT",
                new Token.Token[]
                {
                    new ProjectToken()
                }
            };

            yield return new object[]
            {
                "target",
                new Token.Token[]
                {
                    new TargetToken()
                }
            };

            yield return new object[]
            {
                "Target",
                new Token.Token[]
                {
                    new TargetToken()
                }
            };

            yield return new object[]
            {
                "TARGET",
                new Token.Token[]
                {
                    new TargetToken()
                }
            };

            yield return new object[]
            {
                "task",
                new Token.Token[]
                {
                    new TaskToken()
                }
            };

            yield return new object[]
            {
                "Task",
                new Token.Token[]
                {
                    new TaskToken()
                }
            };

            yield return new object[]
            {
                "TASK",
                new Token.Token[]
                {
                    new TaskToken()
                }
            };

            yield return new object[]
            {
                "/",
                new Token.Token[]
                {
                    new SlashToken()
                }
            };

            yield return new object[]
            {
                "[",
                new Token.Token[]
                {
                    new LeftBracketToken()
                }
            };

            yield return new object[]
            {
                "]",
                new Token.Token[]
                {
                    new RightBracketToken()
                }
            };

            yield return new object[]
            {
                "=",
                new Token.Token[]
                {
                    new EqualToken()
                }
            };

            yield return new object[]
            {
                ",",
                new Token.Token[]
                {
                    new CommaToken()
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
                    new SlashToken(),
                    new MessageToken()
                }
            };

            yield return new object[]
            {
                "/project[]/target/error",
                new Token.Token[]
                {
                    new SlashToken(),
                    new ProjectToken(),
                    new LeftBracketToken(),
                    new RightBracketToken(),
                    new SlashToken(),
                    new TargetToken(),
                    new SlashToken(),
                    new ErrorToken()
                }
            };

            yield return new object[]
            {
                "   /   target[ ] / warning",
                new Token.Token[]
                {
                    new SlashToken(),
                    new TargetToken(),
                    new LeftBracketToken(),
                    new RightBracketToken(),
                    new SlashToken(),
                    new WarningToken()
                }
            };

            yield return new object[]
            {
                "///message///warning///error///",
                new Token.Token[]
                {
                    new SlashToken(),
                    new SlashToken(),
                    new SlashToken(),
                    new MessageToken(),
                    new SlashToken(),
                    new SlashToken(),
                    new SlashToken(),
                    new WarningToken(),
                    new SlashToken(),
                    new SlashToken(),
                    new SlashToken(),
                    new ErrorToken(),
                    new SlashToken(),
                    new SlashToken(),
                    new SlashToken(),
                }
            };

            yield return new object[]
            {
                "message message",
                new Token.Token[]
                {
                    new MessageToken(),
                    new MessageToken()
                }
            };

            yield return new object[]
            {
                "messagemessage",
                new Token.Token[]
                {
                    new MessageToken(),
                    new MessageToken()
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