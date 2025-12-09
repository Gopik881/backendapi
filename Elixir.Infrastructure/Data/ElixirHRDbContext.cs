using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Elixir.Infrastructure.Data;
public  class ElixirHRDbContext : DbContext
{
    public ElixirHRDbContext()
    {
    }

    public ElixirHRDbContext(DbContextOptions<ElixirHRDbContext> options)
        : base(options)
    {
    }
    #region DbSets
    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AccountHistory> AccountHistories { get; set; }

    public virtual DbSet<ActiveSession> ActiveSession { get; set; }
    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<ClientAccess> ClientAccesses { get; set; }

    public virtual DbSet<ClientAdminInfo> ClientAdminInfos { get; set; }

    public virtual DbSet<ClientCompaniesMapping> ClientCompaniesMappings { get; set; }

    public virtual DbSet<ClientContactDetail> ClientContactDetails { get; set; }

    public virtual DbSet<ClientReportingToolLimit> ClientReportingToolLimits { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Company5TabOnboardingHistory> Company5TabOnboardingHistories { get; set; }

    public virtual DbSet<CompanyAdminUser> CompanyAdminUsers { get; set; }

    public virtual DbSet<CompanyAdminUsersHistory> CompanyAdminUsersHistories { get; set; }

    public virtual DbSet<CompanyHistory> CompanyHistories { get; set; }

    public virtual DbSet<CompanyOnboardingStatus> CompanyOnboardingStatuses { get; set; }

    public virtual DbSet<CountryMaster> CountryMasters { get; set; }

    public virtual DbSet<CurrencyMaster> CurrencyMasters { get; set; }

    public virtual DbSet<ElixirUser> ElixirUsers { get; set; }

    public virtual DbSet<ElixirUsersHistory> ElixirUsersHistories { get; set; }

    public virtual DbSet<EscalationContact> EscalationContacts { get; set; }

    public virtual DbSet<EscalationContactsHistory> EscalationContactsHistories { get; set; }

    public virtual DbSet<Horizontal> Horizontals { get; set; }

    public virtual DbSet<Master> Masters { get; set; }

    public virtual DbSet<MenuItem> MenuItems { get; set; }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<ModuleMapping> ModuleMappings { get; set; }

    public virtual DbSet<ModuleMappingHistory> ModuleMappingHistories { get; set; }
    public virtual DbSet<ModuleMaster> ModuleMasters { get; set; }
    public virtual DbSet<ModuleScreen> ModuleScreens { get; set; }

    public virtual DbSet<ModuleScreensMaster> ModuleScreensMasters { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<ReportAccess> ReportAccesses { get; set; }

    public virtual DbSet<ReportingAdmin> ReportingAdmins { get; set; }

    public virtual DbSet<ReportingToolLimit> ReportingToolLimits { get; set; }

    public virtual DbSet<ReportingToolLimitsHistory> ReportingToolLimitsHistories { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<UserGroupMenuMapping> UserGroupMenuMappings { get; set; }

    public virtual DbSet<StateMaster> StateMasters { get; set; }

    public virtual DbSet<SubMenuItem> SubMenuItems { get; set; }

    public virtual DbSet<SubMenuItemsAccessMapping> SubMenuItemsAccessMappings { get; set; }

    public virtual DbSet<SubModule> SubModules { get; set; }
    public virtual DbSet<SubModuleMaster> SubModuleMasters { get; set; }
    public virtual DbSet<SubModuleScreen> SubModuleScreens { get; set; }

    public virtual DbSet<SystemPolicy> SystemPolicies { get; set; }

    public virtual DbSet<TelephoneCodeMaster> TelephoneCodeMasters { get; set; }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<SuperUser> SuperUsers { get; set; }

    public virtual DbSet<UserGroup> UserGroups { get; set; }

    public virtual DbSet<UserGroupMapping> UserGroupMappings { get; set; }

    public virtual DbSet<UserNotificationsMapping> UserNotificationsMappings { get; set; }

    public virtual DbSet<UserPasswordHistory> UserPasswordHistories { get; set; }

    public virtual DbSet<WebQueryAdmin> WebQueryAdmins { get; set; }

    public virtual DbSet<WebQueryHorizontalCheckboxItem> WebQueryHorizontalCheckboxItems { get; set; }


    public virtual DbSet<BulkUploadErrorList> BulkUploadErrorLists { get; set; }
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AccountConfiguration());
        modelBuilder.ApplyConfiguration(new AccountHistoryConfiguration());
        modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new ClientAccessConfiguration());
        modelBuilder.ApplyConfiguration(new ClientAdminInfoConfiguration());
        modelBuilder.ApplyConfiguration(new ClientCompaniesMappingConfiguration());
        modelBuilder.ApplyConfiguration(new ClientConfiguration());
        modelBuilder.ApplyConfiguration(new ClientContactDetailConfiguration());
        modelBuilder.ApplyConfiguration(new ClientReportingToolLimitConfiguration());
        modelBuilder.ApplyConfiguration(new Company5TabOnboardingHistoryConfiguration());
        modelBuilder.ApplyConfiguration(new CompanyAdminUserConfiguration());
        modelBuilder.ApplyConfiguration(new CompanyAdminUsersHistoryConfiguration());
        modelBuilder.ApplyConfiguration(new CompanyConfiguration());
        modelBuilder.ApplyConfiguration(new CompanyHistoryConfiguration());
        modelBuilder.ApplyConfiguration(new CompanyOnboardingStatusConfiguration());
        modelBuilder.ApplyConfiguration(new CountryMasterConfiguration());
        modelBuilder.ApplyConfiguration(new CurrencyMasterConfiguration());
        modelBuilder.ApplyConfiguration(new ElixirUserConfiguration());
        modelBuilder.ApplyConfiguration(new ElixirUsersHistoryConfiguration());
        modelBuilder.ApplyConfiguration(new EscalationContactConfiguration());
        modelBuilder.ApplyConfiguration(new EscalationContactsHistoryConfiguration());
        modelBuilder.ApplyConfiguration(new HorizontalConfiguration());
        modelBuilder.ApplyConfiguration(new MasterConfiguration());
        modelBuilder.ApplyConfiguration(new MenuItemConfiguration());
        modelBuilder.ApplyConfiguration(new ModuleConfiguration());
        modelBuilder.ApplyConfiguration(new ModuleMappingConfiguration());
        modelBuilder.ApplyConfiguration(new ModuleMappingHistoryConfiguration());
        modelBuilder.ApplyConfiguration(new ModuleScreensMasterConfiguration());
        modelBuilder.ApplyConfiguration(new NotificationConfiguration());
        modelBuilder.ApplyConfiguration(new ReportAccessConfiguration());
        modelBuilder.ApplyConfiguration(new ReportConfiguration());
        modelBuilder.ApplyConfiguration(new ReportingAdminConfiguration());
        modelBuilder.ApplyConfiguration(new ReportingToolLimitConfiguration());
        modelBuilder.ApplyConfiguration(new ReportingToolLimitsHistoryConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserGroupMenuMappingConfiguration());
        modelBuilder.ApplyConfiguration(new StateMasterConfiguration());
        modelBuilder.ApplyConfiguration(new SubMenuItemConfiguration());
        modelBuilder.ApplyConfiguration(new SubMenuItemsAccessMappingConfiguration());
        modelBuilder.ApplyConfiguration(new SubModuleConfiguration());
        modelBuilder.ApplyConfiguration(new SystemPolicyConfiguration());
        modelBuilder.ApplyConfiguration(new TelephoneCodeMasterConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new SuperUserConfiguration());
        modelBuilder.ApplyConfiguration(new UserGroupConfiguration());
        modelBuilder.ApplyConfiguration(new UserGroupMappingConfiguration());
        modelBuilder.ApplyConfiguration(new UserNotificationsMappingConfiguration());
        modelBuilder.ApplyConfiguration(new UserPasswordHistoryConfiguration());
        modelBuilder.ApplyConfiguration(new WebQueryAdminConfiguration());
        modelBuilder.ApplyConfiguration(new WebQueryHorizontalCheckboxItemConfiguration());
        modelBuilder.ApplyConfiguration(new BulkUploadErrorListConfiguration());

        modelBuilder.ApplyConfiguration(new ActiveSessionConfiguration());
    }
}
