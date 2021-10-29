using System;
using Contacts.Application.Commands;
using Contacts.Application.Queries;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Contacts.Tests.TestInfra;
using MediatR;

namespace Contacts.Tests
{
    [TestCaseOrderer("Contacts.Tests.TestInfra.AlphabeticalOrderer", "Contacts.Tests")]
    public class ContactsCommandsTest : IClassFixture<ContactsCommandsTestFixture>
    {
        private readonly ContactsCommandsTestFixture _fixture;

        public ContactsCommandsTest(ContactsCommandsTestFixture fixture)
        {
            this._fixture = fixture;
        }

        [Fact]
        public async void Test_001_Create_Contact()
        {
            var mediator = _fixture.Provider.GetService<IMediator>();
            var res = await mediator.Send(new CreateContactCommand
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "jd@example.com",
                    Description = "",
                    CompanyName = "Example"
                }
            );
            Assert.NotNull(res);
            Assert.NotEqual(res.Id, Guid.Empty);
            Assert.NotEmpty(res.Etag);
        }

        [Fact]
        public async void Test_002_Delete_Contact()
        {
            var mediator = _fixture.Provider.GetService<IMediator>();
            var res =
                await mediator.Send(new DeleteContactCommand { Id = Guid.NewGuid(), Etag = null });
            Assert.NotNull(res);
            Assert.NotEqual(res.Id, Guid.Empty);
            Assert.NotEmpty(res.Etag);
        }

        [Fact]
        public async void Test_003_UpdateCompany_Contact()
        {
            var mediator = _fixture.Provider.GetService<IMediator>();
            var res =
                await mediator.Send(new UpdateContactCompanyCommand
                {
                    Id = Guid.NewGuid(),
                    Etag = null,
                    CompanyName = "NewComp",
                    Street = "Street",
                    HouseNumber = "232",
                    PostalCode = "12312",
                    City = "Palo Alto",
                    Country = "US"
                });
            Assert.NotNull(res);
            Assert.NotEqual(res.Id, Guid.Empty);
            Assert.NotEmpty(res.Etag);
        }

        [Fact]
        public async void Test_004_UpdateDescription_Contact()
        {
            var mediator = _fixture.Provider.GetService<IMediator>();
            var res =
                await mediator.Send(new UpdateContactDescriptionCommand
                {
                    Id = Guid.NewGuid(),
                    Etag = null,
                    Description = "New description"
                });
            Assert.NotNull(res);
            Assert.NotEqual(res.Id, Guid.Empty);
            Assert.NotEmpty(res.Etag);
        }

        [Fact]
        public async void Test_005_UpdateEmail_Contact()
        {
            var mediator = _fixture.Provider.GetService<IMediator>();
            var res =
                await mediator.Send(new UpdateContactEmailCommand
                {
                    Id = Guid.NewGuid(),
                    Etag = null,
                    Email = "jd@ex.com"
                });
            Assert.NotNull(res);
            Assert.NotEqual(res.Id, Guid.Empty);
            Assert.NotEmpty(res.Etag);
        }

        [Fact]
        public async void Test_006_UpdateName_Contact()
        {
            var mediator = _fixture.Provider.GetService<IMediator>();
            var res =
                await mediator.Send(new UpdateContactNameCommand()
                {
                    Id = Guid.NewGuid(),
                    Etag = null,
                    FirstName = "Jim",
                    LastName = "Stark"
                });
            Assert.NotNull(res);
            Assert.NotEqual(res.Id, Guid.Empty);
            Assert.NotEmpty(res.Etag);
        }

        [Fact]
        public async void Test_007_Read_Contact()
        {
            var mediator = _fixture.Provider.GetService<IMediator>();
            var res =
                await mediator.Send(new ReadContactQuery
                {
                    Id = Guid.NewGuid(),
                    Etag = null
                });
            Assert.NotNull(res);
            Assert.NotNull(res.Id);
            Assert.NotEmpty(res.Etag);
        }

        [Fact]
        public async void Test_007_Read_All_Contact()
        {
            var mediator = _fixture.Provider.GetService<IMediator>();
            var res =
                await mediator.Send(new ReadAllContactsQuery { PageSize = 10, ContinuationToken = null });
            Assert.NotNull(res);
            Assert.NotNull(res.Items);
        }
    }
}