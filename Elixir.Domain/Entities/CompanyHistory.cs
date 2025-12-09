namespace Elixir.Domain.Entities;

public partial class CompanyHistory
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public int? LastUpdatedBy { get; set; }

    public DateTime? LastUpdatedOn { get; set; }

    public int? CompanyId { get; set; }

    public string? CompanyName { get; set; }

    public string? CompanyCode { get; set; }

    public bool? IsEnabled { get; set; }

    public decimal? CompanyStorageConsumedGb { get; set; }

    public decimal? CompanyStorageTotalGb { get; set; }

    public int? ClientId { get; set; }

    public bool? IsUnderEdit { get; set; }

    public int? AccountManagerId { get; set; }

    public int? CompanyAdminId { get; set; }

    public string? Address1 { get; set; }

    public string? Address2 { get; set; }

    public int? StateId { get; set; }

    public string? ZipCode { get; set; }

    public int? CountryId { get; set; }

    public int? TelephoneCodeId { get; set; }

    public string? PhoneNumber { get; set; }

    public bool? BillingAddressSameAsCompany { get; set; }

    public string? BillingAddress1 { get; set; }

    public string? BillingAddress2 { get; set; }

    public int? BillingStateId { get; set; }

    public string? BillingZipCode { get; set; }

    public int? BillingCountryId { get; set; }

    public int? BillingTelephoneCodeId { get; set; }

    public string? BillingPhoneNumber { get; set; }

    public bool? MfaEnabled { get; set; }

    public bool? MfaEmail { get; set; }

    public bool? MfaSms { get; set; }

    public int Version { get; set; }

    public bool IsDeleted { get; set; }

}
