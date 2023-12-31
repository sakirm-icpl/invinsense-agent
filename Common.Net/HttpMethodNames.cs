namespace Common.Net
{
    public readonly struct HttpMethodNames
    {
        public readonly string Method;

        private HttpMethodNames(string method)
        {
            Method = method;
        }

        public static readonly HttpMethodNames Get = new HttpMethodNames("Get");
        public static readonly HttpMethodNames Put = new HttpMethodNames("Put");
        public static readonly HttpMethodNames Post = new HttpMethodNames("Post");
        public static readonly HttpMethodNames Delete = new HttpMethodNames("Delete");

        public override string ToString()
        {
            return Method;
        }
    }
}
