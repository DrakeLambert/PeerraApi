namespace DrakeLambert.Peerra.WebApi.Web.Entities
{
    public class Skill
    {
        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                _name = value?.ToLower();
            }
        }

        public Skill(string name)
        {
            Name = name;
        }
    }
}
