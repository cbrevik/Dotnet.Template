using FluentValidation;

namespace Api.Routes.Weather.Models;

public record PostWeatherRequest(DateOnly Date, int TemperatureC, string? Summary); 

public class PostWeatherRequestValidator : AbstractValidator<PostWeatherRequest>
{
    public PostWeatherRequestValidator()
    {
        RuleFor(x => x.Date).NotEmpty()
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now))
            .WithMessage("Date must be greater than or equal to today's date");
        
        RuleFor(x => x.TemperatureC)
            .InclusiveBetween(-90, 60)
            .WithMessage("Temperature must be between -90 and 60. Only support for Earth temperatures.");
    }
}