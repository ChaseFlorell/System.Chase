namespace System.Chase
{
    public class FluentUriBuilder
    {
        private readonly UriBuilder _builder;

        public FluentUriBuilder()
        {
            _builder = new UriBuilder();
        }

        public Uri Build() => _builder.Uri;

        public FluentUriBuilder Credentials(string userName, string password)
        {
            _builder.Password = password;
            _builder.UserName = userName;
            return this;
        }

        public FluentUriBuilder Fragment(string fragment)
        {
            _builder.Fragment = fragment;
            return this;
        }

        public FluentUriBuilder Host(string host)
        {
            _builder.Host = host;
            return this;
        }

        public FluentUriBuilder Path(string path)
        {
            _builder.Path = path;
            return this;
        }

        public FluentUriBuilder Port(string port)
        {
            _builder.Port = port;
            return this;
        }

        public FluentUriBuilder Query(string query)
        {
            _builder.Query = query;
            return this;
        }

        public FluentUriBuilder Scheme(string scheme)
        {
            _builder.Scheme = scheme;
            return this;
        }
    }
}
