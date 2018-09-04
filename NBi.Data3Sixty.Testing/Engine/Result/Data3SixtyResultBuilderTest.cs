﻿using NUnit.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using System.Text;
using System.Threading.Tasks;
using NBi.Data3Sixty.Engine.Filter;
using NBi.Data3Sixty.Engine.Result;
using Moq;

namespace NBi.Data3Sixty.Testing.Engine.Result
{
    public class Data3SixtyResultBuilderTest
    {
        [Test]
        public void Execute_TestResult_ParseIdentifier()
        {
            var test = Mock.Of<ITest>(
                x => x.IsSuite == false 
                && x.TestName==new TestName() 
                && x.Properties == new Dictionary<string, object>() { { "Identifier", "122" } });

            var raw = new TestResult(test);
            raw.SetResult(ResultState.Error, "Score is not achieved - score: 0.75", string.Empty);
            var builder = new Data3SixtyResultBuilder();
            var result = builder.Execute(raw);
            Assert.That(result[0].RuleImplementationID, Is.EqualTo(122));
        }

        [Test]
        public void Execute_TestResult_ParseScore()
        {
            var test = Mock.Of<ITest>(
                x => x.IsSuite == false
                && x.TestName == new TestName()
                && x.Properties == new Dictionary<string, object>() { { "Identifier", "122" } });

            var raw = new TestResult(test);
            raw.SetResult(ResultState.Error, "Score is not achieved - score: 0.75", string.Empty);
            var builder = new Data3SixtyResultBuilder();
            var result = builder.Execute(raw);
            Assert.That(result[0].PassFraction, Is.EqualTo(0.75));
            Assert.That(result[0].FailFraction, Is.EqualTo(0.25));
        }

        [Test]
        public void Execute_TestResultSuccess_ParseScore()
        {
            var test = Mock.Of<ITest>(
                x => x.IsSuite == false
                && x.TestName == new TestName()
                && x.Properties == new Dictionary<string, object>() { { "Identifier", "123" } });

            var raw = new TestResult(test);
            raw.SetResult(ResultState.Success, "Wow great Score- score: 1", string.Empty);
            var builder = new Data3SixtyResultBuilder();
            var result = builder.Execute(raw);
            Assert.That(result[0].PassFraction, Is.EqualTo(1));
            Assert.That(result[0].FailFraction, Is.EqualTo(0));
        }

        [Test]
        public void Execute_TestResultFailed_ParsePassed()
        {
            var test = Mock.Of<ITest>(
                x => x.IsSuite == false
                && x.TestName == new TestName()
                && x.Properties == new Dictionary<string, object>() { { "Identifier", "122" } });

            var raw = new TestResult(test);
            raw.SetResult(ResultState.Error, "Score is not achieved - score: 0.75", string.Empty);
            var builder = new Data3SixtyResultBuilder();
            var result = builder.Execute(raw);
            Assert.That(result[0].Passed, Is.False);
        }

        [Test]
        public void Execute_TestResultSuccess_ParsePassed()
        {
            var test = Mock.Of<ITest>(
                x => x.IsSuite == false
                && x.TestName == new TestName()
                && x.Properties == new Dictionary<string, object>() { { "Identifier", "123" } });

            var raw = new TestResult(test);
            raw.SetResult(ResultState.Success, "Wow great Score- score: 1", string.Empty);
            var builder = new Data3SixtyResultBuilder();
            var result = builder.Execute(raw);
            Assert.That(result[0].Passed, Is.True);
        }

    }
}
