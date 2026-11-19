namespace SplitwiseClone.Exceptions;

public abstract class DomainException(string message) : Exception(message);

public abstract class NotFoundException(string entity, object id)
    : DomainException($"{entity} with ID {id} was not found.");

public abstract class ConflictException(string message) : DomainException(message);

public class ValidationException(IDictionary<string, string[]> errors)
    : DomainException("One or more validation errors occurred.")
{
    public IDictionary<string, string[]> Errors { get; } = errors;
}

public class GroupNotFoundException(int groupId) : NotFoundException("Group", groupId);

public class UserNotInGroupException(int userId, int groupId)
    : ConflictException($"User with ID {userId} is not a member of group {groupId}.");