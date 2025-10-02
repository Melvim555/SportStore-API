using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SportStore.Application.Validation;

/// <summary>
/// Atributo de validação para preços com máximo 2 casas decimais
/// </summary>
public class PrecoValidationAttribute : ValidationAttribute
{
    private readonly double _minValue;
    private readonly double _maxValue;

    public PrecoValidationAttribute(double minValue = 0.01, double maxValue = 999999.99)
    {
        _minValue = minValue;
        _maxValue = maxValue;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return new ValidationResult("Preço é obrigatório");

        if (value is not decimal preco)
            return new ValidationResult("Preço deve ser um número válido");

        // Verificar se tem mais de 2 casas decimais
        var precoString = preco.ToString("F");
        var partes = precoString.Split('.');
        
        if (partes.Length > 1 && partes[1].Length > 2)
        {
            return new ValidationResult("Preço deve ter no máximo 2 casas decimais (ex: 29.90)");
        }

        // Verificar range
        if (preco < (decimal)_minValue || preco > (decimal)_maxValue)
        {
            return new ValidationResult($"Preço deve estar entre R$ {_minValue:F2} e R$ {_maxValue:F2}");
        }

        return ValidationResult.Success;
    }
}
