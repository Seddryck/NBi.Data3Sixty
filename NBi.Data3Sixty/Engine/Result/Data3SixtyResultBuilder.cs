using NUnit.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace NBi.Data3Sixty.Engine.Result
{
    public class Data3SixtyResultBuilder
    {
        public IList<RuleResult> Execute(TestResult nunitResult)
        {
            return ParseChild(Enumerable.Repeat(nunitResult, 1)).ToList();
        }

        private IEnumerable<RuleResult> ParseChild(IEnumerable<TestResult> nunitResults)
        {
            var childResults = new List<RuleResult>();
            foreach (var r in nunitResults)
            {
                if (r.Test.IsSuite)
                {
                    var results = ParseChild(r.Results.Cast<TestResult>());
                    childResults.AddRange(results);
                }
                else
                {
                    var rr = ParseElement(r);
                    if (rr != null)
                        childResults.Add(rr);
                }

            }
            return childResults;
        }

        private RuleResult ParseElement(TestResult nunitResult)
        {
            if (!nunitResult.Executed)
                return null;

            return new RuleResult()
            {
                RuleImplementationID = TryParseImplementationIdentifier(ParseProperties("Identifier", nunitResult), out var identifier) ? identifier : 0,
                PassFraction = ParseMessage(nunitResult.Message),
                FailFraction = 1 - ParseMessage(nunitResult.Message),
                Passed = nunitResult.IsSuccess,
                EffectiveDate = DateTime.Now,
                RunDate = DateTime.Now,
            };
        }

        private string ParseProperties(string name, TestResult nunitResult)
            => nunitResult.Test.Properties.Contains(name) ? nunitResult.Test.Properties[name].ToString() : string.Empty;

        private bool TryParseImplementationIdentifier(string fullId, out int implementationId)
        {
            implementationId = 0;
            fullId = fullId.Replace("/", ".");
            var ids = fullId.Split(new[] { '.' });
            if (ids.Length != 2)
                return false;

            return Int32.TryParse(ids[1], out implementationId);
        }

        protected virtual decimal ParseMessage(string msg)
        {
            if (string.IsNullOrEmpty(msg))
                throw new InvalidMessageException("NUnit result has no field 'Message' or this field is not filled.");

            if (msg.StartsWith("NBi.NUnit.Runtime.CustomStackTraceAssertionException"))
                msg = msg.Substring(msg.IndexOf('{'));

            if (!msg.StartsWith("{"))
                throw new InvalidMessageException("NUnit result has a field 'Message' but this field hasn't the expected format.");

            var obj = JObject.Parse(msg);

            if (obj.TryGetValue("score", out var parsed))
            {
                if (parsed.HasValues)
                    throw new InvalidMessageException($"NUnit result has a field 'Message' and information about the 'score' but multiple values are returned: '{parsed}'");

                return (decimal)parsed.Value<float>();
            }
            throw new InvalidMessageException("NUnit result has a field 'Message' but it doesn't contain any information about the 'score'.");
        }



    }
}
