using CoEditor.Domain.Dependencies;
using CoEditor.Domain.Model;
using CoEditor.Domain.UseCase;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace CoEditor.Domain.Tests;

public class GetTemplatesUseCaseTests
{
    [Fact]
    public async Task GetTemplatesAsync()
    {
        // Arrange
        var loggerStub = new NullLoggerFactory().CreateLogger<GetTemplatesUseCase>();
        var templatesStub = new Template
        {
            Id = Guid.NewGuid(),
            UserName = "testUser",
            Language = Language.En,
            DefaultTemplate = false,
            Name = "Name",
            Text = "Text"
        };
        var templateRepositoryMock = new Mock<ITemplateRepository>();
        templateRepositoryMock
            .Setup(x => x.GetTemplatesAsync("testUser", Language.De))
            .ReturnsAsync([templatesStub]);
        var templateService = new GetTemplatesUseCase(
            templateRepositoryMock.Object,
            loggerStub
        );

        // Act
        var result = await templateService.GetTemplatesAsync("testUser", Language.De);

        // Assert
        templateRepositoryMock.Verify(x => x.GetTemplatesAsync("testUser", Language.De), Times.Once);
        Assert.Equal(3, result.Length);
        Assert.Equal(templatesStub, result[0]);
        //TODO: Check System templates as well
    }
}
