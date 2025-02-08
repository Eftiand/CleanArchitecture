namespace coaches.Modules.Shared.Application.Common.Exceptions;

public class UnsupportedColourException(string code)
    : Exception($"Colour \"{code}\" is unsupported.");
