namespace D42_POC_Demo
{
    public struct Column
    {
        public string name;
        public string type;
        public string descr;

        public object[] all
        {
            get { return new object[] { name, type, descr }; }
        }
    }
}