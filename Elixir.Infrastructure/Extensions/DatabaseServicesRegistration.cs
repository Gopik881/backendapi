using Elixir.Application.Common.Models;
using Elixir.Application.Common.SingleSession.Interface;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using Elixir.Infrastructure.Data;
using Elixir.Infrastructure.Persistance.Repositories;
using Elixir.Infrastructure.Persistance.Services;
using Elixir.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Elixir.Infrastructure.Extensions;

public static class DatabaseServicesRegistration
{
    public static IServiceCollection AddDBInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {

        // Retrieve DB Connection String from Configuration
        //var connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;

        var dbConfigSection = configuration.GetSection("ConnectionStrings");
        services.Configure<DBConfigSettings>(dbConfigSection);
        var dbConfigSettings = dbConfigSection.Get<DBConfigSettings>();

        var connectionString = dbConfigSettings.ConnectionStringTemplate
            .Replace("{Server}", dbConfigSettings.Server)
            .Replace("{Database}", dbConfigSettings.Database)
            .Replace("{UserId}", dbConfigSettings.UserId)
            .Replace("{Password}", dbConfigSettings.Password);

        // Register repositories
        services.AddScoped<IAccountHistoryRepository, AccountHistoryRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IClientAccessRepository, ClientAccessRepository>();
        services.AddScoped<IClientAdminInfoRepository, ClientAdminInfoRepository>();
        services.AddScoped<IClientCompaniesMappingRepository, ClientCompaniesMappingRepository>();
        services.AddScoped<IClientContactDetailsRepository, ClientContactDetailsRepository>();
        services.AddScoped<IClientReportingToolLimitsRepository, ClientReportingToolLimitsRepository>();
        services.AddScoped<IClientsRepository, ClientsRepository>();
        services.AddScoped<ICompaniesRepository, CompaniesRepository>();
        services.AddScoped<ICompanyAdminUsersHistoryRepository, CompanyAdminUsersHistoryRepository>();
        services.AddScoped<ICompanyAdminUsersRepository, CompanyAdminUsersRepository>();
        services.AddScoped<ICompanyHistoryRepository, CompanyHistoryRepository>();
        services.AddScoped<ICompanyOnboardingStatusRepository, CompanyOnboardingStatusRepository>();
        services.AddScoped<ICompany5TabOnboardingHistoryRepository, Company5TabOnboardingHistoryRepository>();
        services.AddScoped<ICountryMasterRepository, CountryMasterRepository>();
        services.AddScoped<ICurrencyMasterRepository, CurrencyMasterRepository>();
        services.AddScoped<IElixirUsersHistoryRepository, ElixirUsersHistoryRepository>();
        services.AddScoped<IElixirUsersRepository, ElixirUsersRepository>();
        services.AddScoped<IEscalationContactsHistoryRepository, EscalationContactsHistoryRepository>();
        services.AddScoped<IEscalationContactsRepository, EscalationContactsRepository>();
        services.AddScoped<IHorizontalsRepository, HorizontalsRepository>();
        services.AddScoped<IMasterRepository, MasterRepository>();
        services.AddScoped<IMenuItemsRepository, MenuItemsRepository>();
        services.AddScoped<IModuleMappingHistoryRepository, ModuleMappingHistoryRepository>();
        services.AddScoped<IModuleMappingRepository, ModuleMappingRepository>();
        services.AddScoped<IModuleScreensMasterRepository, ModuleScreensMasterRepository>();
        services.AddScoped<IModulesRepository, ModulesRepository>();
        services.AddScoped<INotificationsRepository, NotificationsRepository>();
        services.AddScoped<IReportAccessRepository, ReportAccessRepository>();
        services.AddScoped<IReportingAdminRepository, ReportingAdminRepository>();
        services.AddScoped<IReportingToolLimitsHistoryRepository, ReportingToolLimitsHistoryRepository>();
        services.AddScoped<IReportingToolLimitsRepository, ReportingToolLimitsRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IRolesRepository, RolesRepository>();
        services.AddScoped<IUserGroupMenuMappingRepository, UserGroupMenuMappingRepository>();
        services.AddScoped<IStateMasterRepository, StateMasterRepository>();
        services.AddScoped<ISubMenuItemsAccessMappingRepository, SubMenuItemsAccessMappingRepository>();
        services.AddScoped<ISubMenuItemsRepository, SubMenuItemsRepository>();
        services.AddScoped<ISubModulesRepository, SubModulesRepository>();
        services.AddScoped<ISystemPoliciesRepository, SystemPoliciesRepository>();
        services.AddScoped<ITelephoneCodeMasterRepository, TelephoneCodeMasterRepository>();
        services.AddScoped<IUserGroupMappingsRepository, UserGroupMappingsRepository>();
        services.AddScoped<IUserGroupsRepository, UserGroupsRepository>();
        services.AddScoped<IUserNotificationsMappingRepository, UserNotificationsMappingRepository>();
        services.AddScoped<IUserPasswordHistoryRepository, UserPasswordHistoryRepository>();
        services.AddScoped<ISuperUsersRepository, SuperUsersRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IWebQueryAdminRepository, WebQueryAdminRepository>();
        services.AddScoped<IWebQueryHorizontalCheckboxItemsRepository, WebQueryHorizontalCheckboxItemsRepository>();
        services.AddScoped<IBulkUploadErrorListRepository, BulkUploadErrorListRepository>();
        
        services.AddScoped<IEntityReferenceService, EntityReferenceService>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<Func<ITransactionRepository>>(sp => () => sp.GetRequiredService<ITransactionRepository>());

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ISessionService, SessionService>();

        // Other infrastructure-related registrations (Database, Logging, etc.)

        services.AddDbContext<ElixirHRDbContext>(options => options.UseSqlServer(connectionString));

        return services;
    }
}
