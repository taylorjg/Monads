using System.Collections.Generic;
using System.Linq;

namespace ReaderAllAboutMonadsExample
{
    public abstract class Template
    {
        public abstract string Show();
    }

    public class TextTemplate : Template
    {
        private readonly string _s;

        public TextTemplate(string s)
        {
            _s = s;
        }

        public string String { get { return _s; } }

        public override string Show()
        {
            return String;
        }
    }

    public class VariableTemplate : Template
    {
        private readonly Template _template;

        public VariableTemplate(Template template)
        {
            _template = template;
        }

        public Template Template { get { return _template; } }

        public override string Show()
        {
            return string.Format("${{{0}}}", Template.Show());
        }
    }

    public class QuoteTemplate : Template
    {
        private readonly Template _template;

        public QuoteTemplate(Template template)
        {
            _template = template;
        }

        public Template Template { get { return _template; } }

        public override string Show()
        {
            return string.Format("$\"{0}\"", Template.Show());
        }
    }

    public class IncludeTemplate : Template
    {
        private readonly Template _template;
        private readonly IReadOnlyList<Definition> _definitions;

        public IncludeTemplate(Template template, IReadOnlyList<Definition> definitions)
        {
            _template = template;
            _definitions = definitions;
        }

        public Template Template { get { return _template; } }
        public IReadOnlyList<Definition> Definitions { get { return _definitions; } }

        public override string Show()
        {
            var name = Template.Show();
            var definitions = string.Join(", ", Definitions.Select(d => d.Show()));
            return Definitions.Count == 0
                       ? string.Format("$<{0}>", name)
                       : string.Format("$<{0}|{1}>", name, definitions);
        }
    }

    public class CompoundTemplate : Template
    {
        private readonly IReadOnlyList<Template> _templates;

        public CompoundTemplate(IReadOnlyList<Template> templates)
        {
            _templates = templates;
        }

        public IReadOnlyList<Template> Templates { get { return _templates; } }

        public override string Show()
        {
            // concatMap :: (a -> [b]) -> [a] -> [b]
            // concatMap f =  foldr ((++) . f) []
            // concatMap show ts
            return Flinq.Enumerable.FoldRight(Templates, string.Empty, (t, acc) => acc + t.Show());
        }
    }
}
