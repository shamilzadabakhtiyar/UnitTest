using JobApplicationLibrary.Models;
using JobApplicationLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationLibrary
{
    public class ApplicationEvaluator
    {
        private const int minAge = 18;
        private const int autoAcceptedYearOfExperience = 15;
        private List<string> techStackList = new List<string>() { "C#", "RabbitMQ", "Microservice", "Visual Studio" };
        private readonly IIdentityValidator _identityValidator;

        public ApplicationEvaluator(IIdentityValidator identityValidator)
        {
            _identityValidator = identityValidator;
        }

        public ApplicationResult Evaluate(JobApplication form)
        {
            if(form.Applicant is null)
                throw new ArgumentNullException();

            if (form.Applicant.Age < minAge)
                return ApplicationResult.AutoRejected;

            _identityValidator.ValidationMode = form.Applicant.Age > 50 ? ValidationMode.Detailed : ValidationMode.Quick;

            if (_identityValidator.CountryDataProvider.CountryData.Country != "Azerbaijan")
                return ApplicationResult.TransferredToCto;

            var validIdentity = _identityValidator.IsValid(form.Applicant.IdentityNumber);
            if (!validIdentity)
                return ApplicationResult.TransferredToHr;

            var sr = GetTechStackSimilarityRate(form.TechStackList);

            if (sr < 25)
                return ApplicationResult.AutoRejected;

            if (sr > 75 && form.YearsOfExperience >= autoAcceptedYearOfExperience)
                return ApplicationResult.AutoAccepted;

            return ApplicationResult.AutoAccepted;
        }

        private int GetTechStackSimilarityRate(List<string> techStacks)
        {
            var matchedCount = techStacks
                .Where(i => techStackList.Contains(i, StringComparer.OrdinalIgnoreCase))
                .Count();

            return (int)((double)matchedCount / techStackList.Count * 100);
        }
    }

    public enum ApplicationResult
    {
        AutoRejected,
        TransferredToHr,
        TransferredToLead,
        TransferredToCto,
        AutoAccepted,
    }
}
