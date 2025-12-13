namespace Ae.Poc.Identity.Security;

public static class AppPolicies
{
    public const string AdminOnly = "AdminOnly";
    public const string HRManagerOnly = "HRManagerOnly";
    public const string MustBelongToHRDepartment = "MustBelongToHRDepartment";
}
