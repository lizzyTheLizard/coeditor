namespace CoEditor.Integration.Cosmos;

public class CosmosException(string message) : Exception(message)
{
    public static CosmosException NotFoundException(Type type, Guid id)
    {
        return new CosmosException($"{type.Name}:{id} was not found");
    }

    public static CosmosException AlreadyPresentException(Type type, Guid id)
    {
        return new CosmosException($"{type.Name}:{id} already exists");
    }
}
