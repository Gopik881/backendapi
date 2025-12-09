using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Company.Queries.GetCompany5TabDetailsByCompanyId.GetCompositeCompany5Tab;
using MediatR;
using Moq;
using Xunit;

public class GetCompositeCompany5TabCommandHandlerTests
{
    //[Fact]
    //public async Task Handle_ReturnsCombinedDto()
    //{
    //    var mediator = new Mock<IMediator>();
    //    mediator.Setup(m => m.Send(It.IsAny<IRequest<Company5TabCompanyDto>>(), default)).ReturnsAsync(new Company5TabCompanyDto());
    //    mediator.Setup(m => m.Send(It.IsAny<IRequest<Company5TabAccountDto>>(), default)).ReturnsAsync(new Company5TabAccountDto());
    //    mediator.Setup(m => m.Send(It.IsAny<IRequest<Company5TabCompanyAdminDto>>(), default)).ReturnsAsync(new Company5TabCompanyAdminDto());
    //    mediator.Setup(m => m.Send(It.IsAny<IRequest<object>>(), default)).ReturnsAsync(new object());

    //    var handler = new GetCompositeCompany5TabCommandHandler(mediator.Object);
    //    var result = await handler.Handle(new GetCompositeCompany5TabCommand(1, 2, false), CancellationToken.None);

    //    Assert.NotNull(result);
    //    Assert.NotNull(result.Company5TabCompanyDto);
    //    Assert.NotNull(result.Company5TabAccountDto);
    //    Assert.NotNull(result.Company5TabCompanyAdminDto);
    //}
}