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
    CountryShortName VARCHAR(10) NOT NULL,
    Description VARCHAR(MAX),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_CountryMaster PRIMARY KEY (Id),
    CONSTRAINT UQ_CountryMaster_CountryName UNIQUE (CountryName),
    CONSTRAINT UQ_CountryMaster_CountryShortName UNIQUE (CountryShortName),
    CONSTRAINT UQ_CountryMaster_CountryName_CountryShortName UNIQUE (CountryName, CountryShortName)
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
    TelephoneCode VARCHAR(5) NOT NULL,
    Description VARCHAR(MAX),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_TelephoneCodeMaster PRIMARY KEY (Id),
    CONSTRAINT UQ_TelephoneCodeMaster_CountryId_TelephoneCode UNIQUE (CountryId, TelephoneCode)
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
    StateShortName VARCHAR(10) NOT NULL,
    Description VARCHAR(MAX),
    IsDeleted BIT NOT NULL DEFAULT 1,
    CONSTRAINT PK_StateMaster PRIMARY KEY (Id),
    CONSTRAINT UQ_StateMaster_CountryId_StateName_StateShortName UNIQUE (CountryId, StateName, StateShortName),
    CONSTRAINT UQ_StateMaster_StateShortName UNIQUE (StateShortName)
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
    CurrencyShortName VARCHAR(5) NOT NULL,
    Description VARCHAR(MAX),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_CurrencyMaster PRIMARY KEY (Id),
    CONSTRAINT UQ_CurrencyMaster_CountryId_CurrencyName_CurrencyShortName UNIQUE (CountryId, CurrencyName, CurrencyShortName)
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
    IsActive BIT NOT NULL DEFAULT 1,
    CategoryName VARCHAR(255) NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_Category PRIMARY KEY (Id)
);
GO

INSERT INTO Category (CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, CategoryName, IsActive, IsDeleted)
VALUES 
(GETDATE(), GETDATE(), 1, 1, 'Time Management', 0, 0),
(GETDATE(), GETDATE(), 1, 1, 'Exit', 0, 0);
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
('countrymaster', GETDATE(), GETDATE(), NULL, NULL, '/common-master/country-master', 0),
('statemaster', GETDATE(), GETDATE(), NULL, NULL, '/common-master/state-master', 0),
('currencymaster', GETDATE(), GETDATE(), NULL, NULL, '/common-master/currency-master', 0),
('telephonecodemaster', GETDATE(), GETDATE(), NULL, NULL, '/common-master/telephone-master', 0),
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
    IsActive BIT NOT NULL DEFAULT 1,
    ClientName VARCHAR(50) NOT NULL,
    ClientInfo VARCHAR(50),
    IsEnabled BIT DEFAULT 1,
    ClientCode VARCHAR(12) NOT NULL,
    Address1 VARCHAR(255),
    Address2 VARCHAR(255),
    StateId INT,
    CountryId INT,
    ZipCode VARCHAR(20),
    PhoneNumber VARCHAR(12),
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
    'Default Client', 'CLT001', 1, GETDATE(), GETDATE(), 0
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
ADD CONSTRAINT FK_ClientAccess_ClientId FOREIGN KEY (ClientId) REFERENCES Clients (Id) ON DELETE CASCADE;
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
    PhoneNumber VARCHAR(12),
    Email VARCHAR(45),
    Designation VARCHAR(50),
    ClientId INT,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_ClientAdminInfo PRIMARY KEY (Id)
);
GO
ALTER TABLE ClientAdminInfo
ADD CONSTRAINT FK_ClientAdminInfo_ClientId FOREIGN KEY (ClientId) REFERENCES Clients (Id) ON DELETE CASCADE,
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
    PhoneNumber VARCHAR(12),
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
ADD CONSTRAINT FK_ClientContactDetails_ClientId FOREIGN KEY (ClientId) REFERENCES Clients (Id) ON DELETE CASCADE,
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
ADD CONSTRAINT FK_ClientReportingToolLimits_ClientId FOREIGN KEY (ClientId) REFERENCES Clients (Id) ON DELETE CASCADE;
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
    IsActive BIT NOT NULL DEFAULT 1,
    CompanyName VARCHAR(50),
    CompanyCode VARCHAR(50),
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
    PhoneNumber VARCHAR(12),
    BillingAddressSameAsCompany BIT DEFAULT 0,
    BillingAddress1 VARCHAR(255),
    BillingAddress2 VARCHAR(255),
    BillingStateId INT,
    BillingZipCode VARCHAR(20),
    BillingCountryId INT,
    BillingTelephoneCodeId INT,
    BillingPhoneNumber VARCHAR(12),
    MfaEnabled BIT DEFAULT 0,
    MfaEmail BIT DEFAULT 0,
    MfaSms BIT DEFAULT 0,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_Companies PRIMARY KEY (Id),
    CONSTRAINT UQ_Companies_CompanyCode UNIQUE (CompanyCode),
    CONSTRAINT UQ_Companies_CompanyName UNIQUE (CompanyName)
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
    'Default Company', 'DEF001', 1, 1, GETDATE(), GETDATE()
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
    PhoneNumber VARCHAR(12),
    BillingAddressSameAsCompany BIT DEFAULT 0,
    BillingAddress1 VARCHAR(255),
    BillingAddress2 VARCHAR(255),
    BillingStateId INT,
    BillingZipCode VARCHAR(20),
    BillingCountryId INT,
    BillingTelephoneCodeId INT,
    BillingPhoneNumber VARCHAR(12),
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
ADD CONSTRAINT FK_Account_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id) ON DELETE CASCADE;
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
ADD CONSTRAINT FK_AccountHistory_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id) ON DELETE CASCADE;
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
ADD CONSTRAINT FK_CompanyOnboardingStatus_ClientId FOREIGN KEY (ClientId) REFERENCES Clients (Id) ON DELETE CASCADE,
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
ADD CONSTRAINT FK_ClientCompaniesMapping_ClientId FOREIGN KEY (ClientId) REFERENCES Clients (Id) ON DELETE CASCADE,
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
    PhoneNumber VARCHAR(12),
    Email VARCHAR(255) NOT NULL,
    Designation VARCHAR(50),
    Department VARCHAR(255),
    Remarks VARCHAR(MAX),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_EscalationContacts PRIMARY KEY (Id)
);
GO
ALTER TABLE EscalationContacts
ADD CONSTRAINT FK_EscalationContacts_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id) ON DELETE CASCADE,
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
    PhoneNumber VARCHAR(12),
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
ADD CONSTRAINT FK_EscalationContactsHistory_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id) ON DELETE CASCADE,
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
    IsActive BIT NOT NULL DEFAULT 1,
    RoleName VARCHAR(50) NOT NULL,
    Description VARCHAR(500),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_Roles PRIMARY KEY (Id)
);
GO
INSERT INTO Roles (RoleName, Description, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsActive) 
VALUES 
    ('SuperAdmin', 'Administrator role with full access rights.', GETDATE(), NULL, GETDATE(), NULL, 1),
    ('Account Manager', 'Role with permissions to edit content.', GETDATE(), NULL, GETDATE(), NULL, 1),
    ('Checker', 'Role with Approve/Reject Rights', GETDATE(), NULL, GETDATE(), NULL, 1),
    ('Migration User', 'Role with Approve/Reject Rights', GETDATE(), NULL, GETDATE(), NULL, 1),
    ('Delegate User', 'Role with Delete Rights', GETDATE(), NULL, GETDATE(), NULL, 1),
    ('Company Admin', 'Company Rights', GETDATE(), NULL, GETDATE(), NULL, 1);
GO

-- Users Table
IF OBJECT_ID('Users') IS NOT NULL
    DROP TABLE Users;
GO
CREATE TABLE Users (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    IsActive BIT NOT NULL DEFAULT 1,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    Email VARCHAR(255) NOT NULL,
	EmailHash int NOT NULL,
    PasswordHash VARCHAR(255),
	Salt VARCHAR(64) NULL,
    TelephoneCodeId INT,
    PhoneNumber VARCHAR(15) NOT NULL,
    Location VARCHAR(100),
    Designation VARCHAR(100) NOT NULL,
    ProfilePicture VARCHAR(MAX),
    RoleId INT DEFAULT 5,
    IsEnabled BIT DEFAULT 1,
    UserStorageConsumed DECIMAL(10, 2) DEFAULT 0.00,
    FailedLoginAttempts INT DEFAULT 0,
    LastFailedAttempt DATETIME,
    IsLockedOut BIT NOT NULL DEFAULT 0,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_Users PRIMARY KEY (Id),
    CONSTRAINT UQ_Users_Email UNIQUE (Email)
);
GO

CREATE INDEX IX_Users_EmailHash ON Users(EmailHash);
GO
ALTER TABLE Users
ADD CONSTRAINT FK_Users_RoleId FOREIGN KEY (RoleId) REFERENCES Roles (Id),
    CONSTRAINT FK_Users_TelephoneCodeId FOREIGN KEY (TelephoneCodeId) REFERENCES TelephoneCodeMaster (Id);
GO
CREATE INDEX IX_Users_RoleId ON Users (RoleId);
GO

INSERT INTO Users (
    FirstName, LastName, Email,EmailHash,Salt, PasswordHash, TelephoneCodeId, PhoneNumber,
    Location, Designation, ProfilePicture, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy,
    RoleId, IsActive, IsEnabled, UserStorageConsumed
) VALUES (
    'Kelly', 'Benny', 'superadmin@tmi.com',1608402803,'ckGZLYG5486TEYMQ+CtlJc4FSV2TXyel0XcCIn3Dhr0=',
    'd6c7c74e447323182d4fb7de40d7e30e724c0c03881bf48cbab6279e9fb98154',
    3, '7345678888', 'Japan', 'SE', NULL,
    GETDATE(), GETDATE(), NULL, NULL,
    1, 1, 1, 0.00
);
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
    IsActive BIT NOT NULL DEFAULT 1,
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
INSERT INTO UserGroups (
    GroupName, GroupType, CreatedBy, Description, IsEnabled, CreatedAt, UpdatedAt, UpdatedBy, IsActive
) VALUES
    ('Account Manager', 'Default', 1, '', 1, GETDATE(), NULL, NULL, 1),
    ('Checker', 'Default', 1, NULL, 1, GETDATE(), NULL, NULL, 1),
    ('Migration User', 'Default', 1, NULL, 1, GETDATE(), NULL, NULL, 1);
GO

-- ClientAccountManagers Table
IF OBJECT_ID('ClientAccountManagers') IS NOT NULL
    DROP TABLE ClientAccountManagers;
GO
CREATE TABLE ClientAccountManagers (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    ClientId INT NOT NULL,
    UserGroupId INT NOT NULL,
    AccountManagerUserId INT NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_ClientAccountManagers PRIMARY KEY (Id)
);
GO
ALTER TABLE ClientAccountManagers
ADD CONSTRAINT FK_ClientAccountManagers_ClientId FOREIGN KEY (ClientId) REFERENCES Clients (Id) ON DELETE CASCADE,
    CONSTRAINT FK_ClientAccountManagers_UserGroupId FOREIGN KEY (UserGroupId) REFERENCES UserGroups (Id) ON DELETE CASCADE,
    CONSTRAINT FK_ClientAccountManagers_AccountManagerUserId FOREIGN KEY (AccountManagerUserId) REFERENCES Users (Id) ON DELETE NO ACTION;
GO
CREATE INDEX IX_ClientAccountManagers_ClientId ON ClientAccountManagers (ClientId);
GO
CREATE INDEX IX_ClientAccountManagers_UserGroupId ON ClientAccountManagers (UserGroupId);
GO

-- AccountManagers Table
IF OBJECT_ID('AccountManagers') IS NOT NULL
    DROP TABLE AccountManagers;
GO
CREATE TABLE AccountManagers (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    IsActive BIT NOT NULL DEFAULT 1,
    CompanyId INT NOT NULL,
    UserId INT NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_AccountManagers PRIMARY KEY (Id)
);
GO
ALTER TABLE AccountManagers
ADD CONSTRAINT FK_AccountManagers_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id) ON DELETE CASCADE,
    CONSTRAINT FK_AccountManagers_UserId FOREIGN KEY (UserId) REFERENCES Users (Id) ON DELETE NO ACTION;
GO
CREATE INDEX IX_AccountManagers_CompanyId ON AccountManagers (CompanyId);
GO

-- Checkers Table
IF OBJECT_ID('Checkers') IS NOT NULL
    DROP TABLE Checkers;
GO
CREATE TABLE Checkers (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    CompanyId INT NOT NULL,
    UserId INT NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_Checkers PRIMARY KEY (Id)
);
GO
ALTER TABLE Checkers
ADD CONSTRAINT FK_Checkers_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id) ON DELETE CASCADE,
    CONSTRAINT FK_Checkers_UserId FOREIGN KEY (UserId) REFERENCES Users (Id) ON DELETE NO ACTION;
GO
CREATE INDEX IX_Checkers_CompanyId ON Checkers (CompanyId);
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
    TelephoneCodeId INT,
    PhoneNumber VARCHAR(15),
    Designation VARCHAR(100),
    RoleId INT,
    IsEnabled BIT DEFAULT 1,
    PasswordHash VARCHAR(255),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_CompanyAdminUsers PRIMARY KEY (Id)
);
GO
ALTER TABLE CompanyAdminUsers
ADD CONSTRAINT FK_CompanyAdminUsers_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id),
    CONSTRAINT FK_CompanyAdminUsers_RoleId FOREIGN KEY (RoleId) REFERENCES Roles (Id),
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
    PhoneNumber VARCHAR(15),
    Designation VARCHAR(100),
    RoleId INT,
    IsEnabled BIT DEFAULT 1,
    PasswordHash VARCHAR(255),
    Version INT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_CompanyAdminUsersHistory PRIMARY KEY (Id)
);
GO
ALTER TABLE CompanyAdminUsersHistory
ADD CONSTRAINT FK_CompanyAdminUsersHistory_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id),
    CONSTRAINT FK_CompanyAdminUsersHistory_RoleId FOREIGN KEY (RoleId) REFERENCES Roles (Id),
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
    CompanyId INT NOT NULL,
    UserGroupId INT NOT NULL,
    UserId INT NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_ElixirUsers PRIMARY KEY (Id)
);
GO
ALTER TABLE ElixirUsers
ADD CONSTRAINT FK_ElixirUsers_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id) ON DELETE CASCADE,
    CONSTRAINT FK_ElixirUsers_UserGroupId FOREIGN KEY (UserGroupId) REFERENCES UserGroups (Id) ON DELETE CASCADE,
    CONSTRAINT FK_ElixirUsers_UserId FOREIGN KEY (UserId) REFERENCES Users (Id) ON DELETE NO ACTION;
GO
CREATE INDEX IX_ElixirUsers_CompanyId ON ElixirUsers (CompanyId);
GO
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
    CompanyId INT NOT NULL,
    UserGroupId INT NOT NULL,
    UserId INT NOT NULL,
    Version INT,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_ElixirUsersHistory PRIMARY KEY (Id)
);
GO
ALTER TABLE ElixirUsersHistory
ADD CONSTRAINT FK_ElixirUsersHistory_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id) ON DELETE CASCADE,
    CONSTRAINT FK_ElixirUsersHistory_UserGroupId FOREIGN KEY (UserGroupId) REFERENCES UserGroups (Id) ON DELETE CASCADE,
    CONSTRAINT FK_ElixirUsersHistory_UserId FOREIGN KEY (UserId) REFERENCES Users (Id) ON DELETE NO ACTION;
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
ADD CONSTRAINT FK_UserGroupMappings_UserGroupId FOREIGN KEY (UserGroupId) REFERENCES UserGroups (Id) ON DELETE CASCADE,
    CONSTRAINT FK_UserGroupMappings_UserId FOREIGN KEY (UserId) REFERENCES Users (Id) ON DELETE CASCADE;
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
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_UserPasswordHistory PRIMARY KEY (Id)
);
GO
ALTER TABLE UserPasswordHistory
ADD CONSTRAINT FK_UserPasswordHistory_UserId FOREIGN KEY (UserId) REFERENCES Users (Id) ON DELETE CASCADE;
GO
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
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_Horizontals PRIMARY KEY (Id)
);
GO
ALTER TABLE Horizontals
ADD CONSTRAINT FK_Horizontals_UserGroupId FOREIGN KEY (UserGroupId) REFERENCES UserGroups (Id) ON DELETE CASCADE;
GO
CREATE INDEX IX_Horizontals_UserGroupId ON Horizontals (UserGroupId);
GO

INSERT INTO Horizontals (CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, UserGroupId, HorizontalName, Description)
VALUES 
(GETDATE(), GETDATE(), 1, 1, 1, 'Letter Generation Admin', 'Letter Template'),
(GETDATE(), GETDATE(), 1, 1, 1, 'Document Management Admin', 'Document'),
(GETDATE(), GETDATE(), 1, 1, 1, 'Web Query Admin', 'Web Query'),
(GETDATE(), GETDATE(), 1, 1, 1, 'Email Admin', 'Email'),
(GETDATE(), GETDATE(), 1, 1, 1, 'Reporting Admin', 'Report');
GO

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
ADD CONSTRAINT FK_WebQueryHorizontalCheckboxItems_HorizontalId FOREIGN KEY (HorizontalId) REFERENCES Horizontals (Id) ON DELETE CASCADE;
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
    IsActive BIT NOT NULL DEFAULT 1,
    CompanyId INT,
    Status VARCHAR(10) DEFAULT NULL,
    UserId INT,
    Reason VARCHAR(MAX),
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_Company5TabOnboardingHistory PRIMARY KEY (Id),
    CONSTRAINT CK_Company5TabOnboardingHistory_Status CHECK (Status IN ('New', 'Pending', 'Approved', 'Rejected'))
);
GO
ALTER TABLE Company5TabOnboardingHistory
ADD CONSTRAINT FK_Company5TabOnboardingHistory_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id),
    CONSTRAINT FK_Company5TabOnboardingHistory_UserId FOREIGN KEY (UserId) REFERENCES Users (Id);
GO
CREATE INDEX IX_Company5TabOnboardingHistory_CompanyId ON Company5TabOnboardingHistory (CompanyId);
GO

-- =============================================
-- Module: Module Management
-- =============================================

-- Modules Table
IF OBJECT_ID('Modules') IS NOT NULL
    DROP TABLE Modules;
GO
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
INSERT INTO Modules (
    ModuleName, Description, ModuleUrl, IsEnabled,
    CreatedBy, UpdatedBy, CreatedAt, UpdatedAt
) VALUES
    ('Core HR', 'Core HR Main Module', 'https://example.com/corehr', 1, 1, 1, GETDATE(), GETDATE()),
    ('Exit', 'Exit control module', 'http://exit.com', 0, 1, 1, GETDATE(), GETDATE()),
    ('Time Management', 'Time management module', 'http://timemanagement.com', 0, 1, 1, GETDATE(), GETDATE());
GO

-- SubModules Table
IF OBJECT_ID('SubModules') IS NOT NULL
    DROP TABLE SubModules;
GO
CREATE TABLE SubModules (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    SubModuleName VARCHAR(50) NOT NULL,
    ModuleId INT,
    IsEnabled BIT DEFAULT 1,
    SubModuleParentLevel INT NOT NULL DEFAULT 0,
    SubModuleLevel INT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_SubModules PRIMARY KEY (Id)
);
GO
ALTER TABLE SubModules
ADD CONSTRAINT FK_SubModules_ModuleId FOREIGN KEY (ModuleId) REFERENCES Modules (Id) ON DELETE CASCADE;
GO
CREATE INDEX IX_SubModules_ModuleId ON SubModules (ModuleId);
GO

-- ModuleScreensMaster Table
IF OBJECT_ID('ModuleScreensMaster') IS NOT NULL
    DROP TABLE ModuleScreensMaster;
GO
CREATE TABLE ModuleScreensMaster (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    ModuleMasterName VARCHAR(45) NOT NULL,
    SubModuleId INT,
    IsModuleMasterType BIT DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_ModuleScreensMaster PRIMARY KEY (Id)
);
GO
ALTER TABLE ModuleScreensMaster
ADD CONSTRAINT FK_ModuleScreensMaster_SubModuleId FOREIGN KEY (SubModuleId) REFERENCES SubModules (Id);
GO
CREATE INDEX IX_ModuleScreensMaster_SubModuleId ON ModuleScreensMaster (SubModuleId);
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
ADD CONSTRAINT FK_ModuleMapping_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id) ON DELETE CASCADE,
    CONSTRAINT FK_ModuleMapping_ModuleId FOREIGN KEY (ModuleId) REFERENCES Modules (Id) ON DELETE CASCADE,
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
ADD CONSTRAINT FK_ModuleMappingHistory_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id) ON DELETE CASCADE,
    CONSTRAINT FK_ModuleMappingHistory_ModuleId FOREIGN KEY (ModuleId) REFERENCES Modules (Id) ON DELETE CASCADE,
    CONSTRAINT FK_ModuleMappingHistory_SubModuleId FOREIGN KEY (SubModuleId) REFERENCES SubModules (Id);
GO
CREATE INDEX IX_ModuleMappingHistory_CompanyId ON ModuleMappingHistory (CompanyId);
GO
CREATE INDEX IX_ModuleMappingHistory_ModuleId ON ModuleMappingHistory (ModuleId);
GO

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
    ('Master Management', 'Manage users and roles', '', 1, GETDATE(), 1, 'masterManagement'),
    ('System Policy', 'Manage users and roles', '/password-policy', 1, GETDATE(), 1, 'passwordPolicy'),
    ('User Management', 'Manage users and roles', '', 1, GETDATE(), 1, 'userManagement'),
    ('Client/Company (All)', 'Manage users and roles', '', 1, GETDATE(), 1, 'company'),
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
ADD CONSTRAINT FK_SubMenuItems_MenuItemId FOREIGN KEY (MenuItemId) REFERENCES MenuItems (Id) ON DELETE CASCADE;
GO
INSERT INTO SubMenuItems (SubMenuItemName, CreatedAt, Description, CreatedBy, SubMenuItemsUrl, MenuItemId)
VALUES 
    ('Module List', GETDATE(), 'Screen for creating modules', 1, 'module-management/module-list', 2),
    ('Create Module', GETDATE(), 'List of modules', 1, 'module-management/create-module', 2),
    ('Module Structure', GETDATE(), 'Screen for managing module structure', 1, 'module-management/module-structure', 2),
    ('Common Master List', GETDATE(), 'List of common masters', 1, '/common-master', 3),
    ('System Policy', GETDATE(), NULL, 1, NULL, 4),
    ('User Group List', GETDATE(), 'Screen for creating user groups', 1, '/user-management/user-group-list', 5),
    ('Create User Group', GETDATE(), 'List of user groups', 1, '/user-management/create-user-group', 5),
    ('Users List', GETDATE(), 'Screen for creating users', 1, '/user-management/user-list', 5),
    ('Create Users', GETDATE(), 'List of users', 1, '/user-management/create-user', 5),
    ('User Mapping', GETDATE(), 'Screen for mapping users', 1, '/user-management/user-mapping', 5),
    ('Company List', GETDATE(), 'Screen for creating clients or companies', 1, 'company/company-list', 6),
    ('Onboarding Status', GETDATE(), 'List of companies for onboarding', 1, 'company/onboarding-status', 6),
    ('Company Creation', GETDATE(), 'List of companies', 1, 'company/company-creation', 6),
    ('Client Creation', GETDATE(), 'Screen for managing password policies', 1, 'company/client-creation', 6),
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
ADD CONSTRAINT FK_SubMenuItemsAccessMapping_SubMenuItemsId FOREIGN KEY (SubMenuItemsId) REFERENCES SubMenuItems (Id) ON DELETE CASCADE,
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

-- RoleUserGroupMenuMapping Table
IF OBJECT_ID('RoleUserGroupMenuMapping') IS NOT NULL
    DROP TABLE RoleUserGroupMenuMapping;
GO
CREATE TABLE RoleUserGroupMenuMapping (
    Id INT IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL,
    CreatedBy INT,
    UpdatedBy INT,
    IsActive BIT NOT NULL DEFAULT 1,
    RoleId INT NOT NULL,
    SubMenuItemId INT NOT NULL,
    UserGroupId INT,
    IsAllCompanies BIT DEFAULT 0,
    CreateAccess BIT DEFAULT 0,
    ViewOnlyAccess BIT DEFAULT 0,
    EditAccess BIT DEFAULT 0,
    ApproveAccess BIT DEFAULT 0,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_RoleUserGroupMenuMapping PRIMARY KEY (Id)
);
GO
ALTER TABLE RoleUserGroupMenuMapping
ADD CONSTRAINT FK_RoleUserGroupMenuMapping_RoleId FOREIGN KEY (RoleId) REFERENCES Roles (Id) ON DELETE CASCADE,
    CONSTRAINT FK_RoleUserGroupMenuMapping_SubMenuItemId FOREIGN KEY (SubMenuItemId) REFERENCES SubMenuItems (Id) ON DELETE CASCADE,
    CONSTRAINT FK_RoleUserGroupMenuMapping_UserGroupId FOREIGN KEY (UserGroupId) REFERENCES UserGroups (Id);
GO
CREATE INDEX IX_RoleUserGroupMenuMapping_RoleId ON RoleUserGroupMenuMapping (RoleId);
GO
CREATE INDEX IX_RoleUserGroupMenuMapping_SubMenuItemId ON RoleUserGroupMenuMapping (SubMenuItemId);
GO
CREATE INDEX IX_RoleUserGroupMenuMapping_UserGroupId ON RoleUserGroupMenuMapping (UserGroupId);
GO

INSERT INTO RoleUserGroupMenuMapping (
    RoleId, SubMenuItemId, CreateAccess, ViewOnlyAccess, EditAccess, ApproveAccess,
    CreatedAt, CreatedBy, UserGroupId
) VALUES
(1, 7, 1, 0, 0, 0, GETDATE(), 1, 1),
(1, 6, 0, 1, 1, 0, GETDATE(), 1, 1),
(1, 9, 1, 0, 0, 0, GETDATE(), 1, 1),
(1, 8, 0, 1, 1, 0, GETDATE(), 1, 1),
(1, 10, 1, 0, 0, 0, GETDATE(), 1, 1),
(1, 13, 1, 0, 0, 0, GETDATE(), 1, 1),
(1, 14, 1, 0, 0, 0, GETDATE(), 1, 1),
(1, 12, 0, 1, 1, 0, GETDATE(), 1, 1),
(1, 11, 0, 1, 1, 0, GETDATE(), 1, 1),
(1, 2, 1, 0, 0, 0, GETDATE(), 1, 1),
(1, 1, 0, 1, 1, 0, GETDATE(), 1, 1),
(1, 3, 0, 1, 0, 0, GETDATE(), 1, 1),
(1, 5, 0, 1, 1, 0, GETDATE(), 1, 1),
(1, 4, 1, 1, 0, 0, GETDATE(), 1, 1),
(1, 20, 1, 0, 0, 0, GETDATE(), 1, 1),
(1, 21, 1, 0, 0, 0, GETDATE(), 1, 1),
(2, 3, 0, 1, 0, 0, GETDATE(), 1, 1),
(2, 11, 0, 1, 0, 0, GETDATE(), 1, 1),
(2, 12, 0, 1, 0, 0, GETDATE(), 1, 1),
(3, 3, 0, 1, 0, 0, GETDATE(), 1, 1),
(3, 11, 0, 1, 0, 0, GETDATE(), 1, 1),
(3, 12, 0, 1, 0, 0, GETDATE(), 1, 1),
(4, 3, 0, 1, 0, 0, GETDATE(), 1, 1),
(4, 11, 0, 1, 0, 0, GETDATE(), 1, 1),
(4, 12, 0, 1, 0, 0, GETDATE(), 1, 1),
(5, 1, 0, 1, 0, 0, GETDATE(), 1, 1),
(5, 3, 0, 1, 0, 0, GETDATE(), 1, 1),
(5, 11, 0, 1, 0, 0, GETDATE(), 1, 1),
(5, 12, 0, 1, 0, 0, GETDATE(), 1, 1),
(1, 15, 1, 1, 1, 1, GETDATE(), 1, 1),
(1, 16, 1, 1, 1, 1, GETDATE(), 1, 1),
(1, 17, 1, 1, 1, 1, GETDATE(), 1, 1),
(1, 18, 1, 1, 1, 1, GETDATE(), 1, 1),
(1, 19, 1, 1, 1, 1, GETDATE(), 1, 1);
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
ADD CONSTRAINT FK_Report_CategoryId FOREIGN KEY (CategoryId) REFERENCES Category (Id) ON DELETE CASCADE;
GO
CREATE INDEX IX_Report_CategoryId ON Report (CategoryId);
GO

INSERT INTO Report (
    CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, ReportName, CategoryId, IsSelected
) VALUES
(GETDATE(), GETDATE(), NULL, NULL, 'Attendance', 1, 0),
(GETDATE(), GETDATE(), NULL, NULL, 'Biometric System', 1, 0),
(GETDATE(), GETDATE(), NULL, NULL, 'Retention', 2, 0),
(GETDATE(), GETDATE(), NULL, NULL, 'Attrition Rate', 2, 0);
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
ADD CONSTRAINT FK_ReportAccess_UserGroupId FOREIGN KEY (UserGroupId) REFERENCES UserGroups (Id) ON DELETE CASCADE,
    CONSTRAINT FK_ReportAccess_UserId FOREIGN KEY (UserId) REFERENCES Users (Id) ON DELETE CASCADE,
    CONSTRAINT FK_ReportAccess_ReportId FOREIGN KEY (ReportId) REFERENCES Report (Id) ON DELETE CASCADE;
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
    UserId INT NOT NULL,
    IsSelected BIT DEFAULT 0,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_ReportingAdmin PRIMARY KEY (Id)
);
GO
ALTER TABLE ReportingAdmin
ADD CONSTRAINT FK_ReportingAdmin_UserGroupId FOREIGN KEY (UserGroupId) REFERENCES UserGroups (Id) ON DELETE CASCADE,
    CONSTRAINT FK_ReportingAdmin_UserId FOREIGN KEY (UserId) REFERENCES Users (Id) ON DELETE CASCADE;
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
ADD CONSTRAINT FK_UserNotificationsMapping_UserId FOREIGN KEY (UserId) REFERENCES Users (Id) ON DELETE CASCADE,
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
    IsActive BIT NOT NULL DEFAULT 1,
    MaxLength INT,
    MinLength INT,
    NoOfUpperCase INT,
    NoOfLowerCase INT,
    NoOfSpecialCharacters INT,
    SpecialCharactersAllowed VARCHAR(15),
    HistoricalPasswords INT,
    PasswordValidityDays INT,
    UnsuccessfulAttempts INT,
    LockInPeriodInMinutes INT,
    SessionTimeoutMinutes INT DEFAULT 30,
    FileSizeLimitMb INT DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_SystemPolicies PRIMARY KEY (Id)
);
GO

INSERT INTO SystemPolicies (
    CreatedAt, UpdatedAt, CreatedBy, UpdatedBy,
    IsActive, MaxLength, MinLength, NoOfUpperCase,
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
ADD CONSTRAINT FK_ReportingToolLimits_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id) ON DELETE CASCADE;
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
ADD CONSTRAINT FK_ReportingToolLimitsHistory_CompanyId FOREIGN KEY (CompanyId) REFERENCES Companies (Id) ON DELETE CASCADE;
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
    UserId INT NOT NULL,
    IsSelected BIT DEFAULT 0,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_WebQueryAdmin PRIMARY KEY (Id)
);
GO
ALTER TABLE WebQueryAdmin
ADD CONSTRAINT FK_WebQueryAdmin_UserGroupId FOREIGN KEY (UserGroupId) REFERENCES UserGroups (Id) ON DELETE CASCADE,
    CONSTRAINT FK_WebQueryAdmin_UserId FOREIGN KEY (UserId) REFERENCES Users (Id) ON DELETE CASCADE;
GO
CREATE INDEX IX_WebQueryAdmin_UserGroupId ON WebQueryAdmin (UserGroupId);
GO

INSERT INTO WebQueryAdmin (
    CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, UserGroupId, UserId, IsSelected
) VALUES (
    GETDATE(), GETDATE(), 1, 1, 1, 1, 1
);
GO