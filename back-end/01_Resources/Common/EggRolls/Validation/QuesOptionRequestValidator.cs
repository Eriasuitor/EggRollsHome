//using FluentValidation;
//using Newegg.API.Validation;
//using Newegg.MIS.API.EggRolls.RequestEntities;
//using System.Text.RegularExpressions;

//namespace Newegg.MIS.API.EggRolls.Validation
//{
//    public class QuesOptionRequestValidator : CustomerValidator<QuesOptionPartsCountByOidRequest>
//    {
//        public QuesOptionRequestValidator()
//        {
//            RuleSet(ApplyTo.Get, () =>
//            {
//                RuleFor(dto => dto.QuestionnaireID, true)
//                .GreaterThan(0)
//                .WithMessage("'QuestionnaireID' must be greater than '0'");
//                RuleFor(dto => dto.TopicID, true)
//                .GreaterThan(0)
//               .WithMessage("'TopicID' must be greater than '0'");

//                RuleFor(dto => dto.OptionID, true)
//               .NotNullorEmpty()
//               .Must(OptionIDValid)
//              .WithMessage("'OptionID' must be a-z or A-Z");
//            });
//        }

//        public bool OptionIDValid(string OptionID)
//        {
//            if(OptionID.Length == 1)
//            return Regex.IsMatch(OptionID, @"[A-Z]|[a-z]");
//            return false;
//        }
//    }
//}
