using FluentValidation;
using Newegg.API.Validation;
using Newegg.MIS.API.EggRolls.RequestEntities;


namespace Newegg.MIS.API.EggRolls.Validation
{
    public class EMailRequestValidator : CustomerValidator<EMailRequest>
    {
        public EMailRequestValidator()
        {
            RuleSet(ApplyTo.Post, () =>
            {
                RuleFor(dto => dto.From, true)
                    .Must(FromIsValid)
                    .OverridePropertyName("From")
                    .WithMessage("From is illegal");

                RuleFor(dto => dto.To, true)
                    .Must(ToIsValid)
                    .OverridePropertyName("To")
                    .WithMessage("To is illegal");
            });
        }

        public static bool FromIsValid(string From)
        {
            string FromPattern = "^[A-Za-z]*\\.[A-Za-z]*\\.[A-Za-z]*$";
            if (System.Text.RegularExpressions.Regex.IsMatch(From, FromPattern))
            {
                return true;
            }
            return false;
        }

        public static bool ToIsValid(string To)
        {
            string ToPattern = "[\\w!#$%&'*+/=?^_`{|}~-]+(?:\\.[\\w!#$%&'*+/=?^_`{|}~-]+)*@(?:[\\w](?:[\\w-]*[\\w])?\\.)+[\\w](?:[\\w-]*[\\w])?";
            if (To.Contains(";"))
            {
                var TosList = To.Split(';');
                foreach (string itemTo in TosList)
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(itemTo, ToPattern))
                    {
                        return false;
                    }
                }
                return true;
            }
            else if (To == null || To == "")
            {
                return false;
            }
            else if (System.Text.RegularExpressions.Regex.IsMatch(To, ToPattern))
            {
                if (To.Substring(0, To.LastIndexOf("@") - 1).Contains("@"))
                {
                    return false;
                }
                return true;
            }
            else return false;
            
        }
    }
}
