using CoEditor.Domain.Dependencies;
using CoEditor.Domain.Model;
using CoEditor.Domain.UseCase;
using Microsoft.Extensions.Logging;
using Moq;

namespace CoEditor.Domain.Tests;

public class GetTemplatesUseCaseTests
{
    [Fact]
    public async Task GetTemplatesAsync()
    {
        // Arrange
        var loggerStub = new Mock<ILogger<GetTemplatesUseCase>>();
        Template[] templatesStub = [new() { Id = Guid.NewGuid(), Name = "Name", Text = "Text" }];
        var templateRepositoryMock = new Mock<ITemplateRepository>();
        templateRepositoryMock
            .Setup(x => x.GetTemplatesAsync("testUser", Language.De))
            .ReturnsAsync(templatesStub);
        var templateService = new GetTemplatesUseCase(
            templateRepositoryMock.Object,
            loggerStub.Object
        );

        // Act
        var result = await templateService.GetTemplatesAsync("testUser", Language.De);

        // Assert
        Assert.Equal(templatesStub, result);
        templateRepositoryMock.Verify(x => x.GetTemplatesAsync("testUser", Language.De), Times.Once);
    }
}
