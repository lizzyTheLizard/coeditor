using CoEditor.Domain.Model;
using CoEditor.Domain.Outgoing;
using Moq;

namespace CoEditor.Domain.Tests;

public class TemplateServiceTests
{
    [Fact]
    public async Task GetTemplatesAsync()
    {
        // Arrange
        Template[] templatesStub = [new() { Id = Guid.NewGuid(), Name = "Name", Text = "Text" }];
        var templateRepository = new Mock<ITemplateRepository>();
        templateRepository.Setup(x => x.GetTemplatesAsync("testUser", Language.DE)).ReturnsAsync(templatesStub);
        var templateService = new TemplateService(templateRepository.Object);

        // Act
        var result = await templateService.GetTemplatesAsync(Language.DE, "testUser");

        // Assert
        Assert.Equal(templatesStub, result);
        templateRepository.Verify(x => x.GetTemplatesAsync("testUser", Language.DE), Times.Once);
    }
}
