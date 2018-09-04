using NUnit.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                RuleImplementationID = Int32.TryParse(ParseProperties("Identifier", nunitResult), out var identifier) ? identifier : 0,
                PassFraction = ParseMessage(nunitResult.Message),
                FailFraction = 1 - ParseMessage(nunitResult.Message),
                Passed = nunitResult.IsSuccess,
                EffectiveDate = DateTime.Now,
                RunDate = DateTime.Now,
            };

        }

        private string ParseProperties(string name, TestResult nunitResult)
            => nunitResult.Test.Properties.Contains(name) ? nunitResult.Test.Properties[name].ToString() : string.Empty;

        private const string SCORE_PATTERN = "score: ";

        protected virtual double ParseMessage(string msg)
        {
            if (string.IsNullOrEmpty(msg))
                throw new InvalidMessageException("NUnit result has no field 'Message' or this field is not filled.");
            if (msg.IndexOf(SCORE_PATTERN) == -1)
                throw new InvalidMessageException("NUnit result has a field 'Message' but it doesn't contain any information about the 'score'.");
            var start = msg.IndexOf(SCORE_PATTERN) + SCORE_PATTERN.Length;
            var end = msg.IndexOf(" ", start + 1)==-1 ? msg.Length : msg.IndexOf(" ", start + 1);
            var parsed = msg.Substring(start, end - start);
            return double.TryParse(parsed, out var value) ? value : throw new InvalidMessageException($"NUnit result has a field 'Message' and information about the 'score' but the value '{parsed}' is not parsable.");
        }
            
    }
}
