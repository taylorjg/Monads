using System;

namespace ReaderAllAboutMonadsExample
{
    public class NamedTemplate
    {
        private readonly string _name;
        private readonly Template _template;

        public NamedTemplate(string name, Template template)
        {
            _name = name;
            _template = template;
        }

        public string Name { get { return _name; } }
        public Template Template { get { return _template; } }

        public Tuple<string, Template> StripName()
        {
            return Tuple.Create(Name, Template);
        }

        public string Show()
        {
            return string.Format("[{0}]{1}[END]{2}", Name, Template.Show(), System.Environment.NewLine);
        }
    }
}
