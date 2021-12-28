using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_Book.Contracts.v1.Requests;

namespace Tweet_Book.Validators
{
    public class CreateTagRequestValidator:AbstractValidator<CreateTagRequest>
    {
        public CreateTagRequestValidator()
        {
            RuleFor(p => p.Name)
                .Empty();
              //  .Matches("^[a-zA-Z0-9 ]*$");
        }
    }
}
