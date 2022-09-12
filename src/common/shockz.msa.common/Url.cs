namespace shockz.msa.common
{
  public class Url
  {
    public const string Api_Gateway = "http://localhost:5300";
    public const string Identity_Server = "https://localhost:7072";
    public const string Movies_Api = "http://localhost:5256";
    public const string Movies_Client = "https://localhost:7216";

    public const string Sign_In = Movies_Client + "/signin-oidc";
    public const string Sign_Out = Movies_Client + "/signout-callback-oidc";

    public const string Movies = "/movies";
    public const string Movies_Id = "/movies/{0}";
  }
}
