namespace MathInter.Source
{
    public class AwsRds
    {
        string AwsAccessKeyId { get; set; }
        string AwsSecretAccessKey { get; set; }
        string RegionEndpoint { get; set; }
        string DbInstanceIdentifier { get; set; }
        string DbInstanceClass { get; set; }
        string Engine { get; set; }
        string MasterUsername { get; set; }
        string MasterPassword { get; set; }
        string DbName { get; set; }

        public AwsRds(string awsAccessKeyId, string awsSecretAccessKey, string regionEndpoint, string dbInstanceIdentifier, 
            string dbInstanceClass, string engine, string masterUsername, string masterPassword, string dbName)
        {
            AwsAccessKeyId = awsAccessKeyId;
            AwsSecretAccessKey = awsSecretAccessKey;
            RegionEndpoint = regionEndpoint;
            DbInstanceIdentifier = dbInstanceIdentifier;
            DbInstanceClass = dbInstanceClass;
            Engine = engine;
            MasterUsername = masterUsername;
            MasterPassword = masterPassword;
            DbName = dbName;
        }
    }
}