
//using FluentValidation;
//using Newegg.API.Validation;
//using Newegg.MIS.API.EggRolls.RequestEntities;
//using System;

//namespace Newegg.MIS.API.EggRolls.Validation
//{

//    public class QuesItemBasicRequestValidator : CustomerValidator<QuesItemBasicRequest>
//    {
//        public QuesItemBasicRequestValidator()
//        {
//                RuleSet(ApplyTo.Put, () =>
//                {
//                    RuleFor(dto => dto.QuestionnaireID, true).GreaterThan(0)
//                    .WithMessage("'QuestionnaireID' must be greater than '0'");

//                    RuleFor(dto => dto.Status, true).Must(StatusIsValid)
//                    .WithMessage("Status must be 0 [draft] or 1 [processing]");

//                    RuleFor(dto => dto.Title,true).NotNullorEmpty()
//                    .WithMessage("Title can not be null or empty");
//                });
//                RuleSet(ApplyTo.Delete, () =>
//                {
//                    RuleFor(dto => dto.QuestionnaireID, true).GreaterThan(0)
//                    .WithMessage("'QuestionnaireID' must be greater than '0'");
//                });
//            RuleSet(ApplyTo.Get, () =>
//             {
//                 RuleFor(dto => dto.QuestionnaireID, true).GreaterThan(0)
//                 .WithMessage("'QuestionnaireID' must be greater than '0'");
//             });
//        }

//        private bool StatusIsValid(int status)
//        {
//            if (status == 0 || status == 1) return true;
//            else return false;
//        }
//    }
//}
