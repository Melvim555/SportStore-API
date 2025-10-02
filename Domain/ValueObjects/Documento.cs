using System.Text.RegularExpressions;

namespace SportStore.Domain.ValueObjects;

/// <summary>
/// Value Object para representar documentos (CPF/CNPJ)
/// Garante validação, formatação e imutabilidade
/// </summary>
public class Documento : IEquatable<Documento>
{
    private readonly string _numero;
    private readonly TipoDocumento _tipo;

    public Documento(string numero)
    {
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("Documento não pode ser vazio", nameof(numero));

        // Remove formatação
        var numeroLimpo = LimparDocumento(numero);

        // Valida formato básico
        if (!numeroLimpo.All(char.IsDigit))
            throw new ArgumentException("Documento deve conter apenas números", nameof(numero));

        // Determina tipo e valida
        if (numeroLimpo.Length == 11)
        {
            _tipo = TipoDocumento.CPF;
            ValidarCPF(numeroLimpo);
        }
        else if (numeroLimpo.Length == 14)
        {
            _tipo = TipoDocumento.CNPJ;
            ValidarCNPJ(numeroLimpo);
        }
        else
        {
            throw new ArgumentException("Documento deve ter 11 dígitos (CPF) ou 14 dígitos (CNPJ)", nameof(numero));
        }

        _numero = numeroLimpo;
    }

    public string Numero => _numero;
    public TipoDocumento Tipo => _tipo;
    public string NumeroFormatado => _tipo == TipoDocumento.CPF ? FormatarCPF(_numero) : FormatarCNPJ(_numero);

    /// <summary>
    /// Converte implicitamente string para Documento
    /// </summary>
    public static implicit operator Documento(string numero) => new(numero);

    /// <summary>
    /// Converte implicitamente Documento para string
    /// </summary>
    public static implicit operator string(Documento documento) => documento._numero;

    /// <summary>
    /// Verifica se é igual a outro Documento
    /// </summary>
    public bool Equals(Documento? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _numero == other._numero && _tipo == other._tipo;
    }

    /// <summary>
    /// Verifica se é igual a outro objeto
    /// </summary>
    public override bool Equals(object? obj)
    {
        return Equals(obj as Documento);
    }

    /// <summary>
    /// Gera hash code
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(_numero, _tipo);
    }

    /// <summary>
    /// Representação string do documento
    /// </summary>
    public override string ToString()
    {
        return NumeroFormatado;
    }

    /// <summary>
    /// Remove formatação do documento
    /// </summary>
    private static string LimparDocumento(string documento)
    {
        return Regex.Replace(documento, @"[^\d]", "");
    }

    /// <summary>
    /// Formata CPF (XXX.XXX.XXX-XX)
    /// </summary>
    private static string FormatarCPF(string cpf)
    {
        return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";
    }

    /// <summary>
    /// Formata CNPJ (XX.XXX.XXX/XXXX-XX)
    /// </summary>
    private static string FormatarCNPJ(string cnpj)
    {
        return $"{cnpj.Substring(0, 2)}.{cnpj.Substring(2, 3)}.{cnpj.Substring(5, 3)}/{cnpj.Substring(8, 4)}-{cnpj.Substring(12, 2)}";
    }

    /// <summary>
    /// Valida CPF usando algoritmo oficial
    /// </summary>
    private static void ValidarCPF(string cpf)
    {
        // Verifica se todos os dígitos são iguais
        if (cpf.All(c => c == cpf[0]))
            throw new ArgumentException("CPF inválido: todos os dígitos são iguais");

        // Valida primeiro dígito verificador
        var soma = 0;
        for (int i = 0; i < 9; i++)
        {
            soma += int.Parse(cpf[i].ToString()) * (10 - i);
        }
        var resto = soma % 11;
        var dv1 = resto < 2 ? 0 : 11 - resto;

        if (int.Parse(cpf[9].ToString()) != dv1)
            throw new ArgumentException("CPF inválido: primeiro dígito verificador incorreto");

        // Valida segundo dígito verificador
        soma = 0;
        for (int i = 0; i < 10; i++)
        {
            soma += int.Parse(cpf[i].ToString()) * (11 - i);
        }
        resto = soma % 11;
        var dv2 = resto < 2 ? 0 : 11 - resto;

        if (int.Parse(cpf[10].ToString()) != dv2)
            throw new ArgumentException("CPF inválido: segundo dígito verificador incorreto");
    }

    /// <summary>
    /// Valida CNPJ usando algoritmo oficial
    /// </summary>
    private static void ValidarCNPJ(string cnpj)
    {
        // Verifica se todos os dígitos são iguais
        if (cnpj.All(c => c == cnpj[0]))
            throw new ArgumentException("CNPJ inválido: todos os dígitos são iguais");

        // Valida primeiro dígito verificador
        var multiplicadores1 = new int[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        var soma = 0;
        for (int i = 0; i < 12; i++)
        {
            soma += int.Parse(cnpj[i].ToString()) * multiplicadores1[i];
        }
        var resto = soma % 11;
        var dv1 = resto < 2 ? 0 : 11 - resto;

        if (int.Parse(cnpj[12].ToString()) != dv1)
            throw new ArgumentException("CNPJ inválido: primeiro dígito verificador incorreto");

        // Valida segundo dígito verificador
        var multiplicadores2 = new int[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        soma = 0;
        for (int i = 0; i < 13; i++)
        {
            soma += int.Parse(cnpj[i].ToString()) * multiplicadores2[i];
        }
        resto = soma % 11;
        var dv2 = resto < 2 ? 0 : 11 - resto;

        if (int.Parse(cnpj[13].ToString()) != dv2)
            throw new ArgumentException("CNPJ inválido: segundo dígito verificador incorreto");
    }

    /// <summary>
    /// Verifica se o documento é um CPF
    /// </summary>
    public bool IsCPF => _tipo == TipoDocumento.CPF;

    /// <summary>
    /// Verifica se o documento é um CNPJ
    /// </summary>
    public bool IsCNPJ => _tipo == TipoDocumento.CNPJ;

    /// <summary>
    /// Retorna apenas os primeiros 3 dígitos para privacidade
    /// </summary>
    public string NumeroMascarado => _tipo == TipoDocumento.CPF 
        ? $"***.***.***-{_numero.Substring(9, 2)}"
        : $"**.***.***/****-{_numero.Substring(12, 2)}";
}

/// <summary>
/// Enum para tipo de documento
/// </summary>
public enum TipoDocumento
{
    CPF = 1,
    CNPJ = 2
}
