//using FluentValidation;
//using Newegg.API.Validation;
//using Newegg.MIS.API.EggRolls.RequestEntities;

//namespace Newegg.MIS.API.EggRolls.Validation
//{
//    public class QuesAnsRequestValidator : CustomerValidator<QuesAnsRequest>
//    {
//        public QuesAnsRequestValidator()
//        {
//            RuleSet(ApplyTo.Get, () =>
//            {
//                RuleFor(dto => dto.QuestionnaireID, true)
//                    .Must(QuestionnaireIDIsValid)
//                    .OverridePropertyName("QuestionnaireID")
//                    .WithMessage("QuestionnaireID must is Non-negative");

//                RuleFor(dto => dto.ShortName, true)
//                    .Must(ShortNameIsValid)
//                    .OverridePropertyName("ShortName")
//                    .WithMessage("ShortName is illegal");
//            });

//            RuleSet(ApplyTo.Post, () =>
//            {
//                RuleFor(dto => dto.QuestionnaireID, true)
//                    .Must(QuestionnaireIDIsValid)
//                    .OverridePropertyName("QuestionnaireID")
//                    .WithMessage("QuestionnaireID must is Non-negative");

//                RuleFor(dto => dto.Department, true)
//                    .Must(DepartmentIsValid)
//                    .OverridePropertyName("Department")
//                    .WithMessage("Department is illegal");
//            });
//        }

//        public static bool QuestionnaireIDIsValid(int? QuestionnaireID)
//        {
//            if (QuestionnaireID == null || QuestionnaireID < 0) return false;
//            else return true;
//        }

//        public static bool ShortNameIsValid(string ShortName)
//        {
//            if (ShortName == null || ShortName == "") return false;
//            else return true;
//        }

//        public static bool DepartmentIsValid(string Department)
//        {
//            string DepartmentPattern = "^[A-Z]*\\s[A-Z]*\\s[A-Z]*\\s[A-Z]*$";
//            if (System.Text.RegularExpressions.Regex.IsMatch(Department, DepartmentPattern))
//            {
//                return true;
//            }
//            return false;
//        }
//    }
//}
