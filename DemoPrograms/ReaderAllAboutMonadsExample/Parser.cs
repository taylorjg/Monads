using System.Collections.Generic;
using ParsersLib;

namespace ReaderAllAboutMonadsExample
{
    public static class Parser
    {
        public static Parser<IEnumerable<NamedTemplate>> TemplateFile()
        {
            return null;
        }

        public static Parser<NamedTemplate> NamedTemplate()
        {
            return null;
        }

        public static Parser<string> Name()
        {
            return null;
        }

        public static Parser<string> End()
        {
            return null;
        }

        public static Parser<Template> Template(string except)
        {
            return null;
        }

        public static Parser<Template> SimpleTemplate(string except)
        {
            return null;
        }

        public static Parser<char> Dollar()
        {
            return null;
        }

        public static Parser<char> LeftBracket()
        {
            return null;
        }

        public static Parser<char> TextChar(string except)
        {
            return null;
        }

        public static Parser<Template> Text(string except)
        {
            return null;
        }

        public static Parser<Template> Variable()
        {
            return null;
        }

        public static Parser<Template> Quote()
        {
            return null;
        }

        public static Parser<Template> Include()
        {
            return null;
        }

        public static Parser<Template> IncludeBody()
        {
            return null;
        }

        public static Parser<IEnumerable<Definition>> Definitions()
        {
            return null;
        }

        public static Parser<Definition> Definition()
        {
            return null;
        }
    }
}
