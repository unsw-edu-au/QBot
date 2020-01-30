using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Teams.Apps.QBot.Data
{

    [DbConfigurationType(typeof(EntitiesConfig))]
    partial class QBotEntities
    {

        public QBotEntities(string connectionString) : base(connectionString)
        {
        }
    }

    public class EntitiesConfig : DbConfiguration
    {
        public EntitiesConfig()
        {
            SetProviderServices("System.Data.EntityClient",
            SqlProviderServices.Instance);
            SetDefaultConnectionFactory(new SqlConnectionFactory());
        }
    }
}
