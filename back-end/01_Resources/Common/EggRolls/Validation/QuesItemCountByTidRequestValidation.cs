
//using FluentValidation;
//using Newegg.API.Validation;
//using Newegg.MIS.API.EggRolls.RequestEntities;

//namespace Newegg.MIS.API.EggRolls.Validation
//{
//    public class QuesItemCountByTidRequestValidation : CustomerValidator<QuesItemCountByTidRequest>
//    {
//        public QuesItemCountByTidRequestValidation()
//        {
//            RuleSet(ApplyTo.Get, () =>
//            {
//                RuleFor(dto => dto.QuestionnaireID, true).GreaterThan(0)
//                .WithMessage("'QuestionnaireID' must be greater than '0'");

//                RuleFor(dto => dto.TopicID, true).GreaterThan(0)
//               .WithMessage("'TopicID' must be greater than '0'");
//            });

//        }
//    }
//}
