// <auto-generated />
namespace gzWeb.Migrations
{
    using System.CodeDom.Compiler;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    using System.Resources;
    
    [GeneratedCode("EntityFramework.Migrations", "6.1.3-40302")]
    public sealed partial class RemoveUniquefromPlatformCustomerIdandmakeitnullable : IMigrationMetadata
    {
        private readonly ResourceManager Resources = new ResourceManager(typeof(RemoveUniquefromPlatformCustomerIdandmakeitnullable));
        
        string IMigrationMetadata.Id
        {
            get { return "201604141437311_Remove Unique from PlatformCustomerId and make it nullable"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return Resources.GetString("Target"); }
        }
    }
}