namespace WriterAllAboutMonadsExample
{
    public abstract class DataBase
    {
    }

    public class Data : DataBase
    {
        public Data(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }

        private readonly string _value;
    }

    public class AnyData : DataBase
    {
    }
}
