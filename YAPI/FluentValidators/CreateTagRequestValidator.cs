using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YAPI.Contracts.V1.Requests;

namespace YAPI.FluentValidators
{
    //Startupda fluent validatorsı containera ekledikten sonra direk olarak model de geçerli olacak
    public class CreateTagRequestValidator : AbstractValidator<CreateTagRequest>
    {
        public CreateTagRequestValidator()
        {
            RuleFor(expression: x => x.TagName)
                .NotEmpty()
                .Matches(expression: "^[a-zA-Z0-9 ]*$")
                .Must(x => x.EndsWith("Tag"));
            //gibi
        }
    }
}
