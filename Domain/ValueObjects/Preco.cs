using System.Globalization;

namespace SportStore.Domain.ValueObjects;

/// <summary>
/// Value Object para representar preços monetários
/// Garante validação e imutabilidade
/// </summary>
public class Preco : IEquatable<Preco>
{
    private readonly decimal _valor;

    public Preco(decimal valor)
    {
        if (valor < 0)
            throw new ArgumentException("Preço não pode ser negativo", nameof(valor));

        if (valor > 999999.99m)
            throw new ArgumentException("Preço não pode ser superior a R$ 999.999,99", nameof(valor));

        // Validar máximo 2 casas decimais
        //if (decimal.Round(valor, 2, MidpointRounding.AwayFromZero) != valor)
            //throw new ArgumentException("Preço deve ter no máximo 2 casas decimais", nameof(valor));

        _valor = valor;
    }

    public decimal Valor => _valor;

    public string FormatoBrasileiro => _valor.ToString("C", new CultureInfo("pt-BR"));

    public string FormatoDecimal => _valor.ToString("F2", CultureInfo.InvariantCulture);

    /// <summary>
    /// Converte implicitamente decimal para Preco
    /// </summary>
    public static implicit operator Preco(decimal valor) => new(valor);

    /// <summary>
    /// Converte implicitamente Preco para decimal
    /// </summary>
    public static implicit operator decimal(Preco preco) => preco._valor;

    /// <summary>
    /// Operador de soma
    /// </summary>
    public static Preco operator +(Preco a, Preco b) => new(a._valor + b._valor);

    /// <summary>
    /// Operador de subtração
    /// </summary>
    public static Preco operator -(Preco a, Preco b) => new(a._valor - b._valor);

    /// <summary>
    /// Operador de multiplicação por quantidade
    /// </summary>
    public static Preco operator *(Preco preco, int quantidade) => new(preco._valor * quantidade);

    /// <summary>
    /// Operador de multiplicação por decimal
    /// </summary>
    public static Preco operator *(Preco preco, decimal multiplicador) => new(preco._valor * multiplicador);

    /// <summary>
    /// Operador de divisão
    /// </summary>
    public static Preco operator /(Preco preco, decimal divisor) => new(preco._valor / divisor);

    /// <summary>
    /// Operadores de comparação
    /// </summary>
    public static bool operator >(Preco a, Preco b) => a._valor > b._valor;
    public static bool operator <(Preco a, Preco b) => a._valor < b._valor;
    public static bool operator >=(Preco a, Preco b) => a._valor >= b._valor;
    public static bool operator <=(Preco a, Preco b) => a._valor <= b._valor;

    /// <summary>
    /// Verifica se é igual a outro Preco
    /// </summary>
    public bool Equals(Preco? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _valor == other._valor;
    }

    /// <summary>
    /// Verifica se é igual a outro objeto
    /// </summary>
    public override bool Equals(object? obj)
    {
        return Equals(obj as Preco);
    }

    /// <summary>
    /// Gera hash code
    /// </summary>
    public override int GetHashCode()
    {
        return _valor.GetHashCode();
    }

    /// <summary>
    /// Representação string do preço
    /// </summary>
    public override string ToString()
    {
        return FormatoBrasileiro;
    }

    /// <summary>
    /// Preço zero
    /// </summary>
    public static readonly Preco Zero = new(0m);

    /// <summary>
    /// Verifica se o preço é zero
    /// </summary>
    public bool IsZero => _valor == 0m;

    /// <summary>
    /// Verifica se o preço é positivo
    /// </summary>
    public bool IsPositive => _valor > 0m;

    /// <summary>
    /// Verifica se o preço é negativo
    /// </summary>
    public bool IsNegative => _valor < 0m;
}
