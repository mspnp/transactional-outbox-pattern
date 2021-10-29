using Contacts.Application.Commands;
using Contacts.Application.Commands.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace Contacts.Tests
{
    [TestCaseOrderer("Contacts.Tests.TestInfra.AlphabeticalOrderer", "Contacts.Tests")]
    public class ValidatorsTest
    {
        [Fact]
        public void Test_001_CreateContactCommandValidator_InvalidProperties()
        {
            var c = new CreateContactCommand
                { FirstName = "", LastName = "", Email = "", Description = "", CompanyName = "" };
            var v = new CreateContactCommandValidator();
            var r = v.TestValidate(c);
            r.ShouldHaveValidationErrorFor(x => x.FirstName);
            r.ShouldHaveValidationErrorFor(x => x.LastName);
            r.ShouldHaveValidationErrorFor(x => x.Email);
            r.ShouldHaveValidationErrorFor(x => x.CompanyName);
        }

        [Fact]
        public void Test_002_CreateContactCommandValidator_ValidProperties()
        {
            var c = new CreateContactCommand
            {
                FirstName = "Christian", LastName = "Dennig", Email = "cd@test.com", Description = "",
                CompanyName = "Test Company"
            };
            var v = new CreateContactCommandValidator();
            var r = v.TestValidate(c);
            r.ShouldNotHaveValidationErrorFor(x => x.FirstName);
            r.ShouldNotHaveValidationErrorFor(x => x.LastName);
            r.ShouldNotHaveValidationErrorFor(x => x.Email);
            r.ShouldNotHaveValidationErrorFor(x => x.CompanyName);
        }
    }
}