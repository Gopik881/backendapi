using Elixir.Application.Common.Constants;
using Elixir.Application.Common.DTOs;
using Elixir.Application.Common.Enums;
using Elixir.Application.Features.Module.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class ModulesRepository : IModulesRepository
{
    private readonly ElixirHRDbContext _dbContext;
    private readonly IEmailService _emailService;
    public ModulesRepository(ElixirHRDbContext dbContext, IEmailService emailService)
    {
        _dbContext = dbContext;
        _emailService = emailService;
    }
    public async Task<IEnumerable<ModuleDto>> GetAllModulesAsync()
    {
        return await _dbContext.Modules
            .Where(m => m.CreatedBy == 1 && m.IsEnabled == true)
            .Select(m => new ModuleDto
            {
                ModuleName = m.ModuleName,
                ModuleId = m.Id
            })
            .ToListAsync();
    }
    public async Task<List<ModuleStrucureResponseDto>> GetModulesWithSubModulesAsync()
    {
        // Fetch all enabled and not deleted modules
        var modules = await _dbContext.Modules
            .Where(m => m.IsEnabled == true && m.IsDeleted == false)
            .ToListAsync();

        // Get all submodules for these modules, parent and child in one go to avoid N+1 queries
        var moduleIds = modules.Select(m => m.Id).ToList();
        var subModules = await _dbContext.SubModules
            .Where(sm => sm.ModuleId != null && moduleIds.Contains(sm.ModuleId.Value) && sm.IsEnabled == true && sm.IsDeleted == false)
            .ToListAsync();

        // Group submodules by their parent id for quick lookup
        var subModulesByParent = subModules
            .GroupBy(sm => sm.SubModuleParentId)
            .ToDictionary(g => g.Key ?? 0, g => g.ToList());

        var result = new List<ModuleStrucureResponseDto>();

        foreach (var module in modules)
        {
            // Top-level submodules (parentId == 0)
            List<SubModule> topLevelSubModules;
            if (subModulesByParent.TryGetValue(0, out var list))
                topLevelSubModules = list.Where(sm => sm.ModuleId == module.Id).ToList();
            else
                topLevelSubModules = new List<SubModule>();

            var moduleDtos = new List<ModuleDto>();

            foreach (var sm in topLevelSubModules)
            {
                // Child submodules (parentId == sm.Id)
                List<SubModule> childSubModules;
                if (subModulesByParent.TryGetValue(sm.Id, out var children))
                    childSubModules = children;
                else
                    childSubModules = new List<SubModule>();

                var subModuleDtos = childSubModules
                    .Select(sub => new SubModuleDto
                    {
                        SubModuleId = sub.Id,
                        SubModuleName = sub.SubModuleName
                    })
                    .ToList();

                moduleDtos.Add(new ModuleDto
                {
                    ModuleId = sm.Id,
                    ModuleName = sm.SubModuleName,
                    SubModules = subModuleDtos
                });
            }

            result.Add(new ModuleStrucureResponseDto
            {
                moduleId = module.Id,
                ModuleName = module.ModuleName,
                Modules = moduleDtos
            });
        }

        return result;
    }

    //public async Task<ModuleStructureResponseV2> UpdateModuleStructure(ModuleCreateDto moduleCreation)
    //{
    //    await UpdateOrActivateModuleAsync(moduleCreation);

    //    //Returning Static Data for Module Structure
    //    var moduleName = await _dbContext.Modules.Where(m => m.IsEnabled == true && m.IsDeleted == false).FirstOrDefaultAsync();

    //    return new ModuleStructureResponseV2
    //    {
    //        ModuleId = moduleName.Id,
    //        ModuleName = moduleName.ModuleName,
    //        ModuleMasters = Enumerable.Range(1, 7).Select(i => new ModuleMasterDto { MasterId = 18, MasterName = $"Default Master {i}" }).ToList(),
    //        ModuleScreens = Enumerable.Range(1, 7).Select(i => new ModuleScreenDto { ModuleScreenId = i, ScreenName = $"Default Screen {i}" }).ToList(),
    //        Submodules = new List<SubModuleDtoV2>
    //        {
    //            new SubModuleDtoV2
    //            {
    //                SubSet = "",
    //                SubModuleScreens = new List<SubModuleScreenDto>
    //                {
    //                    new() {
    //                        SubModuleName = "Sub Module 1",
    //                        Masters =Enumerable.Range(1,3).Select(i=>  new MasterScreenDto { MasterName = $"Master {i}", Screens = new() { "SubScreen 1", "SubScreen 2" }}).ToList()
    //                    },
    //                   new() { SubModuleName = "Sub Module 2", Masters = new List<MasterScreenDto>
    //                        {
    //                            new() { MasterName = "Master 1", Screens = new() { "SubScreen 1", "SubScreen 2" } }
    //                        }
    //                    },
    //                    new() { SubModuleName = "Sub Module 3", Masters = new List<MasterScreenDto>
    //                        {
    //                            new() { MasterName = "Master 2", Screens = new() { "SubScreen 1", "SubScreen 2" } }
    //                        }
    //                    },
    //                    new() { SubModuleName = "Sub Module 4", Masters = new List<MasterScreenDto>
    //                        {
    //                            new() { MasterName = "Master 3", Screens = new() { "SubScreen 1", "SubScreen 2" } }
    //                        }
    //                    },
    //                    new() { SubModuleName = "Sub Module 5", Masters = new List<MasterScreenDto>
    //                        {
    //                            new() { MasterName = "Master 4", Screens = new() { "SubScreen 1", "SubScreen 1" } }
    //                        }
    //                    },
    //                    new() { SubModuleName = "Sub Module 6", Masters = new List<MasterScreenDto>
    //                        {
    //                            new() { MasterName = "Master 5", Screens = new() { "SubScreen 1", "SubScreen 2" } }
    //                        }
    //                    },
    //                    new() { SubModuleName = "Sub Module 7", Masters = new List<MasterScreenDto>
    //                        {
    //                            new() { MasterName = "Master 6", Screens = new() { "SubScreen 1", "SubScreen 2" } }
    //                        }
    //                    },
    //                    new() { SubModuleName = "Sub Module 8", Masters = new List<MasterScreenDto>
    //                        {
    //                            new() { MasterName = "Master 7", Screens = new() { "SubScreen 1", "SubScreen 2" } }
    //                        }
    //                    },
    //                    new() { SubModuleName = "Sub Module 9", Masters = new List<MasterScreenDto>
    //                        {
    //                            new() { MasterName = "Master 8", Screens = new() { "SubScreen 1", "SubScreen 2" } }
    //                        }
    //                    },
    //                    new() { SubModuleName = "Sub Module 10", Masters = new List<MasterScreenDto>
    //                        {
    //                            new() { MasterName = "Master 9", Screens = new() { "SubScreen 1", "SubScreen 2" } }
    //                        }
    //                    },
    //                    new() { SubModuleName = "Sub Module 11", Masters = new List<MasterScreenDto>
    //                        {
    //                            new() { MasterName = "Master 10", Screens = new() { "SubScreen 1", "SubScreen 2" } },
    //                            new() { MasterName = "Master11", Screens = new() { "SubScreen 1", "SubScreen 2" } }
    //                        }
    //                    },
    //                    new() { SubModuleName = "Sub Module 12", Masters = new List<MasterScreenDto>
    //                        {
    //                            new() { MasterName = "Master 12", Screens = new() { "SubScreen 1", "SubScreen 2" } },
    //                            new() { MasterName = "Master 13", Screens = new() { "SubScreen 1", "SubScreen 2" } }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    };
    //}

    public async Task<ModuleStructureResponseV2> UpdateModuleStructure(ModuleCreateDto moduleCreation)
    {
        await UpdateOrActivateModuleAsync(moduleCreation);        

        var module = await _dbContext.Modules
            .Where(m => (m.IsEnabled ?? false) && !m.IsDeleted)
            .Select(m => new ModuleStructureResponseV2
            {
                ModuleId = m.Id,
                ModuleName = m.ModuleName,
                ModuleMasters = _dbContext.ModuleMasters
                    .Where(mm => mm.ModuleId == m.Id && mm.IsEnabled && !mm.IsDeleted)
                    .Select(mm => new ModuleMasterDto { MasterId = mm.Id, MasterName = mm.MasterName })
                    .ToList(),
                ModuleScreens = _dbContext.ModuleScreens
                    .Where(ms => ms.ModuleId == m.Id && ms.IsEnabled && !ms.IsDeleted)
                    .Select(ms => new ModuleScreenDto { ModuleScreenId = ms.Id, ScreenName = ms.ScreenName })
                    .ToList(),
                Submodules = new List<SubModuleDtoV2>
                {
                new SubModuleDtoV2
                {
                    SubSet = "",
                    SubModuleScreens = _dbContext.SubModules
                        .Where(s => s.ModuleId == m.Id && (s.IsEnabled ?? false) && !s.IsDeleted)
                        .Select(s => new SubModuleScreenDto
                        {
                            SubModuleName = s.SubModuleName,
                            Masters = _dbContext.SubModuleMasters
                                .Where(smm => smm.SubModuleId == s.Id && smm.IsEnabled && !smm.IsDeleted)
                                .Select(smm => new MasterScreenDto
                                {
                                    MasterName = smm.MasterName,
                                    Screens = _dbContext.SubModuleScreens
                                        .Where(sms => sms.SubModuleMasterId == smm.Id && sms.IsEnabled && !sms.IsDeleted)
                                        .Select(sms => sms.ScreenName)
                                        .ToList()
                                })
                                .ToList()
                        })
                        .ToList()
                }
                }
            })
            .FirstOrDefaultAsync();

        if (module == null)
            throw new Exception("No active module found.");

        return module;
    }
    public async Task UpdateOrActivateModuleAsync(ModuleCreateDto moduleCreation)
    {
        // Find the module by the new URL (the one to be updated to)
        var newUrlModule = await _dbContext.Modules
            .FirstOrDefaultAsync(m => m.ModuleUrl == moduleCreation.ModuleURL);

        // If updating, get the current module by ID
        Module? currentModule = null;
        if (moduleCreation.ModuleId.HasValue)
        {
            currentModule = await _dbContext.Modules
                .FirstOrDefaultAsync(m => m.Id == moduleCreation.ModuleId.Value);
        }

        // If trying to enable an already enabled URL (and not a URL change), restrict and throw
        if (moduleCreation.ModuleId.HasValue && newUrlModule != null && newUrlModule.CreatedBy == 1 && currentModule.Id != newUrlModule.Id)
        {
            throw new Exception("Module URL already exists and is enabled.");
        }
        // If both exist and are different, this is a URL change scenario
        if (currentModule != null && newUrlModule != null && currentModule.Id != newUrlModule.Id)
        {
            // Set old URL's CreatedBy to 0 (disable)
            currentModule.CreatedBy = 0;
            currentModule.UpdatedAt = DateTime.UtcNow;

            // Set new URL's CreatedBy to 1 (enable)
            newUrlModule.CreatedBy = 1;
            newUrlModule.UpdatedAt = DateTime.UtcNow;

            // Optionally update other fields if needed
            if (!string.IsNullOrEmpty(moduleCreation.ModuleName))
                newUrlModule.ModuleName = moduleCreation.ModuleName;
            if (!string.IsNullOrEmpty(moduleCreation.Description))
                newUrlModule.Description = moduleCreation.Description;
            newUrlModule.IsEnabled = moduleCreation.Status ?? true;

            await _dbContext.SaveChangesAsync();
            return;
        }

        // If not found, throw exception
        if (newUrlModule == null)
            throw new Exception("Module not found.");

        // Check for unique ModuleName
        if (!string.IsNullOrEmpty(moduleCreation.ModuleName))
        {
            if (moduleCreation.ModuleId.HasValue)
            {
                var duplicate = await _dbContext.Modules
                    .AnyAsync(m => m.ModuleName == moduleCreation.ModuleName && m.Id != moduleCreation.ModuleId && m.CreatedBy == 1 && !m.IsDeleted);
                if (duplicate)
                    throw new Exception("Module name must be unique.");
            }
            else
            {
                var duplicate = await _dbContext.Modules
                    .AnyAsync(m => m.ModuleName == moduleCreation.ModuleName && !m.IsDeleted);
                if (duplicate)
                    throw new Exception("Module name must be unique.");
            }
        }

        // If already activated by CreatedBy == 1 and no moduleId, throw exception
        if (moduleCreation.ModuleId <= 0 || moduleCreation.ModuleId == null)
            if (newUrlModule.CreatedBy == 1)
                throw new Exception("Module is already Activated");

        // Update fields if provided, else keep existing
        newUrlModule.ModuleName = moduleCreation.ModuleName ?? newUrlModule.ModuleName;
        newUrlModule.Description = moduleCreation.Description ?? newUrlModule.Description;
        newUrlModule.IsEnabled = moduleCreation.Status ?? true;
        newUrlModule.UpdatedAt = DateTime.UtcNow;

        // Set CreatedAt and CreatedBy if activating
        if (newUrlModule.CreatedBy != 1)
        {
            newUrlModule.CreatedAt = DateTime.UtcNow;
            newUrlModule.CreatedBy = 1;
        }

        await _dbContext.SaveChangesAsync();

        // Send notification if status is false (module is being disabled)
        if (moduleCreation.Status == false)
        {
            // 1. Get all companies mapped to this module
            var companyIds = await _dbContext.ModuleMappings
                .Where(mm => mm.ModuleId == newUrlModule.Id)
                .Select(mm => mm.CompanyId)
                .Distinct()
                .ToListAsync();

            if (companyIds == null || !companyIds.Any())
            {
                companyIds = await _dbContext.ModuleMappingHistories
                    .Where(mm => mm.ModuleId == newUrlModule.Id)
                    .Select(mm => mm.CompanyId)
                    .Distinct()
                    .ToListAsync();
            }

            // 2. Get all company Account Managers from ElixirUsers table (UserGroupId == AppConstants.ACCOUNT_MANAGER_GROUP_ID)
            var accountManagerGroupId = (int)UserGroupRoles.AccountManager; // Ensure this constant exists and is set to 1

            // Replace the selected code with a join to the Users table for user details
            var accountManagerEmails = await (from u in _dbContext.ElixirUsers
                                              join usr in _dbContext.Users on u.UserId equals usr.Id
                                              where companyIds.Contains(u.CompanyId) && u.UserGroupId == accountManagerGroupId
                                              select new { usr.Email, usr.FirstName })
                                             .Distinct()
                                             .ToListAsync();

            // 3. Send email notification to all Account Manager emails
            foreach (var manager in accountManagerEmails)
            {
                _ = Task.Run(async () =>
                {
                    await _emailService.SendEmailAsync(new EmailRequestDto
                    {
                        To = manager.Email,
                        Subject = "Module Status Notification",
                        HtmlBody = $"Dear {manager.FirstName},<br/><br/>The {newUrlModule.ModuleName} Module has been disabled for your company on {DateTime.UtcNow}.<br/><br/>Thank you,<br/>Elixir Team",
                    });
                });
            }
        }
    }

   

    // Pseudocode plan:
    // 1. Query the Modules table for the module with the given moduleId, ensure it's enabled and not deleted.
    // 2. Get company and submodule counts from ModuleMappings and SubModules.
    // 3. Get all submodules for the module, ensure enabled and not deleted.
    // 4. For each submodule, get its masters (SubModuleMasters) and for each master, get its screens (SubModuleScreens).
    // 5. For the module, get ModuleMasters and ModuleScreens.
    // 6. Group submodules by business logic if needed (e.g., by name or other property), but do not hardcode names.
    // 7. Build the ModuleDetailsDto with all data from the database, no hardcoded values.

    public async Task<ModuleDetailsDto> GetModuleViewAsync(int moduleId)
    {
        // 1. Get the module entity
        var module = await _dbContext.Modules
            .Where(m => m.Id == moduleId && !m.IsDeleted)
            .FirstOrDefaultAsync();

        if (module == null)
            throw new Exception($"Module with ID {moduleId} not found.");

        // 2. Get company and submodule counts
       

        var noOfCompanies = _dbContext.ModuleMappings
                   .Where(mm => mm.ModuleId == moduleId)
                   .Select(mm => mm.CompanyId)
                   .Distinct()
                   .Count();
        var noOfSubmodules = moduleId == AppConstants.CORE_HR_ID
            ? _dbContext.SubModules.Where(mm => mm.ModuleId == moduleId).Select(mm => mm.Id).Distinct().Count() - 2
            : _dbContext.SubModules.Where(mm => mm.ModuleId == moduleId).Select(mm => mm.Id).Distinct().Count();

        // 3. Get all submodules for the module
        var subModules = await _dbContext.SubModules
            .Where(sm => sm.ModuleId == moduleId && (sm.IsEnabled ?? false) && !sm.IsDeleted)
            .Join(_dbContext.SubModules, sm => sm.Id, mm => mm.Id, (sm, mm) => sm)
            .OrderBy(sm => sm.Id)
            .ToListAsync();

        // Split submodules for Core HR (moduleId == 1) into FUNDAMENTALS and HORIZONTALS
        List<SubModuleDtoV2> submodules;
        if (moduleId == 1)
        {
            // FUNDAMENTALS: SubModuleParentId == 1
            var fundamentals = subModules.Where(sm => sm.SubModuleParentId == 1)
                .GroupBy(sm => sm.SubModuleName)
                .Select(g => g.First())
                .ToList();
            var horizontals = subModules.Where(sm => sm.SubModuleParentId == 2)
                .GroupBy(sm => sm.SubModuleName)
                .Select(g => g.First())
                .ToList();

            var fundamentalsDtos = new List<SubModuleScreenDto>();
            foreach (var subModule in fundamentals)
            {
                var masters = await _dbContext.SubModuleMasters
                    .Where(smm => smm.SubModuleId == subModule.Id && smm.IsEnabled && !smm.IsDeleted)
                    .OrderBy(smm => smm.Id)
                    .ToListAsync();

                var masterDtos = new List<MasterScreenDto>();
                foreach (var master in masters)
                {
                    var screens = await _dbContext.SubModuleScreens
                        .Where(sms => sms.SubModuleMasterId == master.Id && sms.IsEnabled && !sms.IsDeleted)
                        .OrderBy(sms => sms.Id)
                        .Select(sms => sms.ScreenName)
                        .ToListAsync();

                    masterDtos.Add(new MasterScreenDto
                    {
                        MasterName = master.MasterName,
                        Screens = screens
                    });
                }

                fundamentalsDtos.Add(new SubModuleScreenDto
                {
                    SubModuleName = subModule.SubModuleName,
                    Masters = masterDtos
                });
            }

            var horizontalsDtos = new List<SubModuleScreenDto>();
            foreach (var subModule in horizontals)
            {
                var masters = await _dbContext.SubModuleMasters
                    .Where(smm => smm.SubModuleId == subModule.Id && smm.IsEnabled && !smm.IsDeleted)
                    .OrderBy(smm => smm.Id)
                    .ToListAsync();

                var masterDtos = new List<MasterScreenDto>();
                foreach (var master in masters)
                {
                    var screens = await _dbContext.SubModuleScreens
                        .Where(sms => sms.SubModuleMasterId == master.Id && sms.IsEnabled && !sms.IsDeleted)
                        .OrderBy(sms => sms.Id)
                        .Select(sms => sms.ScreenName)
                        .ToListAsync();

                    masterDtos.Add(new MasterScreenDto
                    {
                        MasterName = master.MasterName,
                        Screens = screens
                    });
                }

                horizontalsDtos.Add(new SubModuleScreenDto
                {
                    SubModuleName = subModule.SubModuleName,
                    Masters = masterDtos
                });
            }

            submodules = new List<SubModuleDtoV2>();
            if (fundamentalsDtos.Any())
            {
                submodules.Add(new SubModuleDtoV2
                {
                    SubSet = "FUNDAMENTALS",
                    SubModuleScreens = fundamentalsDtos
                });
            }
            if (horizontalsDtos.Any())
            {
                submodules.Add(new SubModuleDtoV2
                {
                    SubSet = "HORIZONTALS",
                    SubModuleScreens = horizontalsDtos
                });
            }
        }
        else
        {
            // Default: all submodules in one group, remove duplicates by SubModuleName
            var uniqueSubModules = subModules
                .GroupBy(sm => sm.SubModuleName)
                .Select(g => g.First())
                .ToList();

            var subModuleDtos = new List<SubModuleScreenDto>();
            foreach (var subModule in uniqueSubModules)
            {
                var masters = await _dbContext.SubModuleMasters
                    .Where(smm => smm.SubModuleId == subModule.Id && smm.IsEnabled && !smm.IsDeleted)
                    .OrderBy(smm => smm.Id)
                    .ToListAsync();

                var masterDtos = new List<MasterScreenDto>();
                foreach (var master in masters)
                {
                    var screens = await _dbContext.SubModuleScreens
                        .Where(sms => sms.SubModuleMasterId == master.Id && sms.IsEnabled && !sms.IsDeleted)
                        .OrderBy(sms => sms.Id)
                        .Select(sms => sms.ScreenName)
                        .ToListAsync();

                    masterDtos.Add(new MasterScreenDto
                    {
                        MasterName = master.MasterName,
                        Screens = screens
                    });
                }

                subModuleDtos.Add(new SubModuleScreenDto
                {
                    SubModuleName = subModule.SubModuleName,
                    Masters = masterDtos
                });
            }

            submodules = new List<SubModuleDtoV2>
            {
                new SubModuleDtoV2
                {
                    SubSet = "",
                    SubModuleScreens = subModuleDtos
                }
            };
        }

        // 5. Get ModuleMasters and ModuleScreens
        var moduleMasters = await _dbContext.ModuleMasters
            .Where(mm => mm.ModuleId == moduleId && mm.IsEnabled && !mm.IsDeleted)
            .OrderBy(mm => mm.Id)
            .Select(mm => new ModuleMasterDto
            {
                MasterId = mm.Id,
                MasterName = mm.MasterName
            })
            .ToListAsync();

        var moduleScreens = await _dbContext.ModuleScreens
            .Where(ms => ms.ModuleId == moduleId && ms.IsEnabled && !ms.IsDeleted)
            .OrderBy(ms => ms.Id)
            .Select(ms => new ModuleScreenDto
            {
                ModuleScreenId = ms.Id,
                ScreenName = ms.ScreenName
            })
            .ToListAsync();

        // 7. Compose the response
        var response = new ModuleDetailsDto
        {
            ModuleId = module.Id,
            ModuleName = module.ModuleName,
            Description = module.Description,
            ModuleURL = module.ModuleUrl,
            Status = module.IsEnabled,
            NoOfCompanies = noOfCompanies,
            NoOfSubmodules = noOfSubmodules,
            ModuleMasters = moduleMasters,
            ModuleScreens = moduleScreens,
            Submodules = submodules,
            CreatedAt = module.CreatedAt,
            UpdatedAt = module.UpdatedAt,
            UsersCount = 0
        };

        return response;
    }
    
    //public async Task<List<object>> GetModuleSubmoduleListAsync(List<int> moduleIds)
    //{
    //    var modules = await _dbContext.Modules
    //        .Where(m => moduleIds.Contains(m.Id) && (m.IsEnabled ?? false) && !m.IsDeleted)
    //        .ToListAsync();

    //    var subModules = await _dbContext.SubModules
    //        .Where(sm => moduleIds.Contains(sm.ModuleId ?? 0) && (sm.IsEnabled ?? false) && !sm.IsDeleted)
    //        .ToListAsync();

    //    var result = new List<object>();

    //    foreach (var module in modules.DistinctBy(m => m.Id))
    //    {
    //        if (module.Id == 1)
    //        {
    //            var fundamentals = new[] { "Master Management", "Exit", "Onboarding", "Confirmation", "Admin Console" };
    //            var horizontals = new[] { "Email", "Notifications and Alerts", "Web Query", "Document Management", "Reporting Tool", "Questionnaries" };

    //            var children = new List<object>();

    //            var fundamentalsChildren = subModules
    //                .Where(sm => fundamentals.Contains(sm.SubModuleName))
    //                .OrderBy(sm => sm.Id)
    //                .Select(sm => new { subModuleName = sm.SubModuleName, subModuleId = sm.Id })
    //                .ToList();

    //            if (fundamentalsChildren.Any())
    //            {
    //                children.Add(new
    //                {
    //                    parentId = 1,
    //                    parentName = "FUNDAMENTALS",
    //                    moduleId = module.Id,
    //                    children = fundamentalsChildren
    //                });
    //            }

    //            var horizontalsChildren = subModules
    //                .Where(sm => horizontals.Contains(sm.SubModuleName))
    //                .OrderBy(sm => sm.Id)
    //                .Select(sm => new { subModuleName = sm.SubModuleName, subModuleId = sm.Id })
    //                .ToList();

    //            if (horizontalsChildren.Any())
    //            {
    //                children.Add(new
    //                {
    //                    parentId = 2,
    //                    parentName = "HORIZONTALS",
    //                    moduleId = module.Id,
    //                    children = horizontalsChildren
    //                });
    //            }

    //            if (children.Any())
    //            {
    //                result.Add(new
    //                {
    //                    moduleName = module.ModuleName,
    //                    moduleId = module.Id,
    //                    children
    //                });
    //            }
    //        }
    //        else
    //        {
    //            var parents = subModules
    //                .Where(sm => sm.ModuleId == module.Id && (sm.SubModuleParentId ?? 0) == 0)
    //                .DistinctBy(sm => sm.Id)
    //                .ToList();

    //            if (parents.Any())
    //            {
    //                var children = new List<object>
    //                {
    //                    new
    //                    {
    //                        parentName = module.ModuleName,
    //                        managementModuleId = module.Id,
    //                        children = parents
    //                            .OrderBy(sm => sm.Id)
    //                            .Select(sm => new { masterName = sm.SubModuleName, masterId = sm.Id })
    //                            .ToList()
    //                    }
    //                };

    //                result.Add(new
    //                {
    //                    moduleName = module.ModuleName,
    //                    moduleId = module.Id,
    //                    children
    //                });
    //            }
    //        }
    //    }

    //    return result;
    //}

    public async Task<List<object>> GetModuleSubmoduleListAsync(List<int> moduleIds)
    {
        var modules = await _dbContext.Modules
            .Where(m => moduleIds.Contains(m.Id) && (m.IsEnabled ?? false) && !m.IsDeleted)
            .ToListAsync();

        var subModules = await _dbContext.SubModules
            .Where(sm => moduleIds.Contains((int)sm.ModuleId) && (sm.IsEnabled ?? false) && !sm.IsDeleted)
            .ToListAsync();

        var result = new List<object>();

        foreach (var module in modules.DistinctBy(m => m.Id))
        {
            if (module.Id == 1)
            {
                var fundamentals = subModules
                    .Where(sm => sm.ModuleId == module.Id && sm.SubModuleParentId == 1) // IDs 1–5 for FUNDAMENTALS
                    .OrderBy(sm => sm.Id)
                    .ToList();

                var horizontals = subModules
                    .Where(sm => sm.ModuleId == module.Id && sm.SubModuleParentId == 2) // IDs 6–12 for HORIZONTALS
                    .OrderBy(sm => sm.Id)
                    .ToList();

                var children = new List<object>();

                var fundamentalsChildren = fundamentals
                    .OrderBy(sm => sm.Id)
                    .Select(sm => new { subModuleName = sm.SubModuleName, subModuleId = sm.Id })
                    .ToList();

                if (fundamentalsChildren.Any())
                {
                    children.Add(new
                    {
                        parentId = 1,
                        parentName = "FUNDAMENTALS",
                        moduleId = module.Id,
                        children = fundamentalsChildren
                    });
                }

                var horizontalsChildren = horizontals
                    .OrderBy(sm => sm.Id)
                    .Select(sm => new { subModuleName = sm.SubModuleName, subModuleId = sm.Id })
                    .ToList();

                if (horizontalsChildren.Any())
                {
                    children.Add(new
                    {
                        parentId = 2,
                        parentName = "HORIZONTALS",
                        moduleId = module.Id,
                        children = horizontalsChildren
                    });
                }

                if (children.Any())
                {
                    result.Add(new
                    {
                        moduleName = module.ModuleName,
                        moduleId = module.Id,
                        children
                    });
                }
            }
            else
            {
                var parents = subModules
                    .Where(sm => sm.ModuleId == module.Id && (sm.SubModuleParentId == 0 || sm.SubModuleParentId == null) && (sm.IsEnabled ?? false) && !sm.IsDeleted)
                    .DistinctBy(sm => sm.Id)
                    .ToList();

                if (parents.Any())
                {
                    var children = new List<object>
                    {
                        new
                        {
                            parentName = module.ModuleName,
                            managementModuleId = module.Id,
                            children = parents
                                .OrderBy(sm => sm.Id)
                                .Select(sm => new { masterName = sm.SubModuleName, masterId = sm.Id })
                                .ToList()
                        }
                    };

                    result.Add(new
                    {
                        moduleName = module.ModuleName,
                        moduleId = module.Id,
                        children
                    });
                }
            }
        }

        return result;
    }

    //public List<object> GetModuleMastersAndScreens(List<int> moduleIds, bool IsMaster)
    //{
    //    var modules = _dbContext.Modules
    //        .Where(m => moduleIds.Contains(m.Id))
    //        .Select(m => new { m.Id, m.ModuleName })
    //        .ToList();

    //    var responses = new List<object>();

    //    foreach (var module in modules)
    //    {
    //        if (module.Id == 1) // Core HR specific structure
    //        {
    //            responses.Add(new ModuleResponseDto
    //            {
    //                ParentName = module.ModuleName,
    //                Children = new List<ModuleChildrenDto>
    //                {
    //                    new ModuleChildrenDto
    //                    {
    //                        SectionName = "FUNDAMENTALS",
    //                        Details = new Dictionary<string, List<string>>
    //                        {
    //                            { "Admin Console", Enumerable.Range(1, 3).Select(i => IsMaster ? $"Master {i}" : $"Screen {i}").ToList() },
    //                            { "Masters Management", Enumerable.Range(1, 3).Select(i => IsMaster ? $"Master {i}" : $"Screen {i}").ToList() },
    //                            { "Onboarding", Enumerable.Range(1, 3).Select(i => IsMaster ? $"Master {i}" : $"Screen {i}").ToList() },
    //                            { "Confirmation", Enumerable.Range(1, 3).Select(i => IsMaster ? $"Master {i}" : $"Screen {i}").ToList() }
    //                        }
    //                    },
    //                    new ModuleChildrenDto
    //                    {
    //                        SectionName = "HORIZONTALS",
    //                        Details = new Dictionary<string, List<string>>
    //                        {
    //                            { "Email", Enumerable.Range(1, 3).Select(i => IsMaster ? $"Master {i}" : $"Screen {i}").ToList() },
    //                            { "Notifications and Alerts", Enumerable.Range(1, 3).Select(i => IsMaster ? $"Master {i}" : $"Screen {i}").ToList() },
    //                            { "Web Query", Enumerable.Range(1, 3).Select(i => IsMaster ? $"Master {i}" : $"Screen {i}").ToList() },
    //                            { "Document Management", Enumerable.Range(1, 3).Select(i => IsMaster ? $"Master {i}" : $"Screen {i}").ToList() },
    //                            { "Reporting Tool", Enumerable.Range(1, 3).Select(i => IsMaster ? $"Master {i}" : $"Screen {i}").ToList() },
    //                            //{ "Letter Generation", Enumerable.Range(1, 3).Select(i => IsMaster ? $"Master {i}" : $"Screen {i}").ToList() },
    //                            { "Questionnaires", Enumerable.Range(1, 3).Select(i => IsMaster ? $"Master {i}" : $"Screen {i}").ToList() }
    //                        }
    //                    }
    //                }
    //            });
    //        }
    //        else // Default Payroll structure
    //        {
    //            responses.Add(new DefaultModuleResponseDto
    //            {
    //                ParentName = module.ModuleName,
    //                Children = new Dictionary<string, List<string>>
    //                {
    //                    {
    //                        module.ModuleName,
    //                        Enumerable.Range(1, 7).Select(i => IsMaster ? $"DefaultMaster {i}" : $"DefaultScreen {i}").ToList()
    //                    },
    //                    {
    //                        "SubModule 1",
    //                        Enumerable.Range(1, 3).Select(i => IsMaster ? $"Master {i}" : $"Screen {i}").ToList()
    //                    },
    //                    {
    //                        "SubModule 2",
    //                        Enumerable.Range(1, 3).Select(i => IsMaster ? $"Master {i}" : $"Screen {i}").ToList()
    //                    },
    //                    {
    //                        "SubModule 3",
    //                        Enumerable.Range(1, 3).Select(i => IsMaster ? $"Master {i}" : $"Screen {i}").ToList()
    //                    }
    //                }
    //            });
    //        }
    //    }

    //    return responses;
    //}

    public List<object> GetModuleMastersAndScreens(List<int> moduleIds, bool IsMaster)
    {
        var modules = _dbContext.Modules
            .Where(m => moduleIds.Contains(m.Id) && (m.IsEnabled ?? false) && !m.IsDeleted)
            .Select(m => new { m.Id, m.ModuleName })
            .ToList();

        var subModules = _dbContext.SubModules
            .Where(sm => moduleIds.Contains((int)sm.ModuleId) && (sm.IsEnabled ?? false) && !sm.IsDeleted)
            .ToList();

        var responses = new List<object>();

        foreach (var module in modules)
        {
            if (module.Id == 1) // Core HR specific structure
            {
                var fundamentalSubModules = subModules
                    .Where(sm => sm.ModuleId == module.Id && sm.SubModuleParentId == 1) // IDs 1–5 for FUNDAMENTALS
                    .OrderBy(sm => sm.Id)
                    .ToList();

                var horizontalSubModules = subModules
                    .Where(sm => sm.ModuleId == module.Id && sm.SubModuleParentId == 2) // IDs 6–12 for HORIZONTALS
                    .OrderBy(sm => sm.Id)
                    .ToList();
                
                var coreHrchildren = new Dictionary<string, List<string>>();
                var children = new List<ModuleChildrenDto>();

                // Add module-level masters or screens
                coreHrchildren[module.ModuleName] = IsMaster
                    ? _dbContext.ModuleMasters
                        .Where(mm => mm.ModuleId == module.Id && mm.IsEnabled && !mm.IsDeleted)
                        .OrderBy(mm => mm.Id)
                        .Select(mm => mm.MasterName)
                        //.Take(7)
                        .ToList()
                    : _dbContext.ModuleScreens
                        .Where(ms => ms.ModuleId == module.Id && ms.IsEnabled && !ms.IsDeleted)
                        .OrderBy(ms => ms.Id)
                        .Select(ms => ms.ScreenName)
                        //.Take(7)
                        .ToList();

                if (fundamentalSubModules.Any())
                {
                    var fundamentalDetails = new Dictionary<string, List<string>>();
                    foreach (var subModule in fundamentalSubModules)
                    {
                        fundamentalDetails[subModule.SubModuleName] = IsMaster
                            ? _dbContext.SubModuleMasters
                                .Where(smm => smm.SubModuleId == subModule.Id && smm.IsEnabled && !smm.IsDeleted)
                                .OrderBy(smm => smm.Id)
                                .Select(smm => smm.MasterName)
                                //.Take(3)
                                .ToList()
                            : _dbContext.SubModuleScreens
                                .Where(sms => sms.SubModuleId == subModule.Id && sms.IsEnabled && !sms.IsDeleted)
                                .OrderBy(sms => sms.Id)
                                .Select(sms => sms.ScreenName)
                                //.Take(3)
                                .ToList();
                    }

                    children.Add(new ModuleChildrenDto
                    {
                        SectionName = "FUNDAMENTALS",
                        Details = fundamentalDetails
                    });
                }

                if (horizontalSubModules.Any())
                {
                    var horizontalDetails = new Dictionary<string, List<string>>();
                    foreach (var subModule in horizontalSubModules)
                    {
                        horizontalDetails[subModule.SubModuleName] = IsMaster
                            ? _dbContext.SubModuleMasters
                                .Where(smm => smm.SubModuleId == subModule.Id && smm.IsEnabled && !smm.IsDeleted)
                                .OrderBy(smm => smm.Id)
                                .Select(smm => smm.MasterName)
                                //.Take(3)
                                .ToList()
                            : _dbContext.SubModuleScreens
                                .Where(sms => sms.SubModuleId == subModule.Id && sms.IsEnabled && !sms.IsDeleted)
                                .OrderBy(sms => sms.Id)
                                .Select(sms => sms.ScreenName)
                                //.Take(3)
                                .ToList();
                    }

                    children.Add(new ModuleChildrenDto
                    {
                        SectionName = "HORIZONTALS",
                        Details = horizontalDetails
                    });
                }

                if (children.Any())
                {
                    responses.Add(new ModuleResponseDto
                    {
                        ParentName = module.ModuleName,
                        Children = children,
                        coreHrChildren = coreHrchildren
                    });
                }
            }
            else // Default structure for other modules
            {
                var children = new Dictionary<string, List<string>>();

                // Add module-level masters or screens
                children[module.ModuleName] = IsMaster
                    ? _dbContext.ModuleMasters
                        .Where(mm => mm.ModuleId == module.Id && mm.IsEnabled && !mm.IsDeleted)
                        .OrderBy(mm => mm.Id)
                        .Select(mm => mm.MasterName)
                        //.Take(7)
                        .ToList()
                    : _dbContext.ModuleScreens
                        .Where(ms => ms.ModuleId == module.Id && ms.IsEnabled && !ms.IsDeleted)
                        .OrderBy(ms => ms.Id)
                        .Select(ms => ms.ScreenName)
                        //.Take(7)
                        .ToList();

                // Add submodule masters or screens
                var moduleSubModules = subModules
                    .Where(sm => sm.ModuleId == module.Id)
                    .OrderBy(sm => sm.Id)
                    //.Take(3) // Limit to 3 submodules to match original code
                    .ToList();

                foreach (var subModule in moduleSubModules)
                {
                    children[subModule.SubModuleName] = IsMaster
                        ? _dbContext.SubModuleMasters
                            .Where(smm => smm.SubModuleId == subModule.Id && smm.IsEnabled && !smm.IsDeleted)
                            .OrderBy(smm => smm.Id)
                            .Select(smm => smm.MasterName)
                            //.Take(3)
                            .ToList()
                        : _dbContext.SubModuleScreens
                            .Where(sms => sms.SubModuleId == subModule.Id && sms.IsEnabled && !sms.IsDeleted)
                            .OrderBy(sms => sms.Id)
                            .Select(sms => sms.ScreenName)
                            //.Take(3)
                            .ToList();
                }

                if (children.Any())
                {
                    responses.Add(new DefaultModuleResponseDto
                    {
                        ParentName = module.ModuleName,
                        Children = children
                    });
                }
            }
        }

        return responses;
    }

}



