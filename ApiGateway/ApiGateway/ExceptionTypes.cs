using System;

namespace ApiGateway
{
    public class NotFoundException : Exception { public NotFoundException(string message) : base(message) { } };
}
