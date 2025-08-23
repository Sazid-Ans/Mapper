using Microsoft.AspNetCore.Identity;

namespace DataTypeMapping.Model.Customs
{
    public class IdentityOperationResult
    {
        public IdentityResult IdentityResult { get; }
        public bool Ok { get; }

        private IdentityOperationResult(IdentityResult identityResult , bool ok)
        {
            IdentityResult = identityResult;
            Ok = ok;
        }

        //factory methods to keep the B.L clean
        public static IdentityOperationResult Success() 
        {
            return new IdentityOperationResult(IdentityResult.Success , true);
        }

        public static IdentityOperationResult Failed(IdentityResult identityResult)
        {
          return new IdentityOperationResult(identityResult, false);
        }

        public static IdentityOperationResult Ambiguous()
        {
            return new IdentityOperationResult (IdentityResult.Success , false);
        }
    }
}
