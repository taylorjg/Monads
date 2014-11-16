namespace ReaderAllAboutMonadsExample
{
    public class Definition
    {
        private readonly Template _templateT;
        private readonly Template _templateD;

        public Definition(Template templateT, Template templateD)
        {
            _templateT = templateT;
            _templateD = templateD;
        }

        public Template TemplateT { get { return _templateT; } }
        public Template TemplateD { get { return _templateD; } }

        public string Show()
        {
            return string.Format("{0}={1}", TemplateT.Show(), TemplateD.Show());
        }
    }
}
