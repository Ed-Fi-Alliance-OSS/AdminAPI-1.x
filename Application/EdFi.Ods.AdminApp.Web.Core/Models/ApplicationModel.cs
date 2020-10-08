using FluentValidation;

namespace EdFi.Ods.AdminApp.Web.Models
{
    public class ApplicationModel
    {
        public int ApplicationId { get; set; }

        public string ApplicationName { get; set; }

        public string DisplayName { get; set; }

        public string ClaimSetName { get; set; }

        public string ProfileName { get; set; }
    }

    public class ApplicationModelValidator : AbstractValidator<ApplicationModel>
    {
        public ApplicationModelValidator(IApplicationInterface applicationInterface)
        {
            RuleFor(x => x.ApplicationId).NotNull();
            RuleFor(x => x.ApplicationName).Length(5, 10);
            RuleFor(x => x.DisplayName).Length(5, 25);
            RuleFor(x => x.ClaimSetName).NotEmpty().Equal(applicationInterface.GetClaimSet());
            RuleFor(x => x.ProfileName).Length(5, 20);
        }
    }

    public interface IApplicationInterface
    {
        string GetClaimSet();
    }

    public class ApplicationClass: IApplicationInterface
    {
        public string GetClaimSet() => "EdFi-Ods-Sandbox";
    }
}
