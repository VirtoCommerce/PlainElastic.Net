@set NUGET="packages\NuGet.CommandLine.2.8.3\tools\NuGet.exe"

@echo ==========================
@echo NuGet package publishing.
@%NUGET% Push NuGet\VirtoCommerce.PlainElastic.Net.1.1.56-beta.nupkg
@if not errorlevel 0 goto errors

@echo PlainElastic.Net publishing sucessfull.
@goto end

:error
@echo Error occured during PlainElastic.Net publishing.
@pause

:end
@pause
