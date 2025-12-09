using Elixir.Application.Features.UserRightsMetatadata.DTOs;

namespace Elixir.Application.Features.UserRightsMetatadata.Queries.GetUserRightsMetadata;

public class UserRightsMetadataGenerator
{
    public UserRightsMetedataResponseDto GetUserRightsMetadataForUserType1()
    {
        return new UserRightsMetedataResponseDto
        {
            Message = "User rights retrieved successfully.",
            Data = new UserRightsDataDto
            {
                UserRights = new List<UserRightDto>
            {
                new UserRightDto
                {
                    RoleID = 1,
                    ModuleName = "User Management",
                    Screens = new List<ScreenDto>
                    {
                        new ScreenDto
                        {
                            ScreenID = 6, // Create User Group
                            ScreenName = "Create User Group",
                            Permissions = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = false, Edit = false, Approve = false, Create = false }
                            },
                            Checkbox = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = false, Edit = false, Approve = false, Create = true }
                            },
                            Dependencies = new List<DependencyDto>
                            {
                                new DependencyDto { ScreenID = "7", PermissionType = "Edit" }
                            },
                            Parents = new List<ParentDto>(),
                            ViewOptions = null
                        },
                        new ScreenDto
                        {
                            ScreenID = 7, // User Group List
                            ScreenName = "User Group List",
                            Permissions = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = false, Edit = false, Approve = false, Create = false }
                            },
                            Checkbox = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = true, Edit = true, Approve = false, Create = false }
                            },
                            Dependencies = new List<DependencyDto>(),
                            Parents = new List<ParentDto>
                            {
                                new ParentDto { ScreenID = "6", PermissionType = "Create" }
                            },
                            ViewOptions = null
                        },
                        new ScreenDto
                        {
                            ScreenID = 8, // Create Users
                            ScreenName = "Create Users",
                            Permissions = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = false, Edit = false, Approve = false, Create = false }
                            },
                            Checkbox = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = false, Edit = false, Approve = false, Create = true }
                            },
                            Dependencies = new List<DependencyDto>
                            {
                                new DependencyDto { ScreenID = "9", PermissionType = "Edit" }
                            },
                            Parents = new List<ParentDto>(),
                            ViewOptions = null
                        },
                        new ScreenDto
                        {
                            ScreenID = 9, // Users List
                            ScreenName = "Users List",
                            Permissions = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = false, Edit = false, Approve = false, Create = false }
                            },
                            Checkbox = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = true, Edit = true, Approve = false, Create = false }
                            },
                            Dependencies = new List<DependencyDto>(),
                            Parents = new List<ParentDto>
                            {
                                new ParentDto { ScreenID = "8", PermissionType = "Create" }
                            },
                            ViewOptions = null
                        },
                        new ScreenDto
                        {
                            ScreenID = 10, // User Mapping
                            ScreenName = "User Mapping",
                            Permissions = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = false, Edit = false, Approve = false, Create = false }
                            },
                            Checkbox = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = false, Edit = false, Approve = false, Create = true }
                            },
                            Dependencies = new List<DependencyDto>(),
                            Parents = new List<ParentDto>(),
                            ViewOptions = null
                        }
                    }
                },
                new UserRightDto
                {
                    RoleID = 1,
                    ModuleName = "Client/Company",
                    Screens = new List<ScreenDto>
                    {
                        new ScreenDto
                        {
                            ScreenID = 11, // Client/Company Creation
                            ScreenName = "Client/Company Creation",
                            Permissions = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = false, Edit = false, Approve = false, Create = false }
                            },
                            Checkbox = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = false, Edit = false, Approve = false, Create = true }
                            },
                            Dependencies = new List<DependencyDto>
                            {
                                new DependencyDto { ScreenID = "13", PermissionType = "Edit" },
                                new DependencyDto { ScreenID = "12", PermissionType = "Edit" },
                                new DependencyDto { ScreenID = "15", PermissionType = "Edit" }
                            },
                            Parents = new List<ParentDto>(),
                            ViewOptions = null
                        },
                        new ScreenDto
                        {
                            ScreenID = 12, // Company Onboarding List
                            ScreenName = "Company Onboarding List",
                            Permissions = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = false, Edit = false, Approve = false, Create = false, IsAllCompanies = false }
                            },
                            Checkbox = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = true, Edit = true, Approve = false, Create = false, IsAllCompanies = false }
                            },
                            Dependencies = new List<DependencyDto>(),
                            Parents = new List<ParentDto>
                            {
                                new ParentDto { ScreenID = "11", PermissionType = "Create" }
                            },
                            ViewOptions = new ViewOptionsDto { AllCompanies = false, Custom = false }
                        },
                        new ScreenDto
                        {
                            ScreenID = 13, // Company List
                            ScreenName = "Company List",
                            Permissions = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = false, Edit = false, Approve = false, Create = false , IsAllCompanies = false}
                            },
                            Checkbox = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = true, Edit = true, Approve = false, Create = false, IsAllCompanies = false }
                            },
                            Dependencies = new List<DependencyDto>(),
                            Parents = new List<ParentDto>
                            {
                                new ParentDto { ScreenID = "11", PermissionType = "Create" }
                            },
                            ViewOptions = new ViewOptionsDto { AllCompanies = false, Custom = false }
                        },
                          new ScreenDto
                        {
                            ScreenID = 15, // Client List
                            ScreenName = "Client List",
                            Permissions = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = false, Edit = false, Approve = false, Create = false , IsAllCompanies = false}
                            },
                            Checkbox = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = true, Edit = true, Approve = false, Create = false, IsAllCompanies = false }
                            },
                            Dependencies = new List<DependencyDto>(),
                            Parents = new List<ParentDto>
                            {
                                new ParentDto { ScreenID = "11", PermissionType = "Create" }
                            },
                            ViewOptions = new ViewOptionsDto { AllCompanies = false, Custom = false }
                        }
                    }
                },
                new UserRightDto
                {
                    RoleID = 1,
                    ModuleName = "Module Management",
                    Screens = new List<ScreenDto>
                    {
                        new ScreenDto
                        {
                            ScreenID = 1, // Create Module
                            ScreenName = "Create Module",
                            Permissions = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = false, Edit = false, Approve = false, Create = false }
                            },
                            Checkbox = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = false, Edit = false, Approve = false, Create = true }
                            },
                            Dependencies = new List<DependencyDto>
                            {
                                new DependencyDto { ScreenID = "2", PermissionType = "Edit" }
                            },
                            Parents = new List<ParentDto>(),
                            ViewOptions = null
                        },
                        new ScreenDto
                        {
                            ScreenID = 2, // Module List
                            ScreenName = "Module List",
                            Permissions = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = false, Edit = false, Approve = false, Create = false }
                            },
                            Checkbox = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = true, Edit = true, Approve = false, Create = false }
                            },
                            Dependencies = new List<DependencyDto>(),
                            Parents = new List<ParentDto>
                            {
                                new ParentDto { ScreenID = "1", PermissionType = "Create" }
                            },
                            ViewOptions = null
                        },
                        new ScreenDto
                        {
                            ScreenID = 3, // Module Structure
                            ScreenName = "Module Structure",
                            Permissions = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = false, Edit = false, Approve = false, Create = false }
                            },
                            Checkbox = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = true, Edit = false, Approve = false, Create = false }
                            },
                            Dependencies = new List<DependencyDto>(),
                            Parents = new List<ParentDto>(),
                            ViewOptions = null
                        }
                    }
                },
                new UserRightDto
                {
                    RoleID = 1,
                    ModuleName = "Master Management",
                    Screens = new List<ScreenDto>
                    {
                        new ScreenDto
                        {
                            ScreenID = 4, // Common Master List
                            ScreenName = "Common Master List",
                            Permissions = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = false, Edit = false, Approve = false, Create = false }
                            },
                            Checkbox = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = true, Edit = false, Approve = false, Create = true }
                            },
                            Dependencies = new List<DependencyDto>(),
                            Parents = new List<ParentDto>(),
                            ViewOptions = null
                        }
                    }
                },
                new UserRightDto
                {
                    RoleID = 1,
                    ModuleName = "System Policies",
                    Screens = new List<ScreenDto>
                    {
                        new ScreenDto
                        {
                            ScreenID = 5, // System Policy
                            ScreenName = "System Policies",
                            Permissions = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = false, Edit = false, Approve = false, Create = false }
                            },
                            Checkbox = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = true, Edit = true, Approve = false, Create = false }
                            },
                            Dependencies = new List<DependencyDto>(),
                            Parents = new List<ParentDto>(),
                            ViewOptions = null
                        }
                    }
                },
                new UserRightDto
                {
                    RoleID = 1,
                    ModuleName = "Configurators",
                    Screens = new List<ScreenDto>
                    {
                        new ScreenDto
                        {
                            ScreenID = 21, // API Configurator
                            ScreenName = "API Configurator",
                            Permissions = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = false, Edit = false, Approve = false, Create = false }
                            },
                            Checkbox = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = false, Edit = false, Approve = false, Create = true }
                            },
                            Dependencies = new List<DependencyDto>(),
                            Parents = new List<ParentDto>(),
                            ViewOptions = null
                        },
                        new ScreenDto
                        {
                            ScreenID = 22, // SSO Configurator
                            ScreenName = "SSO Configurator",
                            Permissions = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = false, Edit = false, Approve = false, Create = false }
                            },
                            Checkbox = new List<PermissionDto>
                            {
                                new PermissionDto { ViewOnly = false, Edit = false, Approve = false, Create = true }
                            },
                            Dependencies = new List<DependencyDto>(),
                            Parents = new List<ParentDto>(),
                            ViewOptions = null
                        }
                    }
                }


            },
                Horizontals = new List<HorizontalDto>
            {
                new HorizontalDto
                {
                    HorizontalID = 46,
                    HorizontalName = "Letter Generation Admin",
                    Description = "Letter Template",
                    IsSelected = false,
                    CheckboxItems = new List<CheckboxItemDto>()
                },
                new HorizontalDto
                {
                    HorizontalID = 47,
                    HorizontalName = "Document Management Admin",
                    Description = "Document",
                    IsSelected = false,
                    CheckboxItems = new List<CheckboxItemDto>()
                },
                new HorizontalDto
                {
                    HorizontalID = 48,
                    HorizontalName = "Web Query Admin",
                    Description = "Web Query",
                    IsSelected = false,
                    CheckboxItems = new List<CheckboxItemDto>
                    {
                        new CheckboxItemDto { CheckboxItemID = 31, CheckboxName = "Elixir", IsSelected = false },
                        new CheckboxItemDto { CheckboxItemID = 32, CheckboxName = "Company", IsSelected = false },
                        new CheckboxItemDto { CheckboxItemID = 33, CheckboxName = "Client", IsSelected = false }
                    }
                },
                new HorizontalDto
                {
                    HorizontalID = 49,
                    HorizontalName = "Email Admin ",
                    Description = "Email",
                    IsSelected = false,
                    CheckboxItems = new List<CheckboxItemDto>()
                },
                new HorizontalDto
                {
                    HorizontalID = 50,
                    HorizontalName = "Reporting Admin",
                    Description = "Report",
                    IsSelected = false,
                    CheckboxItems = new List<CheckboxItemDto>()
                }
            },
                ReportingAdmins = new List<object>
                {
                    new ReportingAdminDto
                    {
                        ReportingAdminId = 1,
                        name = "Elixir Reporting Admin",
                        IsSelected = false
                    },
                    new ReportingAdminDto
                    {
                        ReportingAdminId = 2,
                        name = "Elixir Company/Client Reporting Admin",
                        IsSelected = false
                    }
                },
                ReportAccesses = new List<ReportAccessDto>
            {
                new ReportAccessDto
                {
                    Categories = new List<ReportCategoryDto>
                    {
                        new ReportCategoryDto
                        {
                            Id = 2,
                            Name = "PlaceHolder 1",//"Time Management",
                            CategoryId = 2,
                            IsSelected = false,
                            //CanDownloadReport = null,
                            SubReports = new List<object>()
                        },
                        new ReportCategoryDto
                        {
                            Id = 3,
                            Name = "PlaceHolder 2",//"Exit",
                            CategoryId = 3,
                            IsSelected = false,
                            //CanDownloadReport = null,
                            SubReports = new List<object>()
                        }
                    },
                    Reports = new List<ReportDto>
                    {
                        new ReportDto
                        {
                            Id = 2,
                            Name = "PlaceHolder 1",//"Time Management",
                            CategoryId = 2,
                            IsSelected = false,
                            //CanDownloadReport = null,
                            SubReports = new List<SubReportDto>
                            {
                                new SubReportDto
                                {
                                    Id = 1,
                                    Name = "PlaceHolder 3",//"Attendance",
                                    CategoryId = 2,
                                    IsSelected = false,
                                    //CanDownloadReport = null,
                                    SubReports = new List<object>()
                                },
                                new SubReportDto
                                {
                                    Id = 2,
                                    Name = "PlaceHolder 4",//"Biometric System",
                                    CategoryId = 2,
                                    IsSelected = false,
                                    //CanDownloadReport = null,
                                    SubReports = new List<object>()
                                }
                            }
                        },
                        new ReportDto
                        {
                            Id = 3,
                            Name = "PlaceHolder 2",//"Exit",
                            CategoryId = 3,
                            IsSelected = false,
                            //CanDownloadReport = null,
                            SubReports = new List<SubReportDto>
                            {
                                new SubReportDto
                                {
                                    Id = 3,
                                    Name = "PlaceHolder 5",//"Retention",
                                    CategoryId = 3,
                                    IsSelected = false,
                                    //CanDownloadReport = null,
                                    SubReports = new List<object>()
                                },
                                new SubReportDto
                                {
                                    Id = 4,
                                    Name = "PlaceHolder 6",//"Attrition Rate",
                                    CategoryId = 3,
                                    IsSelected = false,
                                    //CanDownloadReport = null,
                                    SubReports = new List<object>()
                                }
                            }
                        }
                    },
                    CanDownloadReports = false
                }
            }
            }
        };
    }

    public UserRightsMetedataResponseDto GetUserRightsMetadataForUserType2()
    {
        return new UserRightsMetedataResponseDto
        {
            Message = "User rights retrieved successfully.",
            Data = new UserRightsDataDto
            {
                UserRights = new List<UserRightDto>
                {
                    new UserRightDto
                    {
                        RoleID = 1,
                        ModuleName = "Client/Company (All)",
                        Screens = new List<ScreenDto>
                        {
                            new ScreenDto
                            {
                                ScreenID = 12,
                                ScreenName = "Company Onboarding List",
                                Permissions = new List<PermissionDto>
                                {
                                    new PermissionDto { ViewOnly = false, Edit = false, Approve = false, Create = false }
                                },
                                Checkbox = new List<PermissionDto>
                                {
                                    new PermissionDto { ViewOnly = true, Edit = true, Approve = false, Create = false }
                                },
                                Dependencies = new List<DependencyDto>(),
                                Parents = new List<ParentDto>
                                {
                                    new ParentDto { ScreenID = "13", PermissionType = "Create" }
                                },
                                ViewOptions = new ViewOptionsDto { AllCompanies = false, Custom = false }
                            },
                            new ScreenDto
                            {
                                ScreenID = 13,
                                ScreenName = "Company List",
                                Permissions = new List<PermissionDto>
                                {
                                    new PermissionDto { ViewOnly = false, Edit = false, Approve = false, Create = false }
                                },
                                Checkbox = new List<PermissionDto>
                                {
                                    new PermissionDto { ViewOnly = true, Edit = true, Approve = false, Create = false }
                                },
                                Dependencies = new List<DependencyDto>(),
                                Parents = new List<ParentDto>
                                {
                                    new ParentDto { ScreenID = "12", PermissionType = "Create" }
                                },
                                ViewOptions = new ViewOptionsDto { AllCompanies = false, Custom = false }
                            }
                        }
                    },
                    new UserRightDto
                    {
                        RoleID = 1,
                        ModuleName = "Module Management",
                        Screens = new List<ScreenDto>
                        {
                            new ScreenDto
                            {
                                ScreenID = 2,
                                ScreenName = "Module List",
                                Permissions = new List<PermissionDto>
                                {
                                    new PermissionDto { ViewOnly = false, Edit = false, Approve = false, Create = false }
                                },
                                Checkbox = new List<PermissionDto>
                                {
                                    new PermissionDto { ViewOnly = true, Edit = true, Approve = false, Create = false }
                                },
                                Dependencies = new List<DependencyDto>(),
                                Parents = new List<ParentDto>
                                {
                                    new ParentDto { ScreenID = "2", PermissionType = "Create" }
                                },
                                ViewOptions = null
                            },
                            new ScreenDto
                            {
                                ScreenID = 3,
                                ScreenName = "Module Structure",
                                Permissions = new List<PermissionDto>
                                {
                                    new PermissionDto { ViewOnly = false, Edit = false, Approve = false, Create = false }
                                },
                                Checkbox = new List<PermissionDto>
                                {
                                    new PermissionDto { ViewOnly = true, Edit = false, Approve = false, Create = false }
                                },
                                Dependencies = new List<DependencyDto>(),
                                Parents = new List<ParentDto>(),
                                ViewOptions = null
                            }
                        }
                    }
                }
            }
        };

    }
}
