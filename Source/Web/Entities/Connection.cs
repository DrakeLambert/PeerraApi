namespace DrakeLambert.Peerra.WebApi.Web.Entities
{
    public class Connection
    {
        public string RequestorUsername { get; set; }

        public string TargetUsername { get; set; }

        public string Message { get; set; }

        public bool Accepted { get; set; }

        public bool Declined { get; set; }

        public string Status
        {
            get
            {
                if (Accepted)
                {
                    return "Accepted";
                }
                if (Declined)
                {
                    return "Declined";
                }
                return "Pending";
            }
        }
    }
}
