namespace PollManagement.Domain.Exceptions;

public class PollClosedException : Exception
{
    public PollClosedException(string message) : base(message) { }
}