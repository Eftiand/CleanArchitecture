namespace Shared.Const;

public static class CommonConstants
{
    public static class Assemblies
    {
        public const string Application = "CleanArchitecture.Application";
        public const string Domain = "Domain";
        public const string Infrastructure = "Infrastructure";
        public const string WebApi = "WebApi";
    }
    public static class Api
    {
        public static class Version
        {
            public const string V1 = "v1";
            public const string V2 = "v2";
        }
    }
    public static class Aspire
    {
        public const string RabbitMq = "rabbitmq";
        public const string Postgres = "postgres";
    }

}
