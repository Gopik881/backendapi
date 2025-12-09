using Elixir.Domain.Entities;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Elixir.Application.Common.Constants;

public class AppConstants
{
    #region Auth Params
    public const int SessionTimeoutInMinutes = 30; // in minutes
    public const string USER_ROLE_ID = "UserRoleId"; // This is used as a key in the JWT token for user role ID
    public const string USER_ID = "UserId"; // This is used as a key in the JWT token for user ID
    public const string COMPANY_CODE = "CompanyCode"; // This is used as a key in the JWT token for company code
    public const string COMPANY_ID = "CompanyId"; // This is used as a key in the JWT token for company code
    public const string IS_SUPER_ADMIN = "IsSuperAdmin";
    public const string IS_SUPER_ADMIN_TRUE = "IsSuperAdminTrue";
    public const string IS_SUPER_ADMIN_FALSE = "IsSuperAdminFalse";
    #endregion

    public const string SUPER_ADMIN_ROLE = "SuperAdmin"; // This is the role name for the super admin user
    public const string COMPANY_ADMIN_ROLE = "CompanyAdmin"; // This is the role name for the admin user


    public const string ACCOUNT_MANAGERS = "accountmanagers";
    public const string CHECKERS = "checkers";
    public const string ELIXIR_USERS = "elixirusers";

    public const string ONBOARDING_STATUS_PENDING = "Pending";
    public const string ONBOARDING_STATUS_APPROVED = "Approved";
    public const string ONBOARDING_STATUS_NEW = "New";
    public const string ONBOARDING_STATUS_REJECTED = "Rejected";

    public const string COMPANY = "COMPANY";
    public const string MODULE = "MODULE";

    public const string NOTAVAILABLE = "N/A";

    public const string DUMMYCLIENTENTRY = "Dummy client entry";

    #region SubMenuItems
    public const string COMPANY_LIST_SUB_MENU_ITEM = "Company List";
    public const string COMPANY_ONBOARDING_SUB_MENU_ITEM = "Onboarding Status";
    public const string COMPANY_ONBOARDING_ITEM_NAME = "Company Onboarding List";
    #endregion


    #region Module
    public const string MODULE_CORE_HR = "Core HR";
    #endregion

    #region User Group
    public const string USER_GROUP_TYPE_CUSTOM = "Custom"; // This is the type for custom user groups
    public const string USER_GROUP_TYPE_DEFAULT = "Default"; // This is the type for default user groups
    public const string USER_GROUP_NAME_MIGRATIONUSER = "Migration User";

    public const string USER_ELIGIBILITY_USERMAPPING_YES = "Yes";
    public const string USER_ELIGIBILITY_USERMAPPING_NO = "No";

    public const string USER_USERMAPPING_ACTIVE = "Active";
    public const string USER_USERMAPPING_INACTIVE = "InActive";

    public const string ACCOUNTMANAGER_GROUPNAME = "Account Manager";
    public const string CHECKER_GROUPNAME = "Checker";


    #endregion

    #region company5TabHistoryByVersionNumberSkippedColumnNames

    public const string COMPANY5TABHISTORYBYVERSIONNUMBER_VERSION = "Version";
    public const string COMPANY5TABHISTORYBYVERSIONNUMBER_ID = "Id";
    public const string COMPANY5TABHISTORYBYVERSIONNUMBER_BY = "By";
    public const string COMPANY5TABHISTORYBYVERSIONNUMBER_AT = "At";


    public const string COMPANY5TABHISTORYBYVERSIONNUMBER_COMPANYHISTORYBYVERSION = "CompanyHistory";
    public const string COMPANY5TABHISTORYBYVERSIONNUMBER_ACCOUNTHISTORYBYVERSION = "AccountHistory";
    public const string COMPANY5TABHISTORYBYVERSIONNUMBER_COMPANYADMINHISTORYBYVERSION = "CompanyAdminHistory";
    public const string COMPANY5TABHISTORYBYVERSIONNUMBER_MODULEMAPPINGHISTORYBYVERSION = "ModuleMappingHistory";
    public const string COMPANY5TABHISTORYBYVERSIONNUMBER_REPORTINGTOOLLIMITHISTORYBYVERSION = "ReportingToolLimitHistory";
    public const string COMPANY5TABHISTORYBYVERSIONNUMBER_ESCALATIONCONTACTHISTORYBYVERSION = "EscalationContactsHistory";
    public const string COMPANY5TABHISTORYBYVERSIONNUMBER_ELIXIRHRUSERHISTORYBYVERSION = "ElixirUserHistory";
    public const string COMPANY5TABHISTORYBYVERSIONNUMBER_HISTORYRESULTBYVERION = "History";

    #endregion

    public const int SUBMENUITEM_ID_COMPANY_CREATION = 13; // This is the ID for the Company List submenu item
    public const int SUBMENUITEM_ID_CLIENT_CREATION = 14; // This is the ID for the Company Onboarding submenu item

    public const int SUBMENUITEM_ID_COMPANY_ONBOARDING_LIST = 12; // This is the ID for the Company Onboarding submenu item
    public const int SUBMENUITEM_ID_COMPANY_LIST = 13; // This is the ID for the Company List submenu item

    #region MetaDataReportingAdminReportNames
    public const string ELIXIR_REPORTING_ADMIN = "Elixir Reporting Admin";
    public const string ELIXIR_COMPANY_CLIENT_REPORTING_ADMIN = "Elixir Company/Client Reporting Admin";

    #endregion

    #region HistoryPopupColumnNames

    public static readonly Dictionary<string, string[]> IncludedProperties = new()
    {
        ["accountInfo"] = new[]
       {
            "CompanyStorageTotalGb","PerUserStorageMb", "UserGroupLimit", "TempUserLimit", "ContractName", "ContractId",
            "StartDate", "EndDate", "RenewalReminderDate", "ContractNoticePeriod",
            "LicenseProcurement", "Pan", "Tan", "Gstin"
        },
        ["reportingToolInfo"] = new[]
       {
           "NoOfReportingAdmins","NoOfCustomReportCreators","SavedReportQueriesInLibrary","DashboardsInLibrary","DashboardsInPersonalLibrary","LetterGenerationAdmins","TemplatesSaved"
        },
        ["companyUser"] = new[]
       {
            "FirstName", "LastName", "Email", "PhoneNumber", "Designation", "IsEnabled"
        },
        ["companyInfo"] = new[]
       {
            "CompanyName", "CompanyCode", "IsEnabled", "CompanyStorageConsumedGb", "CompanyStorageTotalGb",
            "ClientId", "IsUnderEdit", "Address1", "Address2", "StateId", "ZipCode", "CountryId",
            "PhoneNumber", "BillingAddressSameAsCompany", "BillingAddress1", "BillingAddress2",
            "BillingStateId", "BillingZipCode", "BillingCountryId", "BillingPhoneNumber",
            "MfaEnabled", "MfaEmail", "MfaSms","TelephoneCodeId","BillingTelephoneCodeId"
        },
        ["elixirUser"] = new[]
        {
            /* "RoleId", "UserGroupId",*/ "UserId"
         },
                ["moduleMapping"] = new[]
        {
             "ModuleName", "SubModuleName" /*"IsEnabled", "IsMandatory"*/
         },
        ["reportingToolLimits"] = new[]
       {
            "NoOfReportingAdmins", "NoOfCustomReportCreators", "SavedReportQueriesInLibrary",
            "SavedReportQueriesPerUser", "DashboardsInLibrary", "DashboardsInPersonalLibrary",
            "LetterGenerationAdmins", "TemplatesSaved"
        },
        ["escalationContacts"] = new[]
       {
            "FirstName", "LastName", "PhoneNumber", "Email", "Designation", "Department"
        }
    };

    #endregion

    #region NotificationConditionsforSuperAdmin

    public const string NOTIFICATION_CONDITION_FOR_SUPER_ADMIN = "Super Admin"; // This is the condition for super admin notifications
    public const string NOTIFICATION_PROFILE_UPDATED = "Profile Updated"; // This is the notification type for profile updates
    #endregion

    public static readonly HashSet<string> WeakPasswords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "123456", "password", "123456789", "12345", "12345678", "qwerty", "123123", "111111", "abc123", "1234", "1q2w3e4r", "admin"
    };

    #region CoreHrID
    public const int CORE_HR_ID = 1; // This is the ID for the Core HR module

    public const int FUNDAMENTALS_ID = 1; // This is the ID for the Fundamentals submodule
    public const int HORIZONTALS_ID = 2; // This is the ID for the Horizontals submodule

    #endregion

    #region UserRightsNames
    public const string USER_RIGHTS_ACCOUNT_MANAGER = "Account Manager"; // This is the name for the Account Managers user rights
    public const string USER_RIGHTS_CHECKER = "Checker"; // This is the name for the Checkers user rights
    public const string USER_RIGHTS_MIGRATION_USER = "Migration User"; // This is the name for the Migration User user rights

    #endregion

    #region company5tab Version
    public const string EMPTYRECORDS = "EMPTYRECORDS";
    #endregion



    public static class ErrorCodes
    {

        public const string INVALID_TELEPHONE_CODE_DUPLICATE = "Invalid telephone code. Duplicate telephone code found for the same country.";
        #region Template
        public const string IMPORT_FILE_TEMPLATE_MISMATCH = "File template headers are invalid";

        #endregion

        #region ValidationFailed
        public const string VALIDATION_FAILED = "Validation failed";
        #endregion
        #region Validate Company Code
        public const string COMPANY_CODE_MISSING = "Organisation code is required.";
        public const string COMPANY_CODE_INVALID = "Organisation code is Invalid";//"Organisation code is required";
        public const string COMPANY_CODE_VALID = "Organisation code Is Valid";
        public const string DUPLICATE_COMPANY_NAME = "Company name already exists.";
        #endregion

        #region Login
        public const string LOGIN_SUCCESSFUL = "Login successful";
        public const string INVALID_CREDENTIALS = "Incorrect credentials.";
        public const string PASSWORD_POLICY_NOT_FOUND = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        public const string PASSWORD_VALIDATION_FAILED = "Incorrect credentials.";
        public const string PASSWORD_HASH_MISMATCH = "Incorrect credentials.";
        public const string USER_DETAILS_FETCHED_SUCCESSFULLY = "User details fetched successfully";
        public const string USER_CREATED_SUCCESSFULLY = "User created successfully";
        public const string USER_GROUPS_FETCHED_SUCCESSFULLY = "User groups fetched successfully";
        public const string USER_GROUP_USERS_FETCHED_SUCCESSFULLY = "User group users fetched successfully";
        public const string USER_GROUP_CREATION_FAILED = "Failed to create the new record. Please verify your input and try again.";

        public const string USER_GROUP_USER_NOT_ADDED = "The user could not be added.";
        public const string USER_GROUP_USER_REMOVE_FAILED = "The user could not be removed.";
        #endregion

        #region Change Password
        public const string PASSWORD_CHANGE_SUCCESSFUL = "Your password has been changed successfully";
        public const string PASSWORD_CHANGE_FAILED = "Your password change was unsuccessful";
        #endregion

        #region ResetPwd
        public const string RESET_TOKEN_INVALID = "The link is no longer valid.";
        public const string RESET_PASSWORD_FAILURE = "Password reset failed.";
        #endregion
        #region Client Operations
        public const string CLIENT_CREATION_FAILED = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        public const string CLIENT_UPDATE_FAILED = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        public const string CLIENT_DUPLICATEDATA_FOUND = "Duplicate Entry Found, exists with the same name.";

        public const string CLIENT_NAME_ALREADY_EXIST = "Client Name Already Exist";
        public const string CLIENT_CODE_ALREADY_EXIST = "Client Code Already Exist";
        public const string CLIENT_DETAILS_RETRIEVAL_FAILED = "An error occurred while retrieving the requested information.";
        public const string CLIENT_ACCOUNT_MANAGERS_RETRIEVAL_FAILED = "The provided information is invalid. Please review the input fields and try again.";
        public const string CLIENT_UNMAPPED_COMPANIES_RETRIEVAL_FAILED = "An error occurred while retrieving the requested information.";
        public const string CLIENT_DELETION_FAILED = "Record could not be deleted.";
        public const string CLIENT_DETAILS_NOT_FOUND = "No records found matching the criteria";
        public const string CLIENT_DETAILS_FETCHED_UNSUCCESSFULLY = "An error occurred while retrieving the requested information.";
        public const string CLIENT_DELETION_SUCCESSFUL = "Client deleted successfully.";

        public const string CLIENT_CREATION_FAILURE = "Failed to create the new record. Please verify your input and try again.";
       
        public const string CLIENT_NO_CLIENT_FOUND = "No records found matching the criteria";
        public const string CLIENT_RETRIEVAL_FAILED = "An error occurred while retrieving the requested information.";

        #endregion

        #region Import/Export Data
        public const string EXPORT_TABLE_STRUCTURE_FAILED = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        public const string IMPORT_FILE_EMPTY_OR_NOT_PROVIDED = "File is empty or not provided.";
        public const string IMPORT_FILE_SIZE_EXCEEDS_LIMIT = "File size exceeds the limit of {fileSizeLimitMB.FileSizeLimitMb} MB.";
        public const string IMPORT_INVALID_FILE_FORMAT = "Invalid file format. Only .xls, .xlsx, and .csv are allowed.";
        public const string IMPORT_DATA_FAILED = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        public const string EXCEL_BULK_UPLOAD_FAILED = "Upload Partially Failed! The file contains unsupported data. Please check your file and try again.";
        public const string INVALID_URL_LOCATION_PROVIDED = "Invalid URL location provided. Please provide a valid URL location for the file upload.";
        public const string FILE_FORMAT_NOT_SUPPORTED = "Invalid file format (Only .xlsx are allowed) or file size greater than the set limit.";
        #endregion

        #region Master Data Bulk Upload
        public const string COUNTRY_MASTER_BULK_UPLOAD_FAILED = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        public const string STATE_MASTER_BULK_UPLOAD_FAILED = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        public const string TELEPHONE_CODE_MASTER_BULK_UPLOAD_FAILED = "Upload Failed! The file contains unsupported data. Please check your file and try again.";
        public const string CURRENCY_MASTER_BULK_UPLOAD_FAILED = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        #endregion

        #region Check ID
        public const string CHECK_ID_IN_USE_FAILED = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        #endregion

        #region Filter
        public const string FILTER_NO_RECORDS_FOUND = "No records found matching the criteria matching the criteria.";
        public const string FILTER_INVALID_PAGE_NUMBER = "Invalid page number. Please provide a page between 1 and {totalPages}.";
        #endregion

        #region 5Tab Operations
        public const string UPDATE_COMPANY_5TAB_FAILED = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        public const string GET_COMPANY_5TAB_FAILED = "An error occurred while retrieving the requested information.";
        public const string APPROVAL_CHECK_REJECTED = "Your request is Rejected for ";

        public const string APPROVAL_CHECK_FAILED = "Unable to check approval status.";
        public const string WITHDRAW_COMPANY_VERSION_FAILED = "Failed to withdraw 5 tab edit request.";
        public const string GET_5TAB_ONBOARDING_HISTORY_FAILED = "An error occurred while retrieving the requested information.";
        public const string GET_LAST_TWO_VERSIONS_FAILED = "An error occurred while retrieving the requested information.";
        public const string GET_USERS_BY_GROUP_5TAB_FAILED = "An error occurred while retrieving the requested information.";
        #endregion

        public const string INVALID_DATA_FORMAT = "File is empty or not provided.";//"Upload Failed! The file contains unsupported data. Please check your file and try again.";

        #region Import/Export Data
        public const string EXPORT_DATA_FAILED = "We couldn't export the table structure at this moment. Please retry later or reach out to support for assistance.";
        public const string IMPORT_FILE_EMPTY = "File is empty or not provided.";
        #endregion
        #region Country Master
        public const string COUNTRY_MASTER_RETRIEVAL_FAILED = "An error occurred while retrieving the requested information.";
        public const string COUNTRY_MASTER_RETRIEVAL_SUCCESS = "Country data fetched successfully.";
        public const string COMMAN_MASTER_RETRIEVAL_SUCCESS = "Master data fetched successfully.";
        public const string COUNTRY_MASTER_NO_RECORDS_FOUND = "No records found matching the criteria";
        public const string COUNTRY_MASTER_NO_DATA_PROVIDED = "[Input Validation Message - Missing Mandatory Fields].";
        public const string COUNTRY_MASTER_ADD_FAILED = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        public const string COUNTRY_MASTER_UPDATE_FAILED = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        public const string COUNTRY_MASTER_DELETE_FAILED = "Country is in use and cannot be deleted.";
        public const string COUNTRY_DUPLICATE_ENTRY = "Country already exists with the same name or code.";
        public const string COUNTRY_CREATED_SUCCESSFULLY = "Country created successfully.";
        public const string COUNTRY_NOT_FOUND = "No records found matching the criteria.";

        public const string MASTER_NO_DUPLICATES = "No Duplicates Found.";

        public const string STATES_NOT_FOUND = "No Records Found.";

        public const string SYSTEM_POLICY_UPDATE_FAILED = "The update could not be completed. Please make sure all fields are correctly filled and try again.";

        public const string COUNTRY_CREATION_FAILED = "Failed to create the new record. Please verify your input and try again.";


        public const string COUNTRY_UPDATED_SUCCESSFULLY = "Country updated successfully.";
        public const string COUNTRY_UPDATE_FAILED = "The update could not be completed. Please make sure all fields are correctly filled and try again.";

        public const string SYSTEM_POLICY_FETCHED_SUCCESSFULLY = "System policy fetched successfully.";
        #endregion

        #region Currency Master
        public const string CURRENCY_MASTER_RETRIEVAL_FAILED = "An error occurred while retrieving the requested information.";
        public const string COMMAN_MASTER_CURRENCY_RETRIEVAL_SUCCESS = "Currency data fetched successfully.";
        public const string CURRENCY_MASTER_NO_RECORD_FOUND = "No records found matching the criteria";
        public const string CURRENCY_MASTER_INPUT_DATA_NULL_OR_EMPTY = "[Input Validation Message - Missing Mandatory Fields].";
        public const string CURRENCY_MASTER_CREATE_FAILED = "Failed to create the new record. Please verify your input and try again.";
        public const string CURRENCY_MASTER_ID_MISMATCH = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        public const string CURRENCY_MASTER_RECORD_NOT_FOUND = "No records found matching the criteria.";
        public const string CURRENCY_MASTER_UPDATE_FAILED = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        public const string CURRENCY_MASTER_NOT_FOUND = "No records found matching the criteria.";
        public const string CURRENCY_MASTER_DELETE_FAILED = "The record could not be deleted.";
        public const string CURRENCY_MASTER_MAPPING_FAILED = "Currency is in use can not remapped.";
        public const string CURRENCY_MASTER_DUPLICATE_FOUND = "Currency already exists with the same name or code.";
        public const string CURRENCY_MASTER_ALREADY_MAPPED = "A country or currency with the same name or short name already exists.";
        #endregion

        #region Email
        public const string EMAIL_SEND_SUCCESS = "Email sent successfully.";
        public const string EMAIL_EXIST = "Email Already Exist";
        public const string EMAIL_NOT_FOUND = "Email not found.";
        public const string EMAIL_INVALID_REQUEST = "[Input Validation Message - Missing Mandatory Fields].";
        public const string EMAIL_SENDING_FAILED = "The email could not be sent.";
        #endregion

        #region Module Management
        public const string MODULE_MANAGEMENT_UPDATE_FAILED = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        public const string MODULE_MANAGEMENT_GET_VIEW_FAILED = "An error occurred while retrieving the requested information.";
        public const string MODULE_MANAGEMENT_GET_TMI_DASHBOARD_FAILED = "An error occurred while retrieving the requested information.";
        public const string MODULE_MANAGEMENT_GET_MODULE_MAPPING_FAILED = "An error occurred while retrieving the requested information.";
        public const string MODULE_MANAGEMENT_GET_MODULE_MAPPING_SUCCESS = "Modules fetched successfully.";
        #endregion

        #region Company
        public const string COMPANY_RETRIEVAL_FAILED = "An error occurred while retrieving the requested information.";
        public const string COMPANY_NO_DETAILS_FOUND = "No records found matching the criteria";
        public const string COMPANY_NO_COMPANY_FOUND = "No records found matching the criteria";
        public const string COMPANY_UPDATE_FAILED = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        public const string COMPANY_EDIT_FAILED = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        public const string COMPANY_REQUEST_DATA_INVALID = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        public const string COMPANY_SAVE_FAILED = "The save could not be completed.";
        #endregion

        #region Profile
        public const string PROFILE_NOT_FOUND = "Profile not found.";
        public const string PROFILE_UPDATE_FAILED = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        public const string PROFILE_UPDATE_SUCCESSFUL = "Profile updated successfully.";
        #endregion

        #region Notifications
        public const string NOTIFICATION_NOT_FOUND = "The selected notification was not found.";
        #endregion

        #region Password Policy
        public const string PASSWORD_POLICY_EDIT_FAILED = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        #endregion

        #region Reset Password
        public const string RESET_PASSWORD_INVALID_OR_MISSING_TOKEN = "The password setup link is no longer valid. Please request a new one.";
        public const string RESET_LINK_VALID = "The link is valid";
        public const string RESET_PASSWORD_TOKEN_EXPIRED = "The link is no longer valid.";
        public const string RESET_PASSWORD_INVALID_OR_TAMPERED_TOKEN = "The link is no longer valid.";
        public const string RESET_PASSWORD_FAILED = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        #endregion

        #region State Master
        public const string STATE_MASTER_RETRIEVAL_FAILED = "An error occurred while retrieving the requested information.";
        public const string STATE_MASTER_NOT_FOUND = "No records found matching the criteria.";
        public const string STATE_MASTER_INPUT_DATA_NULL_OR_EMPTY = "[Input Validation Message - Missing Mandatory Fields].";
        public const string STATE_MASTER_CREATE_FAILED = "An error occurred while retrieving the requested information.";
        public const string STATE_MASTER_ID_MISMATCH = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        public const string STATE_MASTER_RECORD_NOT_FOUND = "No records found matching the criteria.";
        public const string STATE_MASTER_UPDATE_FAILED = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        public const string STATE_MASTER_UPDATE_SUCCESSFUL = "State updated successfully."; 
        public const string STATE_MASTER_NOT_FOUND_FOR_DELETION = "No records found matching the criteria.";
        public const string STATE_MASTER_DELETE_FAILED = "The record could not be deleted.";
        public const string STATE_MASTER_DELETE_SUCCESSFUL = "State deleted successfully.";
        public const string STATE_MASTER_NO_STATES_FOR_COUNTRY = "No records found matching the criteria.";
        public const string STATE_NOT_FOUND_FOR_COUNTRY = "No records found matching the criteria.";
        public const string STATES_FETECHED_SUCCESSFULLY = "States fetched successfully.";
        public const string STATE_DUPLICATE_ENTRY = "Duplicate Entry Found, exists with the same name.";
        public const string STATE_CREATED_SUCCESSFULLY = "State created successfully.";

        public const string STATE_CREATION_FAILED = "Failed to create the new record. Please verify your input and try again.";

        #endregion
        
        #region Telephone Code Master
        public const string TELEPHONE_CODE_MASTER_RETRIEVAL_FAILED = "An error occurred while retrieving the requested information.";
        public const string TELEPHONE_CODE_MASTER_CREATE_FAILED = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        public const string TELEPHONE_CODE_MASTER_ID_MISMATCH = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        public const string TELEPHONE_CODE_MASTER_RECORD_NOT_FOUND = "No records found matching the criteria.";
        public const string TELEPHONE_CODE_MASTER_UPDATE_FAILED = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        public const string TELEPHONE_CODE_MASTER_NOT_FOUND = "No records found matching the criteria.";
        public const string TELEPHONE_CODE_MASTER_DELETE_FAILED = "No Records Found.";
        public const string TELEPOHONE_CODES_FETECHED_SUCCESSFULLY = "Telephone codes fetched successfully.";
        public const string TELEPHONE_CODE_CREATED_SUCCESSFULLY = "Telephone code created successfully.";
        public const string TELEPHONE_CODE_MASTER_DUPLICATE_FOUND = "Telephone code already exists with the same name or code.";
        public const string INVALID_TELEPHONE_CODE_FORMAT = "Invalid telephone code format.";
        #endregion

        #region User Group Users
        public const string USER_GROUP_USERS_RETRIEVAL_FAILED = "An error occurred while retrieving the requested information.";
        public const string USER_GROUP_USERS_USER_NOT_FOUND = "No users found for this Account.";
        public const string USER_PROFILE_NOTFOUND = "Profile not found.";
        public const string USER_GROUP_USERS_CREATE_FAILED = "The user group could not be created. Please make sure all fields are correctly filled and try again.";
        public const string USER_GROUP_CREATED_SUCCESS = "User group created successfully.";
        public const string USER_GROUP_USERS_UPDATE_FAILED = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        public const string USER_GROUP_USERS_ADD_MAPPING_FAILED = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        public const string USER_GROUP_USERS_ADD_MAPPING_SUCCESS = "Done! Your changes were successful.";
        public const string USER_GROUP_USERS_REMOVE_MAPPING_FAILED = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        public const string USER_GROUP_USERS_REMOVE_USER_CRITICAL_COMPANIES = "User is assigned to a company with minimum participation. User cannot be deleted.";
        public const string USER_GROUP_USERS_REMOVE_USER_FAILED = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        public const string USER_GROUP_USERS_ENABLE_DISABLE_FAILED = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        public const string NO_VALID_USER_DATA_FOUND_IN_THE_UPLOADED_FILE = "No valid user data found in the uploaded file. Please check the file and try again.";
        public const string USER_GROUP_USERS_REMOVE_MAPPING_SUCCESS = "User mapping removed successfully.";

        public const string USER_GROUP_NOT_FOUND_OR_DISABLED = "Cannot add users to a disabled user group.";
        #endregion

        #region User Rights
        public const string USER_RIGHTS_RETRIEVAL_FAILED = "An error occurred while retrieving the requested information.";
        public const string USER_RIGHTS_GROUP_NAME_EMPTY = "[Input Validation Message - Missing Mandatory Fields].";
        public const string USER_RIGHTS_INVALID_USER_GROUP_DATA = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        public const string USER_RIGHTS_CREATE_GROUP_FAILED = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        public const string USER_RIGHTS_EDIT_GROUP_FAILED = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        public const string USER_RIGHTS_DELETE_GROUP_FAILED = "The update could not be completed. Please make sure all fields are correctly filled and try again.";
        public const string USER_RIGHTS_GROUP_DELETED_SUCCESS = "User group deleted successfully.";
        public const string USER_RIGHTS_NO_USERS_MAPPED = "No users are mapped to the specified user group.";

        public const string USER_GROUP_DELETE_FAILED = "The record could not be deleted.";
        #endregion

        #region Confirmation Messages
        public const string LOGOUT_CONFIRMATION = "Are you sure you want to Log out";
        public const string CHANGE_PASSWORD_CONFIRMATION = "Are you sure you want to change your password";
        public const string DELETE_MASTER_LIST_ITEM_CONFIRMATION = "Are you sure you want to delete this item from the master list? This action cannot be undone";
        public const string CREATE_USER_GROUP_CONFIRMATION = "Are you sure you want to save changes?";
        public const string DELETE_USER_CONFIRMATION = "Are you sure you want to delete this user?";
        public const string DISABLE_COMPANY_CONFIRMATION = "Are you sure you want to disable this company?";
        public const string SAVE_CHANGES_CONFIRMATION = "Are you sure you want to save the changes?";
        public const string DELETE_CONTACT_CONFIRMATION = "Are you sure you want to delete this contact?";
        public const string DELETE_CLIENT_CONFIRMATION = "Are you sure you want to delete this client?";
        public const string WITHDRAW_REQUEST_CONFIRMATION = "Are you sure you want to withdraw this request?";
        #endregion

        #region Success Messages
        public const string SYSTEM_POLICY_SAVE_SUCCESS = "Done! Your changes were successful";
        public const string COMMON_MASTER_LIST_REFRESH_SUCCESS = "Done! New items are now available in the list";
        public const string COMMON_MASTER_LIST_DELETE_SUCCESS = "Done! Items have been deleted from the list";
        public const string COMMON_MASTER_COUNTRY_DELETE_SUCCESS = "Country is in use and cannot be deleted.";
        public const string COMMON_MASTER_LIST_FILE_UPLOAD_SUCCESS = "Upload complete! Data has been successfully added to the list.";
        public const string USER_MAPPING_UPDATE_SUCCESS = "Done! Your changes were successful.";
        public const string COMPANY_LIST_UPDATE_SUCCESS = "Done! Your changes were successful.";

        public const string BULK_UPLOAD_FAILED = "An error occurred while importing the data. Please review your input and try again.";
        public const string BULK_UPLOAD_PARTIALLY_SUCCESS = "Bulk Upload Partially Completed.";
        #endregion

        #region Other Failure Messages
        public const string CURRENCY_MASTER_LIST_LINKED_ITEM_DELETE_ATTEMPT = "Currency is in use and cannot be deleted.";//"Important! This master item is in use and cannot be deleted";
        public const string COMMON_MASTER_LIST_LINKED_ITEM_DELETE_ATTEMPT = "country is in use and cannot be deleted.";//"Important! This master item is in use and cannot be deleted";
        public const string COMMON_MASTER_LIST_INVALID_FILE_CONTENT = "Upload Failed! The file contains unsupported data. Please check your file and try again.";
        public const string COMMON_MASTER_LIST_FILE_SIZE_EXCEEDS_LIMIT = "Upload Failed! File size too large. Please reduce the file size and try again.";
        public const string CREATE_USER_GROUP_NON_EMPTY_DELETE_ATTEMPT = "There are users currently present in this user group. Remove the users before deleting this user group.";
        public const string USER_LIST_DISABLE_LAST_CHECKER = "Can not be Disabled as Jeny is the only checker in Reliance";
        public const string USER_LIST_USER_ASSIGNED_TO_COMPANY = "User is assigned to a company with minimum participation. User can not be deleted";
        public const string ACCESS_DENIED = "Note! User does not have access to this screen";
        public const string COMPANY_LIST_ENABLE_CLIENT_LESS_THAN_TWO_COMPANIES = "Minimum of 2 companies must be mapped to enable the client.";
        public const string COMPANY_LIST_VALIDATION_FAILURE = "Error! Minimum of 2 companies must be mapped to enable the client.";
        public const string CRITICAL_GROUPS_NOT_FOUND = "Critical user groups not found.";

        public const string TELEPHONE_CODE_MASTER_LIST_LINKED_ITEM_DELETE_ATTEMPT = "Telephone code is in use and cannot be deleted.";//"Important! This master item is in use and cannot be deleted";
        public const string COMMON_MASTER_LIST_LINKED_ITEM_DELETE_CANNOT_REMAP = "country is in use and cannot be remapped.";
        #endregion
    }

}
