using System.Linq;
using DrakeLambert.Peerra.WebApi.SharedKernel.Dto;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DrakeLambert.Peerra.WebApi.Web.Dto
{
    public class ErrorDto
    {
        public string Error { get; set; }

        public ErrorDto(string error)
        {
            Error = error;
        }

        public ErrorDto(Result result) : this(result.Error)
        { }

        public ErrorDto(ModelStateDictionary modelState)
        {
            Error = modelState
                .Select(entry => entry.Value.Errors)
                .SelectMany(errors => errors)
                .Select(error => error.ErrorMessage)
                .Aggregate((messages, message) => messages + " " + message);
        }
    }
}
