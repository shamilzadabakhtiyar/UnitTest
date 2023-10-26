using FluentAssertions;
using JobApplicationLibrary.Models;
using JobApplicationLibrary.Services;
using Moq;

namespace JobApplicationLibrary.UnitTest
{
    public class ApplicationEvaluateUnitTest
    {
        // UnitOfWork_Condition_ExpectedResult

        [Test]
        public void Application_WithUnderAge_TransferredToAutoReject()
        {
            // Arrange
            var evaluator = new ApplicationEvaluator(null);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 17
                }
            };

            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert
            //Assert.AreEqual(ApplicationResult.AutoRejected, appResult);
            appResult.Should().Be(ApplicationResult.AutoRejected);
        }

        [Test]
        public void Application_WithNoTechStack_TransferredToAutoReject()
        {
            // Arrange
            var mock = new Mock<IIdentityValidator>();
            mock.Setup(i => i.IsValid(It.IsAny<string>())).Returns(true);
            mock.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("Azerbaijan");

            var evaluator = new ApplicationEvaluator(mock.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 18
                },
                TechStackList = new List<string>()
            };

            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert
            //Assert.AreEqual(ApplicationResult.AutoRejected, appResult);
            appResult.Should().Be(ApplicationResult.AutoRejected);
        }

        [Test]
        public void Application_WithTechStackOver75P_TransferredToAutoAccept()
        {
            // Arrange
            var mock = new Mock<IIdentityValidator>();
            mock.Setup(i => i.IsValid(It.IsAny<string>())).Returns(true);
            mock.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("Azerbaijan");

            var evaluator = new ApplicationEvaluator(mock.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 18
                },
                TechStackList = new List<string>() { "C#", "RabbitMQ", "Microservice" }
        };

            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert
            //Assert.AreEqual(ApplicationResult.AutoAccepted, appResult);
            appResult.Should().Be(ApplicationResult.AutoAccepted);
        }

        [Test]
        public void Application_WithInValidindentityNumber_TransferredToHr()
        {
            // Arrange
            var mock = new Mock<IIdentityValidator>();
            mock.Setup(i => i.IsValid(It.IsAny<string>())).Returns(false);
            mock.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("Azerbaijan");

            var evaluator = new ApplicationEvaluator(mock.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 18
                }
            };

            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert
            //Assert.AreEqual(ApplicationResult.TransferredToHr, appResult);
            appResult.Should().Be(ApplicationResult.TransferredToHr);
        }

        [Test]
        public void Application_WithOfficeLocation_TransferredToCto()
        {
            // Arrange
            var mock = new Mock<IIdentityValidator>();
            mock.Setup(i => i.IsValid(It.IsAny<string>())).Returns(false);
            mock.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("Spain");

            var evaluator = new ApplicationEvaluator(mock.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 18
                }
            };

            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert
            //Assert.AreEqual(ApplicationResult.TransferredToCto, appResult);
            appResult.Should().Be(ApplicationResult.TransferredToCto);
        }

        [Test]
        public void Application_WithOver50_ValidationModeToDetailed()
        {
            // Arrange
            var mock = new Mock<IIdentityValidator>();
            mock.SetupAllProperties();
            mock.Setup(i => i.IsValid(It.IsAny<string>())).Returns(false);
            mock.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("Azerbaijan");
            //mock.SetupProperty(i => i.ValidationMode);

            var evaluator = new ApplicationEvaluator(mock.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 51
                }
            };

            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert
            //Assert.AreEqual(ValidationMode.Detailed, mock.Object.ValidationMode);
            mock.Object.ValidationMode.Should().Be(ValidationMode.Detailed);
        }

        [Test]
        public void Application_WithNullApplicant_ThrowsArgumentNullException()
        {
            // Arrange
            var mock = new Mock<IIdentityValidator>();

            var evaluator = new ApplicationEvaluator(mock.Object);
            var form = new JobApplication();

            // Action
            Action appResultAction = () => evaluator.Evaluate(form);

            // Assert
            appResultAction.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Application_WithDefaultValue_IsValidCalled()
        {
            // Arrange
            var mock = new Mock<IIdentityValidator>();
            mock.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("Azerbaijan");

            var evaluator = new ApplicationEvaluator(mock.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 18,
                    IdentityNumber = "123"
                },
                TechStackList = new List<string>()
            };

            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert
            //mock.Verify(i => i.IsValid("12"), "IsValidMethod should be called with 12");
            mock.Verify(i => i.IsValid(It.IsAny<string>()));
        }

        [Test]
        public void Application_WithYoungAge_IsValidNeverCalled()
        {
            // Arrange
            var mock = new Mock<IIdentityValidator>();
            mock.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("Azerbaijan");

            var evaluator = new ApplicationEvaluator(mock.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 17,
                    IdentityNumber = "123"
                },
                TechStackList = new List<string>()
            };

            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert
            //mock.Verify(i => i.IsValid("12"), "IsValidMethod should be called with 12");
            mock.Verify(i => i.IsValid(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void Application_WithYoungAge_IsValidOnceCalled()
        {
            // Arrange
            var mock = new Mock<IIdentityValidator>();
            mock.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("Azerbaijan");

            var evaluator = new ApplicationEvaluator(mock.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 18,
                    IdentityNumber = "123"
                },
                TechStackList = new List<string>()
            };

            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert
            //mock.Verify(i => i.IsValid("12"), "IsValidMethod should be called with 12");
            mock.Verify(i => i.IsValid(It.IsAny<string>()), Times.Once);
        }
    }
}