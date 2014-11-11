using System.Collections.Generic;

namespace ReaderAllAboutMonadsExample
{
    public class Environment
    {
        private readonly IReadOnlyDictionary<string, Template> _templates;
        private readonly IReadOnlyDictionary<string, string> _variables;

        public Environment(IReadOnlyDictionary<string, Template> templates, IReadOnlyDictionary<string, string> variables)
        {
            _templates = templates;
            _variables = variables;
        }

        public IReadOnlyDictionary<string, Template> Templates { get { return _templates; } }
        public IReadOnlyDictionary<string, string> Variables { get { return _variables; } }
    }
}
