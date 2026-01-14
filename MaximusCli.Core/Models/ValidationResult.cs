namespace MaximusCli.Core.Models;

/// <summary>
/// Represents the result of a configuration validation operation.
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the validation was successful.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Gets or sets the list of validation error messages.
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of validation warnings (non-fatal issues).
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Creates a successful validation result.
    /// </summary>
    public static ValidationResult Valid(List<string>? warnings = null)
    {
        return new ValidationResult
        {
            IsValid = true,
            Warnings = warnings ?? new List<string>()
        };
    }

    /// <summary>
    /// Creates a failed validation result.
    /// </summary>
    public static ValidationResult Invalid(params string[] errors)
    {
        return new ValidationResult
        {
            IsValid = false,
            Errors = errors.ToList()
        };
    }

    /// <summary>
    /// Creates a failed validation result with a list of errors.
    /// </summary>
    public static ValidationResult Invalid(List<string> errors)
    {
        return new ValidationResult
        {
            IsValid = false,
            Errors = errors
        };
    }
}
