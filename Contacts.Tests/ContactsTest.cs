using System;
using Contacts.Domain;
using Contacts.Domain.Events;
using Contacts.Tests.TestInfra;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Contacts.Tests;

[TestCaseOrderer("Contacts.Tests.TestInfra.AlphabeticalOrderer", "Contacts.Tests")]
public class ContactsTest : IClassFixture<ContactsTestFixture>
{
    public ContactsTestFixture Fixture { get; }

    public ContactsTest(ContactsTestFixture fixture)
    {
        Fixture = fixture;
    }

    [Fact]
    public void Test_001_Create_Contact_WithId_Has_CreatedEvent()
    {
        var c = Contact.CreateNew(Guid.Parse("0001f5bb-d4a9-48ba-af80-3db52bfd7468"));
        Assert.True(c.DomainEvents.Count == 1);
        Assert.True(typeof(ContactCreatedEvent) == c.DomainEvents[0].GetType());
    }

    [Fact]
    public void Test_002_Create_Contact_Has_CreatedEvent()
    {
        var c = Contact.CreateNew();
        Assert.True(c.DomainEvents.Count == 1);
        Assert.True(typeof(ContactCreatedEvent) == c.DomainEvents[0].GetType());
    }

    [Fact]
    public void Test_003_Create_From_Payload_Has_Zero_DomainEvents()
    {
        var test = JObject.Parse(Fixture.JsonObject).ToObject<Contact>();
        Assert.NotNull(test);
        Assert.True(test.DomainEvents.Count == 0);
    }

    [Fact]
    public void Test_004_UpdateCompany_Has_DomainEvent()
    {
        var test = JObject.Parse(Fixture.JsonObject).ToObject<Contact>();
        Assert.NotNull(test);
        test.SetCompany("Test", test.Company.Street, test.Company.HouseNumber, test.Company.PostalCode,
            test.Company.City, test.Company.Country);
        Assert.True(test.DomainEvents.Count == 1);
        Assert.True(typeof(ContactCompanyUpdatedEvent) == test.DomainEvents[0].GetType());
    }

    [Fact]
    public void Test_005_UpdateCompany_Throws_Without_Name()
    {
        var test = JObject.Parse(Fixture.JsonObject).ToObject<Contact>();
        Assert.NotNull(test);

        void Act() => test.SetCompany("", test.Company.Street, test.Company.HouseNumber, test.Company.PostalCode,
            test.Company.City, test.Company.Country);

        Assert.Throws<ArgumentException>(Act);
        Assert.True(test.DomainEvents.Count == 0);
    }

    [Fact]
    public void Test_006_UpdateName_Has_DomainEvent()
    {
        var test = JObject.Parse(Fixture.JsonObject).ToObject<Contact>();
        Assert.NotNull(test);
        test.SetName("Test", "Test");
        Assert.True(test.DomainEvents.Count == 1);
        Assert.True(typeof(ContactNameUpdatedEvent) == test.DomainEvents[0].GetType());
    }

    [Fact]
    public void Test_007_UpdateName_Throws_Without_FirstName()
    {
        var test = JObject.Parse(Fixture.JsonObject).ToObject<Contact>();
        Assert.NotNull(test);
        void Act() => test.SetName("", "");
        Assert.Throws<ArgumentException>(Act);
        Assert.True(test.DomainEvents.Count == 0);
    }

    [Fact]
    public void Test_008_RemoveAllEvents()
    {
        var test = JObject.Parse(Fixture.JsonObject).ToObject<Contact>();
        Assert.NotNull(test);
        test.SetName("Test", "Test");
        Assert.True(test.DomainEvents.Count == 1);
        test.RemoveAllEvents();
        Assert.True(test.DomainEvents.Count == 0);
    }

    [Fact]
    public void Test_009_RemoveSingleEvent()
    {
        var test = JObject.Parse(Fixture.JsonObject).ToObject<Contact>();
        Assert.NotNull(test);
        test.SetName("Test", "Test");
        Assert.True(test.DomainEvents.Count == 1);
        var evt = test.DomainEvents[0];
        test.RemoveEvent(evt);
        Assert.True(test.DomainEvents.Count == 0);
    }

    [Fact]
    public void Test_010_UpdateObject_SameVO_Multiple_Replaces_DomainEvent()
    {
        var test = JObject.Parse(Fixture.JsonObject).ToObject<Contact>();
        Assert.NotNull(test);
        test.SetName("Test", "Test");
        Assert.True(test.DomainEvents.Count == 1);
        Assert.True(typeof(ContactNameUpdatedEvent) == test.DomainEvents[0].GetType());
        test.SetName("Another Test", "Another Test");
        Assert.True(test.DomainEvents.Count == 1);
        Assert.True(typeof(ContactNameUpdatedEvent) == test.DomainEvents[0].GetType());
    }

    [Fact]
    public void Test_011_UpdateObject_Multiple_Adds_DomainEvent()
    {
        var test = JObject.Parse(Fixture.JsonObject).ToObject<Contact>();
        Assert.NotNull(test);
        test.SetName("Test", "Test");
        Assert.True(test.DomainEvents.Count == 1);
        Assert.True(typeof(ContactNameUpdatedEvent) == test.DomainEvents[0].GetType());
        test.SetEmail("a@b.com");
        Assert.True(test.DomainEvents.Count == 2);
        Assert.True(typeof(ContactEmailUpdatedEvent) == test.DomainEvents[1].GetType());
    }

    [Fact]
    public void Test_012_UpdateObject_Multiple_Adds_And_SameVO_DomainEvent()
    {
        var test = JObject.Parse(Fixture.JsonObject).ToObject<Contact>();
        Assert.NotNull(test);
        test.SetName("Test", "Test");
        Assert.True(test.DomainEvents.Count == 1);
        Assert.True(typeof(ContactNameUpdatedEvent) == test.DomainEvents[0].GetType());
        test.SetEmail("a@b.com");
        Assert.True(test.DomainEvents.Count == 2);
        Assert.True(typeof(ContactEmailUpdatedEvent) == test.DomainEvents[1].GetType());
        test.SetEmail("x@y.com");
        Assert.True(test.DomainEvents.Count == 2);
        Assert.True(typeof(ContactEmailUpdatedEvent) == test.DomainEvents[1].GetType());
        Assert.True(((ContactEmailUpdatedEvent)test.DomainEvents[1]).Email == "x@y.com");
    }
}