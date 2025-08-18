using Microsoft.AspNetCore.Identity;

namespace DataTypeMapping.Model.Customs
{
    public class IdentityOperationResult
    {
        public IdentityResult IdentityResult { get; }
        public bool WasCreated { get; }

        private IdentityOperationResult(IdentityResult identityResult , bool wasCreated)
        {
            IdentityResult = identityResult;
            WasCreated = wasCreated;
        }

        //factory methods to keep the B.L clean
        public static IdentityOperationResult Created() 
        {
            return new IdentityOperationResult(IdentityResult.Success , true);
        }

        public static IdentityOperationResult Failed(IdentityResult identityResult)
        {
          return new IdentityOperationResult(identityResult , false);
        }

        public static IdentityOperationResult AlreadyExists()
        {
            return new IdentityOperationResult (IdentityResult.Success , false);
        }

    }
}
