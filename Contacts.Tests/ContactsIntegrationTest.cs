using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contacts.Domain;
using Contacts.Infrastructure;
using Contacts.Infrastructure.Context;
using Contacts.Infrastructure.Exceptions;
using Contacts.Tests.TestInfra;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Contacts.Tests
{
    [TestCaseOrderer("Contacts.Tests.TestInfra.AlphabeticalOrderer", "Contacts.Tests")]
    public class ContactsIntegrationTest : IClassFixture<ContactsIntegrationTestFixture>
    {
        private readonly ContactsIntegrationTestFixture _fixture;

        public ContactsIntegrationTest(ContactsIntegrationTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async void Test_001_Create_Contact()
        {
            var uow = _fixture.Provider.GetService<IUnitOfWork>();

            var c = Contact.CreateNew(_fixture.CurrentId);
            c.SetCompany("Example", "Street", "1a", "092821", "Palo Alto", "US");
            c.SetDescription("This is a contact");
            c.SetEmail("jd@example.com");
            c.SetName("John", "Doe");
            if (uow != null)
            {
                uow.ContactsRepo.Create(c);
                var res = await uow.CommitAsync();
                Assert.Equal(2, res.Count);
            }
        }

        [Fact]
        public async void Test_002_Read_Contact()
        {
            var uow = _fixture.Provider.GetService<IUnitOfWork>();
            if (uow != null)
            {
                var (contact, etag) = await uow.ContactsRepo.ReadAsync(_fixture.CurrentId, null);
                Assert.NotNull(contact);
                Assert.NotEmpty(etag);
            }
        }

        [Fact]
        public async void Test_003_UpdateCompany_Contact()
        {
            var uow = _fixture.Provider.GetService<IUnitOfWork>();
            if (uow != null)
            {
                var (contact, etag) = await uow.ContactsRepo.ReadAsync(_fixture.CurrentId, null);

                contact.SetCompany("NewComp", contact.Company.Street, contact.Company.HouseNumber,
                    contact.Company.PostalCode, contact.Company.City, contact.Company.Country);

                uow.ContactsRepo.Update(contact, etag);
                var res = await uow.CommitAsync();
                Assert.Equal(2, res.Count);
            }
        }

        [Fact]
        public async void Test_004_UpdateName_Contact()
        {
            var uow = _fixture.Provider.GetService<IUnitOfWork>();
            if (uow != null)
            {
                var (contact, etag) = await uow.ContactsRepo.ReadAsync(_fixture.CurrentId, null);

                contact.SetName("Jim", "Stark");

                uow.ContactsRepo.Update(contact, etag);
                var res = await uow.CommitAsync();
                Assert.Equal(2, res.Count);
            }
        }

        [Fact]
        public async void Test_005_UpdateDescription_Contact()
        {
            var uow = _fixture.Provider.GetService<IUnitOfWork>();
            if (uow != null)
            {
                var (contact, etag) = await uow.ContactsRepo.ReadAsync(_fixture.CurrentId, null);

                contact.SetDescription("This is a new description");

                uow.ContactsRepo.Update(contact, etag);
                var res = await uow.CommitAsync();
                Assert.Equal(2, res.Count);
            }
        }

        [Fact]
        public async void Test_006_UpdateEmail_Contact()
        {
            var uow = _fixture.Provider.GetService<IUnitOfWork>();
            if (uow != null)
            {
                var (contact, etag) = await uow.ContactsRepo.ReadAsync(_fixture.CurrentId, null);

                contact.SetEmail("jd@ex.com");

                uow.ContactsRepo.Update(contact, etag);
                var res = await uow.CommitAsync();
                Assert.Equal(2, res.Count);
            }
        }

        [Fact]
        public async void Test_007_ReadAll_Contacts()
        {
            var uow = _fixture.Provider.GetService<IUnitOfWork>();
            if (uow != null)
            {
                var (contacts, hasMore, nextToken) =
                    await uow.ContactsRepo.ReadAllAsync(4, "");
                Assert.True(contacts.Count > 0);

                while (hasMore && !string.IsNullOrEmpty(nextToken))
                {
                    (contacts, hasMore, nextToken) =
                        await uow.ContactsRepo.ReadAllAsync(4, nextToken);
                }
            }
        }

        
        [Fact]
        public void Test_008_Create_SameContact_Multi()
        {
            var uow = _fixture.Provider.GetService<IUnitOfWork>();
            var ctx = _fixture.Provider.GetService<IContainerContext>();

            var c = Contact.CreateNew(_fixture.CurrentId);
            c.SetCompany("Example", "Street", "1a", "092821", "Palo Alto", "US");
            c.SetDescription("This is a contact");
            c.SetEmail("jd@example.com");
            c.SetName("John", "Doe");
            if (ctx != null && uow != null)
            {
                uow.ContactsRepo.Create(c);
                uow.ContactsRepo.Create(c);
                Assert.True(ctx.DataObjects.Count == 1);
                ctx.Reset();
            }
        }

        [Fact]
        public async void Test_009_Nothing_To_Save()
        {
            var uow = _fixture.Provider.GetService<IUnitOfWork>();

            if (uow != null)
            {
                var res = await uow.CommitAsync();
                Assert.Empty(res);
            }
        }

        [Fact]
        public async void Test_010_Save_Single()
        {
            var uow = _fixture.Provider.GetService<IUnitOfWork>();
            var ctx = _fixture.Provider.GetService<IContainerContext>();
            var jObj = @"
                {
                    ""name"": {
                        ""firstName"": ""John"",
                        ""lastName"": ""Doe""
                    },
                    ""description"": ""This is a description"",
                    ""email"": ""jd@example.com"",
                    ""company"": {
                        ""companyName"": ""Example"",
                        ""street"": ""Street"",
                        ""houseNumber"": ""1a"",
                        ""postalCode"": ""092821"",
                        ""city"": ""Palo Alto"",
                        ""country"": ""US""
                    },
                    ""id"": ""0001f5bb-d4a9-48ba-af80-3db52bfd7461""
                }";
            var c = JObject.Parse(jObj).ToObject<Contact>();
            if (ctx != null)
                ctx.Add(new DataObject<Contact>(c.Id.ToString(), c.Id.ToString(), "contact",
                    c, null, 10, EntityState.Created));
            if (uow != null)
            {
                var res = await uow.CommitAsync();
                Assert.Single(res);
            }
        }

        [Fact]
        public async void Test_011_Save_Single_Unmodified()
        {
            var uow = _fixture.Provider.GetService<IUnitOfWork>();
            var ctx = _fixture.Provider.GetService<IContainerContext>();
            var jObj = @"
                {
                    ""name"": {
                        ""firstName"": ""John"",
                        ""lastName"": ""Doe""
                    },
                    ""description"": ""This is a description"",
                    ""email"": ""jd@example.com"",
                    ""company"": {
                        ""companyName"": ""Example"",
                        ""street"": ""Street"",
                        ""houseNumber"": ""1a"",
                        ""postalCode"": ""092821"",
                        ""city"": ""Palo Alto"",
                        ""country"": ""US""
                    },
                    ""id"": ""0001f5bb-d4a9-48ba-af80-3db52bfd7462""
                }";
            var c = JObject.Parse(jObj).ToObject<Contact>();
            if (ctx != null)
                ctx.Add(new DataObject<Contact>(c.Id.ToString(), c.Id.ToString(), "contact",
                    c, null, 10));
            if (uow != null)
            {
                var res = await uow.CommitAsync();
                Assert.Empty(res);
            }
        }

        [Fact]
        public async void Test_012_Save_Single_Unmodified_Etag()
        {
            var uow = _fixture.Provider.GetService<IUnitOfWork>();
            var ctx = _fixture.Provider.GetService<IContainerContext>();
            var jObj = @"
                {
                    ""name"": {
                        ""firstName"": ""John"",
                        ""lastName"": ""Doe""
                    },
                    ""description"": ""This is a description"",
                    ""email"": ""jd@example.com"",
                    ""company"": {
                        ""companyName"": ""Example"",
                        ""street"": ""Street"",
                        ""houseNumber"": ""1a"",
                        ""postalCode"": ""092821"",
                        ""city"": ""Palo Alto"",
                        ""country"": ""US""
                    },
                    ""id"": ""0001f5bb-d4a9-48ba-af80-3db52bfd7463""
                }";
            var c = JObject.Parse(jObj).ToObject<Contact>();

            if (ctx != null)
                ctx.Add(new DataObject<Contact>(c.Id.ToString(), c.Id.ToString(), "contact",
                    c, "xyz", 10));

            if (uow != null)
            {
                var res = await uow.CommitAsync();
                Assert.Empty(res);
            }
        }

        [Fact]
        public async void Test_013_Read_NotModified_Contact()
        {
            var uow = _fixture.Provider.GetService<IUnitOfWork>();
            if (uow != null)
            {
                var (contact, etag) = await uow.ContactsRepo.ReadAsync(_fixture.CurrentId, null);
                Assert.NotNull(contact);
                Assert.NotEmpty(etag);

                // Test NotModified exception
                async Task<(Contact, string)> Action() => await uow.ContactsRepo.ReadAsync(contact.Id, etag);

                await Assert.ThrowsAsync<DomainObjectNotModifiedException>(Action);
            }
        }

        [Fact]
        public async void Test_014_MidAir_Collision()
        {
            var uow = _fixture.Provider.GetService<IUnitOfWork>();
            if (uow != null)
            {
                var (contact, etag) = await uow.ContactsRepo.ReadAsync(_fixture.CurrentId, null);

                contact.SetCompany("NewComp", contact.Company.Street, contact.Company.HouseNumber,
                    contact.Company.PostalCode, contact.Company.City, contact.Company.Country);

                // use wrong etag -- provoke mid-air collision
                uow.ContactsRepo.Update(contact, "12");

                async Task<List<IDataObject<Entity>>> Action() => await uow.CommitAsync();

                await Assert.ThrowsAsync<DomainObjectPreconditionFailedException>(Action);
            }
        }
        
        [Fact]
        public async void Test_015_Delete_Contact()
        {
            var uow = _fixture.Provider.GetService<IUnitOfWork>();
            if (uow != null)
            {
                await uow.ContactsRepo.DeleteAsync(_fixture.CurrentId, null);
                var res = await uow.CommitAsync();
                Assert.Equal(2, res.Count);
            }
        }

        [Fact]
        public async void Test_016_Update_Single_MidAir_Collision()
        {
            var uow = _fixture.Provider.GetService<IUnitOfWork>();
            var ctx = _fixture.Provider.GetService<IContainerContext>();
            const string jObj = @"
                {
                    ""name"": {
                        ""firstName"": ""John"",
                        ""lastName"": ""Doe""
                    },
                    ""description"": ""This is a description"",
                    ""email"": ""jd@example.com"",
                    ""company"": {
                        ""companyName"": ""Example"",
                        ""street"": ""Street"",
                        ""houseNumber"": ""1a"",
                        ""postalCode"": ""092821"",
                        ""city"": ""Palo Alto"",
                        ""country"": ""US""
                    },
                    ""id"": ""0001f5bb-d4a9-48ba-af80-3db52bfd7464""
                }";
            var c = JObject.Parse(jObj).ToObject<Contact>();

            ctx?.Add(new DataObject<Contact>(c.Id.ToString(), c.Id.ToString(), "contact",
                c, null, 10, EntityState.Created));

            if (uow == null) return;

            var res = await uow.CommitAsync();
            Assert.Single(res);

            ctx.Add(new DataObject<Contact>(c.Id.ToString(), c.Id.ToString(), "contact", c,
                "123", 5, EntityState.Updated));

            async Task<List<IDataObject<Entity>>> Action() => await uow.CommitAsync();

            await Assert.ThrowsAsync<DomainObjectPreconditionFailedException>(Action);
        }

        [Fact]
        public async void Test_017_Delete_Single_MidAir_Collision()
        {
            var uow = _fixture.Provider.GetService<IUnitOfWork>();
            var ctx = _fixture.Provider.GetService<IContainerContext>();
            const string jObj = @"
                {
                    ""name"": {
                        ""firstName"": ""John"",
                        ""lastName"": ""Doe""
                    },
                    ""description"": ""This is a description"",
                    ""email"": ""jd@example.com"",
                    ""company"": {
                        ""companyName"": ""Example"",
                        ""street"": ""Street"",
                        ""houseNumber"": ""1a"",
                        ""postalCode"": ""092821"",
                        ""city"": ""Palo Alto"",
                        ""country"": ""US""
                    },
                    ""id"": ""0001f5bb-d4a9-48ba-af80-3db52bfd7465""
                }";
            var c = JObject.Parse(jObj).ToObject<Contact>();

            ctx?.Add(new DataObject<Contact>(c.Id.ToString(), c.Id.ToString(), "contact",
                c, null, 10, EntityState.Created));

            if (uow != null)
            {
                var res = await uow.CommitAsync();
                Assert.Single(res);

                ctx.Add(new DataObject<Contact>(c.Id.ToString(), c.Id.ToString(), "contact", c,
                    "123", 5, EntityState.Deleted));

                async Task<List<IDataObject<Entity>>> Action() => await uow.CommitAsync();

                await Assert.ThrowsAsync<DomainObjectPreconditionFailedException>(Action);
            }
        }

        [Fact]
        public async void Test_018_Update_Single_NotFound()
        {
            var uow = _fixture.Provider.GetService<IUnitOfWork>();
            var ctx = _fixture.Provider.GetService<IContainerContext>();
            const string jObj = @"
                {
                    ""name"": {
                        ""firstName"": ""John"",
                        ""lastName"": ""Doe""
                    },
                    ""description"": ""This is a description"",
                    ""email"": ""jd@example.com"",
                    ""company"": {
                        ""companyName"": ""Example"",
                        ""street"": ""Street"",
                        ""houseNumber"": ""1a"",
                        ""postalCode"": ""092821"",
                        ""city"": ""Palo Alto"",
                        ""country"": ""US""
                    },
                    ""id"": ""0001f5bb-d4a9-48ba-af80-3db52bfd7466""
                }";
            var c = JObject.Parse(jObj).ToObject<Contact>();

            ctx?.Add(new DataObject<Contact>(c.Id.ToString(), c.Id.ToString(), "contact",
                c, null, 10, EntityState.Created));

            if (uow != null)
            {
                var res = await uow.CommitAsync();
                Assert.Single(res);

                ctx.Add(new DataObject<Contact>("0001f5bb-d4a9-48ba-af80-3db52bfd7459",
                    "0001f5bb-d4a9-48ba-af80-3db52bfd7459", "contact", c,
                    "123", 5, EntityState.Updated));

                async Task<List<IDataObject<Entity>>> Action() => await uow.CommitAsync();

                await Assert.ThrowsAsync<DomainObjectNotFoundException>(Action);
            }
        }

        [Fact]
        public async void Test_019_Create_Single_Conflict()
        {
            var uow = _fixture.Provider.GetService<IUnitOfWork>();
            var ctx = _fixture.Provider.GetService<IContainerContext>();
            var jObj = @"
                {
                    ""name"": {
                        ""firstName"": ""John"",
                        ""lastName"": ""Doe""
                    },
                    ""description"": ""This is a description"",
                    ""email"": ""jd@example.com"",
                    ""company"": {
                        ""companyName"": ""Example"",
                        ""street"": ""Street"",
                        ""houseNumber"": ""1a"",
                        ""postalCode"": ""092821"",
                        ""city"": ""Palo Alto"",
                        ""country"": ""US""
                    },
                    ""id"": ""0001f5bb-d4a9-48ba-af80-3db52bfd7455""
                }";
            var c = JObject.Parse(jObj).ToObject<Contact>();
            if (ctx != null)
                ctx.Add(new DataObject<Contact>(c.Id.ToString(), c.Id.ToString(), "contact",
                    c, null, 10, EntityState.Created));
            if (uow != null)
            {
                await uow.CommitAsync();
                // Create same document again...
                var d = JObject.Parse(jObj).ToObject<Contact>();
                ctx.Add(new DataObject<Contact>(d.Id.ToString(), d.Id.ToString(), "contact",
                    d, null, 10, EntityState.Created));

                async Task<List<IDataObject<Entity>>> Action() => await uow.CommitAsync();

                await Assert.ThrowsAsync<DomainObjectConflictException>(Action);
            }
        }

        [Fact]
        public async void Test_020_Delete_Single()
        {
            var uow = _fixture.Provider.GetService<IUnitOfWork>();
            var ctx = _fixture.Provider.GetService<IContainerContext>();
            var jObj = @"
                {
                    ""name"": {
                        ""firstName"": ""John"",
                        ""lastName"": ""Doe""
                    },
                    ""description"": ""This is a description"",
                    ""email"": ""jd@example.com"",
                    ""company"": {
                        ""companyName"": ""Example"",
                        ""street"": ""Street"",
                        ""houseNumber"": ""1a"",
                        ""postalCode"": ""092821"",
                        ""city"": ""Palo Alto"",
                        ""country"": ""US""
                    },
                    ""id"": ""0001f5bb-d4a9-48ba-af80-3db52bfd7422""
                }";
            var c = JObject.Parse(jObj).ToObject<Contact>();
            if (ctx != null)
                ctx.Add(new DataObject<Contact>(c.Id.ToString(), c.Id.ToString(), "contact",
                    c, null, 10, EntityState.Created));
            if (uow != null)
            {
                var res = await uow.CommitAsync();
                Assert.Single(res);

                c.SetDeleted();
                ctx.Add(new DataObject<Contact>(c.Id.ToString(), c.Id.ToString(), "contact", c, res[0].Etag, 5,
                    EntityState.Deleted));
                res.Clear();
                res = await uow.CommitAsync();
                Assert.Equal(2, res.Count);
            }
        }

        [Fact]
        public async void Test_021_Read_Single_NotModified()
        {
            var uow = _fixture.Provider.GetService<IUnitOfWork>();
            var ctx = _fixture.Provider.GetService<IContainerContext>();
            const string jObj = @"
                {
                    ""name"": {
                        ""firstName"": ""John"",
                        ""lastName"": ""Doe""
                    },
                    ""description"": ""This is a description"",
                    ""email"": ""jd@example.com"",
                    ""company"": {
                        ""companyName"": ""Example"",
                        ""street"": ""Street"",
                        ""houseNumber"": ""1a"",
                        ""postalCode"": ""092821"",
                        ""city"": ""Palo Alto"",
                        ""country"": ""US""
                    },
                    ""id"": ""0001f5bb-d4a9-48ba-af80-3db52bfd7470""
                }";
            var c = JObject.Parse(jObj).ToObject<Contact>();

            ctx?.Add(new DataObject<Contact>(c.Id.ToString(), c.Id.ToString(), "contact",
                c, null, 10, EntityState.Created));

            if (uow != null)
            {
                var res = await uow.CommitAsync();
                Assert.Single(res);

                async Task<(Contact, string)> Action() => await uow.ContactsRepo.ReadAsync(c.Id, res[0].Etag);

                await Assert.ThrowsAsync<DomainObjectNotModifiedException>(Action);
            }
        }

        [Fact]
        public async void Test_022_Create_Contact_Conflict()
        {
            var uow = _fixture.Provider.GetService<IUnitOfWork>();

            var c = Contact.CreateNew(_fixture.CurrentId);
            c.SetCompany("Example", "Street", "1a", "092821", "Palo Alto", "US");
            c.SetDescription("This is a contact");
            c.SetEmail("jd@example.com");
            c.SetName("John", "Doe");
            if (uow != null)
            {
                uow.ContactsRepo.Create(c);

                async Task<List<IDataObject<Entity>>> Action() => await uow.CommitAsync();

                await Assert.ThrowsAsync<DomainObjectConflictException>(Action);
            }
        }

        [Fact]
        public async void Test_023_Delete_Contact()
        {
            var uow = _fixture.Provider.GetService<IUnitOfWork>();
            var id = Guid.NewGuid();
            var c = Contact.CreateNew(id);
            c.SetCompany("Example", "Street", "1a", "092821", "Palo Alto", "US");
            c.SetDescription("This is a contact");
            c.SetEmail("jd@example.com");
            c.SetName("John", "Doe");
            if (uow != null)
            {
                uow.ContactsRepo.Create(c);

                var res = await uow.CommitAsync();
                var contactResult = res.FirstOrDefault(r => r is DataObject<Contact>);

                await uow.ContactsRepo.DeleteAsync(c.Id, contactResult?.Etag);
            }
        }

        [Fact]
        public async void Test_024_Delete_Contact_NotFound()
        {
            var uow = _fixture.Provider.GetService<IUnitOfWork>();

            async Task Action() => await uow.ContactsRepo.DeleteAsync(Guid.NewGuid(), null);

            await Assert.ThrowsAsync<DomainObjectNotFoundException>(Action);
        }
    }
}