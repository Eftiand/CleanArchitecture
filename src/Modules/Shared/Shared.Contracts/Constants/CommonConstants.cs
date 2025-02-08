namespace coaches.Modules.Shared.Contracts.Constants;

public static class CommonConstants
{
    public static class Assemblies
    {
        public const string TodosApplication = "coaches.Module.Todos.Application";
        public const string Infrastructure = "coaches.Infrastructure";
        public const string WebApi = "coaches.Web";

        public static class Modules
        {
            private const string Namespace = "coaches.Modules";

            private static readonly string[] ModuleNames =
            {
                //"Kernel",
                "Todos",
                "Shared",
            };

            public static Dictionary<string, string> AllApplications => ModuleNames.ToDictionary(
                name => name,
                name => $"{Namespace}.{name}.Application"
            );

            public static Dictionary<string, string> AllContracts => ModuleNames.ToDictionary(
                name => name,
                name => $"{Namespace}.{name}.Contracts"
            );

            public static Dictionary<string, string> AllDomains => ModuleNames.ToDictionary(
                name => name,
                name => $"{Namespace}.{name}.Domain"
            );
        }
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
