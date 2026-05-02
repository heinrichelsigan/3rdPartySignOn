using System.Security.Claims;

namespace ThirdPartySignOn.Saml.Data
{
    
    /// <summary>
    /// ClaimReduced represents a reduced saml2 claim
    /// </summary>
    public class SamlClaimReduced
    {

        #region properties

        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public string ClaimIssuer { get; set; }
        public string ClaimSchema { get; set; }


        private static readonly SamlClaimReduced _emptyClaim = new SamlClaimReduced();
        public static SamlClaimReduced EmptyClaim { get => _emptyClaim; }

        #endregion properties


        #region ctors 

        public SamlClaimReduced() 
        { 
            ClaimType = string.Empty;
            ClaimValue = string.Empty;
            ClaimIssuer = string.Empty;
            ClaimSchema = string.Empty;
        }

        public SamlClaimReduced(string claimType, string claimValue, string claimIssuer, string claimSchema) : this()
        {
            this.ClaimType = claimType;
            this.ClaimValue = claimValue;
            this.ClaimIssuer = claimIssuer;
            this.ClaimSchema = claimSchema;
        }


        public SamlClaimReduced(Claim claim) : this()
        {
            if (claim != null)
            {
                this.ClaimType = claim.Type;
                this.ClaimValue = claim.Value;
                this.ClaimIssuer = claim.Issuer;
                this.ClaimSchema = claim.ValueType;
            }
        }

        public SamlClaimReduced(SamlClaimReduced sClaim) : this()
        {
            if (sClaim != null)
            {
                this.ClaimType = sClaim.ClaimType;
                this.ClaimValue = sClaim.ClaimValue;
                this.ClaimIssuer = sClaim.ClaimIssuer;
                this.ClaimSchema = sClaim.ClaimSchema;
            }
        }

        #endregion ctors         

    }

}
