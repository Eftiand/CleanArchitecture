namespace coaches.Modules.Shared.Contracts.Constants;

public static class CommonConstants
{
    public static class Assemblies
    {
        public const string TodosApplication = "coaches.Module.Todos.Application";
        public const string Infrastructure = "coaches.Infrastructure";
        public const string WebApi = "coaches.WebApi";

        public const string Modules = "coaches.Modules";
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
        public const string Redis = "redis";
        public const string Postgres = "postgres";
    }
}
