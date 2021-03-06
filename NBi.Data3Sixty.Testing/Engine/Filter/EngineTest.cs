﻿using NBi.Data3Sixty.Engine.Filter;
using NBi.NUnit.Runtime;
using NBi.NUnit.Runtime.Embed;
using NBi.NUnit.Runtime.Embed.Result;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Embed.Testing
{
    public class EngineTest
    {
        [Test]
        public void Execute_WithIdentifierFilter_Ran()
        {
            var embed = new Engine();
            var filter = new IdentifierFilter("123");
            var result = embed.Execute(@"Resources\SmallTestSuite.config", filter);
            var builder = new FlatResultBuilder();
            var agg = builder.Execute(result);
            Assert.That(agg.Count, Is.EqualTo(1));
        }

        [Test]
        public void Execute_WithImplementationIdentifierFilter_Ran()
        {
            var embed = new Engine();
            var filter = new IdentifierFilter("124.133");
            var result = embed.Execute(@"Resources\SmallTestSuite.config", filter);
            var builder = new FlatResultBuilder();
            var agg = builder.Execute(result);
            Assert.That(agg.Count, Is.EqualTo(1));
        }
    }
}
