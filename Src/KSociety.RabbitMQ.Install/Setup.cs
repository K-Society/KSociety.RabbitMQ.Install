using KSociety.Base.InstallAction;
using System;
using WixSharp;
using WixSharp.Bootstrapper;

namespace KSociety.RabbitMQ.Install
{
    internal static class Setup
    {
        private const string ErlangVersion = "25.1.2";
        private const string RabbitMqVersion = "3.11.2";

        private const string Product = "RabbitMQ";
        private const string Manufacturer = "K-Society";
        private static string _installSystemVersion = "1.0.0.0";

        public static void Main()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            _installSystemVersion = fileVersionInfo.FileVersion;

            var productMsiUninstall = BuildMsiUninstall();
            var productMsiRabbitMqConf = BuildMsiRabbitMqConf();
            var productMsiHandle = BuildMsiHandle();
            var productMsiErlangX86 = BuildMsiErlangX86();
            var productMsiErlangX64 = BuildMsiErlangX64();
            var productMsiRabbitMq = BuildMsiRabbitMq();
            var productMsiRegistryX86 = BuildMsiRegistryX86();
            var productMsiRegistryX64 = BuildMsiRegistryX64();

            var bootstrapper =
                new Bundle("RabbitMQ System",
                    new MsiPackage(productMsiUninstall)
                    {
                        DisplayInternalUI = false,
                        Compressed = true,
                        Visible = false,
                        Cache = false,
                        MsiProperties = "UNINSTALLER_PATH=[UNINSTALLER_PATH]"
                    },
                    new MsiPackage(productMsiRabbitMqConf) { DisplayInternalUI = false, Compressed = true },
                    new ExePackage(@".\Files\Otp\otp_win64_" + ErlangVersion + ".exe")
                    {
                        Name = "Erlang OTP 25 (x64) - " + ErlangVersion,
                        InstallCommand = "/S",
                        Permanent = true,
                        Vital = true,
                        InstallCondition = "VersionNT64",
                        Compressed = true,
                        PerMachine = true,
                        Cache = false
                    },
                    new ExePackage(@".\Files\Otp\otp_win32_" + ErlangVersion + ".exe")
                    {
                        Name = "Erlang OTP 25 (x86) - " + ErlangVersion,
                        InstallCommand = "/S",
                        Permanent = true,
                        Vital = true,
                        InstallCondition = "NOT VersionNT64",
                        Compressed = true,
                        PerMachine = true,
                        Cache = false
                    },
                    new MsiPackage(productMsiHandle) { DisplayInternalUI = false, Compressed = true, Visible = false },
                    new MsiPackage(productMsiErlangX86) { DisplayInternalUI = false, Compressed = true, Visible = false, InstallCondition = "NOT VersionNT64" },
                    new MsiPackage(productMsiErlangX64) { DisplayInternalUI = false, Compressed = true, Visible = false, InstallCondition = "VersionNT64" },
                    new MsiPackage(productMsiRabbitMq) { DisplayInternalUI = false, Compressed = true, Visible = false },
                    new MsiPackage(productMsiRegistryX86) { DisplayInternalUI = false, Compressed = true, InstallCondition = "NOT VersionNT64" },
                    new MsiPackage(productMsiRegistryX64) { DisplayInternalUI = false, Compressed = true, InstallCondition = "VersionNT64" }
                )
                {
                    UpgradeCode = new Guid("A81A42A6-1AA5-4EA3-A80A-4AAD40DA255C"),
                    Version = new Version(_installSystemVersion),
                    Manufacturer = "K-Society",
                    AboutUrl = "https://github.com/K-Society/KSociety.RabbitMQ.Install",
                    Variables = new[]
                    {
                        new Variable("UNINSTALLER_PATH",
                            $@"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\{"Package Cache"}\{"[WixBundleProviderKey]"}\{Manufacturer + "." + Product}.exe")
                    }
                };

            bootstrapper.Build(Manufacturer + "." + Product + ".exe");

            if (System.IO.File.Exists(productMsiUninstall)) { System.IO.File.Delete(productMsiUninstall); }
            if (System.IO.File.Exists(productMsiRabbitMqConf)) { System.IO.File.Delete(productMsiRabbitMqConf); }
            if (System.IO.File.Exists(productMsiHandle)) { System.IO.File.Delete(productMsiHandle); }
            if (System.IO.File.Exists(productMsiErlangX86)) { System.IO.File.Delete(productMsiErlangX86); }
            if (System.IO.File.Exists(productMsiErlangX64)) { System.IO.File.Delete(productMsiErlangX64); }
            if (System.IO.File.Exists(productMsiRabbitMq)) { System.IO.File.Delete(productMsiRabbitMq); }
            if (System.IO.File.Exists(productMsiRegistryX86)) { System.IO.File.Delete(productMsiRegistryX86); }
            if (System.IO.File.Exists(productMsiRegistryX64)) { System.IO.File.Delete(productMsiRegistryX64); }

        }// Main.

        private static string BuildMsiUninstall()
        {
            var project =
                new Project("RabbitMQ Uninstall",
                    new Dir(new Id("PROGRAMMENUUNINSTALL"), @"%ProgramMenu%\K-Society\RabbitMQ",
                        new ExeFileShortcut("Uninstall RabbitMQ", "[UNINSTALLER_PATH]", "")
                    ) //Shortcut
                )
                {
                    InstallScope = InstallScope.perMachine,
                    Version = new Version("1.0.0.0"),
                    GUID = new Guid("EA84AF06-A621-4C13-A2FD-A6FDF3D99846"),
                    UI = WUI.WixUI_ProgressOnly,
                    ControlPanelInfo = new ProductInfo
                    {
                        Manufacturer = "K-Society"
                    }
                };

            return project.BuildMsi("RabbitMQ Uninstall.msi");
        }

        private static string BuildMsiRabbitMqConf()
        {
            var commonApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            Environment.SetEnvironmentVariable("RabbitMQConf", @"Conf");
            Environment.SetEnvironmentVariable("ProgramData", commonApplicationData);

            #region [Feature]

            Feature binaries = new Feature("Binaries", "Binaries", true, false);
            Feature rabbitMqConf = new Feature("RabbitMQConf", "RabbitMQ Configuration files", true);

            binaries.Children.Add(rabbitMqConf);

            #endregion
            
            var project =
                new Project("RabbitMQConf",
                    new Dir(new Id("PROGRAMDATA"), @"%ProgramData%\RabbitMQ",
                        new File(rabbitMqConf, @"%RabbitMQConf%\definitions.json"),
                        new File(rabbitMqConf, @"%RabbitMQConf%\enabled_plugins"),
                        new File(rabbitMqConf, @"%RabbitMQConf%\rabbitmq.conf",
                            new EnvironmentVariable(rabbitMqConf, "RABBITMQ_CONFIG_FILE", @"%RabbitMQConf%\rabbitmq.conf")
                            ),
                        new EnvironmentVariable(rabbitMqConf, "RABBITMQ_BASE", @"%ProgramData%\RabbitMQ")
                        {
                            System = true
                        },
                        new Dir(new Id("CONFD"), @"conf.d",
                        new EnvironmentVariable(rabbitMqConf, "RABBITMQ_CONFIG_FILES", @"%ProgramData%\RabbitMQ\conf.d")
                        {
                            System = true
                        })
                    ) // PROGRAMDATA.
                )
                {
                    InstallScope = InstallScope.perMachine,
                    Version = new Version("1.0.0.0"),
                    GUID = new Guid("ED2AC15D-FFCB-4931-AC42-593539E282CA"),
                    UI = WUI.WixUI_ProgressOnly,
                    ControlPanelInfo = new ProductInfo
                    {
                        Manufacturer = "K-Society"
                    }
                };

            return project.BuildMsi();
        }

        private static string BuildMsiHandle()
        {
            Environment.SetEnvironmentVariable("Handle", @"Files\Handle");

            #region [Feature]

            Feature binaries = new Feature("Binaries", "Binaries", true, false);
            Feature handle = new Feature("Handle", "Handle", true);

            binaries.Children.Add(handle);
            #endregion

            var project =
                new Project("Handle",
                    new Dir(new Id("INSTALLDIR"), @"%ProgramFiles%\Handle",
                        new File(handle, @"%Handle%\handle.exe"),
                        new File(handle, @"%Handle%\handle64.exe"),
                        new File(handle, @"%Handle%\handle64a.exe"),

                        new EnvironmentVariable(handle, "Path", @"%ProgramFiles%\Handle\")
                        {
                            System = true,
                            Part = EnvVarPart.last
                        }
                    ) // INSTALLDIR.
                )
                {
                    InstallScope = InstallScope.perMachine,
                    Platform = Platform.x86,
                    Version = new Version("1.0.0.0"),
                    GUID = new Guid("2B1F03C6-9798-45E0-9190-944171A7A0DA"),
                    UI = WUI.WixUI_ProgressOnly,
                    ControlPanelInfo = new ProductInfo
                    {
                        Manufacturer = "K-Society"
                    }
                };

            return project.BuildMsi();
        }

        private static string BuildMsiErlangX86()
        {
            Compiler.AutoGeneration.InstallDirDefaultId = null;
            #region [Feature]

            Feature binaries = new Feature("Binaries", "Binaries", true, false);
            Feature erlang = new Feature("Erlang", "Erlang V" + ErlangVersion, true);

            binaries.Children.Add(erlang);

            #endregion

            var project =
                new Project("Erlang " + ErlangVersion + " X86",
                    new EnvironmentVariable(erlang, "ERLANG_HOME", @"C:\Program Files\Erlang OTP")
                    {
                        System = true
                    }
                )
                {
                    InstallScope = InstallScope.perMachine,
                    Platform = Platform.x86,
                    Version = new Version("1.0.0.0"),
                    GUID = new Guid("F22E6F9B-2FE8-43DF-A2B0-582F4025B1DD"),
                    UI = WUI.WixUI_ProgressOnly,
                    ControlPanelInfo = new ProductInfo
                    {
                        Manufacturer = "K-Society"
                    }
                };

            return project.BuildMsi();
        }

        private static string BuildMsiErlangX64()
        {
            Compiler.AutoGeneration.InstallDirDefaultId = null;
            #region [Feature]

            Feature binaries = new Feature("Binaries", "Binaries", true, false);
            Feature erlang = new Feature("Erlang", "Erlang V" + ErlangVersion, true);

            binaries.Children.Add(erlang);

            #endregion

            var project =
                new Project("Erlang " + ErlangVersion + " X64",
                    new EnvironmentVariable(erlang, "ERLANG_HOME", @"C:\Program Files\Erlang OTP")
                    {
                        System = true
                    }
                )
                {
                    InstallScope = InstallScope.perMachine,
                    Platform = Platform.x64,
                    Version = new Version("1.0.0.0"),
                    GUID = new Guid("5FC2D1AC-28D2-4C68-A7BB-8DB68CBA8FD8"),
                    UI = WUI.WixUI_ProgressOnly,
                    ControlPanelInfo = new ProductInfo
                    {
                        Manufacturer = "K-Society"
                    }
                };

            return project.BuildMsi();
        }

        private static string BuildMsiRabbitMq()
        {
            Environment.SetEnvironmentVariable("RabbitMQ", @"Files" + @"\rabbitmq_server-" + RabbitMqVersion);
            Environment.SetEnvironmentVariable("RabbitMQConf", @"Conf");

            #region [Feature]

            Feature binaries = new Feature("Binaries", "Binaries", true, false);
            Feature rabbitMq = new Feature("RabbitMQ", "RabbitMQ V" + RabbitMqVersion, true);

            binaries.Children.Add(rabbitMq);

            #endregion

            var project =
                new Project("RabbitMQ " + RabbitMqVersion,
                    new FirewallException(new Id("RABBITMQFW"))
                    {
                        Name = "RabbitMq",
                        Port = "5672",
                        Scope = FirewallExceptionScope.any,
                        Profile = FirewallExceptionProfile.all,
                        Protocol = FirewallExceptionProtocol.tcp
                    },
                    new FirewallException(new Id("RABBITMQWEBFW"))
                    {
                        Name = "RabbitMqWeb",
                        Port = "15672",
                        Scope = FirewallExceptionScope.any,
                        Profile = FirewallExceptionProfile.all,
                        Protocol = FirewallExceptionProtocol.tcp
                    },
                    new Dir(new Id("INSTALLDIR"), @"%ProgramFiles%\RabbitMQ Server",
                        new Dir(new Id("RABBITMQINSTALLDIR"), rabbitMq, "rabbitmq_server-" + RabbitMqVersion,
                            new Files(rabbitMq, @"%RabbitMQ%\*.*"))
                    ) // INSTALLDIR.
                )
                {
                    InstallScope = InstallScope.perMachine,
                    Platform = Platform.x64,
                    Version = new Version("1.0.0.0"),
                    GUID = new Guid("D7D238D5-F79A-4C9E-BEBC-D4FA6D01E601"),
                    UI = WUI.WixUI_ProgressOnly,
                    ControlPanelInfo = new ProductInfo
                    {
                        Manufacturer = "K-Society"
                    },

                    Actions = new WixSharp.Action[]
                    {
                        new ElevatedManagedAction(ExecutionManager.ExecuteCommand, Return.check, When.After, Step.InstallFiles, Condition.NOT_Installed)
                        {
                            UsesProperties = @"ExeName=[RABBITMQINSTALLDIR]sbin\rabbitmq-service.bat, Args=install",
                            ActionAssembly = typeof(ExecutionManager).Assembly.Location
                        },
                        //new ElevatedManagedAction(ExecutionManager.ExecuteCommand, Return.check, When.After, Step.InstallFiles, Condition.NOT_Installed)
                        //{
                        //    UsesProperties = @"ExeName=[RABBITMQINSTALLDIR]sbin\rabbitmq-service.bat, Args=start",
                        //    ActionAssembly = typeof(ExecutionManager).Assembly.Location
                        //},
                        new ElevatedManagedAction(ExecutionManager.ExecuteCommand, Return.check, When.Before, Step.RemoveFiles, Condition.BeingUninstalled)
                        {
                            UsesProperties = @"ExeName=[RABBITMQINSTALLDIR]sbin\rabbitmq-service.bat, Args=remove",
                            ActionAssembly = typeof(ExecutionManager).Assembly.Location
                        }
                    }
                };

            return project.BuildMsi();
        }

        private static string BuildMsiRegistryX86()
        {
            var registry = new Feature("RegistryX86");
            var project =
                new Project("RabbitMQ System RegistryX86",
                    new RegKey(registry, RegistryHive.LocalMachine, @"SOFTWARE\" + Manufacturer + @"\" + Product,

                        new RegValue("Version", _installSystemVersion)),

                    new RemoveRegistryValue(registry, @"SOFTWARE\" + Manufacturer + @"\" + Product)
                )
                {
                    InstallScope = InstallScope.perMachine,
                    Version = new Version("1.0.0.0"),
                    GUID = new Guid("7D8DC46D-34BF-48C2-998E-8DEBCE3F1DAF"),
                    UI = WUI.WixUI_ProgressOnly,
                    ControlPanelInfo = new ProductInfo
                    {
                        Manufacturer = "K-Society"
                    }
                };

            return project.BuildMsi();
        }// BuildMsiRegistryX86.

        private static string BuildMsiRegistryX64()
        {
            var registry = new Feature("RegistryX64");
            var project =
                new Project("RabbitMQ System RegistryX64",
                    new RegKey(registry, RegistryHive.LocalMachine, @"SOFTWARE\" + Manufacturer + @"\" + Product,

                        new RegValue("Version", _installSystemVersion))
                    {
                        Win64 = true
                    },

                    new RemoveRegistryValue(registry, @"SOFTWARE\" + Manufacturer + @"\" + Product)
                )
                {
                    InstallScope = InstallScope.perMachine,
                    Platform = Platform.x64,
                    Version = new Version("1.0.0.0"),
                    GUID = new Guid("5845B649-7068-4BC3-BD81-67E9E8651B0B"),
                    UI = WUI.WixUI_ProgressOnly,
                    ControlPanelInfo = new ProductInfo
                    {
                        Manufacturer = "K-Society"
                    }
                };

            return project.BuildMsi();
        }// BuildMsiRegistryX64.
    }
}

//
// The solution that worked for me was to edit the registry key to enable long path behaviour, setting the value to 1. This is a new opt-in feature for Windows 10
// HKLM\SYSTEM\CurrentControlSet\Control\FileSystem LongPathsEnabled(Type: REG_DWORD)
// I got this solution from a named section of the article that @james-hill posted.
//     https://learn.microsoft.com/windows/desktop/FileIO/naming-a-file#maximum-path-length-limitation
// 
