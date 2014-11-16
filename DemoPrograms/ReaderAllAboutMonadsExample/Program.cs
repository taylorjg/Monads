using System;
using System.Collections.Generic;
using System.Linq;
using MonadLib;

namespace ReaderAllAboutMonadsExample
{
    public class Program
    {
        private static void Main()
        {
        }

        private static Maybe<string> LookupVar(string name, Environment env)
        {
            string v;
            return env.Variables.TryGetValue(name, out v) ? Maybe.Just(v) : Maybe.Nothing<string>();
        }

        private static Maybe<Template> LookupTemplate(string name, Environment env)
        {
            Template t;
            return env.Templates.TryGetValue(name, out t) ? Maybe.Just(t) : Maybe.Nothing<Template>();
        }

        private static Environment AddDefs(IEnumerable<KeyValuePair<string, string>> defs, Environment env)
        {
            var newDefs = defs.Concat(env.Variables).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            return new Environment(env.Templates, newDefs);
        }

        private static Reader<Environment, KeyValuePair<string, string>> ResolveDef(Definition definition)
        {
            return Resolve(definition.TemplateT).Bind(
                name => Resolve(definition.TemplateD).Bind(
                    value => Reader<Environment>.Return(new KeyValuePair<string, string>(name, value))));
        }

        private static Reader<Environment, string> Resolve(Template template)
        {
            Func<string, Func<Environment, Maybe<string>>> partiallyApplyLookupVar = name => env => LookupVar(name, env);
            Func<string, Func<Environment, Maybe<Template>>> partiallyApplyLookupTemplate = name => env => LookupTemplate(name, env);

            if (template is TextTemplate)
            {
                return Reader<Environment>.Return((template as TextTemplate).String);
            }

            if (template is VariableTemplate)
            {
                var t = (template as VariableTemplate).Template;
                return Resolve(t).Bind(
                    varName => Reader.Asks(partiallyApplyLookupVar(varName)).Bind(
                        varValue => Reader<Environment>.Return(Maybe.MapOrDefault(string.Empty, x => x, varValue))));
            }

            if (template is QuoteTemplate)
            {
                var t = (template as QuoteTemplate).Template;
                return Resolve(t).Bind(
                    tmplName => Reader.Asks(partiallyApplyLookupTemplate(tmplName)).Bind(
                        body => Reader<Environment>.Return(Maybe.MapOrDefault(string.Empty, x => x.ToString(), body))));
            }

            if (template is IncludeTemplate)
            {
                var t = (template as IncludeTemplate).Template;
                var ds = (template as IncludeTemplate).Definitions;

                return Resolve(t).Bind(
                    tmplName => Reader.Asks(partiallyApplyLookupTemplate(tmplName)).Bind(
                        body => body.Match(
                            t2 => Reader.MapM(ResolveDef, ds).Bind(defs => Reader.Local(r => AddDefs(defs, r), Resolve(t2))),
                            () => Reader<Environment>.Return(string.Empty))));
            }

            if (template is CompoundTemplate)
            {
                var ts = (template as CompoundTemplate).Templates;
                return Reader.MapM(Resolve, ts).LiftM(string.Concat);
            }

            throw new InvalidOperationException("Unknown Template type");
        }
    }
}
