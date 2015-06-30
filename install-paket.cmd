del z:\code\structuremap\src\AspNetHarness\ripple.dependencies.config
del z:\code\structuremap\src\FubuMVC.StructureMap3\ripple.dependencies.config
del z:\code\structuremap\src\FubuMVC.StructureMap3.Testing\ripple.dependencies.config
del z:\code\structuremap\src\HTML\ripple.dependencies.config
del z:\code\structuremap\src\NestedLibrary\ripple.dependencies.config
del z:\code\structuremap\src\StructureMap\ripple.dependencies.config
del z:\code\structuremap\src\StructureMap.AutoFactory\ripple.dependencies.config
del z:\code\structuremap\src\StructureMap.AutoMocking\ripple.dependencies.config
del z:\code\structuremap\src\StructureMap.AutoMocking.Moq\ripple.dependencies.config
del z:\code\structuremap\src\StructureMap.AutoMocking.Moq.Testing\ripple.dependencies.config
del z:\code\structuremap\src\StructureMap.AutoMocking.Testing\ripple.dependencies.config
del z:\code\structuremap\src\StructureMap.Configuration.Xml\ripple.dependencies.config
del z:\code\structuremap\src\StructureMap.LegacyAttributeSupport\ripple.dependencies.config
del z:\code\structuremap\src\StructureMap.LegacyAttributeSupport.Testing\ripple.dependencies.config
del z:\code\structuremap\src\StructureMap.Net4\ripple.dependencies.config
del z:\code\structuremap\src\StructureMap.Testing\ripple.dependencies.config
del z:\code\structuremap\src\StructureMap.Testing.GenericWidgets\ripple.dependencies.config
del z:\code\structuremap\src\StructureMap.Testing.Widget\ripple.dependencies.config
del z:\code\structuremap\src\StructureMap.Testing.Widget2\ripple.dependencies.config
del z:\code\structuremap\src\StructureMap.Testing.Widget3\ripple.dependencies.config
del z:\code\structuremap\src\StructureMap.Testing.Widget4\ripple.dependencies.config
del z:\code\structuremap\src\StructureMap.Testing.Widget5\ripple.dependencies.config
del z:\code\structuremap\src\StructureMap.Web\ripple.dependencies.config
del z:\code\structuremap\src\StructureMap.Web.Testing\ripple.dependencies.config
del z:\code\structuremap\src\StructureMap.Xml.Testing\ripple.dependencies.config
del z:\code\structuremap\ripple.config

paket install
paket add nuget Bottles project AspNetHarness --hard
paket add nuget FubuCore project AspNetHarness --hard
paket add nuget FubuLocalization project AspNetHarness --hard
paket add nuget FubuMVC.Core project AspNetHarness --hard
paket add nuget HtmlTags project AspNetHarness --hard
paket add nuget Bottles project FubuMVC.StructureMap3 --hard
paket add nuget FubuCore project FubuMVC.StructureMap3 --hard
paket add nuget FubuLocalization project FubuMVC.StructureMap3 --hard
paket add nuget FubuMVC.Core project FubuMVC.StructureMap3 --hard
paket add nuget HtmlTags project FubuMVC.StructureMap3 --hard
paket add nuget Bottles project FubuMVC.StructureMap3.Testing --hard
paket add nuget FubuCore project FubuMVC.StructureMap3.Testing --hard
paket add nuget FubuLocalization project FubuMVC.StructureMap3.Testing --hard
paket add nuget FubuMVC.Core project FubuMVC.StructureMap3.Testing --hard
paket add nuget HtmlTags project FubuMVC.StructureMap3.Testing --hard
paket add nuget NUnit project FubuMVC.StructureMap3.Testing --hard
paket add nuget RhinoMocks project FubuMVC.StructureMap3.Testing --hard
paket add nuget NUnit project HTML --hard
paket add nuget RhinoMocks project HTML --hard
paket add nuget Castle.Core project StructureMap.AutoFactory --hard
paket add nuget RhinoMocks project StructureMap.AutoMocking --hard
paket add nuget Moq project StructureMap.AutoMocking.Moq --hard
paket add nuget Moq project StructureMap.AutoMocking.Moq.Testing --hard
paket add nuget NUnit project StructureMap.AutoMocking.Moq.Testing --hard
paket add nuget NUnit project StructureMap.AutoMocking.Testing --hard
paket add nuget RhinoMocks project StructureMap.AutoMocking.Testing --hard
paket add nuget NUnit project StructureMap.LegacyAttributeSupport.Testing --hard
paket add nuget RhinoMocks project StructureMap.LegacyAttributeSupport.Testing --hard
paket add nuget Castle.Core project StructureMap.Testing --hard
paket add nuget NUnit project StructureMap.Testing --hard
paket add nuget RhinoMocks project StructureMap.Testing --hard
paket add nuget NUnit project StructureMap.Web.Testing --hard
paket add nuget NUnit project StructureMap.Xml.Testing --hard
