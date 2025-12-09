namespace Elixir.Application.Features.Company.DTOs;

public class Company5TabCompanyDto
{
    public int CompanyId { get; set; }
    public int? CompanyStorageGB { get; set; }
    public int? PerUserStorageMB { get; set; }
    public string CompanyName { get; set; }
    public string ClientName { get; set; }
    public string ClientCode { get; set; }
    public string? CompanyCode { get; set; }
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public int StateId { get; set; }
    public string? StateName { get; set; }
    public int CountryId { get; set; }
    public string? CountryName { get; set; }
    public string ZipCode { get; set; }
    public string PhoneShortCutCode { get; set; }
    public int? TelephoneCodeId { get; set; }
    public string PhoneNumber { get; set; }
    public bool? MultiFactor { get; set; }
    public bool? IsSms { get; set; }
    public bool? IsEmail { get; set; }
    public bool? IsActive { get; set; }
    public bool? SameAddressForBilling { get; set; }
    public string? BillingAddress1 { get; set; }
    public string? BillingAddress2 { get; set; }
    public int? BillingStateId { get; set; }
    public string? BillingStateName { get; set; }
    public int BillingCountryId { get; set; }
    public string? BillingCountryName { get; set; }
    public string? BillingZipCode { get; set; }
    public string? BillingPhoneShortCutCode { get; set; }
    public int? BillingTelePhoneCodeId { get; set; }
    public string? BillingPhoneNo { get; set; }
    public int? CreatedBy { get; set; }
    public bool? IsOriginalAccountManager { get; set; }
}
