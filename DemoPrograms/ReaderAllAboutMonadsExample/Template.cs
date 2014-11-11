using System.Collections.Generic;

namespace ReaderAllAboutMonadsExample
{
    public abstract class Template
    {
    }

    public class T : Template
    {
        private readonly string _s;

        public T(string s)
        {
            _s = s;
        }

        public string String { get { return _s; } }
    }

    public class V : Template
    {
        private readonly Template _template;

        public V(Template template)
        {
            _template = template;
        }

        public Template Template { get { return _template; } }
    }

    public class Q : Template
    {
        private readonly Template _template;

        public Q(Template template)
        {
            _template = template;
        }

        public Template Template { get { return _template; } }
    }

    public class I : Template
    {
        private readonly Template _template;
        private readonly IReadOnlyList<Definition> _definitions;

        public I(Template template, IReadOnlyList<Definition> definitions)
        {
            _template = template;
            _definitions = definitions;
        }

        public Template Template { get { return _template; } }
        public IReadOnlyList<Definition> Definitions { get { return _definitions; } }
    }

    public class C : Template
    {
        private readonly IReadOnlyList<Template> _templates;

        public C(IReadOnlyList<Template> templates)
        {
            _templates = templates;
        }

        public IReadOnlyList<Template> Templates { get { return _templates; } }
    }
}
