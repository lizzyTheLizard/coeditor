namespace CoEditor.Integration.Cosmos;

public class CosmosException(string message) : Exception(message);

public class NotFoundException(Type type, Guid id) : CosmosException($"{type.Name}:{id} was not found");

public class AlreadyPresentException(Type type, Guid id) : CosmosException($"{type.Name}:{id} already exists");
