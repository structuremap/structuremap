namespace StructureMap.Diagnostics
{
    public enum DoctorResult
    {
        BootstrapperFailure,
        BootstrapperCouldNotBeFound,
        Success,
        ConfigurationErrors,
        ValidationErrors,
        BuildErrors
    }
}