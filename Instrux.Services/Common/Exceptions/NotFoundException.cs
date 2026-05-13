namespace Instrux.Services.Common.Exceptions;

public class NotFoundException(string name, object key)
    : Exception($"{name} ({key}) not found.")
{
}
