using System.Security.Claims;

namespace Saml2AuthGateway.Data
{
    
    /// <summary>
    /// ClaimReduced represents a reduced saml2 claim
    /// </summary>
    public class Saml2ClaimReduced
    {

        #region properties

        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public string ClaimIssuer { get; set; }
        public string ClaimSchema { get; set; }


        private static readonly Saml2ClaimReduced _emptyClaim = new Saml2ClaimReduced();
        public static Saml2ClaimReduced EmptyClaim { get => _emptyClaim; }

        #endregion properties


        #region ctors }

        public Saml2ClaimReduced() 
        { 
            ClaimType = string.Empty;
            ClaimValue = string.Empty;
            ClaimIssuer = string.Empty;
            ClaimSchema = string.Empty;
        }

        public Saml2ClaimReduced(string claimType, string claimValue, string claimIssuer, string claimSchema) : this()
        {
            this.ClaimType = claimType;
            this.ClaimValue = claimValue;
            this.ClaimIssuer = claimIssuer;
            this.ClaimSchema = claimSchema;
        }


        public Saml2ClaimReduced(Claim claim) : this()
        {
            if (claim != null)
            {
                this.ClaimType = claim.Type;
                this.ClaimValue = claim.Value;
                this.ClaimIssuer = claim.Issuer;
                this.ClaimSchema = claim.ValueType;
            }
        }

        public Saml2ClaimReduced(Saml2ClaimReduced sClaim) : this()
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
