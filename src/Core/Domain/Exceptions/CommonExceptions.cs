namespace CleanArchitecture.Domain.Exceptions;

public static class CommonExceptions
{
    public static class DomainExceptions
    {
        public static NotFoundException<TDomain> NotFound<TDomain>() => new();
    }
}

public class NotFoundException<TDomain>() : BaseException();

public class BaseException : Exception;
