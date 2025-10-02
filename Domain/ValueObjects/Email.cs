using System.Text.RegularExpressions;

namespace SportStore.Domain.ValueObjects;

/// <summary>
/// Value Object para representar emails com valida��o robusta
/// Garante valida��o, normaliza��o e imutabilidade
/// </summary>
public class Email : IEquatable<Email>
{
    private readonly string _value;

    // Regex mais robusto para valida��o de email
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9]([a-zA-Z0-9._-]*[a-zA-Z0-9])?@[a-zA-Z0-9]([a-zA-Z0-9.-]*[a-zA-Z0-9])?\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled
    );

    // TLDs v�lidos mais comuns
    private static readonly HashSet<string> ValidTlds = new(StringComparer.OrdinalIgnoreCase)
    {
        "com", "org", "net", "edu", "gov", "mil", "int", "br", "co.uk", "de", "fr", "it", "es", "pt", "jp", "cn", "in", "au", "ca", "mx", "ar", "cl", "pe", "co", "ve", "ec", "py", "uy", "bo", "gy", "sr", "gf", "info", "biz", "name", "pro", "aero", "coop", "museum", "travel", "jobs", "mobi", "tel", "asia", "cat", "post", "xxx", "edu", "gov", "mil", "int", "arpa"
    };

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email n�o pode ser vazio", nameof(value));

        var trimmedValue = value.Trim();

        if (trimmedValue.Length > 254) // RFC 5321 limit
            throw new ArgumentException("Email muito longo (m�ximo 254 caracteres)", nameof(value));

        if (trimmedValue.Length < 5) // m�nimo: a@b.co
            throw new ArgumentException("Email muito curto (m�nimo 5 caracteres)", nameof(value));

        if (!EmailRegex.IsMatch(trimmedValue))
            throw new ArgumentException("Formato de email inv�lido", nameof(value));

        // Validar TLD
        var domain = ExtrairDominio(trimmedValue);
        if (!ValidarTld(domain))
            throw new ArgumentException($"Dom�nio '{domain}' n�o � v�lido", nameof(value));

        // Validar partes espec�ficas
        ValidarPartesEmail(trimmedValue);

        _value = trimmedValue.ToLowerInvariant();
    }

    public string Value => _value;

    /// <summary>
    /// Converte implicitamente Email para string
    /// </summary>
    public static implicit operator string(Email email) => email._value;

    /// <summary>
    /// Converte implicitamente string para Email
    /// </summary>
    public static implicit operator Email(string email) => new(email);

    /// <summary>
    /// Verifica se � igual a outro Email
    /// </summary>
    public bool Equals(Email? other) => other != null && _value == other._value;

    /// <summary>
    /// Verifica se � igual a outro objeto
    /// </summary>
    public override bool Equals(object? obj) => Equals(obj as Email);

    /// <summary>
    /// Gera hash code
    /// </summary>
    public override int GetHashCode() => _value.GetHashCode();

    /// <summary>
    /// Representa��o string do email
    /// </summary>
    public override string ToString() => _value;

    /// <summary>
    /// Verifica se um email � v�lido (sem lan�ar exce��o)
    /// </summary>
    public static bool IsValid(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var trimmedEmail = email.Trim();

            if (trimmedEmail.Length > 254 || trimmedEmail.Length < 5)
                return false;

            if (!EmailRegex.IsMatch(trimmedEmail))
                return false;

            var domain = ExtrairDominio(trimmedEmail);
            return ValidarTld(domain);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Extrai o usu�rio (parte antes do @)
    /// </summary>
    public string Usuario => _value.Split('@')[0];

    /// <summary>
    /// Extrai o dom�nio (parte depois do @)
    /// </summary>
    public string Dominio => _value.Split('@')[1];

    /// <summary>
    /// Verifica se o email � de um dom�nio espec�fico
    /// </summary>
    public bool IsFromDomain(string domain)
    {
        return string.Equals(Dominio, domain, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Verifica se o email � de um provedor comum
    /// </summary>
    public bool IsFromCommonProvider()
    {
        var commonProviders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "gmail.com", "yahoo.com", "hotmail.com", "outlook.com", "live.com",
            "uol.com.br", "bol.com.br", "terra.com.br", "ig.com.br", "globo.com"
        };

        return commonProviders.Contains(Dominio);
    }

    /// <summary>
    /// Retorna email mascarado para privacidade (us***@gmail.com)
    /// </summary>
    public string Mascarado
    {
        get
        {
            var usuario = Usuario;
            if (usuario.Length <= 2)
                return $"***@{Dominio}";

            var inicio = usuario.Substring(0, 2);
            var asteriscos = new string('*', usuario.Length - 2);
            return $"{inicio}{asteriscos}@{Dominio}";
        }
    }

    /// <summary>
    /// Extrai dom�nio de um email
    /// </summary>
    private static string ExtrairDominio(string email)
    {
        var parts = email.Split('@');
        return parts.Length > 1 ? parts[1] : string.Empty;
    }

    /// <summary>
    /// Valida TLD (Top Level Domain)
    /// </summary>
    private static bool ValidarTld(string domain)
    {
        if (string.IsNullOrEmpty(domain))
            return false;

        var parts = domain.Split('.');
        if (parts.Length < 2)
            return false;

        var tld = parts[^1]; // �ltimo elemento
        return ValidTlds.Contains(tld.ToLowerInvariant());
    }

    /// <summary>
    /// Valida partes espec�ficas do email
    /// </summary>
    private static void ValidarPartesEmail(string email)
    {
        var parts = email.Split('@');
        if (parts.Length != 2)
            throw new ArgumentException("Email deve conter exatamente um @", nameof(email));

        var usuario = parts[0];
        var dominio = parts[1];

        // Validar usu�rio
        if (usuario.Length > 64) // RFC 5321 limit
            throw new ArgumentException("Parte do usu�rio muito longa (m�ximo 64 caracteres)", nameof(email));

        if (usuario.StartsWith('.') || usuario.EndsWith('.'))
            throw new ArgumentException("Usu�rio n�o pode come�ar ou terminar com ponto", nameof(email));

        if (usuario.Contains(".."))
            throw new ArgumentException("Usu�rio n�o pode conter pontos consecutivos", nameof(email));

        // Validar dom�nio
        if (dominio.Length > 253) // RFC 5321 limit
            throw new ArgumentException("Dom�nio muito longo (m�ximo 253 caracteres)", nameof(email));

        if (dominio.StartsWith('.') || dominio.EndsWith('.'))
            throw new ArgumentException("Dom�nio n�o pode come�ar ou terminar com ponto", nameof(email));

        if (dominio.Contains(".."))
            throw new ArgumentException("Dom�nio n�o pode conter pontos consecutivos", nameof(email));

        // Validar se dom�nio tem pelo menos um ponto
        if (!dominio.Contains('.'))
            throw new ArgumentException("Dom�nio deve conter pelo menos um ponto", nameof(email));
    }
}
