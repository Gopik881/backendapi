-- Database: ElixirHRDb
-- Note: The CREATE DATABASE statement is commented out as per user instruction that it's not needed in SQL Server.
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ElixirHRDb')
BEGIN
    CREATE DATABASE ElixirHRDb;
END;
GO

-- USE statement to select the database
USE ElixirHRDb;
GO

-- =============================================
-- Module: Master Data
-- =============================================

-- CountryMaster Table
IF OBJECT_ID('CountryMaster') IS NOT NULL
    DROP TABLE CountryMaster;
GO
CREATE TABLE CountryMaster (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    CountryName VARCHAR(255) NOT NULL,
    CountryShortName VARCHAR(255) NOT NULL,
    Description VARCHAR(MAX),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_CountryMaster PRIMARY KEY (Id),
    CONSTRAINT UQ_CountryMaster_CountryName UNIQUE (CountryName),
    CONSTRAINT UQ_CountryMaster_CountryShortName UNIQUE (CountryShortName),
    --CONSTRAINT UQ_CountryMaster_CountryName_CountryShortName UNIQUE (CountryName, CountryShortName) this is not requied as they are already checked to be individually unique
);
GO

INSERT INTO CountryMaster (
    CountryName, CountryShortName, Description, 
    CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, IsDeleted
) VALUES
('Canada', 'CA', 'Country in North America', GETDATE(), GETDATE(), NULL, NULL, 0),
('Australia', 'AU', 'Country in Oceania', GETDATE(), GETDATE(), NULL, NULL, 0),
('Germany', 'DE', 'Country', GETDATE(), GETDATE(), NULL, NULL, 0),
('France', 'FR', 'Country in Europe', GETDATE(), GETDATE(), NULL, NULL, 0),
('Japan', 'JP', 'Country in East Asia', GETDATE(), GETDATE(), NULL, NULL, 0),
('China', 'CN', 'Country in East Asia', GETDATE(), GETDATE(), NULL, NULL, 0),
('Brazil', 'BR', 'Country in South America', GETDATE(), GETDATE(), NULL, NULL, 0),
('Albania', 'AL', 'Country', GETDATE(), GETDATE(), NULL, NULL, 0),
('Bolivia', 'BO', NULL, GETDATE(), GETDATE(), NULL, NULL, 0),
('Cambodia', 'KH', NULL, GETDATE(), GETDATE(), NULL, NULL, 0),
('Cyprus', 'CY', 'Country', GETDATE(), GETDATE(), NULL, NULL, 0),
('Czechia (Czech Republic)', 'CZ', NULL, GETDATE(), GETDATE(), NULL, NULL, 0),
('Finland', 'FI', NULL, GETDATE(), GETDATE(), NULL, NULL, 0),
('Kuwait', 'KW', NULL, GETDATE(), GETDATE(), NULL, NULL, 0),
('India', 'IN', 'Country', GETDATE(), GETDATE(), NULL, NULL, 0),
('Singapore', 'SG', 'Country', GETDATE(), GETDATE(), NULL, NULL, 0),
('Myanmar', 'MM', '', GETDATE(), GETDATE(), NULL, NULL, 0),
('Nepal', 'NP', 'Country', GETDATE(), GETDATE(), NULL, NULL, 0),
('Malaysia', 'MY', NULL, GETDATE(), GETDATE(), NULL, NULL, 0);
GO

-- TelephoneCodeMaster Table
IF OBJECT_ID('TelephoneCodeMaster') IS NOT NULL
    DROP TABLE TelephoneCodeMaster;
GO
CREATE TABLE TelephoneCodeMaster (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    CountryId INT NOT NULL,
    TelephoneCode VARCHAR(255) NOT NULL,
    Description VARCHAR(MAX),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_TelephoneCodeMaster PRIMARY KEY (Id),
    CONSTRAINT UQ_TelephoneCodeMaster_CountryId UNIQUE (CountryId)
    --CONSTRAINT UQ_TelephoneCodeMaster_CountryId_TelephoneCode UNIQUE (CountryId, TelephoneCode)
);
GO
ALTER TABLE TelephoneCodeMaster
ADD CONSTRAINT FK_TelephoneCodeMaster_CountryId FOREIGN KEY (CountryId) REFERENCES CountryMaster (Id) ON UPDATE CASCADE;
GO
CREATE INDEX IX_TelephoneCodeMaster_CountryId ON TelephoneCodeMaster (CountryId);
GO

INSERT INTO TelephoneCodeMaster (
    CountryId, TelephoneCode, Description, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, IsDeleted
) VALUES
(1, '+1', 'Canada', GETDATE(), GETDATE(), NULL, NULL, 0),
(2, '+61', 'Australia', GETDATE(), GETDATE(), NULL, NULL, 0),
(5, '+81', 'Japan', GETDATE(), GETDATE(), NULL, NULL, 0),
(15, '+91', 'India', GETDATE(), GETDATE(), NULL, NULL, 0);
GO

-- StateMaster Table37
IF OBJECT_ID('StateMaster') IS NOT NULL
    DROP TABLE StateMaster;
GO
CREATE TABLE StateMaster (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    CountryId INT NOT NULL,
    StateName VARCHAR(255) NOT NULL,
    StateShortName VARCHAR(255) NOT NULL,
    Description VARCHAR(MAX),
    IsDeleted BIT NOT NULL DEFAULT 1,
    CONSTRAINT PK_StateMaster PRIMARY KEY (Id),
    --CONSTRAINT UQ_StateMaster_CountryId_StateName UNIQUE (CountryId,StateName),
    --CONSTRAINT UQ_StateMaster_CountryId__StateShortName UNIQUE (CountryId,StateShortName)
    --CONSTRAINT UQ_StateMaster_CountryId_StateName_StateShortName UNIQUE (CountryId, StateName, StateShortName) -- this is not required as they are already checked to be individually unique
);
GO
ALTER TABLE StateMaster
ADD CONSTRAINT FK_StateMaster_CountryId FOREIGN KEY (CountryId) REFERENCES CountryMaster (Id) ON UPDATE CASCADE;
GO
CREATE INDEX IX_StateMaster_CountryId ON StateMaster (CountryId);
GO

INSERT INTO StateMaster (
    CountryId, StateName, StateShortName, Description,
    CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, IsDeleted
) VALUES
(15, 'Maharashtra', 'MH', 'State in Western India', GETDATE(), GETDATE(), NULL, NULL, 0),
(15, 'Karnataka', 'KA', 'State in Southern India', GETDATE(), GETDATE(), NULL, NULL, 0),
(1, 'British Columbia', 'BC', 'Province on the West Coast', GETDATE(), GETDATE(), NULL, NULL, 0),
(1, 'Alberta', 'AB', 'Province in Western Canada', GETDATE(), GETDATE(), NULL, NULL, 0),
(2, 'New South Wales', 'NSW', 'State in Southeastern Australia', GETDATE(), GETDATE(), NULL, NULL, 0),
(2, 'Victoria', 'VIC', 'State in Southeastern Australia', GETDATE(), GETDATE(), NULL, NULL, 0),
(2, 'Western Australia', 'WA', 'State in Western Australia', GETDATE(), GETDATE(), NULL, NULL, 0);
GO

-- CurrencyMaster Table
IF OBJECT_ID('CurrencyMaster') IS NOT NULL
    DROP TABLE CurrencyMaster;
GO
CREATE TABLE CurrencyMaster (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    CountryId INT NOT NULL,
    CurrencyName VARCHAR(255) NOT NULL,
    CurrencyShortName VARCHAR(255) NOT NULL,
    Description VARCHAR(MAX),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_CurrencyMaster PRIMARY KEY (Id),
    CONSTRAINT UQ_CurrencyMaster_CountryId_CurrencyName UNIQUE (CountryId, CurrencyName),
    CONSTRAINT UQ_CurrencyMaster_CountryId_CurrencyShortName UNIQUE (CountryId, CurrencyShortName)
);
GO
ALTER TABLE CurrencyMaster
ADD CONSTRAINT FK_CurrencyMaster_CountryId FOREIGN KEY (CountryId) REFERENCES CountryMaster (Id) ON UPDATE CASCADE;
GO
CREATE INDEX IX_CurrencyMaster_CountryId ON CurrencyMaster (CountryId);
GO

INSERT INTO CurrencyMaster (
    CreatedAt, UpdatedAt, CreatedBy, UpdatedBy,
    CountryId, CurrencyName, CurrencyShortName, Description, IsDeleted
) VALUES
(GETDATE(), GETDATE(), 1, 1, 15, 'Indian Rupee', 'INR', 'Currency of India', 0),
(GETDATE(), GETDATE(), 1, 1, 1, 'Canadian Dollar', 'CAD', 'Currency of Canada', 0),
(GETDATE(), GETDATE(), 1, 1, 2, 'Australian Dollar', 'AUD', 'Currency of Australia', 0),
(GETDATE(), GETDATE(), 1, 1, 5, 'Japanese Yen', 'JPY', 'Currency of Japan', 0);
GO

-- Category Table
IF OBJECT_ID('Category') IS NOT NULL
    DROP TABLE Category;
GO
CREATE TABLE Category (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    IsEnabled BIT NOT NULL DEFAULT 1,
    CategoryName VARCHAR(255) NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_Category PRIMARY KEY (Id)
);
GO

INSERT INTO Category (CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, CategoryName, IsEnabled, IsDeleted)
VALUES 
(GETDATE(), GETDATE(), 1, 1, 'PlaceHolder 1', 0, 0),
(GETDATE(), GETDATE(), 1, 1, 'PlaceHolder 2', 0, 0);
GO

-- Master Table
IF OBJECT_ID('Master') IS NOT NULL
    DROP TABLE Master;
GO
CREATE TABLE Master (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    MasterName VARCHAR(255) NOT NULL,
    MasterScreenUrl VARCHAR(255),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_Master PRIMARY KEY (Id)
);
GO

INSERT INTO Master (
    MasterName, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, MasterScreenUrl, IsDeleted
) VALUES
('Country Master', GETDATE(), GETDATE(), NULL, NULL, '/common-master/country-master', 0),
('State Master', GETDATE(), GETDATE(), NULL, NULL, '/common-master/state-master', 0),
('Currency Master', GETDATE(), GETDATE(), NULL, NULL, '/common-master/currency-master', 0),
('Telephone Code Master', GETDATE(), GETDATE(), NULL, NULL, '/common-master/telephone-master', 0),
('users', GETDATE(), GETDATE(), NULL, NULL, NULL, 0),
('defaultgrouplist', GETDATE(), GETDATE(), NULL, NULL, NULL, 0),
('commonmasterlist', GETDATE(), GETDATE(), NULL, NULL, NULL, 0),
('usergroups', GETDATE(), GETDATE(), NULL, NULL, NULL, 0),
('modulelist', GETDATE(), GETDATE(), NULL, NULL, NULL, 0);
GO

-- =============================================
-- Module: Client
-- =============================================

-- Clients Table
IF OBJECT_ID('Clients') IS NOT NULL
    DROP TABLE Clients;
GO
CREATE TABLE Clients (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    ClientName VARCHAR(50) NOT NULL,
    ClientInfo VARCHAR(50),
    IsEnabled BIT DEFAULT 1,
    ClientCode VARCHAR(12) NOT NULL,
    Address1 VARCHAR(255),
    Address2 VARCHAR(255),
    StateId INT,
    CountryId INT,
    ZipCode VARCHAR(20),
    PhoneNumber VARCHAR(20),
    PhoneCodeId INT,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_Clients PRIMARY KEY (Id)
);
GO
ALTER TABLE Clients
ADD CONSTRAINT FK_Clients_StateId FOREIGN KEY (StateId) REFERENCES StateMaster (Id),
    CONSTRAINT FK_Clients_CountryId FOREIGN KEY (CountryId) REFERENCES CountryMaster (Id),
    CONSTRAINT FK_Clients_PhoneCodeId FOREIGN KEY (PhoneCodeId) REFERENCES TelephoneCodeMaster (Id);
GO
CREATE INDEX IX_Clients_ClientId ON Clients (Id);
GO

INSERT INTO Clients (
    ClientName, ClientCode, IsEnabled, CreatedAt, UpdatedAt, IsDeleted
) VALUES (
    'Default Client', 'CLT001', 0, GETDATE(), GETDATE(), 0
);
GO

-- ClientAccess Table
IF OBJECT_ID('ClientAccess') IS NOT NULL
    DROP TABLE ClientAccess;
GO
CREATE TABLE ClientAccess (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    EnableWebQuery BIT DEFAULT 0,
    EnableReportAccess BIT DEFAULT 0,
    ClientUserLimit INT,
    ClientId INT,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_ClientAccess PRIMARY KEY (Id)
);
GO
ALTER TABLE ClientAccess
ADD CONSTRAINT FK_ClientAccess_ClientId FOREIGN KEY (ClientId) REFERENCES Clients (Id);
GO
CREATE INDEX IX_ClientAccess_ClientId ON ClientAccess (ClientId);
GO

-- ClientAdminInfo Table
IF OBJECT_ID('ClientAdminInfo') IS NOT NULL
    DROP TABLE ClientAdminInfo;
GO
CREATE TABLE ClientAdminInfo (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    FirstName VARCHAR(45),
    LastName VARCHAR(45),
    CountryId INT,
    PhoneNumber VARCHAR(20),
    Email VARCHAR(45),
    Designation VARCHAR(50),
    ClientId INT,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_ClientAdminInfo PRIMARY KEY (Id)
);
GO
ALTER TABLE ClientAdminInfo
ADD CONSTRAINT FK_ClientAdminInfo_ClientId FOREIGN KEY (ClientId) REFERENCES Clients (Id),
    CONSTRAINT FK_ClientAdminInfo_CountryId FOREIGN KEY (CountryId) REFERENCES CountryMaster (Id);
GO
CREATE INDEX IX_ClientAdminInfo_ClientId ON ClientAdminInfo (ClientId);
GO

-- ClientContactDetails Table
IF OBJECT_ID('ClientContactDetails') IS NOT NULL
    DROP TABLE ClientContactDetails;
GO
CREATE TABLE ClientContactDetails (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    FirstName VARCHAR(45),
    LastName VARCHAR(45),
    CountryId INT,
    PhoneNumber VARCHAR(20),
    Email VARCHAR(255),
    Designation VARCHAR(50),
    Department VARCHAR(45),
    Remarks VARCHAR(MAX),
    ClientId INT,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_ClientContactDetails PRIMARY KEY (Id)
);
GO
ALTER TABLE ClientContactDetails
ADD CONSTRAINT FK_ClientContactDetails_ClientId FOREIGN KEY (ClientId) REFERENCES Clients (Id),
    CONSTRAINT FK_ClientContactDetails_CountryId FOREIGN KEY (CountryId) REFERENCES CountryMaster (Id);
GO
CREATE INDEX IX_ClientContactDetails_ClientId ON ClientContactDetails (ClientId);
GO

-- ClientReportingToolLimits Table
IF OBJECT_ID('ClientReportingToolLimits') IS NOT NULL
    DROP TABLE ClientReportingToolLimits;
GO
CREATE TABLE ClientReportingToolLimits (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    ClientReportingAdmins VARCHAR(36),
    ClientCustomerReportCreators VARCHAR(36),
    ClientSavedReportQueriesLibrary VARCHAR(36),
    ClientSavedReportQueriesPerUser VARCHAR(36),
    ClientDashboardLibrary VARCHAR(36),
    ClientDashboardPersonalLibrary VARCHAR(36),
    ClientId INT,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_ClientReportingToolLimits PRIMARY KEY (Id)
);
GO
ALTER TABLE ClientReportingToolLimits
ADD CONSTRAINT FK_ClientReportingToolLimits_ClientId FOREIGN KEY (ClientId) REFERENCES Clients (Id);
GO
CREATE INDEX IX_ClientReportingToolLimits_ClientId ON ClientReportingToolLimits (ClientId);
GO

-- =============================================
-- Module: Company
-- =============================================

-- Companies Table
IF OBJECT_ID('Companies') IS NOT NULL
    DROP TABLE Companies;
GO
CREATE TABLE Companies (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    LastUpdatedBy INT,
    LastUpdatedOn DATETIME DEFAULT GETDATE(),
    CompanyName VARCHAR(50),
    ClientName VARCHAR(50),
    CompanyCode VARCHAR(50),
    IsEnabled BIT DEFAULT 1,
    IsActive BIT DEFAULT 0,
    CompanyStorageConsumedGb DECIMAL(10, 2) DEFAULT 0.00,
    CompanyStorageTotalGb DECIMAL(10, 2) DEFAULT 0.00,
    ClientId INT DEFAULT 1,
    IsUnderEdit BIT DEFAULT 0,
    AccountManagerId INT,
    CompanyAdminId INT,
    Address1 VARCHAR(255),
    Address2 VARCHAR(255),
    StateId INT,
    ZipCode VARCHAR(20),
    CountryId INT,
    TelephoneCodeId INT,
    PhoneNumber VARCHAR(20),
    BillingAddressSameAsCompany BIT DEFAULT 0,
    BillingAddress1 VARCHAR(255),
    BillingAddress2 VARCHAR(255),
    BillingStateId INT,
    BillingZipCode VARCHAR(20),
    BillingCountryId INT,
    BillingTelephoneCodeId INT,
    BillingPhoneNumber VARCHAR(20),
    MfaEnabled BIT DEFAULT 0,
    MfaEmail BIT DEFAULT 0,
    MfaSms BIT DEFAULT 0,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_Companies PRIMARY KEY (Id),
    -- CONSTRAINT UQ_Companies_CompanyCode UNIQUE (CompanyCode),
    -- CONSTRAINT UQ_Companies_CompanyName UNIQUE (CompanyName)
);
GO
ALTER TABLE Companies
ADD CONSTRAINT FK_Companies_ClientId FOREIGN KEY (ClientId) REFERENCES Clients (Id),
    CONSTRAINT FK_Companies_StateId FOREIGN KEY (StateId) REFERENCES StateMaster (Id),
    CONSTRAINT FK_Companies_CountryId FOREIGN KEY (CountryId) REFERENCES CountryMaster (Id),
    CONSTRAINT FK_Companies_BillingStateId FOREIGN KEY (BillingStateId) REFERENCES StateMaster (Id),
    CONSTRAINT FK_Companies_BillingCountryId FOREIGN KEY (BillingCountryId) REFERENCES CountryMaster (Id),
    CONSTRAINT FK_Companies_TelephoneCodeId FOREIGN KEY (TelephoneCodeId) REFERENCES TelephoneCodeMaster (Id),
    CONSTRAINT FK_Companies_BillingTelephoneCodeId FOREIGN KEY (BillingTelephoneCodeId) REFERENCES TelephoneCodeMaster (Id);
GO
CREATE INDEX IX_Companies_ClientId ON Companies (ClientId);
GO

INSERT INTO Companies (
    CompanyName, CompanyCode, ClientId, IsEnabled, CreatedAt, UpdatedAt
) VALUES (
    'Default Company', 'TMI', 1, 1, GETDATE(), GETDATE()
);
GO

-- CompanyHistory Table
IF OBJECT_ID('CompanyHistory') IS NOT NULL
    DROP TABLE CompanyHistory;
GO
CREATE TABLE CompanyHistory (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    LastUpdatedBy INT,
    LastUpdatedOn DATETIME DEFAULT GETDATE(),
    CompanyId INT,
    CompanyName VARCHAR(255),
    CompanyCode VARCHAR(255),
    IsEnabled BIT DEFAULT 1,
    CompanyStorageConsumedGb DECIMAL(10, 2) DEFAULT 0.00,
    CompanyStorageTotalGb DECIMAL(10, 2) DEFAULT 0.00,
    ClientId INT DEFAULT 1,
    IsUnderEdit BIT DEFAULT 0,
    AccountManagerId INT,
    CompanyAdminId INT,
    Address1 VARCHAR(255),
    Address2 VARCHAR(255),
    StateId INT,
    ZipCode VARCHAR(20),
    CountryId INT,
    TelephoneCodeId INT,
    PhoneNumber VARCHAR(20),
    BillingAddressSameAsCompany BIT DEFAULT 0,
    BillingAddress1 VARCHAR(255),
    BillingAddress2 VARCHAR(255),
    BillingStateId INT,
    BillingZipCode VARCHAR(20),
    BillingCountryId INT,
    BillingTelephoneCodeId INT,
    BillingPhoneNumber VARCHAR(20),
    MfaEnabled BIT DEFAULT 0,
    MfaEmail BIT DEFAULT 0,
    MfaSms BIT DEFAULT 0,
    Version INT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_CompanyHistory PRIMARY KEY (Id)
);
GO
ALTER TABLE CompanyHistory
ADD CONSTRAINT FK_CompanyHistory_ClientId FOREIGN KEY (ClientId) REFERENCES Clients (Id),
    CONSTRAINT FK_CompanyHistory_StateId FOREIGN KEY (StateId) REFERENCES StateMaster (Id),
    CONSTRAINT FK_CompanyHistory_CountryId FOREIGN KEY (CountryId) REFERENCES CountryMaster (Id),
    CONSTRAINT FK_CompanyHistory_BillingStateId FOREIGN KEY (BillingStateId) REFERENCES StateMaster (Id),
    CONSTRAINT FK_CompanyHistory_BillingCountryId FOREIGN KEY (BillingCountryId) REFERENCES CountryMaster (Id),
    CONSTRAINT FK_CompanyHistory_TelephoneCodeId FOREIGN KEY (TelephoneCodeId) REFERENCES TelephoneCodeMaster (Id),
    CONSTRAINT FK_CompanyHistory_BillingTelephoneCodeId FOREIGN KEY (BillingTelephoneCodeId) REFERENCES TelephoneCodeMaster (Id);
GO
CREATE INDEX IX_CompanyHistory_ClientId ON CompanyHistory (ClientId);
GO

-- Account Table
IF OBJECT_ID('Account') IS NOT NULL
    DROP TABLE Account;
GO
CREATE TABLE Account (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    CompanyId INT NOT NULL,
    PerUserStorageMb DECIMAL(10, 2) DEFAULT 0.00,
    UserGroupLimit INT,
    TempUserLimit INT,
    ContractName VARCHAR(255),
    ContractId VARCHAR(50),
    StartDate DATETIME,
    EndDate DATETIME,
    IsOpenEnded BIT DEFAULT 0,
    RenewalReminderDate DATETIME,
    ContractNoticePeriod INT,
    LicenseProcurement VARCHAR(50),
    Pan VARCHAR(10),
    Tan VARCHAR(10),
    Gstin VARCHAR(15),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_Account PRIMARY KEY (Id)
);
GO
ALTER TABLE Account
ADD CONSTRAINT FK_Account_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id);
GO
CREATE INDEX IX_Account_CompanyId ON Account (CompanyId);
GO

-- AccountHistory Table
IF OBJECT_ID('AccountHistory') IS NOT NULL
    DROP TABLE AccountHistory;
GO
CREATE TABLE AccountHistory (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT NOT NULL,
    UpdatedBy INT,
    LastUpdatedBy INT,
    LastUpdatedOn DATETIME DEFAULT GETDATE(),
    CompanyId INT NOT NULL,
    PerUserStorageMb DECIMAL(10, 2) DEFAULT 0.00,
    UserGroupLimit INT,
    TempUserLimit INT,
    ContractName VARCHAR(255),
    ContractId VARCHAR(50),
    StartDate DATETIME,
    EndDate DATETIME,
    IsOpenEnded BIT DEFAULT 0,
    RenewalReminderDate DATETIME,
    ContractNoticePeriod INT,
    LicenseProcurement VARCHAR(50),
    Pan VARCHAR(10),
    Tan VARCHAR(10),
    Gstin VARCHAR(15),
    Version INT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_AccountHistory PRIMARY KEY (Id)
);
GO
ALTER TABLE AccountHistory
ADD CONSTRAINT FK_AccountHistory_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id);
GO
CREATE INDEX IX_AccountHistory_CompanyId ON AccountHistory (CompanyId);
GO

-- CompanyOnboardingStatus Table
IF OBJECT_ID('CompanyOnboardingStatus') IS NOT NULL
    DROP TABLE CompanyOnboardingStatus;
GO
CREATE TABLE CompanyOnboardingStatus (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT NOT NULL,
    UpdatedBy INT,
    IsActive BIT NOT NULL DEFAULT 1,
    ClientId INT NOT NULL,
    CompanyId INT NOT NULL,
    OnboardingStatus VARCHAR(10) NOT NULL,
    RejectedReason VARCHAR(MAX),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_CompanyOnboardingStatus PRIMARY KEY (Id),
    CONSTRAINT CK_CompanyOnboardingStatus_OnboardingStatus CHECK (OnboardingStatus IN ('New', 'Pending', 'Rejected', 'Approved'))
);
GO
ALTER TABLE CompanyOnboardingStatus
ADD CONSTRAINT FK_CompanyOnboardingStatus_ClientId FOREIGN KEY (ClientId) REFERENCES Clients (Id),
    CONSTRAINT FK_CompanyOnboardingStatus_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id) ON DELETE NO ACTION;
GO
CREATE INDEX IX_CompanyOnboardingStatus_ClientId ON CompanyOnboardingStatus (ClientId);
GO
CREATE INDEX IX_CompanyOnboardingStatus_CompanyId ON CompanyOnboardingStatus (CompanyId);
GO

-- ClientCompaniesMapping Table
IF OBJECT_ID('ClientCompaniesMapping') IS NOT NULL
    DROP TABLE ClientCompaniesMapping;
GO
CREATE TABLE ClientCompaniesMapping (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    ClientId INT NOT NULL,
    CompanyId INT NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_ClientCompaniesMapping PRIMARY KEY (Id)
);
GO
ALTER TABLE ClientCompaniesMapping
ADD CONSTRAINT FK_ClientCompaniesMapping_ClientId FOREIGN KEY (ClientId) REFERENCES Clients (Id),
    CONSTRAINT FK_ClientCompaniesMapping_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id) ON DELETE NO ACTION;
GO
CREATE INDEX IX_ClientCompaniesMapping_ClientId ON ClientCompaniesMapping (ClientId);
GO
CREATE INDEX IX_ClientCompaniesMapping_CompanyId ON ClientCompaniesMapping (CompanyId);
GO

-- EscalationContacts Table
IF OBJECT_ID('EscalationContacts') IS NOT NULL
    DROP TABLE EscalationContacts;
GO
CREATE TABLE EscalationContacts (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    CompanyId INT NOT NULL,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100),
    TelephoneCodeId INT,
    PhoneNumber VARCHAR(20),
    Email VARCHAR(255) NOT NULL,
    Designation VARCHAR(50),
    Department VARCHAR(255),
    Remarks VARCHAR(MAX),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_EscalationContacts PRIMARY KEY (Id)
);
GO
ALTER TABLE EscalationContacts
ADD CONSTRAINT FK_EscalationContacts_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id),
    CONSTRAINT FK_EscalationContacts_TelephoneCodeId FOREIGN KEY (TelephoneCodeId) REFERENCES TelephoneCodeMaster (Id);
GO
CREATE INDEX IX_EscalationContacts_CompanyId ON EscalationContacts (CompanyId);
GO

-- EscalationContactsHistory Table
IF OBJECT_ID('EscalationContactsHistory') IS NOT NULL
    DROP TABLE EscalationContactsHistory;
GO
CREATE TABLE EscalationContactsHistory (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    CompanyId INT NOT NULL,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100),
    TelephoneCodeId INT,
    PhoneNumber VARCHAR(20),
    Email VARCHAR(255) NOT NULL,
    Designation VARCHAR(50),
    Department VARCHAR(255),
    Remarks VARCHAR(MAX),
    Version INT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_EscalationContactsHistory PRIMARY KEY (Id)
);
GO
ALTER TABLE EscalationContactsHistory
ADD CONSTRAINT FK_EscalationContactsHistory_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id),
    CONSTRAINT FK_EscalationContactsHistory_TelephoneCodeId FOREIGN KEY (TelephoneCodeId) REFERENCES TelephoneCodeMaster (Id);
GO
CREATE INDEX IX_EscalationContactsHistory_CompanyId ON EscalationContactsHistory (CompanyId);
GO

-- =============================================
-- Module: User
-- =============================================

-- Roles Table
IF OBJECT_ID('Roles') IS NOT NULL
    DROP TABLE Roles;
GO
CREATE TABLE Roles (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    RoleName VARCHAR(50) NOT NULL,
    Description VARCHAR(500),
    IsEnabled BIT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_Roles PRIMARY KEY (Id)
);
GO
INSERT INTO Roles (RoleName, Description, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsEnabled) 
VALUES 
    ('SuperAdmin', 'Administrator role with full access rights.', GETDATE(), NULL, GETDATE(), NULL, 1),
    ('AccountManager', 'Role with permissions to edit content.', GETDATE(), NULL, GETDATE(), NULL, 1),
    ('Checker', 'Role with Approve/Reject Rights', GETDATE(), NULL, GETDATE(), NULL, 1),
    ('MigrationUser', 'Role with Approve/Reject Rights', GETDATE(), NULL, GETDATE(), NULL, 1),
    ('DelegateAdmin', 'Role with Delete Admin Rights', GETDATE(), NULL, GETDATE(), NULL, 1),
    ('CompanyAdmin', 'Company Rights', GETDATE(), NULL, GETDATE(), NULL, 1);
GO

-- Super Users Table
IF OBJECT_ID('SuperUsers') IS NOT NULL
    DROP TABLE SuperUsers;
GO
CREATE TABLE SuperUsers (
    Id INT IDENTITY(1,1),
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    Email VARCHAR(255) NOT NULL,
	EmailHash int NOT NULL,
    PasswordHash VARCHAR(255),
	Salt VARCHAR(64) NULL,
    TelephoneCodeId INT,
    PhoneNumber VARCHAR(20) NOT NULL,
    Location VARCHAR(100),
    Designation VARCHAR(100) NOT NULL,
    ProfilePicture VARCHAR(MAX),
    LastSessionActiveUntil DATETIME NULL,
    IsEnabled BIT DEFAULT 1,
    FailedLoginAttempts INT DEFAULT 0,
    LastFailedAttempt DATETIME,
    IsLockedOut BIT NOT NULL DEFAULT 0,
    IsDeleted BIT NOT NULL DEFAULT 0,
    ResetPasswordToken VARCHAR(MAX) NULL,
    CONSTRAINT PK_SuperUsers PRIMARY KEY (Id),
    CONSTRAINT UQ_SuperUsers_Email UNIQUE (Email)
);
GO

CREATE INDEX IX_SuperUsers_EmailHash ON SuperUsers(EmailHash);
GO

INSERT INTO SuperUsers(
    FirstName, LastName, Email,EmailHash,Salt, PasswordHash, TelephoneCodeId, PhoneNumber,
    Location, Designation, ProfilePicture, IsEnabled) VALUES (
    'Kelly', 'Benny', 'superadmin@yopmail.com',587839511,'Sd53pNAfrQR4UzqlZp6nh9El/Ad7VHepn0Mo/LhzVTU=',
    'LJXc3+V4pHYeaxupBF/s9lXgqF+2iRiDj1d5AyhQe+A=',  1, '6281368680', 'India', 'Super Admin', NULL,1);
GO

---- Users Table
--IF OBJECT_ID('Users') IS NOT NULL
--    DROP TABLE Users;
--GO
--CREATE TABLE Users (
--    Id INT IDENTITY(1,1),
--    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
--    UpdatedAt DATETIME NULL,
--    CreatedBy INT,
--    UpdatedBy INT,
--    FirstName VARCHAR(100) NOT NULL,
--    LastName VARCHAR(100) NOT NULL,
--    Email VARCHAR(255) NOT NULL,
--	EmailHash int NOT NULL,
--    PasswordHash VARCHAR(255),
--	Salt VARCHAR(64) NULL,
--    TelephoneCodeId INT,
--    PhoneNumber VARCHAR(20) NOT NULL,
--    Location VARCHAR(100),
--    Designation VARCHAR(100) NOT NULL,
--    ProfilePicture VARCHAR(MAX),
--    IsEnabled BIT DEFAULT 1,
--    UserStorageConsumed DECIMAL(10, 2) DEFAULT 0.00,
--    FailedLoginAttempts INT DEFAULT 0,
--    LastFailedAttempt DATETIME,
--    IsLockedOut BIT NOT NULL DEFAULT 0,
--    LastSessionActiveUntil DATETIME NULL,
--    IsDeleted BIT NOT NULL DEFAULT 0,
--    ResetPasswordToken VARCHAR(MAX) NULL,
--    CONSTRAINT PK_Users PRIMARY KEY (Id),
--    CONSTRAINT UQ_Users_Email UNIQUE (Email)
--);
--GO

--CREATE INDEX IX_Users_EmailHash ON Users(EmailHash);
--GO
--ALTER TABLE Users
--ADD CONSTRAINT FK_Users_TelephoneCodeId FOREIGN KEY (TelephoneCodeId) REFERENCES TelephoneCodeMaster (Id);
--GO

-- Users table
IF OBJECT_ID('Users') IS NOT NULL
    DROP TABLE Users;
GO

-- Create Users table with IDENTITY starting at 2
CREATE TABLE Users (
    Id INT IDENTITY(2,1),  -- IDENTITY seed set to 2
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    Email VARCHAR(255) NOT NULL,
    EmailHash INT NOT NULL,
    PasswordHash VARCHAR(255),
    Salt VARCHAR(64) NULL,
    TelephoneCodeId INT,
    PhoneNumber VARCHAR(20) NOT NULL,
    Location VARCHAR(100),
    Designation VARCHAR(100) NOT NULL,
    ProfilePicture VARCHAR(MAX),
    IsEnabled BIT DEFAULT 1,
    UserStorageConsumed DECIMAL(10, 2) DEFAULT 0.00,
    FailedLoginAttempts INT DEFAULT 0,
    LastFailedAttempt DATETIME,
    IsLockedOut BIT NOT NULL DEFAULT 0,
    LastSessionActiveUntil DATETIME NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    ResetPasswordToken VARCHAR(MAX) NULL,
    CONSTRAINT PK_Users PRIMARY KEY (Id),
    CONSTRAINT UQ_Users_Email UNIQUE (Email)
);
GO

---- Ensure the next inserted ID is 2 (in case the default didn't take effect)
--DBCC CHECKIDENT ('Users', RESEED, 1);
--GO

-- Supporting index and foreign key
CREATE INDEX IX_Users_EmailHash ON Users(EmailHash);
GO

ALTER TABLE Users
ADD CONSTRAINT FK_Users_TelephoneCodeId FOREIGN KEY (TelephoneCodeId) REFERENCES TelephoneCodeMaster (Id);
GO


-- UserGroups Table
IF OBJECT_ID('UserGroups') IS NOT NULL
    DROP TABLE UserGroups;
GO
CREATE TABLE UserGroups (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    GroupName VARCHAR(255) NOT NULL,
    GroupType VARCHAR(10) NOT NULL,
    Description VARCHAR(MAX),
    IsEnabled BIT DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_UserGroups PRIMARY KEY (Id),
    CONSTRAINT CK_UserGroups_GroupType CHECK (GroupType IN ('Default', 'Custom'))
);
GO
CREATE INDEX IX_UserGroups_CreatedBy ON UserGroups (CreatedBy);
GO
INSERT INTO [dbo].[UserGroups]
           ([GroupName],[GroupType],[Description],[IsEnabled],[IsDeleted])
     VALUES
           ('Account Manager','Default','Company Account Manager Group',1,0),
		   ('Checker','Default','Company Checker Group',1,0),
		   ('Migration User','Default','Company Migration User Group',1,0)
           -- ('Delegate Admin','Custom','Delegate Admin User Group',1,0)
GO




-- CompanyAdminUsers Table
IF OBJECT_ID('CompanyAdminUsers') IS NOT NULL
    DROP TABLE CompanyAdminUsers;
GO
CREATE TABLE CompanyAdminUsers (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    CompanyId INT,
    FirstName VARCHAR(100),
    LastName VARCHAR(100),
    Email VARCHAR(255) NOT NULL,
    EmailHash int NOT NULL,
    PasswordHash VARCHAR(255),
    Salt VARCHAR(64) NULL,
    TelephoneCodeId INT,
    PhoneNumber VARCHAR(20),
    Designation VARCHAR(100),
    IsEnabled BIT DEFAULT 1,    
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_CompanyAdminUsers PRIMARY KEY (Id)
);
GO
ALTER TABLE CompanyAdminUsers
ADD CONSTRAINT FK_CompanyAdminUsers_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id),
    CONSTRAINT FK_CompanyAdminUsers_TelephoneCodeId FOREIGN KEY (TelephoneCodeId) REFERENCES TelephoneCodeMaster (Id);
GO
CREATE INDEX IX_CompanyAdminUsers_CompanyId ON CompanyAdminUsers (CompanyId);
GO

-- CompanyAdminUsersHistory Table
IF OBJECT_ID('CompanyAdminUsersHistory') IS NOT NULL
    DROP TABLE CompanyAdminUsersHistory;
GO
CREATE TABLE CompanyAdminUsersHistory (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    CompanyId INT,
    FirstName VARCHAR(100),
    LastName VARCHAR(100),
    Email VARCHAR(255) NOT NULL,
    TelephoneCodeId INT,
    PhoneNumber VARCHAR(20),
    Designation VARCHAR(100),
    IsEnabled BIT DEFAULT 1,
    PasswordHash VARCHAR(255),
    Version INT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_CompanyAdminUsersHistory PRIMARY KEY (Id)
);
GO
ALTER TABLE CompanyAdminUsersHistory
ADD CONSTRAINT FK_CompanyAdminUsersHistory_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id),
    CONSTRAINT FK_CompanyAdminUsersHistory_TelephoneCodeId FOREIGN KEY (TelephoneCodeId) REFERENCES TelephoneCodeMaster (Id);
GO
CREATE INDEX IX_CompanyAdminUsersHistory_CompanyId ON CompanyAdminUsersHistory (CompanyId);
GO

-- ElixirUsers Table
IF OBJECT_ID('ElixirUsers') IS NOT NULL
    DROP TABLE ElixirUsers;
GO
CREATE TABLE ElixirUsers (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    ClientId INT NULL,
    RoleId INT NOT NULL,
    CompanyId INT NULL,
    UserGroupId INT NULL,
    UserId INT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_ElixirUsers PRIMARY KEY (Id)
);
GO
ALTER TABLE ElixirUsers
ADD 
-- CONSTRAINT FK_ElixirUsers_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id),
    CONSTRAINT FK_ElixirUsers_UserGroupId FOREIGN KEY (UserGroupId) REFERENCES UserGroups (Id);
    -- CONSTRAINT FK_ElixirUsers_UserId FOREIGN KEY (UserId) REFERENCES Users (Id) ON DELETE NO ACTION;
GO
ALTER TABLE ElixirUsers
ADD CONSTRAINT UQ_ElixirUsers_ClientRoleCompanyGroupUser UNIQUE (ClientId, RoleId, CompanyId, UserGroupId, UserId);
GO
--CREATE INDEX IX_ElixirUsers_CompanyId ON ElixirUsers (CompanyId);
--GO
CREATE INDEX IX_ElixirUsers_UserId ON ElixirUsers (UserId);
GO



-- ElixirUsersHistory Table
IF OBJECT_ID('ElixirUsersHistory') IS NOT NULL
    DROP TABLE ElixirUsersHistory;
GO
CREATE TABLE ElixirUsersHistory (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    ClientId INT NULL,
    RoleId INT NULL,
    CompanyId INT NULL,
    UserGroupId INT NULL,
    UserId INT NULL,
    Version INT,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_ElixirUsersHistory PRIMARY KEY (Id)
);
GO
ALTER TABLE ElixirUsersHistory
ADD CONSTRAINT FK_ElixirUsersHistory_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id),
    CONSTRAINT FK_ElixirUsersHistory_UserGroupId FOREIGN KEY (UserGroupId) REFERENCES UserGroups (Id),
    CONSTRAINT FK_ElixirUsersHistory_UserId FOREIGN KEY (UserId) REFERENCES Users (Id) ON DELETE NO ACTION;
GO
ALTER TABLE ElixirUsers
ADD CONSTRAINT UQ_ElixirUsersHistory_ClientRoleCompanyGroupUser UNIQUE (ClientId, RoleId, CompanyId, UserGroupId, UserId);
GO
CREATE INDEX IX_ElixirUsersHistory_CompanyId ON ElixirUsersHistory (CompanyId);
GO
CREATE INDEX IX_ElixirUsersHistory_UserId ON ElixirUsersHistory (UserId);
GO

-- UserGroupMappings Table
IF OBJECT_ID('UserGroupMappings') IS NOT NULL
    DROP TABLE UserGroupMappings;
GO
CREATE TABLE UserGroupMappings (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT NOT NULL,
    UpdatedBy INT,
    UserId INT NOT NULL,
    UserGroupId INT NOT NULL,
    IsEligible BIT DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_UserGroupMappings PRIMARY KEY (Id),
    CONSTRAINT UQ_UserGroupMappings_UserId_UserGroupId UNIQUE (UserId, UserGroupId)
);
GO
ALTER TABLE UserGroupMappings
ADD CONSTRAINT FK_UserGroupMappings_UserGroupId FOREIGN KEY (UserGroupId) REFERENCES UserGroups (Id),
    CONSTRAINT FK_UserGroupMappings_UserId FOREIGN KEY (UserId) REFERENCES Users (Id);
GO
CREATE INDEX IX_UserGroupMappings_UserGroupId ON UserGroupMappings (UserGroupId);
GO

-- UserPasswordHistory Table
IF OBJECT_ID('UserPasswordHistory') IS NOT NULL
    DROP TABLE UserPasswordHistory;
GO
CREATE TABLE UserPasswordHistory (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UserId INT NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    Salt VARCHAR(64) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_UserPasswordHistory PRIMARY KEY (Id)
);
GO
--ALTER TABLE UserPasswordHistory
--ADD CONSTRAINT FK_UserPasswordHistory_UserId FOREIGN KEY (UserId) REFERENCES Users (Id);
--GO
CREATE INDEX IX_UserPasswordHistory_UserId ON UserPasswordHistory (UserId);
GO

-- Horizontals Table
IF OBJECT_ID('Horizontals') IS NOT NULL
    DROP TABLE Horizontals;
GO
CREATE TABLE Horizontals (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    UserGroupId INT NOT NULL,
    HorizontalName VARCHAR(255) NOT NULL,
    Description VARCHAR(MAX),
    IsSelected BIT NOT NULL DEFAULT 0,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_Horizontals PRIMARY KEY (Id)
);
GO
ALTER TABLE Horizontals
ADD CONSTRAINT FK_Horizontals_UserGroupId FOREIGN KEY (UserGroupId) REFERENCES UserGroups (Id);
GO
CREATE INDEX IX_Horizontals_UserGroupId ON Horizontals (UserGroupId);
GO

--INSERT INTO Horizontals (CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, UserGroupId, HorizontalName, Description)
--VALUES 
--(GETDATE(), GETDATE(), 1, 1, 2, 'Letter Generation Admin', 'Letter Template'),
--(GETDATE(), GETDATE(), 1, 1, 2, 'Document Management Admin', 'Document'),
--(GETDATE(), GETDATE(), 1, 1, 2, 'Web Query Admin', 'Web Query'),
--(GETDATE(), GETDATE(), 1, 1, 2, 'Email Admin', 'Email'),
--(GETDATE(), GETDATE(), 1, 1, 2, 'Reporting Admin', 'Report');
--GO

-- WebQueryHorizontalCheckboxItems Table
IF OBJECT_ID('WebQueryHorizontalCheckboxItems') IS NOT NULL
    DROP TABLE WebQueryHorizontalCheckboxItems;
GO
CREATE TABLE WebQueryHorizontalCheckboxItems (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    HorizontalId INT NOT NULL,
    CheckboxItemName VARCHAR(50) NOT NULL,
    IsSelected BIT DEFAULT 0,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_WebQueryHorizontalCheckboxItems PRIMARY KEY (Id)
);
GO
ALTER TABLE WebQueryHorizontalCheckboxItems
ADD CONSTRAINT FK_WebQueryHorizontalCheckboxItems_HorizontalId FOREIGN KEY (HorizontalId) REFERENCES Horizontals (Id);
GO
CREATE INDEX IX_WebQueryHorizontalCheckboxItems_HorizontalId ON WebQueryHorizontalCheckboxItems (HorizontalId);
GO

INSERT INTO WebQueryHorizontalCheckboxItems (
    CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, HorizontalId, CheckboxItemName, IsSelected
) VALUES 
    (GETDATE(), GETDATE(), 1, 1, 1, 'Elixir', 0),
    (GETDATE(), GETDATE(), 1, 1, 1, 'Company', 0),
    (GETDATE(), GETDATE(), 1, 1, 1, 'Client', 0);
GO

-- =============================================
-- Continue Module: Company
-- =============================================

-- Company5TabOnboardingHistory Table
IF OBJECT_ID('Company5TabOnboardingHistory') IS NOT NULL
    DROP TABLE Company5TabOnboardingHistory;
GO
CREATE TABLE Company5TabOnboardingHistory (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    CompanyId INT,
    Status VARCHAR(10) DEFAULT NULL,
    UserId INT,
    Reason VARCHAR(MAX),
    IsEnabled BIT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_Company5TabOnboardingHistory PRIMARY KEY (Id),
    CONSTRAINT CK_Company5TabOnboardingHistory_Status CHECK (Status IN ('New', 'Pending', 'Approved', 'Rejected'))
);
GO
ALTER TABLE Company5TabOnboardingHistory
ADD CONSTRAINT FK_Company5TabOnboardingHistory_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id)
    -- CONSTRAINT FK_Company5TabOnboardingHistory_UserId FOREIGN KEY (UserId) REFERENCES Users (Id);
GO
CREATE INDEX IX_Company5TabOnboardingHistory_CompanyId ON Company5TabOnboardingHistory (CompanyId);
GO

-- =============================================
-- Module: Module Management
-- =============================================

---- Modules Table
--IF OBJECT_ID('Modules') IS NOT NULL
--    DROP TABLE Modules;
--GO
--CREATE TABLE Modules (
--    Id INT IDENTITY(1,1),
--    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
--    UpdatedAt DATETIME NULL,
--    CreatedBy INT NOT NULL,
--    UpdatedBy INT,
--    ModuleName VARCHAR(255) NOT NULL,
--    Description VARCHAR(500) NOT NULL,
--    ModuleUrl VARCHAR(500) NOT NULL,
--    IsEnabled BIT DEFAULT 1,
--    IsDeleted BIT NOT NULL DEFAULT 0,
--    CONSTRAINT PK_Modules PRIMARY KEY (Id)
--);
--GO
--INSERT INTO Modules (
--    ModuleName, Description, ModuleUrl, IsEnabled,
--    CreatedBy, UpdatedBy, CreatedAt, UpdatedAt
--) VALUES
--    ('Core HR', 'Core HR Main Module', 'https://example.com/corehr', 1, 1, 1, GETDATE(), GETDATE()),
--    ('Exit', 'Exit control module', 'http://exit.com', 0, 0, 1, GETDATE(), GETDATE()),
--    ('Time Management', 'Time management module', 'http://timemanagement.com', 0, 0, 1, GETDATE(), GETDATE());
--GO

---- SubModules Table
--IF OBJECT_ID('SubModules') IS NOT NULL
--    DROP TABLE SubModules;
--GO
--CREATE TABLE SubModules (
--    Id INT IDENTITY(1,1),
--    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
--    UpdatedAt DATETIME NULL,
--    CreatedBy INT,
--    UpdatedBy INT,
--    SubModuleName VARCHAR(50) NOT NULL,
--    ModuleId INT,
--    IsEnabled BIT DEFAULT 1,
--    SubModuleParentId INT,
--    IsDeleted BIT NOT NULL DEFAULT 0,
--    CONSTRAINT PK_SubModules PRIMARY KEY (Id)
--);
--GO
--ALTER TABLE SubModules
--ADD CONSTRAINT FK_SubModules_ModuleId FOREIGN KEY (ModuleId) REFERENCES Modules (Id);
--GO
--CREATE INDEX IX_SubModules_ModuleId ON SubModules (ModuleId);
--GO

----SubModules Table Insert Query
--INSERT INTO [SubModules] (
--    CreatedAt,
--    UpdatedAt,
--    CreatedBy,
--    UpdatedBy,
--    SubModuleName,
--    ModuleId,
--    IsEnabled,
--    SubModuleParentId,
--    IsDeleted
--)
--VALUES
--    (GETDATE(), NULL, NULL, NULL, 'FUNDAMENTALS', 1, 1, 0, 0),
--    (GETDATE(), NULL, NULL, NULL, 'HORIZONTALS', 1, 1, 0, 0),
--    (GETDATE(), NULL, NULL, NULL, 'Master Management', 1, 1, 1, 0),
--    (GETDATE(), NULL, NULL, NULL, 'Exit', 1, 1, 1, 0),
--    (GETDATE(), NULL, NULL, NULL, 'Onboarding', 1, 1, 1, 0),
--    (GETDATE(), NULL, NULL, NULL, 'Confirmation', 1, 1, 1, 0),
--    (GETDATE(), NULL, NULL, NULL, 'Admin Console', 1, 1, 1, 0),
--    (GETDATE(), NULL, NULL, NULL, 'Email', 1, 1, 2, 0),
--    (GETDATE(), NULL, NULL, NULL, 'Notifications and Alerts', 1, 1, 2, 0),
--    (GETDATE(), NULL, NULL, NULL, 'Web Query', 1, 1, 2, 0),
--    (GETDATE(), NULL, NULL, NULL, 'Document Management', 1, 1, 2, 0),
--    (GETDATE(), NULL, NULL, NULL, 'Reporting Tool', 1, 1, 2, 0),
--    (GETDATE(), NULL, NULL, NULL, 'Questionnaries', 1, 1, 2, 0),

--    (GETDATE(), NULL, NULL, NULL, 'Letter Generation', 1, 1, 2, 0),

--    (GETDATE(), NULL, NULL, NULL, 'Sub Module 1', 2, 1, 0, 0),
--    (GETDATE(), NULL, NULL, NULL, 'Sub Module 2', 2, 1, 0, 0),
--    (GETDATE(), NULL, NULL, NULL, 'Sub Module 3', 2, 1, 0, 0),

--    (GETDATE(), NULL, NULL, NULL, 'Sub Module 4', 3, 1, 0, 0),
--    (GETDATE(), NULL, NULL, NULL, 'Sub Module 5', 3, 1, 0, 0),
--    (GETDATE(), NULL, NULL, NULL, 'Sub Module 6', 3, 1, 0, 0);

---- ModuleScreensMaster Table
--IF OBJECT_ID('ModuleScreensMaster') IS NOT NULL
--    DROP TABLE ModuleScreensMaster;
--GO
--CREATE TABLE ModuleScreensMaster (
--    Id INT IDENTITY(1,1),
--    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
--    UpdatedAt DATETIME NULL,
--    CreatedBy INT,
--    UpdatedBy INT,
--    ModuleMasterName VARCHAR(45) NOT NULL,
--    SubModuleId INT,
--    IsModuleMasterType BIT DEFAULT 1,
--    IsDeleted BIT NOT NULL DEFAULT 0,
--    CONSTRAINT PK_ModuleScreensMaster PRIMARY KEY (Id)
--);
--GO
--ALTER TABLE ModuleScreensMaster
--ADD CONSTRAINT FK_ModuleScreensMaster_SubModuleId FOREIGN KEY (SubModuleId) REFERENCES SubModules (Id);
--GO
--CREATE INDEX IX_ModuleScreensMaster_SubModuleId ON ModuleScreensMaster (SubModuleId);
--GO

---- ModuleMapping Table
--IF OBJECT_ID('ModuleMapping') IS NOT NULL
--    DROP TABLE ModuleMapping;
--GO
--CREATE TABLE ModuleMapping (
--    Id INT IDENTITY(1,1),
--    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
--    UpdatedAt DATETIME NULL,
--    CreatedBy INT,
--    UpdatedBy INT,
--    CompanyId INT NOT NULL,
--    ModuleId INT NOT NULL,
--    SubModuleId INT,
--    IsEnabled BIT DEFAULT 1,
--    IsMandatory BIT DEFAULT 0,
--    IsDeleted BIT NOT NULL DEFAULT 0,
--    CONSTRAINT PK_ModuleMapping PRIMARY KEY (Id)
--);
--GO
--ALTER TABLE ModuleMapping
--ADD CONSTRAINT FK_ModuleMapping_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id),
--    CONSTRAINT FK_ModuleMapping_ModuleId FOREIGN KEY (ModuleId) REFERENCES Modules (Id),
--    CONSTRAINT FK_ModuleMapping_SubModuleId FOREIGN KEY (SubModuleId) REFERENCES SubModules (Id);
--GO
--CREATE INDEX IX_ModuleMapping_CompanyId ON ModuleMapping (CompanyId);
--GO
--CREATE INDEX IX_ModuleMapping_ModuleId ON ModuleMapping (ModuleId);
--GO

---- ModuleMappingHistory Table
--IF OBJECT_ID('ModuleMappingHistory') IS NOT NULL
--    DROP TABLE ModuleMappingHistory;
--GO
--CREATE TABLE ModuleMappingHistory (
--    Id INT IDENTITY(1,1),
--    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
--    UpdatedAt DATETIME NULL,
--    CreatedBy INT,
--    UpdatedBy INT,
--    CompanyId INT NOT NULL,
--    ModuleId INT NOT NULL,
--    SubModuleId INT,
--    IsEnabled BIT DEFAULT 1,
--    IsMandatory BIT DEFAULT 0,
--    Version INT,
--    IsDeleted BIT NOT NULL DEFAULT 0,
--    CONSTRAINT PK_ModuleMappingHistory PRIMARY KEY (Id)
--);
--GO
--ALTER TABLE ModuleMappingHistory
--ADD CONSTRAINT FK_ModuleMappingHistory_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id),
--    CONSTRAINT FK_ModuleMappingHistory_ModuleId FOREIGN KEY (ModuleId) REFERENCES Modules (Id),
--    CONSTRAINT FK_ModuleMappingHistory_SubModuleId FOREIGN KEY (SubModuleId) REFERENCES SubModules (Id);
--GO
--CREATE INDEX IX_ModuleMappingHistory_CompanyId ON ModuleMappingHistory (CompanyId);
--GO
--CREATE INDEX IX_ModuleMappingHistory_ModuleId ON ModuleMappingHistory (ModuleId);
--GO

-- =============================================
-- Module: Menu Management
-- =============================================

-- MenuItems Table
IF OBJECT_ID('MenuItems') IS NOT NULL
    DROP TABLE MenuItems;
GO
CREATE TABLE MenuItems (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedBy INT,
    MenuItemName VARCHAR(50) NOT NULL,
    Description VARCHAR(MAX),
    MenuItemsUrl VARCHAR(255),
    IsEnabled BIT DEFAULT 1,
    MenuItemsIcon VARCHAR(255),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_MenuItems PRIMARY KEY (Id)
);
GO
INSERT INTO MenuItems (
    MenuItemName, Description, MenuItemsUrl, IsEnabled,
    CreatedAt, CreatedBy, MenuItemsIcon
) VALUES
    ('Dashboard', 'Manage users and roles', '/home', 1, GETDATE(), 1, 'dashboard'),
    ('Module Management', 'Manage users and roles', '', 1, GETDATE(), 1, 'moduleManagement'),
    ('Masters Management', 'Manage users and roles', '', 1, GETDATE(), 1, 'masterManagement'),
    ('System Policies', 'Manage users and roles', '/password-policy', 1, GETDATE(), 1, 'passwordPolicy'),
    ('User Management', 'Manage users and roles', '', 1, GETDATE(), 1, 'userManagement'),
    ('Client/Company', 'Manage users and roles', '', 1, GETDATE(), 1, 'company'),
    ('Horizontals', 'Manage users and roles', '', 1, GETDATE(), 1, 'horizontals'),
    ('Configurators', 'Manage users and roles', '', 1, GETDATE(), 1, 'rule_settings');
GO

-- SubMenuItems Table
IF OBJECT_ID('SubMenuItems') IS NOT NULL
    DROP TABLE SubMenuItems;
GO
CREATE TABLE SubMenuItems (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedBy INT,
    SubMenuItemName VARCHAR(50) NOT NULL,
    Description VARCHAR(MAX),
    SubMenuItemsUrl VARCHAR(255),
    IsEnabled BIT DEFAULT 1,
    MenuItemId INT,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_SubMenuItems PRIMARY KEY (Id)
);
GO
ALTER TABLE SubMenuItems
ADD CONSTRAINT FK_SubMenuItems_MenuItemId FOREIGN KEY (MenuItemId) REFERENCES MenuItems (Id);
GO
INSERT INTO SubMenuItems (SubMenuItemName, CreatedAt, Description, CreatedBy, SubMenuItemsUrl, MenuItemId)
VALUES 
    ('Create Module', GETDATE(), 'List of modules', 1, 'module-management/create-module', 2),
    ('Module List', GETDATE(), 'Screen for creating modules', 1, 'module-management/module-list', 2),    
    ('Module Structure', GETDATE(), 'Screen for managing module structure', 1, 'module-management/module-structure', 2),    
    ('Common Masters List', GETDATE(), 'List of common masters', 1, '/common-master', 3),    
    ('System Policies', GETDATE(), NULL, 1, NULL, 4),    
    ('Create User Group', GETDATE(), 'List of user groups', 1, '/user-management/create-user-group', 5),
    ('User Group List', GETDATE(), 'Screen for creating user groups', 1, '/user-management/user-group-list', 5),  
    ('Create User', GETDATE(), 'List of users', 1, '/user-management/create-user', 5),
    ('User List', GETDATE(), 'Screen for creating users', 1, '/user-management/user-list', 5),    
    ('User Mapping', GETDATE(), 'Screen for mapping users', 1, '/user-management/user-mapping', 5),    
    ('Company Creation', GETDATE(), 'List of companies', 1, 'company/company-creation', 6),
    ('Company Onboarding List', GETDATE(), 'List of companies for onboarding', 1, 'company/onboarding-status', 6), 
    ('Company List', GETDATE(), 'Screen for creating clients or companies', 1, 'company/company-list', 6),   
    ('Client Creation', GETDATE(), 'Screen for managing password policies', 1, 'company/client-creation', 6),  
    ('Client List', GETDATE(), 'Screen for creating clients or companies', 1, 'company/client-list', 6),
    ('Reports', GETDATE(), NULL, 1, NULL, 7),
    ('Web Query', GETDATE(), NULL, 1, NULL, 7),
    ('Letter Generation', GETDATE(), NULL, 1, NULL, 7),
    ('Document Management', GETDATE(), NULL, 1, NULL, 7),
    ('Email', GETDATE(), NULL, 1, NULL, 7),    
    ('API Configurator', GETDATE(), NULL, 1, NULL, 8),
    ('SSO Configurator', GETDATE(), NULL, 1, NULL, 8);
GO

-- SubMenuItemsAccessMapping Table
IF OBJECT_ID('SubMenuItemsAccessMapping') IS NOT NULL
    DROP TABLE SubMenuItemsAccessMapping;
GO
CREATE TABLE SubMenuItemsAccessMapping (
    Id INT IDENTITY(1,1),
    SubMenuItemsId INT NOT NULL,
    AccessToSubMenuItemsId INT NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_SubMenuItemsAccessMapping PRIMARY KEY (Id)
);
GO
ALTER TABLE SubMenuItemsAccessMapping
ADD CONSTRAINT FK_SubMenuItemsAccessMapping_SubMenuItemsId FOREIGN KEY (SubMenuItemsId) REFERENCES SubMenuItems (Id),
    CONSTRAINT FK_SubMenuItemsAccessMapping_AccessToSubMenuItemsId FOREIGN KEY (AccessToSubMenuItemsId) REFERENCES SubMenuItems (Id) ON DELETE NO ACTION;
GO
INSERT INTO SubMenuItemsAccessMapping (SubMenuItemsId, AccessToSubMenuItemsId) 
VALUES 
    (6, 7),
    (7, 6),
    (9, 8),
    (8, 9),
    (11, 12),
    (12, 11);
GO

-- UserGroupMenuMapping Table
IF OBJECT_ID('UserGroupMenuMapping') IS NOT NULL
    DROP TABLE UserGroupMenuMapping;
GO
CREATE TABLE UserGroupMenuMapping (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    SubMenuItemId INT NOT NULL,
    UserGroupId INT,
    IsAllCompanies BIT DEFAULT 0,
    CreateAccess BIT DEFAULT 0,
    ViewOnlyAccess BIT DEFAULT 0,
    EditAccess BIT DEFAULT 0,
    ApproveAccess BIT DEFAULT 0,
    IsEnabled BIT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_UserGroupMenuMapping PRIMARY KEY (Id)
);
GO
ALTER TABLE UserGroupMenuMapping
ADD CONSTRAINT FK_UserGroupMenuMapping_SubMenuItemId FOREIGN KEY (SubMenuItemId) REFERENCES SubMenuItems (Id),
    CONSTRAINT FK_UserGroupMenuMapping_UserGroupId FOREIGN KEY (UserGroupId) REFERENCES UserGroups (Id);
GO
CREATE INDEX IX_UserGroupMenuMapping_SubMenuItemId ON UserGroupMenuMapping (SubMenuItemId);
GO
CREATE INDEX IX_UserGroupMenuMapping_UserGroupId ON UserGroupMenuMapping (UserGroupId);
GO


INSERT INTO UserGroupMenuMapping (
    SubMenuItemId, CreateAccess, ViewOnlyAccess, EditAccess, ApproveAccess,
    CreatedAt, CreatedBy, UserGroupId
) VALUES
--Account Manager
--(3, 0, 0, 1, 0, GETDATE(), 1, 1), --Module Structure Screen
(13, 0, 0, 1, 0, GETDATE(), 1, 1),
(12, 0, 0, 1, 0, GETDATE(), 1, 1),

(16, 0, 1, 0, 0, GETDATE(), 1, 1),
(17, 0, 1, 0, 0, GETDATE(), 1, 1),
(18, 0, 1, 0, 0, GETDATE(), 1, 1),
(19, 0, 1, 0, 0, GETDATE(), 1, 1),
(20, 0, 1, 0, 0, GETDATE(), 1, 1),
--Checker
--(3, 0, 1, 0, 0, GETDATE(), 1, 2),--Module Structure Screen
(13, 0, 1, 0, 0, GETDATE(), 1, 2),
(12, 0, 1, 0, 0, GETDATE(), 1, 2),

(16, 0, 1, 0, 0, GETDATE(), 1, 2),
(17, 0, 1, 0, 0, GETDATE(), 1, 2),
(18, 0, 1, 0, 0, GETDATE(), 1, 2),
(19, 0, 1, 0, 0, GETDATE(), 1, 2),
(20, 0, 1, 0, 0, GETDATE(), 1, 2),
--Migration User
--(3, 0, 1, 0, 0, GETDATE(), 1, 3),--Module Structure Screen
(13, 0,1, 0, 0, GETDATE(), 1, 3),
(12, 0, 1, 0, 0, GETDATE(), 1, 3),

(16, 0, 1, 0, 0, GETDATE(), 1, 3),
(17, 0, 1, 0, 0, GETDATE(), 1, 3),
(18, 0, 1, 0, 0, GETDATE(), 1, 3),
(19, 0, 1, 0, 0, GETDATE(), 1, 3),
(20, 0, 1, 0, 0, GETDATE(), 1, 3)
GO

-- =============================================
-- Module: Reporting
-- =============================================

-- Report Table
IF OBJECT_ID('Report') IS NOT NULL
    DROP TABLE Report;
GO
CREATE TABLE Report (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    ReportName VARCHAR(255) NOT NULL,
    CategoryId INT NOT NULL,
    IsSelected BIT DEFAULT 0, 
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_Report PRIMARY KEY (Id)
);
GO
ALTER TABLE Report
ADD CONSTRAINT FK_Report_CategoryId FOREIGN KEY (CategoryId) REFERENCES Category (Id);
GO
CREATE INDEX IX_Report_CategoryId ON Report (CategoryId);
GO

INSERT INTO Report (
    CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, ReportName, CategoryId, IsSelected
) VALUES
--(GETDATE(), GETDATE(), NULL, NULL, 'Attendance', 1, 0),
--(GETDATE(), GETDATE(), NULL, NULL, 'Biometric System', 1, 0),
--(GETDATE(), GETDATE(), NULL, NULL, 'Retention', 2, 0),
--(GETDATE(), GETDATE(), NULL, NULL, 'Attrition Rate', 2, 0);
(GETDATE(), GETDATE(), NULL, NULL, 'PlaceHolder 3', 1, 0),
(GETDATE(), GETDATE(), NULL, NULL, 'PlaceHolder 4', 1, 0),
(GETDATE(), GETDATE(), NULL, NULL, 'PlaceHolder 5', 2, 0),
(GETDATE(), GETDATE(), NULL, NULL, 'PlaceHolder 6', 2, 0);
GO

-- ReportAccess Table
IF OBJECT_ID('ReportAccess') IS NOT NULL
    DROP TABLE ReportAccess;
GO
CREATE TABLE ReportAccess (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    UserGroupId INT NOT NULL,
    UserId INT NOT NULL,
    ReportId INT NOT NULL,
    CanDownload BIT DEFAULT 0,
    IsSelected BIT DEFAULT 0,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_ReportAccess PRIMARY KEY (Id)
);
GO
ALTER TABLE ReportAccess
ADD CONSTRAINT FK_ReportAccess_UserGroupId FOREIGN KEY (UserGroupId) REFERENCES UserGroups (Id),
    -- CONSTRAINT FK_ReportAccess_UserId FOREIGN KEY (UserId) REFERENCES Users (Id),
    CONSTRAINT FK_ReportAccess_ReportId FOREIGN KEY (ReportId) REFERENCES Report (Id);
GO
CREATE INDEX IX_ReportAccess_UserGroupId ON ReportAccess (UserGroupId);
GO

-- ReportingAdmin Table
IF OBJECT_ID('ReportingAdmin') IS NOT NULL
    DROP TABLE ReportingAdmin;
GO
CREATE TABLE ReportingAdmin (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    UserGroupId INT NOT NULL,
    ReportingAdminId INT NOT NULL,
    IsSelected BIT DEFAULT 0,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_ReportingAdmin PRIMARY KEY (Id)
);
GO
ALTER TABLE ReportingAdmin
ADD CONSTRAINT FK_ReportingAdmin_UserGroupId FOREIGN KEY (UserGroupId) REFERENCES UserGroups (Id),
    CONSTRAINT FK_ReportingAdmin_UserId FOREIGN KEY (UserId) REFERENCES Users (Id);
GO
CREATE INDEX IX_ReportingAdmin_UserGroupId ON ReportingAdmin (UserGroupId);
GO

-- =============================================
-- Module: Auditing
-- =============================================

-- AuditLog Table
IF OBJECT_ID('AuditLog') IS NOT NULL
    DROP TABLE AuditLog;
GO
CREATE TABLE AuditLog (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedBy INT,
    CompanyId INT NOT NULL,
    Action VARCHAR(50) NOT NULL,
    EntityName VARCHAR(255) NOT NULL,
    EntityId INT NOT NULL,
    Details VARCHAR(MAX),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_AuditLog PRIMARY KEY (Id)
);
GO
ALTER TABLE AuditLog
ADD CONSTRAINT FK_AuditLog_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id) ON DELETE NO ACTION;
GO
CREATE INDEX IX_AuditLog_CompanyId ON AuditLog (CompanyId);
GO

-- =============================================
-- Module: Notifications
-- =============================================

-- Notifications Table
IF OBJECT_ID('Notifications') IS NOT NULL
    DROP TABLE Notifications;
GO
CREATE TABLE Notifications (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    Title VARCHAR(255) NOT NULL,
    Message VARCHAR(MAX) NOT NULL,
    NotificationType VARCHAR(50) NOT NULL,
    IsRead BIT DEFAULT 0,
    UserId INT,
    CompanyId INT,
    IsActive BIT DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_Notifications PRIMARY KEY (Id)
);
GO

-- UserNotificationsMapping Table
IF OBJECT_ID('UserNotificationsMapping') IS NOT NULL
    DROP TABLE UserNotificationsMapping;
GO
CREATE TABLE UserNotificationsMapping (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    UserId INT NOT NULL,
    NotificationId INT NOT NULL,
    IsRead BIT DEFAULT 0,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_UserNotificationsMapping PRIMARY KEY (Id)
);
GO
ALTER TABLE UserNotificationsMapping
ADD CONSTRAINT FK_UserNotificationsMapping_UserId FOREIGN KEY (UserId) REFERENCES Users (Id),
    CONSTRAINT FK_UserNotificationsMapping_NotificationId FOREIGN KEY (NotificationId) REFERENCES Notifications (Id) ON DELETE NO ACTION;
GO
CREATE INDEX IX_UserNotificationsMapping_UserId ON UserNotificationsMapping (UserId);
GO

-- SystemPolicies Table
IF OBJECT_ID('SystemPolicies') IS NOT NULL
    DROP TABLE SystemPolicies;
GO
CREATE TABLE SystemPolicies (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    MaxLength INT,
    MinLength INT,
    NoOfUpperCase INT,
    NoOfLowerCase INT,
    NoOfSpecialCharacters INT,
    SpecialCharactersAllowed VARCHAR(15),
    HistoricalPasswords INT,
    PasswordValidityDays INT 
        CHECK (PasswordValidityDays >= 0 AND PasswordValidityDays <= 999999999), -- 9 digits
    UnsuccessfulAttempts INT 
        CHECK (UnsuccessfulAttempts >= 0 AND UnsuccessfulAttempts <= 99999),     -- 5 digits
    LockInPeriodInMinutes INT,
    SessionTimeoutMinutes INT DEFAULT 30,
    FileSizeLimitMb INT DEFAULT 1,
    IsEnabled BIT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_SystemPolicies PRIMARY KEY (Id)
);
GO

INSERT INTO SystemPolicies (
    CreatedAt, UpdatedAt, CreatedBy, UpdatedBy,
    IsEnabled, MaxLength, MinLength, NoOfUpperCase,
    NoOfLowerCase, NoOfSpecialCharacters, SpecialCharactersAllowed,
    HistoricalPasswords, PasswordValidityDays, UnsuccessfulAttempts,
    LockInPeriodInMinutes, SessionTimeoutMinutes, FileSizeLimitMb
) VALUES (
    GETDATE(), GETDATE(), 1, 1,
    1, 25, 5, 1,
    1, 2, '@#$%*',
    3, 1000, 2,
    1, 5, 1
);
GO

-- ReportingToolLimits Table
IF OBJECT_ID('ReportingToolLimits') IS NOT NULL
    DROP TABLE ReportingToolLimits;
GO
CREATE TABLE ReportingToolLimits (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    CompanyId INT NOT NULL,
    NoOfReportingAdmins INT DEFAULT 0,
    NoOfCustomReportCreators INT DEFAULT 0,
    SavedReportQueriesInLibrary INT DEFAULT 0,
    SavedReportQueriesPerUser INT DEFAULT 0,
    DashboardsInLibrary INT DEFAULT 0,
    DashboardsInPersonalLibrary INT DEFAULT 0,
    LetterGenerationAdmins INT DEFAULT 0,
    TemplatesSaved INT DEFAULT 0,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_ReportingToolLimits PRIMARY KEY (Id)
);
GO
ALTER TABLE ReportingToolLimits
ADD CONSTRAINT FK_ReportingToolLimits_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id);
GO
CREATE INDEX IX_ReportingToolLimits_CompanyId ON ReportingToolLimits (CompanyId);
GO

-- ReportingToolLimitsHistory Table
IF OBJECT_ID('ReportingToolLimitsHistory') IS NOT NULL
    DROP TABLE ReportingToolLimitsHistory;
GO
CREATE TABLE ReportingToolLimitsHistory (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    CompanyId INT NOT NULL,
    NoOfReportingAdmins INT DEFAULT 0,
    NoOfCustomReportCreators INT DEFAULT 0,
    SavedReportQueriesInLibrary INT DEFAULT 0,
    SavedReportQueriesPerUser INT DEFAULT 0,
    DashboardsInLibrary INT DEFAULT 0,
    DashboardsInPersonalLibrary INT DEFAULT 0,
    LetterGenerationAdmins INT DEFAULT 0,
    TemplatesSaved INT DEFAULT 0,
    Version INT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_ReportingToolLimitsHistory PRIMARY KEY (Id)
);
GO
ALTER TABLE ReportingToolLimitsHistory
ADD CONSTRAINT FK_ReportingToolLimitsHistory_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id);
GO
CREATE INDEX IX_ReportingToolLimitsHistory_CompanyId ON ReportingToolLimitsHistory (CompanyId);
GO

-- WebQueryAdmin Table
IF OBJECT_ID('WebQueryAdmin') IS NOT NULL
    DROP TABLE WebQueryAdmin;
GO
CREATE TABLE WebQueryAdmin (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    UserGroupId INT NOT NULL,
    IsSelected BIT DEFAULT 0,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_WebQueryAdmin PRIMARY KEY (Id)
);
GO
ALTER TABLE WebQueryAdmin
ADD CONSTRAINT FK_WebQueryAdmin_UserGroupId FOREIGN KEY (UserGroupId) REFERENCES UserGroups (Id);
GO
CREATE INDEX IX_WebQueryAdmin_UserGroupId ON WebQueryAdmin (UserGroupId);
GO

INSERT INTO WebQueryAdmin (
    CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, UserGroupId, IsSelected
) VALUES (
    GETDATE(), GETDATE(), 1, 1, 1, 1
);
GO


-- BulkUploadErrorTable Table
IF OBJECT_ID('BulkUploadErrorList') IS NOT NULL
    DROP TABLE BulkUploadErrorList;
GO
CREATE TABLE BulkUploadErrorList (
    Id BIGINT IDENTITY(1,1),
	ProcessId UNIQUEIDENTIFIER,
	RowId INT,
	ErrorField VARCHAR(100),
    ErrorMessage VARCHAR(250),
	CONSTRAINT PK_BulkUploadErrorList PRIMARY KEY (Id)
);
GO
CREATE INDEX IX_BulkUploadErrorList_ProcessId ON BulkUploadErrorList (ProcessId);
GO



/****** Drop Existing Tables if They Exist ******/
IF OBJECT_ID('SubModuleScreens') IS NOT NULL DROP TABLE SubModuleScreens;
GO
IF OBJECT_ID('SubModuleMasters') IS NOT NULL DROP TABLE SubModuleMasters;
GO
IF OBJECT_ID('ModuleScreens') IS NOT NULL DROP TABLE ModuleScreens;
GO
IF OBJECT_ID('ModuleMasters') IS NOT NULL DROP TABLE ModuleMasters;
GO
IF OBJECT_ID('SubModules') IS NOT NULL DROP TABLE SubModules;
GO
IF OBJECT_ID('Modules') IS NOT NULL DROP TABLE Modules;
GO

/****** Create Modules Table ******/
CREATE TABLE Modules (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT NOT NULL,
    UpdatedBy INT,
    ModuleName VARCHAR(255) NOT NULL,
    Description VARCHAR(500) NOT NULL,
    ModuleUrl VARCHAR(500) NOT NULL,
    IsEnabled BIT DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_Modules PRIMARY KEY (Id)
);
GO

/****** Insert Modules Data ******/
SET IDENTITY_INSERT [dbo].[Modules] ON
GO
INSERT [dbo].[Modules] ([Id], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModuleName], [Description], [ModuleUrl], [IsEnabled], [IsDeleted])
VALUES 
(1, GETDATE(), GETDATE(), 1, 1, N'Core HR', N'Core HR Main Module', N'https://example.com/corehr', 1, 0),
(2, GETDATE(), NULL, 0, NULL, N'Payroll Management', N'Salary processing, tax calculations, and payroll reports', N'https://hrms.company.com/payroll', 0, 0),
(3, GETDATE(), NULL, 0, NULL, N'Time & Attendance', N'Time tracking, attendance monitoring, and shift management', N'https://hrms.company.com/timeattendance', 0, 0),
(4, GETDATE(), NULL, 0, NULL, N'Performance Management', N'Performance reviews, goal setting, and appraisals', N'https://hrms.company.com/performance', 0, 0),
(5, GETDATE(), NULL, 0, NULL, N'Learning & Development', N'Training programs, skill development, and certifications', N'https://hrms.company.com/learning', 0, 0),
(6, GETDATE(), NULL, 0, NULL, N'Recruitment & Onboarding', N'Hiring process, candidate management, and onboarding', N'https://hrms.company.com/recruitment', 0, 0),
(7, GETDATE(), NULL, 0, NULL, N'Employee Self Service', N'Employee portal for personal information and requests', N'https://hrms.company.com/selfservice', 0, 0),
(8, GETDATE(), NULL, 0, NULL, N'Compliance & Reporting', N'Regulatory compliance, audit trails, and statutory reports', N'https://hrms.company.com/customersupport', 0, 0)
GO
SET IDENTITY_INSERT [dbo].[Modules] OFF
GO

/****** Create SubModules Table ******/
CREATE TABLE SubModules (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    SubModuleName VARCHAR(50) NOT NULL,
    ModuleId INT,
    IsEnabled BIT DEFAULT 1,
    SubModuleParentId INT,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_SubModules PRIMARY KEY (Id)
);
GO
ALTER TABLE SubModules
ADD CONSTRAINT FK_SubModules_ModuleId FOREIGN KEY (ModuleId) REFERENCES Modules (Id);
GO
CREATE INDEX IX_SubModules_ModuleId ON SubModules (ModuleId);
GO

/****** Insert SubModules Data ******/
SET IDENTITY_INSERT [dbo].[SubModules] ON
GO
INSERT [dbo].[SubModules] ([Id], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [SubModuleName], [ModuleId], [IsEnabled], [SubModuleParentId], [IsDeleted])
VALUES 
(1, GETDATE(), NULL, NULL, NULL, N'FUNDAMENTALS', 1, 1, 0, 0),
(2, GETDATE(), NULL, NULL, NULL, N'HORIZONTALS', 1, 1, 0, 0),
(3, GETDATE(), NULL, NULL, NULL, N'Master Management', 1, 1, 1, 0),
(4, GETDATE(), NULL, NULL, NULL, N'Exit', 1, 1, 1, 0),
(5, GETDATE(), NULL, NULL, NULL, N'Onboarding', 1, 1, 1, 0),
(6, GETDATE(), NULL, NULL, NULL, N'Confirmation', 1, 1, 1, 0),
(7, GETDATE(), NULL, NULL, NULL, N'Admin Console', 1, 1, 1, 0),
(8, GETDATE(), NULL, NULL, NULL, N'Email', 1, 1, 2, 0),
(9, GETDATE(), NULL, NULL, NULL, N'Notifications and Alerts', 1, 1, 2, 0),
(10, GETDATE(), NULL, NULL, NULL, N'Web Query', 1, 1, 2, 0),
(11, GETDATE(), NULL, NULL, NULL, N'Document Management', 1, 1, 2, 0),
(12, GETDATE(), NULL, NULL, NULL, N'Reporting Tool', 1, 1, 2, 0),
(13, GETDATE(), NULL, NULL, NULL, N'Questionnaries', 1, 1, 2, 0),
(14, GETDATE(), NULL, NULL, NULL, N'Letter Generation', 1, 1, 2, 0),
(15, GETDATE(), NULL, 1, NULL, N'Salary Structure', 2, 1, 0, 0),
(16, GETDATE(), NULL, 1, NULL, N'Payroll Processing', 2, 1, 0, 0),
(17, GETDATE(), NULL, 1, NULL, N'Tax Management', 2, 1, 0, 0),
(18, GETDATE(), NULL, 1, NULL, N'Provident Fund', 2, 1, 0, 0),
(19, GETDATE(), NULL, 1, NULL, N'Time Tracking', 3, 1, 0, 0),
(20, GETDATE(), NULL, 1, NULL, N'Leave Management', 3, 1, 0, 0),
(21, GETDATE(), NULL, 1, NULL, N'Shift Management', 3, 1, 0, 0),
(22, GETDATE(), NULL, 1, NULL, N'Overtime Management', 3, 1, 0, 0),
(23, GETDATE(), NULL, 1, NULL, N'Goal Setting', 4, 1, 0, 0),
(24, GETDATE(), NULL, 1, NULL, N'Performance Reviews', 4, 1, 0, 0),
(25, GETDATE(), NULL, 1, NULL, N'360 Feedback', 4, 1, 0, 0),
(26, GETDATE(), NULL, 1, NULL, N'Training Programs', 5, 1, 0, 0),
(27, GETDATE(), NULL, 1, NULL, N'Skill Assessment', 5, 1, 0, 0),
(28, GETDATE(), NULL, 1, NULL, N'Certification Tracking', 5, 1, 0, 0),
(29, GETDATE(), NULL, 1, NULL, N'Job Posting', 6, 1, 0, 0),
(30, GETDATE(), NULL, 1, NULL, N'Candidate Management', 6, 1, 0, 0),
(31, GETDATE(), NULL, 1, NULL, N'Interview Scheduling', 6, 1, 0, 0),
(32, GETDATE(), NULL, 1, NULL, N'Onboarding Process', 6, 1, 0, 0),
(33, GETDATE(), NULL, 1, NULL, N'Personal Information', 7, 1, 0, 0),
(34, GETDATE(), NULL, 1, NULL, N'Leave Requests', 7, 1, 0, 0),
(35, GETDATE(), NULL, 1, NULL, N'Payslip Access', 7, 1, 0, 0),
(36, GETDATE(), NULL, 1, NULL, N'Statutory Reports', 8, 1, 0, 0),
(37, GETDATE(), NULL, 1, NULL, N'Audit Management', 8, 1, 0, 0),
(38, GETDATE(), NULL, 1, NULL, N'Policy Management', 8, 1, 0, 0)
GO
SET IDENTITY_INSERT [dbo].[SubModules] OFF
GO

/****** Create ModuleMasters Table ******/
CREATE TABLE [dbo].[ModuleMasters](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
    [CreatedBy] [int] NULL,
    [UpdatedBy] [int] NULL,
    [ModuleId] [int] NOT NULL,
    [MasterName] [varchar](255) NOT NULL,
    [IsEnabled] [bit] NOT NULL DEFAULT 1,
    [IsDeleted] [bit] NOT NULL DEFAULT 0,
    CONSTRAINT [PK_ModuleMasters] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ModuleMasters] WITH CHECK ADD CONSTRAINT [FK_ModuleMasters_ModuleId] FOREIGN KEY([ModuleId])
REFERENCES [dbo].[Modules] ([Id])
GO
ALTER TABLE [dbo].[ModuleMasters] CHECK CONSTRAINT [FK_ModuleMasters_ModuleId]
GO

/****** Insert ModuleMasters Data ******/
SET IDENTITY_INSERT [dbo].[ModuleMasters] ON
GO
INSERT [dbo].[ModuleMasters] ([Id], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModuleId], [MasterName], [IsEnabled], [IsDeleted])
VALUES 
(1, GETDATE(), NULL, 1, NULL, 1, N'Employee Categories', 1, 0),
(2, GETDATE(), NULL, 1, NULL, 1, N'Department Master', 1, 0),
(3, GETDATE(), NULL, 1, NULL, 1, N'Designation Master', 1, 0),
(4, GETDATE(), NULL, 1, NULL, 1, N'Location Master', 1, 0),
(5, GETDATE(), NULL, 1, NULL, 1, N'Pay Components', 1, 0),
(6, GETDATE(), NULL, 1, NULL, 1, N'Tax Slabs', 1, 0),
(7, GETDATE(), NULL, 1, NULL, 1, N'Bank Master', 1, 0),
(8, GETDATE(), NULL, 1, NULL, 2, N'Shift Types', 1, 0),
(9, GETDATE(), NULL, 1, NULL, 2, N'Leave Types', 1, 0),
(10, GETDATE(), NULL, 1, NULL, 3, N'Holiday Calendar', 1, 0),
(11, GETDATE(), NULL, 1, NULL, 3, N'KPI Master', 1, 0),
(12, GETDATE(), NULL, 1, NULL, 4, N'Rating Scale', 1, 0),
(13, GETDATE(), NULL, 1, NULL, 4, N'Course Catalog', 1, 0),
(14, GETDATE(), NULL, 1, NULL, 5, N'Trainer Master', 1, 0),
(15, GETDATE(), NULL, 1, NULL, 5, N'Job Roles', 1, 0),
(16, GETDATE(), NULL, 1, NULL, 6, N'Recruitment Sources', 1, 0),
(17, GETDATE(), NULL, 1, NULL, 6, N'Request Types', 1, 0),
(18, GETDATE(), NULL, 1, NULL, 7, N'Compliance Checklist', 1, 0),
(19, GETDATE(), NULL, 1, NULL, 7, N'Compliance Checklist', 1, 0),
(20, GETDATE(), NULL, 1, NULL, 8, N'Compliance Checklist', 1, 0),
(21, GETDATE(), NULL, 1, NULL, 8, N'Compliance Checklist', 1, 0)
GO
SET IDENTITY_INSERT [dbo].[ModuleMasters] OFF
GO

/****** Create ModuleScreens Table ******/
CREATE TABLE [dbo].[ModuleScreens](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
    [CreatedBy] [int] NULL,
    [UpdatedBy] [int] NULL,
    [ModuleId] [int] NOT NULL,
    [ScreenName] [varchar](255) NOT NULL,
    [IsEnabled] [bit] NOT NULL DEFAULT 1,
    [IsDeleted] [bit] NOT NULL DEFAULT 0,
    CONSTRAINT [PK_ModuleScreens] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ModuleScreens] WITH CHECK ADD CONSTRAINT [FK_ModuleScreens_ModuleId] FOREIGN KEY([ModuleId])
REFERENCES [dbo].[Modules] ([Id])
GO
ALTER TABLE [dbo].[ModuleScreens] CHECK CONSTRAINT [FK_ModuleScreens_ModuleId]
GO

/****** Insert ModuleScreens Data ******/
SET IDENTITY_INSERT [dbo].[ModuleScreens] ON
GO
INSERT [dbo].[ModuleScreens] ([Id], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [ModuleId], [ScreenName], [IsEnabled], [IsDeleted])
VALUES 
(1, GETDATE(), NULL, 1, NULL, 1, N'Employee Profile', 1, 0),
(2, GETDATE(), NULL, 1, NULL, 1, N'Employee Search', 1, 0),
(3, GETDATE(), NULL, 1, NULL, 1, N'Organization Chart', 1, 0),
(4, GETDATE(), NULL, 1, NULL, 1, N'Employee Reports', 1, 0),
(5, GETDATE(), NULL, 1, NULL, 1, N'Payroll Dashboard', 1, 0),
(6, GETDATE(), NULL, 1, NULL, 1, N'Salary Register', 1, 0),
(7, GETDATE(), NULL, 1, NULL, 1, N'Payroll Reports', 1, 0),
(8, GETDATE(), NULL, 1, NULL, 2, N'Attendance Dashboard', 1, 0),
(9, GETDATE(), NULL, 1, NULL, 2, N'Time Sheet', 1, 0),
(10, GETDATE(), NULL, 1, NULL, 3, N'Leave Calendar', 1, 0),
(11, GETDATE(), NULL, 1, NULL, 3, N'Performance Dashboard', 1, 0),
(12, GETDATE(), NULL, 1, NULL, 4, N'Appraisal Forms', 1, 0),
(13, GETDATE(), NULL, 1, NULL, 4, N'Training Dashboard', 1, 0),
(14, GETDATE(), NULL, 1, NULL, 5, N'Course Catalog', 1, 0),
(15, GETDATE(), NULL, 1, NULL, 5, N'Recruitment Dashboard', 1, 0),
(16, GETDATE(), NULL, 1, NULL, 6, N'Candidate Pipeline', 1, 0),
(17, GETDATE(), NULL, 1, NULL, 6, N'Employee Portal', 1, 0),
(18, GETDATE(), NULL, 1, NULL, 7, N'Self Service Dashboard', 1, 0),
(19, GETDATE(), NULL, 1, NULL, 7, N'Compliance Dashboard', 1, 0),
(20, GETDATE(), NULL, 1, NULL, 8, N'Audit Reports', 1, 0),
(21, GETDATE(), NULL, 1, NULL, 8, N'Audit Reports', 1, 0)
GO
SET IDENTITY_INSERT [dbo].[ModuleScreens] OFF
GO

/****** Create SubModuleMasters Table ******/
CREATE TABLE [dbo].[SubModuleMasters](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
    [CreatedBy] [int] NULL,
    [UpdatedBy] [int] NULL,
    [SubModuleId] [int] NOT NULL,
    [MasterName] [varchar](255) NOT NULL,
    [IsEnabled] [bit] NOT NULL DEFAULT 1,
    [IsDeleted] [bit] NOT NULL DEFAULT 0,
    CONSTRAINT [PK_SubModuleMasters] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[SubModuleMasters] WITH CHECK ADD CONSTRAINT [FK_SubModuleMasters_SubModuleId] FOREIGN KEY([SubModuleId])
REFERENCES [dbo].[SubModules] ([Id])
GO
ALTER TABLE [dbo].[SubModuleMasters] CHECK CONSTRAINT [FK_SubModuleMasters_SubModuleId]
GO

/****** Insert SubModuleMasters Data ******/
SET IDENTITY_INSERT [dbo].[SubModuleMasters] ON
GO
INSERT [dbo].[SubModuleMasters] ([Id], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [SubModuleId], [MasterName], [IsEnabled], [IsDeleted])
VALUES 
(1, GETDATE(), NULL, 1, NULL, 3, N'Document Types', 1, 0),
(2, GETDATE(), NULL, 1, NULL, 3, N'Document Categories', 1, 0),
(3, GETDATE(), NULL, 1, NULL, 4, N'Exit Reasons', 1, 0),
(4, GETDATE(), NULL, 1, NULL, 4, N'Exit Checklist', 1, 0),
(5, GETDATE(), NULL, 1, NULL, 5, N'Onboarding Stages', 1, 0),
(6, GETDATE(), NULL, 1, NULL, 5, N'Onboarding Documents', 1, 0),
(7, GETDATE(), NULL, 1, NULL, 6, N'Confirmation Criteria', 1, 0),
(8, GETDATE(), NULL, 1, NULL, 6, N'Probation Policies', 1, 0),
(9, GETDATE(), NULL, 1, NULL, 7, N'Admin Roles', 1, 0),
(10, GETDATE(), NULL, 1, NULL, 7, N'Access Levels', 1, 0),
(11, GETDATE(), NULL, 1, NULL, 8, N'Email Templates', 1, 0),
(12, GETDATE(), NULL, 1, NULL, 8, N'Email Categories', 1, 0),
(13, GETDATE(), NULL, 1, NULL, 9, N'Notification Types', 1, 0),
(14, GETDATE(), NULL, 1, NULL, 9, N'Alert Priorities', 1, 0),
(15, GETDATE(), NULL, 1, NULL, 10, N'Query Types', 1, 0),
(16, GETDATE(), NULL, 1, NULL, 10, N'Query Status', 1, 0),
(17, GETDATE(), NULL, 1, NULL, 11, N'Document Formats', 1, 0),
(18, GETDATE(), NULL, 1, NULL, 11, N'Storage Locations', 1, 0),
(19, GETDATE(), NULL, 1, NULL, 12, N'Report Types', 1, 0),
(20, GETDATE(), NULL, 1, NULL, 12, N'Report Filters', 1, 0),
(21, GETDATE(), NULL, 1, NULL, 13, N'Questionnaire Types', 1, 0),
(22, GETDATE(), NULL, 1, NULL, 13, N'Question Categories', 1, 0),
(23, GETDATE(), NULL, 1, NULL, 14, N'Letter Templates', 1, 0),
(24, GETDATE(), NULL, 1, NULL, 14, N'Letter Formats', 1, 0),
(25, GETDATE(), NULL, 1, NULL, 15, N'Allowance Types', 1, 0),
(26, GETDATE(), NULL, 1, NULL, 15, N'Asset Types', 1, 0),
(27, GETDATE(), NULL, 1, NULL, 16, N'Payroll Frequency', 1, 0),
(28, GETDATE(), NULL, 1, NULL, 16, N'Payroll Status', 1, 0),
(29, GETDATE(), NULL, 1, NULL, 17, N'Tax Exemption Sections', 1, 0),
(30, GETDATE(), NULL, 1, NULL, 17, N'Investment Types', 1, 0),
(31, GETDATE(), NULL, 1, NULL, 18, N'PF Scheme Types', 1, 0),
(32, GETDATE(), NULL, 1, NULL, 19, N'Clock Types', 1, 0),
(33, GETDATE(), NULL, 1, NULL, 19, N'Biometric Devices', 1, 0),
(34, GETDATE(), NULL, 1, NULL, 20, N'Leave Policies', 1, 0),
(35, GETDATE(), NULL, 1, NULL, 20, N'Leave Approval Matrix', 1, 0),
(36, GETDATE(), NULL, 1, NULL, 21, N'Shift Patterns', 1, 0),
(37, GETDATE(), NULL, 1, NULL, 21, N'Roster Templates', 1, 0),
(38, GETDATE(), NULL, 1, NULL, 22, N'Overtime Policies', 1, 0),
(39, GETDATE(), NULL, 1, NULL, 23, N'Goal Categories', 1, 0),
(40, GETDATE(), NULL, 1, NULL, 23, N'Goal Priorities', 1, 0),
(41, GETDATE(), NULL, 1, NULL, 24, N'Review Cycles', 1, 0),
(42, GETDATE(), NULL, 1, NULL, 24, N'Review Templates', 1, 0),
(43, GETDATE(), NULL, 1, NULL, 25, N'Feedback Types', 1, 0),
(44, GETDATE(), NULL, 1, NULL, 26, N'Training Categories', 1, 0),
(45, GETDATE(), NULL, 1, NULL, 26, N'Training Methods', 1, 0),
(46, GETDATE(), NULL, 1, NULL, 27, N'Skill Categories', 1, 0),
(47, GETDATE(), NULL, 1, NULL, 27, N'Competency Levels', 1, 0),
(48, GETDATE(), NULL, 1, NULL, 28, N'Certification Bodies', 1, 0),
(49, GETDATE(), NULL, 1, NULL, 29, N'Job Boards', 1, 0),
(50, GETDATE(), NULL, 1, NULL, 29, N'Job Categories', 1, 0),
(51, GETDATE(), NULL, 1, NULL, 30, N'Candidate Sources', 1, 0),
(52, GETDATE(), NULL, 1, NULL, 30, N'Application Status', 1, 0),
(53, GETDATE(), NULL, 1, NULL, 31, N'Interview Types', 1, 0),
(54, GETDATE(), NULL, 1, NULL, 31, N'Interview Rounds', 1, 0),
(55, GETDATE(), NULL, 1, NULL, 32, N'Onboarding Tasks', 1, 0),
(56, GETDATE(), NULL, 1, NULL, 32, N'Welcome Kit Items', 1, 0),
(57, GETDATE(), NULL, 1, NULL, 33, N'Address Types', 1, 0),
(58, GETDATE(), NULL, 1, NULL, 33, N'Contact Types', 1, 0),
(59, GETDATE(), NULL, 1, NULL, 34, N'Request Categories', 1, 0),
(60, GETDATE(), NULL, 1, NULL, 35, N'Payslip Templates', 1, 0),
(61, GETDATE(), NULL, 1, NULL, 36, N'Report Templates', 1, 0),
(62, GETDATE(), NULL, 1, NULL, 36, N'Statutory Forms', 1, 0),
(63, GETDATE(), NULL, 1, NULL, 37, N'Audit Types', 1, 0),
(64, GETDATE(), NULL, 1, NULL, 37, N'Finding Categories', 1, 0),
(65, GETDATE(), NULL, 1, NULL, 38, N'Policy Categories', 1, 0),
(66, GETDATE(), NULL, 1, NULL, 38, N'Policy Versions', 1, 0)
GO
SET IDENTITY_INSERT [dbo].[SubModuleMasters] OFF
GO

/****** Create SubModuleScreens Table ******/
CREATE TABLE [dbo].[SubModuleScreens](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] [datetime] NULL,
    [CreatedBy] [int] NULL,
    [UpdatedBy] [int] NULL,
    [SubModuleId] [int] NOT NULL,
    [SubModuleMasterId] [int] NOT NULL,
    [ScreenName] [varchar](255) NOT NULL,
    [IsEnabled] [bit] NOT NULL DEFAULT 1,
    [IsDeleted] [bit] NOT NULL DEFAULT 0,
    CONSTRAINT [PK_SubModuleScreens] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[SubModuleScreens] WITH CHECK ADD CONSTRAINT [FK_SubModuleScreens_SubModuleId] FOREIGN KEY([SubModuleId])
REFERENCES [dbo].[SubModules] ([Id])
GO
ALTER TABLE [dbo].[SubModuleScreens] CHECK CONSTRAINT [FK_SubModuleScreens_SubModuleId]
GO
ALTER TABLE [dbo].[SubModuleScreens] WITH CHECK ADD CONSTRAINT [FK_SubModuleScreens_SubModuleMasterId] FOREIGN KEY([SubModuleMasterId])
REFERENCES [dbo].[SubModuleMasters] ([Id])
GO
ALTER TABLE [dbo].[SubModuleScreens] CHECK CONSTRAINT [FK_SubModuleScreens_SubModuleMasterId]
GO

/****** Insert SubModuleScreens Data ******/
SET IDENTITY_INSERT [dbo].[SubModuleScreens] ON
GO
INSERT [dbo].[SubModuleScreens] ([Id], [CreatedAt], [UpdatedAt], [CreatedBy], [UpdatedBy], [SubModuleId], [SubModuleMasterId], [ScreenName], [IsEnabled], [IsDeleted])
VALUES 
(1, GETDATE(), NULL, 1, NULL, 3, 1, N'Document Upload', 1, 0),
(2, GETDATE(), NULL, 1, NULL, 3, 2, N'Document Verification', 1, 0),
(3, GETDATE(), NULL, 1, NULL, 4, 3, N'Exit Interview', 1, 0),
(4, GETDATE(), NULL, 1, NULL, 4, 4, N'Clearance Form', 1, 0),
(5, GETDATE(), NULL, 1, NULL, 5, 5, N'Onboarding Dashboard', 1, 0),
(6, GETDATE(), NULL, 1, NULL, 5, 6, N'Document Submission', 1, 0),
(7, GETDATE(), NULL, 1, NULL, 6, 7, N'Confirmation Review', 1, 0),
(8, GETDATE(), NULL, 1, NULL, 6, 8, N'Probation Status', 1, 0),
(9, GETDATE(), NULL, 1, NULL, 7, 9, N'Admin Dashboard', 1, 0),
(10, GETDATE(), NULL, 1, NULL, 7, 10, N'Access Management', 1, 0),
(11, GETDATE(), NULL, 1, NULL, 8, 11, N'Email Composition', 1, 0),
(12, GETDATE(), NULL, 1, NULL, 8, 12, N'Email Template Management', 1, 0),
(13, GETDATE(), NULL, 1, NULL, 9, 13, N'Notification Dashboard', 1, 0),
(14, GETDATE(), NULL, 1, NULL, 9, 14, N'Alert Configuration', 1, 0),
(15, GETDATE(), NULL, 1, NULL, 10, 15, N'Query Submission', 1, 0),
(16, GETDATE(), NULL, 1, NULL, 10, 16, N'Query Tracking', 1, 0),
(17, GETDATE(), NULL, 1, NULL, 11, 17, N'Document Repository', 1, 0),
(18, GETDATE(), NULL, 1, NULL, 11, 18, N'Document Search', 1, 0),
(19, GETDATE(), NULL, 1, NULL, 12, 19, N'Report Generation', 1, 0),
(20, GETDATE(), NULL, 1, NULL, 12, 20, N'Report Customization', 1, 0),
(21, GETDATE(), NULL, 1, NULL, 13, 21, N'Questionnaire Builder', 1, 0),
(22, GETDATE(), NULL, 1, NULL, 13, 22, N'Questionnaire Responses', 1, 0),
(23, GETDATE(), NULL, 1, NULL, 14, 23, N'Letter Drafting', 1, 0),
(24, GETDATE(), NULL, 1, NULL, 14, 24, N'Letter Preview', 1, 0),
(25, GETDATE(), NULL, 1, NULL, 15, 25, N'Salary Structure Setup', 1, 0),
(26, GETDATE(), NULL, 1, NULL, 15, 26, N'Pay Component Master', 1, 0),
(27, GETDATE(), NULL, 1, NULL, 16, 27, N'Payroll Processing', 1, 0),
(28, GETDATE(), NULL, 1, NULL, 16, 28, N'Payroll Validation', 1, 0),
(29, GETDATE(), NULL, 1, NULL, 17, 29, N'Tax Declaration', 1, 0),
(30, GETDATE(), NULL, 1, NULL, 17, 30, N'Investment Proof Upload', 1, 0),
(31, GETDATE(), NULL, 1, NULL, 18, 31, N'PF Calculation', 1, 0),
(32, GETDATE(), NULL, 1, NULL, 19, 32, N'Time Punch', 1, 0),
(33, GETDATE(), NULL, 1, NULL, 19, 33, N'Attendance Regularization', 1, 0),
(34, GETDATE(), NULL, 1, NULL, 20, 34, N'Leave Application', 1, 0),
(35, GETDATE(), NULL, 1, NULL, 20, 35, N'Leave Approval', 1, 0),
(36, GETDATE(), NULL, 1, NULL, 21, 36, N'Shift Assignment', 1, 0),
(37, GETDATE(), NULL, 1, NULL, 21, 37, N'Roster Planning', 1, 0),
(38, GETDATE(), NULL, 1, NULL, 22, 38, N'Overtime Request', 1, 0),
(39, GETDATE(), NULL, 1, NULL, 23, 39, N'Goal Setting', 1, 0),
(40, GETDATE(), NULL, 1, NULL, 23, 40, N'Goal Tracking', 1, 0),
(41, GETDATE(), NULL, 1, NULL, 24, 41, N'Performance Review', 1, 0),
(42, GETDATE(), NULL, 1, NULL, 24, 42, N'Review Dashboard', 1, 0),
(43, GETDATE(), NULL, 1, NULL, 25, 43, N'360 Feedback Form', 1, 0),
(44, GETDATE(), NULL, 1, NULL, 26, 44, N'Training Calendar', 1, 0),
(45, GETDATE(), NULL, 1, NULL, 26, 45, N'Training Enrollment', 1, 0),
(46, GETDATE(), NULL, 1, NULL, 27, 46, N'Skill Matrix', 1, 0),
(47, GETDATE(), NULL, 1, NULL, 27, 47, N'Competency Assessment', 1, 0),
(48, GETDATE(), NULL, 1, NULL, 28, 48, N'Certificate Management', 1, 0),
(49, GETDATE(), NULL, 1, NULL, 29, 49, N'Job Posting', 1, 0),
(50, GETDATE(), NULL, 1, NULL, 29, 50, N'Job Applications', 1, 0),
(51, GETDATE(), NULL, 1, NULL, 30, 51, N'Candidate Screening', 1, 0),
(52, GETDATE(), NULL, 1, NULL, 30, 52, N'Application Tracking', 1, 0),
(53, GETDATE(), NULL, 1, NULL, 31, 53, N'Interview Scheduling', 1, 0),
(54, GETDATE(), NULL, 1, NULL, 31, 54, N'Interview Feedback', 1, 0),
(55, GETDATE(), NULL, 1, NULL, 32, 55, N'Onboarding Checklist', 1, 0),
(56, GETDATE(), NULL, 1, NULL, 32, 56, N'Welcome Kit Setup', 1, 0),
(57, GETDATE(), NULL, 1, NULL, 33, 57, N'Personal Information', 1, 0),
(58, GETDATE(), NULL, 1, NULL, 33, 58, N'Contact Management', 1, 0),
(59, GETDATE(), NULL, 1, NULL, 34, 59, N'Leave Request', 1, 0),
(60, GETDATE(), NULL, 1, NULL, 35, 60, N'Payslip Download', 1, 0),
(61, GETDATE(), NULL, 1, NULL, 36, 61, N'Statutory Reports', 1, 0),
(62, GETDATE(), NULL, 1, NULL, 36, 62, N'Form Generation', 1, 0),
(63, GETDATE(), NULL, 1, NULL, 37, 63, N'Audit Planning', 1, 0),
(64, GETDATE(), NULL, 1, NULL, 37, 64, N'Finding Management', 1, 0),
(65, GETDATE(), NULL, 1, NULL, 38, 65, N'Policy Repository', 1, 0),
(66, GETDATE(), NULL, 1, NULL, 38, 66, N'Version Control', 1, 0)
GO
SET IDENTITY_INSERT [dbo].[SubModuleScreens] OFF
GO

-- ModuleMapping Table
IF OBJECT_ID('ModuleMapping') IS NOT NULL
    DROP TABLE ModuleMapping;
GO
CREATE TABLE ModuleMapping (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    CompanyId INT NOT NULL,
    ModuleId INT NOT NULL,
    SubModuleId INT,
    IsEnabled BIT DEFAULT 1,
    IsMandatory BIT DEFAULT 0,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_ModuleMapping PRIMARY KEY (Id)
);
GO
ALTER TABLE ModuleMapping
ADD CONSTRAINT FK_ModuleMapping_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id),
    CONSTRAINT FK_ModuleMapping_ModuleId FOREIGN KEY (ModuleId) REFERENCES Modules (Id),
    CONSTRAINT FK_ModuleMapping_SubModuleId FOREIGN KEY (SubModuleId) REFERENCES SubModules (Id);
GO
CREATE INDEX IX_ModuleMapping_CompanyId ON ModuleMapping (CompanyId);
GO
CREATE INDEX IX_ModuleMapping_ModuleId ON ModuleMapping (ModuleId);
GO

-- ModuleMappingHistory Table
IF OBJECT_ID('ModuleMappingHistory') IS NOT NULL
    DROP TABLE ModuleMappingHistory;
GO
CREATE TABLE ModuleMappingHistory (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    CompanyId INT NOT NULL,
    ModuleId INT NOT NULL,
    SubModuleId INT,
    IsEnabled BIT DEFAULT 1,
    IsMandatory BIT DEFAULT 0,
    Version INT,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_ModuleMappingHistory PRIMARY KEY (Id)
);
GO
ALTER TABLE ModuleMappingHistory
ADD CONSTRAINT FK_ModuleMappingHistory_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id),
    CONSTRAINT FK_ModuleMappingHistory_ModuleId FOREIGN KEY (ModuleId) REFERENCES Modules (Id),
    CONSTRAINT FK_ModuleMappingHistory_SubModuleId FOREIGN KEY (SubModuleId) REFERENCES SubModules (Id);
GO
CREATE INDEX IX_ModuleMappingHistory_CompanyId ON ModuleMappingHistory (CompanyId);
GO
CREATE INDEX IX_ModuleMappingHistory_ModuleId ON ModuleMappingHistory (ModuleId);
GO


-- Drop ActiveSession table if it exists
IF OBJECT_ID('ActiveSession') IS NOT NULL
    DROP TABLE ActiveSession;
GO

-- Create ActiveSession table
CREATE TABLE ActiveSession (
    Id UNIQUEIDENTIFIER NOT NULL,              -- Session ID (GUID)
    UserId INT NOT NULL,                       -- User ID
    Token NVARCHAR(MAX) NOT NULL,              -- Token string
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),  -- Session creation time
    ExpiresAt DATETIME NOT NULL,               -- Session expiration time
    IsActive BIT NOT NULL DEFAULT 1,           -- Active status
    IsDeleted BIT NOT NULL DEFAULT 0,          -- Soft delete flag
    CONSTRAINT PK_ActiveSession PRIMARY KEY (Id)
);
GO

---- Add foreign key constraint (assuming Users table exists)
--ALTER TABLE ActiveSession
--ADD CONSTRAINT FK_ActiveSession_UserId FOREIGN KEY (UserId) REFERENCES Users (Id);
--GO

-- Create index on UserId
CREATE INDEX IX_ActiveSession_UserId ON ActiveSession (UserId);
GO


--Store Procedure for Clone TenantDatabase
/****** Object:  StoredProcedure [dbo].[CloneElixirTenantDatabase]    Script Date: 7/11/2025 10:23:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CloneElixirTenantDatabase]
    @SourceDB NVARCHAR(128),
    @TargetDB NVARCHAR(128),
    @ElasticPool NVARCHAR(128)
AS
BEGIN
    DECLARE @SQL NVARCHAR(MAX);
	IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = @TargetDB)
    BEGIN
        SET @SQL = 'CREATE DATABASE [' + @TargetDB + '] AS COPY OF [' + @SourceDB + ']
        ( SERVICE_OBJECTIVE = ELASTIC_POOL(name = [' + @ElasticPool + '] ))';
    END
    EXEC(@SQL);
END



